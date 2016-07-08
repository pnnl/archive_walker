# FOevidence = PerOut$FOamp
# FOfreq = PerOut$FOfreq
# Ideally, PreviousTime will just be one minute before CurrentTime. If data is missing,
# this might not be the case. Not assuming that this is the case makes it so that FO continuity across missing data can be observed.
UpdateDailyDet = function(DailyDet,DailyDetFreq,FOfreq,FOevidence,FreqTol,CurrentTime,PreviousTime,KeepChan) {
  # Update DailyDet, which summarizes detections for the day
  
  CurrentTime = as.POSIXct(CurrentTime)
  PreviousTime = as.POSIXct(PreviousTime)
  
  # For each detected FO frequency
  for (FOidx in seq_along(FOfreq)) {
    if (length(DailyDetFreq) > 0) {
      # Difference between previously detected FO frequencies and this frequency
      FreqErr = abs(DailyDetFreq - FOfreq[FOidx])
      # Index of previously detected FO with closest frequency. Must be less that FreqTol away
      PrevDetIdx = which((FreqErr == min(FreqErr)) & (FreqErr < FreqTol))
      
      if (length(PrevDetIdx) > 1) {PrevDetIdx=PrevDetIdx[1]}
    } else {
      # No FOs have been detected yet
      PrevDetIdx = integer(0)
    }
    
    
    if (length(PrevDetIdx) > 0) {
      # Frequency has been detected before
      
      # Gets the most recent end time for this FO frequency
      ThisEndTime = DailyDet[[PrevDetIdx]]$EndTime
      ThisEndTime = ThisEndTime[length(ThisEndTime)]
      
      
      if (ThisEndTime >= PreviousTime) {
        # This detection is from the same period (FO was detected in previous minute)
        PeakEvidence = max(FOevidence[,FOidx])
        PeakChan = KeepChan[FOevidence[,FOidx] == PeakEvidence]
        DailyDetFreq[PrevDetIdx] = FOfreq[FOidx]  # Update frequency
        
        # Just counts the number of entries (periods)
        ThisIdx = dim(DailyDet[[PrevDetIdx]])[1]
        
        # Update end time to current time
        DailyDet[[PrevDetIdx]][ThisIdx,"EndTime"] = CurrentTime
        
        # Update frequency
        DailyDet[[PrevDetIdx]][ThisIdx,"Freq"] = FOfreq[FOidx]
        
        if (PeakEvidence > DailyDet[[PrevDetIdx]][ThisIdx,"PeakEvidence"]) {
          # This is a new peak, so update peak time, amplitude, and channel
          DailyDet[[PrevDetIdx]][ThisIdx,"PeakTime"] = CurrentTime
          DailyDet[[PrevDetIdx]][ThisIdx,"PeakEvidence"] = PeakEvidence
          DailyDet[[PrevDetIdx]][ThisIdx,"PeakChan"] = PeakChan
        }
      } else {
        # This detection is from a new period (FO was detected earlier in the day, but not in the previous minute)
        # Add new values for this period (so far this period is just the current minute) and update the frequency
        PeakEvidence = max(FOevidence[,FOidx])
        PeakChan = KeepChan[FOevidence[,FOidx] == PeakEvidence]
        DailyDetFreq[PrevDetIdx] = FOfreq[FOidx]
        DailyDet[[PrevDetIdx]] = rbind(DailyDet[[PrevDetIdx]], 
                                          data.frame(Freq=FOfreq[FOidx], StartTime=CurrentTime, EndTime=CurrentTime, PeakTime=CurrentTime, PeakEvidence=PeakEvidence, PeakChan=PeakChan, stringsAsFactors=FALSE))
      }
    } else {
      # Frequency has not been detected before
      # Add this FO as a new entry
      PeakEvidence = max(FOevidence[,FOidx])
      PeakChan = KeepChan[FOevidence[,FOidx] == PeakEvidence]
      DailyDetFreq = c(DailyDetFreq, FOfreq[FOidx])
      DailyDet[[length(DailyDet)+1]] = data.frame(Freq=FOfreq[FOidx], StartTime=CurrentTime, EndTime=CurrentTime, PeakTime=CurrentTime, PeakEvidence=PeakEvidence, PeakChan=PeakChan, stringsAsFactors=FALSE)
    }
  }
  
  list(DailyDet=DailyDet,DailyDetFreq=DailyDetFreq)
}