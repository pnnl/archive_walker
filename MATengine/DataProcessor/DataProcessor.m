% function PMU= DataProcessor(PMUstruct, ProcessXML,NumStages,FlagBitInterpo,FlagBitInput,NumFlags)
% This function carries out all the data processing operation.
%
% Inputs:
	% PMU: struct array of PMUs containing data to be processed
    % ProcessXML: structure containing configuration from the input XML
    % file for data processing
        % ProcessXML.Processing: struct array containing
        % information on data processing
            % ProcessXML.Processing.Filter: struct array containing
            % information on filtering operation
            % ProcessXML.Processing.Interpolate: struct array containing
            % information on missing data interpolation
            % ProcessXML.Processing.Wrap: struct array containing
            % information on angle wraping operation
            % ProcessXML.Processing.Unwrap: struct array containing
            % information on angle unwraping operation
            % ProcessXML.Processing.MultiRate: struct array containing
            % information on datarate change operation
    % NumStages: Number of stages fir data processing
    % FlagBitInterpo: Bit to be flagged to indicate data point is
    % interpolated
    % FlagBitInput: Bit to be flagged to indicate flagged data input
    % NumFlags: Number of bits used to indicate flagged data point  
% Outputs:
    % PMU: struct array of PMUs containing processed data
%        
%Created by: Urmila Agrawal(urmila.agrawal@pnnl.gov) on 06/22/2016
% Modified on 22 July, 2016 by Urmila Agrawal
%   Number of flags in the output PMU strucutre is reduced to 2

function [PMU, FinalCondosFilter, FinalCondosMultiRate, FinalAngles] = DataProcessor(PMUstruct, ProcessXML,NumStages,FlagBitInterpo,FlagBitInput,NumFlags,InitialCondosFilter,InitialCondosMultiRate,PastAngles)

NumPMU = length(PMUstruct);
PMU = PMUstruct;
for PMUind = 1:NumPMU
    [NData,NChan]= size(PMUstruct(PMUind).Data); %gives number of data points and number of data channels
    %Number of flags is reduced to '2' for processed data file. First flag bit
    %indicates flagged input data files and 2nd flag bit is for indicating
    %interpolated data
    PMU(PMUind).Flag = false(NData,NChan,NumFlags); %flag matrix
    FlagMat = false(NData,NChan);
    %Finds rows and columns of the data points flagged by different
    %filter operations
    [Flaggedrow, FlaggedCol] = find(sum(PMUstruct(PMUind).Flag,3) >0);
    FlaggedElement = length(Flaggedrow);
    % creates a matrix whose elements are set to true corresponding to
    % the flagged data points
    for flaggedInd = 1:FlaggedElement
        FlagMat(Flaggedrow(flaggedInd), FlaggedCol(flaggedInd)) = true;
    end
    %Assigns the FlagMat value to the flag matrix of the concatenated
    %PMU structure
    PMU(PMUind).Flag(:,:,FlagBitInput) = FlagMat;
end

%data processor first carries out angle unwraping operation
% if isempty(PastAngles)
%     PastAngles = [];
% end
if isfield(ProcessXML.Processing,'Unwrap')
    [PMU, FinalAngles] = DPUnwrap(PMU,ProcessXML.Processing.Unwrap,PastAngles);
else
    FinalAngles = [];
end
%second, data processor then interpolates missing data, and flagged data if user
%specifies
if isfield(ProcessXML.Processing,'Interpolate')
    PMU = DPinterpolation(PMU,ProcessXML.Processing.Interpolate,FlagBitInterpo);
end

%Then, data processor carries out filter and multirate operation for
%specified number of stages
%
% Create structure to store the final conditions of each filter. The
% structure has as many elements as there are stages, even if some stages
% don't have filters.
FinalCondosFilter = cell(1,NumStages);
FinalCondosMultiRate = cell(1,NumStages);
if isempty(InitialCondosFilter)
    InitialCondosFilter = cell(1,NumStages);
end
if isempty(InitialCondosMultiRate)
    InitialCondosMultiRate = cell(1,NumStages);
end
for StageIdx = 1:NumStages
    
    if isfield(ProcessXML.Processing.Stages{StageIdx},'Filter')
        [PMU, FinalCondosFilter{StageIdx}] = DPfilterStep(PMU,ProcessXML.Processing.Stages{StageIdx}.Filter, InitialCondosFilter{StageIdx});
    end
    
    if isfield(ProcessXML.Processing.Stages{StageIdx},'Multirate')
        [PMU, FinalCondosMultiRate{StageIdx}] = DPMultiRate(PMU,ProcessXML.Processing.Stages{StageIdx}.Multirate, InitialCondosMultiRate{StageIdx});
    end
end
%data processor carries out angle wraping operation at last
if isfield(ProcessXML.Processing,'Wrap')
    PMU = DPWrap(PMU,ProcessXML.Processing.Wrap);
end

% Change names, types, and units of signals
if isfield(ProcessXML,'NameTypeUnit')
    PMU = ChangeNameTypeUnit(PMU,ProcessXML.NameTypeUnit);
end