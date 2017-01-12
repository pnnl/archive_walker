function WriteEventListXML(EventList,XMLname,Lite)

%Open the output file handle
fHandle=fopen(XMLname,'wt');

%Write the header item
fprintf(fHandle,'<?xml version="1.0" encoding="UTF-8" ?>\n');

AllFields = fieldnames(EventList).';
for ThisField = AllFields
    % Ringdown portion
    if strfind(ThisField{1},'Ringdown')==1
        fprintf(fHandle,'<!-- *************** -->\n');
        fprintf(fHandle,'<!-- Ringdown Events -->\n');
        fprintf(fHandle,'<!-- *************** -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<Ringdown>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<Start>' datestr(EventList.(ThisField{1})(idx).Start, 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
            fprintf(fHandle,['\t<End>' datestr(EventList.(ThisField{1})(idx).End, 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
            
            if Lite == 0
                for idx2 = 1:length(EventList.(ThisField{1})(idx).Channel)
                    fprintf(fHandle,'\t<Channel>\n');
                    fprintf(fHandle,['\t\t<PMU>' EventList.(ThisField{1})(idx).PMU{idx2} '</PMU>\n']);
                    fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1})(idx).Channel{idx2} '</Name>\n']);
                    fprintf(fHandle,'\t</Channel>\n');
                end
            end

            fprintf(fHandle,'</Ringdown>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
    
    % Wind Ramp portion
    if strfind(ThisField{1},'WindRamp')==1
        fprintf(fHandle,'<!-- **************** -->\n');
        fprintf(fHandle,'<!-- Wind Ramp Events -->\n');
        fprintf(fHandle,'<!-- **************** -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<WindRamp>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<PMU>' EventList.(ThisField{1})(idx).PMU '</PMU>\n']);
            fprintf(fHandle,['\t<Channel>' EventList.(ThisField{1})(idx).Channel '</Channel>\n']);
            fprintf(fHandle,['\t<TrendStart>' EventList.(ThisField{1})(idx).TrendStart '</TrendStart>\n']);
            fprintf(fHandle,['\t<TrendEnd>' EventList.(ThisField{1})(idx).TrendEnd '</TrendEnd>\n']);
            fprintf(fHandle,['\t<TrendValue>' num2str(EventList.(ThisField{1})(idx).TrendValue) '</TrendValue>\n']);
            fprintf(fHandle,'</WindRamp>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
    
    % Forced Oscillation portion
    if strfind(ThisField{1},'ForcedOscillation')==1
        fprintf(fHandle,'<!-- ************************* -->\n');
        fprintf(fHandle,'<!-- Forced Oscillation Events -->\n');
        fprintf(fHandle,'<!-- ************************* -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<ForcedOscillation>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<OverallStart>' datestr(EventList.(ThisField{1})(idx).OverallStart, 'mm/dd/yy HH:MM:SS.FFF') '</OverallStart>\n']);
            fprintf(fHandle,['\t<OverallEnd>' datestr(EventList.(ThisField{1})(idx).OverallEnd, 'mm/dd/yy HH:MM:SS.FFF') '</OverallEnd>\n']);

            for idx2 = 1:length(EventList.(ThisField{1})(idx).Frequency)
                fprintf(fHandle,'\t<Occurrence>\n');
                fprintf(fHandle,['\t\t<OccurrenceID>' EventList.(ThisField{1})(idx).OccurrenceID{idx2} '</OccurrenceID>\n']);
                fprintf(fHandle,['\t\t<Frequency>' num2str(EventList.(ThisField{1})(idx).Frequency(idx2)) '</Frequency>\n']);
                fprintf(fHandle,['\t\t<Start>' datestr(EventList.(ThisField{1})(idx).Start(idx2), 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
                fprintf(fHandle,['\t\t<End>' datestr(EventList.(ThisField{1})(idx).End(idx2), 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
                fprintf(fHandle,['\t\t<Persistence>' num2str(EventList.(ThisField{1})(idx).Persistence(idx2)) '</Persistence>\n']);
                fprintf(fHandle,['\t\t<AlarmFlag>' num2str(EventList.(ThisField{1})(idx).AlarmFlag(idx2)) '</AlarmFlag>\n']);
                
                if Lite == 0
                    for idx3 = 1:length(EventList.(ThisField{1})(idx).Channel{idx2})
                        fprintf(fHandle,'\t\t<Channel>\n');
                        fprintf(fHandle,['\t\t\t<Name>' EventList.(ThisField{1})(idx).Channel{idx2}{idx3} '</Name>\n']);
                        fprintf(fHandle,['\t\t\t<Amplitude>' num2str(EventList.(ThisField{1})(idx).Amplitude{idx2}(idx3)) '</Amplitude>\n']);
                        fprintf(fHandle,['\t\t\t<SNR>' num2str(EventList.(ThisField{1})(idx).SNR{idx2}(idx3)) '</SNR>\n']);
                        fprintf(fHandle,['\t\t\t<Coherence>' num2str(EventList.(ThisField{1})(idx).Coherence{idx2}(idx3)) '</Coherence>\n']);
                        fprintf(fHandle,'\t\t</Channel>\n');
                    end
                end

                fprintf(fHandle,'\t</Occurrence>\n');
            end

            fprintf(fHandle,'</ForcedOscillation>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
    
    % Out Of Range Frequency portion
    if strfind(ThisField{1},'OutOfRangeFrequency')==1
        fprintf(fHandle,'<!-- ***************************** -->\n');
        fprintf(fHandle,'<!-- Out-of-Range Frequency Events -->\n');
        fprintf(fHandle,'<!-- ***************************** -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<OutOfRangeFrequency>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<Start>' datestr(EventList.(ThisField{1})(idx).Start, 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
            fprintf(fHandle,['\t<End>' datestr(EventList.(ThisField{1})(idx).End, 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
            
            if Lite == 0
                for idx2 = 1:length(EventList.(ThisField{1})(idx).Channel)
                    fprintf(fHandle,'\t<Channel>\n');
                    fprintf(fHandle,['\t\t<PMU>' EventList.(ThisField{1})(idx).PMU{idx2} '</PMU>\n']);
                    fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1})(idx).Channel{idx2} '</Name>\n']);
                    fprintf(fHandle,'\t</Channel>\n');
                end
            end

            fprintf(fHandle,'</OutOfRangeFrequency>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
    
    % Out Of Range General portion
    if strfind(ThisField{1},'OutOfRangeGeneral')==1
        fprintf(fHandle,'<!-- *************************** -->\n');
        fprintf(fHandle,'<!-- Out-of-Range General Events -->\n');
        fprintf(fHandle,'<!-- *************************** -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<OutOfRangeGeneral>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<Start>' datestr(EventList.(ThisField{1})(idx).Start, 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
            fprintf(fHandle,['\t<End>' datestr(EventList.(ThisField{1})(idx).End, 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
            
            if Lite == 0
                for idx2 = 1:length(EventList.(ThisField{1})(idx).Channel)
                    fprintf(fHandle,'\t<Channel>\n');
                    fprintf(fHandle,['\t\t<PMU>' EventList.(ThisField{1})(idx).PMU{idx2} '</PMU>\n']);
                    fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1})(idx).Channel{idx2} '</Name>\n']);
                    fprintf(fHandle,'\t</Channel>\n');
                end
            end

            fprintf(fHandle,'</OutOfRangeGeneral>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
end


%Close the file handle
fclose(fHandle);