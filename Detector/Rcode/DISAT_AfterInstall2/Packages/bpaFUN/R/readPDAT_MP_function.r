##' This function reads in a DST file using multiple processes
##' @export
##' @param folders contains fullname folders of raw PMU data
##'
##' @return No object returned
##'
##' @author Brett Amidan / Lucas Tate
##'
##' @examples
##' readDST_MP(input="D:/Data/Data.csv",rdata.path="D:/Test")

readPDAT_MP <- function(folders,rdata.path,num.jobs=2,newOnly=TRUE) {
  
  require(snowfall)
  
  #### Make a list of all files to process
  filenameVec <- NULL
  for (i in folders) {
    templist <- dir(i,full.names=TRUE)
    templist2 <- dir(i)
    ## make the folder in Rdata
    temp2 <- unlist(strsplit(templist2[1],split="_",fixed=TRUE))
    ## make the folder (if it doesn't exist)
    rdatafolder <- paste(rdata.path,substring(temp2[2],1,4),
                         substring(temp2[2],5,6),substring(temp2[2],7,8),sep="/")
    dir.create(rdatafolder,showWarnings=FALSE,recursive=TRUE)
    
    ## remove any that were already read
    if (newOnly) {
      rdatalist <- dir(rdatafolder)
      rdatalist2 <- gsub(".Rdata",".pdat",rdatalist)
      indy <- is.element(templist2,rdatalist2)
      templist <- templist[!indy]
    }
    filenameVec <- c(filenameVec,templist)
  } # ends i
  ## make the list
  filename.list <- vector(mode="list",length=length(filenameVec))
  if(length(filenameVec)!=0) {
    for (i in 1:length(filenameVec)) {
      ## get the output path
      temp2 <- unlist(strsplit(filenameVec[i],split="_",fixed=TRUE))  # Break the filename apart where there are underscores
      ind <- nchar(temp2) == 8
      temp2 <- temp2[ind]                                               # Keep only the part that has date information
      temp2 <- paste(rdata.path,substring(temp2,1,4),
                     substring(temp2,5,6),substring(temp2,7,8),sep="/")  # Make a path with year, month, and day folders
      ## add to the list
      temp <- c(filenameVec[i],temp2)
      names(temp) <- c("filename","outfolder")
      filename.list[[i]] <- temp
    } # ends i
    ### Run the reader using multiple processes
    sfInit(parallel=TRUE,cpus=num.jobs)
    
    #  sfExport("readPMU_PDAT")
    
    #   tic = Sys.time()
    
    out <- sfLapply(filename.list,readPDAT)
    
    #   toc = Sys.time() - tic
    #   print(toc)
    
    sfStop()
    
    invisible()
  }
} # ends function