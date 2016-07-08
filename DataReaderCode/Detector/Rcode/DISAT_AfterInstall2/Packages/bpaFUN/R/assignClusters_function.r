##' This function assigns the cluster to each data point and summarizes the results
##' @export
##' @param filesVec is a vector containing the full path of all signature files
##'
##' @param clusterDefs is output from the createClusters function
##'
##' @param diststats contains the means and sd for each variable
##'
##' @return list of distances data is from cluster and proportion of observations in each cluster
##'
##' @author Brett Amidan
##'
##' @examples
##' assignClusters()

assignClusters <- function(filesVec,clusterDefs,diststats,numjobs=1,SC=FALSE) {

  require(snowfall)
  require(bpaFUN)

  ## make the filesVec a list
  filesList <- as.list(filesVec)
  
  if (length(filesList) > 0) {
    ## create the summaries
    sfInit(parallel=TRUE,cpus=numjobs)

    ## run the single processor assignClusters function multiprocessed
    output <- sfLapply(filesList,assignClusters_SP,clusterDefs=clusterDefs,
      diststats=diststats,SC=SC)

    sfStop()
    
  } # ends if

  #### Create cluster vector and distance vector
  cluster <- NULL
  distance <- NULL
  for (i in 1:length(output)) {
    ## cluster
    cluster <- c(cluster,output[[i]][,"Cluster"])
    ## distance
    distance <- c(distance,output[[i]][,"Distance"])
  }
  
  list(cluster=cluster,distance=distance)
  
} # ends function
