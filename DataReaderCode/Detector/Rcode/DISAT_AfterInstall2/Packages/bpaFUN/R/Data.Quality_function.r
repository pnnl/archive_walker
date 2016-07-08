##' This function evaluates the data quality
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


Data.Quality <- function(Rdata.path,DQ.path,phasors=phasors,delta.criteria=2) {

  require(bpaFUN)
  
  #start.time <- proc.time()[3]
  ## get a list of files for the day
  file.list <- dir(Rdata.path)

  ### Set up all the output
  tem1 <- unlist(strsplit(Rdata.path,split="/"))
  time.vec <- as.character(seq(from=as.POSIXct(paste(tem1[(length(tem1)-2)],"-",
    tem1[(length(tem1)-1)],"-",tem1[(length(tem1))]," 00:00",sep="")),
    length=1440,by=60))
    
  ## set up Data Quality Info log
  flag.dq <- min.dq <- max.dq <- delta.dq <- numna.dq <- zero.dq <- matrix(0,
    nrow=length(time.vec),ncol=0)
  rownames(flag.dq) <- rownames(min.dq) <- rownames(max.dq) <-
    rownames(numna.dq) <- rownames(delta.dq) <- rownames(zero.dq) <- time.vec

  ### loop thru each file
  for (i in file.list) {
#  start.time <- proc.time()[3]
#    print(i)
    ### load the data
    load(paste(Rdata.path,"/",i,sep=""))
    data <- data/10000
    colnames(data)<- toupper(colnames(data))

    ## identify the row in the output
    rowid <- paste("20",substring(i,6,7),"-",substring(i,8,9),"-",
      substring(i,10,11)," ",substring(i,12,13),":",substring(i,14,15),
      ":00",sep="")
      
    ## Add any columns needed for the output
    ind <- grepl(".FLAG",colnames(data),fixed=TRUE)
    poss.cols <- colnames(data)[!ind]
    ind2 <- !is.element(poss.cols,colnames(min.dq))
    if (sum(ind2)>0) {
      temp <- matrix(0,nrow=length(time.vec),ncol=sum(ind2))
      rownames(temp) <- time.vec
      colnames(temp) <- poss.cols[ind2]
      min.dq <- cbind(min.dq,temp)
      zero.dq <- cbind(zero.dq,temp)
      max.dq <- cbind(max.dq,temp)
      delta.dq <- cbind(delta.dq,temp)
      numna.dq <- cbind(numna.dq,temp)
    }
    ## do the same for the flags
    poss.cols <- colnames(data)[ind]
    ind2 <- !is.element(poss.cols,colnames(flag.dq))
    if (sum(ind2)>0) {
      temp <- matrix(0,nrow=length(time.vec),ncol=sum(ind2))
      rownames(temp) <- time.vec
      colnames(temp) <- poss.cols[ind2]
      flag.dq <- cbind(flag.dq,temp)
    }
    
    #################################################
    #### Identify flags > 127
    
    ind <- grepl("FLAG",colnames(data))
    flags <- colnames(data)[ind]
    indy <- data[,ind] > 127 & !is.na(data[,ind])
    cs <- colSums(indy)
    ind2 <- cs > 0
    
    if (sum(ind2)>0) {
      for (j in 1:sum(ind2)) {
        flag.dq[rowid,names(cs)[ind2][j]] <- cs[ind2][j]
      } # ends i
    } # ends if
    
    ################################################
    #### Calculate the number of NAs for each column
    
    ## trim flags off data
    data <- data[,!ind]
    
    ind.na <- colSums(is.na(data))
    ind3 <- ind.na > 0
    if (sum(ind3)>0) {
      for (j in 1:sum(ind3)) {
        numna.dq[rowid,names(ind.na)[ind3][j]] <- ind.na[ind3][j]
      } # ends i
    } # ends if
    
    #################################################
    #### Exceed maxs or mins for Voltage
    ind4 <- phasors[,"Type"]=="V"
    voltages <- paste(phasors[ind4,"VarName"],".V",sep="")
    ind5 <- is.element(colnames(data),voltages)

    ## grab the voltage references
    indy <- match(colnames(data[,ind5]),paste(phasors[,"VarName"],".V",sep=""))
    voltref <- phasors[indy,"VoltageRef"]
    ## create the max/min limits (0.75 to 1.25)
    voltref.maxlim <- matrix(voltref*1.25,nrow=nrow(data),ncol=sum(ind5),byrow=TRUE)
    colnames(voltref.maxlim) <- colnames(data)[ind5]
    voltref.minlim <- matrix(voltref*0.75,nrow=nrow(data),ncol=sum(ind5),byrow=TRUE)
    colnames(voltref.minlim) <- colnames(data)[ind5]

    ## determine if < min
    ind.min <- data[,ind5] < voltref.minlim
    cs <- colSums(ind.min)
    ind6 <- cs > 0
    if (sum(ind6)>0) {
      for (j in 1:sum(ind6)) {
        min.dq[rowid,names(cs)[ind6][j]] <- cs[ind6][j]
      }
    }

    ## determine if > max
    ind.max <- data[,ind5] > voltref.maxlim
    cs <- colSums(ind.max)
    ind6 <- cs > 0
    if (sum(ind6)>0) {
      for (j in 1:sum(ind6)) {
        max.dq[rowid,names(cs)[ind6][j]] <- cs[ind6][j]
      }
    }

    ###################################################
    #### Exceed maxs or mins for Current
    ind4 <- phasors[,"Type"]=="I"
    currents <- paste(phasors[ind4,"VarName"],".I",sep="")
    ind5 <- is.element(colnames(data),currents)
    ## zeros (< 0.05)
    ind6 <- data[,ind5] < 0.05 &!is.na(data[,ind5])
    cs <- colSums(ind6)
    ind7 <- cs > 0
    if (sum(ind7) > 0)  {
      for (j in 1:sum(ind7)) {
        zero.dq[rowid,names(cs)[ind7][j]] <- cs[ind7][j]
      }
    }
    ## maximums ( > 3)
    ind6 <- data[,ind5] > 3 &!is.na(data[,ind5])
    cs <- colSums(ind6)
    ind7 <- cs > 0
    if (sum(ind7) > 0)  {
      for (j in 1:sum(ind7)) {
        max.dq[rowid,names(cs)[ind7][j]] <- cs[ind7][j]
      }
    }

    ####################################################
    #### Exceed maxs or mins for Frequency
    ind4 <- phasors[,"Type"]=="F"
    freqs <- phasors[ind4,"VarName"]
    ind5 <- is.element(colnames(data),freqs)
    ## minimums (< 59)
    ind6 <- data[,ind5] < 59 &!is.na(data[,ind5])
    cs <- colSums(ind6)
    ind7 <- cs > 0
    if (sum(ind7) > 0)  {
      for (j in 1:sum(ind7)) {
        min.dq[rowid,names(cs)[ind7][j]] <- cs[ind7][j]
      }
    }
    ## maximums (> 61)
    ind6 <- data[,ind5] > 61 &!is.na(data[,ind5])
    cs <- colSums(ind6)
    ind7 <- cs > 0
    if (sum(ind7) > 0)  {
      for (j in 1:sum(ind7)) {
        max.dq[rowid,names(cs)[ind7][j]] <- cs[ind7][j]
      }
    }

    ####################################################
    #### Exceed maxs or mins for Phase Angle
    ind4 <- phasors[,"Type"]=="V"
    angles <- paste(phasors[ind4,"VarName"],".VA",sep="")
    ind4b <- phasors[,"Type"]=="I"
    angles <- c(angles,paste(phasors[ind4,"VarName"],".IA",sep=""))
    ind5 <- is.element(colnames(data),angles)
    ## minimums (< -180)
    ind6 <- data[,ind5] < -180 &!is.na(data[,ind5])
    cs <- colSums(ind6)
    ind7 <- cs > 0
    if (sum(ind7) > 0)  {
      for (j in 1:sum(ind7)) {
        min.dq[rowid,names(cs)[ind7][j]] <- cs[ind7][j]
      }
    }
    ## maximums (> 180)
    ind6 <- data[,ind5] > 180 &!is.na(data[,ind5])
    cs <- colSums(ind6)
    ind7 <- cs > 0
    if (sum(ind7) > 0)  {
      for (j in 1:sum(ind7)) {
        max.dq[rowid,names(cs)[ind7][j]] <- cs[ind7][j]
      }
    }

    #####################################################
    ####### Phase Angle Delta
    delta.out <- DeltaCheck(data.mat=data[,ind5],delta=5,
      delta.criteria=20,constant=60)
    delta.dq[rowid,names(delta.out)] <- delta.out

    #####################################################
    ####### Frequency Delta
    ind5 <- is.element(colnames(data),freqs)
    delta.out <- DeltaCheck(data.mat=data[,ind5],delta=.1,
      delta.criteria=20,constant=600,angle=FALSE)
    delta.dq[rowid,names(delta.out)] <- delta.out

    #####################################################
    ####### Voltage Delta
    ind5 <- is.element(colnames(data),voltages)
    delta.out <- DeltaCheck(data.mat=data[,ind5],delta=5,
      delta.criteria=20,constant=60,angle=FALSE)
    delta.dq[rowid,names(delta.out)] <- delta.out

    #####################################################
    ####### Current Delta
    ind5 <- is.element(colnames(data),currents)
    delta.out <- DeltaCheck(data.mat=data[,ind5],delta=.1,
      delta.criteria=20,constant=1800,angle=FALSE)
    delta.dq[rowid,names(delta.out)] <- delta.out

#  print(paste(round((proc.time()[3]-start.time)/60,1)," minutes",sep=""))
  } # ends i loop

  
  ## output
  save(list=c("delta.dq","min.dq","max.dq","numna.dq","flag.dq","zero.dq"),
    file=paste(DQ.path,"/",gsub("-","",substring(time.vec[1],1,10),fixed=TRUE),
    ".Rdata",sep=""))
  
} # ends function

  