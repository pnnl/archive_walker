##' This function calculates the GAS scores for a signature matrix
##' @export
##' @param filename 
##'
##' @param clusterOutput
##'
##' @param baselineOutput
##'
##' @author Brett Amidan
##'
##' @examples
##' sigGAS()

  sigGAS <- function(sig,clusterOutput,baselineOutput,clusters) {

    require(bpaFUN)

    output <- NULL
    ## cluster info
    proptable <- table(clusterOutput$membership)/length(clusterOutput$membership)
    ## determine the cluster props for each data point
    ind <- match(as.character(clusters),names(proptable))
    clustprops <- proptable[ind]

    ## loop thru each analysis
    for (i in 1:length(baselineOutput)) {
      ## calculate the Mahalanobis distances
      mdout <- mahaDist(the.data=sig,
        colmeans=baselineOutput[[i]][["clusterMeans"]],
        colsds=baselineOutput[[i]][["clusterSDs"]],
        pcaOut=baselineOutput[[i]][["pcaOut"]])
      ## calculate the gas
      if (sum(!is.na(mdout))>1) {
      tempscores <- calcGAS(dataVec=mdout,
        gammaParams=baselineOutput[[i]]$gammaParams,clusterProp=clustprops)
      } else {
        tempscores <- rep(NA,length(mdout))
      }
      output <- cbind(output,tempscores)
    } # ends i
    colnames(output) <- names(baselineOutput)
    rownames(output) <- names(clusters)
    output
  } # ends function
