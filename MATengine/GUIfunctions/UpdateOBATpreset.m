% function Result = UpdateOBATpreset(FileName,Detector,AWconfigFile,OBATpresetFile)
%
% This is one of the top-level functions intended to be called by the GUI.
% It updates the PreSets.xml file used by OBAT to include a new preset
% corresponding to an event exported as a JSIS-CSV. It also adds the
% included signals and their GPS coordinates to the list of PMUs in the
% preset file.
%
% Called by: 
%   The AW GUI
%
% Calls: 
%   []
%
% Inputs:
%   NewPreset - This is the name of the new preset. This should match the
%       name of the data file exported by AW. String.
%   Detector - Name of the detector (Ringdown or OutOfRangeGeneral) that
%       the data was exported from. String.
%   AWconfigFile - Full path to the current Archive Walker configuration
%       file. String.
%   OBATpresetFile - Full path to the OBAT preset file that is to be
%       updated. String.
%
% Outputs:
%   Result - Specifies the outcome of the update (success or failure). String. 

function Result = UpdateOBATpreset(NewPreset,Detector,AWconfigFile,OBATpresetFile)

% Read the OBAT preset file into cell A
fid = fopen(OBATpresetFile,'r');
i = 1;
tline = fgetl(fid);
A{i} = tline;
while ischar(tline)
    i = i+1;
    tline = fgetl(fid);
    A{i} = tline;
end
fclose(fid);

LengthOfFile = length(A)-1;

% Iterate through the current preset file and capture the following:
PMUname = {};   % Names of PMUs that have their GPS coordinates listed
PresetName = {};    % Names of each preset
% Flags to indicate whether a line is in the preset or PMU section. If so,
% they are examined to fill in the PMUname and PresetName arrays
LookForPresets = false;
LookForPMUs = false;
for k = 2:LengthOfFile
    CurrTextVal = A{k};
    
    if contains(CurrTextVal,'<Preview>')
        % Start of presets section - begin examining lines for preset names
        LookForPresets = true;
    elseif contains(CurrTextVal,'</Preview>')
        % End of presets section - stop examining lines for preset names
        LookForPresets = false;
    elseif contains(CurrTextVal,'<PMUs>')
        % Start of PMUs section - begin examining lines for PMU names
        LookForPMUs = true;
    elseif contains(CurrTextVal,'</PMUs>')
        % End of PMUs section - stop examining lines for PMU names
        LookForPMUs = false;
    else
        if LookForPresets
            % If this line contains the beginning of a preset, record its
            % name.
            if contains(CurrTextVal,'<PreSet Name="')
                SS = strsplit(CurrTextVal,'"');
                PresetName{length(PresetName)+1} = SS{2};
            end
        elseif LookForPMUs
            % If this line contains the beginning of a PMU description,
            % record its name.
            if contains(CurrTextVal,'<PMU Name="')
                SS = strsplit(CurrTextVal,'"');
                PMUname{length(PMUname)+1} = SS{2};
            end
        end
    end
end

% Read the Archive Walker configuration file
Config = fun_xmlread_comments(AWconfigFile);
% Retain the portion related to the specified detector
DetectorConfig = Config.Config.DetectorConfig.Configuration.(Detector);
if length(DetectorConfig) == 1
    DetectorConfig = {DetectorConfig};
end
% Iterate through each detector and its PMUs to capture the names of each
% signal that was used in the detectors. Unique pairs of PMUs and signals
% are retained. SigAll captures all listed signals, while the other three
% arrays only capture those that are not already listed in the OBAT presets
% file.
PMUtoInclude = {};
SIGtoInclude = {};
PMUandSIGtoInclude = {};
SIGall = {};
for Didx = 1:length(DetectorConfig)
    if length(DetectorConfig{Didx}.PMU) == 1
        DetectorConfig{Didx}.PMU = {DetectorConfig{Didx}.PMU};
    end
    for Pidx = 1:length(DetectorConfig{Didx}.PMU)
        if length(DetectorConfig{Didx}.PMU{Pidx}.Channel) == 1
            DetectorConfig{Didx}.PMU{Pidx}.Channel = {DetectorConfig{Didx}.PMU{Pidx}.Channel};
        end
        for Cidx = 1:length(DetectorConfig{Didx}.PMU{Pidx}.Channel)
            if sum(strcmp(DetectorConfig{Didx}.PMU{Pidx}.Channel{Cidx}.Name,PMUname)) == 0
                PMUtoInclude = [PMUtoInclude DetectorConfig{Didx}.PMU{Pidx}.Name];
                SIGtoInclude = [SIGtoInclude DetectorConfig{Didx}.PMU{Pidx}.Channel{Cidx}.Name];
                PMUandSIGtoInclude = [PMUandSIGtoInclude [PMUtoInclude{end} SIGtoInclude{end}]];
            end
            SIGall = [SIGall DetectorConfig{Didx}.PMU{Pidx}.Channel{Cidx}.Name];
        end
    end
end
[~,Uidx] = unique(PMUandSIGtoInclude);
PMUtoInclude = PMUtoInclude(Uidx);
SIGtoInclude = SIGtoInclude(Uidx);
%
SIGall = unique(SIGall);

