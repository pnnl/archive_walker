##' This function creates the atypical tables
##' @export
##' @param sigPath
##'
##' @return atypicality tables
##'
##' @author Brett Amidan
##'
##' @examples
##' atypTables()

atypTables <- function(sigPath,gasMatrix,outputPath,
  analysisTypes,atypCutoff,clusterOutput,numjobs=1) {

  require(hwriter)

  ## get list all months
  monthsVec <- rev(sort(unique(substring(rownames(gasMatrix),1,7))))
  ## list of different analysis groups
  groups <- colnames(gasMatrix)

  ##### Create Front End HTML Table ####################
  outTable <- matrix(NA,nrow=length(monthsVec),ncol=length(groups))
  cnames <- NULL
  for (i in 1:length(groups)) {
    outTable[,i] <- paste('<a href="file:///',outputPath,'/',
      monthsVec,'/',groups[i],'.html">',monthsVec,'</a>',sep="")
    cnames <- c(cnames,analysisTypes[[i]]$title)
  }
  colnames(outTable) <- cnames
  
  outclust <- paste("<a href='file:///",outputPath,
    "/Clusters/Cluster.html'>Cluster Table</a>",sep="")

  ###### Make the html table  #################
  p <- openPage('DISAT.html',dirname=outputPath,
    link.css='http://www.ebi.ac.uk/~gpau/hwriter/hwriter.css')
  hwrite("Atypicality Results", p, center=TRUE, heading=1)
  hwrite(outTable, p, row.bgcolor='#ffffaa')
  hwrite("Cluster Summaries", p, center=TRUE, heading=1)
  hwrite(outclust, p, center=TRUE, row.bgcolor='#ffffaa')
  closePage(p)

  ### Create Atypicality Reports for each Month
  
  sfInit(parallel=TRUE,cpus=numjobs)
  ## run the single processor function multiprocessed
  output <- sfLapply(as.list(monthsVec),atypTables_SP,gasMatrix=gasMatrix,
    sigPath=sigPath,outputPath=outputPath,analysisTypes=analysisTypes,
    atypCutoff=atypCutoff,clusterOutput=clusterOutput)

  sfStop()

  invisible()

  

} # ends function
