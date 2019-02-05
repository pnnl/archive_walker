function [Preset,Server,Instance,GEPPort,SystemXML,ID] = ReadOHpresets(XMLFileIn)

%Get the file handle
fHandle = fopen(XMLFileIn,'rt');

%Read the whole file in
RawTextData=textscan(fHandle,'%s','Delimiter',['\r','\n']);

%Close the file handle
fclose(fHandle);

%Get the total number of lines found
LengthOfFile=size(RawTextData{1},1);

%Just see if it has 'xml version' in it
if (isempty(strfind(RawTextData{1}{1},'xml version')))
    error('Please specify a valid XML file!');
end

FN = {};
FV = {};
for k = 2:LengthOfFile
    CurrTextVal = RawTextData{1}{k};
    
    if ~contains(CurrTextVal, '=')
        SS = strsplit(CurrTextVal,{'<','>','/'});
        idx = find(cellfun(@length,SS) > 0);
        if length(idx) > 1
            error('didn''t read right');
        end
        FN = [FN SS{idx}];
        FV = [FV ' '];
    else
        FieldName = strsplit(CurrTextVal(2:end),{' '});
        FieldName = FieldName{1};
        
        FN = [FN FieldName];
        FV = [FV ' '];
        
        % Check if all contents for this field are on the same line
        if contains(CurrTextVal, ['</' FieldName '>'])
            % All subfields are listed on this line
            
            % Get the subfield names and values listed on this line
%             SS = strsplit(CurrTextVal(2:end),{' ','=','>','"'});
            
            CurrTextVal = strrep(CurrTextVal,'""','"EMPTY"');
            SS = strsplit(CurrTextVal(2:end),{'="','" '});
            SS{1} = SS{1}(strfind(SS{1},' ')+1:end);
            SS{end} = SS{end}(1:strfind(SS{end},'"')-1);
            FNall = SS(1:2:end);
            FVall = SS(2:2:end);
            
%             FNall = SS(2:2:end-2);
%             FVall = SS(3:2:end-2);
            
            for idx = 1:length(FNall)
                FN = [FN FNall{idx}];
                FV = [FV FVall{idx}];
            end
            
            FN = [FN FieldName];
            FV = [FV ' '];
        else
            SS = strsplit(CurrTextVal(2:end),{'=','>','"'});
            SS = [strsplit(SS{1},' ') SS(2:end)];
            FNall = SS(2:2:end-1);
            FVall = SS(3:2:end-1);
            
            for idx = 1:length(FNall)
                FN = [FN FNall{idx}];
                FV = [FV FVall{idx}];
            end
        end
    end
end

