function GenerateWindAppReport(ConfigFile,StartTime,EndTime,SortType,ReportPath,DataXML,FileDirectory,NumDQandCustomStages,InitializationPath,ProcessXML,NumProcessingStages,FlagBitInterpo,FlagBitInput,NumFlagsProcessor,PostProcessCustXML,NumPostProcessCustomStages,PMUbyFile,FileLength,Num_Flags,EventPath)

if exist(ReportPath,'file') ~= 0
    warning('Cannot overwrite existing file.');
    return
end

PMUfileDir = DataXML.ReaderProperties.FilePath;

% Midnight of each day specified by user
t = floor(datenum(StartTime)):datenum(EndTime);

% Iterate through the days, load the event XML, and store WindApp events.
WindApp = [];
for tidx = 1:length(t)
    EventXMLfile = [EventPath '\EventList_' datestr(t(tidx),'yymmdd') '.XML'];
    if exist(EventXMLfile,'file')
        % Read the XML
        EventXML = fun_xmlread_comments(EventXMLfile);
        % Convert to the EventList format used in Archive Walker
        EventXML = EventListXML2MAT(EventXML);
        
        if isfield(EventXML,'WindApp')
            WindApp = [WindApp EventXML.WindApp];
        end
    end
end

% Load the current event XML and store WindApp events.
EventXMLfile = [EventPath '\EventList_Current.XML'];
% Read the XML
EventXML = fun_xmlread_comments(EventXMLfile);
% Convert to the EventList format used in Archive Walker
EventXML = EventListXML2MAT(EventXML);
%
if isfield(EventXML,'WindApp')
    WindApp = [WindApp EventXML.WindApp];
end

if isempty(WindApp)
    return
end


% Remove any events with duplicate IDs (this happens when an event occurs
% on two days)
[~,uIdx] = unique({WindApp.ID});
WindApp = WindApp(uIdx);


% Sort the events
if strcmp(SortType,'EventSize')
    [~,SortIdx] = sort([WindApp.ExtremaFactor],'descend');
elseif strcmp(SortType,'NewestFirst')
    [~,SortIdx] = sort([WindApp.Start],'descend');
elseif strcmp(SortType,'NewestLast')
    [~,SortIdx] = sort([WindApp.Start],'ascend');
else
    warning('SortType was not specified correctly. Options are: EventSize, NewestFirst, and NewestLast. Events will not be sorted.');
    SortIdx = 1:length(WindApp);
end
WindApp = WindApp(SortIdx);


% Iterate through the events and remove those that don't fit the time
% criteria
KillIdx = [];
for Eidx = 1:length(WindApp)
    if (WindApp(Eidx).End < datenum(StartTime)) || (datenum(EndTime) < WindApp(Eidx).Start)
        KillIdx = [KillIdx Eidx];
    end
end
WindApp(KillIdx) = [];
if isempty(WindApp)
    warning('No wind application events fit the specified time range. No report generated.');
    return
end


Hist = 31;
[exMean, eSep, pSep, Cat] = WindAppCatAndRank(EventPath,Hist);
% Iterate through the events and assign category and rank
EventRank = zeros(1,length(WindApp));
EventCat = cell(1,length(WindApp));
for Eidx = 1:length(WindApp)
    MaxWindPower = 0;
    for SigIdx = 1:length(WindApp(Eidx).WindPower)
        WindPower = strsplit(WindApp(Eidx).WindPower{SigIdx},',');
        MaxWindPower = max([MaxWindPower abs(str2double(WindPower{3}))]);
    end
    
    [~,CatIdx] = min(abs(exMean-WindApp(Eidx).Extrema));
    EventCat{Eidx} = Cat{CatIdx};
    
    if WindApp(Eidx).ExtremaFactor > eSep(CatIdx)
        % Rank 1 or 2
        if MaxWindPower > pSep(CatIdx)
            % Rank 1
            EventRank(Eidx) = 1;
        else
            % Rank 2
            EventRank(Eidx) = 2;
        end
    else
        % Rank 3 or 4
        if MaxWindPower > pSep(CatIdx)
            % Rank 3
            EventRank(Eidx) = 3;
        else
            % Rank 4
            EventRank(Eidx) = 4;
        end
    end
end


% Start the report
word = actxserver('Word.Application');      %start Word
% word.Visible =1;                            %make Word Visible
document=word.Documents.Add;                %create new Document
selection=word.Selection;                   %set Cursor
selection.Font.Name='Courier New';          %set Font

% Add a title
selection.ParagraphFormat.Alignment =1;     % Center alignment 
selection.Font.Size=16;                      %set Size
selection.Font.Bold=1;  
selection.TypeText('Wind Application Report');
selection.TypeParagraph;                     %line break

selection.Font.Size=12;                      %set Size
selection.Font.Bold=0;  

