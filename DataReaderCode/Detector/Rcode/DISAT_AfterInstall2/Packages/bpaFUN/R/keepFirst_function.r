##' This function trims a vector of times keeping only the first one in consecutive minutes
##' @export
##' @param timeVec is a vector of the times (minutes)
##'
##' @author Brett Amidan
##'
##' @examples
##' keepFirst()

keepFirst <- function(timeVec) {

  diffvec <- diff(as.POSIXct(timeVec))
  keepind <- rep(TRUE,length(timeVec))
  ## remove any consecutive diffvec=1
  for (i in 1:length(diffvec)) {
    if (diffvec[i]<=5) keepind[i+1] <- FALSE
  } # ends
  timeVec[keepind]
  
} # ends function
