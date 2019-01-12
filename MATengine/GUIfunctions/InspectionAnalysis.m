function Res = InspectionAnalysis(Func,Data,t,Params)

save('Inputs.mat');
Res = struct();
return

switch Func
    case 'Spectral'
        Res = InspectionSpectral(Data,t,Params);
    otherwise
        warning(['The function ' Func ' was not recognized.']);
        Res = struct();
end