##' This function gets Data for performance envelopes
##' @export
##' @param atypTime
##'
##' @param rdataPath
##'
##' @param minutes is # of minutes to plot
##'
##' @return plots performance envelopes
##'
##' @author Brett Amidan
##'
##' @examples
##' plotPerfEnv()

getData <- function(atypTime,rdataPath,minutes=11) {

  require(bpaFUN)
  data(phasors)
  
  tpi <- as.character(seq(as.POSIXct(atypTime)-60*(floor((minutes-1)/2)),
    as.POSIXct(atypTime)+60*(floor((minutes-1)/2)),by=60))

  ### collect the middle data
  for (k in 1:length(tpi)) {
    tempPath <- paste(rdataPath,"/",substring(tpi[k],1,4),"/",
      substring(tpi[k],6,7),"/",substring(tpi[k],9,10),sep="")
    tempList <- dir(tempPath)
    ind5 <- grepl(paste(substring(tpi[k],3,4),substring(tpi[k],6,7),
      substring(tpi[k],9,10),substring(tpi[k],12,13),substring(tpi[k],15,16),
      sep=""),tempList)
    if (sum(ind5)==1) {
      load(paste(tempPath,"/",tempList[ind5],sep=""))
      data <- DQfilter(data/10000,phasors)
      ## remove flags
      indy <- grepl("flag",colnames(data))
      data <- data[,!indy]
      ## calculate angle differences
      indySA <- is.element("MLN1.NTHB.VA",colnames(data))
      indyMA <- is.element("MAR1.NTHB.VA",colnames(data))
      indyNA <- is.element("MONR.NTHB.VA",colnames(data))
      indy <- grepl(".VA",colnames(data),fixed=TRUE) |
        grepl(".IA",colnames(data),fixed=TRUE)
      if (indySA | indyMA | indyNA) {
        temp1 <- NULL
        if (indySA) {
          temp <- apply(data[,indy],MARGIN=2,CalcAngDiff,Ang2=data[,"MLN1.NTHB.VA"])
          colnames(temp) <- gsub(".VA",".SA",colnames(temp))
          colnames(temp) <- gsub(".IA",".SA",colnames(temp))
          temp1 <- cbind(temp1,temp)
        }
        if (indyMA) {
          temp <- apply(data[,indy],MARGIN=2,CalcAngDiff,Ang2=data[,"MAR1.NTHB.VA"])
          colnames(temp) <- gsub(".VA",".MA",colnames(temp))
          colnames(temp) <- gsub(".IA",".MA",colnames(temp))
          temp1 <- cbind(temp1,temp)
        }
        if (indyNA) {
          temp <- apply(data[,indy],MARGIN=2,CalcAngDiff,Ang2=data[,"MONR.NTHB.VA"])
          colnames(temp) <- gsub(".VA",".NA",colnames(temp))
          colnames(temp) <- gsub(".IA",".NA",colnames(temp))
          temp1 <- cbind(temp1,temp)
        }
        data <- cbind(data,temp1)
      } # ends if
      
      ## remove angles
      indy <- grepl(".VA",colnames(data),fixed=TRUE) | grepl(".IA",colnames(data),fixed=TRUE)
      data <- data[,!indy]
      
      ## if k=1 then setup output
      if (k == 1) {
        fulldata <- data
      } else {
        fulldata <- smartRbindMat(fulldata,data)
      }
    } # ends if
    gc(reset=TRUE)
  } # ends k

  fulldata
} # ends function
