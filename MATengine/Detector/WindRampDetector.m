%   function [DetectionResults, AdditionalOutput] = WindRampDetector(PMUstruct,Parameters,PastAdditionalOutput)
%
%   This function implemements detection of wind ramp events. The detector
%   operates by monitoring upward and downward trends in the data. A
%   low-pass filter is applied to the signals to help the detector focus on
%   large trends.
%
%   Inputs:
%           PMUstruct: PMU structure in a common format for all PMUs
%           Parameters: User specified values for carrying out detection:
%                 Fstop = Stopband frequency (Hz) - default if ommitted is 0.01 Hz
%                 Fpass = Passband frequency (Hz) - default if ommitted is Fstop/2 Hz
%                 Apass = Passband Ripple (dB) - default if ommitted is 1 dB
%                 Astop = Stopband Attenuation (dB) - default if ommitted is 60 dB
%                 TimeMin = Lower time for detection threshold - default if ommitted is 20 seconds
%                 TimeMax = Upper time for detection threshold - default if ommitted is 5*TimeMin seconds
%                 ValMin = Lower value for detection threshold - default if ommitted is 50 (presumed MW)
%                 ValMax = Upper value for detection threshold - default if ommitted is 10*ValMin (presumed MW)
%           PastAdditionalOutput: past AdditionalOutput output from this
%                                 function that can be used for various
%                                 purposes. In this function they are used
%                                 to track trends across multiple files.
%                                 See the AdditionalOutput output for
%                                 specifications.
%
%   Outputs:
%           DetectionResults: a structure array with an element for each
%                             channel under analysis. Fields are:
%                 PMU = a string specifying the PMU associated with the channel
%                 Channel = a string specifying the channel name
%                 TrendStart = a vector containing datenum values
%                       specifying the start time of each trend that
%                       triggered a detection
%                 TrendEnd = a vector containing datenum values
%                       specifying the start time of each trend that
%                       triggered a detection
%                 TrendValue = a vector specifying the size of the trend
%           AdditionalOutput: a structure array that stores information
%                             needed for continuity when this function is 
%                             called again. Field are:
%                 FinalConditions = a cell array with length equal to the
%                       number of filter stages to be applied. Each cell
%                       contains the final conditions of the respective
%                       filter for all channels.
%                 extremaLoc = vector of sample points of the last extrema 
%                              in the current file referenced to the first
%                              sample of the next file. One entry for each
%                              channel.
%                 extrema = vector of values of the last extrema in the 
%                           current file. One entry for each channel.
%                 extremaType = cell array of extrema types ('peak' or
%                               'valley') for the last extrema in the
%                               current file. One entry for each channel.
%                 
%
% Created by Jim Follum (james.follum@pnnl.gov) November, 2016

function [DetectionResults, AdditionalOutput] = WindRampDetector(PMUstruct,Parameters,PastAdditionalOutput)

MakePlots = false;
MakeGIF = false;
FigNum = 25;

%% Store the channels for analysis in a matrix. PMU and channel names are
% stored in cell arrays. Also returns a time vector t with units of seconds
% and the sampling rate fs.
try
    [Data, DataPMU, DataChannel, DataType, DataUnit, t, fs, TimeString] = ExtractData(PMUstruct,Parameters);
catch
    warning('Input data for the wind ramp detector could not be used.');
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


if isempty(PastAdditionalOutput)
    h = fdesign.lowpass('fp,fst,ap,ast', Fpass, Fstop, Apass, Astop, fs);
    Hd = design(h, 'butter', ...
        'MatchExactly', 'stopband');
    
    NumFilt = size(Hd.sosMatrix,1);
    
    % Group delay in seconds
    gd = mean(grpdelay(Hd,linspace(0,Fpass,1000),fs))/fs;
    
    % Calculate the impulse response of the filter and find the number of
    % samples for the exponential envelope to decay by 60 dB. This number
    % of samples will be used to start up filters when initial conditions
    % are not available.
    [pk,loc] = findpeaks(abs(impz(Hd.sosMatrix)));
    % Fit f(x) = a*exp(b*x) to the peaks (positive and negative) of the
    % impulse response 
    f = fit(loc,pk,'exp1');
    % Number of samples for exponential to decay by 60 dB
    % See https://www.dsprelated.com/freebooks/mdft/Exponentials.html
    ex = round(log(1000)*(-1/f.b));

    PastAdditionalOutput = struct('FinalConditions',{cell(1,NumFilt)},...
        'extremaLoc',zeros(1,size(Data,2)),...
        'extrema',zeros(1,size(Data,2)),...
        'extremaType',{cell(1,size(Data,2))},...
        'Hd',Hd,...
        'gd',gd,...
        'ex',ex,...
        'ProcessedSamples',0);
