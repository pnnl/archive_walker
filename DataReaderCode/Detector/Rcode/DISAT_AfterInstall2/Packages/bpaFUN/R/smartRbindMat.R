##' This function Row bind matrices whose column names may not be the same.
##' @export
##' @param items to row bind
##'
##' @return bound matrix
##'
##' @author Landon Sego
##'
##' @examples
##' smartRbindMat()

smartRbindMat <- function(..., distinguish=FALSE) {

  # Preliminary handling of the objects
  mats <- list(...)
  obj.names <- as.character(substitute(list(...)))[-1]
  names(mats) <- obj.names

  # If any of the objects are NULL, remove them from the list

  # Verify they are all dataframes or matrices, and obtain the union of the
  # column names.  If rownames are missing, they are assigned a number
  # with the object name as a prefix, separated by a ":"
  union.colnames <- NULL
  cnt <- 0

  check.objects <- function(x) {

    # Increment the counter (and record the value outside the function)
    assign("cnt", cnt + 1, inherits=TRUE)

    # This allows the possibility that one of objects was NULL
    if (!is.null(x)) {
      
      # Verify we're working with a matrix
      if (!is.matrix(x))
          stop("'", obj.names[cnt], "' is not a matrix\n")
  
      cnames <- colnames(x)
  
      # Make sure the names are not missing
      if (is.null(cnames))
        stop("'", obj.names[cnt], "' does not have column names\n")
  
      # Column names must be unique
      if (length(unique(cnames)) < length(cnames))
        stop("'", obj.names[cnt], "' does not have unique column names\n")
  
      # Find the union of all the column names, record the value outside the function
      assign("union.colnames", union(union.colnames, cnames), inherits=TRUE)
  
      # Add in rownames if they're not present
      if (distinguish) {
        if (is.null(rownames(x)))
          rownames(x) <- paste(obj.names[cnt], 1:NROW(x), sep=":")
      }

    } # if (!is.null(x))

    x
        
  } # check.objects()

  # Makes it easier to trace an error
  mats <- try(lapply(mats, check.objects), silent=TRUE)
  if (class(mats) == "try-error")
    stop(mats)

  # Add in NA columns where necessary and reorder to give the same ordering
  addinNA <- function(x) {

    # Identify the differences
    missing.cols <- setdiff(union.colnames, colnames(x))

    # Add in missing columns
    if (length(missing.cols)) {
      for (mc in missing.cols) {
        x <- cbind(x, rep(NA,NROW(x)))
        colnames(x)[NCOL(x)] <- mc
      }
    }

    # Prevent matrices from 1 row from being collapsed into a named vector
    if (NROW(x)==1)
      matrix(x[,union.colnames], nrow=1,
             dimnames=list(rownames(x),union.colnames))
    else
      x[,union.colnames]

  } # addinNA

  # Makes it easier to trace an error
  mats <- try(lapply(mats, addinNA), silent=TRUE)
  if (class(mats) == "try-error")
    stop(mats)

  # Text that, when executed, will bind all the matrices together with
  # a single call to rbind()
  rbind.text <- paste("rbind(",
                      paste(paste("mats[[", names(mats), "]]", sep='"'),
                            collapse=","),
                      ")", sep="")

  # return the result
  eval(parse(text=rbind.text))
  
} # smartRbind()

