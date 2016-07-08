# FOidx represents the user picking out a frequency to examine. Not sure how to do this better right now.

MakeMap_Shape3 = function(DetOut,KeepChan,MultiChanNames,SubLats,SubLons,SubNames, Alg="Per", FOfreq) {
  FreqErr = abs(DetOut$FOfreq - FOfreq)
  FOidx = which(FreqErr == min(FreqErr))
  FOfreq = DetOut$FOfreq[FOidx]
  
  
  FOevidence = rep(NA,length(MultiChanNames))
  FOevidence[KeepChan] = DetOut$FOevidence[,FOidx]
  
  FOshape = rep(0,length(FOevidence))
  FOshape[KeepChan] = DetOut$FOshape[,FOidx]
  
  Clrs = colorRampPalette(c("red1","white","deepskyblue"))(256)
  ClrBins = seq(0,pi,length.out=length(Clrs))
  
  FOshapeMag = Mod(FOshape)
  FOshapeAng = abs(Arg(FOshape))
  
  SubNamesPlot = sapply(MultiChanNames,substr,start=1,stop=4)
  SubLatsPlot = rep(0,length(SubNamesPlot))
  SubLonsPlot = rep(0,length(SubNamesPlot))
  SubClrs = rep(NA,length(SubNamesPlot))
  for (idx in 1:length(SubNamesPlot)) {
    ThisIdx = which(SubNamesPlot[idx] == SubNames)
    if (length(ThisIdx) > 0) {
      SubLatsPlot[idx] = SubLats[ThisIdx]
      SubLonsPlot[idx] = SubLons[ThisIdx]
    } else {
      warning(SubNamesPlot[idx], " cannot be plotted until it is added to SCmapInfo.R.", call.=FALSE)
    }
    
    
    if (!is.na(FOshapeAng[idx])) {
      Err = abs(ClrBins - FOshapeAng[idx])
      SubClrs[idx] = Clrs[which(Err == min(Err))]
    }
  }
  
  # So that those with zero evidence show up on the map in grey as smallest size circles
  SubClrs[FOevidence == 0] = "grey"
  
  # So that excluded PMUs show up on the map in black as smallest size circles
  SubClrs[is.na(FOevidence)] = "black"
  FOevidence[is.na(FOevidence)] = 0
  
  # Scale evidence between 3 and 12 (makes good dot sizes)
  FOevidence = (FOevidence-min(FOevidence))/(max(FOevidence)-min(FOevidence))*2 + 1
  
  if (FreqErr[FOidx]>0.1) {
    TitleClr = 'red'
  } else {
    TitleClr = 'black'
  }
  
  library(maps)
  
  map('state',
        region=c('washington','oregon','idaho','montana','california','nevada','utah','wyoming'),
        xlim=c(-124.7,-111),
        ylim=c(41,50),
        mar=c(2.1, 0.1, 0.1, 0.1)
      )
  points(SubLonsPlot,SubLatsPlot,bg=SubClrs,pch=21,cex=FOevidence,col="black")
  title(paste("Frequency:",round(FOfreq,3),"Hz"),col.main=TitleClr)
  # image.plot(legend.only=T, axis.args=list(at=c(0,180), cex.axis=1.3), legend.cex=1.3, smallplot=c(0.2,0.8,0.15,0.2), zlim=c(0,180), col=Clrs, horizontal = TRUE, legend.lab = "Phasing (degrees)")
  image.plot(legend.only=T, axis.args=list(at=c(0,180), cex.axis=1.3), legend.cex=1.3, smallplot=c(0.8,0.85,0.05,0.845), zlim=c(0,180), col=Clrs, horizontal = FALSE, legend.lab = "Phasing (degrees)")
}