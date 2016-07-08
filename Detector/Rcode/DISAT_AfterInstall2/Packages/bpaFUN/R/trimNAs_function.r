##' This function calculates total variance from between and within
##' @export
##' @param the.data is the matrix of data
##'
##' @return most complete matrix without NAs
##'
##' @author Brett Amidan
##'
##' @examples
##' trimNAs()

trimNAs <- function(the.data) {


  ### trim columns that have > 80% NA
  namat <- is.na(the.data)
  ind.col.na <- colSums(namat)/nrow(namat) > 0.80
  newdata <- the.data[,!ind.col.na]
  ### trim rows that have > 80% NA
  namat <- is.na(newdata)
  ind.row.na <- rowSums(namat)/ncol(namat) > 0.80
  newdata <- newdata[!ind.row.na,]
  ### trim columns with > 50% NA
  namat <- is.na(newdata)
  ind.col.na <- colSums(namat)/nrow(namat) > 0.50
  newdata <- newdata[,!ind.col.na]
  ### trim rows that have > 50% NA
  namat <- is.na(newdata)
  ind.row.na <- rowSums(namat)/ncol(namat) > 0.50
  newdata <- newdata[!ind.row.na,]
  ### trim columns with > 30% NA
  namat <- is.na(newdata)
  ind.col.na <- colSums(namat)/nrow(namat) > 0.30
  newdata <- newdata[,!ind.col.na]
  ## remove any rows with any NAs
  namat <- is.na(newdata)
  ind.row.na <- rowSums(namat) > 0
  if (sum(!ind.row.na)>(nrow(namat)/2) & sum(!ind.row.na)>20) {
    newdata <- newdata[!ind.row.na,]
  } else {
    ## make another cut of columns
    ### trim columns with > 10% NA
    ind.col.na <- colSums(namat)/nrow(namat) > 0.10
    if (sum(!ind.col.na)>2) {
      newdata <- newdata[,!ind.col.na]
      ## try to remove any rows with any NAs
      namat <- is.na(newdata)
      ind.row.na <- rowSums(namat) > 0
      if (sum(!ind.row.na)>(nrow(namat)/2) & sum(!ind.row.na)>20) {
        newdata <- newdata[!ind.row.na,]
      } else {
        ## trim columns with > 5% NA
        ind.col.na <- colSums(namat)/nrow(namat) > 0.05
        newdata <- newdata[,!ind.col.na]
        ## remove any rows with any NAs
        namat <- is.na(newdata)
        ind.row.na <- rowSums(namat) > 0
        newdata <- newdata[!ind.row.na,]
      } # ends if/else
    } else {
      newdata <- newdata[!ind.row.na,]
    }
  }
  
  newdata

} # ends function