X = struct();
OpnrTrack = {};
FldIdxTrack = [];
Flds = {};
FldsIdx = [];
for idx = 1:length(FN)
    if strcmp(FV{idx}, ' ')
        if ~isempty(OpnrTrack)
            if strcmp(OpnrTrack{end}, FN{idx})
                OpnrTrack(end) = [];
                FldsIdx(FldIdxTrack(end),2) = idx;
                
                FldIdxTrack(end) = [];
            else
                OpnrTrack = [OpnrTrack FN{idx}];
                Flds = [Flds FN{idx}];
                FldsIdx = [FldsIdx; idx NaN];
                
                FldIdxTrack = [FldIdxTrack size(FldsIdx,1)];
                
                if CheckIfField(X,OpnrTrack) == 0
                    % Add field
                    ToEval = StoreInStruct(X,OpnrTrack);
                    eval([ToEval ' = struct();']);
                end
            end
        else
            OpnrTrack = [OpnrTrack FN{idx}];
            Flds = [Flds FN{idx}];
            FldsIdx = [FldsIdx; idx NaN];
            
            FldIdxTrack = [FldIdxTrack size(FldsIdx,1)];
            
            if CheckIfField(X,OpnrTrack) == 0
                % Add field
                ToEval = StoreInStruct(X,OpnrTrack);
                eval([ToEval ' = struct();']);
            end
        end
    else
        if CheckIfField(X,OpnrTrack) == 0
            ToEval = StoreInStruct(X,OpnrTrack);
            eval([ToEval '.' FN{idx} ' = ''' FV{idx} ''';']);
        else
            if CheckIfField(X,[OpnrTrack FN{idx}]) == 0
                ToEval = StoreInStruct(X,OpnrTrack);
                ArrayIdx = '(1)';
            elseif eval(['length(' StoreInStruct(X,OpnrTrack) ')']) == 0
                ToEval = StoreInStruct(X,OpnrTrack);
                ArrayIdx = '(1)';
            elseif eval(['isempty(' StoreInStruct(X,OpnrTrack) ['(' num2str(eval(['length(' StoreInStruct(X,OpnrTrack) ')'])) ')'] '.' FN{idx} ')'])
                ToEval = StoreInStruct(X,OpnrTrack);
                ArrayIdx = ['(' num2str(eval(['length(' ToEval ')'])) ')'];
            else
                ToEval = StoreInStruct(X,OpnrTrack);
                ArrayIdx = ['(' num2str(eval(['length(' ToEval ')'])+1) ')'];
            end
            eval([ToEval ArrayIdx '.' FN{idx} ' = ''' FV{idx} ''';']);
        end
    end
end


% Convert to our structure (meta only)
MT = cell(1,length(X.Presets.Preset));
Preset = MT;
Server = MT;
Instance = MT;
GEPPort = MT;
SystemXML = MT;
ID = MT;
for idx = 1:length(X.Presets.Preset)
    Preset{idx} = X.Presets.Preset(idx).name;
    
    NumSigs = length(X.Presets.Preset(idx).Signal);
    SigID = '';
    for SigIdx = 1:NumSigs
        SigID = [SigID ',' X.Presets.Preset(idx).Signal(SigIdx).ID];
        
        if isempty(Server{idx})
            Server{idx} = X.Presets.Preset(idx).Signal(SigIdx).Server;
        elseif ~strcmp(Server{idx}, X.Presets.Preset(idx).Signal(SigIdx).Server)
            error('All signals within a preset must be from the same server');
        end
        
        if isempty(Instance{idx})
            Instance{idx} = X.Presets.Preset(idx).Signal(SigIdx).Instance;
        elseif ~strcmp(Instance{idx}, X.Presets.Preset(idx).Signal(SigIdx).Instance)
            error('All signals within a preset must be from the same server');
        end
        
        if isempty(GEPPort{idx})
            GEPPort{idx} = X.Presets.Preset(idx).Signal(SigIdx).GEPPort;
        elseif ~strcmp(GEPPort{idx}, X.Presets.Preset(idx).Signal(SigIdx).GEPPort)
            error('All signals within a preset must be from the same server');
        end
        
        if isempty(SystemXML{idx})
            SystemXML{idx} = X.Presets.Preset(idx).Signal(SigIdx).SystemXML;
        elseif ~strcmp(SystemXML{idx}, X.Presets.Preset(idx).Signal(SigIdx).SystemXML)
            error('All signals within a preset must be from the same server');
        end
    end
    ID{idx} = SigID(2:end);
end
end


function X = StoreInStruct(X,FN)

ToExe = 'X';
for idx = 1:length(FN)
    ToExe = [ToExe '.(''' FN{idx} ''')'];
    if idx < length(FN)
        ToExe = [ToExe '(' num2str(eval(['length(' ToExe ')'])) ')'];
    end
end

X = ToExe;
end


function res = CheckIfField(X,FN)

ToExe = 'X';
for idx = 1:length(FN)-1
    ToExe = [ToExe '.(''' FN{idx} ''')'];
    if idx < length(FN)-1
        ToExe = [ToExe '(' num2str(eval(['length(' ToExe ')'])) ')'];
    end
end

res = isfield(eval(ToExe),FN{end});
end