##' This function calculates the principle component output
##' @export
##' @param the.data is the matrix of data
##'
##' @param colmeans are the column means
##'
##' @param colsds are the column st devs
##'
##' @param pcaCutoff is the percent variance to be explained by the PCA default = 0.80
##'
##' @return PCA output
##'
##' @author Brett Amidan
##'
##' @examples
##' mypca()

mypca <- function(the.data,colmeans,colsds,pcaCutoff=0.80) {

  #### remove any columns with 0% variance
  ind <- colsds == 0
  the.data <- the.data[,!ind]
  colmeans <- colmeans[!ind]
  colsds <- colsds[!ind]
  
  #### center and scale the data
  temp <- the.data - matrix(colmeans,nrow=nrow(the.data),ncol=length(colmeans),
    byrow=TRUE)
  temp <- temp / matrix(colsds,nrow=nrow(temp),ncol=length(colsds),byrow=TRUE)
  
  #### Perform PCA
  pc.out <- prcomp(temp,center=FALSE,scale.=FALSE)
  sdev <- pc.out$sdev
  cumprop <- cumsum(sdev^2)/max(cumsum(sdev^2))
  ind.var.exp <- cumprop < pcaCutoff
  num.comp <- sum(ind.var.exp)+1
  
  #### output
  list(pcaOut=pc.out,numComp=num.comp)

} # ends function
