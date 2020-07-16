function lam = SelectAlternativeHEparams(u,s2,n)

s2p = s2/u^2;

lam0bar = (1:n)/sum(1:n);
if (s2p < 1/n) || (1 < s2p)
    error('Solution only exists when sigma^2^prime (variance of RMS divided by squared mean of RMS) is between 1/n and 1.');
elseif s2p > (4*n+2)/(3*n*(n+1))
    % Use Version B
    vrsn = 'B';
    lam1bar = [zeros(1,n-1) 1];
    t = 2/(3*n-2) * (-1 + sqrt(3*n/(2*(n-1)) * (1-2*n+(n+1)*(3*n-2)/2*s2p)));
    lamt = (1-t)*lam0bar + t*lam1bar;
else
    % Use Version C
    vrsn = 'C';
    lam2bar = ones(1,n)/n;
    t = 1 - sqrt(3*(n+1)/(n-1)*(s2p*n-1));
    lamt = (1-t)*lam0bar + t*lam2bar;
end

lam = lamt*u;

% uhat = sum(lam);
% s2hat = sum(lam.^2);

% uErrPct = abs(u-uhat)/u*100;
% s2ErrPct = abs(s2-s2hat)/s2*100;