# Removes entries with peaks occurring after or coincidentally with CurrentTime
# Adjusts EndTime of entries where EndTime was after or coincidental with CurrentTime
CleanDailyDet = function(DailyDet,DailyDetFreq,CurrentTime) {
  KillFreq = c()
  # For each of the detected frequencies
  for (idx in seq_along(DailyDet)) {
    # All periods for this frequency
    temp = DailyDet[[idx]]
    
    KillObs = c()
    # For each of the periods when this FO was detected
    for (idx2 in 1:dim(temp)[1]) {
      # If CurrentTime preceeds or is coincidental with PeakTime, remove the period
      # Otherwise, if CurrentTime preceeds or is coincidental with EndTime, set EndTime back one minute
      if (CurrentTime <= temp[idx2,"PeakTime"]) {
        # Remove this period
        KillObs = c(KillObs,idx2)
      } else if (CurrentTime <= temp[idx2,"EndTime"]) {
        # Set the EndTime back one minute
        tempTime = CurrentTime
        tempTime$min = tempTime$min-1
        # R is brilliant and will store minutes beyond 59 while leaving the hours in place in the previous command. It will still display correctly.
        # This line makes it so that the value is stored as a meaningful date. This is important for code later on.
        tempTime = as.POSIXct(toString(tempTime))
        
        temp[idx2,"EndTime"] = tempTime
      }
    }
    
    if (length(KillObs) > 0) {
      temp = temp[-KillObs,]
    }
    
    if (dim(temp)[1] > 0) {
      # Still observations left
      DailyDet[[idx]] = temp
    } else {
      # No periods of this frequency left, kill it
      KillFreq = c(KillFreq,idx)
    }
  }
  # Remove frequencies that had all periods removed
  if (length(KillFreq) > 0) {
    DailyDet = DailyDet[-KillFreq]
    DailyDetFreq = DailyDetFreq[-KillFreq]
  }
  
  list(DailyDet=DailyDet,DailyDetFreq=DailyDetFreq)
}