else
    Hd = PastAdditionalOutput.Hd;
    gd = PastAdditionalOutput.gd;
    ex = PastAdditionalOutput.ex;
    
    NumFilt = size(Hd.sosMatrix,1);
end

AdditionalOutput = struct('FinalConditions',{cell(1,NumFilt)},...
    'extremaLoc',zeros(1,size(Data,2)),...
    'extrema',zeros(1,size(Data,2)),...
    'extremaType',{cell(1,size(Data,2))},...
    'Hd',Hd,...
    'gd',gd,...
    'ex',ex,...
    'ProcessedSamples',PastAdditionalOutput.ProcessedSamples+size(Data,1));

AdditionalOutput(1).Data = Data;
AdditionalOutput(1).DataPMU = DataPMU;
AdditionalOutput(1).DataChannel = DataChannel;
AdditionalOutput(1).DataType = DataType;
AdditionalOutput(1).DataUnit = DataUnit;
AdditionalOutput(1).TimeString = TimeString;



%% Perform detection

if MakePlots
    if ~ishandle(FigNum)
        % figure doesn't exist yet
        figure(FigNum); 
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
        ylim([0 1000])
    end
    figure(FigNum)
    subplot(1,2,1);
    xlims = xlim;
    hold on;
    plot(t+xlims(2),Data(:,1),'k','LineWidth',2);
end



% NaN values in a channel are unacceptable. If a channel has any NaN
% values, it is discarded from analysis.
DiscardChannelIdx = find(sum(isnan(Data),1) > 0);

% Only active power, reactive power, and OTHER signal types are acceptable. If a
% channel has any other type, it is discarded from analysis.
DiscardChannelIdx = sort([DiscardChannelIdx find((~strcmp(DataType,'P')) & (~strcmp(DataType,'Q')) & (~strcmp(DataType,'OTHER')))]);

Data(:,DiscardChannelIdx) = NaN;

KeepChannelIdx = setdiff(1:size(Data,2),DiscardChannelIdx);


% If no initial conditions are available, get some by filtering data with a 
% constant value equal to the first sample of Data.
% The filters used for this detector have significant transients that lead
% to several minutes of false detections if this isn't done.
if isempty(PastAdditionalOutput.FinalConditions{1})
    TempData = ones(ex,1)*Data(1,:);
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
    TempData = ones(ex,1)*Data(1,chan);
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

% Add clean, filtered data
AdditionalOutput(1).DataFilt = Data;

if MakePlots
    IncIdx = find(diff(Data(:,1)) > 0);
    DecIdx = find(diff(Data(:,1)) < 0);
    DataInc = Data(:,1);
    DataInc(DecIdx) = NaN;
    DataDec = Data(:,1);
    DataDec(IncIdx) = NaN;
    plot(t+xlims(2)-gd,DataInc,'r','LineWidth',2);
    plot(t+xlims(2)-gd,DataDec,'g','LineWidth',2);
    % plot(t+xlims(2)-gd/fs,Data(:,1),'r');
    hold off;
    xlim([t(1)+xlims(2)-gd-300 t(end)+xlims(2)+1/fs]);
    ylim([0 1000]);
    % xlim([xlims(1) round(t(end)+xlims(2))]);
end

