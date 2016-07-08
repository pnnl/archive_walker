# GMSC = function(SigMat,win,Noverlap,K0,sample.rate,OmegaB)
#
# INPUTS:
# SigMat = matrix of signals for analysis
#          Contains a row for each channel and a column for each sample, dimension NumSig x N
# win = periodogram window of length L<N -> determines the number of windows that are averaged
# Noverlap = amount of overlap between windows
# K0 = amount of zero padding for periodogram
# sample.rate = sample rate of signals in SigMat
# OmegaB = frequency bins of interest corresponding to the GMSC of length K0

GMSC_faster = function(SigMat,win,Noverlap,K0,sample.rate,OmegaB,Nmed=NA,NumCPUs) {
  #   source('~/Spectral Coherence FY15/ApplicationToPMUdata/Algs/cpsd_function.R')
  
#   med.filter.wrap = function(x,width) {
#     library(robfilter)
#     unlist(med.filter(x,width)$level)
#   }
  
  DWper = function(AllFFTin,Nmed,medIdx) {
#     library(FBN)
    if (is.na(Nmed)) {
      Pxy = rowSums(abs(AllFFTin[medIdx,])^2)
      PxyMedFilt = NA
    } else {
      Pxy = rowSums(abs(AllFFTin[medIdx,])^2)
#       PxyMedFilt = rowSums(apply(abs(AllFFTin[medIdx,])^2, MARGIN=2, FUN=medianFilter, windowSize=Nmed))/log(2)
      # PxyMedFilt = rowSums(apply(abs(AllFFTin[medIdx,])^2, MARGIN=2, FUN=med.filter.wrap, width=Nmed))/log(2)
      PxyMedFilt = rowSums(apply(abs(AllFFTin[medIdx,])^2, MARGIN=2, FUN=runmed, k=Nmed))/log(2)
    }
    list(Pxy=Pxy,PxyMedFilt=PxyMedFilt)
  }
  
  NumSig = dim(SigMat)[1]  # Number of channels to be included
  SigNames = rownames(SigMat) # Names of the channels
  B = length(OmegaB)  # Number of frequency bins to examine
  
  if (NumSig==1) {
    G = rep(0,B)
    
    P = matrix(0,1,K0) # This one is used to calculate the GMSC
    P[1,] = pwelch(SigMat[1,],win,Noverlap,K0,sample.rate,Nmed=NA)$Pxx
    
    PmedFilt = matrix(0,1,K0) # This one is used to calculate the test statistic for the periodogram method (median filtered)
    PmedFilt[1,] = pwelch(SigMat[1,],win,Noverlap,K0,sample.rate,Nmed)$Pxx
  } else {
    AllFFT = GetAllFFT(SigMat,win,Noverlap,K0,sample.rate)$AllFFT
    
    # Set up the Sigma matrix. Each slice (for each frequency bin) is 0.5 times an identity matrix.
    # The 0.5 is used because later each slice is conjugate transposed and added to itself.
    # Thus, the diagonals in the end are all 1.
    # The conjugate transpose and addition is used because only the upper triangle of each slice is filled in in the for loops below
    Sigma = array(0.5*diag(NumSig),c(NumSig,NumSig,B))
    
    
    if (is.na(Nmed)) {
      medIdx = 1:OmegaB[length(OmegaB)]
    } else {
      medIdx = 1:(OmegaB[length(OmegaB)]+Nmed) # Will produce the same result as filtering the whole signal, but faster
    }
    
    # Using parallel processing, find the Daniell-Welch periodogram for each signal
    sfInit(parallel=TRUE,cpus=NumCPUs)
    DwperOut <- sfLapply(AllFFT,DWper,Nmed,medIdx)
    sfStop()
    
    P = matrix(0,NumSig,K0)  # This one is used to calculate the GMSC
    PmedFilt = matrix(0,NumSig,K0)  # This one is used to calculate the test statistic for the periodogram method (median filtered)
    for (Idx in 1:NumSig) {
      P[Idx,medIdx] = DwperOut[[Idx]]$Pxy[medIdx]
#       PmedFilt[Idx,medIdx] = DwperOut[[Idx]]$PxyMedFilt[medIdx]
      if (!is.na(Nmed)) {
        PmedFilt[Idx,medIdx] = DwperOut[[Idx]]$PxyMedFilt[medIdx]
      }
    }
    
    for (RowIdx in 2:NumSig) {
      for (ColIdx in seq(RowIdx-1,1,-1)) {
        if (RowIdx != ColIdx) {
          # Dimensions of AllFFT are NumSamp X NumAverage X Channel
          # The simple periodograms from one channel are multiplied by the conjugate of periodograms from another channel
          # Then the average is taken to get the Welch periodogram (division by number of averages is in GetAllFFT) 
          Pxy = rowSums(AllFFT[[RowIdx]] * Conj(AllFFT[[ColIdx]]))
          
          Sigma[RowIdx,ColIdx,] = Pxy[OmegaB]/sqrt(P[RowIdx,OmegaB]*P[ColIdx,OmegaB])
        }
      }
    }
    
    # Only the upper triangles of the slices were filled in.
    # Take conjugate transpose and add to fill in the lower triangles.
    Sigma = Sigma + Conj(aperm(Sigma,c(2,1,3)))
    
    
    # Calculate GMSC from eigenvalues of Sigma    
    G = rep(0,B)
    for (b in 1:B) {
      G[b] = max(eigen(Sigma[,,b],only.values=TRUE)$values)
    }
    G = (1/(NumSig-1)*(G-1))^2
    
    # Apply median filter (if requested) to remove content due to FOs
    if (!is.na(Nmed)) {
#       G = medianFilter(inputData=G, windowSize=Nmed)
      # G = unlist(med.filter(G,width=Nmed)$level)
      G = runmed(G,Nmed)
    }
  }
  
  
#   list(G=G, P=PmedFilt)
  
  if (is.na(Nmed)) {
    list(G=G, P=P)
  } else {
    list(G=G, P=PmedFilt)
  }
}