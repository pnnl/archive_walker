%           PastAdditionalOutput: past AdditionalOutput outputs from this
%                                 function that can be used for various
%                                 purposes.

function [DetectionResults, AdditionalOutput] = WindRampDetector(PMUstruct,Parameters,PastAdditionalOutput)

MakePlots = true;
MakeGIF = true;

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the periodogram detector could not be used.');
    DetectionResults = struct([]);
    AdditionalOutput = struct([]);
    return
end

% Using the outputs from ExtractData(), make sure that NaN values are
% handled appropriately and that signal types and units are appropriate.
% This comment is general for all detectors, write code specific to the
% detector you're working on.


%% Get detector parameters

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. 
% Additional inputs, such as the length of the input data or the sampling 
% rate, can be added as necessary. 
ExtractedParameters = ExtractParameters(Parameters);

% Store the parameters in variables for easier access
Fpass = ExtractedParameters.Fpass;  % Passband frequency (Hz)
Fstop = ExtractedParameters.Fstop;  % Stopband frequency (Hz)
Apass = ExtractedParameters.Apass;  % Passband Ripple (dB)
Astop = ExtractedParameters.Astop;  % Stopband Attenuation (dB)
TimeMin = ExtractedParameters.TimeMin;  % Lower time for detection threshold
TimeMax = ExtractedParameters.TimeMax;  % Upper time for detection threshold
ValMin = ExtractedParameters.ValMin;  % Lower value for detection threshold
ValMax = ExtractedParameters.ValMax;  % Upper value for detection threshold
DetSlope = ExtractedParameters.DetSlope;    % Slope of detection threshold line
DetYint = ExtractedParameters.DetYint;      % Y-intercept of detection threshold line

%% Based on the specified parameters, initialize useful variables

%
h = fdesign.lowpass('fp,fst,ap,ast', Fpass, Fstop, Apass, Astop, fs);
Hd = design(h, 'butter', ...
    'MatchExactly', 'stopband');

%% Perform detection

if MakePlots
    if ~ishandle(11)
        % figure doesn't exist yet
        figure(11); 
        propedit;

        subplot(1,2,1); plot(0); xlim([-1 0]);
        xlabel('Time (sec)');
        ylabel('MW');

        subplot(1,2,2);
        xlabel('Trend Duration (sec)');
        ylabel('Trend Magnitude (MW)');
        hold on;
        plot([0 TimeMin],ValMin*ones(1,2),'r');
        plot([TimeMin TimeMax],DetSlope*[TimeMin TimeMax] + DetYint,'r');
        hold off;
        ylim([0 350])
    end
    figure(11)
    subplot(1,2,1);
    xlims = xlim;
    hold on;
    plot(t+xlims(2),Data(:,1),'k','LineWidth',2);
end

NumFilt = size(Hd.sosMatrix,1);
AdditionalOutput = struct('FinalConditions',{cell(1,NumFilt)},...
    'extremaLoc',zeros(1,size(Data,2)),...
    'extrema',zeros(1,size(Data,2)),...
    'extremaType',{cell(1,size(Data,2))});
if isempty(PastAdditionalOutput)
    PastAdditionalOutput = struct('FinalConditions',{cell(1,NumFilt)},...
        'extremaLoc',zeros(1,size(Data,2)),...
        'extrema',zeros(1,size(Data,2)),...
        'extremaType',{cell(1,size(Data,2))});
end



% NaN values in a channel are unacceptable. If a channel has any NaN
% values, it is discarded from analysis.
DiscardChannelIdx = find(sum(isnan(Data),1) > 0);

% Only active power, reactive power, and OTHER signal types are acceptable. If a
% channel has any other type, it is discarded from analysis.
DiscardChannelIdx = sort([DiscardChannelIdx find((~strcmp(DataType,'P')) & (~strcmp(DataType,'Q')) & (~strcmp(DataType,'OTHER')))]);

Data(:,DiscardChannelIdx) = NaN;

KeepChannelIdx = setdiff(1:size(Data,2),DiscardChannelIdx);


% If no initial conditions are available, get some by filtering 10
% minutes of data with a constant value equal to the first sample of Data.
% The filters used for this detector have significant transients that lead
% to several minutes of false detections if this isn't done.
if isempty(PastAdditionalOutput.FinalConditions{1})
    TempData = ones(fs*60*10,1)*Data(1,:);
    for FiltIdx = 1:NumFilt
        [TempData(:,KeepChannelIdx),temp] = filter(Hd.sosMatrix(FiltIdx,1:3),Hd.sosMatrix(FiltIdx,4:6),TempData(:,KeepChannelIdx));
        PastAdditionalOutput.FinalConditions{FiltIdx} = zeros(size(temp,1),size(Data,2));
        PastAdditionalOutput.FinalConditions{FiltIdx}(:,KeepChannelIdx) = temp;
        
        TempData(:,KeepChannelIdx) = TempData(:,KeepChannelIdx)*Hd.ScaleValues(FiltIdx);
    end
