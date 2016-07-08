##' This function creates the cluster definitions and storymeisters
##' @export
##' @param class.list is a list containing all classes for all variables
##'
##' @return list of cluster definitions matrix and storymeister vector
##'
##' @author Brett Amidan
##'
##' @examples
##' createClusters()

createClusters <- function(class.list) {

  numclusts <- 1
  for (i in 1:length(class.list)) {
    numclusts <- numclusts*length(class.list[[i]])
  }

  ## setup output
  storymeister <- rep("",length=numclusts)
  
  #### find all combinations
  centers <- expand.grid(class.list)
  
  #### create the storymeister for cluster
  for (i in 1:nrow(centers)) {
    paragraph <- ""
    ## loop thru each variable and add a sentence
    for (j in 1:ncol(centers)) {
      indy <- is.element(class.list[[j]],centers[i,j])
      newsentence <- paste(gsub("."," ",colnames(centers)[j],fixed=TRUE)," is ",
        names(class.list[[j]])[indy], "(",class.list[[j]][indy],").",sep="")
      paragraph <- paste(paragraph," ",newsentence,sep="")
    } # ends j
    ## store in storymeister
    storymeister[i] <- paragraph
  } # ends i

  list(centers=centers,storymeister=storymeister)
  
} # ends function
