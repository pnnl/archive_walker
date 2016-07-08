##' This function summarizes a days worth of data
##' @export
##' @param input contains the path of the day to summarize
##'
##' @param summary.path is the folder where the summary stats are stored
##'
##' @return No object returned
##'
##' @author Brett Amidan
##'
##' @examples
##' summarizeData(input=".../Rdata/2013/10/01",summary.path=".../Summary")

summarizeData <- function(input,summary.path,phasors) {

  require(pnlStat)
  require(bpaFUN)
  
  ## get list of all files
  file.list <- dir(input)
  
  if (length(file.list)>0) {
  ## loop thru each file
  for (i in 1:length(file.list)) {
    #print(file.list[i])
    ## load the data
    try(load(paste(input,"/",file.list[i],sep="")))
    if (is.element("data",ls())) {
      ## run the DQfilter
      data <- DQfilter(data=data/10000,phasors=phasors)
      ## calculate means/sd of all the F,V,I
      last2 <- substring(colnames(data),nchar(colnames(data))-1)
      ind <- is.element(last2,c(".V","eq",".I"))
      temp1 <- colMeans(data[,ind],na.rm=TRUE)/10000
      temp2 <- apply(data[,ind]/10000,MARGIN=2,sd,na.rm=TRUE)
      ## calculate means/sd of all the Angle 1st order diffs
      ind2 <- is.element(last2,c("VA","IA"))
      temp1 <- c(temp1,colMeans((diff(data[,ind2])/1000),na.rm=TRUE))
      temp2 <- c(temp2,apply((diff(data[,ind2])/1000),MARGIN=2,sd,na.rm=TRUE))
      m1 <- matrix(temp1,nrow=1,dimnames=list(rownames(data)[1],names(temp1)))
      m2 <- matrix(temp2,nrow=1,dimnames=list(rownames(data)[1],names(temp2)))
      ## get number of NAs
      temp3 <- colSums(is.na(data[,ind | ind2]))
      m3 <- matrix(temp3,nrow=1,dimnames=list(rownames(data)[1],names(temp3)))
      ## store
      if (i ==1)  {
        means <- m1
        stdevs <- m2
        numnas <- m3
      } else {
        means <- smartRbindMat(means,m1)
        stdevs <- smartRbindMat(stdevs,m2)
        numnas <- smartRbindMat(numnas,m3)
      }
    } else {
      print(paste(file.list[i], " didn't read",sep=""))
    }
    rm(list="data")
  } # ends i

  temp <- unlist(strsplit(input,"/"))
  outfile <- paste(temp[length(temp)-2],temp[length(temp)-1],temp[length(temp)],sep="")
  
  ### save the output
  try(save(list=c("means","stdevs","numnas"),file=paste(summary.path,"/",
    outfile,".Rdata",sep="")))
    
  }
  invisible()
} # ends function

