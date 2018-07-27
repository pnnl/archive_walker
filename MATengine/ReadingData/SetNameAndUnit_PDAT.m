% function PMUstruct = SetNameAndUnit_PDAT(PMUstruct)
% This function checks and sets name and unit of signals measured by PMU
% 
% Inputs:
	% PMUstruct: structure in the common format for all PMUs in XML file
        % PMUstruct(i).Signal_Type: a cell array of strings specifying
        % signal(s) type in the i^th PMU
                                    %size: 1 by number of data channel
        % PMUstruct(i).Signal_Name: a cell array of strings specifying
        % name of signal(s) in the i^th PMU
                                    %size: 1 by number of data channel
        % PMUstruct(i).Signal_Unit: a cell array of strings specifying
        % unit of signal(s) in the i^th PMU
                                    %size: 1 by number of data channel  
%     
% Outputs:
    % PMUstruct
%    
%Created by
%
% Updated on July 11, 2016 by Tao Fu
%   updated the code to handle more than one digital signals with signal names dig1, dig2, ...   
%

function PMUstruct = SetNameAndUnit_PDAT(PMUstruct)

NumPMU = length(PMUstruct);

for PMUidx = 1:NumPMU
    NumSig = length(PMUstruct(PMUidx).Signal_Name);
    for SigIdx = 1:NumSig
        SigName = PMUstruct(PMUidx).Signal_Name{SigIdx};
        SigName = strsplit(SigName,'.');
        
        if length(SigName) == 3
            % Phasor
            if length(SigName{2}) ~= 16
                warning(['Signal name ' SigName{2} ' does not match convention. Setting signal type to OTHER.']);
                SigType = 'OTHER';
                SigUnit = 'O';
            elseif strcmp(SigName{2}(15:16),'IP')
                % Positive Sequence Current
                switch SigName{3}
                    case 'MAG'
                        % Current magnitude
                        SigType = 'IMP';
                        SigUnit = 'A';
                    case 'ANG'
                        % Current angle
                        SigType = 'IAP';
                        SigUnit = 'DEG';
                    otherwise
                        warning(['Signal name ' SigName{2} '.' SigName{3} ' does not match convention. Setting signal type to OTHER.']);
                        SigType = 'OTHER';
                        SigUnit = 'O';
                end
            elseif strcmp(SigName{2}(15),'V')
                % Voltage
                switch SigName{2}(16)
                    case 'P'
                        % Positive Sequence
                        SigType = 'P';
                    case 'A'
                        % Phase A
                        SigType = 'A';
                    case 'B'
                        % Phase B
                        SigType = 'B';
                    case 'C'
                        % Phase C
                        SigType = 'C';
                    otherwise
                        warning(['Signal name ' SigName{2} ' does not match convention. Setting signal type to OTHER.']);
                        SigType = 'OTHER';
                        SigUnit = 'O';
                end
                
                switch SigName{3}
                    case 'MAG'
                        % Voltage magnitude
                        SigType = ['VM' SigType];
                        SigUnit = 'V';
                    case 'ANG'
                        % Voltage angle
                        SigType = ['VA' SigType];
                        SigUnit = 'DEG';
                    otherwise
                        warning(['Signal name ' SigName{2} '.' SigName{3} ' does not match convention. Setting signal type to OTHER.']);
                        SigType = 'OTHER';
                        SigUnit = 'O';
                end
            else
%                 warning(['Signal name ' SigName{2} ' does not match convention. Setting signal type to OTHER.']);
                SigType = 'OTHER';
                SigUnit = 'O';
            end
        elseif length(SigName) == 2
            % Not a phasor
            if(strcmp(SigName{2}, 'frq'))
                % Frequency signal
                SigType = 'F';
                SigUnit = 'Hz';
            elseif(strcmp(SigName{2},'rocof'))
                % Rate of change of frequency signal
                SigType = 'RCF';
                SigUnit = 'mHz/sec';
            elseif(strfind(SigName{2}, 'dig'))
                    % Digital signal
                    SigType = 'D';
                    SigUnit = 'D';
            else
                % Analog - either active or reactive power
                if length(SigName{2}) ~= 16
                    warning(['Signal name ' SigName{2} ' does not match convention. Setting signal type to OTHER.']);
                    SigType = 'OTHER';
                    SigUnit = 'O';
                else
                    if strcmp(SigName{2}(15:16),'MW')
                        % Active power
                        SigType = 'P';
                        SigUnit = 'MW';
                    elseif strcmp(SigName{2}(15:16),'MV')
                        % Reactive power
                        SigType = 'Q';
                        SigUnit = 'MVAR';
                    else
                        warning(['Signal name ' SigName{2} ' does not match convention. Setting signal type to OTHER.']);
                        SigType = 'OTHER';
                        SigUnit = 'O';
                    end
                end
            end
        else
            warning(['Signal name ' PMUstruct(PMUidx).Signal_Name{SigIdx} ' does not match convention. Setting signal type to OTHER.']);
            SigType = 'OTHER';
            SigUnit = 'O';
        end
        
        PMUstruct(PMUidx).Signal_Type{SigIdx} = SigType;
        PMUstruct(PMUidx).Signal_Unit{SigIdx} = SigUnit;
    end
end