% Initialize structure to output detection results
DetectionResults = struct('PMU',[],'Channel',[],'TrendStart',[],'TrendEnd',[],'TrendValue',[],'ValueStart',[],'ValueEnd',[]);
if AdditionalOutput.ProcessedSamples >= AdditionalOutput.gd*fs
    % Keep ProcessedSamples from growing without bound
    AdditionalOutput.ProcessedSamples = AdditionalOutput.gd*fs;
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
                    extremaLocs = [round(PastAdditionalOutput.extremaLoc(chan)*fs) extremaLocs];
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
                    extremaLocs = [round(PastAdditionalOutput.extremaLoc(chan)*fs) extremaLocs];
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
            AdditionalOutput.extremaLoc(chan) = -(size(Data,1)-extremaLocs(end-1))/fs;
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
                    tstr{idx} = datestr(datenum(TimeString(temp(idx))) - gd/(60*60*24), 'mm/dd/yy HH:MM:SS.FFF');
                end
                for idx = find(temp<0)
                    tstr{idx} = datestr(datenum(TimeString(1)) - gd/(60*60*24) + (temp(idx)-1)/(fs*60*60*24) , 'mm/dd/yy HH:MM:SS.FFF');
                end
                DetectionResults(chan).TrendStart = tstr;

                temp = extremaLocs(find(DetResults)+1);
                tstr = cell(1,length(temp));
                for idx = 1:length(temp)
                    tstr{idx} = datestr(datenum(TimeString(temp(idx))) - gd/(60*60*24) , 'mm/dd/yy HH:MM:SS.FFF');
                end
                DetectionResults(chan).TrendEnd = tstr;

                DetectionResults(chan).TrendValue = TrendAmount(DetResults);
                DetectionResults(chan).ValueStart = extrema(DetResults);
                DetectionResults(chan).ValueEnd = extrema(find(DetResults)+1);
            end

            if MakePlots
                figure(FigNum);
                subplot(1,2,2);
                hold on; plot(TrendTime,abs(TrendAmount),'k.','MarkerSize',10); hold off;
                drawnow;
            end

            if MakePlots && MakeGIF
                if ((chan == 1) && (t(end)+xlims(2)-gd > 480))
                    filename = 'windramp2.gif';
                    frame = getframe(FigNum);
                    im = frame2im(frame);
                    [imind,cm] = rgb2ind(im,256);

                    if isempty(DetectionResults(chan).TrendEnd)
                        NumFrame = 1;
                    else
                        NumFrame = 10;
                    end

                    for framecount = 1:NumFrame
                        if t(end)+xlims(2)-gd < 540
                          imwrite(imind,cm,filename,'gif', 'Loopcount',inf,'DelayTime',0.25);
                        else
                          imwrite(imind,cm,filename,'gif','WriteMode','append','DelayTime',0.25);
                        end
                    end
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

if isfield(Parameters,'Scale')
    Scale = Parameters.Scale;
    switch Scale
        case 'Short'
            Fpass = 0.03;
            Fstop = 0.05;
            Apass = 1;
            Astop = 60;
            ValMin = 50;
            ValMax = 300;
            TimeMin = 30;
            TimeMax = 300;
        case 'Medium'
            Fpass = 0.001;
            Fstop = 0.003;
            Apass = 1;
            Astop = 60;
            ValMin = 200;
            ValMax = 500;
            TimeMin = 600;
            TimeMax = 12000;
        case 'Long'
            Fpass = 0.00005;
            Fstop = 0.0002;
            Apass = 1;
            Astop = 60;
            ValMin = 400;
            ValMax = 1000;
            TimeMin = 14400;
            TimeMax = 45000;
        otherwise
            Fpass = [];
            Fstop = [];
            Apass = [];
            Astop = [];
            ValMin = [];
            ValMax = [];
            TimeMin = [];
            TimeMax = [];
    end
else
    Fpass = [];
    Fstop = [];
    Apass = [];
    Astop = [];
    ValMin = [];
    ValMax = [];
    TimeMin = [];
    TimeMax = [];
end

% Stopband frequency parameter
if isfield(Parameters,'Fstop')
    % Use specified value
    Fstop = str2double(Parameters.Fstop);
    
    if isnan(Fstop)
        % str2double sets the value to NaN when it can't make it a number
        error('Fstop is not a number.');
    else
        % Must be greater than 0
        if Fstop < 0
            error('Fstop must be greater than 0.');
        end
    end
elseif isempty(Fstop)
    error('Either Scale or Fstop must be specified for the wind ramp detector.');
end

