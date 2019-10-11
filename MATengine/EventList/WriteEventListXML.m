function WriteEventListXML(EventList,XMLname,Lite)

%Open the output file handle
fHandle=fopen(XMLname,'wt');

%Write the header item
fprintf(fHandle,'<?xml version="1.0" encoding="UTF-8" ?>\n');
fprintf(fHandle,'<Events>\n');

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
            fprintf(fHandle,['\t<ValueStart>' num2str(EventList.(ThisField{1})(idx).ValueStart) '</ValueStart>\n']);
            fprintf(fHandle,['\t<ValueEnd>' num2str(EventList.(ThisField{1})(idx).ValueEnd) '</ValueEnd>\n']);
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
                        fprintf(fHandle,['\t\t\t<PMU>' EventList.(ThisField{1})(idx).PMU{idx2}{idx3} '</PMU>\n']);
                        fprintf(fHandle,['\t\t\t<Unit>' EventList.(ThisField{1})(idx).Unit{idx2}{idx3} '</Unit>\n']);
                        fprintf(fHandle,['\t\t\t<Amplitude>' num2str(EventList.(ThisField{1})(idx).Amplitude{idx2}(idx3)) '</Amplitude>\n']);
                        fprintf(fHandle,['\t\t\t<SNR>' num2str(EventList.(ThisField{1})(idx).SNR{idx2}(idx3)) '</SNR>\n']);
                        fprintf(fHandle,['\t\t\t<Coherence>' num2str(EventList.(ThisField{1})(idx).Coherence{idx2}(idx3)) '</Coherence>\n']);
                        fprintf(fHandle,'\t\t</Channel>\n');
                    end
                end
                
                % Add DEF information
                if isfield(EventList.(ThisField{1})(idx),'DEF')
                    for pIdx = 1:size(EventList.(ThisField{1})(idx).DEF,1)
                        fprintf(fHandle,'\t\t<Path>\n');
                        fprintf(fHandle,['\t\t\t<From>' EventList.(ThisField{1})(idx).PathDescription{1,pIdx} '</From>\n']);
                        fprintf(fHandle,['\t\t\t<To>' EventList.(ThisField{1})(idx).PathDescription{2,pIdx} '</To>\n']);
                        fprintf(fHandle,['\t\t\t<DEF>' num2str(EventList.(ThisField{1})(idx).DEF(pIdx,idx2)) '</DEF>\n']);
                        fprintf(fHandle,'\t\t</Path>\n');
                    end
                end

                fprintf(fHandle,'\t</Occurrence>\n');
            end

            fprintf(fHandle,'</ForcedOscillation>\n');
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
            fprintf(fHandle,['\t<Extrema>' num2str(EventList.(ThisField{1})(idx).Extrema) '</Extrema>\n']);
            fprintf(fHandle,['\t<ExtremaFactor>' num2str(EventList.(ThisField{1})(idx).ExtremaFactor) '</ExtremaFactor>\n']);
            
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
    
    % Thevenin portion
    if strfind(ThisField{1},'Thevenin')==1
        fprintf(fHandle,'<!-- ************************ -->\n');
        fprintf(fHandle,'<!-- Voltage Stability Events -->\n');
        fprintf(fHandle,'<!-- ************************ -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<Thevenin>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<Start>' datestr(EventList.(ThisField{1})(idx).Start, 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
            fprintf(fHandle,['\t<End>' datestr(EventList.(ThisField{1})(idx).End, 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
            
            for idx2 = 1:length(EventList.(ThisField{1})(idx).Sub)
                fprintf(fHandle,'\t<Sub>\n');
                fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1})(idx).Sub{idx2} '</Name>\n']);
                fprintf(fHandle,'\t</Sub>\n');
            end
            
            fprintf(fHandle,'</Thevenin>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
%     
%     % Modemeter portion
%     if strfind(ThisField{1},'ModeMeter')==1
%         fprintf(fHandle,'<!-- *********************** -->\n');
%         fprintf(fHandle,'<!-- ModeMeter Events -->\n');
%         fprintf(fHandle,'<!-- *********************** -->\n');
% %         for idx = 1:length(EventList.(ThisField{1}).Mode)
% %             fprintf(fHandle,'<Mode>\n');
% %             fprintf(fHandle,['\t<ID>' EventList.(ThisField{1}).Mode(idx).ID '</ID>\n']);
% %             fprintf(fHandle,['\t<Start>' datestr(EventList.(ThisField{1}).Mode(idx).Start, 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
% %             fprintf(fHandle,['\t<End>' datestr(EventList.(ThisField{1}).Mode(idx).End, 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
% %             for idx2 = 1:length(EventList.(ThisField{1}).Mode(idx).Channel)
% %                 fprintf(fHandle,'\t<Channel>\n');
% %                 %                 fprintf(fHandle,['\t\t<PMU>' EventList.(ThisField{1})(idx).PMU{idx2} '</PMU>\n']);
% %                 fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1}).Mode(idx).Channel{idx2} '</Name>\n']);
% %                 for idx3 = 1:length(EventList.(ThisField{1}).Mode(idx).Channel)
% %                     fprintf(fHandle,'\t<Methods>\n');
% %                     %                     fprintf(fHandle,['\t\t<PMU>' EventList.(ThisField{1})Mode(idx).PMU{idx2} '</PMU>\n']);
% %                     fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1}).Mode(idx).Channel{idx2}.MethodName(idx3) '</Name>\n']);
% %                     printf(fHandle,'\t<Methods>\n');
% %                 end
% %                 fprintf(fHandle,'\t</Channel>\n');
% %             end
% %         end
%         fprintf(fHandle,'</Mode>\n');
%         fprintf(fHandle,'<!-- -->\n');
%     end
%     
    
    % Wind Application portion
    if strfind(ThisField{1},'WindApp')==1
        fprintf(fHandle,'<!-- *********************** -->\n');
        fprintf(fHandle,'<!-- Wind Application Events -->\n');
        fprintf(fHandle,'<!-- *********************** -->\n');
        for idx = 1:length(EventList.(ThisField{1}))
            fprintf(fHandle,'<WindApp>\n');
            fprintf(fHandle,['\t<ID>' EventList.(ThisField{1})(idx).ID '</ID>\n']);
            fprintf(fHandle,['\t<Start>' datestr(EventList.(ThisField{1})(idx).Start, 'mm/dd/yy HH:MM:SS.FFF') '</Start>\n']);
            fprintf(fHandle,['\t<End>' datestr(EventList.(ThisField{1})(idx).End, 'mm/dd/yy HH:MM:SS.FFF') '</End>\n']);
            fprintf(fHandle,['\t<Extrema>' num2str(EventList.(ThisField{1})(idx).Extrema) '</Extrema>\n']);
            fprintf(fHandle,['\t<ExtremaFactor>' num2str(EventList.(ThisField{1})(idx).ExtremaFactor) '</ExtremaFactor>\n']);
            
            if Lite == 0
                for idx2 = 1:length(EventList.(ThisField{1})(idx).Channel)
                    fprintf(fHandle,'\t<Channel>\n');
                    fprintf(fHandle,['\t\t<PMU>' EventList.(ThisField{1})(idx).PMU{idx2} '</PMU>\n']);
                    fprintf(fHandle,['\t\t<Name>' EventList.(ThisField{1})(idx).Channel{idx2} '</Name>\n']);
                    fprintf(fHandle,'\t</Channel>\n');
                end
            end
            
            for idx2 = 1:length(EventList.(ThisField{1})(idx).WindPower)
                fprintf(fHandle,'\t<WindPower>\n');
                fprintf(fHandle,['\t\t<Info>' EventList.(ThisField{1})(idx).WindPower{idx2} '</Info>\n']);
                fprintf(fHandle,'\t</WindPower>\n');
            end

            fprintf(fHandle,'</WindApp>\n');
            fprintf(fHandle,'<!-- -->\n');
        end
    end
end
fprintf(fHandle,'</Events>\n');

%Close the file handle
fclose(fHandle);