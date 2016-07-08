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

rationale <- function(atypTime,sigPath,baselineOutput,atypCutoff,lpv.vec=NULL,
  top=10,threshold.limits=c(-3.0,-2.0,-1.0,1.0,2.0,3.0),SC=FALSE) {

  ##### get the signature matrix
  load(paste(sigPath,"/",substring(atypTime,1,10),".Rdata",sep=""))

  ##### load gas matrix
  load(paste(sigPath,"/gasMatrix.Rdata",sep=""))

  output <- NULL

  ### Find out where atypical
  ind <- rownames(gasMatrix)==atypTime
  ind2 <- gasMatrix[ind,]>atypCutoff & !is.na(gasMatrix[ind,])
  if (any(ind2)) {
    ### find the groups in which atypical and identify variables
    atypGroups <- colnames(gasMatrix)[ind2]

    ## set up output
    output <- vector(mode="list",length=length(atypGroups))
    names(output) <- atypGroups

    for (j in atypGroups) {
      temp <- baselineOutput[[j]]
      tempVars <- intersect(colnames(sig),names(temp$clusterMeans))
      ### Calculate the zscores
      zscores <- (sig[atypTime,tempVars]-temp$clusterMeans[tempVars]) /
        temp$clusterSDs[tempVars]
      zscoresSort <- sort(abs(zscores),decreasing=TRUE)

      ##### create matrix with significance information
      sigInfo <- matrix(NA,nrow=0,ncol=4)
      paragraph <- NULL
      
      # vector to store frequencies associated with the atypical peak values
      PeakFreq = c()

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
        
        if (SC) { # Only do this if spectral coherence results are being considered
          # This replaces ".Valu" in the zscores name to ".Freq" so that it grabs the right column of the signature matrix
          PeakFreq = c(PeakFreq,sig[atypTime,paste(substr(names(zscores)[ind],1,nchar(names(zscores)[ind])-5),".Freq",sep="")])
        }
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
        
        if (SC) { # Only do this if spectral coherence results are being considered
          # This replaces ".Valu" in the zscores name to ".Freq" so that it grabs the right column of the signature matrix
          PeakFreq = c(PeakFreq,sig[atypTime,paste(substr(names(zscores)[ind],1,nchar(names(zscores)[ind])-5),".Freq",sep="")])
        }
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
        
        if (SC) { # Only do this if spectral coherence results are being considered
          # This replaces ".Valu" in the zscores name to ".Freq" so that it grabs the right column of the signature matrix
          PeakFreq = c(PeakFreq,sig[atypTime,paste(substr(names(zscores)[ind],1,nchar(names(zscores)[ind])-5),".Freq",sep="")])
        }
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
        
        if (SC) { # Only do this if spectral coherence results are being considered
          # This replaces ".Valu" in the zscores name to ".Freq" so that it grabs the right column of the signature matrix
          PeakFreq = c(PeakFreq,sig[atypTime,paste(substr(names(zscores)[ind],1,nchar(names(zscores)[ind])-5),".Freq",sep="")])
        }
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
        
        if (SC) { # Only do this if spectral coherence results are being considered
          # This replaces ".Valu" in the zscores name to ".Freq" so that it grabs the right column of the signature matrix
          PeakFreq = c(PeakFreq,sig[atypTime,paste(substr(names(zscores)[ind],1,nchar(names(zscores)[ind])-5),".Freq",sep="")])
        }
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
        
        if (SC) { # Only do this if spectral coherence results are being considered
          # This replaces ".Valu" in the zscores name to ".Freq" so that it grabs the right column of the signature matrix
          PeakFreq = c(PeakFreq,sig[atypTime,paste(substr(names(zscores)[ind],1,nchar(names(zscores)[ind])-5),".Freq",sep="")])
        }
      }

      colnames(sigInfo) <- c("Variable","ZScore","Value","Magnitude")

      ## order by abs zscore
      if (nrow(sigInfo)>1) {
        tempz <- abs(as.numeric(sigInfo[,"ZScore"]))
        ind.sort <- sort(tempz,index.return=TRUE,decreasing=TRUE)
        sigInfo <- sigInfo[ind.sort$ix,]
        
        if (SC) { # Spectral coherence
          PeakFreq = PeakFreq[ind.sort$ix]
        }
      }

      ## Make the paragraph
      if (nrow(sigInfo)>0) {
        for (i in 1:min(nrow(sigInfo),top)) {
          if(!SC) {
            # Brett's
            tvar <- unlist(strsplit(sigInfo[i,"Variable"],split="~"))
            if (tvar[2]=="a" & tvar[3]=="mean") val2 <- "value (mean="
            if (tvar[2]=="a" & tvar[3]=="stdev") val2 <- "variability (SD="
            if (tvar[2]=="b" & tvar[3]=="mean") val2 <- "slope (mean="
            if (tvar[2]=="b" & tvar[3]=="stdev") val2 <- "slope variability (SD="
            
            sentence <- paste(tvar[1], " ",val2,
                              round(as.numeric(sigInfo[i,"Value"]),2),") is ",
                              sigInfo[i,"Magnitude"],".",sep="")
            paragraph <- paste(paragraph," ",sentence,sep="")
          } else {
            # Spectral Coherence
            sentence <- paste(substring(sigInfo[i,"Variable"],1,(nchar(sigInfo[i,"Variable"])-5)), " Peak value = ",
                              round(as.numeric(sigInfo[i,"Value"]),2)," at ",round(PeakFreq[i],1)," Hz"," is ",
                              sigInfo[i,"Magnitude"],".",sep="")
            paragraph <- paste(paragraph," ",sentence,sep="")
          }
          
        }
      }

      ### Get list of variables
      variables <- NULL
      ## loop thru each of the possible GAS scores that are significant
      ## get list of significant variables
      if (nrow(sigInfo)>0) {
        if(!SC) {
          # Brett's
          tempv <- matrix(unlist(strsplit(sigInfo[,"Variable"],split="~")),
                          ncol=3,byrow=TRUE)
          variables <- tempv[,1]
        } else {
          # Spectral COherence
          variables <- substr(sigInfo[,"Variable"],1,nchar(sigInfo[,"Variable"])-5)
        }
      }
      ## store
      if (!SC) {
        # Brett's
        output[[j]] <- list(sigInfo=sigInfo,paragraph=paragraph,variables=variables)
      } else {
        # Spectral Coherence
        output[[j]] <- list(sigInfo=sigInfo,paragraph=paragraph,variables=variables,PeakFreq=PeakFreq)
      }
    } # ends j

  } # ends if

  output

}
