function CheckSignalNameError(SignalName, Existing_Signal_Name)

if isempty(find(strcmp(SignalName, Existing_Signal_Name), 1))
    return;
else 
    error('Error: Name of customized signal is repeated.');
end

