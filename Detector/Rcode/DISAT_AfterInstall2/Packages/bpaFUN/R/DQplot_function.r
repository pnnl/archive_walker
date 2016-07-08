##' This function plots Data Quality results for a day and a pmu
##' @export
##' @param dateVal is the date to plot (ie 2014-03-01)
##'
##' @param pmu is PMU name
##'
##' @param type is either Flag or Filter
##'
##' @param sigPath is the path to the signature folder
##'
##' @return an image plot
##'
##' @author Brett Amidan
##'
##' @examples
##' DQplot()

# dateVal <- "2014-03-08"
# sigPath <- "//olympus/projects/disat/Sigs_PDAT"
# pmu <- "CPJK500A1SA"
# type <- "Filter"

DQplot <- function(dateVal,pmu,type,sigPath) {

  require(bpaFUN)

  ### get the signature data
  load(paste(sigPath,"/",dateVal,".Rdata",sep=""))
  
  ### get selected dq data
  dqData <- get(paste("dq",type,sep=""))

  ### trim data to selected PMU
  cnames <- colnameTranslator(colnames(dqData))

  ind <- cnames["PMU",]==pmu
  dqData <- dqData[,ind]

  if (ncol(dqData)>1) {
    ### plot data
    par(mar=c(5,8,3,.5))
    if (max(dqData) > 0) {
      image(x=1:nrow(dqData),y=1:ncol(dqData),dqData,col=gray(c(1,.7,.4,0)),
        yaxt="n",ylab="",xlab=substring(rownames(dqData)[1],1,10),xaxt="n",
        main=pmu)
    } else {
      image(x=1:nrow(dqData),y=1:ncol(dqData),dqData,col="white",
        yaxt="n",ylab="",xlab=substring(rownames(dqData)[1],1,10),xaxt="n",
        main=pmu)
    }
    axis(side=2,at=1:ncol(dqData),labels=paste(cnames["Bus",ind],".",
      cnames["Variable",ind],sep=""),cex.axis=.75,las=1)
    axis(side=1,at=seq(1,nrow(dqData),by=60),labels=substring(rownames(dqData),
      12,13)[seq(1,nrow(dqData),by=60)],las=1,cex.axis=.75)
  } else {
    print("NO Data Selected")
  }
  invisible()
  
} # ends function
