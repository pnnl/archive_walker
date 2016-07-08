##' This function calculates gas based on the updated baseline
##' @export
##' @param the.data is the matrix of data
##'
##' @return most complete matrix without NAs
##'
##' @author Brett Amidan
##'
##' @examples
##' trimNAs()

updateGAS <- function(clusterOutput,baselineOutput,sigPath,
  daysToUse=730,numjobs=numjobs) {

  require(snowfall)
  
  #### Determine the list of signature files to calculate GAS for
  ## make sure days are during daysToUse time frame
  lastday <- as.POSIXct(substring(names(clusterOutput$membership)[length(clusterOutput$membership)],
    1,10))
  startday <- lastday - daysToUse*60*24*60
  possdays <- substring(as.character(seq(startday,lastday,by=60*24*60)),1,10)

  ## get signature data from possdays
  siglisting <- dir(sigPath,pattern="20")
  indy <- is.element(substring(siglisting,1,10),possdays)
  siglisting <- paste(sigPath,"/",siglisting[indy],sep="")
  
  ### Single Process Function
  single.process <- function(filename,clusterOutput,baselineOutput) {

    require(bpaFUN)
    
    ## load the data
    load(filename)
    # 
    ind5 <- is.element(rownames(sig),names(clusters))
    sig <- sig[ind5,]
    
    output <- NULL
    ## cluster info
    proptable <- table(clusterOutput$membership)/length(clusterOutput$membership)
    ## determine the cluster props for each data point
    ind <- match(as.character(clusters),names(proptable))
    clustprops <- proptable[ind]

    ## loop thru each analysis
    for (i in 1:length(baselineOutput)) {
      ## calculate the Mahalanobis distances
      mdout <- mahaDist(the.data=sig,
        colmeans=baselineOutput[[i]][["clusterMeans"]],
        colsds=baselineOutput[[i]][["clusterSDs"]],
        pcaOut=baselineOutput[[i]][["pcaOut"]])
      ## calculate the gas
      if (sum(!is.na(mdout))>1) {
      tempscores <- calcGAS(dataVec=mdout,
        gammaParams=baselineOutput[[i]]$gammaParams,clusterProp=clustprops)
      } else {
        tempscores <- rep(NA,length(mdout))
      }
      output <- cbind(output,tempscores)
    } # ends i
    colnames(output) <- names(baselineOutput)
    rownames(output) <- names(clusters)
    output
  } # ends function
  
  ### run all data thru multiprocess of "single.process"
  sfInit(parallel=TRUE,cpus=numjobs)

  ## run the single processor assignClusters function multiprocessed
  output2 <- sfLapply(as.list(siglisting),single.process,
    clusterOutput=clusterOutput,baselineOutput=baselineOutput)

  sfStop()

  ###  Combine all gas scores into one matrix
  gasMatrix <- NULL
  for (i in 1:length(output2)) {
    temp <- output2[[i]]
    gasMatrix <- rbind(gasMatrix,temp)
  } # ends i
  ## sort by time (rownames)
  ind.sort <- sort(rownames(gasMatrix))
  gasMatrix <- gasMatrix[ind.sort,,drop=FALSE]

  gasMatrix
} # ends function
