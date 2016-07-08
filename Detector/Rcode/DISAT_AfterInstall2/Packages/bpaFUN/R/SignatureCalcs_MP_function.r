##' This function calculates the signatures with multiple processes for each day
##' @export
##' @param Rdata.paths are all the paths with a month's worth of Rdata objects
##'
##' @param Sig.path  is path where signatures will be stored
##'
##' @param ref.angle is name of reference angle
##'
##' @param ref.angle.label is a vector of single letters associated with each ref angle
##'
##' @return signatures for a single day
##'
##' @author Brett Amidan
##'
##' @examples
##' SignatureCalcs_MP()

SignatureCalcs_MP <- function(Rdata.paths,Sig.path,ref.angle=NULL,
  ref.angle.label=NULL,rm.vars=NULL,skip=60,sample.rate=60,do.minute=TRUE,
  do.linear=FALSE,do.new.only=TRUE,phasors=NULL,numjobs=1) {

  require(bpaFUN)
  require(snowfall)

  output <- NULL
  for (i in 1:length(Rdata.paths)) {

    ## get list of filenames
    filename.list <- data.frame(dir(Rdata.paths[i],
      full.names=TRUE),stringsAsFactors=FALSE)
    colnames(filename.list) <- "Rdata.path"
    
    ## remove any already done
    if (do.new.only) {
      siglist <- dir(Sig.path)
      ## get dates of data to do
      temp <- strsplit(filename.list[,1],"/")
      tempfun <- function(x) {
        paste(x[length(x)-2],x[length(x)-1],x[length(x)],sep="-")
      }
      temp2 <- unlist(lapply(temp,tempfun))
      ind <- is.element(temp2,substring(siglist,1,10))
      filename.list <- data.frame(filename.list[!ind,],stringsAsFactors=FALSE)
      colnames(filename.list) <- "Rdata.path"
    }
    
    ## make it a list
    filename.list <- as.list(filename.list$Rdata.path)

    x.vec <- seq(-0.5,0.5,length=(sample.rate+1))

    if (length(filename.list)>0) {
      ## create the summaries
      sfInit(parallel=TRUE,cpus=numjobs)

      out <- sfLapply(filename.list,IVTcalc,Sig.path=Sig.path,
        ref.angle=ref.angle,ref.angle.label=ref.angle.label,
        rm.vars=rm.vars,x.vec=x.vec,skip=skip,
        do.minute=do.minute,sample.rate=sample.rate,do.linear=do.linear,
        phasors=phasors)

      sfStop()
    }

  } # ends i
  invisible()
} # ends function
