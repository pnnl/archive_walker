function F = hypoexpcdf(x,lam)

if size(lam,2) ~= 1
    lam = lam';
end

a = 1./lam;
A = zeros(1,length(lam));
for k = 1:length(lam)
    kk = 1:length(lam);
    kk(k) = [];
    A(k) = prod(a(kk)./(a(kk)-a(k)));
end

F = 1 - A * exp(-a.*x);