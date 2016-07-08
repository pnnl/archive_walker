##' This function does the multiprocessing of summarizing each day of data
##' @export
##' @param rdata.path is the folder where rdata is stored
##'
##' @param summary.path is the folder where the summary stats are stored
##'
##' @return No object returned
##'
##' @author Brett Amidan
##'
##' @examples
##' summarizeData_MP(rdata.path="//olympus/disat/Rdata",summary.path="//olympus/disat/Summary")


summarizeData_MP <- function(rdata.path,summary.path,phasors,num.jobs) {

  require(snowfall)
  require(bpaFUN)

  inputs <- NULL
  
  year.vec <- dir(rdata.path)
  ## loop thru the years in rdata.path
  for (year in year.vec) {
    month.vec <- dir(paste(rdata.path,"/",year,sep=""))
    ## loop thru the months
    for (month in month.vec) {
      day.vec <- dir(paste(rdata.path,"/",year,"/",month,sep=""))
      ## only include those that haven't been done
      sum.dir <- dir(summary.path)
      indy <- substring(sum.dir,1,4)==year & substring(sum.dir,5,6)==month
      sum.dir <- sum.dir[indy]
      indy2 <- is.element(day.vec,substring(sum.dir,7,8))
      day.vec <- day.vec[!indy2]
      if (length(day.vec) > 0) {
        ## make a list of paths
        inputs <- c(inputs,paste(rdata.path,"/",year,"/",month,"/",day.vec,
          sep=""))
      }
    } # ends month loop
  } # ends year loop
  inputs <- as.list(inputs)
  
  ## create the summaries
  sfInit(parallel=TRUE,cpus=num.jobs)

  out <- sfLapply(inputs,summarizeData,summary.path=summary.path,phasors=phasors)

  sfStop()

  invisible()
} # ends function

