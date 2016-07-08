##' This function reads in a DST file using multiple processes
##' @export
##' @param folders contains fullname folders of raw PMU data
##'
##' @param phasors is a data frame of the phasor info for the dataset
##'
##' @return No object returned
##'
##' @author Brett Amidan
##'
##' @examples
##' readDST_MP(input="D:/Data/Data.csv",rdata.path="D:/Test")

readDST_MP <- function(folders,rdata.path,phasors,BPAe=TRUE,replace=FALSE,
  num.jobs=2) {

  require(snowfall)
  require(bpaFUN)

  #### Make a list of all files to process
  filenameVec <- NULL
  for (i in folders) {
    filenameVec <- c(filenameVec,dir(i,full.names=TRUE))
  } # ends i
  ## make the list
  filename.list <- vector(mode="list",length=length(filenameVec))
  for (i in 1:length(filenameVec)) {
    ## get the output path
    temp2 <- unlist(strsplit(filenameVec[i],split="_",fixed=TRUE))
    temp2 <- temp2[length(temp2)]
    temp2 <- paste(rdata.path,"/20",substring(temp2,1,2),"/",
      substring(temp2,3,4),"/",substring(temp2,5,6),sep="")
    ## make the folder
    dir.create(temp2,showWarnings=FALSE,recursive=TRUE)
    ## add to the list
    temp <- c(filenameVec[i],temp2)
    names(temp) <- c("filename","outfolder")
    filename.list[[i]] <- temp
  } # ends i

  ### Run the reader using multiple processes
  sfInit(parallel=TRUE,cpus=num.jobs)

  out <- sfLapply(filename.list,readDST,phasors=phasors)

  sfStop()

  invisible()
} # ends function