% If a name was provided for a new preset, attempt to add it.
Result = '';
if ~isempty(NewPreset)
    if sum(strcmp(PresetName,NewPreset)) > 0
        % This preset name is already present in the OBAT preset file. Take
        % no further action.
        Result = ' - Preset already existed.';
        Anew1 = A;
    else
        % The preset name is available.
        % Create an update to the A array with room for the new preset.
        Anew1 = cell(1,length(A)+2+length(SIGall));
        % Find the top of the preset list.
        PresetStart = find(contains(A(1:end-1),'<Preview>'));
        %
        % Retain everything up to the point where the preset list starts
        Anew1(1:PresetStart) = A(1:PresetStart);
        % Start a new preset
        k = PresetStart+1;
        Anew1{k} = ['    <PreSet Name="' NewPreset '">'];
        k = k+1;
        % Iterate through the signals, adding new lines the the OBAT preset
        % file as you go
        for idx = 1:length(SIGall)
            Anew1{k} = ['      <Signal>' SIGall{idx} '</Signal>'];
            k = k+1;
        end
        % Conclude the preset
        Anew1{k} = '    </PreSet>';
        %
        % Retain everything after the new preset
        Anew1(PresetStart+2+length(SIGall)+1:end) = A(PresetStart+1:end);
    end
else
    % New preset was not desired.
    Anew1 = A;
end


% The next section takes the GPS coordinates from the Archive Walker
% configuration and updates the OBAT preset file with them.

if ~isfield(Config.Config,'SignalMappingPlotConfig')
    % This version of Archive Walker does not include locations for
    % signals.
    Anew2 = Anew1;
elseif ~isfield(Config.Config.SignalMappingPlotConfig,'Signal')
    % No signal locations were listed in this configuration.
    Anew2 = Anew1;
else
    % Read the portion of the Archive Walker configuration that contains GPS
    % coordinates for analyzed signals.
    MapConfig = Config.Config.SignalMappingPlotConfig.Signal;
    if length(MapConfig) == 1
        MapConfig = {MapConfig};
    end
    
    % Iterate through each of the entries. If 1) the signal is included in
    % those exported by Archive Walker, 2) is not already listed in the
    % OBAT preset file, and 3) is specified as a dot (single location) in
    % the Archive Walker configuration file, add it to the list.
    KeepIdx = [];
    Lat = {};
    Lon = {};
    for idx = 1:length(MapConfig)
        if strcmp(MapConfig{idx}.Type,'Dot')
            if sum(strcmp(MapConfig{idx}.PMU,PMUtoInclude) & strcmp(MapConfig{idx}.SignalName,SIGtoInclude)) == 1
                KeepIdx = [KeepIdx idx];
                Lat = [Lat MapConfig{idx}.Sites.Site.Latitude];
                Lon = [Lon MapConfig{idx}.Sites.Site.Longitude];
            end
        end
    end
    SIGtoInclude = SIGtoInclude(KeepIdx);
    
    % Create a new version of the A cell array with enough room for the
    % additional entries. Each signal (PMU in the preset file) requires 5
    % lines.
    Anew2 = cell(1,length(Anew1)+5*length(SIGtoInclude));
    % Index where the section in the OBAT preset file starts
    PMUsStart = find(contains(Anew1(1:end-1),'<PMUs>'));
    % Retain everything prior to the section of PMUs
    Anew2(1:PMUsStart) = Anew1(1:PMUsStart);
    %
    % Iterate through each signal to be added. k tracks the line in the
    % updated preset file (index of Anew2).
    k = PMUsStart+1;
    for idx = 1:length(SIGtoInclude)
        % Each entry has 5 lines
        for L = 1:5
            switch L
                case 1
                    % First line starts the PMU field and specifies its
                    % name
                    Anew2{k} = ['    <PMU Name="' SIGtoInclude{idx} '">'];
                    k = k + 1;
                case 2
                    % Line two gives a short name. The full name is used
                    % here.
                    Anew2{k} = ['      <ShortName>' SIGtoInclude{idx} '</ShortName>'];
                    k = k + 1;
                case 3
                    % Line three gives the longitude.
                    Anew2{k} = ['      <Long>' Lon{idx} '</Long>'];
                    k = k + 1;
                case 4
                    % Line three gives the latitude.
                    Anew2{k} = ['      <Lat>' Lat{idx} '</Lat>'];
                    k = k + 1;
                case 5
                    % Final line closes the PMU field.
                    Anew2{k} = '    </PMU>';
                    k = k + 1;
            end
        end
    end
    %
    % Retain the portion of the OBAT preset file following the inserted
    % line.
    Anew2(PMUsStart+5*length(SIGtoInclude)+1:end) = Anew1(PMUsStart+1:end);
end

% Create backup before writing over the original file
XMLFileInBkup = strrep(OBATpresetFile,'.xml','_bkup.xml');
if exist(XMLFileInBkup,'file')
    Result = ['The input preset file could not be backed up before being overwritten because ' XMLFileInBkup ' already exists. Update was canceled.'];
    return;
end
stat = copyfile(OBATpresetFile,XMLFileInBkup);
if stat ~= 1
    Result = 'The input preset file could not be backed up before being overwritten. Update was canceled.';
    return
end

% Write cell A into xml
try
    fid = fopen(OBATpresetFile, 'w');
catch
    Result = 'Attempt to open preset file for writing failed. Update was canceled.';
    return
end
%
try
    % This is where the contents of Anew2 are used to overwrite the
    % original OBAT preset file.
    for i = 1:numel(Anew2)
        if Anew2{i+1} == -1
            fprintf(fid,'%s', Anew2{i});
            break
        else
            fprintf(fid,'%s\n', Anew2{i});
        end
    end
    fclose(fid);
catch
    try
        fclose(fid);
    catch
    end
    
    try
        delete(OBATpresetFile);
        movefile(XMLFileInBkup,OBATpresetFile);
        Result = 'An error occurred. Update was canceled.';
        return;
    catch
        Result = ['An error occurred. Attempt to restore original failed. Backup stored at ' XMLFileInBkup];
        return;
    end
end

delete(XMLFileInBkup);
Result = ['Success' Result];