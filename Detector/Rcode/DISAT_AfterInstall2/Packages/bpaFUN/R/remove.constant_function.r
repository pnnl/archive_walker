##' This function NA's all data constant for a given time
##' @export
##' @param data.vec
##'
##' @param constant
##'
##' @return DQ Report
##'
##' @author Brett Amidan
##'
##' @examples
##' remove.constant(input="D:/Data/Data.csv",rdata.path="D:/Test")

remove.constant <- function(data.vec, constant=30) {

  ## constant is the number of records needed to be similar
  ind <- !is.na(data.vec)
  start.row <- c(1:length(data.vec))[ind][1]
  
  while (start.row < (length(data.vec)-constant)) {
    if (!is.na(data.vec[start.row])) {
      ## do this if start.row value is not NA
      ind <- data.vec == data.vec[start.row] | is.na(data.vec)
      sim.rows <- c(1:length(data.vec))[ind]
      sim.rows <- sim.rows[sim.rows>start.row]
      ## continue if enough similar
      if (length(sim.rows) >= constant) {
        temp <- c(diff(sim.rows),1)
        ind2 <- temp != 1
        end.row <- sim.rows[ind2][1]
        ## if all the same to the end, do this
        if (sum(ind2)==0) {
          end.row <- length(data.vec)
        }
        if ((end.row - start.row) > constant) {
          data.vec[start.row:end.row] <- NA
          start.row <- end.row + 1
        } else {
          start.row <- end.row + 1
        }
      } else {
        start.row <- start.row + 1
      }
    } else {
      ## find next start.row that is not NA if start.row value is NA
      temprows <- c(1:length(data.vec))[!is.na(data.vec)]
      ind <- temprows > start.row
      start.row <- temprows[ind][1]
    } # ends if / else
  } # ends while
  
  data.vec
  
} # ends function
