##' This function explores an individual atypicality PDAT data
##' @export
##' @param filename is full filename of the signature matrix of interest
##'
##' @param clusterDefs is output from the createClusters function
##'
##' @param diststats contains the means and sd for each variable
##'
##' @return plots and rationale
##'
##' @author Brett Amidan
##'
##' @examples
##' exploreAtyp_SP2()

exploreAtyp_SP2 <- function(atypTime,sigPath,outputPath,rdataPath,
  baselineOutput,atypCutoff,analysisTypes,ref.angle,
  ref.angle.label,pe,SC=FALSE) {

  require(bpaFUN)

  #### Setup output folder
  newOutput <- paste(outputPath,"/",substring(atypTime,1,10),
    "/",gsub(":","",substring(atypTime,12,16)),sep="")
  dir.create(newOutput,showWarnings=FALSE,recursive=TRUE)
  
  #### Determine the variables that are significant and create the rationale
  Rationale <- rationale(atypTime=atypTime,sigPath=sigPath,
    baselineOutput=baselineOutput,atypCutoff=atypCutoff,
    threshold.limits=c(-3.0,-2.25,-1.5,1.5,2.25,3.0),SC=SC)
  save(list="Rationale",file=paste(newOutput,"/Rationale.Rdata",sep=""))

  ## determine if perf env plots are needed
  vars <- NULL
  for (i in 1:length(Rationale)) vars <- c(vars,Rationale[[i]]$variables)
  vars <- unique(vars)
  indy <- !is.element(paste(vars,".jpg",sep=""),dir(newOutput))
  ## check if raw data exists
  tempDir <- paste(rdataPath,"/",substring(atypTime,1,4),"/",
    substring(atypTime,6,7),"/",substring(atypTime,9,10),sep="")
  dirContent <- dir(tempDir,full.names=TRUE)
  if (length(dirContent)>0) {
    ind5 <- grepl(paste(substring(atypTime,12,13),substring(atypTime,15,16),
      sep=""),dirContent)
    if (sum(ind5)>0) {
      data.exists <- TRUE
    } else {
      data.exists <- FALSE
    }
  } else {
    data.exists <- FALSE
  }
  
  if (sum(indy)>0 & data.exists) {
    #### Make the Performance Envelope Plots
    plotPerfEnv2(atypTime=atypTime,sigPath=sigPath,outputPath=newOutput,
      rdataPath=rdataPath,Rationale=Rationale,top=15,PEtime=30,
      ref.angle=ref.angle,ref.angle.label=ref.angle.label,pe=pe,SC=SC)
    gc(reset=TRUE)
  }
  
  
  if(!SC) {
    # Brett's
    #### Make a Zoomed in Atypicality Plot
    load(paste(sigPath,"/gasMatrix.Rdata",sep=""))
    indy <- c(1:nrow(gasMatrix))[rownames(gasMatrix)==atypTime]
    if (length(indy)==1) {
      rows <- seq(indy-60,indy+60,by=1)
      ## make plot +/- 60 minutes for each significant GAS score
      for (i in 1:length(Rationale)) {
        jpeg(file=paste(newOutput,"/",names(Rationale)[i],".jpg",sep=""),
             width=7,height=7,units="in",res=150)
        plot(as.POSIXct(rownames(gasMatrix)[rows]),
             gasMatrix[rows,names(Rationale)[i]],ylab="Atypicality Score",
             xlab=substring(atypTime,1,10),type="l",
             ylim=c(0,max(gasMatrix[rows,names(Rationale)[i]],na.rm=TRUE)),
             main=analysisTypes[[names(Rationale)[i]]][["title"]],col="blue",lwd=3)
        abline(v=as.POSIXct(atypTime),col="red",lty=2)
        dev.off()
      } # ends i
    } # ends if
  } else {
    # Spectral Coherence
    #### Make maps showing spectral coherence results with the atypicality score plot below
    plotSCmaps(newOutput=newOutput, atypTime=atypTime, sigPath=sigPath,
      Rationale=Rationale, ReplaceOld=FALSE)
  }
  
  invisible()
  
} # ends function
