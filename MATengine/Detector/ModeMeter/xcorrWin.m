function [r,lag,Q] = xcorrWin(y,w,maxLag)

N = length(y);
[Q,lag] = xcorr(w,maxLag,'none');
r = xcorr(y.*w,maxLag,'none');
r = (N-abs(lag'))./(N*Q) .* r;