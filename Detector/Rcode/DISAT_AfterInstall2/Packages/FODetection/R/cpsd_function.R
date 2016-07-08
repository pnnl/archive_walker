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

cpsd = function(x,y,win,nOverlap,nfft,Fs) {
  
  N = length(x)  # Length of signal
  L = length(win)  # length of each window
  M = 1 + floor((N-L)/(L-nOverlap))  # Number of averages
  U = sum(win^2)  # scaling term for spectral estimates
  
  Pxy = rep(0,nfft)  # Cross-power spectral density estimate between signals x and y calculated via Welch's method
  Px = rep(0,nfft)  # Welch periodogram
  Py = rep(0,nfft)  # Welch periodogram
  
  # For each window to be averaged
  for (m in 1:M) {
    # indices for this window
    v = ((m-1)*(L-nOverlap)+1):(m*(L-nOverlap)+nOverlap)
    
    xw = x[v]*win  # windowed signal
    x0 = c(xw, rep(0,nfft-L)) # zero padded signal
    X = fft(x0)/sqrt(U)  # Scaled fft
    Px = Px + 1/M*abs(X)^2
    
    yw = y[v]*win  # windowed signal
    y0 = c(yw, rep(0,nfft-L))  # zero padded signal
    Y = fft(y0)/sqrt(U)  # scaled fft
    Py = Py + 1/M*abs(Y)^2
    
    # Add estimates from this window into averages
    Pxy = Pxy + 1/M*X*Conj(Y)
  }
  
  f = Fs*(0:(nfft-1))/nfft  # associated frequency vector
  
  list(Pxy=Pxy,f=f,Px=Px,Py=Py)
}