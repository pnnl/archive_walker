DailyMSE = function(DailyDet,DailyDetFreq) {
  # Default values
  MSEpeak = integer(0)
  MSEdur = integer(0)
  
  Peak = 0
  Dur = 0
  for (FOidx in seq_along(DailyDet)) {
    # Check if this frequency is most significant based on size of peak
    ThesePeaks = DailyDet[[FOidx]]$PeakEvidence
    MSEidx = which(ThesePeaks == max(ThesePeaks))
    
    # In case multiple periods have the same peak, choose the most recent one
    MSEidx = MSEidx[length(MSEidx)]
    
    if (ThesePeaks[MSEidx] > Peak) {
      # Biggest peak so far, update the evidence based MSE
      MSEpeak = DailyDet[[FOidx]][MSEidx,]
      
      # Update the MSE peak
      Peak = ThesePeaks[MSEidx]
    }
    
    
    # Check if this frequency is most significant based on duration
#     TheseDur = DailyDet[[FOidx]]$EndTime - DailyDet[[FOidx]]$StartTime + 1 # broken
    TheseDur = as.numeric(difftime(DailyDet[[FOidx]]$EndTime,DailyDet[[FOidx]]$StartTime,units="mins"))+1
    MSEidx = which(TheseDur == max(TheseDur))
    # In case more than one has equal duration, choose one with biggest peak
    # If multiple have same size peak, choose the most recent one
    tempIdx = which(DailyDet[[FOidx]][MSEidx,"PeakEvidence"] == max(DailyDet[[FOidx]][MSEidx,"PeakEvidence"]))
    MSEidx = MSEidx[tempIdx[length(tempIdx)]]
    
    if (TheseDur[MSEidx] > Dur) {
      # Longest duration so far, update the duration based MSE
      MSEdur = DailyDet[[FOidx]][MSEidx,]
      MSEdur$Duration = as.numeric(TheseDur[MSEidx])
      
      # Update the MSE duration and associated peak
      Dur = TheseDur[MSEidx]
      DurPeak = DailyDet[[FOidx]][MSEidx,"PeakEvidence"]  # Peak for this detection - used to rank detections of same duration
    } else if ((TheseDur[MSEidx] == Dur) & (DailyDet[[FOidx]][MSEidx,"PeakEvidence"] > DurPeak)) {
      # Current detection is same duration with bigger peak - new most significant event
      MSEdur = DailyDet[[FOidx]][MSEidx,]
      MSEdur$Duration = as.numeric(TheseDur[MSEidx])
      
      # Update the MSE duration and associated peak
      Dur = TheseDur[MSEidx] # already equal, just for consistency
      DurPeak = DailyDet[[FOidx]][MSEidx,"PeakEvidence"]  # Peak for this detection - used to rank detections of same duration
    }
  }
  
  list(MSEpeak=MSEpeak, MSEdur=MSEdur)
}
