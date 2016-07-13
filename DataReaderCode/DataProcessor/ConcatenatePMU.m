% function ConcateStruct = ConcatenatePMU(DataProcessorStruct)
%This function concatenats different PMU structure  sent. The dimension of
%PMU structure to be concatenated should be same.
%Inputs:
        % DataProcessStruct: Strucutre consisting of different PMU
        % structure to be concatenated Dimension: 1 by number of PMU
        % strucutres to be concatenated
%        
% Output:
        % ConcateStruct: Concatenated PMU Strucutre consisting of one PMU
        % structure
% Created by Urmila Agrawal (urmila.agrawal@pnnl.gov) on 06/22/2016
%
% Modified on June 24, 2016 by Tao Fu
%   modified to use a PMU strucuture cell list as input 
%
% Modified on July 11, 2016 by Tao Fu
%   deleted the added part when NUMPMUstrucutre = 1 to match output data
%   strucuter format
%

function ConcateStruct = ConcatenatePMU(DataProcessorStruct,FlagBitInput,NumFlags)
NumPMUstruct = length(DataProcessorStruct);
% if(NumPMUstruct == 1)
%     % only 1 PMU structure array in the list
%     ConcateStruct = DataProcessorStruct;
%     return;
% end

NumPMU = length(DataProcessorStruct{1});
%% Concatenating PMU structure
for PMUind = 1:NumPMU
    [NData,NChan]= size(DataProcessorStruct{1}(PMUind).Data); %gives number of data points
    % Initializing concatenated PMU
    ConcateStruct.PMU(PMUind).PMU_Name = DataProcessorStruct{1}(PMUind).PMU_Name;   % PMU name
    ConcateStruct.PMU(PMUind).Time_Zone = DataProcessorStruct{1}(PMUind).Time_Zone;         % time zone; for now this is just the PST time
    ConcateStruct.PMU(PMUind).Signal_Name = DataProcessorStruct{1}(PMUind).Signal_Name;
    ConcateStruct.PMU(PMUind).Signal_Type = DataProcessorStruct{1}(PMUind).Signal_Type;
    ConcateStruct.PMU(PMUind).Signal_Unit = DataProcessorStruct{1}(PMUind).Signal_Unit;
    ConcateStruct.PMU(PMUind).Stat = zeros(NumPMUstruct*NData,NChan);
    ConcateStruct.PMU(PMUind).Data = zeros(NumPMUstruct*NData,NChan);
    ConcateStruct.PMU(PMUind).Flag = false(NumPMUstruct*NData,NChan,NumFlags); %flag matrix
    ConcateStruct.PMU(PMUind).Signal_Time.Time_String = cell(NumPMUstruct*NData,1);
    ConcateStruct.PMU(PMUind).Signal_Time.Signal_datenum = zeros(1,NumPMUstruct*NData);
    for PMUStructInd = 1:NumPMUstruct
        if size(DataProcessorStruct{PMUStructInd}(PMUind).Data) ~= [NData,NChan]
            error('Structures to be concatenated are of different sizes.');
        end
        ConcateStruct.PMU(PMUind).Signal_Time.Time_String((PMUStructInd-1)*NData+1:PMUStructInd*NData) = DataProcessorStruct{PMUStructInd}(PMUind).Signal_Time.Time_String;
        ConcateStruct.PMU(PMUind).Signal_Time.Signal_datenum((PMUStructInd-1)*NData+1:PMUStructInd*NData) = DataProcessorStruct{PMUStructInd}(PMUind).Signal_Time.Signal_datenum;
        ConcateStruct.PMU(PMUind).Stat((PMUStructInd-1)*NData+1:PMUStructInd*NData) = DataProcessorStruct{PMUStructInd}(PMUind).Stat;
        ConcateStruct.PMU(PMUind).Data((PMUStructInd-1)*NData+1:PMUStructInd*NData,:) = DataProcessorStruct{PMUStructInd}(PMUind).Data;
        FlagMat = false(NData,NChan);
        [Flaggedrow, FlaggedCol] = find(sum(DataProcessorStruct{PMUStructInd}(PMUind).Flag,3) >0);
        FlaggedElement = length(Flaggedrow);
        for flaggedInd = 1:FlaggedElement
            FlagMat(Flaggedrow(flaggedInd), FlaggedCol(flaggedInd)) = true;
        end
        ConcateStruct.PMU(PMUind).Flag((PMUStructInd-1)*NData+1:PMUStructInd*NData,:,FlagBitInput) = FlagMat;
    end
end
[ConcateStruct.PMU(PMUind).Signal_Time.Signal_datenum,Ind] = sort(ConcateStruct.PMU(PMUind).Signal_Time.Signal_datenum,'ascend');
ConcateStruct.PMU(PMUind).Signal_Time.Time_String = ConcateStruct.PMU(PMUind).Signal_Time.Time_String(Ind);
ConcateStruct.PMU(PMUind).Stat = ConcateStruct.PMU(PMUind).Stat(Ind);
ConcateStruct.PMU(PMUind).Data = ConcateStruct.PMU(PMUind).Data(Ind,:);
ConcateStruct.PMU(PMUind).Flag = ConcateStruct.PMU(PMUind).Flag(Ind,:,:);
end
