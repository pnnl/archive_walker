##' This function all the class centers for each variable
##' @export
##' @param diststats contains all the distributive statistics for each variable, means and stdevs
##'
##' @return list of class centers
##'
##' @author Brett Amidan
##'
##' @examples
##' allClasses()

allClasses <- function(diststats,SC=FALSE) {
  
  output <- vector(mode = "list", length = ncol(diststats))
  names(output) <- colnames(diststats)
  ind1 <- grepl("mean", colnames(diststats))
  for (i in 1:sum(ind1)) {
    vals <- c(diststats[,ind1]["mean",i]+3*diststats[,ind1]["sd",i],
              diststats[,ind1]["mean",i],
              diststats[,ind1]["mean",i]-3*diststats[,ind1]["sd",i])
    names(vals) <- c("High", "Normal", "Low")
    output[[colnames(diststats)[ind1][i]]] <- vals
  }
  
  ind1 <- grepl("sd", colnames(diststats))
  if (SC==FALSE) {
    # Brett's signatures
    for (i in 1:sum(ind1)) {
      vals <- c(diststats[,ind1,drop=FALSE]["mean",i]+2*diststats[,ind1,drop=FALSE]["sd",i],
                diststats[,ind1,drop=FALSE]["mean",i])
      names(vals) <- c("High", "Normal")
      output[[colnames(diststats)[ind1][i]]] <- vals
    }
  } else {
    # Spectral coherence signatures
    for (i in 1:sum(ind1)) {
      vals <- c(diststats[,ind1,drop=FALSE]["mean",i],
                diststats[,ind1,drop=FALSE]["mean",i]-2*diststats[,ind1,drop=FALSE]["sd",i])
      names(vals) <- c("Normal", "Low")
      output[[colnames(diststats)[ind1][i]]] <- vals
    }
  }
  
  
  output
}