end
% Similarly, if the initial conditions are all zero (because the channel
% was not included in the analysis of the previous file) get initial conditions.
for chan = intersect(find(sum(PastAdditionalOutput.FinalConditions{1}) == 0), KeepChannelIdx)
    TempData = ones(fs*60*10,1)*Data(1,chan);
    for FiltIdx = 1:NumFilt
        [TempData,PastAdditionalOutput.FinalConditions{FiltIdx}(:,chan)] = filter(Hd.sosMatrix(FiltIdx,1:3),Hd.sosMatrix(FiltIdx,4:6),TempData);
        TempData = TempData*Hd.ScaleValues(FiltIdx);
    end
end

for FiltIdx = 1:NumFilt
    [Data(:,KeepChannelIdx),temp] = filter(Hd.sosMatrix(FiltIdx,1:3),Hd.sosMatrix(FiltIdx,4:6),Data(:,KeepChannelIdx),PastAdditionalOutput.FinalConditions{FiltIdx}(:,KeepChannelIdx));
    AdditionalOutput.FinalConditions{FiltIdx} = zeros(size(temp,1),size(Data,2));
    AdditionalOutput.FinalConditions{FiltIdx}(:,KeepChannelIdx) = temp;
    Data(:,KeepChannelIdx) = Data(:,KeepChannelIdx)*Hd.ScaleValues(FiltIdx);
end

gd = mean(grpdelay(Hd,linspace(0,Fpass,1000),fs));

if MakePlots
    IncIdx = find(diff(Data(:,1)) > 0);
    DecIdx = find(diff(Data(:,1)) < 0);
    DataInc = Data(:,1);
    DataInc(DecIdx) = NaN;
    DataDec = Data(:,1);
    DataDec(IncIdx) = NaN;
    plot(t+xlims(2)-gd/fs,DataInc,'r','LineWidth',2);
    plot(t+xlims(2)-gd/fs,DataDec,'g','LineWidth',2);
    % plot(t+xlims(2)-gd/fs,Data(:,1),'r');
    hold off;
    xlim([t(1)+xlims(2)-gd/fs-300 round(t(end)+xlims(2))]);
    ylim([0 800]);
    % xlim([xlims(1) round(t(end)+xlims(2))]);
