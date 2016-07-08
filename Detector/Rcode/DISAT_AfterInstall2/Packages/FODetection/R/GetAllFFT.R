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

# AllFFT = list()
# #   AllFFT = array(NA, dim=c(nfft,M,NumSig))
# # For each window to be averaged
# for (m in 1:M) {
#   # indices for this window
#   v = ((m-1)*(L-nOverlap)+1):(m*(L-nOverlap)+nOverlap)
#   
#   # For each signal
#   for (SigIdx in 1:NumSig) {
#     xw = SigMat[SigIdx,v]*win # windowed signal
#     x0 = c(xw, rep(0,nfft-L)) # zero padded signal
#     #       AllFFT[,m,SigIdx] = fft(x0)/(sqrt(U*M))  # fft
#     AllFFT[[SigIdx]][,m] = fft(x0)/(sqrt(U*M))  # fft
#   }
# }

GetAllFFT = function(SigMat,win,nOverlap,nfft,Fs) {
  
  NumSig = dim(SigMat)[1]
  N = dim(SigMat)[2]  # Length of signal
  L = length(win)  # length of each window
  M = 1 + floor((N-L)/(L-nOverlap))  # Number of averages
  U = sum(win^2)  # scaling term for spectral estimates
  
  AllFFT = list()
  # For each signal
  for (SigIdx in 1:NumSig) {
    AllFFT[[SigIdx]] = matrix(NA,nfft,M)
    
    # For each window to be averaged
    for (m in 1:M) {
      # indices for this window
      v = ((m-1)*(L-nOverlap)+1):(m*(L-nOverlap)+nOverlap)
      
      xw = SigMat[SigIdx,v]*win # windowed signal
      x0 = c(xw, rep(0,nfft-L)) # zero padded signal
      AllFFT[[SigIdx]][,m] = fft(x0)/(sqrt(U*M))  # fft
    }
  }
  
  f = Fs*(0:(nfft-1))/nfft  # associated frequency vector
  
  list(AllFFT=AllFFT,f=f)
}