##' This function filters the data quality issues, leaving NAs
##' @export
##' @param Rdata.path
##'
##' @param DQ.path
##'
##' @param phasors is a data frame of the phasor info for the dataset
##'
##' @param delta.criteria
##'
##' @return DQ reports
##'
##' @author Brett Amidan
##'
##' @examples
##' Data.Quality()


DQfilter <- function(data,phasors=NULL) {
  
  CN = colnames(data)
  
  if (is.null(phasors)) {
    # PDAT
    
    # Get PMU names
    pmus = vector(mode = "character", length=length(CN))
    for (idx in 1:length(CN)) {
      pmus[idx] = unlist(strsplit(CN[idx],split=".",fixed=TRUE))[1]
    }
    
    
    # Get colids (.V, VA, .I, IA, and eq to match colids for DST files)
    colids = vector(mode = "character", length=length(CN))
    for (idx in 1:length(CN)) {
      temp = unlist(strsplit(CN[idx],split=".",fixed=TRUE))      # Break signal name apart at periods
      if (!is.na(temp[3])) {                                     # If the signal name has 3 parts (excludes analogs)
        if (temp[3] == "MAG") {                                  # If it is a magnitude signal
          temp = unlist(strsplit(temp[2],split="_",fixed=TRUE))  # Break the middle part of the signal name apart at underscores (not always necessary)
          temp = temp[length(temp)]                              # Grab the last part
          if (xor(grepl("V",temp),grepl("I",temp))) {            # Throw a warning if a V and an I appear in temp - can't tell if it's current or voltage or something else
            if (grepl("V",temp)) {                               # The signal is a voltage magnitude
              colids[idx] = '.V'
            } else {                                             # The signal is a current magnitude
              colids[idx] = '.I'
            }
          } else {
            warning('The data quality filter cannot determine if the signal with name ', CN[idx], ' is a voltage or current. It will not be filtered.')
          }
        } else if (temp[3] == "ANG") {                           # If it is an angle signal
          temp = unlist(strsplit(temp[2],split="_",fixed=TRUE))  # Break the middle part of the signal name apart at underscores (not always necessary)
          temp = temp[length(temp)]                              # Grab the last part
          if (xor(grepl("V",temp),grepl("I",temp))) {            # Throw a warning if a V and an I appear in temp - can't tell if it's current or voltage or something else
            if (grepl("V",temp)) {                               # The signal is a voltage angle
              colids[idx] = 'VA'
            } else {                                             # The signal is a current angle
              colids[idx] = 'IA'
            }
          } else {
            warning('The data quality filter cannot determine if the signal with name ', CN[idx], ' is a voltage or current. It will not be filtered.')
          }
        } else if (temp[3] == "freq") {                          # If it is a frequency signal
          colids[idx] = 'eq'
        }
      } else {
        if (temp[2] == "freq") colids[idx] = 'eq'
      }
    }
    
    
    # Set the limit for the PMU flags
    FlagLim = 4095
  } else {
    # DST
    pmus <- substring(CN,1,4)
    colids <- substring(CN,nchar(CN)-1)  
    FlagLim = 127
  }
  
  
  
  ### Create DQ output
  items <- c("n","flag","filter")
  dq <- matrix(0,nrow=length(items),ncol=ncol(data))
  rownames(dq) <- items
  colnames(dq) <- CN
  ## populate n
  dq["n",] <- colSums(!is.na(data))
  
  # number of NAs in each column to start with (useful later)
  StartNA = dim(data)[1] - dq["n",]
  
  #################################################
  #### Identify flags > FlagLim
  
  if (is.null(phasors)) {
    # PDAT
    ind <- grepl("stat",CN)
  } else {
    #DST
    ind <- grepl("flag",CN)
  }
  
  flags <- CN[ind]
  indy <- data[,ind] > FlagLim & !is.na(data[,ind])
  rowid <- rownames(data)[rowSums(indy)>0]
  if (length(rowid)>0) {
    for (j in 1:length(rowid)) {
      ind2 <- indy[rowid[j],]
      cols <- flags[ind2]
      
      if (is.null(phasors)) {
        # PDAT
        for (idx in (1:length(cols))) {
          cols[idx] = unlist(strsplit(cols[idx],split=".",fixed=TRUE))[1]
        }
      } else {
        #DST
        cols = substring(cols,1,4)
      }
      
      ind3 <- is.element(pmus,cols)
      data[rowid[j],ind3] <- NA
    } # ends j
  } # ends if
  # Number of NAs due to flags = total number of NAs - number that were there from the start
  dq["flag",] <- colSums(is.na(data)) - StartNA
  
  
  #################################################
  #### Remove data signals whose names don't specify a reference voltage
  if (is.null(phasors)) {
    # PDAT
    KillCols = !(grepl("34",CN) | grepl("230",CN) | grepl("500",CN))
    data[,CN[KillCols]] = NA
    colids[KillCols] = "dead"
  }
  
  
  #################################################
  #### Exceed maxs or mins for Voltage
  ind <- colids==".V"
  inda <- colids=="VA"
  
  ## grab the voltage references
  if (is.null(phasors)) {
    # PDAT
    vref34idx = grepl("34",CN)[ind==TRUE]
    vref230idx = grepl("230",CN)[ind==TRUE]
    vref500idx = grepl("500",CN)[ind==TRUE]
    
    voltref = rep(0,sum(ind))
    voltref[vref34idx==TRUE] = 34
    voltref[vref230idx==TRUE] = 230
    voltref[vref500idx==TRUE] = 500
  } else {
    #DST
    indy <- match(colnames(data[,ind]),paste(phasors[,"VarName"],".V",sep=""))
    voltref <- phasors[indy,"VoltageRef"]
  }
  ## create the max/min limits (0.75 to 1.25)
  voltref.maxlim <- matrix(voltref*1.25,nrow=nrow(data),ncol=sum(ind),byrow=TRUE)
  colnames(voltref.maxlim) <- CN[ind]
  voltref.minlim <- matrix(voltref*0.75,nrow=nrow(data),ncol=sum(ind),byrow=TRUE)
  colnames(voltref.minlim) <- CN[ind]
  
  
  
  ### TRUE where data<0.75*ref
  indLow3 <- data[,ind] < voltref.minlim & !is.na(data[,ind])
  
  ### TRUE where data> 1.25*ref
  indHigh3 <- data[,ind] > voltref.maxlim & !is.na(data[,ind])
  
  # True where data<0.75*ref OR data>1.25*ref
  ind3 = indLow3 | indHigh3
  
  # Where more than 1/3 are high OR low, NA the whole variable
  rsum <- colSums(ind3) > (nrow(data)/3)
  data[,ind][,rsum] <- NA
  
  ## Need to NA all associated angles
  if (sum(rsum)>0) {
    if (is.null(phasors)) {
      # PDAT
      rsum2 = gsub("MAG","ANG",CN[ind][rsum])
    } else {
      # DST
      rsum2 <- paste(CN[ind][rsum],"A",sep="")
    }
    ind22 <- is.element(rsum2,colnames(data))
    data[,rsum2[ind22]] <- NA
  }
  
  
  ## NA individual points below limit
  ind3 <- data[,ind] < voltref.minlim & !is.na(data[,ind])
  data[,ind][ind3] <- NA
  data[,inda][ind3] <- NA  ## NA corresponding angles
  
  
  ## NA individual points above limit
  ind3 <- data[,ind] > voltref.maxlim & !is.na(data[,ind])
  data[,ind][ind3] <- NA
  data[,inda][ind3] <- NA  ## NA corresponding angles
  
  ###################################################
  #### Exceed maxs or mins for Current
  ind <- colids==".I"
  inda <- colids=="IA"
  
  ## TRUE where data<0.1
  indLow3 <- data[,ind] < 0.1 & !is.na(data[,ind])
  
  ## TRUE where data>3
  indHigh3 <- data[,ind] > 3 & !is.na(data[,ind])
  
  # True where data<0.1 OR data>3
  ind3 = indLow3 | indHigh3
  
  # Where more than 1/3 are high OR low, NA the whole variable
  rsum <- colSums(ind3) > (nrow(data)/3)
  data[,ind][,rsum] <- NA
  ## Need to NA all associated angles
  if (sum(rsum)>0) {
    if (is.null(phasors)) {
      # PDAT
      rsum2 = gsub("MAG","ANG",CN[ind][rsum])
    } else {
      # DST
      rsum2 <- paste(CN[ind][rsum],"A",sep="")
    }
    ind22 <- is.element(rsum2,colnames(data))
    data[,rsum2[ind22]] <- NA
  }
  
  
  ## NA individual points below 0.1
  ind3 <- data[,ind] < 0.1 & !is.na(data[,ind])
  data[,ind][ind3] <- NA
  ## NA corresponding angles
  data[,inda][ind3] <- NA
  
  
  ## NA individual points above 3
  ind3 <- data[,ind] > 3 & !is.na(data[,ind])
  data[,ind][ind3] <- NA
  ## NA corresponding angles
  data[,inda][ind3] <- NA
  
  ####################################################
  #### Exceed maxs or mins for Frequency
  ind <- colids=="eq"
  
  ## TRUE where data<59.5
  indLow3 <- data[,ind] < 59.5 & !is.na(data[,ind])
  
  ## TRUE where data>60.5
  indHigh3 <- data[,ind] > 60.5 & !is.na(data[,ind])
  
  # True where data<59.5 OR data>60.5
  ind3 = indLow3 | indHigh3
  
  # Where more than 1/3 are high OR low, NA the whole variable
  rsum <- colSums(ind3) > (nrow(data)/3)
  data[,ind][,rsum] <- NA
  
  
  ## NA individual points below 59
  ind3 <- data[,ind] < 59 & !is.na(data[,ind])
  data[,ind][ind3] <- NA
  
  ## NA individual points above 61
  ind3 <- data[,ind] > 61 & !is.na(data[,ind])
  data[,ind][ind3] <- NA
  
  ### Remove any that have less than 5 unique values
  tempfun <- function(vec) length(unique(vec))
  ind4 <- apply(data[,ind],MARGIN=2,tempfun) <= 5
  data[,ind][,ind4] <- NA
  
  ####################################################
  #### NA whole PMU if most of it is bad (>.75)
  ind.na <- is.na(data)
  ind.na2 <- colSums(ind.na)>(nrow(data)/2)
  tab <- table(ind.na2,pmus)
  tab <- tab/colSums(tab)
  na.pmus <- colnames(tab)[tab["TRUE",] >= 0.75]
  ## NA whole PMU
  
#   pmus[idx] = unlist(strsplit(CN[idx],split=".",fixed=TRUE))[1]
#   pmus <- substring(CN,1,4)
  
  ind.na3 <- is.element(pmus,na.pmus)
  data[,ind.na3] <- NA
  
  # Number of NAs due to filter = total number of NAs - number from flag - number that were there from the start
  dq["filter",] <- colSums(is.na(data)) - dq["flag",] - StartNA
  
  list(data=data,dq=dq)
  
} # ends function

