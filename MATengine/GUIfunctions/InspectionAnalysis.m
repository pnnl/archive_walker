function Res = InspectionAnalysis(Func,Data,t,Params)

switch Func
    case 'Spectral'
        Res = InspectionSpectral(Data,Params);
    otherwise
        warning(['The function ' Func ' was not recognized.']);
        Res = struct();
end