function [ModeEst, Mtrack, sPoles] = SelectMode(a,fs,DesiredModes,Mtrack)

% Find all poles (includes spurious roots)
zPoles = roots(a);  % Find z-domain poles (see eq. (2.8))
sPoles = log(zPoles)*fs;    % Transform to s-domain (see eq. (2.7))

% If a specific mode was not specified, return all poles (sPoles) and set
% other outputs to empty.
if isempty(DesiredModes)
    ModeEst = [];
    Mtrack = {};
    return
end

% 0: Remove all modes outside specified frequency range.
sPolesTemp = sPoles;
sPolesFreqTemp = imag(sPoles)/(2*pi);
sPolesDampTemp = -cos(angle(sPoles))*100;
KillIdx = unique([find(sPolesFreqTemp < DesiredModes(1)); find(sPolesFreqTemp > DesiredModes(2));find(sPolesDampTemp > DesiredModes(4));]);
sPolesTemp(KillIdx) = [];
sPolesFreqTemp(KillIdx) = [];

if length(sPolesTemp) > 1
    % 1: Which is closest to past s-domain estimate (specified frequency if not
    % available)?
    if imag(DesiredModes(3)) ~= 0
        % Past s-domain estimate is available
        sPoleErr = abs(sPolesTemp - DesiredModes(3));
        SelectIdx = find(sPoleErr == min(sPoleErr));
    else
        % Last estimate not available, use specified frequency instead
        SelectIdx = near(sPolesFreqTemp,DesiredModes(3));
        
        % In case they are equally distant (only happens when a junk mode
        % meter is set up, but prevents crashing)
        if isempty(SelectIdx)
            SelectIdx = 1;
        end
    end

    ModeEst = sPolesTemp(SelectIdx);
    Mtrack{length(Mtrack)+1} = sPolesTemp;
elseif isempty(sPolesTemp)
    % No possibilities for this mode were identified
    ModeEst = NaN; 
    Mtrack{length(Mtrack)+1} = NaN;
else
    % One possibility for this mode was identified
    ModeEst = sPolesTemp;    
    Mtrack{length(Mtrack)+1} = sPolesTemp;
end