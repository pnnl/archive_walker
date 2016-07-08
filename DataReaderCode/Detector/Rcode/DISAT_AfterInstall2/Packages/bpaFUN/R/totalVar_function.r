##' This function calculates total variance from between and within
##' @export
##' @param the.data is the matrix of data
##'
##' @param variableTypes is a vector of the abbreviations of which variables to include in the calculation default is c("FREQ","V")
##'
##' @return matrix with sqrt(totalVariance) (SD) on the columns
##'
##' @author Brett Amidan
##'
##' @examples
##' totalVar()

totalVar <- function(the.data,variableTypes=c("FREQ","V")) {

  output <- NULL
  ## loop thru each variableType
  for (i in variableTypes) {
    output <- cbind(output,sqrt(the.data[,paste(i,".wsd",sep="")]^2 +
      the.data[,paste(i,".bsd",sep="")]^2))
  } # ends i
  colnames(output) <- paste(variableTypes,".sd",sep="")
  
  output
} # ends function
