##' This function 
##' @export
##' @param rawPath path of raw data i.e. "J:/"
##' @param sigPath path of signature data i.e. "D:/DISAT_PDAT/Sigs_PDAT"
##'
##' @return 
##'
##' @author Lucas Tate / Brett Amidan
##'
##' @examples
##' LeftToProcess_PDAT()

leftToProcessPDAT <- function(rawPath, sigPath) {
  
  #Declare return variable that will contain list of day paths to be processed
  daysToProcessPath <- NULL
  
  #Identify last processed file from signatures folder
  lastSigFile <- tail(dir(sigPath, pattern="20"), n = 1)
  
  #Check to make sure signatures exist in sigPath
  if(length(lastSigFile) !=0 ){
    
    #Find year of last signature file
    lastSigFileYear <- substring(lastSigFile, 1, 4)
    
    #Identify year folders in rawPath directory
    rawPathDir <- dir(rawPath)
    
    #Identify year folders in rawPath after latest signature file year
    yearsAfterSig <- rawPathDir[rawPathDir > lastSigFileYear]
    
    #Add all days in years after latest signature file to be processed
    if(length(yearsAfterSig) > 0) {
      for (i in 1:length(yearsAfterSig)) {
        daysToProcessPath <- c(daysToProcessPath, dir(paste(rawPath, yearsAfterSig[i], sep = "/"), full.names = TRUE))
      }
    }
    
    #Find day of last signature file
    lastSigFileDay <- substring(lastSigFile, 3, 10)
    lastSigFileDay <- gsub("-", "", lastSigFileDay)
    
    #Define path to sig file year in raw files
    rawPathSigYear <- paste(rawPath, lastSigFileYear, sep = "/")
    
    #Identify day folders in path to raw files for sig year
    rawPathSigYearDir <- dir(rawPathSigYear)
    
    #Identify day folders in path to raw files for sig year more current than sig day
    daysAfterSig <- rawPathSigYearDir[rawPathSigYearDir > lastSigFileDay]
    
    #If extra days beyond sig file, add them to be processed
    if(length(daysAfterSig) > 0) {
      for (i in 1:length(daysAfterSig)) {
        daysToProcessPath <- c(daysToProcessPath, paste(rawPathSigYear, daysAfterSig[i], sep = "/"))
      }
    }
    
    ##Identify last minute processed with signature file
    load(paste(sigPath,"/", lastSigFile,sep=""))
    lastMinute <- gsub(":", "", substring(rownames(csum)[nrow(csum)], 12), fixed = TRUE)
    
    #Identify raw minutes in day of last sig file
    rawMinutes <- dir(paste(rawPathSigYear, lastSigFileDay, sep = "/"))
    rawMinutes <- substr(rawMinutes, nchar(rawMinutes) - 10, nchar(rawMinutes) - 7)
    
    #Identify minutes beyond latest row in sig file
    extraMinutes <- rawMinutes[rawMinutes > lastMinute]
    lastSigFileDayPath <- paste(rawPathSigYear, lastSigFileDay,sep = "/")
    
    #If extra minutes, add day of last sig file to be reprocessed
    if (length(extraMinutes) != 0) {
      daysToProcessPath <- c(daysToProcessPath, lastSigFileDayPath  )
    }
    
  } else {
    #If there are no signatures, process all raw data for each year/day
    
    #Find paths for each year of raw PDAT data
    yearsToProcess <- dir(rawPath)
    yearsToProcessPath <- paste(rawPath, dir(rawPath), sep = "/")
    
    
    #Find paths for each day of raw PDAT data
    for (i in 1:length(yearsToProcess)) {
      daysToProcessPath <- c(daysToProcessPath, paste(yearsToProcessPath[i], dir(yearsToProcessPath[i]), sep = "/"))
    }
    
  }
  return(daysToProcessPath)
}