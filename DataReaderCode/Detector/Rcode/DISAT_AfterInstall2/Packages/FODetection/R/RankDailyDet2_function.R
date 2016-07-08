# I need a function similar to RankMSE_function that takes in a DailyDet_YYYYMMDD_Per.Rdata file and ranks the detected oscillations by peak and duration

RankDailyDet2 = function(DailyDet) {
  DailyDet2 = data.frame()
  for (DetIdx in seq_along(DailyDet)) {
    Peaks = DailyDet[[DetIdx]][,"PeakEvidence"]
    temp = DailyDet[[DetIdx]][Peaks==max(Peaks), c("Freq","PeakTime","PeakEvidence","PeakChan")]
    temp["Duration"] = sum(as.numeric(difftime(DailyDet[[DetIdx]]$EndTime, DailyDet[[DetIdx]]$StartTime, units="mins") + 1))
    DailyDet2 = rbind(DailyDet2, temp)
  }
  
  # Sort by frequency
  SortIdx = with(DailyDet2, order(Freq))
  DailyDetRankFreq = DailyDet2[SortIdx,]
  
  # Rank by evidence
  DailyDetRankPeak = DailyDet2[with(DailyDet2, order(-PeakEvidence)),]
  
  # Rank by duration
  DailyDetRankDur = DailyDet2[with(DailyDet2, order(-Duration, -PeakEvidence)),]
  
  # Sort by time
  DailyDetRankTime = DailyDet2[with(DailyDet2, order(PeakTime, -PeakEvidence)),]
  
  list(DailyDetRankFreq=DailyDetRankFreq, DailyDetRankPeak=DailyDetRankPeak,
       DailyDetRankDur=DailyDetRankDur, DailyDetRankTime=DailyDetRankTime)
}