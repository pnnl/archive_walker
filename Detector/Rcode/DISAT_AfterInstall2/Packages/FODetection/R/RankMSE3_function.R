# Goes through a month's worth of MSE files (located in each month's DailyMSE file) and sorts the MSEs from each day
#   MSEpath = "C:\\Users\\foll154\\Documents\\Spectral Coherence FY15\\ApplicationToPMUdata\\Output\\Output2\\2014\\03\\DailyMSE"

RankMSE3 = function(MSEpath,Alg) {
  if (!file.exists(MSEpath)) {
    return(NULL)
  }
  
  # Get a list of all MSE files for the month. Each file corresponds to one day
  MSEfiles = list.files(path = MSEpath)
  
  # Keep only the MSE files for the algorithm of interest
  if (Alg=="Per") {
    MSEfiles = MSEfiles[grepl("Per",MSEfiles)]
  } else {
    MSEfiles = MSEfiles[grepl("SC",MSEfiles)]
  }
  
  MSEpeakAll = data.frame()  # Holds the MSEs based on the size of the peak
  MSEdurAll = data.frame()   # Holds the MSEs based on duration
  for (ThisMSEfile in MSEfiles) { # For each of the MSE files (one file per day)
    # Load the MSE file
    load(paste(MSEpath,"\\",ThisMSEfile,sep=""))
    
    # Add the current day's MSE to the month's list
    temp = MSE$MSEpeak
    if (length(temp) > 0) {
      temp["Duration"] = sum(as.numeric(difftime(temp$EndTime, temp$StartTime, units="mins") + 1))
      MSEpeakAll = rbind(MSEpeakAll,temp)
      
      MSEdurAll = rbind(MSEdurAll,MSE$MSEdur)
    }
  }
  
  if (length(MSEpeakAll) > 0) {
    # These are the most significant events based on peak - now sort by evidence
    MSEpeak = MSEpeakAll[with(MSEpeakAll, order(-PeakEvidence)),]
    
    # These are the most significant events based on duration - now sort by duration
    MSEdur = MSEdurAll[with(MSEdurAll, order(-Duration, -PeakEvidence)),]
  } else {
    # Nothing to return
    MSEpeak = NULL
    MSEdur = NULL
  }
  
  list(MSEpeak=MSEpeak, MSEdur=MSEdur)
}