function TimeLoc = RunTimeLocalization(Data,Freq,Parameters,fs)

NumF = length(Freq);
N = size(Data,1);

Mrat = Freq/fs;
if size(Mrat,1) > 1
    Mrat = Mrat.';
end


Nmin = Parameters.LocMinLength*fs;
Nstep = Parameters.LocLengthStep*fs;

nvec = Nmin:Nstep:N;
if nvec(end) ~= N
    nvec = [nvec N];
end

Kstep = Parameters.LocRes*fs;
kvec = Nmin:Kstep:N;
if kvec(end) ~= N
    kvec = [kvec N];
end

E = exp((0:N-1)' * (-1i*2*pi*Mrat));
EX = zeros(size(Data,2),N,NumF);
for c = 1:size(Data,2)
    EX(c,:,:) = (Data(:,c)*ones(1,NumF)) .* E;
end

J = zeros(length(nvec),NumF);
Jloc = zeros(length(nvec),NumF);
for n = 1:length(nvec)
    Y = NaN(size(EX,1),length(kvec),size(EX,3));
    for k = 1:length(kvec)
        idx = (kvec(k)-nvec(n)+1):kvec(k);
        if (idx(1) > 0)
            Y(:,k,:) = sum(EX(:,idx,:),2);
        end
    end
    if size(Data,2) > 1
        % Multichannel test statistic
        [J(n,:),Jloc(n,:)] = max(squeeze(sum(1/nvec(n)*abs(Y).^2)));
    else
        % Single channel test statistic
        [J(n,:),Jloc(n,:)] = max(squeeze(1/nvec(n)*abs(Y).^2));
    end
end

TimeLoc = zeros(NumF,2);
for m = 1:NumF
    [MaxVal,MaxIdx] = max(J(:,m));
    if isnan(MaxVal)
        TimeLoc(m,:) = NaN;
    else
        TimeLoc(m,:) = [(kvec(Jloc(MaxIdx,m))-nvec(MaxIdx)+1) kvec(Jloc(MaxIdx,m))];
    end
end