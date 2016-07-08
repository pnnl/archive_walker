##' This function explores an individual atypicality
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
##' exploreAtyp_SP()

exploreAtyp_SP <- function(atypTime,sigPath,outputPath,rdataPath,
  baselineOutput,atypCutoff,analysisTypes,phasors,ref.angle,
  ref.angle.label,pe) {

  require(bpaFUN)

  #### Setup output folder
  newOutput <- paste(outputPath,"/",substring(atypTime,1,10),
    "/",gsub(":","",substring(atypTime,12,16)),sep="")
  dir.create(newOutput,showWarnings=FALSE,recursive=TRUE)
  
  #### Determine the variables that are significant and create the rationale
  Rationale <- rationale(atypTime=atypTime,sigPath=sigPath,
    baselineOutput=baselineOutput,atypCutoff=atypCutoff,
    threshold.limits=c(-3.0,-2.25,-1.5,1.5,2.25,3.0))
  save(list="Rationale",file=paste(newOutput,"/Rationale.Rdata",sep=""))

  ## determine if perf env plots are needed
  vars <- NULL
  for (i in 1:length(Rationale)) vars <- c(vars,Rationale[[i]]$variables)
  vars <- unique(vars)
  indy <- !is.element(paste(vars,".jpg",sep=""),dir(newOutput))
  
  if (sum(indy)>0) {
    #### Make the Performance Envelope Plots
    plotPerfEnv(atypTime=atypTime,sigPath=sigPath,outputPath=newOutput,
      rdataPath=rdataPath,Rationale=Rationale,top=15,PEtime=30,phasors=phasors,
      ref.angle=ref.angle,ref.angle.label=ref.angle.label,pe=pe)
    gc(reset=TRUE)
  }
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
  
  invisible()
  
} # ends function