selection.TypeText(['Report Start: ' StartTime]);
selection.TypeParagraph;
selection.TypeText(['Report End:   ' EndTime]);
selection.TypeParagraph;
selection.TypeText(['Configuration File: ' ConfigFile]);
selection.ParagraphFormat.Alignment =0;     % Left alignment
selection.TypeParagraph;
selection.TypeParagraph;
selection.TypeParagraph;
selection.TypeText(['Events in this report are divided by rank and category. Rank refers to the significance of the event.'...
    ' Events with large deviations of voltage/frequency from normal and high MW output are denoted as Rank 1 events.'...
    ' Rank 4 events are the least significant. See the table below. Events are also categorized as Frequency, 230 kV, 500 kV, or Other'...
    ' based on the value of the signals involved in the event. ']);
selection.TypeParagraph;
selection.TypeParagraph;
selection.TypeText('                      | Rank 2        Rank 1');
selection.TypeParagraph;
selection.TypeText('Severity (Hz/kV/etc)  |');
selection.TypeParagraph;
selection.TypeText('                      | Rank 4        Rank 3');
selection.TypeParagraph;
selection.TypeText('                      ----------------------');
selection.TypeParagraph;
selection.TypeText('                            MW Output');
selection.TypeParagraph;
selection.TypeParagraph;
selection.TypeParagraph;
selection.TypeText('Note: Wind plant output at the time of the event is shown in the Wind Signals legend, appended to the right of the name.');



