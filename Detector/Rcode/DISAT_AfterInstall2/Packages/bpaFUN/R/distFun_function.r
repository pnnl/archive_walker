##' This function calculates the distance a vector is from all rows in a matrix and finds min
##' @export
##' @param the.vec is the vector of data to compare to the matrix
##'
##' @param the.matrix is the centers to compare distance the.vec is
##'
##' @param diststats contains the means and sd for each variable
##'
##' @return a vectorwith cluster number and distance for the vector of data
##'
##' @author Brett Amidan
##'
##' @examples
##' distFun()

distFun <- function(the.vec,the.matrix,diststats) {

  x2 <- matrix(the.vec,ncol=ncol(the.matrix),nrow=nrow(the.matrix),byrow=TRUE) - the.matrix
  ## standardize using sd for each
  x3 <- x2 / matrix(diststats["sd",colnames(x2)],nrow=nrow(x2),ncol=ncol(x2),
    byrow=TRUE)
  dists <- sqrt(rowSums(x3^2))
  ## find cluster the.vec is closest to
  indy <- dists == min(dists)
  cluster <- c(1:nrow(the.matrix))[indy][1]

  ## calculate minimum dist based on .mean variables only
  ind2 <- grepl(".mean",colnames(x3),fixed=TRUE)
  dists2 <- sqrt(rowSums(x3[,ind2]^2))
  output <- c(cluster,dists2[cluster])
  
  names(output) <- c("Cluster","Dist")
  
  output
} # ends function
