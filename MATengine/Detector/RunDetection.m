% function DetectionResults = RunDetection(PMUstruct,DetectorXML)
%
% This function implements all of the application's detectors:
%   Periodogram method for forced oscillation detection by calling the
%   PeriodogramDetector(PMUstruct,Parameters) function
%
%   Spectral coherence method for forced oscillation detection by calling
%   the SpectralCoherenceDetector(PMUstruct,Parameters) function
%
%   Ringdown detection by calling the RingdownDetector(PMUstruct,Parameters)
%   function
%
%   General out-of-range event detector by calling the 
%   OutOfRangeGeneralDetector(PMUstruct,Parameters) function
%
%   Out-of-range voltage detector by calling the 
%   OutOfRangeVoltageDetector(PMUstruct,Parameters) function
%
%   Wind ramp detector by calling the WIndRampDetector(PMUstruct,Parameters)
%   function
%
%
% All detection results are stored in a structure adhering the the
% specifications provided in the event detection specifications document.
%
%
% Inputs:
%   PMUstruct: structure in the common format for PMU data. See the
%              specifications for the data reader.
%   DetectorXML: structure containing information from the detector's
%                configuration XML file.
%   DetectorTypes:  Cell array listing the detectors to be implemented (if
%                   included in DetectorXML). Options are:
%                       Periodogram
%                       SpectralCoherence
%                       Ringdown
%                       OutOfRangeGeneral
%                       OutOfRangeVoltage
%                       WindRamp
%   PastAdditionalOutput:   Allows the AdditionalOutput output from
%                           previous calls to be passed into the current
%                           call to the detector so that values, such as
%                           initial conditions for filters, can be used.
%   TimeStamp:  The time stamp as a string that should be applied to the
%               detection results. If omitted, the default is the final 
%               time stamp from the first PMU:
%               PMUstruct(1).Signal_Time.Time_String{end}
%
% Outputs:
%   DetectionResults: structure containing all detection results. See the
%                     event detection specifications document.
%   AdditionalOutput: structure containing additional information from the
%                     detection methods. A similar structure to
%                     DetectionResults is used, but it is not included in
%                     the specifications document. This output is intended
%                     to provide flexibility for algorithm implementation
%                     and the ability to pass variables for plotting, etc.
%        
% Created by: Jim Follum (james.follum@pnnl.gov) on 07/08/2016
% Completed with initial testing by Jim Follum on 7/11/2016

function [DetectionResults, AdditionalOutput] = RunDetection(PMUstruct,DetectorXML,DetectorTypes,PastAdditionalOutput,TimeStamp)

%#function RingdownDetector
%#function OutOfRangeGeneralDetector
%#function WindRampDetector
%#function PeriodogramDetector
%#function SpectralCoherenceDetector
%#function TheveninDetector
%#function ModeMeter

% If TimeStamp was not included, set it to the default value
if ~exist('TimeStamp','var')
    TimeStamp = PMUstruct(1).Signal_Time.Time_String{end};
end

% Initialize the detection results and addition information structures
DetectionResults = struct('TimeStamp',TimeStamp);
% AdditionalOutput = struct('TimeStamp',TimeStamp);

AdditionalOutput = PastAdditionalOutput;
AdditionalOutput(1).TimeStamp = TimeStamp;

for DetectorType = DetectorTypes
    
    % Note that the {1} following each DetectorType is necessary.
    % DetectorType on its own is a cell. Adding the {1} accesses the string
    % within the cell.
    
    % Check if the current DetectorType was included in the detector's
    % configuration file. If so, run the detector.
    if isfield(DetectorXML,DetectorType{1})
        % Find the number of separate instances of this detector type.
        NumDetectors = length(DetectorXML.(DetectorType{1}));
        if NumDetectors == 1
            % By default, the contents of DetectorXML.(DetectorType{1}) would not be
            % in a cell array because length is one. This makes it so the same
            % indexing can be used in the following for loop.
            DetectorXML.(DetectorType{1}) = {DetectorXML.(DetectorType{1})};
        end
        
        % Implement each instance of this detector type
        for DetectorIndex = 1:NumDetectors
            % Implement the detector. For the periodogram detector, the 
            % following code is equivalent to the contents of eval():
            % PeriodogramDetector(PMUstruct,DetectorXML.Periodogram{DetectorIndex})
            if ~isfield(PastAdditionalOutput,DetectorType{1})
                eval('PastAdditionalOutput(DetectorIndex).(DetectorType{1}) = [];');
            end
            
            if DetectorIndex > length(PastAdditionalOutput)
                eval('PastAdditionalOutput(DetectorIndex).(DetectorType{1}) = [];');
            end
            
            [DetectionResults(DetectorIndex).(DetectorType{1}), AdditionalOutput(DetectorIndex).(DetectorType{1})]...
                = eval([DetectorType{1} 'Detector(PMUstruct,DetectorXML.(DetectorType{1}){DetectorIndex}, PastAdditionalOutput(DetectorIndex).(DetectorType{1}))']);
        end
    end
end