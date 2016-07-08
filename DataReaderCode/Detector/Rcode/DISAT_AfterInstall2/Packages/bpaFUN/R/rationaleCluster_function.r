##' This function creates a rationale of significant parameters
##' @export
##' @param timeVec is a vector of the times (minutes)
##'
##' @return rationale and significant variables
##'
##' @author Brett Amidan
##'
##' @examples
##' rationale()

rationaleCluster <- function(atypTime,sigPath,baselineOutput,last2Months,
  atypCutoff,lpv.vec=NULL,top=10,
  threshold.limits=c(-3.0,-2.0,-1.0,1.0,2.0,3.0)) {

  ##### get the signature matrix and cluster assignment
  load(paste(sigPath,"/",substring(atypTime,1,10),".Rdata",sep=""))
  
  ## Atypical Time's cluster
  clusterVal <- clusters[atypTime]

  output <- NULL
  
  ### Find out if atypical at the cluster
  ind <- rownames(clusterBaselines[[as.character(clusterVal)]][["gasMatrix"]])==
    atypTime
  if (sum(ind)==1) {
  ind2 <- clusterBaselines[[as.character(clusterVal)]][["gasMatrix"]][ind,]>atypCutoff &
    !is.na(clusterBaselines[[as.character(clusterVal)]][["gasMatrix"]][ind,])
  if (any(ind2)) {
    ### find the groups in which atypical and identify variables
    atypGroups <- colnames(clusterBaselines[[as.character(clusterVal)]][["gasMatrix"]])[ind2]

    ## set up output
    output <- vector(mode="list",length=length(atypGroups))
    names(output) <- atypGroups
    
    for (j in atypGroups) {
      temp <- clusterBaselines[[as.character(clusterVal)]][["baseline"]][[j]]
      ### Calculate the zscores
      zscores <- (sig[atypTime,names(temp$clusterMeans)]-temp$clusterMeans) /
        temp$clusterSDs
      zscoresSort <- sort(abs(zscores),decreasing=TRUE)

      ##### create matrix with significance information
      sigInfo <- matrix(NA,nrow=0,ncol=4)
      paragraph <- NULL
      
      #### find elements that are "very low"
      ind <- zscores < threshold.limits[1] & !is.na(zscores)
      if (sum(ind)>0)  {
        tempInfo <- cbind(names(zscores)[ind],zscores[ind],
          sig[atypTime,names(zscores)[ind]],rep("very low", sum(ind)))
        sigInfo <- rbind(sigInfo,tempInfo)
        if (!is.matrix(sigInfo)) {
          sigInfo <- matrix(sigInfo,nrow=1,ncol=4)
        }
        rownames(sigInfo) <- NULL
      }

      #### find elements that are "low"
      ind <- zscores < threshold.limits[2] & zscores > threshold.limits[1] &
        !is.na(zscores)
      if (sum(ind)>0)  {
        tempInfo <- cbind(names(zscores)[ind],zscores[ind],
          sig[atypTime,names(zscores)[ind]],rep("low", sum(ind)))
        sigInfo <- rbind(sigInfo,tempInfo)
        if (!is.matrix(sigInfo)) {
          sigInfo <- matrix(sigInfo,nrow=1,ncol=4)
        }
        rownames(sigInfo) <- NULL
      }

      #### find elements that are "marginally low"
      ind <- zscores < threshold.limits[3] & zscores > threshold.limits[2] &
        !is.na(zscores)
      if (sum(ind)>0)  {
        tempInfo <- cbind(names(zscores)[ind],zscores[ind],
          sig[atypTime,names(zscores)[ind]],rep("marginally low", sum(ind)))
        sigInfo <- rbind(sigInfo,tempInfo)
        if (!is.matrix(sigInfo)) {
          sigInfo <- matrix(sigInfo,nrow=1,ncol=4)
        }
        rownames(sigInfo) <- NULL
      }

      #### find elements that are "marginally high"
      ind <- zscores < threshold.limits[5] & zscores > threshold.limits[4] &
        !is.na(zscores)
      if (sum(ind)>0)  {
        tempInfo <- cbind(names(zscores)[ind],zscores[ind],
          sig[atypTime,names(zscores)[ind]],rep("marginally high", sum(ind)))
        sigInfo <- rbind(sigInfo,tempInfo)
        if (!is.matrix(sigInfo)) {
          sigInfo <- matrix(sigInfo,nrow=1,ncol=4)
        }
        rownames(sigInfo) <- NULL
      }

      #### find elements that are "high"
      ind <- zscores < threshold.limits[6] & zscores > threshold.limits[5] &
        !is.na(zscores)
      if (sum(ind)>0)  {
        tempInfo <- cbind(names(zscores)[ind],zscores[ind],
          sig[atypTime,names(zscores)[ind]],rep("high", sum(ind)))
        sigInfo <- rbind(sigInfo,tempInfo)
        if (!is.matrix(sigInfo)) {
          sigInfo <- matrix(sigInfo,nrow=1,ncol=4)
        }
        rownames(sigInfo) <- NULL
      }

      #### find elements that are "very high"
      ind <- zscores > threshold.limits[6] & !is.na(zscores)
      if (sum(ind)>0)  {
        tempInfo <- cbind(names(zscores)[ind],zscores[ind],
          sig[atypTime,names(zscores)[ind]],rep("very high", sum(ind)))
        sigInfo <- rbind(sigInfo,tempInfo)
        if (!is.matrix(sigInfo)) {
          sigInfo <- matrix(sigInfo,nrow=1,ncol=4)
        }
        rownames(sigInfo) <- NULL
      }

      colnames(sigInfo) <- c("Variable","ZScore","Value","Magnitude")

      ## order by abs zscore
      if (nrow(sigInfo)>1) {
        tempz <- as.numeric(sigInfo[,"ZScore"])
        ind.sort <- sort(tempz,index.return=TRUE,decreasing=TRUE)
        sigInfo <- sigInfo[ind.sort$ix,]
        
      }
      
      ## Make the paragraph
      if (nrow(sigInfo)>0) {
        for (i in 1:min(nrow(sigInfo),top)) {
          tvar <- unlist(strsplit(sigInfo[i,"Variable"],split="~"))
          if (tvar[2]=="a" & tvar[3]=="mean") val2 <- "value (mean="
          if (tvar[2]=="a" & tvar[3]=="stdev") val2 <- "variability (SD="
          if (tvar[2]=="b" & tvar[3]=="mean") val2 <- "slope (mean="
          if (tvar[2]=="b" & tvar[3]=="stdev") val2 <- "slope variability (SD="

          sentence <- paste(tvar[1], " ",val2,
            round(as.numeric(sigInfo[i,"Value"]),2),") is ",
            sigInfo[i,"Magnitude"],".",sep="")
          paragraph <- paste(paragraph," ",sentence,sep="")
        }
      }
      ## store
      output[[j]] <- list(sigInfo=sigInfo,paragraph=paragraph)

    } # ends j
    
  } # ends if
  } # ends if
  
  output

}
