function [PMU,PMUbyFile,DataInfo,FileInfo,FileLength] = LoadFocusFiles(FileIdx,SkippedFiles,FileInfo,PMUbyFile,idx,DataInfo,focusFile,FileLength,Num_Flags,FileDirectory)

if FileIdx < SkippedFiles+1
    % This FileIdx corresponds to a missing file
    idx44 = 1:length(FileInfo);
else
    % Any missing files have been accounted for.
    % Load the files corresponding to the earliest FocusFileTime
    %
    % For each directory where a file was available for the earliest
    % FocusFileTime, load the file and store it in PMUbyFile
    RealTimePause = true;
    for idx3 = find(idx)
        % If in real time mode, pause 10 seconds to make sure file
        % is completely written. The RealTimePause logical limits
        % this pause to the first file loaded. 
        if RealTimePause && isempty(DataInfo.DateTimeEnd)
            RealTimePause = false;
            
            % If a database is being used, the pause doesn't need to be as
            % long.
            if strcmpi(FileInfo(idx3).FileType, 'OpenHistorian') || strcmpi(FileInfo(idx3).FileType, 'openPDC') || strcmpi(FileInfo(idx3).FileType, 'PI')
                PauseDuration = 1;
            else
                PauseDuration = 10;
            end
            
            pause(PauseDuration);
        end

        % ***********
        % Data Reader
        % ***********

        % Create the PMU structure
        try
            if(strcmpi(FileInfo(idx3).FileType, 'pdat'))
                % pdat format
                [PMUbyFileTemp,tPMU] = pdatReadnCreateStruct(focusFile{idx3},Num_Flags,FileLength);
            elseif(strcmpi(FileInfo(idx3).FileType, 'csv'))
                % JSIS_CSV format
                [PMUbyFileTemp,tPMU] = JSIS_CSV_2_Mat(focusFile{idx3},Num_Flags);
            elseif(strcmpi(FileInfo(idx3).FileType, 'powHQ'))
                [PMUbyFileTemp,tPMU] = POWreadHQ(focusFile{idx3},Num_Flags);
            elseif(strcmpi(FileInfo(idx3).FileType, 'uPMUdat'))
                [PMUbyFileTemp,tPMU] = uPMUdatReader(focusFile{idx3},Num_Flags,FileLength);
            elseif(strcmpi(FileInfo(idx3).FileType, 'PI'))
                [PMUbyFileTemp,tPMU] = PIreaderDLL(focusFile{idx3},Num_Flags,FileLength,FileInfo(idx3).FileMnemonic,DataInfo.PresetFile);
            elseif(strcmpi(FileInfo(idx3).FileType, 'OpenHistorian'))
                [PMUbyFileTemp,tPMU] = OHreader(focusFile{idx3},Num_Flags,FileLength,FileInfo(idx3).FileMnemonic,DataInfo.PresetFile);
            elseif(strcmpi(FileInfo(idx3).FileType, 'openPDC'))
                [PMUbyFileTemp,tPMU] = openPDCreader(focusFile{idx3},Num_Flags,FileLength,FileInfo(idx3).FileMnemonic,DataInfo.PresetFile);
            end
            
            FailToRead = 0;
        catch
            FailToRead = 1;
        end
        
        if FailToRead == 0
            % Check for consistency in file length, which is assumed to
            % be a whole number of seconds
            ThisFileLength = round((tPMU(end)-tPMU(1) + tPMU(2)-tPMU(1))*24*60*60);
            if isempty(FileLength)
                % FileLength hasn't been established yet
                FileLength = ThisFileLength;
            elseif FileLength ~= ThisFileLength
                % The length of this file doesn't match previously
                % loaded files. Issue a warning and indicate that this
                % file should be set to NaN. Then continue to next
                % index so that the traits of this file do not get
                % stored.
                warning(['The length of ' focusFile{idx3} ' is inconsistent with other files. It will be excluded']);
                idx(idx3) = false;
                continue
            end
        else
            warning([focusFile{idx3} ' is possibly corrupt and could not be read.']);
            idx(idx3) = false;
            continue
        end
        
        % If possible, check to make sure the number of samples in this
        % file matches the previous file
        if ~isempty(FileInfo(idx3).tPMU)
            if length(FileInfo(idx3).tPMU) ~= length(tPMU)
                warning([focusFile{idx3} ' does not contain the right number of samples and will be skipped.']);
                idx(idx3) = false;
                continue
            end
        end

        % Store information for this file
        PMUbyFile{idx3} = PMUbyFileTemp;
        FileInfo(idx3).tPMU = tPMU;
        FileInfo(idx3).lastFocusFile = focusFile{idx3};
    end

    idx44 = find(~idx);
