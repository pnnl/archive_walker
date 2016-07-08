##' This function updates the baseline for a given cluster
##' @export
##' @param filename is full filename of the signature matrix of interest
##'
##' @param clusterDefs is output from the createClusters function
##'
##' @param diststats contains the means and sd for each variable
##'
##' @param method is "C" if mean/sd determined from cluster data or "U" if from universal calculation
##'
##' @return cluster assignment information for all minutes in the day of data
##'
##' @author Brett Amidan
##'
##' @examples
##' assignClusters_SP()

updateBaseline_SP <- function(clusterVal,clusterOutput,analysisTypes,sigPath,
  daysToUse=730,random=FALSE,method="C",SC=FALSE) {

  require(bpaFUN)

  ### create a list of signature elements
  sigElements <- NULL
  for (i in 1:length(analysisTypes)) {
    sigElements <- c(sigElements,analysisTypes[[i]]$signatureTypes)
  }
  sigElements <- unique(sigElements)
  
  ###########  Sample data from the cluster of interest  ###########
  ## find the days that have the cluster of interest
  ind <- is.element(clusterOutput$membership,clusterVal) &
    !is.na(clusterOutput$membership)
  days <- names(clusterOutput$membership)[ind]
  if (!random) {
    ## Only keep <100,000 points / randomly select days until over 100000
    if (length(days)>100000) {
      daytab <- table(substring(days,1,10))
      dayselect <- sample(names(daytab),size=length(daytab))
      daytab2 <- cumsum(daytab[dayselect])
      numkeep <- sum(daytab2<100000)+1
      days <- sort(names(daytab2)[1:numkeep])
    } else {
      days <- unique(substring(days,1,10))
    }
    ## make sure days are during daysToUse time frame
    lastday <- as.POSIXct(substring(names(clusterOutput$membership)[length(clusterOutput$membership)],
      1,10))
    startday <- lastday - daysToUse*60*24*60
    keepdays <- substring(as.character(seq(startday,lastday,by=60*24*60)),1,10)
    indb <- is.element(days,keepdays)
    days <- days[indb]
  } else {
    ## random selection of days
    days <- unique(substring(days,1,10))
    probs <- c(1:length(days))
    if (length(days) > daysToUse) 
      days <- sort(sample(unique(days),size=daysToUse,prob=probs/sum(probs)))
  }
  
  ## set up sig matrix
  bigsig <- NULL
  for (i in 1:length(days)) {
    ## load signature matrix for the day
    try(load(paste(sigPath,"/",days[i],".Rdata",sep="")),silent=TRUE)
    if (is.element("sig",ls())) {
      ## trim the variables by signature element
      ind1 <- rep(FALSE,ncol(sig))
      for (j in 1:length(sigElements)) {
        ind1 <- ind1 | grepl(sigElements[j],colnames(sig),fixed=TRUE)
      } # ends j
      sig <- sig[,ind1]

      ## trim only the minutes from the cluster
      ind2 <- is.element(rownames(sig),names(clusterOutput$membership)[ind])
      if (sum(ind2)==1) {
        temp <- matrix(sig[ind2,],nrow=1)
        colnames(temp) <- colnames(sig)
        rownames(temp) <- rownames(sig)[ind2]
        sig <- temp
      } else if (sum(ind2)>1) {
        sig <- sig[ind2,]
      }
      ## add to bigsig
      if (length(bigsig)==0) {
        bigsig <- sig
      } else {
        ## check for columns in bigsig that aren't in sig
        indy3 <- !is.element(colnames(bigsig),colnames(sig))
        if (sum(indy3) > 0) {
          ## add to sig
          add.mat <- matrix(NA,nrow=nrow(sig),ncol=sum(indy3))
          rownames(add.mat) <- rownames(sig)
          colnames(add.mat) <- colnames(bigsig)[indy3]
          sig <- cbind(sig,add.mat)
        }
        ## check for columns that don't exist yet, and then add them
        indy2 <- !is.element(colnames(sig),colnames(bigsig))
        if (sum(indy2) > 0) {
          ## add to bigsig
          add.mat <- matrix(NA,nrow=nrow(bigsig),ncol=sum(indy2))
          rownames(add.mat) <- rownames(bigsig)
          colnames(add.mat) <- colnames(sig)[indy2]
          bigsig <- cbind(bigsig,add.mat)
        }
        if (nrow(sig)==1) {
          temp <- matrix(sig[,colnames(bigsig)],nrow=1)
          rownames(temp) <- rownames(sig)
          colnames(temp) <- colnames(bigsig)
          sig <- temp
        } else {
          sig <- sig[,colnames(bigsig)]
        }
        bigsig <- rbind(bigsig,sig)
      } # if/else
      rm("sig")
    } # ends if
    gc(reset=TRUE)
    #print(i)
  } # ends i loop
    
  ##### Setup output
  output <- vector(mode="list",length=length(analysisTypes))
  names(output) <- names(analysisTypes)
  gasMatrix <- matrix(NA,nrow=nrow(bigsig),ncol=length(analysisTypes))
  colnames(gasMatrix) <- names(analysisTypes)
  rownames(gasMatrix) <- rownames(bigsig)
  
  ##### loop thru each analysis type
  for (i in 1:length(analysisTypes)) {
  
    #### trim data according to the analysis type
    ## get requested variable types
    if(!SC) {
      # Brett's
      indy1 <- rep(FALSE,ncol(bigsig))
      for (j in 1:length(analysisTypes[[i]]$variableTypes)) {
        indy1 <- indy1 | grepl(paste(".",analysisTypes[[i]]$variableTypes[j],
                                     "~",sep=""),colnames(bigsig))
      } # ends j loop
    } else {
      # Spectral Coherence
      indy1 <- rep(FALSE,ncol(bigsig))
      for (j in 1:length(analysisTypes[[i]]$variableTypes)) {
        indy1 <- indy1 | grepl(paste(".",analysisTypes[[i]]$variableTypes[j],sep=""),colnames(bigsig))
      } # ends j loop
    }
    
    ## get requested signature types
    indy2 <- rep(FALSE,ncol(bigsig))
    for (j in 1:length(analysisTypes[[i]]$signatureTypes)) {
      indy2 <- indy2 | grepl(analysisTypes[[i]]$signatureTypes[j],
        colnames(bigsig))
    } # ends j loop
    indy <- indy1 & indy2
    tempsig <- bigsig[,indy]
#    ## remove any columns with >75% NAs
#    indy <- colSums(is.na(tempsig))/nrow(tempsig)<.75
#    tempsig <- tempsig[,indy]
    
    #### trim data to a complete dataset (no NAs)
    tempsig2 <- trimNAs(the.data=tempsig)
    
    if (nrow(tempsig2)>5 & ncol(tempsig2)>1) {
      #### calculate raw mean / sd for each column of data
      if (method=="U") {
        clusterMeans <- clusterOutput$sigStats["mean",colnames(tempsig2)]
        clusterSDs <- clusterOutput$sigStats["sd",colnames(tempsig2)]
      } else {
        clusterMeans <- apply(tempsig[,colnames(tempsig2)],MARGIN=2,mean,na.rm=TRUE)
        clusterSDs <- apply(tempsig[,colnames(tempsig2)],MARGIN=2,sd,na.rm=TRUE)
      }
      #### calculate PCA loadings
      pcaOut <- mypca(the.data=tempsig2,colmeans=clusterMeans,colsds=clusterSDs)

      #### calculate mahalanobis distance using the PCA scores
      mdOut <- mahaDist(the.data=tempsig,colmeans=clusterMeans,colsds=clusterSDs,
        pcaOut=pcaOut)

      #### fit distances to gamma and calculate rate and shape
      gammaParams <- gammaDist(dataVec=mdOut)

      #### calculate GAS score for each data point in cluster
      ## calculate cluster proportion
      indna <- !is.na(clusterOutput$membership)
      clusterProp <- sum(is.element(clusterOutput$membership[indna],clusterVal)) / sum(indna)
      tempscores <- calcGAS(dataVec=mdOut,gammaParams=gammaParams,
        clusterProp=clusterProp)
      gasMatrix[names(tempscores),i] <- tempscores

      #### store pca loadings, raw mean & sd, gamma parameters
      ####    for the given analysisType
      output[[i]] <- list(pcaOut=pcaOut,clusterMeans=clusterMeans,
        clusterSDs=clusterSDs, gammaParams=gammaParams)
    }
  } ## ends i loop

  list(baseline=output,gasMatrix=gasMatrix)
  
} # ends function
