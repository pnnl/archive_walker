function [M,SampleRate,SigName,SigType,SigUnit,time] = GetHQdata(FilePath)

load(FilePath);

% TimeStamp = datetime(strrep(strrep(strrep(FirstPointTimeStamp,'h',':'),'m',':'),'s',''));

data = TransposedData' * 10 / 2^15;
time = 0:1/SampleRate:(length(data)-1)/SampleRate;
time=time';
% Scaling input data;

va = data(:,1)*100;        % Main Bus voltage Line to Neutral
vb = data(:,3)*100;
vc = data(:,2)*100;

ia1 = -data(:,5)*2400;         % Total Current
ib1 = -data(:,6)*2400;
ic1 = -data(:,4)*2400;

ia2 = data(:,7)*640;         % B1C Current
ib2 = -data(:,9)*640;
ic2 = data(:,8)*640;

% This is what BPA provided to PNNL, but it was changed based on Bernie's
% presentation that Dmitry sent.
% ia3 = -data(:,12)*640;        % PHA Current
% ib3 = -data(:,11)*640;
% ic3 = -data(:,10)*640;
% Update:
ia3 = data(:,10)*640;        % PHA Current
ib3 = data(:,12)*640;
ic3 = data(:,11)*640;

ia4 = data(:,13)*640;        % PHB Current
ib4 = data(:,15)*640;
ic4 = data(:,14)*640;

ia5 = data(:,16)*1000;        % B1A Current
ib5 = data(:,18)*1000;
ic5 = data(:,17)*1000;

ia6 = data(:,19)*640;        % B1A Current
ib6 = data(:,21)*640;
ic6 = data(:,20)*640;


M = [va vb vc ia1 ib1 ic1 ia2 ib2 ic2 ia3 ib3 ic3 ia4 ib4 ic4 ia5 ib5 ic5 ia6 ib6 ic6];

SigName = {'va' 'vb' 'vc' 'ia1' 'ib1' 'ic1' 'ia2' 'ib2' 'ic2' 'ia3' 'ib3' 'ic3' 'ia4' 'ib4' 'ic4' 'ia5' 'ib5' 'ic5' 'ia6' 'ib6' 'ic6'};
SigType = cell(1,length(SigName));
SigType{1} = 'VWA';
SigType{2} = 'VWB';
SigType{3} = 'VWC';
SigType(4:3:end) = {'IWA'};
SigType(5:3:end) = {'IWB'};
SigType(6:3:end) = {'IWC'};
SigUnit = cell(1,length(SigName));
SigUnit(1:3) = {'V'};
SigUnit(4:end) = {'A'};