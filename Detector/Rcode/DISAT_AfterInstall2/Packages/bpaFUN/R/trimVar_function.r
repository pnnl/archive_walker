##' This function calculates the variance from a trimmed data vector
##' @export
##' @param x
##'
##' @param p is the proportion to trim from each end (default=.05)
##'
##' @return variance
##'
##' @author Brett Amidan
##'
##' @examples
##' trimVar(x=c(1:10),p=0.05)


trimVar <- function(x,p=0.05)  {
	## x is a vector of data
	## p is the proportion to trim off of each end
	
	n <- length(x)
	id <- (floor(p*n)+1):(ceiling((1-p)*n))
	out <- var(sort(x)[id],na.rm=T)
		
	out
	
}

