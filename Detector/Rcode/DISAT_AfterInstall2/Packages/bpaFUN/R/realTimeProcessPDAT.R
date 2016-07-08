##' This function 
##' @export
##' @param rawPath path of raw data i.e. "J:/"
##' @param sigPath path of signature data i.e. "D:/DISAT_PDAT/Sigs_PDAT"
##' @param rdataPath path of rdata data i.e. "D:/DISAT_PDAT/Rdata_PDAT"
##' @param outputPath path of output data data i.e. "D:/DISAT_PDAT/Output_PDAT"
##' @param sleepTime duration of sleep in seconds
##' @param numSleeps number of sleeps before stopping
##'
##' @return 
##'
##' @author Lucas Tate / Brett Amidan
##'
##' @examples
##' realTimeProcess()

realTimeProcessPDAT <- function(rawPath,sigPath,sigPathSC,rdataPath,outputPath,outputPathSC,
  sleepTime=300, numSleeps=24,ref.angle,ref.angle.label,vars,atypCutoff,atypCutoffSC,
  analysisTypes,analysisTypesSC,numjobs=20) {

  count <- 0
  while(count < numSleeps) {
    #Return list of raw file folders that still need to be processed
    foldersToProcess <- sort(leftToProcessPDAT(rawPath=rawPath, sigPath=sigPath))
    
    if (is.null(foldersToProcess)) {
      Sys.sleep(sleepTime)
      count <- count + 1
      print(paste("Sleep = ",round(count/numSleeps*100,0),"%",sep=""))
    } else {
      timer <- proc.time()[3]
      print(paste("Processing data from ",length(foldersToProcess)," days",
        sep=""))
        
      #Read in raw files and create Rdata files
      readPDAT_MP(folders=foldersToProcess,rdata.path=rdataPath,num.jobs=numjobs,newOnly=TRUE)
      #print("read")
      
      rdataPaths <- paste(rdataPath,"/20",substr(foldersToProcess,nchar(foldersToProcess)-5,nchar(foldersToProcess)-4),
                          "/",substr(foldersToProcess,nchar(foldersToProcess)-3, nchar(foldersToProcess)-2),"/",
                          substr(foldersToProcess,nchar(foldersToProcess)-1, nchar(foldersToProcess)),sep="")

      ## clean up any bad files (usually incomplete minutes cause this)
      for (i in 1:length(rdataPaths)) {
        tempdir <- dir(rdataPaths[i])
        ## find common string size
        commonss <- as.numeric(names(rev(sort(table(nchar(tempdir)))))[1])
        ## remove any that aren't the same size
        indy <- nchar(tempdir) != commonss
        if (sum(indy)>0) {
#          print(rdataPaths[i])
          for (j in 1:sum(indy)) {
            file.remove(paste(rdataPaths[i],"/",tempdir[indy][j],sep=""))
          } # ends j
        } # ends if
      } # ends i

      ### load baseline info
      load(paste(sigPath,"/univBaseline.Rdata",sep=""))
      ### load performance envelope info
      load(paste(sigPath,"/perfEnvelope.Rdata",sep=""))
      
      #Create Signature Files
      rtSigCalc(Rdata.paths=rdataPaths,Sig.path=sigPath,ref.angle=ref.angle,
                ref.angle.label=ref.angle.label,vars=vars,
                skip=60,sample.rate=60,do.minute=TRUE,
                do.linear=FALSE,numjobs=numjobs)

      ### Update Cluster / Atypicality Scores
      rtCalculations(sigPath=sigPath,outputPath=outputPath,rdataPath=rdataPath,
               atypCutoff=atypCutoff,analysisTypes=analysisTypes,
               baselineOutput=univBaseline$baseline,ref.angle=ref.angle,
               ref.angle.label=ref.angle.label,pe=pe,SC=FALSE)




      ### load baseline info - Spectral Coherence
      load(paste(sigPathSC,"/univBaseline.Rdata",sep=""))
      ### load performance envelope info
      load(paste(sigPathSC,"/perfEnvelope.Rdata",sep=""))

      ### Create spec coherence signature files
#      rtSigCalc(Rdata.paths=rdataPaths,Sig.path=sigPathSC,
#                sample.rate=60,numjobs=numjobs,SC=TRUE)

      ### Update Cluster / Atypicality Scores for Spectral Coherence
#      rtCalculations(sigPath=sigPathSC,outputPath=outputPathSC,rdataPath=rdataPath,
#        atypCutoff=atypCutoffSC,analysisTypes=analysisTypesSC,
#        baselineOutput=univBaseline$baseline,ref.angle=NULL,
#        ref.angle.label=NULL,pe=pe,SC=TRUE)



      print(paste("New files from ",foldersToProcess," processed in ",
        round((proc.time()[3]-timer)/60,1), " minutes",sep=""))
        
      count <- 0
    }
  }
}
