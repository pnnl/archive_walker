function Res = InspectionAnalysis(Func,Data,t,Params)

switch Func
    case 'Spectral'
        try
            Res = InspectionSpectral(Data,Params);
        catch
            Res = struct();
        end
    otherwise
        warning(['The function ' Func ' was not recognized.']);
        Res = struct();
end