% function PMU= DataProcessor(PMUstruct, ProcessXML,NumStages,FlagBitInterpo,FlagBitInput,NumFlags)
% This function carries out all the data processing operation.
%
% Inputs:
	% PMU: struct array of PMUs containing data to be processed
    % ProcessXML: structure containing configuration from the input XML
    % file for data processing
        % ProcessXML.Configuration.Processing: struct array containing
        % information on data processing
            % ProcessXML.Configuration.Processing.Filter: struct array containing
            % information on filtering operation
            % ProcessXML.Configuration.Processing.Interpolate: struct array containing
            % information on missing data interpolation
            % ProcessXML.Configuration.Processing.Wrap: struct array containing
            % information on angle wraping operation
            % ProcessXML.Configuration.Processing.Unwrap: struct array containing
            % information on angle unwraping operation
            % ProcessXML.Configuration.Processing.MultiRate: struct array containing
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

function PMU= DataProcessor(PMUstruct, ProcessXML,NumStages,FlagBitInterpo,FlagBitInput,NumFlags)

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
if isfield(ProcessXML.Configuration.Processing,'Unwrap')
    PMU = DPUnwrap(PMU,ProcessXML.Configuration.Processing.Unwrap);
end
%second, data processor then interpolates missing data, and flagged data if user
%specifies
if isfield(ProcessXML.Configuration.Processing,'Interpolate')
    PMU = DPinterpolation(PMU,ProcessXML.Configuration.Processing.Interpolate,FlagBitInterpo);
end

%Then, data processor carries out filter and multirate operation for
%specified number of stages
for StageIdx = 1:NumStages
    
    if isfield(ProcessXML.Configuration.Processing.Stages{StageIdx},'Filter')
        PMU = DPfilterStep(PMU,ProcessXML.Configuration.Processing.Stages{StageIdx}.Filter);
    end
    
    if isfield(ProcessXML.Configuration.Processing.Stages{StageIdx},'Multirate')
        PMU= DPMultiRate(PMU,ProcessXML.Configuration.Processing.Stages{StageIdx}.Multirate);
    end
end
%data processor carries out angle wraping operation at last
if isfield(ProcessXML.Configuration.Processing,'Wrap')
    PMU = DPWrap(PMU,ProcessXML.Configuration.Processing.Wrap);
end

% Change names, types, and units of signals
if isfield(ProcessXML.Configuration,'NameTypeUnit')
    PMU = ChangeNameTypeUnit(PMU,ProcessXML.Configuration.NameTypeUnit);
end