end

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'TrendStart',[],'TrendEnd',[],'TrendValue',[]);
for chan = 1:size(Data,2)
    % Store the PMU and channel name
    DetectionResults(chan).PMU = DataPMU{chan};
    DetectionResults(chan).Channel = DataChannel{chan};
    
    % Check if channel is to be included
    if ismember(chan,DiscardChannelIdx)
        % This channel is not to be included - set to NaN
        DetectionResults(chan).TrendStart = NaN;
        DetectionResults(chan).TrendEnd = NaN;
    else
        % This channel is okay to be included
        [pk,pkLocs] = findpeaks(Data(:,chan));
        [val,valLocs] = findpeaks(-Data(:,chan));
        [extremaLocs,sortIdx] = sort([pkLocs.' valLocs.']);
        extrema = [pk.' -val.'];
        extrema = extrema(sortIdx);

        % Add in final point
        extrema = [extrema Data(end,chan)];
        extremaLocs = [extremaLocs size(Data,1)];

        if strcmp(PastAdditionalOutput.extremaType{chan}, 'peak')
            % Most recent extrema was a peak
            % First extrema in this file will be less than
            % the first point if the trend continued.
            if Data(extremaLocs(1),chan) < Data(1,chan)
                % Trend continued - add previous extrema
                extremaLocs = [PastAdditionalOutput.extremaLoc(chan) extremaLocs];
                extrema = [PastAdditionalOutput.extrema(chan) extrema];
            else
                % Trend did not continue - initial point is an extrema
                extremaLocs = [1 extremaLocs];
                extrema = [Data(1,chan) extrema];
            end
        elseif strcmp(PastAdditionalOutput.extremaType{chan}, 'valley')
            % Most recent extrema was a valley
            % First extrema in this file will be greater than
            % the first point if the trend continued.
            if Data(extremaLocs(1),chan) > Data(1,chan)
                % Trend continued - add previous extrema
                extremaLocs = [PastAdditionalOutput.extremaLoc(chan) extremaLocs];
                extrema = [PastAdditionalOutput.extrema(chan) extrema];
            else
                % Trend did not continue - initial point is an extrema
                extremaLocs = [1 extremaLocs];
                extrema = [Data(1,chan) extrema];
            end
        else
            % There was no previous extrema
            % For example, this may be the first file analyzed.
            % Consider the initial point an extrema
            extremaLocs = [1 extremaLocs];
            extrema = [Data(1,chan) extrema];
        end

        % Store extrema before final point in the file for next time
        AdditionalOutput.extremaLoc(chan) = -(size(Data,1)-extremaLocs(end-1));
        AdditionalOutput.extrema(chan) = extrema(end-1);
        if extrema(end-1) > extrema(end)
            % This extrema is a peak
            AdditionalOutput.extremaType{chan} = 'peak';
        else
            % This extrema is a valley
            AdditionalOutput.extremaType{chan} = 'valley';
        end

        % Duration of each trend
        TrendTime = diff(extremaLocs)/fs;
        TrendAmount = diff(extrema);

        DetResults = false(1,length(TrendTime));
        DetResults((abs(TrendAmount) > ValMin) & (TrendTime < TimeMin)) = true;
        DetResults(abs(TrendAmount) > (DetSlope*TrendTime + DetYint)) = true;
        DetResults(TrendTime > TimeMax) = false;
        DetResults(abs(TrendAmount) < ValMin) = false;
        if sum(DetResults) > 0
            temp = extremaLocs(find(DetResults));
            tstr = cell(1,length(temp));
            for idx = find(temp>0)
                tstr{idx} = datestr(datenum(TimeString(temp(idx))) - gd/(fs*60*60*24), 'mm/dd/yy HH:MM:SS.FFF');
            end
            for idx = find(temp<0)
                tstr{idx} = datestr(datenum(TimeString(1)) - gd/(fs*60*60*24) + (temp(idx)-1)/(fs*60*60*24) , 'mm/dd/yy HH:MM:SS.FFF');
            end
            DetectionResults(chan).TrendStart = tstr;

            temp = extremaLocs(find(DetResults)+1);
            tstr = cell(1,length(temp));
            for idx = 1:length(temp)
                tstr{idx} = datestr(datenum(TimeString(temp(idx))) - gd/(fs*60*60*24) , 'mm/dd/yy HH:MM:SS.FFF');
            end
            DetectionResults(chan).TrendEnd = tstr;

            DetectionResults(chan).TrendValue = TrendAmount(DetResults);
        end

        if MakePlots
            if t(end)+xlims(2)-gd/fs > 240
                figure(11);
                subplot(1,2,2);
                hold on; plot(TrendTime,abs(TrendAmount),'k.','MarkerSize',10); hold off;
            end
            drawnow;
        end

        if MakePlots && MakeGIF
            if ((chan == 1) && (t(end)+xlims(2)-gd/fs > 480))
                filename = 'windramp.gif';
                frame = getframe(11);
                im = frame2im(frame);
                [imind,cm] = rgb2ind(im,256);
                if t(end)+xlims(2)-gd/fs < 540
                  imwrite(imind,cm,filename,'gif', 'Loopcount',inf,'DelayTime',0.25);
                else
                  imwrite(imind,cm,filename,'gif','WriteMode','append','DelayTime',0.25);
                end
            end
        end
    end
end


end

%% Nested functions

% This function pulls the parameters out of the structure containing the
% XML information. It turns strings into numbers as necessary and sets
% default values for parameters that were not specified. Additional inputs,
% such as the length of the input data or the sampling rate, can be added
% as necessary. 
function ExtractedParameters = ExtractParameters(Parameters)

% Stopband frequency parameter
if isfield(Parameters,'Fstop')
    % Use specified value
    Fstop = str2double(Parameters.Fstop);
    
    if isnan(Fstop)
        % str2double sets the value to NaN when it can't make it a number
        warning('Fstop is not a number. Default of 0.01 will be used.');
        Fstop = 0.01;
    else
        % Must be greater than 0
        if Fstop < 0
            warning('Fstop must be greater than 0. Default of 0.01 will be used.');
            Fstop = 0.01;
        end
    end
else
    % Use default value
    Fstop = 0.01;
end

