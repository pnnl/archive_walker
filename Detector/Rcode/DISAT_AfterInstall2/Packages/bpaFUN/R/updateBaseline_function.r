##' This function updates the baseline information for analyses
##' @export
##' @param sigPath is the path to the signature data
##'
##' @param clusterOutput 
##'
##' @param analysisTypes
##'
##' @return baseline output - pca loadings, mean/sd, gamma parameters, performance envelopes
##'
##' @author Brett Amidan
##'
##' @examples
##' updateBaseline()

#install.packages("bpaFUN", repos="http://martingale.pnl.gov/computing/repos")
#sigPath <- "G:/Sigs"
#rdataPath <- "G:/Rdata"
#outputPath <- "G:/Output"
#load("G:/Sigs/clusterOutput.RData")
#data(analysisTypes)
#data(phasors)
#atypCutoff=20
#numjobs=9

updateBaseline <- function(sigPath,clusterOutput,outputPath,rdataPath,
  analysisTypes,atypCutoff=20,phasors,ref.angle,ref.angle.label,numjobs=1,SC=FALSE) {

  require(snowfall)
  require(bpaFUN)
  timer <- proc.time()[3]

  ### Determine all possible clusters
  possClust <- sort(unique(clusterOutput$membership))

  ################### Universal baseline #########################
  univBaseline <- updateBaseline_SP(clusterVal=possClust,
    clusterOutput=clusterOutput,analysisTypes=analysisTypes,
    sigPath=sigPath,daysToUse=150,random=TRUE,method="U",SC=SC)
  save(list="univBaseline",file=paste(sigPath,"/univBaseline.Rdata",sep=""))
  print(paste("Universal baseline created (Total time elapsed = ",
    round((proc.time()[3]-timer)/3600,2)," hours)",sep=""))
  gc(reset=TRUE)

  gasMatrix <- updateGAS(clusterOutput=clusterOutput,
    baselineOutput=univBaseline$baseline,sigPath=sigPath,
    daysToUse=730,numjobs=numjobs)
  save(list="gasMatrix",file=paste(sigPath,"/gasMatrix.Rdata",sep=""))
  print(paste("Universal atypicality scores calculated (Total time elapsed = ",
    round((proc.time()[3]-timer)/3600,2)," hours)",sep=""))
  gc(reset=TRUE)

  ################### Cluster baselines ##########################
  ### make this a list for multiprocessing
  clustList <- as.list(possClust)

  if (length(clustList) > 0) {
    ## create the summaries
#    sfInit(parallel=TRUE,cpus=numjobs)

    ## run the single processor assignClusters function multiprocessed
#    output <- sfLapply(clustList,updateBaseline_SP,clusterOutput=clusterOutput,
#      analysisTypes=analysisTypes,sigPath=sigPath,daysToUse=730)

#    sfStop()
#    names(output) <- as.character(possClust)
#    clusterBaselines <- output
#    save(list="clusterBaselines",file=paste(sigPath,"/clusterBaselines.Rdata",sep=""))

    ###  Combine all gas scores into one matrix
#    gasMatrix <- NULL
#    for (i in 1:length(output)) {
#      temp <- output[[i]]$gasMatrix
#      if (nrow(temp)<15) {
#        temp[1:nrow(temp),1:ncol(temp)] <- 25
#      }
#      gasMatrix <- rbind(gasMatrix,temp)
#    } # ends i
    ## sort by time (rownames)
#    ind.sort <- sort(rownames(gasMatrix))
#    gasMatrix <- gasMatrix[ind.sort,]

    ## write out gas matrix
#    save(list="gasMatrix",file=paste(sigPath,"/gasMatrix_Cluster.Rdata",sep=""))
#    print(paste("Cluster baselines created (Total time elapsed = ",
#      round((proc.time()[3]-timer)/3600,2)," hours)",sep=""))
#    gc(reset=TRUE)


    #### Perform GAS based on last 2 months of data
#    last2Months <- updateBaseline_SP(clusterVal=possClust,
#      clusterOutput=clusterOutput,analysisTypes=analysisTypes,
#      sigPath=sigPath,daysToUse=60)
    
    #### Calculate GAS based on last 2 months of data
#    last2Months$gasMatrix <- updateGAS(clusterOutput=clusterOutput,
#      baselineOutput=last2Months$baseline,sigPath=sigPath,daysToUse=730,
#      numjobs=numjobs)
#    gasMatrix <- last2Months$gasMatrix
#    save(list="gasMatrix",file=paste(sigPath,"/gasMatrix_2M.Rdata",sep=""))
#    save(list="last2Months",file=paste(sigPath,"/last2Months.Rdata",sep=""))
#    print(paste("Last 2 Months baseline created (Total time elapsed = ",
#      round((proc.time()[3]-timer)/3600,2)," hours)",sep=""))
#    gc(reset=TRUE)

    #### Calculate performance envelopes for each Variable / by month
    pe <- createPerfEnvelope(sigPath=sigPath,baselineOutput=univBaseline,
      daysToUse=730,SC=SC)
    save(list="pe",file=paste(sigPath,"/perfEnvelope.Rdata",sep=""))
    print(paste("Performance envelopes created (Total time elapsed = ",
      round((proc.time()[3]-timer)/3600,2)," hours)",sep=""))
    gc(reset=TRUE)

    #### Explore Atypicalities
    exploreAtyp(sigPath=sigPath,outputPath=outputPath,rdataPath=rdataPath,
      atypCutoff=atypCutoff,baselineOutput=univBaseline$baseline,
      analysisTypes=analysisTypes,phasors=phasors,ref.angle=ref.angle,
      ref.angle.label=ref.angle.label,pe=pe,numjobs=numjobs,SC=SC)
    print(paste("Atypicality plots produced (Total time elapsed = ",
      round((proc.time()[3]-timer)/3600,2)," hours)",sep=""))
    gc(reset=TRUE)

  } else {
    print("No Baseline Created")
  } # ends if/else

  ###########################################################
  ####  Create Output
  
  print(paste("Baselines updated (Total time elapsed = ",
    round((proc.time()[3]-timer)/60,2)," minutes)",sep=""))

  list(univBaseline=univBaseline)

} # ends function

  