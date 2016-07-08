##' this function calculates the differences between 2 common ISO angles
##' @export
##' @param Ang1 is vector of 1 angle data
##'
##' @param Ang2 is vector of 2nd angle data
##'
##' @return Angle difference calcs
##'
##' @author Brett Amidan
##'
##' @examples
##' CalcAngDiff()

CalcAngDiff <- function (Ang1,Ang2) {
  ## angle difference between -180 and 180

  Ang1mAng20 = (Ang1 - Ang2) %% 360
  Ang1mAng2 = Ang1mAng20 -360*(Ang1mAng20> 180)
  names(Ang1mAng2) <- names(Ang1)
  Ang1mAng2
  
} # ends function

