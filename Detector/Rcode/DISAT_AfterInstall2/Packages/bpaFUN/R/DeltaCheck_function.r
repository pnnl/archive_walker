##' This function Checks the delta for DQ
##' @export
##' @param data.mat
##'
##' @param delta
##'
##' @param delta.criteria
##'
##' @param constant
##'
##' @param angle
##'
##' @return DQ report
##'
##' @author Brett Amidan
##'
##' @examples
##' DeltaCheck()

DeltaCheck <- function(data.mat,delta,delta.criteria,constant,angle=TRUE) {
  require(bpaFUN)
  
  ### Get the first order differences
  diff.mat <- apply(data.mat,MARGIN=2,diff)
  
  ### if phase angle, then fix any differences around 360
  if (angle) {
    indy <- diff.mat < -350 & diff.mat > -370 & !is.na(diff.mat)
    diff.mat[indy] <- diff.mat[indy] + 360
    indy <- diff.mat > 350 & diff.mat < 370 & !is.na(diff.mat)
    diff.mat[indy] <- diff.mat[indy] - 360
  }
  
  ## set up output
  output <- rep(0,ncol(data.mat))
  names(output) <- colnames(data.mat)
  
  ####################################################
  ## look for exceedances of delta limit (+ or -)

  exceedances <- (diff.mat > delta & !is.na(diff.mat)) |
    (diff.mat < -delta & !is.na(diff.mat))
  ## find out the variables (columns)
  cs <- colSums(exceedances)
  indy2 <- cs > 0
  if (sum(indy2) > 0) {
    output[names(cs[indy2])] <- cs[indy2]
  }
  ### STILL need to determine what to do and then make corrections
  ###   include a look for how long the delta and remove data between delta
  ###   changes
  
  
  ####################################################
  ## look for constants of longer than constant
  
  indy3 <- diff.mat == 0 & !is.na(diff.mat)
  cs <- colSums(indy3)
  indy4 <- cs > (constant-1) 
  ## do this if there are some constants
  if (sum(indy4)>0) {
    for (i in 1:sum(indy4)) {
      ## look for constants > constant measures
      out <- remove.constant(data.vec=data.mat[,names(cs)[indy4][i]],
        constant=constant)
      output[names(cs)[indy4][i]] <- sum(is.na(out))
    } # ends i
  } # ends if
  
  output
} # ends function

  