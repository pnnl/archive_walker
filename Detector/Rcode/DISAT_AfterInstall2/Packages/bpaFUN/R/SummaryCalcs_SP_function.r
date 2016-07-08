##' This function calculates n, sum, and sum of squares for each variable specified and for each data file of interest.  These summary statistics are later used to calculate mean and Sd for each variable over multiple data files.
##' @export
##' @param path.file.names is a vector containing path\filenames (pasted together) for the data files
##'
##' @param variableTypes is a vector of the abbreviations of which variables to include in clustering default is c("FREQ","V")
##'
##' @param cols is a list of columns to be used
##'
##' @return A matrix containing the summary statistics n, sum, and sum of squares for each variable specified by variableTypes.
##'
##' @author Scott K Cooley
##'
##' @examples
##' x <- SummaryCalcs_SP(path.file.name=FilesVec[1],variableTypes=variableTypes)

SummaryCalcs_SP <- function (path.file.name, cols, variableTypes,SC=FALSE) {
  
  require(bpaFUN)
  
  try(load(path.file.name), silent = TRUE)
  
  if (is.element("csum", ls())) {
    
    #### Calculate Cluster Statistics
    X0 <- csum[, cols]
    X1 <- X0[is.na(apply(X0, 1, sum)) == F, ]
    ind.mat <- matrix(as.numeric(sapply(colnames(X1), function(x) {
      sapply(variableTypes, function(y) {grep(y, x)})})),
      length(variableTypes), length(colnames(X1)))
    ind.mat[is.na(ind.mat)] <- 0
    X2 <- X1[, cols]
    
    if (SC==FALSE) {
      # Brett's signatures
      summary.stats.keep <- c(".mean",".sd")
      X3 <- NULL
      for (j in 1:length(variableTypes)) {
        mn.j <- X2[,paste(variableTypes[j],".mean",sep="")]
        tsd.j <- totalVar(the.data=X2,variableTypes=variableTypes[j])
        X3 <- cbind(X3,mn.j,tsd.j)
      }
      colnames(X3) <- paste(rep(variableTypes,each=length(summary.stats.keep)),
                            rep(summary.stats.keep,length(variableTypes)),sep="")
    } else {
      # Spectral coherence signatures
      X3 = X2
    }
    
    summary.calcs.mat <- matrix(NA, 3, ncol(X3))
    rownames(summary.calcs.mat) <- c("n", "sum", "SS")
    colnames(summary.calcs.mat) <- colnames(X3)
    summary.calcs.mat["n", ] <- rep(nrow(X3), ncol(X3))
    summary.calcs.mat["sum", ] <- apply(X3, 2, sum)
    summary.calcs.mat["SS", ] <- apply(X3, 2, function(x) {sum(x^2)})
    clusterSumMat <- summary.calcs.mat
    ind.na <- is.na(sig)
    sumMat <- matrix(NA, nrow = 3, ncol = ncol(sig))
    rownames(sumMat) <- c("n", "Sum", "SS")
    colnames(sumMat) <- colnames(sig)
    sumMat["n", ] <- apply(!ind.na, MARGIN = 2, sum, na.rm = TRUE)
    sumMat["Sum", ] <- apply(sig, MARGIN = 2, sum, na.rm = TRUE)
    sumMat["SS", ] <- apply(sig^2, MARGIN = 2, sum, na.rm = TRUE)
    
  } else {
    clusterSumMat <- NULL
    sumMat <- NULL
  }
  
  
  list(clusterSumMat = clusterSumMat, sumMat = sumMat)
  
}

