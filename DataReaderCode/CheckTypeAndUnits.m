function decision = CheckTypeAndUnits(Type,Units)

%% Voltage Magnitudes

Acceptable(1).Type = 'VMP';
Acceptable(1).Units = {'V' 'kV'};

Acceptable(2).Type = 'VMA';
Acceptable(2).Units = {'V' 'kV'};

Acceptable(3).Type = 'VMB';
Acceptable(3).Units = {'V' 'kV'};

Acceptable(4).Type = 'VMC';
Acceptable(4).Units = {'V' 'kV'};


%% Voltage Angles

Acceptable(5).Type = 'VAP';
Acceptable(5).Units = {'DEG' 'RAD'};

Acceptable(6).Type = 'VAA';
Acceptable(6).Units = {'DEG' 'RAD'};

Acceptable(7).Type = 'VAB';
Acceptable(7).Units = {'DEG' 'RAD'};

Acceptable(8).Type = 'VAC';
Acceptable(8).Units = {'DEG' 'RAD'};


%% Voltage Phasors

Acceptable(9).Type = 'VPP';
Acceptable(9).Units = {'V' 'kV'};

Acceptable(10).Type = 'VPA';
Acceptable(10).Units = {'V' 'kV'};

Acceptable(11).Type = 'VPB';
Acceptable(11).Units = {'V' 'kV'};

Acceptable(12).Type = 'VPC';
Acceptable(12).Units = {'V' 'kV'};


%% Current Magnitudes

Acceptable(13).Type = 'IMP';
Acceptable(13).Units = {'A' 'kA'};

Acceptable(14).Type = 'IMA';
Acceptable(14).Units = {'A' 'kA'};

Acceptable(15).Type = 'IMB';
Acceptable(15).Units = {'A' 'kA'};

Acceptable(16).Type = 'IMC';
Acceptable(16).Units = {'A' 'kA'};


%% Current Angles

Acceptable(17).Type = 'IAP';
Acceptable(17).Units = {'DEG' 'RAD'};

Acceptable(18).Type = 'IAA';
Acceptable(18).Units = {'DEG' 'RAD'};

Acceptable(19).Type = 'IAB';
Acceptable(19).Units = {'DEG' 'RAD'};

Acceptable(20).Type = 'IAC';
Acceptable(20).Units = {'DEG' 'RAD'};


%% Current Phasors

Acceptable(21).Type = 'IPP';
Acceptable(21).Units = {'A' 'kA'};

Acceptable(22).Type = 'IPA';
Acceptable(22).Units = {'A' 'kA'};

Acceptable(23).Type = 'IPB';
Acceptable(23).Units = {'A' 'kA'};

Acceptable(24).Type = 'IPC';
Acceptable(24).Units = {'A' 'kA'};


%% Power

Acceptable(25).Type = 'P';
Acceptable(25).Units = {'MW'};

Acceptable(26).Type = 'Q';
Acceptable(26).Units = {'MVAR'};

Acceptable(27).Type = 'CP';
Acceptable(27).Units = {'MVA'};

Acceptable(28).Type = 'S';
Acceptable(28).Units = {'MVA'};


%% Frequency

Acceptable(29).Type = 'F';
Acceptable(29).Units = {'Hz'};

Acceptable(30).Type = 'RCF';
Acceptable(30).Units = {'mHz/sec' 'Hz/sec'};


%% Digital, Scalar, OTHER

% Digital
Acceptable(31).Type = 'D';
Acceptable(31).Units = {'D'};

% Scalar
Acceptable(32).Type = 'SC';
Acceptable(32).Units = {'SC'};

% OTHER
Acceptable(33).Type = 'OTHER';
Acceptable(33).Units = {'O' 'V' 'kV' 'A' 'kA' 'DEG' 'RAD' 'MW' 'MVAR' 'MVA' 'Hz' 'mHz/sec' 'Hz/sec'};


%% Check

AllTypes = {Acceptable(:).Type};
TypeIdx = find(strcmp(AllTypes,Type));

if ~isempty(TypeIdx)
    UnitIdx = find(strcmp(Acceptable(TypeIdx).Units, Units),1);
    if ~isempty(UnitIdx)
        decision = true;
    else
        decision = false;
    end
else
    decision = false;
end