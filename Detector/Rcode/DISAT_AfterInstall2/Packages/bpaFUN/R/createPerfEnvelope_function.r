##' This function calculates the performance envelopes for the plots
##' @export
##' @param sigPath is the path to the signature data
##'
##' @return performance envelope information for each month / variable
##'
##' @author Brett Amidan
##'
##' @examples
##' createPerfEnvelope()

createPerfEnvelope <- function(sigPath,baselineOutput,daysToUse=730,
                               startNew=TRUE,SC=FALSE) {
  
  siglisting <- dir(sigPath)
  siglisting2 <- dir(sigPath,pattern="20")
  
  ygroups <- 30
  ## look to see if perfEnvelope already exists
  if (is.element("perfEnvelope.Rdata",siglisting) & !startNew) {
    ## if it does, then load it
    load(paste(sigPath,"/perfEnvelope.Rdata",sep=""))
  } else {
    ## if doesn't exist, set up a new one
    ## use variable mean / stdev to set up limits
    ## do for each second
    pe <- vector(mode="list",length=13)
    names(pe) <- c(1:12,"lastDate")
    ### find the means and SDs for each variables
    
    # Original
    #     tempMeans <- baselineOutput[["baseline"]][["All.Mean"]][["clusterMeans"]]
    #     tempSDs <- baselineOutput[["baseline"]][["All.Mean"]][["clusterSDs"]]
    
    # Changed by Jim to accomodate Spectral Coherence
    tempMeans <- baselineOutput[["baseline"]][[1]][["clusterMeans"]]
    tempSDs <- baselineOutput[["baseline"]][[1]][["clusterSDs"]]
    
    ## set up count matrix - columns = each minute of day, rows=+-3 SD
    count.cn <- substring(as.character(seq(as.POSIXct("2010-01-01 00:00"),
                                           as.POSIXct("2010-01-01 23:59"),by=60)),12,16)
    temp.mat <- matrix(0,nrow=ygroups,ncol=length(count.cn))
    colnames(temp.mat) <- count.cn
    clouds <- vector(mode="list",
                     length=length(tempMeans))
    tempnames <- names(tempMeans)
    if(!SC) {
      # Brett's
      tempnames <- substring(tempnames,1,nchar(tempnames)-7)
    } else {
      # Spectral Coherence
      tempnames <- substring(tempnames,1,nchar(tempnames)-5)
    }
    names(clouds) <- tempnames
    ## loop thru each variable
    for (i in 1:length(tempMeans)) {
      ymin <- tempMeans[i] - 3*tempSDs[i]
      ymax <- tempMeans[i] + 3*tempSDs[i]
      ylimits <- seq(ymax,ymin,length=(ygroups+1))
      ymidpts <- signif(apply(cbind(ylimits[1:(length(ylimits)-1)],
                                    ylimits[2:length(ylimits)]),MARGIN=1,mean),5)
      count <- temp.mat
      rownames(count) <- ymidpts
      clouds[[i]] <- list(count=count,ylimits=ylimits,ymidpts=ymidpts)
    } # ends i
    pe$lastDate <- substring(siglisting2[1],1,10)
    for (i in 1:12) {
      pe[[i]] <- clouds
    }
  } # ends if/else
  
  ######## Determine which signature matrices to add to perf Env.
  ## trim to daysToUse
  lastday <- as.POSIXct(substring(siglisting2[length(siglisting2)],1,10))
  startday <- lastday - daysToUse*60*24*60
  ## only keep those that are after the "lastDate"
  if (startday < as.POSIXct(pe$lastDate)) {
    startday <- as.POSIXct(pe$lastDate)+(24*60*60)
  } 
  ## continue if startday < lastday
  if (startday < lastday) {
    possdays <- substring(as.character(seq(startday,lastday,by=60*24*60)),1,10)
    ind <- is.element(substring(siglisting2,1,10),possdays)
    siglisting2 <- siglisting2[ind]
    
    #### Loop thru each signature matrix left to process
    for (i in 1:length(siglisting2)) {
      ## read in signature matrix
      load(paste(sigPath,"/",siglisting2[i],sep=""))
      ## determine which variables exist
      if(!SC) {
        # Brett's
        indy <- is.element(colnames(sig),paste(names(pe[[1]]),"~a~mean",sep=""))
      } else {
        # Spectral Coherence
        indy <- is.element(colnames(sig),paste(names(pe[[1]]),".Valu",sep=""))
      }
      ## identify the month the data occurred
      listpos <- as.numeric(substring(siglisting2[i],6,7))
      
      if (sum(indy)>1) {
        xplace <- substring(rownames(sig),12,16)
        ## loop thru each variable
        for (j in 1:sum(indy)) {
          varname <- colnames(sig)[indy][j]
          if(!SC) {
            # Brett's
            varname2 <- substring(varname,1,nchar(varname)-7)
          } else {
            # Spectral Coherence
            varname2 <- substring(varname,1,nchar(varname)-5)
          }
          temp <- pe[[listpos]][[varname2]][["count"]]
          out <- ygroups + 1 - cut(sig[,varname],
                                   breaks=pe[[listpos]][[varname2]][["ylimits"]],labels=FALSE)
          for (k in 1:length(out)) {
            ##### add counts to the count matrices for each variable
            temp[out[k],xplace[k]] <- 1 + temp[out[k],xplace[k]]
          } # ends k
          pe[[listpos]][[varname2]][["count"]] <- temp
        } # ends j
      } # ends if
      #print(i)
    } # ends i loop
  }
  ## store last date processed
  pe$lastDate <- substring(siglisting2[length(siglisting2)],1,10)
  
  ## output
  pe
  
} # ends function
