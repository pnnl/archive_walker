##' This function calculates summary statistics for clustering
##' @export
##' @param data is the data
##'
##' @return summaries to be used in clustering
##'
##' @author Brett Amidan
##'
##' @examples
##' sum4clusters()

sum4clusters <- function(data5,phasors) {

  require(bpaFUN)
  #######################################
  ### Summarize Frequency

  indy <- grepl(".freq",colnames(data5))
  ## calculate mean
  if (sum(indy)>0) {
  meanfreq <- mean(data5[,indy],na.rm=TRUE)
  } else {
  meanfreq <- NA
  }
  ## calculate within frequency st dev
  if (sum(indy)>1) {
  withinfreq <- mean(apply(data5[,indy],MARGIN=2,sd,na.rm=TRUE))
  ## calculate between frequency st dev
  betweenfreq <- sd(apply(data5[,indy],MARGIN=2,mean,na.rm=TRUE))
  } else {
    withinfreq <- betweenfreq <- NA
  }
  
  ####################################################
  ### Summarize Phase Angle (1st order difference)
  
  indy <- is.element(substring(colnames(data5),nchar(colnames(data5))-1),"DA")
  if (sum(indy)>1) {
  ## calculate mean
  meanDA <- mean(data5[,indy],na.rm=TRUE)
  ## calculate within angle diff st dev
  withinDA <- mean(apply(data5[,indy],MARGIN=2,sd,na.rm=TRUE))
  ## calculate between angle diff st dev
  betweenDA <- sd(apply(data5[,indy],MARGIN=2,mean,na.rm=TRUE))
  } else {
    meanDA <- withinDA <- betweenDA <- NA
  }
  ####################################################
  ### Summarize Voltage
  indy <- is.element(substring(colnames(data5),nchar(colnames(data5))-1),".V")
  if (sum(indy)>1) {
  temp <- data5[,indy]
  ind.match <- match(substring(colnames(temp),1,9),
    as.character(phasors[,"VarName"]))
  voltref <- phasors[ind.match,"VoltageRef"]
  denom <- matrix(voltref,nrow=nrow(temp),ncol=ncol(temp),byrow=TRUE)
  temp <- temp / denom
  ## calculate mean
  meanV <- mean(temp,na.rm=TRUE)
  ## calculate within voltage st dev
  withinV <- mean(apply(temp,MARGIN=2,sd,na.rm=TRUE))
  ## calculate between voltage st dev
  betweenV <- sd(apply(temp,MARGIN=2,mean,na.rm=TRUE))
  } else {
    meanV <- withinV <- betweenV <- NA
  }

  ####################################################
  ### Summarize Current

  indy <- is.element(substring(colnames(data5),nchar(colnames(data5))-1),".I")
  if (sum(indy)>1) {
  ## calculate mean
  meanI <- mean(data5[,indy],na.rm=TRUE)
  ## calculate within Current st dev
  withinI <- mean(apply(data5[,indy],MARGIN=2,sd,na.rm=TRUE))
  ## calculate between Current st dev
  betweenI <- sd(apply(data5[,indy],MARGIN=2,mean,na.rm=TRUE))
  } else {
    meanI <- withinI <- betweenI <- NA
  }

  ## output
  out <- matrix(c(meanfreq,withinfreq,betweenfreq,meanDA,withinDA,betweenDA,meanV,
    withinV,betweenV,meanI,withinI,betweenI),nrow=1)
  colnames(out) <- c("FREQ.mean","FREQ.wsd","FREQ.bsd","DA.mean","DA.wsd",
    "DA.bsd","V.mean","V.wsd","V.bsd","I.mean","I.wsd","I.bsd")
  rownames(out) <- substring(rownames(data5)[1],1,16)
  
  out

} # ends function
