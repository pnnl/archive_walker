﻿Imports System.Globalization
Imports BAWGUI.Core
Imports BAWGUI.Settings.ViewModels
Imports BAWGUI.SignalManagement.ViewModels
Imports VoltageStability.ViewModels

Namespace ViewModels
    Public Class ConfigFileWriter
        Private _svm As SettingsViewModel
        Private _powerTypeDictionary As Dictionary(Of String, String)
        Private _settingsVM As SettingsViewModel
        Private _dqFilterCounter As Integer = 0
        'Public Property DQFilterCounter() As Integer = 0
        '    Get
        '        Return _dqFilterCounter
        '    End Get
        '    Set(ByVal value As Integer = 0)
        '        _dqFilterCounter = value
        '    End Set
        'End Property
        Private _saveToRun As AWRun
        Public Property SaveToRun As AWRun
            Get
                Return _saveToRun
            End Get
            Set(ByVal value As AWRun)
                _saveToRun = value
            End Set
        End Property

        Public Sub New(svm As SettingsViewModel, run As AWRun)
            _svm = svm
            _saveToRun = run
            _powerTypeDictionary = New Dictionary(Of String, String) From {{"Complex", "CP"}, {"Apparent", "S"}, {"Active", "P"}, {"Reactive", "Q"}}
        End Sub

        'Public Sub ConfigFilewriter(svm As SettingsViewModel)
        '    _svm = svm
        '    _powerTypeDictionary = New Dictionary(Of String, String) From {{"Complex", "CP"}, {"Apparent", "S"}, {"Active", "P"}, {"Reactive", "Q"}}
        'End Sub

        Public Sub WriteXmlConfigFile(filename As String)
            Dim _configData As XElement = <Config></Config>
            Dim dataConfig As XElement = <DataConfig>
                                             <Configuration>
                                                 <ReaderProperties></ReaderProperties>
                                             </Configuration>
                                         </DataConfig>
            For Each fileInfo In _svm.DataConfigure.ReaderProperty.InputFileInfos
                Dim info As XElement = <FilePath>
                                           <ExampleFile><%= fileInfo.ExampleFile %></ExampleFile>
                                           <FileDirectory><%= fileInfo.FileDirectory %></FileDirectory>
                                           <FileType><%= fileInfo.FileType %></FileType>
                                           <Mnemonic><%= fileInfo.Mnemonic %></Mnemonic>
                                       </FilePath>
                dataConfig.<Configuration>.<ReaderProperties>.LastOrDefault.Add(info)
            Next
            Dim mode As XElement = <Mode>
                                       <Name><%= _svm.DataConfigure.ReaderProperty.ModeName %></Name>
                                   </Mode>
            Dim dtStart, dtEnd As DateTime
            Select Case _svm.DataConfigure.ReaderProperty.ModeName
                Case ModeType.Archive
                    Try
                        dtStart = DateTime.Parse(_svm.DataConfigure.ReaderProperty.DateTimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
                    Catch ex As Exception
                        dtStart = DateTime.Now
                        'Throw New Exception("Error parsing start time.")
                    End Try
                    Try
                        dtEnd = DateTime.Parse(_svm.DataConfigure.ReaderProperty.DateTimeEnd, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
                    Catch ex As Exception
                        dtEnd = DateTime.Now
                        'Throw New Exception("Error parsing end time.")
                    End Try
                    Dim dtStringStart = dtStart.ToString("yyyy-MM-dd HH:mm:ss") & " GMT"
                    Dim dtStringEnd = dtEnd.ToString("yyyy-MM-dd HH:mm:ss") & " GMT"
                    Dim parameters As XElement = <Params>
                                                     <DateTimeStart><%= dtStringStart %></DateTimeStart>
                                                     <DateTimeEnd><%= dtStringEnd %></DateTimeEnd>
                                                 </Params>
                    mode.Add(parameters)
                Case ModeType.Hybrid
                    Try
                        dtStart = DateTime.Parse(_svm.DataConfigure.ReaderProperty.DateTimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
                    Catch ex As Exception
                        Throw New Exception("Error parsing start time.")
                    End Try
                    Dim dtStringStart = dtStart.ToString("yyyy-MM-dd HH:mm:ss") & " GMT"
                    Dim parameters As XElement = <Params>
                                                     <DateTimeStart><%= dtStringStart %></DateTimeStart>
                                                     <NoFutureWait><%= _svm.DataConfigure.ReaderProperty.NoFutureWait %></NoFutureWait>
                                                     <MaxNoFutureCount><%= _svm.DataConfigure.ReaderProperty.MaxNoFutureCount %></MaxNoFutureCount>
                                                     <FutureWait><%= _svm.DataConfigure.ReaderProperty.FutureWait %></FutureWait>
                                                     <MaxFutureCount><%= _svm.DataConfigure.ReaderProperty.MaxFutureCount %></MaxFutureCount>
                                                     <RealTimeRange><%= _svm.DataConfigure.ReaderProperty.RealTimeRange %></RealTimeRange>
                                                 </Params>
                    mode.Add(parameters)
                Case ModeType.RealTime
                    Dim parameters As XElement = <Params>
                                                     <NoFutureWait><%= _svm.DataConfigure.ReaderProperty.NoFutureWait %></NoFutureWait>
                                                     <MaxNoFutureCount><%= _svm.DataConfigure.ReaderProperty.MaxNoFutureCount %></MaxNoFutureCount>
                                                     <FutureWait><%= _svm.DataConfigure.ReaderProperty.FutureWait %></FutureWait>
                                                     <MaxFutureCount><%= _svm.DataConfigure.ReaderProperty.MaxFutureCount %></MaxFutureCount>
                                                 </Params>
                    mode.Add(parameters)
            End Select
            dataConfig.<Configuration>.<ReaderProperties>.LastOrDefault.Add(mode)
            Dim aStep As XElement
            Dim stage As XElement = <Stages></Stages>
            Dim newStageFlag = True
            For Each singleStep In _svm.DataConfigure.CollectionOfSteps
                If TypeOf singleStep Is Customization AndAlso newStageFlag Then
                    newStageFlag = False
                ElseIf Not newStageFlag AndAlso TypeOf singleStep Is DQFilter Then
                    dataConfig.<Configuration>.LastOrDefault.Add(stage)
                    stage = <Stages></Stages>
                    newStageFlag = True
                End If
                _writeAStep(aStep, singleStep)
                stage.Add(aStep)
            Next
            dataConfig.<Configuration>.LastOrDefault.Add(stage)
            Dim signalSelection As XElement = <SignalSelection></SignalSelection>
            dataConfig.<Configuration>.LastOrDefault.Add(signalSelection)
            _configData.Add(dataConfig)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''Write process config''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim processConfig As XElement = <ProcessConfig><Configuration></Configuration></ProcessConfig>
            If Not String.IsNullOrEmpty(_saveToRun.InitializationPath) Then
                processConfig.<Configuration>.FirstOrDefault.Add(<InitializationPath><%= _saveToRun.InitializationPath %></InitializationPath>)
            Else
                processConfig.<Configuration>.FirstOrDefault.Add(<InitializationPath></InitializationPath>)
            End If
            Dim processing As XElement = <Processing></Processing>
            For Each unWrap In _svm.ProcessConfigure.UnWrapList
                aStep = <Unwrap></Unwrap>
                '<MaxNaN><%= unWrap.MaxNaN %></MaxNaN>
                Dim PMUSignalDictionary = unWrap.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                _writePMUElements(aStep, PMUSignalDictionary)
                processing.Add(aStep)
            Next
            For Each itrplt In _svm.ProcessConfigure.InterpolateList
                aStep = <Interpolate>
                            <Parameters>
                                <Limit><%= itrplt.Limit %></Limit>
                                <Type><%= itrplt.Type %></Type>
                                <FlagInterp><%= itrplt.FlagInterp %></FlagInterp>
                            </Parameters>
                        </Interpolate>
                Dim PMUSignalDictionary = itrplt.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                _writePMUElements(aStep, PMUSignalDictionary)
                processing.Add(aStep)
            Next
            stage = <Stages></Stages>
            newStageFlag = True
            For Each stp In _svm.ProcessConfigure.CollectionOfSteps
                If TypeOf stp Is Multirate AndAlso newStageFlag Then
                    newStageFlag = False
                ElseIf Not newStageFlag AndAlso TypeOf stp Is TunableFilter Then
                    processing.Add(stage)
                    stage = <Stages></Stages>
                    newStageFlag = True
                End If
                If TypeOf stp Is TunableFilter Then
                    aStep = <Filter>
                                <Type><%= stp.Type.ToString() %></Type>
                            </Filter>
                    Select Case stp.Type
                        Case TunableFilterType.HighPass
                            Dim parameters = <Parameters></Parameters>
                            If Not String.IsNullOrEmpty(stp.Order) Then
                                parameters.Add(<Order><%= stp.Order %></Order>)
                            End If
                            If Not String.IsNullOrEmpty(stp.Cutoff) Then
                                parameters.Add(<Cutoff><%= stp.Cutoff %></Cutoff>)
                            End If
                            aStep.Add(parameters)
                        Case TunableFilterType.LowPass
                            Dim parameters = <Parameters></Parameters>
                            If Not String.IsNullOrEmpty(stp.PassRipple) Then
                                parameters.Add(<PassRipple><%= stp.PassRipple %></PassRipple>)
                            End If
                            If Not String.IsNullOrEmpty(stp.StopRipple) Then
                                parameters.Add(<StopRipple><%= stp.StopRipple %></StopRipple>)
                            End If
                            If Not String.IsNullOrEmpty(stp.PassCutoff) Then
                                parameters.Add(<PassCutoff><%= stp.PassCutoff %></PassCutoff>)
                            End If
                            If Not String.IsNullOrEmpty(stp.StopCutoff) Then
                                parameters.Add(<StopCutoff><%= stp.StopCutoff %></StopCutoff>)
                            End If
                            aStep.Add(parameters)
                        Case Else
                            Dim parameters = <Parameters></Parameters>
                            If Not String.IsNullOrEmpty(stp.Numerator) Then
                                parameters.Add(<Numerator><%= stp.Numerator %></Numerator>)
                            End If
                            If Not String.IsNullOrEmpty(stp.Denominator) Then
                                parameters.Add(<Denominator><%= stp.Denominator %></Denominator>)
                            End If
                            aStep.Add(parameters)
                    End Select
                    'For Each parameter In stp.FilterParameters
                    '    Dim para As XElement = New XElement(parameter.ParameterName.ToString, parameter.Value)
                    '    aStep.<Parameters>.LastOrDefault.Add(para)
                    'Next
                    Dim PMUSignalDictionary = DirectCast(stp, TunableFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                ElseIf TypeOf stp Is Multirate Then
                    aStep = <Multirate>
                                <Parameters>
                                    <MultiRatePMU><%= stp.MultiRatePMU %></MultiRatePMU>
                                </Parameters>
                            </Multirate>
                    If stp.FilterChoice = 1 Then
                        Dim newR = <NewRate><%= stp.NewRate %></NewRate>
                        aStep.<Parameters>.LastOrDefault.Add(newR)
                    ElseIf stp.FilterChoice = 2 Then
                        If Not String.IsNullOrEmpty(stp.PElement) Then
                            aStep.<Parameters>.LastOrDefault.Add(<p><%= stp.PElement %></p>)
                        End If
                        If Not String.IsNullOrEmpty(stp.QElement) Then
                            aStep.<Parameters>.LastOrDefault.Add(<q><%= stp.QElement %></q>)
                        End If
                        'Dim p = <p><%= stp.PElement %></p>
                        'Dim q = <q><%= stp.QElement %></q>
                        'aStep.<Parameters>.LastOrDefault.Add(p)
                        'aStep.<Parameters>.LastOrDefault.Add(q)
                    End If
                    Dim PMUSignalDictionary = DirectCast(stp, Multirate).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                End If
                stage.Add(aStep)
            Next
            processing.Add(stage)
            For Each wrp In _svm.ProcessConfigure.WrapList
                aStep = <Wrap></Wrap>
                Dim PMUSignalDictionary = wrp.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                _writePMUElements(aStep, PMUSignalDictionary)
                processing.Add(aStep)
            Next
            processConfig.<Configuration>.LastOrDefault.Add(processing)
            Dim nameTypeUnit As XElement = <NameTypeUnit></NameTypeUnit>
            If _svm.ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then
                For Each pmus In _svm.ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                    For Each signal In pmus.InputChannels
                        Dim pmu = <PMU></PMU>
                        Dim name = <Name><%= signal.PMUName %></Name>
                        pmu.Add(name)
                        Dim crntchnl = <CurrentChannel><%= signal.SignalName %></CurrentChannel>
                        pmu.Add(crntchnl)
                        If pmus.InputChannels.Count > 1 Then
                            Dim newchl = <NewChannel><%= signal.SignalName %></NewChannel>
                            pmu.Add(newchl)
                        Else
                            Dim newchl = <NewChannel><%= pmus.NewChannel %></NewChannel>
                            pmu.Add(newchl)
                        End If
                        Dim newunit = <NewUnit><%= pmus.NewUnit %></NewUnit>
                        pmu.Add(newunit)
                        Dim newtype = <NewType><%= pmus.NewType %></NewType>
                        pmu.Add(newtype)
                        nameTypeUnit.Add(pmu)
                    Next
                Next
            Else
                Dim unit = _svm.ProcessConfigure.NameTypeUnitElement.NewUnit
                If Not String.IsNullOrEmpty(unit) Then
                    nameTypeUnit.Add(<NewUnit><%= unit %></NewUnit>)
                End If
                Dim type = _svm.ProcessConfigure.NameTypeUnitElement.NewType
                If Not String.IsNullOrEmpty(type) Then
                    nameTypeUnit.Add(<NewType><%= type %></NewType>)
                End If
            End If
            processConfig.<Configuration>.LastOrDefault.Add(nameTypeUnit)
            Dim signalSelection2 As XElement = <SignalSelection></SignalSelection>
            processConfig.<Configuration>.LastOrDefault.Add(signalSelection2)
            _configData.Add(processConfig)

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''Write post process config''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim postProcessConfig As XElement = <PostProcessCustomizationConfig><Configuration></Configuration></PostProcessCustomizationConfig>
            If _svm.PostProcessConfigure.CollectionOfSteps.Count > 0 Then
                stage = <Stages></Stages>
                For Each singleStep In _svm.PostProcessConfigure.CollectionOfSteps
                    _writeAStep(aStep, singleStep)
                    stage.Add(aStep)
                Next
                postProcessConfig.<Configuration>.LastOrDefault.Add(stage)
            End If
            'signalSelection = <SignalSelection></SignalSelection>
            'postProcessConfig.<Configuration>.LastOrDefault.Add(signalSelection)
            _configData.Add(postProcessConfig)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''Write detector config''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim detectorConfig As XElement = <DetectorConfig><Configuration></Configuration></DetectorConfig>
            If Not String.IsNullOrEmpty(_saveToRun.EventPath) Then
                detectorConfig.<Configuration>.FirstOrDefault.Add(<EventPath><%= _saveToRun.EventPath %></EventPath>)
            Else
                detectorConfig.<Configuration>.FirstOrDefault.Add(<EventPath></EventPath>)
            End If
            If Not String.IsNullOrEmpty(_svm.DetectorConfigure.ResultUpdateInterval) Then
                detectorConfig.<Configuration>.FirstOrDefault.Add(<ResultUpdateInterval><%= _svm.DetectorConfigure.ResultUpdateInterval %></ResultUpdateInterval>)
            Else
                detectorConfig.<Configuration>.FirstOrDefault.Add(<ResultUpdateInterval></ResultUpdateInterval>)
            End If

            For Each detector In _svm.DetectorConfigure.DetectorList
                Dim element As XElement
                Select Case detector.GetType
                    Case GetType(OutOfRangeFrequencyDetector)
                        Dim dt = DirectCast(detector, OutOfRangeFrequencyDetector)
                        element = <OutOfRangeGeneral></OutOfRangeGeneral>
                        If dt.Type = OutOfRangeFrequencyDetectorType.AvergeWindow AndAlso Not String.IsNullOrEmpty(dt.AverageWindow) Then
                            element.Add(<AverageWindow><%= dt.AverageWindow %></AverageWindow>)
                        ElseIf dt.Type = OutOfRangeFrequencyDetectorType.Nominal AndAlso Not String.IsNullOrEmpty(dt.Nominal) Then
                            element.Add(<Nominal><%= dt.Nominal %></Nominal>)
                        End If
                        If Not String.IsNullOrEmpty(dt.DurationMax) Then
                            element.Add(<DurationMax><%= dt.DurationMax %></DurationMax>)
                        End If
                        If Not String.IsNullOrEmpty(dt.DurationMin) Then
                            element.Add(<DurationMin><%= dt.DurationMin %></DurationMin>)
                        End If
                        If Not String.IsNullOrEmpty(dt.Duration) Then
                            element.Add(<Duration><%= dt.Duration %></Duration>)
                        End If
                        If Not String.IsNullOrEmpty(dt.AnalysisWindow) Then
                            element.Add(<AnalysisWindow><%= dt.AnalysisWindow %></AnalysisWindow>)
                        End If
                        If Not String.IsNullOrEmpty(dt.RateOfChangeMax) Then
                            element.Add(<RateOfChangeMax><%= dt.RateOfChangeMax %></RateOfChangeMax>)
                        End If
                        If Not String.IsNullOrEmpty(dt.RateOfChangeMin) Then
                            element.Add(<RateOfChangeMin><%= dt.RateOfChangeMin %></RateOfChangeMin>)
                        End If
                        If Not String.IsNullOrEmpty(dt.RateOfChange) Then
                            element.Add(<RateOfChange><%= dt.RateOfChange %></RateOfChange>)
                        End If
                        If Not String.IsNullOrEmpty(dt.EventMergeWindow) Then
                            element.Add(<EventMergeWindow><%= dt.EventMergeWindow %></EventMergeWindow>)
                        End If
                    'PMUSignalDictionary = dt.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                'Case GetType(OutOfRangeGeneralDetector)
                '    Dim dt = DirectCast(detector, OutOfRangeGeneralDetector)
                '    element = <OutOfRangeGeneral>
                '                                                                <Max><%= dt.Max %></Max>
                '                                                                <Min><%= dt.Min %></Min>
                '                                                                <AnalysisWindow><%= dt.AnalysisWindow %></AnalysisWindow>
                '                                                                <Duration><%= dt.Duration %></Duration>
                '                                                            </OutOfRangeGeneral>
                    Case GetType(SpectralCoherenceDetector)
                        Dim dt = DirectCast(detector, SpectralCoherenceDetector)
                        element = <SpectralCoherence></SpectralCoherence>
                        If Not String.IsNullOrEmpty(dt.Mode.ToString) Then
                            element.Add(<Mode><%= dt.Mode.ToString %></Mode>)
                        End If
                        If Not String.IsNullOrEmpty(dt.AnalysisLength) Then
                            element.Add(<AnalysisLength><%= dt.AnalysisLength %></AnalysisLength>)
                        End If
                        If Not String.IsNullOrEmpty(dt.Delay) Then
                            element.Add(<Delay><%= dt.Delay %></Delay>)
                        End If
                        If Not String.IsNullOrEmpty(dt.NumberDelays) Then
                            element.Add(<NumberDelays><%= dt.NumberDelays %></NumberDelays>)
                        End If
                        If Not String.IsNullOrEmpty(dt.ThresholdScale) Then
                            element.Add(<ThresholdScale><%= dt.ThresholdScale %></ThresholdScale>)
                        End If
                        If Not String.IsNullOrEmpty(dt.WindowType.ToString) Then
                            element.Add(<WindowType><%= dt.WindowType.ToString %></WindowType>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyInterval) Then
                            element.Add(<FrequencyInterval><%= dt.FrequencyInterval %></FrequencyInterval>)
                        End If
                        If Not String.IsNullOrEmpty(dt.WindowLength) Then
                            element.Add(<WindowLength><%= dt.WindowLength %></WindowLength>)
                        End If
                        If Not String.IsNullOrEmpty(dt.WindowOverlap) Then
                            element.Add(<WindowOverlap><%= dt.WindowOverlap %></WindowOverlap>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyMin) Then
                            element.Add(<FrequencyMin><%= dt.FrequencyMin %></FrequencyMin>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyMax) Then
                            element.Add(<FrequencyMax><%= dt.FrequencyMax %></FrequencyMax>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyTolerance) Then
                            element.Add(<FrequencyTolerance><%= dt.FrequencyTolerance %></FrequencyTolerance>)
                        End If
                    Case GetType(RingdownDetector)
                        Dim dt = DirectCast(detector, RingdownDetector)
                        'element = <Ringdown>
                        '              <RMSmedianFilterTime><%= dt.RMSmedianFilterTime %></RMSmedianFilterTime>
                        '              <RMSlength><%= dt.RMSlength %></RMSlength>
                        '              <RingThresholdScale><%= dt.RingThresholdScale %></RingThresholdScale>
                        '          </Ringdown>
                        element = <Ringdown></Ringdown>
                        If Not String.IsNullOrEmpty(dt.RMSmedianFilterTime) Then
                            element.Add(<RMSmedianFilterTime><%= dt.RMSmedianFilterTime %></RMSmedianFilterTime>)
                        End If
                        If Not String.IsNullOrEmpty(dt.RMSlength) Then
                            element.Add(<RMSlength><%= dt.RMSlength %></RMSlength>)
                        End If
                        If Not String.IsNullOrEmpty(dt.RingThresholdScale) Then
                            element.Add(<RingThresholdScale><%= dt.RingThresholdScale %></RingThresholdScale>)
                        End If
                    Case GetType(WindRampDetector)
                        Dim dt = DirectCast(detector, WindRampDetector)
                        element = <WindRamp></WindRamp>
                        If Not String.IsNullOrEmpty(dt.Fpass) Then
                            element.Add(<Fpass><%= dt.Fpass %></Fpass>)
                        End If
                        If Not String.IsNullOrEmpty(dt.Fstop) Then
                            element.Add(<Fstop><%= dt.Fstop %></Fstop>)
                        End If
                        If Not String.IsNullOrEmpty(dt.Apass) Then
                            element.Add(<Apass><%= dt.Apass %></Apass>)
                        End If
                        If Not String.IsNullOrEmpty(dt.Astop) Then
                            element.Add(<Astop><%= dt.Astop %></Astop>)
                        End If
                        If Not String.IsNullOrEmpty(dt.ValMin) Then
                            element.Add(<ValMin><%= dt.ValMin %></ValMin>)
                        End If
                        If Not String.IsNullOrEmpty(dt.TimeMin) Then
                            element.Add(<TimeMin><%= dt.TimeMin %></TimeMin>)
                        End If
                        If Not String.IsNullOrEmpty(dt.ValMax) Then
                            element.Add(<ValMax><%= dt.ValMax %></ValMax>)
                        End If
                        If Not String.IsNullOrEmpty(dt.TimeMax) Then
                            element.Add(<TimeMax><%= dt.TimeMax %></TimeMax>)
                        End If
                    Case GetType(PeriodogramDetector)
                        Dim dt = DirectCast(detector, PeriodogramDetector)
                        element = <Periodogram></Periodogram>
                        If Not String.IsNullOrEmpty(dt.Mode.ToString) Then
                            element.Add(<Mode><%= dt.Mode.ToString %></Mode>)
                        End If
                        If Not String.IsNullOrEmpty(dt.AnalysisLength) Then
                            element.Add(<AnalysisLength><%= dt.AnalysisLength %></AnalysisLength>)
                        End If
                        If Not String.IsNullOrEmpty(dt.WindowType.ToString) Then
                            element.Add(<WindowType><%= dt.WindowType.ToString %></WindowType>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyInterval) Then
                            element.Add(<FrequencyInterval><%= dt.FrequencyInterval %></FrequencyInterval>)
                        End If
                        If Not String.IsNullOrEmpty(dt.WindowLength) Then
                            element.Add(<WindowLength><%= dt.WindowLength %></WindowLength>)
                        End If
                        If Not String.IsNullOrEmpty(dt.WindowOverlap) Then
                            element.Add(<WindowOverlap><%= dt.WindowOverlap %></WindowOverlap>)
                        End If
                        If Not String.IsNullOrEmpty(dt.MedianFilterFrequencyWidth) Then
                            element.Add(<MedianFilterFrequencyWidth><%= dt.MedianFilterFrequencyWidth %></MedianFilterFrequencyWidth>)
                        End If
                        If Not String.IsNullOrEmpty(dt.Pfa) Then
                            element.Add(<Pfa><%= dt.Pfa %></Pfa>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyMin) Then
                            element.Add(<FrequencyMin><%= dt.FrequencyMin %></FrequencyMin>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyMax) Then
                            element.Add(<FrequencyMax><%= dt.FrequencyMax %></FrequencyMax>)
                        End If
                        If Not String.IsNullOrEmpty(dt.FrequencyTolerance) Then
                            element.Add(<FrequencyTolerance><%= dt.FrequencyTolerance %></FrequencyTolerance>)
                        End If

                        '    <Mode><%= dt.Mode.ToString %></Mode>
                        '    <AnalysisLength><%= dt.AnalysisLength %></AnalysisLength>
                        '    <WindowType><%= dt.WindowType.ToString %></WindowType>
                        '    <FrequencyInterval><%= dt.FrequencyInterval %></FrequencyInterval>
                        '    <WindowLength><%= dt.WindowLength %></WindowLength>
                        '    <WindowOverlap><%= dt.WindowOverlap %></WindowOverlap>
                        '    <MedianFilterFrequencyWidth><%= dt.MedianFilterFrequencyWidth %></MedianFilterFrequencyWidth>
                        '    <Pfa><%= dt.Pfa %></Pfa>
                        '    <FrequencyMin><%= dt.FrequencyMin %></FrequencyMin>
                        '    <FrequencyMax><%= dt.FrequencyMax %></FrequencyMax>
                        '    <FrequencyTolerance><%= dt.FrequencyTolerance %></FrequencyTolerance>
                        '</Periodogram>
                    Case GetType(VoltageStabilityDetectorViewModel)
                        Continue For
                    Case Else
                        Throw New Exception("Error! Unrecognized detector type: " & detector.GetType.ToString & ".")
                End Select
                'If element.HasElements Then
                Dim PMUSignalDictionary = detector.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                _writePMUElements(element, PMUSignalDictionary)
                detectorConfig.<Configuration>.LastOrDefault.Add(element)
                'End If
            Next
            detectorConfig.<Configuration>.LastOrDefault.Add(<Alarming></Alarming>)
            For Each alarm In _svm.DetectorConfigure.AlarmingList
                Dim element As XElement
                Select Case alarm.GetType
                    Case GetType(AlarmingPeriodogram)
                        Dim al = DirectCast(alarm, AlarmingPeriodogram)
                        element = <Periodogram>
                                      <SNRalarm><%= al.SNRalarm %></SNRalarm>
                                      <SNRmin><%= al.SNRmin %></SNRmin>
                                      <TimeMin><%= al.TimeMin %></TimeMin>
                                      <SNRcorner><%= al.SNRcorner %></SNRcorner>
                                      <TimeCorner><%= al.TimeCorner %></TimeCorner>
                                  </Periodogram>
                    Case GetType(AlarmingRingdown)
                        Dim al = DirectCast(alarm, AlarmingRingdown)
                        If Not String.IsNullOrEmpty(al.MaxDuration) Then
                            element = <Ringdown>
                                          <MaxDuration><%= al.MaxDuration %></MaxDuration>
                                      </Ringdown>
                        End If
                    Case GetType(AlarmingSpectralCoherence)
                        Dim al = DirectCast(alarm, AlarmingSpectralCoherence)
                        element = <SpectralCoherence>
                                      <CoherenceAlarm><%= al.CoherenceAlarm %></CoherenceAlarm>
                                      <CoherenceMin><%= al.CoherenceMin %></CoherenceMin>
                                      <TimeMin><%= al.TimeMin %></TimeMin>
                                      <CoherenceCorner><%= al.CoherenceCorner %></CoherenceCorner>
                                      <TimeCorner><%= al.TimeCorner %></TimeCorner>
                                  </SpectralCoherence>
                    Case Else
                        Throw New Exception("Error! Unrecognized alarming detector type: " & alarm.GetType.ToString & ".")
                End Select
                detectorConfig.<Configuration>.<Alarming>.LastOrDefault.Add(element)
            Next
            _configData.Add(detectorConfig)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''Write wind application''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim windApplication As XElement = <WindAppConfig>
                                                  <Configuration></Configuration>
                                              </WindAppConfig>
            _configData.Add(windApplication)
            _configData.Save(filename)
        End Sub

        Private Sub _writeAStep(ByRef aStep As XElement, ByRef singleStep As Object)
            Select Case singleStep.Name
                Case "Scalar Repetition"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters></Parameters>
                            </Customization>

                    If Not String.IsNullOrEmpty(singleStep.CustPMUname) Then
                        aStep.<Parameters>.FirstOrDefault.Add(<CustPMUname><%= singleStep.CustPMUname %></CustPMUname>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.Scalar) Then
                        aStep.<Parameters>.FirstOrDefault.Add(<scalar><%= singleStep.Scalar %></scalar>)
                    End If
                    If singleStep.OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(singleStep.OutputChannels(0).SignalName) Then
                        aStep.<Parameters>.FirstOrDefault.Add(<SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>)
                    End If
                    If singleStep.OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(singleStep.OutputChannels(0).TypeAbbreviation) Then
                        aStep.<Parameters>.FirstOrDefault.Add(<SignalType><%= singleStep.OutputChannels(0).TypeAbbreviation %></SignalType>)
                    End If
                    If singleStep.OutputChannels.Count > 0 AndAlso Not String.IsNullOrEmpty(singleStep.OutputChannels(0).Unit) Then
                        aStep.<Parameters>.FirstOrDefault.Add(<SignalUnit><%= singleStep.OutputChannels(0).Unit %></SignalUnit>)
                    End If
                    If singleStep.TimeSourcePMU IsNot Nothing AndAlso Not String.IsNullOrEmpty(singleStep.TimeSourcePMU.PMU) Then
                        aStep.<Parameters>.LastOrDefault.Add(<TimeSourcePMU><%= singleStep.TimeSourcePMU.PMU %></TimeSourcePMU>)
                    End If
                Case "Addition"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                    <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                </Parameters>
                            </Customization>
                    For Each signal In singleStep.InputChannels
                        Dim term As XElement = <term>
                                                   <PMU><%= signal.PMUName %></PMU>
                                                   <Channel><%= signal.SignalName %></Channel>
                                               </term>
                        aStep.<Parameters>.LastOrDefault.Add(term)
                    Next
                Case "Subtraction"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                    <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                    <minuend>
                                        <PMU><%= singleStep.Minuend.PMUName %></PMU>
                                        <Channel><%= singleStep.Minuend.SignalName %></Channel>
                                    </minuend>
                                    <subtrahend>
                                        <PMU><%= singleStep.Subtrahend.PMUName %></PMU>
                                        <Channel><%= singleStep.Subtrahend.SignalName %></Channel>
                                    </subtrahend>
                                </Parameters>
                            </Customization>
                Case "Multiplication"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                    <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                </Parameters>
                            </Customization>
                    For Each signal In singleStep.InputChannels
                        Dim factor As XElement = <factor>
                                                     <PMU><%= signal.PMUName %></PMU>
                                                     <Channel><%= signal.SignalName %></Channel>
                                                 </factor>
                        aStep.<Parameters>.LastOrDefault.Add(factor)
                    Next
                Case "Division"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                    <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                    <dividend>
                                        <PMU><%= singleStep.Dividend.PMUName %></PMU>
                                        <Channel><%= singleStep.Dividend.SignalName %></Channel>
                                    </dividend>
                                    <divisor>
                                        <PMU><%= singleStep.Divisor.PMUName %></PMU>
                                        <Channel><%= singleStep.Divisor.SignalName %></Channel>
                                    </divisor>
                                </Parameters>
                            </Customization>
                Case "Exponential"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <exponent><%= singleStep.Exponent %></exponent>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                </Parameters>
                            </Customization>
                    For Each pair In singleStep.OutputInputMappingPair
                        Dim signal As XElement = <signal>
                                                     <PMU><%= pair.Value(0).PMUName %></PMU>
                                                     <Channel><%= pair.Value(0).SignalName %></Channel>
                                                     <CustName><%= pair.Key.SignalName %></CustName>
                                                 </signal>
                        aStep.<Parameters>.LastOrDefault.Add(signal)
                    Next
                Case "Sign Reversal", "Absolute Value", "Real Component", "Imaginary Component", "Complex Conjugate", "Angle Calculation"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                </Parameters>
                            </Customization>
                    For Each pair In singleStep.OutputInputMappingPair
                        Dim signal As XElement = <signal>
                                                     <PMU><%= pair.Value(0).PMUName %></PMU>
                                                     <Channel><%= pair.Value(0).SignalName %></Channel>
                                                     <CustName><%= pair.Key.SignalName %></CustName>
                                                 </signal>
                        aStep.<Parameters>.LastOrDefault.Add(signal)
                    Next
                Case "Phasor Creation"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                </Parameters>
                            </Customization>
                    For Each pair In singleStep.OutputInputMappingPair
                        Dim phasor As XElement = <phasor>
                                                     <mag>
                                                         <PMU><%= pair.Value(0).PMUName %></PMU>
                                                         <Channel><%= pair.Value(0).SignalName %></Channel>
                                                     </mag>
                                                     <ang>
                                                         <PMU><%= pair.Value(1).PMUName %></PMU>
                                                         <Channel><%= pair.Value(1).SignalName %></Channel>
                                                     </ang>
                                                     <CustName><%= pair.Key.SignalName %></CustName>
                                                 </phasor>
                        aStep.<Parameters>.LastOrDefault.Add(phasor)
                    Next
                Case "Power Calculation"
                    Dim powerDict = _powerTypeDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <PowType><%= powerDict(singleStep.PowType.ToString) %></PowType>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                </Parameters>
                            </Customization>
                    For Each pair In singleStep.OutputInputMappingPair
                        If pair.Value.Count = 4 Then
                            Dim power As XElement = <power>
                                                        <Vmag>
                                                            <PMU><%= pair.Value(0).PMUName %></PMU>
                                                            <Channel><%= pair.Value(0).SignalName %></Channel>
                                                        </Vmag>
                                                        <Vang>
                                                            <PMU><%= pair.Value(1).PMUName %></PMU>
                                                            <Channel><%= pair.Value(1).SignalName %></Channel>
                                                        </Vang>
                                                        <Imag>
                                                            <PMU><%= pair.Value(2).PMUName %></PMU>
                                                            <Channel><%= pair.Value(2).SignalName %></Channel>
                                                        </Imag>
                                                        <Iang>
                                                            <PMU><%= pair.Value(3).PMUName %></PMU>
                                                            <Channel><%= pair.Value(3).SignalName %></Channel>
                                                        </Iang>
                                                        <CustName><%= pair.Key.SignalName %></CustName>
                                                    </power>
                            aStep.<Parameters>.LastOrDefault.Add(power)
                        ElseIf pair.Value.Count = 2 Then
                            Dim power As XElement = <power>
                                                        <Vphasor>
                                                            <PMU><%= pair.Value(0).PMUName %></PMU>
                                                            <Channel><%= pair.Value(0).SignalName %></Channel>
                                                        </Vphasor>
                                                        <Iphasor>
                                                            <PMU><%= pair.Value(1).PMUName %></PMU>
                                                            <Channel><%= pair.Value(1).SignalName %></Channel>
                                                        </Iphasor>
                                                        <CustName><%= pair.Key.SignalName %></CustName>
                                                    </power>
                            aStep.<Parameters>.LastOrDefault.Add(power)
                        End If
                    Next
                Case "Signal Type/Unit"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <SigType><%= singleStep.OutputChannels(0).TypeAbbreviation %></SigType>
                                    <SigUnit><%= singleStep.OutputChannels(0).Unit %></SigUnit>
                                    <PMU><%= singleStep.InputChannels(0).PMUName %></PMU>
                                    <Channel><%= singleStep.InputChannels(0).SignalName %></Channel>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                    <CustName><%= singleStep.OutputChannels(0).SignalName %></CustName>
                                </Parameters>
                            </Customization>
                Case "Metric Prefix"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters></Parameters>
                            </Customization>
                    'If singleStep.UseCustomPMU Then
                    aStep.<Parameters>.FirstOrDefault.Add(<CustPMUname><%= singleStep.CustPMUname %></CustPMUname>)
                    'End If
                    'If singleStep.UseCustomPMU Then
                    For Each pair In singleStep.OutputInputMappingPair
                            Dim toConvert As XElement = <ToConvert></ToConvert>
                            If Not String.IsNullOrEmpty(pair.Value(0).PMUName) Then
                                toConvert.Add(<PMU><%= pair.Value(0).PMUName %></PMU>)
                            End If
                            If Not String.IsNullOrEmpty(pair.Value(0).SignalName) Then
                                toConvert.Add(<Channel><%= pair.Value(0).SignalName %></Channel>)
                            End If
                            If Not String.IsNullOrEmpty(pair.Key.Unit) Then
                                toConvert.Add(<NewUnit><%= pair.Key.Unit %></NewUnit>)
                            End If
                            If Not String.IsNullOrEmpty(pair.Key.SignalName) Then
                                toConvert.Add(<CustName><%= pair.Key.SignalName %></CustName>)
                            End If
                            aStep.<Parameters>.LastOrDefault.Add(toConvert)
                        Next
                    'Else
                    '    For Each pair In singleStep.OutputInputMappingPair
                    '        Dim toConvert As XElement = <ToConvert></ToConvert>
                    '        If Not String.IsNullOrEmpty(pair.Value(0).PMUName) Then
                    '            toConvert.Add(<PMU><%= pair.Value(0).PMUName %></PMU>)
                    '        End If
                    '        If Not String.IsNullOrEmpty(pair.Value(0).SignalName) Then
                    '            toConvert.Add(<Channel><%= pair.Value(0).SignalName %></Channel>)
                    '        End If
                    '        If Not String.IsNullOrEmpty(pair.Key.Unit) Then
                    '            toConvert.Add(<NewUnit><%= pair.Key.Unit %></NewUnit>)
                    '        End If
                    '        aStep.<Parameters>.LastOrDefault.Add(toConvert)
                    '    Next
                    'End If
                        Case "Angle Conversion"
                    aStep = <Customization>
                                <Name><%= _svm.DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                                <Parameters>
                                    <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                </Parameters>
                            </Customization>
                    For Each pair In singleStep.OutputInputMappingPair
                        Dim toConvert As XElement = <ToConvert>
                                                        <PMU><%= pair.Value(0).PMUName %></PMU>
                                                        <Channel><%= pair.Value(0).SignalName %></Channel>
                                                        <CustName><%= pair.Key.SignalName %></CustName>
                                                    </ToConvert>
                        aStep.<Parameters>.LastOrDefault.Add(toConvert)
                    Next
                Case "Status Flags"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                                <Parameters></Parameters>
                            </Filter>
                    aStep.<Parameters>.LastOrDefault.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    aStep.<Parameters>.LastOrDefault.Add(New XElement("FlagBit", _dqFilterCounter))
                    For Each group In singleStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList
                        For Each subgroup In group.SignalList
                            Dim PMU As XElement = <PMU>
                                                      <Name><%= subgroup.SignalSignature.PMUName %></Name>
                                                  </PMU>
                            aStep.Add(PMU)
                        Next
                    Next
                Case "Zeros", "Missing"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                                <Parameters></Parameters>
                            </Filter>
                    aStep.<Parameters>.LastOrDefault.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    aStep.<Parameters>.LastOrDefault.Add(New XElement("FlagBit", _dqFilterCounter))
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case "Nominal Voltage"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.NomVoltage) Then
                        para.Add(<NomVoltage><%= singleStep.NomVoltage %></NomVoltage>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.VoltMin) Then
                        para.Add(<VoltMin><%= singleStep.VoltMin %></VoltMin>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.VoltMax) Then
                        para.Add(<VoltMax><%= singleStep.VoltMax %></VoltMax>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBit", _dqFilterCounter))
                    aStep.Add(para)
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case "Nominal Frequency"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.FreqMinChan) Then
                        para.Add(<FreqMinChan><%= singleStep.FreqMinChan %></FreqMinChan>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.FreqMaxChan) Then
                        para.Add(<FreqMaxChan><%= singleStep.FreqMaxChan %></FreqMaxChan>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.FreqPctChan) Then
                        para.Add(<FreqPctChan><%= singleStep.FreqPctChan %></FreqPctChan>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.FreqMinSamp) Then
                        para.Add(<FreqMinSamp><%= singleStep.FreqMinSamp %></FreqMinSamp>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.FreqMaxSamp) Then
                        para.Add(<FreqMaxSamp><%= singleStep.FreqMaxSamp %></FreqMaxSamp>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(<FlagBitChan><%= _dqFilterCounter %></FlagBitChan>)
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(<FlagBitSamp><%= _dqFilterCounter %></FlagBitSamp>)
                    aStep.Add(para)
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case "Outliers"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.StdDevMult) Then
                        para.Add(<StdDevMult><%= singleStep.StdDevMult %></StdDevMult>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBit", _dqFilterCounter))
                    aStep.Add(para)
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case "Stale Data"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.StaleThresh) Then
                        para.Add(<StaleThresh><%= singleStep.StaleThresh %></StaleThresh>)
                    End If
                    If Not String.IsNullOrEmpty(singleStep.FlagAllByFreq.ToString) Then
                        para.Add(<FlagAllByFreq><%= singleStep.FlagAllByFreq.ToString.ToUpper %></FlagAllByFreq>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBitFreq", _dqFilterCounter))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBit", _dqFilterCounter))
                    aStep.Add(para)
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case "Data Frame", "Channel"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.PercentBadThresh) Then
                        para.Add(<PercentBadThresh><%= singleStep.PercentBadThresh %></PercentBadThresh>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBit", _dqFilterCounter))
                    aStep.Add(para)
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case "Entire PMU"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.PercentBadThresh) Then
                        para.Add(<PercentBadThresh><%= singleStep.PercentBadThresh %></PercentBadThresh>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBit", _dqFilterCounter))
                    aStep.Add(para)
                    For Each group In singleStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList
                        For Each subgroup In group.SignalList
                            Dim PMU As XElement = <PMU>
                                                      <Name><%= subgroup.SignalSignature.PMUName %></Name>
                                                  </PMU>
                            aStep.Add(PMU)
                        Next
                    Next
                Case "Angle Wrapping"
                    aStep = <Filter>
                                <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            </Filter>
                    Dim para = <Parameters></Parameters>
                    If Not String.IsNullOrEmpty(singleStep.AngleThresh) Then
                        para.Add(<AngleThresh><%= singleStep.AngleThresh %></AngleThresh>)
                    End If
                    para.Add(New XElement("SetToNaN", "TRUE"))
                    _dqFilterCounter = _dqFilterCounter + 1
                    para.Add(New XElement("FlagBit", _dqFilterCounter))
                    aStep.Add(para)
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                Case Else
                    'aStep = <Filter>
                    '            <Name><%= _svm.DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                    '            <Parameters></Parameters>
                    '        </Filter>
                    'For Each parameter In singleStep.FilterParameters
                    '    'Dim a = {parameter}ParameterName
                    '    'Dim para As XElement = <<%= parameter.ParameterName.ToString %>><%= parameter.Value %></>
                    '    Dim para As XElement
                    '    If TypeOf parameter.Value Is Boolean Then
                    '        If parameter.Value Then
                    '            para = New XElement(parameter.ParameterName.ToString, "TRUE")
                    '        Else
                    '            para = New XElement(parameter.ParameterName.ToString, "FALSE")
                    '        End If
                    '    Else
                    '        para = New XElement(parameter.ParameterName.ToString, parameter.Value)
                    '    End If
                    '    aStep.<Parameters>.LastOrDefault.Add(para)
                    'Next
                    'aStep.<Parameters>.LastOrDefault.Add(New XElement("SetToNaN", "TRUE"))
                    'aStep.<Parameters>.LastOrDefault.Add(New XElement("FlagBit", "1"))
                    'If singleStep.Name = "Status Flags" Then
                    '    For Each group In singleStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList
                    '        For Each subgroup In group.SignalList
                    '            Dim PMU As XElement = <PMU>
                    '                                      <Name><%= subgroup.SignalSignature.PMUName %></Name>
                    '                                  </PMU>
                    '            aStep.Add(PMU)
                    '        Next
                    '    Next
                    'Else
                    '    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    '    _writePMUElements(aStep, PMUSignalDictionary)
                    'End If
            End Select
        End Sub

        Private Sub _writePMUElements(aStep As XElement, pMUSignalDictionary As Dictionary(Of String, List(Of SignalSignatureViewModel)))
            For Each pmuGroup In pMUSignalDictionary
                Dim PMU As XElement = <PMU>
                                          <Name><%= pmuGroup.Key %></Name>
                                      </PMU>
                For Each signal In pmuGroup.Value
                    Dim sglName As XElement = <Channel>
                                                  <Name><%= signal.SignalName %></Name>
                                              </Channel>
                    PMU.Add(sglName)
                Next
                aStep.Add(PMU)
            Next
        End Sub

    End Class
End Namespace