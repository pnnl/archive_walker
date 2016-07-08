# FOidx represents the user picking out a frequency to examine. Not sure how to do this better right now.

MakeMap_Shape2 = function(FilePath,MapInfoPath, Alg="Per", FOfreq) {
  
  # Has the GPS coordinates of all the PMUs
  suppressWarnings(try(load(MapInfoPath),silent=TRUE))
  
  suppressWarnings(try(load(FilePath),silent=TRUE))
  
  
  if ((!is.element("MultiChanNames",ls())) | (!is.element("WECCmap",ls()))) {
    p1 = NULL
    f = NULL
  } else {
    if (Alg == "Per") {
      DetOut = PerOut
      KeepChan = KeepChanPer
    } else { #SC
      DetOut = SCout
      KeepChan = KeepChanSC
    }
    
    if (length(DetOut$FOfreq) == 0) {
      # No detections to plot
      p1 = NULL
      f = NULL
    } else {
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
      FOevidence = (FOevidence-min(FOevidence))/(max(FOevidence)-min(FOevidence))*9 + 3
      
      
      sites <- data.frame(SubNames = SubNamesPlot, lat = SubLatsPlot, lon = SubLonsPlot, Valu = FOevidence)
      
      if (FreqErr[FOidx]>0.1) {
        TitleClr = 'red'
      } else {
        TitleClr = 'black'
      }
      
      # plotting the maps
      p1 <- ggmap(WECCmap) +
        geom_point(data = sites, 
                   aes(x = lon, y = lat),
                   fill = SubClrs,
                   colour = "black",
                   alpha = 1,
                   size = sites$Valu,
                   shape = 21) +
        ggtitle(paste("Frequency:",round(FOfreq,3),"Hz")) +
        theme(axis.title = element_blank(), plot.title = element_text(color=TitleClr), legend.position = "none", plot.margin = unit(c(0,5,0,0), "lines"))
      
      list(MapPlot=p1,f=f)
    }
  }
}