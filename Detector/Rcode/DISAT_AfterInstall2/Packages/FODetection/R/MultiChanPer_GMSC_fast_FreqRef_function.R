# FOfreq = MultiChanPer_GMSC(SigMat,win,win_GMSC,Noverlap_GMSC,K0,sample.rate,f,OmegaB,gam_indy,gam_ident,Nmed)
#
# INPUTS:
# SigMat = matrix of signals for analysis
#          Contains a row for each channel and a column for each sample, dimension NumSig x K
#          Even if only a single channel is used, SigMat MUST be a matrix
# win = periodogram window of length K
# win_GMSC = window for use with the GMSC (consequently used for Welch periodograms used as PSD estimates)
# Noverlap = amount of overlap between the windows used to calculate the GMSC (and consequently the Welch periodograms used as PSD estimates)
# K0 = amount of zero padding for periodogram (GMSC and PSD estimates are zero padded to same length to match periodograms)
# sample.rate = sample rate of signals in SigMat
# f = frequency vector corresponding to OmegaB
# OmegaB = frequency bins of interest. f, gam_indy, and gam_ident are already limited to this range.
#          The periodograms calculated in this function will be limited to these bins as well
# gam_indy = threshold if all channels were independent. Limited to frequency bins prescribed by OmegaB
# gam_ident = threshold if all channels were identical. Limited to frequency bins prescribed by OmegaB
# Nmed = order of median filter applied to GMSC and PSD estimates
# FreqTol = tolerance for frequency refinement
#
# OUTPUT:
# FOfreq = vector of the detected forced oscillation frequencies

MultiChanPer_GMSC_fast_FreqRef = function(SigMat,win,win_GMSC,Noverlap_GMSC,K0,sample.rate,f,OmegaB,gam_indy,gam_ident,Nmed,FreqTol,NumCPUs) {  
  NumSig = dim(SigMat)[1]  # Number of channels to be included
  K = dim(SigMat)[2]  # Length of signal in samples
  B = length(OmegaB)  # Number of frequency bins to examine
  
  # Parameters related to the window needed for amplitude estimation
  U = 1/K*sum(win^2) # Scaling parameter for periodogram
  CG = 1/K*abs(sum(win*exp(1i*(pi/(2*K0)*0)*(0:(K-1)))))  # Coherent Gain - Assumes m=K and werr=pi/(2*K0) (see dissertation)
  
  
  if (NumSig > 1) {
    # Multiple channels
    # Calculate the GMSC. Used to scale the detection threshold appropriately
    # Welch periodograms are calculated anyway, so they are returned for use as estimates of the PSD
    
    GMSCout = GMSC_faster(SigMat,win_GMSC,Noverlap_GMSC,K0,sample.rate,OmegaB,Nmed,NumCPUs)
    G = GMSCout$G
    PSD = GMSCout$P
  } else {
    # Single channel
    # gam_indy = gam_ident, so set G=0 (or 1) so that gam = gam_indy = gam_ident
    # PSD estimate is obtained using call to cpsd
    G = rep(0,B)
#     PSD = cpsd(SigMat,SigMat,win_GMSC,Noverlap_GMSC,nfft=K0,Fs=sample.rate)$Px
    PSD = pwelch(SigMat,win_GMSC,Noverlap_GMSC,nfft=K0,Fs=sample.rate,Nmed)$Pxx
    PSD = matrix(PSD,1)
  }
  
  
  # Scale the detection threshold based on the GMSC
  gam = gam_indy*(1-G) + gam_ident*G
  
  Tstat = rep(0,B)  # Initialize the vector for the test statistic
  Tdiff = matrix(0,NumSig,B)  # Difference between the periodogram and PSD - needed for amp estimation
  SigFFT = matrix(0,NumSig,B) # FFTs for each channel. Used to get rough FO shape estimate
  for (SigIdx in 1:NumSig) {  # For each channel
    Sig = SigMat[SigIdx,]  # Current signal under analysis
    P = pwelch(Sig,win,nOverlap=0,nfft=K0,Fs=sample.rate)$Pxx  # Simple periodogram
    Tstat = Tstat + 2*P[OmegaB]/PSD[SigIdx,OmegaB]  # Scale periodogram and add to sum (test statistic)
    
    # Needed for amplitude estimation
    Tdiff[SigIdx,] = P[OmegaB]-PSD[SigIdx,OmegaB]
    
    # Get FFT of the current signal. For use in FO shape estimation
    SigFFT[SigIdx,] = fft(c(Sig*win, rep(0,K0-length(Sig))))[OmegaB]
  }
  Tdiff[Tdiff<0] = 0 # no negative amplitudes
  
  FOfreqIdx = which(Tstat>gam)  # Indices of detected forced oscillations (Tstat has length B)
  if (length(FOfreqIdx) > 0) {
    # Refine FO frequencies
    if (is.na(FreqTol)) {
      # Don't do frequency refinement
      FOfreqIdx = FOfreqIdx
    } else {
      # Do frequency refinement
      FOfreqIdx = FOfreqRefine(Tstat,FOfreqIdx,f,FreqTol)
    }
    FOfreq = f[FOfreqIdx]  # frequencies of detected forced oscillations
    
    FOamp = sqrt(Tdiff[,FOfreqIdx,drop=FALSE]*(4*U)/(K*CG^2))  # Rough estimate of FO amplitudes (Assumes present throughout window and werr=pi/(2*K0))
#     FOpow = FOamp^2/2                  # Rough estimate of the FO powers
#     NzPlusFOpow = rowVars(SigMat)      # Rough estimate of the total power in the analyzed signal
#     NzPow = NzPlusFOpow - FOpow        # Rough estimate of the underlying noise power
#     SNRhat = 10*log10(FOpow/NzPow)     # Rough estimate of the SNR for each FO
    
    # Get a rough estimate of the FO shape by dividing the FFTs from each channel at FO frequencies by the FFT from the channel where the magnitude at the FO frequency is highest
    FOshape = matrix(0,NumSig,length(FOfreqIdx))
    for (fidx in 1:length(FOfreqIdx)) {
      temp = SigFFT[,FOfreqIdx[fidx]]
      FOshape[,fidx] = temp/temp[abs(temp) == max(abs(temp))]
    }
  }
  else {
    # No FOs detected
    FOfreq = integer(0)  # frequencies of detected forced oscillations
    FOamp = integer(0)  # Rough estimate of FO amplitudes
#     SNRhat = integer(0)     # Rough estimate of the SNR for each FO
    FOshape = integer(0) # Rough estimate of FO shapes
  }
  
  
#   Out = list(FOfreq=FOfreq, FOamp=FOamp, FOfreqIdx=FOfreqIdx, TestStat=Tstat, PSDout=PSD[,OmegaB], gam=gam)
  Out = list(FOfreq=FOfreq, FOevidence=FOamp, FOshape=FOshape, FOfreqIdx=FOfreqIdx, TestStat=Tstat, gam=gam)
}