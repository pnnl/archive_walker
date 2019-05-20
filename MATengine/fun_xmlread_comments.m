% function structVal = fun_xmlread_comments(XMLFileIn)
% This function parses a human-readable XML file into a structure in MATLAB.
% 
% Inputs:
	% XMLFileIn: file name/path
%     
% Outputs:
    % structVal: structure of the XML file contents
%     
% Created May 1, 2016 by Frank Tuffner
% Updated May 10, 2016 by Frank Tuffner (comments handling)

function structVal = fun_xmlread_comments(XMLFileIn) %Crude XML parsing function, which will dump it into a structure

%Create an empty output first
structVal = struct([]);

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

%Set the index
IndexVal=2;

%Loop through
while (IndexVal <= LengthOfFile)
    
    %If the current line is an information tag, skip it
    if (~isempty(strfind(RawTextData{1}{IndexVal},'<?')))
        %Count up a line
        IndexVal = IndexVal + 1;
        
        %Onwards
        continue;
    elseif (~isempty(strfind(RawTextData{1}{IndexVal},'<!--')))  %Comment
        %See if the end is on this line
        if (~isempty(strfind(RawTextData{1}{IndexVal},'-->')))
            %Increment us by one and continue
            IndexVal = IndexVal + 1;
            
            continue;
        else %It's a block - find the end
            %Increment first
            IndexVal = IndexVal + 1;
            
            %Find the end block
            while (IndexVal <= LengthOfFile)
                %Scan for the end
                 if (isempty(strfind(RawTextData{1}{IndexVal},'-->')))
                     %Increment
                     IndexVal = IndexVal + 1;
                 else
                     %Index once more
                     IndexVal = IndexVal + 1;
                     
                     %We also found it, so escape
                     break;
                 end
            end
        end
    else    %Some form of data
        %Call the parser
        [structVal,IndexVal] = fun_ParseLine('',RawTextData,IndexVal,LengthOfFile);
        
        %Incremenet
        IndexVal = IndexVal + 1;
    end
end

%Line parsing function - called this way so recursion may occur
function [structParse,IndexValue] = fun_ParseLine(FieldTitle,DataInput,IndexValue,FileLength)

%Check the length first
if (IndexValue > FileLength)
    error('Reached the end of the file without finishing!');
end

%Create a new, blank structure
structParse = struct([]);

%Overall loop
while (IndexValue <= FileLength)
    %Extract the current value
    CurrTextVal = DataInput{1}{IndexValue};

    %See if we have a leading delimiter
    if (CurrTextVal(1) == '<')
        %Just grab the rest of the string
        RestOfString=CurrTextVal(2:end);
    else
        %Parse to the first field (in case there are tabs)
        [~,RestOfStringWToken] = strtok(CurrTextVal,'<');

        %Trim the token
        RestOfString = RestOfStringWToken(2:end);
    end

    %See if we can get any immediate clues on what this is
    if (RestOfString(1) == '!') %Comment
        %Make sure it is really a comment, otherwise error
        if (~isempty(strfind(RestOfString,'!--')))
            %See if the end is on this line
            if (~isempty(strfind(RestOfString,'-->')))
                %Increment us by one and continue
                IndexValue = IndexValue + 1;

                continue;
            else %It's a block - find the end
                %Increment first
                IndexValue = IndexValue + 1;

                %Find the end block
                while (IndexValue <= FileLength)
                    %Scan for the end
                     if (isempty(strfind(DataInput{1}{IndexValue},'-->')))
                         %Increment
                         IndexValue = IndexValue + 1;
                     else
                         %Index once more
                         IndexValue = IndexValue + 1;

                         %We also found it, so escape
                         break;
                     end
                end
            end
        else %Not formatted right, end the same way
            error('XML is poorly formatted, %s end designation was found out of order',FieldName);
        end
    elseif (RestOfString(1) == '/')    %Are we an ending value?
        %It is -- see if it is ours (hopefully)

        %Now parse to the close
        [FieldName,~] = strtok(RestOfString(2:end),'>');

        %See if it matches
        if (strcmp(FieldName,FieldTitle)==1)    %Works
            %May need something here if there is more after this close.  For
            %now, assuming well laid out

            %Increment us
            IndexValue = IndexValue + 1;
            
            %Drop us out of this while, and back out
            break;
        else
            error('XML is poorly formatted, %s end designation was found out of order',FieldName);
        end

    else %New field, onward

        %Now parse to the close
        [FieldName,MoreStringVals] = strtok(RestOfString,'>');

        %See if there is anything else on this line
        if (length(MoreStringVals)>1)

            %See if our ending token is here (hopefully)
            IndexFind = strfind(MoreStringVals(2:end),['</' FieldName '>']);
            
            %Make sure it is not empty
            if (~isempty(IndexFind))
                %Extract - assumes it is the first one - find is already
                %incremented
                DataValue = MoreStringVals(2:IndexFind(1));
            else
                error('Attempt to find ending token %s failed - it wasn''t on the same line',FieldName);
            end
            
            %See if this field already exists
            if (~isempty(structParse))
                if (isfield(structParse(1),FieldName))
                
                    %Get current length
                    currLength = length(structParse(1).(FieldName));

                    %Increment
                    StoreIndex = currLength + 1;

                else
                    StoreIndex = 1;
                end
            else
                StoreIndex = 1;
            end
            
            if (StoreIndex == 1)
                %Store the value
                structParse(1).(FieldName) = DataValue;
            else
                %see if we've adjusted it already
                if (iscell(structParse(1).(FieldName))==1)
                    %Store the value
                    structParse(1).(FieldName){StoreIndex} = DataValue;
                else
                    %Convert it first
                    TempData = structParse(1).(FieldName);
                    
                    %Remove it
                    structParse = rmfield(structParse,FieldName);
                    
                    %Store it back as a cell
                    structParse(1).(FieldName){1} = TempData;
                    
                    %Now store the new one
                    structParse(1).(FieldName){StoreIndex} = DataValue;
                end
            end
            
            %Increment
            IndexValue = IndexValue + 1;

        else %Just the closing.  Inwards
            %Update counter
            IndexValue = IndexValue + 1;
            
            %Call us again, recursively
            [TempDatastructParse,IndexValue] = fun_ParseLine(FieldName,DataInput,IndexValue,FileLength);

            %See if this field already exists
            if (~isempty(structParse))
                if (isfield(structParse(1),FieldName))
                    %Get current length
                    currLength = length(structParse(1).(FieldName));

                    %Increment
                    StoreIndex = currLength + 1;

                else
                    StoreIndex = 1;
                end
            else
                StoreIndex = 1;
            end

            if (StoreIndex == 1)
                %Store the value
                structParse(1).(FieldName) = TempDatastructParse;
            else
                %see if we've adjusted it already
                if (iscell(structParse(1).(FieldName))==1)
                    %Store the value
                    structParse(1).(FieldName){StoreIndex} = TempDatastructParse;
                else
                    %Convert it first
                    TempData = structParse(1).(FieldName);
                    
                    %Remove it
                    structParse = rmfield(structParse,FieldName);
                    
                    %Store it back as a cell
                    structParse(1).(FieldName){1} = TempData;
                    
                    %Now store the new one
                    structParse(1).(FieldName){StoreIndex} = TempDatastructParse;
                end
            end
        end
    end
end