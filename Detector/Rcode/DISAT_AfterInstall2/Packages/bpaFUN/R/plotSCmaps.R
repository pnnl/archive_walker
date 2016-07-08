plotSCmaps = function(newOutput, atypTime, sigPath, Rationale, ReplaceOld=FALSE) {
  require(bpaFUN)
  library(ggmap)
#   library(grid) # redundant with gridExtra
#   library(aqfig) # no longer needed
  library(fields)
  library(gridExtra)
  
  
  # The first line below is temporary until SCmapInfo is added to the bpaFUN package. At that time the second line should be uncommented
#   load(file="C:\\Users\\foll154\\Documents\\DISAT\\DISAT_SpecCohere\\Rcode\\ImplementWithDISAT\\JimsSigCode\\On PIC\\bpaFUN Updates\\SCmapInfo.RData")
  data(SCmapInfo)
  
  
  dir.create(paste(newOutput,"/Maps",sep=""),showWarnings=FALSE,recursive=TRUE)
  
  load(paste(sigPath,"/gasMatrix.Rdata",sep=""))
  indy <- c(1:nrow(gasMatrix))[rownames(gasMatrix)==atypTime]
  # This if statement should always be true, but I'm afraid to remove it.
  if (length(indy)==1) {
    # 30 minutes on each side of atypicality
    rows <- seq(indy-30,indy+30,by=1)
    rows = rows[rows>0]
    rows = rows[rows<nrow(gasMatrix)]
    
    TimesOfInterest = names(gasMatrix[rows,])
    if (ReplaceOld == FALSE) {
      # Check if directory has anything in it already
      ExistingFiles = dir(paste(newOutput,"/Maps",sep=""))
      ExistingFiles = substr(ExistingFiles,1,12)
      
      TimesOfInterestFormat = gsub("-","",gsub(":","",gsub(" ","",TimesOfInterest)))
      
      # Only keep times of interest that haven't already been plotted
      TimesOfInterest = TimesOfInterest[!is.element(TimesOfInterestFormat,ExistingFiles)]
    }
    
    DaysOfInterest = lapply(TimesOfInterest,substr, start=1, stop=10)
    DaysOfInterestUnique = unique(DaysOfInterest)
    
    
    for (ThisDay in DaysOfInterestUnique) {
      load(paste(sigPath,"\\",ThisDay,".Rdata",sep=""))
      CN = colnames(sig)
      
      TimesInDay = grep(ThisDay,TimesOfInterest,value=TRUE)
      for (ThisTime in TimesInDay) {
        SigFreq = sig[ThisTime,grepl(".Freq",CN)]
        SigValu = sig[ThisTime,grepl(".Valu",CN)]
        
        KillNA = is.na(SigFreq) | is.na(SigValu)
        SigFreq = SigFreq[!KillNA]
        SigValu = SigValu[!KillNA]
        
        Freq = rep(NA,length(SubNames))
        Valu = rep(NA,length(SubNames))
        
        for (idx in 1:length(SubNames)) {
          FreqTemp = SigFreq[grepl(SubNames[idx],names(SigFreq))]
          ValuTemp = SigValu[grepl(SubNames[idx],names(SigValu))]
          
          if (length(FreqTemp) > 0) {
            Valu[idx] = max(ValuTemp)[1]
            Freq[idx] = FreqTemp[ValuTemp == max(ValuTemp)][1]
          }
        }
        
        sites <- data.frame(SubNames = SubNames, lat = SubLats, lon = SubLons, Freq = Freq, Valu = Valu)
        sites = sites[is.na(Freq)==FALSE,]
        
        minFreq = 0;
        maxFreq = 20;
        ClrFreqRes = 0.1
        ClrFreqBins = seq(minFreq,maxFreq,ClrFreqRes)
        Rnbow = rainbow(length(ClrFreqBins),end=0.65)
        Clrs = rep(NA,dim(sites)[1])
        for (ClrIdx in 1:dim(sites)[1]) {
          FreqErr = abs(ClrFreqBins - sites$Freq[ClrIdx])
          Clrs[ClrIdx] = Rnbow[which(FreqErr == min(FreqErr))[1]]
        }
        
        
        # plotting the maps
        p1 <- ggmap(WECCmap) +
          geom_point(data = sites, 
                     aes(x = lon, y = lat),
                     fill = Clrs,
                     colour = Clrs,
                     alpha = 0.5,
                     #              size = 12*sqrt(sites$Valu),
                     size = 12*sites$Valu,
                     shape = 21) +
          ggtitle(substr(ThisTime,12,16)) +
          theme(axis.title = element_blank(), legend.position = "none", plot.margin = unit(c(0,5,0,0), "lines"))
        
        
        df = data.frame(time=as.POSIXct(rownames(gasMatrix)[rows]),
                        val = gasMatrix[rows,names(Rationale)])
        dfv = data.frame(time = rep(as.POSIXct(ThisTime),2),
                         val = c(0,max(gasMatrix[rows,names(Rationale)])))
        dfa = data.frame(time = rep(as.POSIXct(atypTime),2),
                         val = c(0,max(gasMatrix[rows,names(Rationale)])))
        p2 = ggplot(df,aes(x = time, y = val)) +
          geom_line(col = 'blue') +
          geom_line(col = 'red', data=dfv, linetype=2) +
          geom_line(col = 'red', data=dfa) +
          ylab("Atypicality Score") +
          xlab(substr(atypTime,1,10))
        
        FileName = gsub("-","",gsub(":","",gsub(" ","",ThisTime)))
        jpeg(filename= paste(newOutput,"/Maps/",FileName,"_FOmap.jpg",sep=""),
             width=7,height=9,units="in",res=150)
        
        image.plot(legend.only=T, zlim=c(minFreq,maxFreq), col=Rnbow, horizontal=TRUE)
        grid.arrange(p1,p2,ncol=1,heights=c(2,1))
        image.plot(legend.only=T, zlim=c(minFreq,maxFreq), col=Rnbow, legend.shrink = 0.3, horizontal = FALSE, legend.lab = "Frequency")
        
        dev.off()
      }
    }
  } # ends if
}