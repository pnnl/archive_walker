##' This function calculates the GAS scores for each data point
##' @export
##' @param dataVec is a vector of the mahalanobis distances
##'
##' @param gammaParams are the parameters from the gamma distribution
##'
##' @param clusterProp is the cluster proportion membership for the given cluster
##'
##' @param gammaCritVal if gamma pvalues are less than this, an assigned pval is given so pvalues of 0 are not calculated
##'
##' @return Global Atypicality Scores for all data points in dataVec
##'
##' @author Brett Amidan
##'
##' @examples
##' calcGAS()

calcGAS <- function(dataVec,gammaParams,clusterProp,gammaCritVal=0.00001) {

  ## gamma p-value
  gammaP <- 1 - pgamma(dataVec,shape=gammaParams$shape,
    rate=gammaParams$rate)

  ## For gamma pvalues less than gamma.crit.val, then an assigned pvalue
  ##  is given that ranges equally between gamma.crit.val and 0,
  ##  this is done so that pvalues of 0 are not calculated.
  ind <- gammaP < gammaCritVal
  if (sum(ind)>0)  {
     num.below <- sum(ind)  ## number below gamma.crit.val
     ## width for num.below segments between gamma.crit.val and 0
     width <- gammaCritVal / num.below
     ## boundaries for the segments
     end.pts.vec <- seq(gammaCritVal,0,by=-width)
     ## mid.pts.vec contaisn pvalues for values less than crit value
     mid.pts.vec <- c(end.pts.vec-width/2)[1:num.below]

     ## next step assign these pvalues to the "too" low gamma pvalues
     nn <- length(dataVec)
     temp.ord <- order(dataVec)[(nn-num.below+1):nn]
     gammaP[temp.ord] <- mid.pts.vec
  }

  ## calculate the global atypicality score
  GAS <- -log(gammaP) - log(clusterProp)

  GAS
} # ends function
