##'  This function 
##' @export
##' @param 
##'
##' @return signatures for a single day
##'
##' @author Jim Follum
##'
##' @examples
##' calcCohere()

calcCohere = function(x,y,win,nOverlap,nfft,Fs) {
  
  N = length(x)  # Length of signal
  L = length(win)  # length of each window
  M = 1 + floor((N-L)/(L-nOverlap))  # Number of averages
  U = sum(win^2)  # scaling term for spectral estimates
  
  Pxx = rep(0,nfft)  # Welch periodogram for signal x
  Pyy = rep(0,nfft)  # Welch periodogram for signal y
  Pxy = rep(0,nfft)  # Cross-power spectral density estimate between signals x and y calculated via Welch's method
  
  # For each window to be averaged
  for (m in 1:M) {
    # indices for this window
    v = ((m-1)*(L-nOverlap)+1):(m*(L-nOverlap)+nOverlap)
    
    xw = x[v]*win  # windowed signal
    x0 = c(xw, rep(0,nfft-L)) # zero padded signal
    X = fft(x0)/sqrt(U)  # Scaled fft 
    
    yw = y[v]*win  # windowed signal
    y0 = c(yw, rep(0,nfft-L))  # zero padded signal
    Y = fft(y0)/sqrt(U)  # scaled fft
    
    # Add estimates from this window into averages
    Pxx = Pxx + 2/Fs/M*X*Conj(X)
    Pyy = Pyy + 2/Fs/M*Y*Conj(Y)
    Pxy = Pxy + 2/Fs/M*X*Conj(Y)
  }
  
  # Scale first frequency bin appropriately
  Pxx[1] = Pxx[1]/2
  Pyy[1] = Pyy[1]/2
  Pxy[1] = Pxy[1]/2
  
  # Grab only positive frequencies. Numerical errors lead to non-zero values for the imaginary parts of Pxx and Pyy
  Pxx = Re(Pxx[1:floor(nfft/2+1)])
  Pyy = Re(Pyy[1:floor(nfft/2+1)])
  Pxy = Pxy[1:floor(nfft/2+1)]
  
  C = abs(Pxy)^2/(Pxx*Pyy)  # Spectral coherence
  F = Fs*c(0:floor(nfft/2))/nfft  # associated frequency vector
  
  list(C=C,F=F)
}