end

% For each directory where a file was not available for the earliest
% FocusFileTime, check to see if the corresponding entry in PMUbyFile
% is available. If it is, set all entries to NaN to indicate that the
% file was missing.
for idx4 = idx44
    if ~isempty(PMUbyFile{idx4})
        % Update timestamps
        FileInfo(idx4).tPMU = FileInfo(idx4).tPMU + (FileInfo(idx4).tPMU(end)-FileInfo(idx4).tPMU(1) + FileInfo(idx4).tPMU(2)-FileInfo(idx4).tPMU(1));
        tString = cellstr(datestr(FileInfo(idx4).tPMU,'yyyy-mm-dd HH:MM:SS.FFF'));
        
        % The structure for the files in this directory is available
        % from a previously loaded file. Set all entries to NaN
        if ~strcmp(PMUbyFile{idx4}(1).File_Name, 'Missing')
            % Entries were not set to NaN previously, so continue
            for pmuIdx = 1:length(PMUbyFile{idx4})
                PMUbyFile{idx4}(pmuIdx).File_Name = 'Missing';
                PMUbyFile{idx4}(pmuIdx).Flag(:) = 1;
                PMUbyFile{idx4}(pmuIdx).Data(:) = NaN;
                PMUbyFile{idx4}(pmuIdx).Stat(:) = NaN;
                PMUbyFile{idx4}(pmuIdx).Signal_Time.Signal_datenum = FileInfo(idx4).tPMU;
                PMUbyFile{idx4}(pmuIdx).Signal_Time.Time_String = tString;
            end
        else
            % Entries were set to NaN previously, but still need to update
            % the time stamps.
            for pmuIdx = 1:length(PMUbyFile{idx4})
                PMUbyFile{idx4}(pmuIdx).Signal_Time.Signal_datenum = FileInfo(idx4).tPMU;
                PMUbyFile{idx4}(pmuIdx).Signal_Time.Time_String = tString;
            end
        end
    end
end


% If there are not example structures available from all 
% directories then processing, detection, etc. cannot be
% implemented. Jump to the next index of the for loop.
% Once an entry in PMUbyFile has been filled it remains filled, so
% this only happens early as an archive is processed.
if sum(cellfun(@isempty,PMUbyFile)) > 0
    % For those where PMUbyFile is empty, FileInfo.tPMU has never
    % been set. But, FileInfo.lastFocusFile has been set, which
    % will cause a problem in getFocusFiles. tPMU cannot be made
    % up, because it impacts FileLength within getFocusFiles. So,
    % FileInfo.lastFocusFile must be set to empty. By doing
    % this, getFocusFiles will depend on DataInfo.DateTimeStart. So
    % DataInfo.DateTimeStart must be updated to that when
    % getFocusFiles is called it doesn't continue to start at the
    % beginning.

    for idx6 = find(cellfun(@isempty,PMUbyFile))
        FileInfo(idx6).lastFocusFile = [];
    end

    % Advance DateTimeStart to the top of the second following
    % the last sample that has been loaded. This is the same
    % value for all non-empty PMU structures in PMUbyFile, 
    % so the first is selected arbitrarily. (Assumes all 
    % directories have files covering the same time frames).
    try
        DataInfo.DateTimeStart = datestr(ceil(FileInfo(find(~cellfun(@isempty,PMUbyFile),1)).tPMU(end)*24*3600)/24/3600, 'yyyy-mm-dd HH:MM:SS');
    catch
        error('Processing cannot begin because none of the initial data files could be read. They may be corrupt or in an unsupported format.');
    end
    
    % This will cause a 'continue' to be executed in the FileIdx for loop
    % that this function is to be placed in.
    PMU = [];
else
    % Put the structures from PMUbyFile together.
    PMU = PMUbyFile{1};
    for idx5 = 2:length(PMUbyFile)
        PMU = [PMU PMUbyFile{idx5}];
    end
end