##' This function assigns the cluster to each data point and summarizes the results
##' @export
##' @param filename is full filename of the signature matrix of interest
##'
##' @param clusterDefs is output from the createClusters function
##'
##' @param diststats contains the means and sd for each variable
##'
##' @return cluster assignment information for all minutes in the day of data
##'
##' @author Brett Amidan
##'
##' @examples
##' assignClusters_SP()

assignClusters_SP <- function(filename,clusterDefs,diststats,SC=FALSE) {
  
  require(bpaFUN)
  
  
  try(load(filename),silent=TRUE)
  
  if (is.element("csum",ls())) {
    indna <- rowSums(!is.na(csum)) == 0
    csum <- csum[!indna, ]
    indna <- rowSums(!is.na(sig)) == 0
    sig <- sig[!indna, ]
    numclusts <- nrow(clusterDefs$centers)
    vars <- unlist(strsplit(colnames(clusterDefs$centers),split = ".", fixed = TRUE))
    ind <- is.element(vars, c("mean", "sd"))
    vars <- unique(vars[!ind])
    if (SC==FALSE) {
      # signatures
      x <- totalVar(the.data=csum,variableTypes=vars)
      x <- cbind(csum[,paste(vars,".mean",sep="")],x)
      
      out <- apply(x,MARGIN=1,distFun,the.matrix=clusterDefs$centers[,colnames(x)],
                   diststats=diststats)
      clusters <- out["Cluster", ]
      distances <- out["Dist", ]
      try(save(list = c("csum", "sig", "clusters","dqFilter","dqFlag"), file = filename),
          silent = TRUE)
      out <- cbind(clusters, distances)
      colnames(out) <- c("Cluster", "Distance")
    } else {
      # Spectral coherence signatures
      x = csum
      
      out <- apply(x,MARGIN=1,distFun,the.matrix=clusterDefs$centers[,colnames(x)],
                   diststats=diststats)
      clusters <- out["Cluster", ]
      distances <- out["Dist", ]
      try(save(list = c("csum", "sig", "clusters","SCdelay"), file = filename),
          silent = TRUE)
      out <- cbind(clusters, distances)
      colnames(out) <- c("Cluster", "Distance")
    }
  } else {
    out <- NULL
  }
  out
  
} # ends function