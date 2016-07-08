# FOfreq = MultiChanSC_GMSC(SigMat,win,Noverlap_SC,K0,sample.rate,f,OmegaB,D,gam)
#
# INPUTS:
# SigMat = matrix of signals for analysis
#          Contains a row for each channel and a column for each sample, dimension NumSig x Ntot
# win = window for use when calculating the magnitude squared coherence by Welch's method
# Noverlap_SC = amount of overlap between the windows used to calculate the MSC
# K0 = amount of zero padding for the MSC
# sample.rate = sample rate of signals in SigMat
# f = frequency vector corresponding to OmegaB
# OmegaB = frequency bins of interest
# D = delay (in samples) for the self coherence signals
# gamScale = detection threshold scaling value
# FreqTol = tolerance for frequency refinement
#
# OUTPUT:
# FOfreq = vector of the detected forced oscillation frequencies

MultiChanSC_GMSC_fast_FreqRef_MP = function(SigMat,win,Noverlap_SC,K0,sample.rate,f,OmegaB,Nsc,D,NumDelay,gamScale,FreqTol,NumCPUs) {
  
  GMSC_MP = function(SigMatGMSC,win,Noverlap_SC,K0,sample.rate,OmegaB,NumDelay) {
    G = GMSC_fast(SigMatGMSC,win,Noverlap_SC,K0,sample.rate,OmegaB)$G
  }
  
  NumSig = dim(SigMat)[1]  # Number of channels to be included
  Ntot = dim(SigMat)[2]  # Number of samples 
  B = length(OmegaB)  # Number of frequency bins to examine
  
  
  SigList = c()
  SigFFT = matrix(0,NumSig,B) # FFTs for each channel. Used to get rough FO shape estimate
  for (SigIdx in 1:NumSig) {
    SigMatGMSC = matrix(0,NumDelay,Nsc)
    for (nDel in 1:NumDelay) {
      SigMatGMSC[nDel,] = SigMat[SigIdx,((Ntot-Nsc+1):Ntot)-(nDel-1)*D]
    }
    SigList[[SigIdx]] = SigMatGMSC
    
    
    # Calculate FFT of signal used for detection. Used in rough estimate of FO shape
    SigTemp = SigMat[SigIdx,(Ntot-Nsc+1-(NumDelay-1)*D):Ntot]  # All data used to calculate GMSC
    SigFFT[SigIdx,] = fft(c(SigTemp, rep(0,K0-length(SigTemp))))[OmegaB]
  }
  
  if (NumDelay > 2) {
    # Using parallel processing, evaluate the GMSC for each channel
    sfInit(parallel=TRUE,cpus=NumCPUs)
    Gout <- sfLapply(SigList,GMSC_MP,win,Noverlap_SC,K0,sample.rate,OmegaB,NumDelay)
    sfStop()
  } else {
    # For two delays, much faster to calculate without GMSC (no need for parallel processing)
    Gout = list()
    for (SigIdx in 1:NumSig) {
      C = cpsd(SigList[[SigIdx]][1,],SigList[[SigIdx]][2,],win,Noverlap_SC,K0,sample.rate)
      Gout[[SigIdx]] = abs(C$Pxy[OmegaB])^2/(C$Px[OmegaB]*C$Py[OmegaB])
    }
  }
  
  
  # GMSC from each channel - reported to give participation of each channel in FO
  Gall = do.call(rbind,Gout)
  
  # Test statistic (average of self-coherences)
  TestStat = colMeans(Gall)
  
  gam = gamScale*median(TestStat)
  FOfreqIdx = which(TestStat>gam)  # Indices of detected forced oscillations (TestStat has length B)
  if (length(FOfreqIdx) > 0) {
    # Refine FO frequencies
    FOfreqIdx = FOfreqRefine(TestStat,FOfreqIdx,f,FreqTol)
    FOfreq = f[FOfreqIdx]  # frequencies of detected forced oscillations
    GatFO = Gall[,FOfreqIdx,drop=FALSE] # Each channel's GMSC (across multiple delays) at FO frequencies
    
    # Get a rough estimate of the FO shape by dividing the FFTs from each channel at FO frequencies by the FFT from the channel where the magnitude at the FO frequency is highest
    FOshape = matrix(0,NumSig,length(FOfreqIdx))
    for (fidx in 1:length(FOfreqIdx)) {
      temp = SigFFT[,FOfreqIdx[fidx]]
      FOshape[,fidx] = temp/temp[abs(temp) == max(abs(temp))]
    }
  }
  else {
    # FO not detected
    FOfreq = integer(0)  # frequencies of detected forced oscillations
    GatFO = integer(0) # Each channel's GMSC (across multiple delays) at FO frequencies
    FOshape = integer(0) # Rough estimate of FO shapes
  }
  
  
  Out = list(FOfreq=FOfreq, FOfreqIdx=FOfreqIdx, FOevidence=GatFO, FOshape=FOshape, TestStat=TestStat, gam=gam)
}