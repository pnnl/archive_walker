function WriteMMreportGraphical(StartTime,EndTime,DampThresh,T,mmPlotInfo,OpConPlotInfo,ReportPath)

%% Start the report
word = actxserver('Word.Application');      %start Word
% word.Visible =1;                            %make Word Visible
document=word.Documents.Add;                %create new Document
selection=word.Selection;                   %set Cursor
selection.Font.Name='Courier New';          %set Font

% Add a title
selection.ParagraphFormat.Alignment =1;     % Center alignment 
selection.Font.Size=16;                      %set Size
selection.Font.Bold=1;  
selection.TypeText('Mode Meter Report');
selection.TypeParagraph;                     %line break

selection.Font.Size=12;                      %set Size
selection.Font.Bold=0;  

selection.TypeText([StartTime ' - ' EndTime]);
selection.TypeParagraph;
selection.TypeText(['Damping Ratio Threshold: ' num2str(DampThresh) '%']);
selection.TypeParagraph;
selection.ParagraphFormat.Alignment =0;     % Left alignment
%%


Modes = {mmPlotInfo.Mode};

for eIdx = 1:size(T,1)
    tSt = T{eIdx,'Start'};
    tEn = T{eIdx,'End'};
    
    % Amount to add to the plot. Specified as a fraction of the event's
    % duration in days.
    ExtraWindow = 0.1*T{eIdx,'DurationHours'}/24;
    
    %% Write event overview  information
    selection.InsertBreak(7);   % New page
    
    selection.TypeText(['Low Damping Period: ' num2str(eIdx)]);
    selection.TypeParagraph;  
    selection.TypeText(['Start Time:         ' datestr(tSt)]);
    selection.TypeParagraph;  
    selection.TypeText(['End Time:           ' datestr(tEn)]);
    selection.TypeParagraph;
    selection.TypeText(['Duration:           ' num2str(T{eIdx,'DurationHours'}) ' Hours']);
    selection.TypeParagraph;
    selection.TypeText(['Mode:               ' T{eIdx,'Mode'}{1}]);
    selection.TypeParagraph;
    selection.TypeText(['Minimum Damping:    ' num2str(T{eIdx,'MinDamping'}) ' %']);
    selection.TypeParagraph;
    
    %% Add Plots
    
    % Special handling for events detected at one timestamp
    if tSt == tEn
        ExtraWindow = 0.25;
    end
    
    for PlotIdx = find(strcmp(Modes,T{eIdx,'Mode'}))
        xIdx = nearTime(mmPlotInfo(PlotIdx).Time,tSt-ExtraWindow):nearTime(mmPlotInfo(PlotIdx).Time,tEn+ExtraWindow);
        
        f1 = figure('PaperPosition',[.25 .25 8 5],'visible','off');
        plot(mmPlotInfo(PlotIdx).Time(xIdx),mmPlotInfo(PlotIdx).Data(xIdx,:),'LineWidth',2)
        ylabel(mmPlotInfo(PlotIdx).Ylabel);
        title(mmPlotInfo(PlotIdx).Title,'Interpreter','none');
        legend(mmPlotInfo(PlotIdx).SignalNames,'Interpreter','none');
        set(gca,'FontSize',14);
        set(gca,'box','on')
        
        print(f1,'-dmeta');                 %print figure to clipboard
        invoke(word.Selection,'Paste');             %paste figure to Word
        close(f1);
        selection.TypeParagraph;
    end
    
    for PlotIdx = 1:length(OpConPlotInfo)
        xIdx = nearTime(OpConPlotInfo(PlotIdx).Time,tSt-ExtraWindow):nearTime(OpConPlotInfo(PlotIdx).Time,tEn+ExtraWindow);
        
        f1 = figure('PaperPosition',[.25 .25 8 5],'visible','off');
        plot(OpConPlotInfo(PlotIdx).Time(xIdx),OpConPlotInfo(PlotIdx).Data(xIdx,:),'LineWidth',2)
        ylabel(OpConPlotInfo(PlotIdx).Ylabel);
        title(OpConPlotInfo(PlotIdx).Title,'Interpreter','none');
        legend(OpConPlotInfo(PlotIdx).SignalNames,'Interpreter','none');
        set(gca,'FontSize',14);
        set(gca,'box','on')
        
        print(f1,'-dmeta');                 %print figure to clipboard
        invoke(word.Selection,'Paste');             %paste figure to Word
        close(f1);
        selection.TypeParagraph;
    end
end

% Close the report
invoke(document,'SaveAs',ReportPath);
word.Quit();

end

function k = nearTime(tvec,val)

[~,k] = min(abs(seconds(tvec - val)));

end