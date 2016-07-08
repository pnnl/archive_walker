# tol = groups of frequencies will be separated by at least tol, in units of Hz
#           To ensure harmonics are resolved, tol should be less than half of the lowest considered frequency
#           and less than half of the main lobe width of the periodogram's window

# diff = function(vec,Del) vec[(Del+1):length(vec)]-vec[1:(length(vec)-Del)] # should be included already. I put it here in case I need it later

FOfreqRefine = function(Tstat,FOfreqIdx,f,tol) {
  # Breaks frequency bins with detections into groups separated by at least tol Hz
  Loc = c(0, which(diff(f[FOfreqIdx],1) > tol), length(FOfreqIdx))
  
  FOfreqRefine = rep(0,length(Loc)-1) # Refined frequency vector
  FOfreqIdxRefine = rep(0,length(Loc)-1) # indices of the refined frequencies
  
  for (L in 1:(length(Loc)-1)) { # For each group
    Lidx = (Loc[L]+1):Loc[L+1]
    MaxIdx = which(Tstat[FOfreqIdx[Lidx]] == max(Tstat[FOfreqIdx[Lidx]]))
    FOfreqIdxRefine[L] = FOfreqIdx[Lidx][MaxIdx]  # Indices of f that correspond to refined frequency estimates
  }
  
  FOfreqIdxRefine=FOfreqIdxRefine
}