% Passband frequency parameter
if isfield(Parameters,'Fpass')
    % Use specified value
    Fpass = str2double(Parameters.Fpass);
    
    if isnan(Fpass)
        % str2double sets the value to NaN when it can't make it a number
        error('Fpass is not a number.');
    else
        % Must be greater than 0
        if Fpass < 0
            error('Fpass must be greater than 0.');
        end
        
        % Must be less than Fstop
        if Fpass >= Fstop
            error(['Fpass must be less than Fstop = ' num2str(Fpass)]);
        end
    end
elseif isempty(Fpass)
    error('Either Scale or Fpass must be specified for the wind ramp detector.');
end

% Stopband attenuation parameter
if isfield(Parameters,'Astop')
    % Use specified value
    Astop = abs(str2double(Parameters.Astop));
    
    if isnan(Astop)
        % str2double sets the value to NaN when it can't make it a number
        error('Astop is not a number.');
    end
elseif isempty(Astop)
    error('Either Scale or Astop must be specified for the wind ramp detector.');
end

% Passband ripple parameter
if isfield(Parameters,'Apass')
    % Use specified value
    Apass = str2double(Parameters.Apass);
    
    if isnan(Apass)
        % str2double sets the value to NaN when it can't make it a number
        error('Apass is not a number.');
    else
        % Must be greater than 0
        if Apass < 0
            error('Apass must be greater than 0.');
        end
    end
elseif isempty(Apass)
    error('Either Scale or Apass must be specified for the wind ramp detector.');
end

% Lower value detection parameter
if isfield(Parameters,'ValMin')
    % Use specified value
    ValMin = str2double(Parameters.ValMin);
    
    if isnan(ValMin)
        % str2double sets the value to NaN when it can't make it a number
        error('ValMin is not a number.');
    else
        % Must be greater than 0
        if ValMin < 0
            error('ValMin must be greater than 0.');
        end
    end
elseif isempty(ValMin)
    error('Either Scale or ValMin must be specified for the wind ramp detector.');
end

% Upper value detection parameter
if isfield(Parameters,'ValMax')
    % Use specified value
    ValMax = str2double(Parameters.ValMax);
    
    if isnan(ValMax)
        % str2double sets the value to NaN when it can't make it a number
        error('ValMax is not a number.');
    else
        % Must be greater than ValMin
        if ValMax < ValMin
            error(['ValMax must be greater than ValMin = ' num2str(ValMin)]);
        end
    end
elseif isempty(ValMax)
    error('Either Scale or ValMax must be specified for the wind ramp detector.');
end

% Lower time detection parameter
if isfield(Parameters,'TimeMin')
    % Use specified value
    TimeMin = str2double(Parameters.TimeMin);
    
    if isnan(TimeMin)
        % str2double sets the value to NaN when it can't make it a number
        error('TimeMin is not a number.');
    else
        % Must be greater than 0
        if TimeMin < 0
            error('TimeMin must be greater than 0.');
        end
    end
elseif isempty(TimeMin)
    error('Either Scale or TimeMin must be specified for the wind ramp detector.');
end

% Upper time detection parameter
if isfield(Parameters,'TimeMax')
    % Use specified value
    TimeMax = str2double(Parameters.TimeMax);
    
    if isnan(TimeMax)
        % str2double sets the value to NaN when it can't make it a number
        error('TimeMax is not a number.');
    else
        % Must be greater than TimeMin
        if TimeMax < TimeMin
            error('TimeMax must be greater than TimeMin.');
        end
    end
elseif isempty(TimeMax)
    error('Either Scale or TimeMax must be specified for the wind ramp detector.');
end

% Find the slope and y-intercept of the detection threshold specified by
% ValMin, ValMax, TimeMin, and TimeMax
DetSlope = (ValMax-ValMin)/(TimeMax-TimeMin);
DetYint = ValMax-DetSlope*TimeMax;

ExtractedParameters = struct('Fstop',Fstop,'Fpass',Fpass,'Astop',Astop,...
    'Apass',Apass,'ValMin',ValMin','ValMax',ValMax,'TimeMin',TimeMin,...
    'TimeMax',TimeMax,'DetSlope',DetSlope,'DetYint',DetYint);

end