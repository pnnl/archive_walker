##' This function uses multiple processors to calculate the mean and SD of specified variables over multiple data files
##' @export
##' @param FilesVec is a vector containing full path (pasted together) for the data files
##'
##' @param numjobs is the number of processors to be used by the function
##'
##' @param cols is a list of columns to be used
##'
##' @param variableTypes is a vector of the abbreviations of which variables to include in clustering default is c("FREQ","V")
##'
##' @return A matrix containing the mean and SD for each variable specified in variableTypes above.
##'
##' @author Scott K Cooley
##'
##' @examples
##' MultiProcMeanSDCalc()


MultiProcMeanSDCalc <- function(FilesVec,numjobs,cols,variableTypes,SC=FALSE) {

  ### I think I should source SummaryCalcs_SP.r or add it to Brett's library and require the library ###
  ### But it seems to wrok OK having sourced SummaryCalcs_SP.r in the wrapper script. ??? ###
  require(snowfall)
  require(bpaFUN)
  
  sfInit(parallel=TRUE,cpus=numjobs)

  #sfExport("SummaryCalcs_SP")

  Summary.Calcs.list <- sfLapply(as.list(FilesVec),SummaryCalcs_SP,
    cols=cols,variableTypes=variableTypes,SC=SC)

  sfStop()

  #Summary.Calcs.list

  #### Calculate the Cluster Summary Statistics and Signature Summary Statistics
  clustSum <- matrix(0,nrow=nrow(Summary.Calcs.list[[1]][[1]]),
    ncol=ncol(Summary.Calcs.list[[1]][[1]]))
  colnames(clustSum) <- colnames(Summary.Calcs.list[[1]][[1]])
  rownames(clustSum) <- rownames(Summary.Calcs.list[[1]][[1]])
  for (i in 1:length(Summary.Calcs.list)) {
    ## cluster summary
    clustSum <- clustSum + Summary.Calcs.list[[i]][["clusterSumMat"]]
    ## signature summary
    if (i==1) {
      sigSum <- Summary.Calcs.list[[i]][["sumMat"]]
    } else {
      indy <- !is.element(colnames(Summary.Calcs.list[[i]][["sumMat"]]),
        colnames(sigSum))
      if (sum(indy)>0) {
        ## add columns to sigSum
        addmat <- matrix(0,nrow=nrow(sigSum),ncol=sum(indy))
        rownames(addmat) <- rownames(sigSum)
        colnames(addmat) <- colnames(Summary.Calcs.list[[i]][["sumMat"]])[indy]
        sigSum <- cbind(sigSum,addmat)
      }
      sigSum[,colnames(Summary.Calcs.list[[i]][["sumMat"]])] <-
        sigSum[,colnames(Summary.Calcs.list[[i]][["sumMat"]])] +
        Summary.Calcs.list[[i]][["sumMat"]]
    } # ends if/else
  } # ends i
  
  #### Calculate Mean / SD
  
  ### Cluster
  clustStats <- rbind(clustSum["sum",]/clustSum["n",],
    sqrt((clustSum["SS",]-(clustSum["sum",]^2/clustSum["n",]))/(clustSum["n",]-1)))
  colnames(clustStats) <- colnames(clustSum)
  rownames(clustStats) <- c("mean","sd")
  
  ### signatures
  sigStats <- rbind(sigSum["Sum",]/sigSum["n",],
    sqrt((sigSum["SS",]-(sigSum["Sum",]^2/sigSum["n",]))/(sigSum["n",]-1)))
  colnames(sigStats) <- colnames(sigSum)
  rownames(sigStats) <- c("mean","sd")
  
  list(clust=clustStats,sig=sigStats)
}






