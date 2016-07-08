##'  This function calculates clusters / atypicality scores real time
##' @export
##' @param sigPath  is path where signatures will be stored
##'
##' @param outputPath is the path where atypical output will be stored
##'
##' @return 
##'
##' @author Brett Amidan
##'
##' @examples
##' rtCalculations()

rtCalculations <- function(sigPath,outputPath,rdataPath,atypCutoff,
  analysisTypes,baselineOutput,ref.angle,ref.angle.label,pe,SC=FALSE) {

  ##### Determine the new data by looking at the gas matrix and the signature
  ##### matrices
  load(paste(sigPath,"/gasMatrix.Rdata",sep=""))
  lastMin <- (rownames(gasMatrix)[nrow(gasMatrix)])
  lastDay <- as.POSIXct(substring(lastMin,1,10))
  if (substring(lastMin,12,16)=="23:59") {
    lastMin <- as.POSIXct(lastMin)+59
  } else {
    lastMin <- as.POSIXct(lastMin)
  }
  ## look for signature matrices on this date or past
  sigListing <- substring(dir(sigPath,pattern="20"),1,10)
  indy <- as.POSIXct(sigListing) >= lastDay
  
  if (sum(indy)>0) {
    ### load cluster output
    load(paste(sigPath,"/clusterOutput.RData",sep=""))
    clusterDefs <- vector(mode="list",length=1)
    names(clusterDefs) <- "centers"
    clusterDefs$centers <- clusterOutput$centers
    
    ### Loop thru each Signature matrix and update
    for (i in 1:sum(indy)) {
      ##### recalculate the clusters
      temp <- assignClusters_SP(filename=paste(sigPath,"/",sigListing[indy][i],".Rdata",sep=""),
        clusterDefs=clusterDefs,diststats=clusterOutput$distStats,SC=SC)
      ##### Determine GAS for new data
      ## get the signature matrix
      load(paste(sigPath,"/",sigListing[indy][i],".Rdata",sep=""))
      ## determine part of signature matrix that is new
      ind2 <- as.POSIXct(rownames(sig)) > lastMin
      ## if there are 2 or more new minutes calculate the gas matrix
      if (sum(ind2)>1) {
        sig <- sig[ind2,]
        out <- sigGAS(sig=sig,clusterOutput=clusterOutput,
          baselineOutput=baselineOutput,clusters=clusters[rownames(sig)])
        ### determine if any are atypical
        if (length(atypCutoff)==1) atypCutoff <- rep(atypCutoff,ncol(out))
        tempMat <- matrix(atypCutoff,nrow=nrow(out),ncol=length(atypCutoff),
          byrow=TRUE)
        indy2 <- out > tempMat & !is.na(out)
        rs <- rowSums(indy2)>0
        atypTimes <- rownames(out)[rs]
        ### add to gasMatrix
        gasMatrix <- rbind(gasMatrix,out)
        ##### save the gas matrix back out
        save(list="gasMatrix",file=paste(sigPath,"/gasMatrix.Rdata",sep=""))

        if (length(atypTimes)>0) {
          ## prepare anomaly info for atypTimes
          if (length(atypTimes)>1) atypTimes <- keepFirst(timeVec=atypTimes)
          ## loop thru atypTimes
          for (atypTime in atypTimes) {
            print(atypTime)
            exploreAtyp_SP2(atypTime=atypTime,rdataPath=rdataPath,
              sigPath=sigPath,outputPath=outputPath,
              baselineOutput=baselineOutput,atypCutoff=atypCutoff,
              analysisTypes=analysisTypes,ref.angle=ref.angle,
              ref.angle.label=ref.angle.label,pe=pe,SC=SC)
          } # ends atypTime loop
        } # ends if
      } # ends if
    } # ends i loop
    
  } # ends if
  
  invisible()
  
} # ends function
