##' This function plots the performance envelopes
##' @export
##' @param the.data is the matrix of data
##'
##' @return plots performance envelopes
##'
##' @author Brett Amidan
##'
##' @examples
##' plotPerfEnv2()

plotPerfEnv2 <- function(atypTime,sigPath,outputPath,rdataPath,Rationale,top=10,
  PEtime=30,ref.angle,ref.angle.label,pe,SC=FALSE) {
  
  # These are needed for plotting data used for spectral coherence
  require(signal)
  diff = function(vec) vec[2:length(vec)]-vec[1:length(vec)-1]

  ## get PE for month of interest
  PE <- pe[[as.numeric(substring(atypTime,6,7))]]
  
  variables <- NULL
  for (i in 1:length(Rationale)) {
    variables <- c(variables,Rationale[[i]]$variables[1:min(c(top,
      length(Rationale[[i]]$variables)))])
  }
  variables <- unique(variables)
  
  if (length(variables)>0) {
    mpTime <- (PEtime-10)/2
    tpi <- as.character(seq(as.POSIXct(atypTime)-60*(mpTime+5),
      as.POSIXct(atypTime)+60*(mpTime+5),by=60))
    ### collect the middle 11 minutes of data
    fulldata <- NULL
    for (k in mpTime:(mpTime+12)) {
      tempPath <- paste(rdataPath,"/",substring(tpi[k],1,4),"/",
        substring(tpi[k],6,7),"/",substring(tpi[k],9,10),sep="")
      tempList <- dir(tempPath)
      ind5 <- grepl(paste(substring(tpi[k],3,4),substring(tpi[k],6,7),
        substring(tpi[k],9,10),substring(tpi[k],12,13),substring(tpi[k],15,16),
        sep=""),tempList) | grepl(paste(substring(tpi[k],3,4),substring(tpi[k],6,7),
        substring(tpi[k],9,10),"_",substring(tpi[k],12,13),substring(tpi[k],15,16),
        sep=""),tempList)
      if (sum(ind5)==1) {
        try(load(paste(tempPath,"/",tempList[ind5],sep="")),silent=TRUE)
        if (is.element("data",ls())) {
        data <- DQfilter(data/10000,phasors=NULL)$data
        ### add angle differences if needed
        ind6 <- is.element(substring(variables,nchar(variables)-1),
          c("SA","MA","NA"))
        if (sum(ind6)>0) {
          ## loop thru each one and create the angle difference
          adddata <- NULL
          temp <- NULL
          for (ang in 1:sum(ind6)) {
            ## find the original angle
            counter <- nchar(variables[ind6][ang])-3
            indy <- is.element(substring(colnames(data),1,counter),
              substring(variables[ind6][ang],1,counter)) 

            for (kk in 1:length(ref.angle)) {
              if (is.element(substring(variables[ind6][ang],
                nchar(variables[ind6][ang])-1),ref.angle.label[kk])) {
                ### calculate the angle difference
                temp <- CalcAngDiff(Ang1=data[,indy],Ang2=data[,ref.angle[kk]])
              }
            } # ends kk
#            if (is.element(substring(variables[ind6][ang],nchar(variables[ind6][ang])-1),"SA")) {
#              ## calculate the angle difference
#              temp <- CalcAngDiff(Ang1=data[,indy],Ang2=data[,"MLN1.NTHB.VA"])
#            } # ends  if
#            if (is.element(substring(variables[ind6][ang],nchar(variables[ind6][ang])-1),"MA")) {
#              ## calculate the angle difference
#              temp <- CalcAngDiff(Ang1=data[,indy],Ang2=data[,"MAR1.NTHB.VA"])
#            } # ends  if
#            if (is.element(substring(variables[ind6][ang],nchar(variables[ind6][ang])-1),"NA")) {
#              ## calculate the angle difference
#              temp <- CalcAngDiff(Ang1=data[,indy],Ang2=data[,"MONR.NTHB.VA"])
#            } # ends  if
            adddata <- cbind(adddata,temp)
          } # ends ang loop
          colnames(adddata) <- variables[ind6]
          if (length(variables)>1) {
            data <- cbind(data,adddata)[,variables]
          } else {
            rnames <- rownames(data)
            data <- matrix(cbind(data,adddata)[,variables],ncol=1)
            rownames(data) <- rnames
            colnames(data) <- variables
          }
        } # ends if
        if (length(variables)==1) {
          data1 <- matrix(data[,variables],ncol=1)
          rownames(data1) <- rownames(data)
          colnames(data1) <- variables
          fulldata <- rbind(fulldata,data1)
        } else {
          fulldata <- rbind(fulldata[,variables],data[,variables])
        }
        rm(list="data")
        }
      } # ends if
    } # ends k

    ### remove any columns with all NAs
    nonasum <- colSums(!is.na(fulldata)) > 0
    if (sum(nonasum)==1) {
      rn <- rownames(fulldata)
      fulldata <- matrix(fulldata[,nonasum],ncol=1)
      colnames(fulldata) <- variables[nonasum]
      rownames(fulldata) <- rn
    } else {
      fulldata <- fulldata[,nonasum]
    }
    variables <- variables[nonasum]
    variables <- variables[!is.na(variables)]
    
    if (length(variables)>0) {
    ### figure time period of interest for the image plot
    ### get ten minutes around atypicality plus 1/2 of PEtime-10 on each side
    ## get signature matrix
    dateVec <- unique(substring(tpi,1,10))
    for (j in 1:length(dateVec)) {
      load(paste(sigPath,"/",dateVec[j],".Rdata",sep=""))
      if (j==1) sig2 <- sig
      if (j>1) {
        ind5 <- intersect(colnames(sig2),colnames(sig))
        sig2 <- rbind(sig2[,ind5],sig[,ind5])
      }
    }
    rows <- intersect(substring(tpi,1,16),rownames(sig2))
    sig2 <- sig2[rows,]

    ## ONLY keep variables that exist in PE
    ind1 <- is.element(variables,names(PE))
    variables <- variables[ind1]
    if (length(variables)>0) {
    ### loop thru each variable and make a perf env plot
    for (j in 1:length(variables)) {
      if(!SC) {
        # Brett's
        ## get the PE for variable
        indpe <- PE[[variables[j]]]
        ## trim the PE for the time period
        count2 <- indpe[["count"]][,substring(tpi,12,16)]
        ymidpts <- indpe[["ymidpts"]]
        ## fix duplicates
        inddup <- duplicated(ymidpts)
        ymidpts[inddup] <- ymidpts[inddup]-.005
        inddup <- duplicated(ymidpts)
        ymidpts[inddup] <- ymidpts[inddup]-.0025
        
        xlimits <- colnames(count2)
        minval <- min(indpe$ylimits,sig2[,paste(variables[j],"~a~mean",sep="")],
                      na.rm=TRUE)
        maxval <- max(indpe$ylimits,sig2[,paste(variables[j],"~a~mean",sep="")],
                      na.rm=TRUE)
        
        jpeg(filename=paste(outputPath,"/",variables[j],".jpg",sep=""),
             width=7,height=9,units="in",res=150)
        par(mfrow=c(2,1))
        ## create the image plot
        image(x=1:ncol(count2),y=rev(ymidpts),t(count2)[,rev(1:nrow(count2))],
              xaxt="n",yaxt="n",ylim=c(minval,maxval),col=gray(seq(1,.3,by=-.05)),
              main=variables[j],xlab=substring(atypTime,1,10),ylab="")
        axis(side=2)
        axis(side=1,at=1:ncol(count2),labels=colnames(count2))
        
        # data(phasors)
        ### Add first part as Signature a means
        points(1:mpTime,sig2[1:mpTime,paste(variables[j],"~a~mean",
                                            sep="")],col="orange",pch=16)
        ### Add last part as Signature a means
        points((nrow(sig2)-mpTime+2):nrow(sig2),
               sig2[(nrow(sig2)-mpTime+2):nrow(sig2),paste(variables[j],"~a~mean",
                                                           sep="")],col="orange",pch=16)
        
        ### Add the 60 Hz data for the middle 11 minutes
        points(mpTime+as.numeric(as.POSIXct(rownames(fulldata))-
                                   as.POSIXct(rownames(fulldata)[1]))/60,fulldata[,variables[j]],
               col="orange",lty=1,type="l",lwd=3)
        
        ### plot atypical minute + and - 1 minute (3 minutes total)
        par(mar=c(4,4.1,1,2.1))
        indy2 <- as.POSIXct(rownames(fulldata))>(as.POSIXct(atypTime)-120) &
          as.POSIXct(rownames(fulldata)) < (as.POSIXct(atypTime)+120) &
          !is.na(fulldata[,variables[j]])
        if (sum(indy2)>1) {
          plot(as.POSIXct(rownames(fulldata))[indy2],fulldata[indy2,variables[j]],
               main="",ylab="",xlab="",pch=16,col="orange",cex=.5)
        }
        ### save plot to file
        dev.off()
      } else {
        # Spectral Coherence
        jpeg(filename=paste(outputPath,"/",variables[j],".jpg",sep=""),
             width=7,height=7,units="in",res=150)
        
        ### plot atypical minute + and - 1 minute (3 minutes total)
        par(mar=c(5,5.1,3,2.1))
        indy2 <- as.POSIXct(rownames(fulldata))>(as.POSIXct(atypTime)-120) &
          as.POSIXct(rownames(fulldata)) < (as.POSIXct(atypTime)+120) &
          !is.na(fulldata[,variables[j]])
        if (sum(indy2)>1) {
          tPlot = as.POSIXct(rownames(fulldata))[indy2]
          tPlot = tPlot[1:(length(tPlot)-1)]
          
          PatchIdx = NULL
          Keepers = which(indy2==TRUE)
          Keepers = Keepers[1]:Keepers[length(Keepers)]
          BadMiddle = intersect(Keepers,which(indy2==FALSE))
          if (length(BadMiddle) > 0) {
            DiffBadMiddle = diff(BadMiddle) # This helps locate the groups of NAs
            StartIdx = BadMiddle[c(0,which(DiffBadMiddle > 1))+1]          # Finds the start of each group of NAs
            EndIdx = BadMiddle[c(which(DiffBadMiddle > 1),length(BadMiddle))]  # Finds the end of each group of NAs
            StartIdx = StartIdx - Keepers[1] + 1
            EndIdx = EndIdx - Keepers[1] + 1
            BadDur = c(0,EndIdx-StartIdx+1)
            
            PatchIdx = rep(0,length(StartIdx))
            for (idx in 1:length(StartIdx)) {
              PatchIdx[idx] = StartIdx[idx] - sum(BadDur[1:idx]) - 1
            }
          }
          
          yy = fulldata[indy2,variables[j]]
          yy = diff(unwrap(yy*pi/180))*60/(2*pi)
          
          if (length(PatchIdx) > 0) {
            for (idx in 1:length(PatchIdx)) {
              yy[PatchIdx[idx]] = mean(c(yy[PatchIdx[idx]-1],yy[PatchIdx[idx]+1]))
            }
          }
          
          yy = yy - yy[1]
          # High-pass filter
          yy = filter(c(0.994791237659377,-0.994791237659377),c(1,-0.989582475318754),yy)
          
          # Get frequency to add to the plot
          
          plot(tPlot,yy,xaxt="n",
               main=paste(variables[j],"\n Peak Frequency: ",round(Rationale$Valu.Peak$PeakFreq[paste(variables[j],".Freq",sep="")],1)," Hz",sep=""),ylab="Deviation from 60 Hz \n (Derived from Voltage Angle)",xlab=substr(tPlot[1],1,10),pch=16,col="orange",cex=.5)
          axis.POSIXct(1,tPlot,,format="%H:%M")
          if (length(PatchIdx) > 0) {
            text(par("usr")[1],par("usr")[3],label="WARNING: Plot is inaccurate near missing data.",adj = c(0,0))
          }
        }
        ### save plot to file
        dev.off()
      }
      
    } # ends j loop
    } # ends if
    } # ends if
  } # ends if


  invisible()
} # ends function
