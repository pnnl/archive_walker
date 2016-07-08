##' This function explores all the atypicalities
##' @export
##' @param timeVec is a vector of the times (minutes)
##'
##' @return Plots and rationale
##'
##' @author Brett Amidan
##'
##' @examples
##' exploreAtyp()

exploreAtyp <- function(sigPath,outputPath,rdataPath,atypCutoff,
  baselineOutput,analysisTypes,phasors,ref.angle,ref.angle.label,pe,numjobs,SC=FALSE) {

  require(snowfall)
  require(bpaFUN)
  
  ###### get list of all times with GAS > atypCutoff
  load(file=paste(sigPath,"/gasMatrix.Rdata",sep=""))
  if (length(atypCutoff)==1) atypCutoff <- rep(atypCutoff,ncol(gasMatrix))

  tempMat <- matrix(atypCutoff,nrow=nrow(gasMatrix),ncol=length(atypCutoff),
    byrow=TRUE)
  indy <- gasMatrix > tempMat & !is.na(gasMatrix)
  rs <- rowSums(indy)>0
  atypTimes <- rownames(gasMatrix)[rs]
  ## based on last 2 months
#  load(file=paste(sigPath,"/gasMatrix_2M.Rdata",sep=""))
#  indna <- !is.na(gasMatrix)
#  indkeep <- rowSums(indna)>0
#  gasMatrix <- gasMatrix[indkeep,]
  ## only keep last 2 months
#  cutval <- as.POSIXct(rownames(gasMatrix)[nrow(gasMatrix)])-60*60*24*61
#  indy2 <- as.POSIXct(rownames(gasMatrix)) > cutval
#  gasMatrix <- gasMatrix[indy2,]
#  indy <- apply(gasMatrix,MARGIN=1,max,na.rm=TRUE) > atypCutoff
#  atypTimes <- c(atypTimes,rownames(gasMatrix)[indy])
  ##
  atypTimes <- sort(unique(atypTimes))
  ## only keep first time (don't report if within 5 minutes of an atypicality)
  if (length(atypTimes)>1) atypTimes <- keepFirst(timeVec=atypTimes)

  ## create an atypicality file
  varset <- gas <- NULL
  for (i in 1:length(atypTimes)) {
    ind <- gasMatrix[atypTimes[i],] > atypCutoff
    varset <- c(varset,paste(colnames(gasMatrix)[ind],collapse=" / "))
    gas <- c(gas,paste(round(gasMatrix[atypTimes[i],ind],2),collapse=" / "))
  }
  atypSum <- data.frame(atypTimes,varset,gas)
  colnames(atypSum) <- c("Time","VariableSet","AtypScore")
  ## write to disk
  write.csv(atypSum,paste(outputPath,"/AtypicalityTable.csv",sep=""),
    quote=FALSE,sep=",",row.names=FALSE,col.names=TRUE)

#################  Multiprocessing doesn't work too well due to RAM
  ### Prepare for multiprocessing
#  sfInit(parallel=TRUE,cpus=min(numjobs,3))

  ## run the single processor assignClusters function multiprocessed
#  output <- sfLapply(as.list(atypTimes),exploreAtyp_SP,rdataPath=rdataPath,
#    sigPath=sigPath,outputPath=outputPath,baselineOutput=baselineOutput,
#    atypCutoff=atypCutoff,analysisTypes=analysisTypes,phasors=phasors,pe=pe)

#  sfStop()

#  for (atypTime in atypTimes[130:456]) {
  for (atypTime in atypTimes) {
    print(atypTime)
    if (is.null(phasors)) { # PDAT
    exploreAtyp_SP2(atypTime=atypTime,rdataPath=rdataPath,sigPath=sigPath,
    outputPath=outputPath,baselineOutput=baselineOutput,atypCutoff=atypCutoff,
    analysisTypes=analysisTypes,ref.angle=ref.angle,
    ref.angle.label=ref.angle.label,pe=pe,SC=SC)

    } else { # DST
    exploreAtyp_SP(atypTime=atypTime,rdataPath=rdataPath,sigPath=sigPath,
    outputPath=outputPath,baselineOutput=baselineOutput,atypCutoff=atypCutoff,
    analysisTypes=analysisTypes,phasors=phasors,ref.angle=ref.angle,
    ref.angle.label=ref.angle.label,pe=pe)
    }
  }

  invisible()
  
} # ends function
