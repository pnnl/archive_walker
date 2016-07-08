##' This function creates the html output pages for the clusters
##' @export
##' @param clusters is a vector containing all the cluster assignments for each minute
##'
##' @param storymeister is a vector containing all the stories for each cluster
##'
##' @param outputPath is the path in which the output will be stored
##'
##' @return Nothing / cluster summaries are saved to disk
##'
##' @author Brett Amidan
##'
##' @examples
##' clusterDisplays()

clusterDisplays <- function(clusters,storymeister,outputPath) {

  require(hwriter)

  ## create folder for storing cluster output
  dir.create(paste(outputPath,"/Clusters",sep=""),showWarnings=FALSE)

  ###### create the output table
  clusters <- clusters[!is.na(clusters)]
  ###### Membership Proportion
  temptab <- rev(sort(table(clusters)))
  Proportion <- paste(round(temptab/length(clusters)*100,3),"% (",
    temptab, " min in ", round(length(clusters)/1440,0)," days)",sep="")
  names(Proportion) <- names(temptab)
  names(storymeister) <- as.character(1:length(storymeister))
  
  outTable <- matrix("",nrow=length(Proportion),ncol=3)
  colnames(outTable) <- c("Cluster","Proportion","ClusterDefinition")
  rownames(outTable) <- names(Proportion)
  ## populate table
  outTable[,"Cluster"] <- names(Proportion)
  outTable[,"Proportion"] <- Proportion
  outTable[,"ClusterDefinition"] <- storymeister[rownames(outTable)]
  rownames(outTable) <- NULL
  
  ###### Make the html table  #################
  p <- openPage('Cluster.html',dirname=paste(outputPath,"/Clusters",sep=""),
    link.css='http://www.ebi.ac.uk/~gpau/hwriter/hwriter.css')
  hwrite("Cluster Information", p, center=TRUE, heading=1)
  hwrite(outTable, p, row.bgcolor='#ffffaa')
  closePage(p)

  ### Make a page for each Cluster
  monthVec <- sort(unique(substring(names(clusters),1,7)))
  for (i in outTable[,"Cluster"]) {
    indy3 <- outTable[,"Cluster"]==i
    tempTab <- matrix(outTable[indy3,],ncol=3)
    colnames(tempTab) <- colnames(outTable)
    
    ## setup count vec
    countVec <- rep(0,length(monthVec))
    names(countVec) <- monthVec
    indy <- clusters==as.numeric(i)
    temp <- table(substring(names(clusters)[indy],1,7))
    countVec[names(temp)] <- temp
    
    ## make a plot over time
    jpeg(filename=paste(outputPath,"/Clusters/",i,".jpg",sep=""),
      width=7,height=5,units="in",res=150)
    barplot(countVec,ylab="Count",las=2)
    dev.off()
    ### plot link
    plotLink <- paste("<img src='file:///",outputPath,"/Clusters/",i,".jpg' />",
      sep="")
    ## set up page for cluster
    p <- openPage(paste(i,'.html',sep=""),
      dirname=paste(outputPath,"/Clusters",sep=""),
      link.css='http://www.ebi.ac.uk/~gpau/hwriter/hwriter.css')
    hwrite(paste("Cluster ",i,sep=""), p, center=TRUE, heading=1)
    hwrite(tempTab, p, row.bgcolor='#ffffaa')
    hwrite(plotLink,p)
    closePage(p)
  } # ends i

  invisible()

} # ends function
