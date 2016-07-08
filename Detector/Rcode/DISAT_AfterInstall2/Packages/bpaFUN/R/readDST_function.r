##' This function reads in a DST file
##' @export
##' @param input contains the filename, which is the full filename of the dst file; and store.path, which is where the output will be stored
##'
##' @param phasors is a data frame of the phasor info for the dataset
##'
##' @return No object returned
##'
##' @author Brett Amidan
##'
##' @examples
##' readDST(input="D:/Data/Data.csv",rdata.path="D:/Test")

readDST <- function(input,phasors) {

  require(bpaPMU)
  
  ###############  Read in the dst data
  data <- readPMU(as.character(input["filename"]),convert=TRUE)

  ## convert all Voltages to same units as BPA
  indy <- is.element(colnames(data),paste(phasors[,"VarName"],".V",sep=""))
  data[,indy] <- data[,indy]/(1000/sqrt(3))
  ## convert currents
  indy <- is.element(colnames(data),paste(phasors[,"VarName"],".I",sep=""))
  data[,indy] <- data[,indy]/(1000)
  
  ## multiple by 10000 and round; to store all as integers
  data <- round(data*10000,0)
  
  ## fix times
  newtime <- as.character(strftime(seq(as.POSIXlt(rownames(data)[1]),
    by=1/60,length=nrow(data)),"%Y-%m-%d %H:%M:%OS5"))
  rownames(data) <- newtime

  unq.times <- unique(substring(rownames(data),1,16))
  
  # get filename
  temp <- unlist(strsplit(as.character(input["filename"]),split="/",fixed=TRUE))
  filename <- unlist(strsplit(temp[length(temp)],split=".",fixed=TRUE))[1]

  ### if 1 minute data do this
  if (length(unq.times)==1) {
    ## save to rdata object
    save(list="data",file=paste(as.character(input["outfolder"]),"/",filename,
      ".Rdata",sep=""))
  } else {
    ## needs to be split for each minute and saved
    data2 <- data
    for (i in unq.times) {
      ## select the correct minute
      ind <- substring(rownames(data2),1,16) == i
      data <- data2[ind,]
      ## get the correct filename
      filename2 <- paste(substring(filename,1,13),substring(i,15),sep="")
      ## save it
      save(list="data",file=paste(as.character(input["outfolder"]),"/",
        filename2,".Rdata",sep=""))
    } # ends i
  } # ends if/else
  
  invisible()
} # ends function