% Passband frequency parameter
if isfield(Parameters,'Fpass')
    % Use specified value
    Fpass = str2double(Parameters.Fpass);
    
    if isnan(Fpass)
        % str2double sets the value to NaN when it can't make it a number
        warning('Fpass is not a number. Default of Fstop/2 will be used.');
        Fpass = Fstop/2;
    else
        % Must be greater than 0
        if Fpass < 0
            warning('Fpass must be greater than 0. Default of Fstop/2 will be used.');
            Fpass = Fstop/2;
        end
        
        % Must be less than Fstop
        if Fpass >= Fstop
            warning('Fpass must be less than Fstop. Default of Fstop/2 will be used.');
            Fpass = Fstop/2;
        end
    end
else
    % Use default value
    Fpass = Fstop/2;
end

% Stopband attenuation parameter
if isfield(Parameters,'Astop')
    % Use specified value
    Astop = abs(str2double(Parameters.Astop));
    
    if isnan(Astop)
        % str2double sets the value to NaN when it can't make it a number
        warning('Astop is not a number. Default of 60 dB will be used.');
        Astop = 60;
    end
else
    % Use default value
    Astop = 60;
end

% Passband ripple parameter
if isfield(Parameters,'Apass')
    % Use specified value
    Apass = str2double(Parameters.Apass);
    
    if isnan(Apass)
        % str2double sets the value to NaN when it can't make it a number
        warning('Apass is not a number. Default of 1 dB will be used.');
        Apass = 1;
    else
        % Must be greater than 0
        if Apass < 0
            warning('Apass must be greater than 0. Default of 1 will be used.');
            Apass = 1;
        end
    end
else
    % Use default value
    Apass = 1;
end

% Lower value detection parameter
if isfield(Parameters,'ValMin')
    % Use specified value
    ValMin = str2double(Parameters.ValMin);
    
    if isnan(ValMin)
        % str2double sets the value to NaN when it can't make it a number
        warning('ValMin is not a number. Default of 50 will be used.');
        ValMin = 50;
    else
        % Must be greater than 0
        if ValMin < 0
            warning('ValMin must be greater than 0. Default of 50 will be used.');
            ValMin = 50;
        end
    end
else
    % Use default value
    ValMin = 50;
end

% Upper value detection parameter
if isfield(Parameters,'ValMax')
    % Use specified value
    ValMax = str2double(Parameters.ValMax);
    
    if isnan(ValMax)
        % str2double sets the value to NaN when it can't make it a number
        warning('ValMax is not a number. Default of 10*ValMin will be used.');
        ValMax = 10*ValMin;
    else
        % Must be greater than ValMin
        if ValMax < ValMin
            warning('ValMax must be greater than ValMin. Default of 10*ValMin will be used.');
            ValMax = 10*ValMin;
        end
    end
else
    % Use default value
    ValMax = 10*ValMin;
end

% Lower time detection parameter
if isfield(Parameters,'TimeMin')
    % Use specified value
    TimeMin = str2double(Parameters.TimeMin);
    
    if isnan(TimeMin)
        % str2double sets the value to NaN when it can't make it a number
        warning('TimeMin is not a number. Default of 20 will be used.');
        TimeMin = 20;
    else
        % Must be greater than 0
        if TimeMin < 0
            warning('TimeMin must be greater than 0. Default of 50 will be used.');
            TimeMin = 20;
        end
    end
else
    % Use default value
    TimeMin = 20;
end

% Upper time detection parameter
if isfield(Parameters,'TimeMax')
    % Use specified value
    TimeMax = str2double(Parameters.TimeMax);
    
    if isnan(TimeMax)
        % str2double sets the value to NaN when it can't make it a number
        warning('TimeMax is not a number. Default of 5*TimeMin will be used.');
        TimeMax = 5*TimeMin;
    else
        % Must be greater than TimeMin
        if TimeMax < TimeMin
            warning('TimeMax must be greater than TimeMin. Default of 5*TimeMin will be used.');
            TimeMax = 5*TimeMin;
        end
    end
else
    % Use default value
    TimeMax = 5*TimeMin;
end

% Find the slope and y-intercept of the detection threshold specified by
% ValMin, ValMax, TimeMin, and TimeMax
DetSlope = (ValMax-ValMin)/(TimeMax-TimeMin);
DetYint = ValMax-DetSlope*TimeMax;

ExtractedParameters = struct('Fstop',Fstop,'Fpass',Fpass,'Astop',Astop,...
    'Apass',Apass,'ValMin',ValMin','ValMax',ValMax,'TimeMin',TimeMin,...
    'TimeMax',TimeMax,'DetSlope',DetSlope,'DetYint',DetYint);

end