##' This function calculates the signatures with multiple processes for each day
##' v2 is built to process PDAT data
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

rtSigCalc <- function(Rdata.paths,Sig.path,ref.angle=NULL,
  ref.angle.label=NULL,vars=NULL,skip=60,sample.rate=60,do.minute=TRUE,
  do.linear=FALSE,numjobs=1,SC=FALSE) {

  require(bpaFUN)
  require(snowfall)

  ## make a list of all days
  filename.list <- as.list(Rdata.paths)

  if (numjobs > length(filename.list)) numjobs <- length(filename.list)
    
  x.vec <- seq(-0.5,0.5,length=(sample.rate+1))

  if (length(filename.list)>0) {
    ## create the summaries
    sfInit(parallel=TRUE,cpus=numjobs)

    if (!SC) {
      # Anomaly Signatures
      out <- sfLapply(filename.list,IVTcalc_v2,Sig.path=Sig.path,
                      ref.angle=ref.angle,ref.angle.label=ref.angle.label,
                      vars=vars,x.vec=x.vec,skip=skip,do.minute=do.minute,
                      sample.rate=sample.rate,do.linear=do.linear)
    } else {
      # Spectral Coherence
      out = sfLapply(filename.list,SCsigCalc,Sig.path=Sig.path,
                     sample.rate=sample.rate,Clims=c(0, 0.2), 
                     Dparam=c(6,16,10), NaveSeg=2^12, NumAve=20, 
                     PctOverlap=0.5)
    }
    

    sfStop()
  }

  invisible()
} # ends function
