##'  This function calculates data quality baseline
##' @export
##' @param sigPath  is path where signatures will be stored
##'
##' @param outputPath is the path where atypical output will be stored
##'
##' @return
##'
##' @author Scott Cooley / Brett Amidan
##'
##' @examples
##' baselineDQ()

baselineDQ <- function(sigPath,outputPath,process.option="all.R.objects",
       process.year=NA,process.month=NA) {

require(bpaFUN)

#### Needed function
DQ.Processing.FUN.SKC <- function(X) {
  hr.vec <- substr(rownames(X),1,13)
  table(hr.vec)

  out.X <- apply(X,2,function(x) {tapply(x,hr.vec,mean)})
  out.X
}

### If we just want to process one month's worth of data ###
if (process.option=="one.month") {
yearProc <- process.year
monthProc <- process.month

yr.mo <- paste(yearProc,monthProc,sep="-")
yr.mo

pmu.data.objs <- dir(sigPath)[intersect(grep(".Rdata",dir(sigPath)),
       grep(paste(yearProc,monthProc,sep="-"),dir(sigPath)))]
}


### If we want to process one year's worth of data ###
### If we just want to process one month's worth of data ###
if (process.option=="one.year") {
yearProc <- "2014"

pmu.data.objs <- dir(sigPath)[intersect(grep(".Rdata",dir(sigPath)),
       grep(paste(yearProc,"-",sep=""),dir(sigPath)))]
}


### If we want to process all the .Rdata objects in a specified data inputs folder ###
if (process.option=="all.R.objects") {
pmu.data.objs <- dir(sigPath,pattern="20")
}

if (!is.element(process.option,c("all.R.objects","one.year","one.month"))) {
  stop('An valid entry for process.option must be specified')
}

### Initiate matrices to catch results from processing ###
out.mat.Flag <- out.mat.Filter <- NULL   

### Loop through the specified days and do processing ###
for (k in 1:length(pmu.data.objs)) {
  load(paste(sigPath,"/",pmu.data.objs[k],sep=""))

  if (exists("dqFlag") & exists("dqFilter")) {

    tempFlag <- DQ.Processing.FUN.SKC(dqFlag)
    tempFilter <- DQ.Processing.FUN.SKC(dqFilter)

    out.mat.Flag <- smartRbindMat(out.mat.Flag,tempFlag)
    out.mat.Filter <- smartRbindMat(out.mat.Filter,tempFilter)
    rm(list=c("dqFlag","dqFilter"))
  } # ends if

}   ### end for k loop ###

dir.create(paste(outputPath,"/DataQuality",sep=""),showWarnings=FALSE)

## store by month
uniqYM <- unique(substring(rownames(out.mat.Flag),1,7))
for (i in uniqYM) {
  indy <- substring(rownames(out.mat.Flag),1,7)==i
  flagSum <- out.mat.Flag[indy,]
  indy2 <- substring(rownames(out.mat.Filter),1,7)==i
  filterSum <- out.mat.Filter[indy2,]
  ### store
  save(list=c("flagSum","filterSum"),file=paste(outputPath,"/DataQuality/",
    i,".Rdata",sep=""))
} # ends i

invisible()

}   ### end function
