GetFOatFreq = function(freq,DailyDet,DailyDetFreq) {
  # Find the peak frequencies accompanying DailyDet 
  # (DailyDetFreq has the most recent, causing problems when searching for FOs on the Inspect Oscillation page based on 
  # frequencies on the Daily Overview table)
  # I took this block of code from RankDailyDet2_function, so it could have been written in a simpler way for this application
  DailyDet2 = data.frame()
  for (DetIdx in seq_along(DailyDet)) {
    Peaks = DailyDet[[DetIdx]][,"PeakEvidence"]
    temp = DailyDet[[DetIdx]][Peaks==max(Peaks), c("Freq","PeakTime","PeakEvidence","PeakChan")]
    DailyDet2 = rbind(DailyDet2, temp)
  }
  if (length(DailyDet2)>0) {
    DailyDetFreq = DailyDet2["Freq"]
    
    # Find detected frequency closest to the input
    FreqErr = abs(freq-DailyDetFreq)
    
    if (length(FreqErr) > 0) {
      Idx = which(FreqErr == min(FreqErr))
    } else {
      Idx = integer(0)
    }
    
    
    if ((length(Idx) > 0) & (min(FreqErr) < 1)) {
      ThisFO = DailyDet[[Idx[1]]]
      
      # Add the duration for each period
      ThisFO["Duration"] = as.numeric(difftime(ThisFO$EndTime, ThisFO$StartTime, units="mins") + 1)
    } else {
      ThisFO = NULL
    }
  } else {
    ThisFO = NULL
  }
  
  
  ThisFO
}