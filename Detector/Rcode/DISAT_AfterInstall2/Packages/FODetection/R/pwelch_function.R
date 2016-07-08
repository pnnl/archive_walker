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

pwelch = function(x,win,nOverlap,nfft,Fs,Nmed=NA) {
  
  N = length(x)  # Length of signal
  L = length(win)  # length of each window
  M = 1 + floor((N-L)/(L-nOverlap))  # Number of averages
  U = 1/L*sum(win^2)
  
  Pxx = rep(0,nfft)  # Welch periodogram for signal x
  
  # For each window to be averaged
  for (m in 1:M) {
    # indices for this window
    v = ((m-1)*(L-nOverlap)+1):(m*(L-nOverlap)+nOverlap)
    
    xw = x[v]*win  # windowed signal
    x0 = c(xw, rep(0,nfft-L)) # zero padded signal
    
    # Add estimates from this window into averages
    if (is.na(Nmed)) {
      Pxx = Pxx + 1/(M*L*U)*abs(fft(x0))^2
    } else {
#       Pxx = Pxx + medianFilter(1/(M*L*U)*abs(fft(x0))^2,Nmed)/log(2)
      # Pxx = Pxx + unlist(med.filter(1/(M*L*U)*abs(fft(x0))^2,width=Nmed)$level)/log(2)
      Pxx = Pxx + runmed(1/(M*L*U)*abs(fft(x0))^2,k=Nmed)/log(2)
    }
    
  }
  
  f = Fs*(0:(nfft-1))/nfft  # associated frequency vector
  
  list(Pxx=Pxx,f=f)
}