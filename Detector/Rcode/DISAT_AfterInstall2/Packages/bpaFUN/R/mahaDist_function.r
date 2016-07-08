##' This function calculates the mahalanobis distances for each row in matrix
##' @export
##' @param the.data is the matrix of data
##'
##' @param colmeans
##'
##' @param colsds
##'
##' @param pcaOut is output from the mypca function
##'
##' @return mahalanobis distance
##'
##' @author Brett Amidan
##'
##' @examples
##' mahaDist()

mahaDist <- function(the.data,colmeans,colsds,pcaOut) {

  #### Make sure the.data has the necessary columns
  ind1 <- is.element(names(colmeans),colnames(the.data))
  if (sum(!ind1)>0) {
    addmat <- matrix(NA,nrow=nrow(the.data),ncol=sum(!ind1))
    colnames(addmat) <- names(colmeans)[!ind1]
    rownames(addmat) <- rownames(the.data)
    the.data <- cbind(the.data,addmat)
  }
  #### trim data to the proper columns
  ind <- is.element(colnames(the.data),names(colmeans))
  the.data <- the.data[,ind]
  #### sort the.data so same order as colmeans
  the.data <- the.data[,names(colmeans)]
  
  #### remove any columns with 0% variance
  ind <- colsds == 0
  the.data <- the.data[,!ind]
  colmeans <- colmeans[!ind]
  colsds <- colsds[!ind]
  
  #### center and scale the data
  temp <- the.data - matrix(colmeans,nrow=nrow(the.data),ncol=length(colmeans),
    byrow=TRUE)
  temp <- temp / matrix(colsds,nrow=nrow(temp),ncol=length(colsds),byrow=TRUE)

  #### replace any NA with 0 (basically replacing NA's with the mean)
  indna <- is.na(temp)
  temp[indna] <- 0
  
  #### translate data using PCA loadings
  temp2 <- temp %*% pcaOut$pcaOut$rot[,1:pcaOut$numComp]
  
  #### calculate the maha distances
  numMat <- temp2^2
  denomVec <- apply(temp2,MARGIN=2,var)
  tempMat <- numMat / matrix(denomVec,nrow=nrow(numMat),ncol=length(denomVec),
    byrow=TRUE)
  mdist <- rowSums(tempMat)
  
  #### output
  mdist

} # ends function
