##' This function calculates the differences between two angles
##' @export
##' @param adata  is angle data
##'
##' @param ref.angle is name of reference angle
##'
##' @return Angle difference calcs
##'
##' @author Brett Amidan
##'
##' @examples
##' CalcPhaseAngleDiff()

CalcPhaseAngleDiff <- function(adata,ref.angle) {

  ## adata contains all the angle data (angles on columns)
  ## ref.angle is the name of the reference angle

  ## make vector of ref.angle
  ra.vec <- adata[,ref.angle]
  ind <- colnames(adata) == ref.angle
  adata <- adata[,!ind]
  
  ## Calculate phase angle difference
  out <- apply(adata,MARGIN=2,CalcAngDiff,Ang2=ra.vec)

  out
} # ends function

