##' This function updates the cluster definitions and cluster assignments
##' @export
##' @param sigPath is the path to the signature data
##'
##' @param variableTypes is a vector of the abbreviations of which variables to include in clustering default is c("FREQ","V")
##'
##' @param cutPercentile is percentile to determine if data is too far from the cluster center; default is 0.99
##'
##' @return Cluster Output list which contains the cluster definitions, cluster membership proportions, cluster membership cutoff, & storymeister
##'
##' @author Brett Amidan
##'
##' @examples
##' updateClusters()

#sigPath <- "G:/Sigs"
#outputPath <- "//olympus/disat/Output"
#install.packages("bpaFUN", repos="http://martingale.pnl.gov/computing/repos")

updateClusters <- function(sigPath,outputPath,variableTypes=c("FREQ","V"),
  cutPercentile=0.9999,numjobs=1,SC=FALSE) {

  require(bpaFUN)
  timer <- proc.time()[3]
  
  #### get a list of all signature matrices in sigPath
  filelist <- dir(sigPath)
  ind <- substring(filelist,1,1)=="2"
  filelist <- filelist[ind]
  
  #### get a list of all columns that will be needed from each matrix
  ## look at first file to get list of columns
  load(paste(sigPath,"/",filelist[1],sep=""))
  cols <- colnames(csum)
  indy <- rep(FALSE,length(cols))
  for (i in 1:length(variableTypes)) {
    ind <- grepl(variableTypes[i],cols)
    indy <- ind | indy
  }
  cols <- cols[indy]
  
  #### Calculate the mean and standard deviation for each variable
  ####    across all signature matrices
  outstats <- MultiProcMeanSDCalc(FilesVec=paste(sigPath,"/",filelist,sep=""),
    numjobs=numjobs,cols=cols,variableTypes=variableTypes,SC=SC)
  diststats <- round(outstats[["clust"]],4)
  sigStats <- outstats[["sig"]]
  print(paste("Cluster Summary Statistics Calculated (Total time elapsed = ",
    round((proc.time()[3]-timer)/60,2)," minutes)",sep=""))
    
  ### These are place holders until function is ready ###
  #diststats <- matrix(c(60.00,0.022,0.005,0.002,1.08,0.002,
  #  0.02,0.002),nrow=2)
  #rownames(diststats) <- c("mean","sd")
  #colnames(diststats) <- c("FREQ.mean","FREQ.sd","V.mean","V.sd")
    
  #### Determine all classifications (centers) for each variable type
  class.list <- allClasses(diststats=diststats,SC=SC)

  #### Create cluster definitions by taking all combinations of the
  ####   classifications for each variable type
  ####   Also Create Storymeister definitions of each cluster
  clusterDefs <- createClusters(class.list=class.list)
  print(paste("Custer definitions created (Total time elapsed = ",
    round((proc.time()[3]-timer)/60,2)," minutes)",sep=""))

  #### Apply a dist function to determine which cluster each data point belongs
  ####    to.  The one with the smallest distance is the match.  Save
  ####    the distance value and tabulate and store the cluster membership.
  dist.out <- assignClusters(filesVec=paste(sigPath,"/",filelist,sep=""),
    clusterDefs=clusterDefs,diststats=diststats,numjobs=numjobs,SC=SC)
  print(paste("Cluster membership assigned (Total time elapsed = ",
    round((proc.time()[3]-timer)/60,2)," minutes)",sep=""))

  #### Calculate the cluster membership cutoff using cutPercentile from distances
  cutoff <- quantile(dist.out$distance,probs=cutPercentile,na.rm=TRUE)
  
  #### Create Cluster Displays for the Output
  clusterDisplays(clusters=dist.out$cluster,storymeister=clusterDefs$storymeister,
    outputPath=outputPath)
  print(paste("Cluster displays created (Total time elapsed = ",
    round((proc.time()[3]-timer)/60,2)," minutes)",sep=""))

  #### Output
  clusterOutput <- list(centers=clusterDefs$centers, cutoff=cutoff,
    membership=dist.out$cluster,storymeister=clusterDefs$storymeister,
    sigStats=sigStats,distStats=diststats)

  save(list="clusterOutput",file=paste(sigPath,"/clusterOutput.RData",sep=""))
  print(paste("Cluster update completed (Total time elapsed = ",
    round((proc.time()[3]-timer)/60,2)," minutes)",sep=""))

  clusterOutput

} # ends function