for RankIdx = unique(EventRank)
    for ThisCat = {'Frequency', '500 kV', '230 kV', 'Other'}
        EventMatch = (EventRank==RankIdx);
        CatMatch = strcmp(ThisCat{1},EventCat);
        if sum(EventMatch & CatMatch) > 0
            selection.InsertBreak(7);
            selection.ParagraphFormat.Alignment =1;     % Center alignment 
            selection.Font.Size=16;                      %set Size
            selection.Font.Bold=1;  
            selection.TypeText(['Rank ' num2str(RankIdx) ' Events']);
            selection.TypeParagraph;
            selection.TypeText(['Category: ' ThisCat{1}]);
            selection.TypeParagraph;                     %line break

            selection.Font.Size=12;                      %set Size
            selection.Font.Bold=0;
            selection.ParagraphFormat.Alignment =0;     % Left alignment
        else
            continue;
        end
        
        % Iterate through the events and generate plots
        for Eidx = find(EventMatch & CatMatch)
            disp(['Writing event ' num2str(Eidx) ' of ' num2str(length(WindApp))]);

            selection.InsertBreak(7);
            selection.TypeText(['Start Time: ' datestr(WindApp(Eidx).Start,'yyyy-mm-dd HH:MM:SS')]);
            selection.TypeParagraph;  
            selection.TypeText(['End Time:   ' datestr(WindApp(Eidx).End,'yyyy-mm-dd HH:MM:SS')]);
            selection.TypeParagraph;


            % Add the file length input.
            % Find the latest start file and the earliest end file
                % start file must be before event
                % end file must be after event
            % Using the file length, walk from start file to end file
            startPMUfilesKeepTimesAll = [];
            endPMUfilesKeepTimesAll = [];
            for DirectoryIdx = 1:length(PMUfileDir)
                startPMUdayFilePath = [FileDirectory{DirectoryIdx} '\' datestr(WindApp(Eidx).Start,'yyyy') '\' datestr(WindApp(Eidx).Start,'yymmdd')];
                PMUfiles = dir(startPMUdayFilePath);
                startPMUfilesKeep = {PMUfiles.name};
                startPMUfilesKeep = startPMUfilesKeep(~[PMUfiles.isdir]);
                startPMUfilesKeepTimes = GetTimesFromNames(startPMUfilesKeep);
                startPMUfilesKeepTimes = sort(startPMUfilesKeepTimes);
                StartFile = find(startPMUfilesKeepTimes > WindApp(Eidx).Start,1)-1;
                startPMUfilesKeepTimesAll = [startPMUfilesKeepTimesAll startPMUfilesKeepTimes(StartFile)];


                endPMUdayFilePath = [FileDirectory{DirectoryIdx} '\' datestr(WindApp(Eidx).End,'yyyy') '\' datestr(WindApp(Eidx).End,'yymmdd')];
                PMUfiles = dir(endPMUdayFilePath);
                endPMUfilesKeep = {PMUfiles.name};
                endPMUfilesKeep = endPMUfilesKeep(~[PMUfiles.isdir]);
                endPMUfilesKeepTimes = GetTimesFromNames(endPMUfilesKeep);
                endPMUfilesKeepTimes = sort(endPMUfilesKeepTimes);
                if WindApp(Eidx).End > endPMUfilesKeepTimes(end)
                    EndFile = length(endPMUfilesKeepTimes);
                else
                    EndFile = find(endPMUfilesKeepTimes > WindApp(Eidx).End,1)-1;
                end
                endPMUfilesKeepTimesAll = [endPMUfilesKeepTimesAll endPMUfilesKeepTimes(EndFile)];
            end
            startPMUfilesKeepTimesAll = max(startPMUfilesKeepTimesAll);
            endPMUfilesKeepTimesAll = min(endPMUfilesKeepTimesAll);
            PMUfileTimes = startPMUfilesKeepTimesAll:(FileLength/(60*60*24)):endPMUfilesKeepTimesAll;

            if length(PMUfileTimes) > 10
                selection.TypeText(['This event would require loading ' num2str(length(PMUfileTimes)) ' files, only first 10 will be be plotted.'])
                selection.TypeParagraph;
                PMUfileTimes = PMUfileTimes(1:10);
            end

            PMU = [];
            for DirectoryIdx = 1:length(PMUfileDir)
                PMUconcat = [];
                for FileIdx = 1:length(PMUfileTimes)
                    yyyymmdd = datestr(PMUfileTimes(FileIdx),'yyyymmdd');
                    hhmmss = datestr(PMUfileTimes(FileIdx),'HHMMSS');

                    PMUfilePath = [FileDirectory{DirectoryIdx} '\' yyyymmdd(1:4) '\' yyyymmdd(3:8) '\' PMUfileDir{DirectoryIdx}.Mnemonic '_' yyyymmdd '_' hhmmss '.' PMUfileDir{DirectoryIdx}.FileType];
                    try
                        if(strcmpi(PMUfileDir{DirectoryIdx}.FileType, 'pdat'))
                            % pdat format
                            PMUtemp = pdatReadnCreateStruct(PMUfilePath,Num_Flags,FileLength);
                        elseif(strcmpi(PMUfileDir{DirectoryIdx}.FileType, 'csv'))
                            % JSIS_CSV format
                            PMUtemp = JSIS_CSV_2_Mat(PMUfilePath,Num_Flags);
                        elseif(strcmpi(FileInfo(idx3).FileType, 'powHQ'))
                            PMUtemp = POWreadHQ(PMUfilePath,Num_Flags);
                        elseif(strcmpi(FileInfo(idx3).FileType, 'uPMUdat'))
                            PMUtemp = uPMUdatReader(PMUfilePath,Num_Flags,FileLength);
                        end

                        PMUconcat = ConcatenatePMU(PMUconcat,PMUtemp);
                    catch
                        % If the file couldn't be loaded because it was missing or
                        % corrupt, replace with NaN
                        PMUtemp = PMUbyFile{DirectoryIdx};
                        for pmuIdx = 1:length(PMUtemp)
                            PMUtemp(pmuIdx).File_Name = 'Missing';
                            PMUtemp(pmuIdx).Flag(:) = 1;
                            PMUtemp(pmuIdx).Data(:) = NaN;
                            PMUtemp(pmuIdx).Stat(:) = NaN;
                            if pmuIdx == 1
                                PMUtemp(pmuIdx).Signal_Time.Signal_datenum = PMUfileTimes(FileIdx) + linspace(0,FileLength,size(PMUtemp(pmuIdx).Data,1)).'/(60/60/24);
                                PMUtemp(pmuIdx).Signal_Time.Time_String = cellstr(datestr(PMUtemp(pmuIdx).Signal_Time.Signal_datenum,'mm/dd/yy HH:MM:SS.FFF'));
                                PMUtemp(pmuIdx).Signal_Time.datetime = datetime(PMUtemp(pmuIdx).Signal_Time.Signal_datenum,'ConvertFrom','datenum','Format','MM/dd/yy HH:mm:ss.SSSSSS');
                            else
                                PMUtemp(pmuIdx).Signal_Time.Signal_datenum = PMUtemp(1).Signal_Time.Signal_datenum;
                                PMUtemp(pmuIdx).Signal_Time.Time_String = PMUtemp(1).Signal_Time.Time_String;
                                PMUtemp(pmuIdx).Signal_Time.datetime = PMUtemp(1).Signal_Time.datetime;
                            end
                        end

                        PMUconcat = ConcatenatePMU(PMUconcat,PMUtemp);
                    end
                end
                PMU = [PMU PMUconcat];
            end


            % Apply data quality filters and signal customizations
            PMU = DQandCustomization(PMU,DataXML,NumDQandCustomStages,Num_Flags);
            % Return only the desired PMUs and signals
            PMU = GetOutputSignals(PMU,DataXML);
            % Load Initialization file
            yyyymmdd = datestr(PMUfileTimes(1),'yyyymmdd');
            hhmmss = datestr(PMUfileTimes(1),'HHMMSS');
            InitializationFilePath = [InitializationPath '\' yyyymmdd(1:4) '\' yyyymmdd(3:8) '\'];
            InitializationFile = [InitializationFilePath 'Initialization_' yyyymmdd '_' hhmmss '.mat'];
            load(InitializationFile);
            % Processing
            PMU = DataProcessor(PMU, ProcessXML, NumProcessingStages, FlagBitInterpo,FlagBitInput,NumFlagsProcessor,InitialCondosFilter,InitialCondosMultiRate,FinalAngles);
            % Return only the desired PMUs and signals
            PMU = GetOutputSignals(PMU, ProcessXML);
            % Post-Processing Customization
            PMU = DQandCustomization(PMU,PostProcessCustXML,NumPostProcessCustomStages,Num_Flags);


            f1 = figure('PaperPosition',[.25 .25 8 5],'visible','off'); hold on;
            for SigIdx = 1:length(WindApp(Eidx).PMU)
                PMUidx = strcmp({PMU.PMU_Name},WindApp(Eidx).PMU{SigIdx});
                ChanIdx = strcmp(PMU(PMUidx).Signal_Name,WindApp(Eidx).Channel{SigIdx});
                tPlot = datetime(datestr(PMU(PMUidx).Signal_Time.Signal_datenum,'yyyy-mm-dd HH:MM:SS.FFF'),'InputFormat','yyyy-MM-dd HH:mm:ss.SSS');
                ChanPlot = PMU(PMUidx).Data(:,ChanIdx);
                plot(tPlot,ChanPlot,'LineWidth',2);
            end
            hold off;
            ylabel(PMU(PMUidx).Signal_Unit{ChanIdx});
            legend(strcat(WindApp(Eidx).PMU,',',WindApp(Eidx).Channel),'Interpreter','none','Location','southoutside');
            title('Event Signals');
            set(gca,'FontSize',14)
            set(gca,'box','on')

            f2 = figure('PaperPosition',[.25 .25 8 5],'visible','off');
            f2lgnd = {};
            f3 = figure('PaperPosition',[.25 .25 8 5],'visible','off');
            f3lgnd = {};
            Plotf3 = false;
            for SigIdx = 1:length(WindApp(Eidx).WindPower)
                WindPower = strsplit(WindApp(Eidx).WindPower{SigIdx},',');
                PMUidx = strcmp({PMU.PMU_Name},WindPower{1});
                ChanIdx = strcmp(PMU(PMUidx).Signal_Name,WindPower{2});
                tPlot = datetime(datestr(PMU(PMUidx).Signal_Time.Signal_datenum,'yyyy-mm-dd HH:MM:SS.FFF'),'InputFormat','yyyy-MM-dd HH:mm:ss.SSS');
                if strcmp(PMU(PMUidx).Signal_Unit{ChanIdx},'MW')
                    f2lgnd{end+1} = WindApp(Eidx).WindPower{SigIdx};
                    figure(f2); hold on;
                elseif strcmp(PMU(PMUidx).Signal_Unit{ChanIdx},'MVAR')
                    f3lgnd{end+1} = WindApp(Eidx).WindPower{SigIdx};
                    figure(f3); hold on;
                    Plotf3 = true;
                end
                plot(tPlot,abs(PMU(PMUidx).Data(:,ChanIdx)),'LineWidth',2); hold off;
            end
            figure(f2);
            ylabel('MW');
            legend(f2lgnd,'Interpreter','none','Location','southoutside');
            title('Wind Signals - Active Power')
            set(gca,'FontSize',14)
            set(gca,'box','on')
            %
            figure(f3);
            ylabel('MVAR');
            legend(f3lgnd,'Interpreter','none','Location','southoutside');
            title('Wind Signals - Reactive Power')
            set(gca,'FontSize',14)
            set(gca,'box','on')

            % Add figures to report
            print(f1,'-dmeta');                 %print figure to clipboard
            invoke(word.Selection,'Paste');             %paste figure to Word
            close(f1);

            selection.TypeParagraph;

            print(f2,'-dmeta');                 %print figure to clipboard
            invoke(word.Selection,'Paste');             %paste figure to Word
            close(f2);

            selection.TypeParagraph;

            if Plotf3
                print(f3,'-dmeta');                 %print figure to clipboard
                invoke(word.Selection,'Paste');             %paste figure to Word
            end
            close(f3);
        end
    end
end

% Close the report
% document.SaveAs2(ReportPath);         %save Document
invoke(document,'SaveAs',ReportPath);
word.Quit();

end

function T = GetTimesFromNames(FileNames)
T = zeros(1,length(FileNames));
for idx = 1:length(FileNames)
    FileParts = strsplit(FileNames{idx},'_');
    T(idx) = datenum([FileParts{2} FileParts{3}(1:6)],'yyyymmddHHMMSS');
end
end