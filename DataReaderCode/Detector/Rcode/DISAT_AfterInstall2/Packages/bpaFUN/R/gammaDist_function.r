##' This function calculates the gamma distribution parameter estimates
##' @export
##' @param dataVec is a vector of the mahalanobis distances
##'
##' @param trimProp
##'
##' @return gamma shape and rate parameters
##'
##' @author Brett Amidan
##'
##' @examples
##' gammaDist()

gammaDist <- function(dataVec,trimProp=0.01) {
  require(bpaFUN)
  
  ##  Trim data to calculate the shape and parameter
  trimmed.var <- trimVar(dataVec,p=trimProp)
  ## p is proportion to trim from each end
  trimmed.mean <- mean(dataVec,trim=trimProp)
  ## trim is proportion to trim from each end

  ## Need to make sure that variance isn't too close to 0
  if (trimmed.var < trimProp)  trimmed.var <- trimProp
  ## Calculate shape and rate
  shape.parameter <- trimmed.mean^2 / trimmed.var
  rate.parameter <- trimmed.mean / trimmed.var

  list(shape=shape.parameter,rate=rate.parameter)

} # ends function
