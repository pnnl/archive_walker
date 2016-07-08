##' This function calculates the performance envelopes for the plots
##' @export
##' @param the.data is the matrix of data
##'
##' @param colmeans
##'
##' @param colsds
##'
##' @param pcaOut is output from the mypca function
##'
##' @return mahalanobis distance
##'
##' @author Brett Amidan
##'
##' @examples
##' mahaDist()

perfEnvelope <- function(sigPath,daysToUse=730) {

  ## get a listing of signature data
  siglisting <- dir(sigPath,pattern="20")
  ## trim to daysToUse
  lastday <- as.POSIXct(substring(siglisting[length(siglisting)],1,10))
  startday <- lastday - daysToUse*60*24*60
  possdays <- substring(as.character(seq(startday,lastday,by=60*24*60)),1,10)
  ind <- is.element(substring(siglisting,1,10),possdays)
  siglisting <- siglisting[ind]
  
  #####

  
} # ends function
