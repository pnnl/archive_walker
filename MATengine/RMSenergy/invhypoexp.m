function t = invhypoexp(p,lam)

RootFunc = @(x) hypoexpcdf(x,lam) - p;
t = fzero(RootFunc,[0 sum(100*lam)]);