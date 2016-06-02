% function PMUstruct = DropOutMissingFilt(PMUstruct,FlagVal)
function PMUstruct = DropOutMissingFilt(PMUstruct,Parameters)

FlagVal = str2num(Parameters.FlagVal);

% Get the time arrays, Data matrix, Flag matrix, and stat vector from the 
% PMU structure
t_datenum = PMUstruct.Signal_Time.Signal_datenum;
t_string = PMUstruct.Signal_Time.Time_String;
Data = PMUstruct.Data;
Flag = PMUstruct.Flag;
Stat = PMUstruct.Stat;

% Differences between each time stamp
Tsteps = diff(t_datenum);
% Estimate the reporting interval as the minimum difference between time
% stamps
TsHat = min(Tsteps);
% Adjust to a reporting interval corresponding to an integer number of
% frames per second
TsHat = (1/round(1/(TsHat*24*3600)))/(24*3600);

% Indices of samples located before missing data begins
BeforeJump = find(Tsteps > 1.5*TsHat);
% Indices of samples located after missing data ends
AfterJump = BeforeJump + 1;

% Arrays and matrices for fixed time, Data, Flag, and Stat fields
t_datenumFix = t_datenum(1):TsHat:t_datenum(end);
t_stringFix = cell(length(t_datenumFix),1);
t_stringFix(:) = {NaN};
DataFix = NaN*ones(length(t_datenumFix),size(Data,2));
FlagFix = FlagVal*ones(length(t_datenumFix),size(Flag,2));
StatFix = NaN*ones(length(t_datenumFix),1);

% These adjustments make it so that the span from an AfterJump index to a 
% BeforeJump index corresponds to a segment of reported data. 
AfterJump = [1; AfterJump];
BeforeJump = [BeforeJump; length(t_datenum)];
SegSamps = BeforeJump - AfterJump + 1;

% For each set of indices from AfterJump to BeforeJump, move the available
% data from the original time, Data, Flag, and Stat fields to the fixed 
% versions.
for idx = 1:length(AfterJump)
    % Find the index of the sample in the fixed time vector that is closest
    % to the start time for this segment.
    tErr = t_datenumFix - t_datenum(AfterJump(idx));
    NewStartIdx = find(abs(tErr) == min(abs(tErr)));
    
    % Move this segment of available data from the original fields to the
    % fixed fields. 
    t_datenumFix(NewStartIdx:NewStartIdx+SegSamps(idx)-1) = t_datenum(AfterJump(idx):BeforeJump(idx));
    t_stringFix(NewStartIdx:NewStartIdx+SegSamps(idx)-1) = t_string(AfterJump(idx):BeforeJump(idx));
    DataFix(NewStartIdx:NewStartIdx+SegSamps(idx)-1,:) = Data(AfterJump(idx):BeforeJump(idx),:);
    FlagFix(NewStartIdx:NewStartIdx+SegSamps(idx)-1,:) = Flag(AfterJump(idx):BeforeJump(idx),:);
    StatFix(NewStartIdx:NewStartIdx+SegSamps(idx)-1) = Stat(AfterJump(idx):BeforeJump(idx));
end

% Replace the original fields with the fixed values
PMUstruct.Signal_Time.Signal_datenum = t_datenumFix;
PMUstruct.Signal_Time.Time_String = t_string;
PMUstruct.Flag = FlagFix;
PMUstruct.Data = DataFix;
PMUstruct.Stat = StatFix;