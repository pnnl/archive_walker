Imports System.Collections.ObjectModel

Imports System.Windows
Imports BAWGUI.Core
Imports BAWGUI.Core.Models
Imports BAWGUI.SignalManagement.ViewModels

Namespace ViewModels
    Partial Public Class SettingsViewModel
        'Private Sub _readConfigFile(ByRef configData As XDocument)

        '    '''''''''''''''''''''' Read DataConfig''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '    Dim fileInfoList = New ObservableCollection(Of InputFileInfo)
        '    Dim inputInformation = From el In configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.Elements Where el.Name = "FilePath" Select el
        '    For Each el In inputInformation
        '        Dim info = New InputFileInfo
        '        info.FileDirectory = el.<FileDirectory>.Value
        '        Dim type = el.<FileType>.Value
        '        If type IsNot Nothing Then
        '            info.FileType = [Enum].Parse(GetType(DataFileType), type.ToLower)
        '        End If
        '        info.Mnemonic = el.<Mnemonic>.Value
        '        fileInfoList.Add(info)
        '        '_buildInputFileFolderTree(info)
        '    Next

        '    DataConfigure.ReaderProperty.InputFileInfos = fileInfoList
        '    Try
        '        DataConfigure.ReaderProperty.ModeName = [Enum].Parse(GetType(ModeType), configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Name>.Value)
        '    Catch ex As Exception
        '        DataConfigure.ReaderProperty.ModeName = ModeType.Archive
        '    End Try
        '    'Dim modeNameValue = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Name>.Value
        '    'If modeNameValue IsNot Nothing Then
        '    '    DataConfigure.ReaderProperty.ModeName = [Enum].Parse(GetType(ModeType), modeNameValue)
        '    'Else
        '    '    DataConfigure.ReaderProperty.ModeName = ""
        '    'End If
        '    Select Case DataConfigure.ReaderProperty.ModeName
        '        Case ModeType.Archive
        '            DataConfigure.ReaderProperty.DateTimeStart = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeStart>.Value
        '            DataConfigure.ReaderProperty.DateTimeEnd = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeEnd>.Value
        '        Case ModeType.Hybrid
        '            DataConfigure.ReaderProperty.DateTimeStart = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeStart>.Value
        '            DataConfigure.ReaderProperty.NoFutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<NoFutureWait>.Value
        '            DataConfigure.ReaderProperty.MaxNoFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxNoFutureCount>.Value
        '            DataConfigure.ReaderProperty.FutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<FutureWait>.Value
        '            DataConfigure.ReaderProperty.MaxFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxFutureCount>.Value
        '            DataConfigure.ReaderProperty.RealTimeRange = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<RealTimeRange>.Value
        '        Case ModeType.RealTime
        '            DataConfigure.ReaderProperty.NoFutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<NoFutureWait>.Value
        '            DataConfigure.ReaderProperty.MaxNoFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxNoFutureCount>.Value
        '            DataConfigure.ReaderProperty.FutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<FutureWait>.Value
        '            DataConfigure.ReaderProperty.MaxFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxFutureCount>.Value
        '        Case Else
        '            Throw New Exception("Error: invalid mode type found in config file.")
        '    End Select
        '    _readDataConfigStages(configData)
        '    '''''''''''''''''''''''''''''''''Read ProcessConfig and PostProcessConfig''''''''''''''''''''''''''''''''''''''''''''''''''''
        '    _readProcessConfig(configData)
        '    _readPostProcessConfig(configData)
        '    '''''''''''''''''''''''''''''''''Read DetectorConfig'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '    _readDetectorConfig(configData)
        'End Sub

        Private Function _readPMUElements(stp As XElement) As ObservableCollection(Of SignalSignatureViewModel)
            Dim inputSignalList = New ObservableCollection(Of SignalSignatureViewModel)
            Dim inputs = From el In stp.Elements Where el.Name = "PMU" Select el
            If inputs.ToList.Count > 0 Then

                For Each aInput In inputs
                    Dim pmuName = aInput.<Name>.Value()
                    Dim channels = From el In aInput.Elements Where el.Name = "Channel" Select el
                    If channels.ToList.Count > 0 Then

                        For Each channel In channels
                            Dim signalName = channel.<Name>.Value
                            Dim signal = _signalMgr.SearchForSignalInTaggedSignals(pmuName, signalName)
                            If signal IsNot Nothing Then
                                inputSignalList.Add(signal)
                            Else
                                Throw New Exception("Error reading config file! Signal with channel name: " & signalName & " in PMU " & pmuName & " not found!")
                            End If
                        Next
                    Else
                        For Each group In _signalMgr.GroupedRawSignalsByPMU
                            For Each subgroup In group.SignalList
                                For Each subsubgroup In subgroup.SignalList
                                    If subsubgroup.SignalSignature.PMUName = pmuName Then
                                        For Each signal In subsubgroup.SignalList
                                            inputSignalList.Add(signal.SignalSignature)
                                        Next
                                    End If
                                Next
                            Next
                        Next
                    End If
                Next
            Else
                Throw New Exception("Warning! No PMU specified, no channel or no PMU is included.")
            End If
            Return inputSignalList
        End Function

#Region "Read Data Config DQ filter and Customization Steps From XML Configure File"
        Private Sub _readDataConfigStages(configData As XDocument)
            Dim CollectionOfSteps As New ObservableCollection(Of Object)
            _signalMgr.GroupedSignalByDataConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)
            _signalMgr.GroupedSignalByDataConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
            Dim stepCounter As Integer = 0
            Dim stages = From el In configData.<Config>.<DataConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
            For Each stage In stages
                Dim steps = From element In stage.Elements Select element
                For Each stp In steps
                    Dim aStep As Object
                    If stp.Name = "Filter" Then
                        Dim filterName = DataConfigure.DQFilterReverseNameDictionary(stp.<Name>.Value)
                        Select Case filterName
                            Case "Status Flags", "Zeros", "Missing"
                                aStep = New DQFilter
                                aStep.Name = filterName
                                '_readPlainDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Nominal Voltage"
                                aStep = New VoltPhasorDQFilter
                                aStep.Name = filterName
                                _readVoltPhasorDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Nominal Frequency"
                                aStep = New FreqDQFilter
                                aStep.Name = filterName
                                _readFreqDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Outliers"
                                aStep = New OutlierDQFilter
                                aStep.Name = filterName
                                _readOutlierDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Stale Data"
                                aStep = New StaleDQFilter
                                aStep.Name = filterName
                                _readStaleDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Data Frame", "Channel", "Entire PMU"
                                aStep = New DataFrameDQFilter
                                aStep.Name = filterName
                                _readDataFramePMUchanPMUallDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Angle Wrapping"
                                aStep = New WrappingFailureDQFilter
                                aStep.Name = filterName
                                _readWrappingFailureDQFilter(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case Else
                                Throw New Exception(String.Format("Wrong DQ Filter name found in Config.xml file: {0}", filterName))
                        End Select
                        'necessaryParams.AddRange(DataConfigure.DQFilterNameParametersDictionary(aStep.Name))
                    ElseIf stp.Name = "Customization" Then
                        Dim thisStepName = DataConfigure.CustomizationReverseNameDictionary(stp.<Name>.Value)
                        Select Case thisStepName
                            Case "Metric Prefix"
                                aStep = New MetricPrefixCust
                                aStep.Name = thisStepName
                                _readMetricPrefixCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case "Scalar Repetition"
                                aStep = New ScalarRepCust
                                aStep.Name = thisStepName
                                _readScalarRepetitionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                            Case Else
                                aStep = New Customization
                                aStep.Name = thisStepName
                        End Select
                        'necessaryParams.AddRange(DataConfigure.CustomizationNameParemetersDictionary(aStep.Name))
                    End If
                    stepCounter += 1
                    aStep.StepCounter = stepCounter
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name

                    'Dim signalForUnitTypeSpecificationCustomization As SignalSignatures = Nothing
                    Select Case aStep.Name
                        Case "Status Flags", "Zeros", "Missing", "Nominal Voltage", "Nominal Frequency", "Outliers", "Stale Data", "Data Frame", "Channel", "Entire PMU", "Angle Wrapping", "Metric Prefix"
                            'pass
                        Case "Scalar Repetition"
                            'pass
                        Case "Addition"
                            _readAdditionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Subtraction"
                            _readSubtractionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Division"
                            _readDivisionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Multiplication"
                            _readMultiplicationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Exponential"
                            _readRaiseExpCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Sign Reversal", "Absolute Value", "Real Component", "Imaginary Component", "Complex Conjugate"
                            _readUnaryCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Angle Calculation"
                            _readAngleCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Phasor Creation"
                            _readPhasorCreationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Signal Type/Unit"
                            _readSpecTypeUnitCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Power Calculation"
                            _readPowerCalculationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter, 1)
                        Case "Angle Conversion"
                            _readAngleConversionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case Else
                            Throw New Exception(String.Format("Wrong stage name found in Config.xml file: {0}", aStep.Name))
                            'Dim params = From ps In stp.<Parameters>.Elements Select ps
                            'For Each pair In params
                            '    Dim paraName = pair.Name.ToString
                            '    If paraName <> "SetToNaN" And paraName <> "FlagBit" Then
                            '        Dim aPair As New ParameterValuePair
                            '        aPair.ParameterName = paraName
                            '        If pair.Value.ToLower = "false" Then
                            '            aPair.Value = False
                            '        ElseIf pair.Value.ToLower = "true" Then
                            '            aPair.Value = True
                            '            'ElseIf aStep.Name = "Nominal Frequency" And paraName = "FlagBit" Then
                            '            '    aPair.IsRequired = False
                            '            '    aPair.Value = pair.Value
                            '        Else
                            '            aPair.Value = pair.Value
                            '        End If
                            '        aStep.FilterParameters.Add(aPair)
                            '    End If
                            'Next
                            'Try
                            '    aStep.InputChannels = _readPMUElements(stp)
                            'Catch ex As Exception
                            '    _addLog("In a data quality filter step: " & aStep.StepCounter.ToString & " of data config. " & ex.Message)
                            'End Try
                            'For Each signal In aStep.InputChannels
                            '    signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                            '    aStep.OutputChannels.Add(signal)
                            'Next
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            ' to add this step that is for sure a DQ filter to the list of step for signal manipulation and customization.
                            ' Don't not move it outside the select case loop!!!!!
                            ' Or it will cause customization steps being added twice.
                            'CollectionOfSteps.Add(aStep)
                            ' Leave this line here! Don't move it!
                            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    End Select
                    If TypeOf aStep Is DQFilter Then
                        Try
                            aStep.InputChannels = _readPMUElements(stp)
                        Catch ex As Exception
                            _addLog("In a data quality filter step: " & aStep.StepCounter.ToString & " of data config. " & ex.Message)
                        End Try
                        For Each signal In aStep.InputChannels
                            signal.PassedThroughDQFilter = signal.PassedThroughDQFilter + 1
                            aStep.OutputChannels.Add(signal)
                        Next
                        CollectionOfSteps.Add(aStep)
                    End If
                    If TypeOf aStep Is Customization Then
                        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(aStep.InputChannels)
                        _signalMgr.GroupedSignalByDataConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                    End If
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(aStep.OutputChannels)
                    _signalMgr.GroupedSignalByDataConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                Next
            Next
            DataConfigure.CollectionOfSteps = CollectionOfSteps
        End Sub

        Private Sub _readWrappingFailureDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)
            Dim angle = params.<AngleThresh>.Value
            If angle IsNot Nothing Then
                aStep.AngleThresh = angle
            End If
        End Sub

        Private Sub _readDataFramePMUchanPMUallDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)
            Dim per = params.<PercentBadThresh>.Value
            If per IsNot Nothing Then
                aStep.PercentBadThresh = per
            End If
        End Sub

        Private Sub _readStaleDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)
            Dim par = params.<StaleThresh>.Value
            If par IsNot Nothing Then
                aStep.StaleThresh = par
            End If
            par = params.<FlagAllByFreq>.Value
            If par IsNot Nothing Then
                If par.ToLower = "true" Then
                    aStep.FlagAllByFreq = True
                Else
                    aStep.FlagAllByFreq = False
                End If
            End If
            'par = params.<FlagBitFreq>.Value
            'If par IsNot Nothing Then
            '    aStep.FlagBitFreq = par
            'End If
        End Sub

        Private Sub _readOutlierDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)
            Dim par = params.<StdDevMult>.Value
            If par IsNot Nothing Then
                aStep.StdDevMult = par
            End If
        End Sub

        Private Sub _readFreqDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)
            Dim par = params.<FreqMaxChan>.Value
            If par IsNot Nothing Then
                aStep.FreqMaxChan = par
            End If
            par = params.<FreqMinChan>.Value
            If par IsNot Nothing Then
                aStep.FreqMinChan = par
            End If
            par = params.<FreqPctChan>.Value
            If par IsNot Nothing Then
                aStep.FreqPctChan = par
            End If
            par = params.<FreqMinSamp>.Value
            If par IsNot Nothing Then
                aStep.FreqMinSamp = par
            End If
            par = params.<FreqMaxSamp>.Value
            If par IsNot Nothing Then
                aStep.FreqMaxSamp = par
            End If
            'par = params.<FlagBitChan>.Value
            'If par IsNot Nothing Then
            '    aStep.FlagBitChan = par
            'End If
            'par = params.<FlagBitSamp>.Value
            'If par IsNot Nothing Then
            '    aStep.FlagBitSamp = par
            'End If
        End Sub

        Private Sub _readVoltPhasorDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)
            Dim par = params.<VoltMin>.Value
            If par IsNot Nothing Then
                aStep.VoltMin = par
            End If
            par = params.<VoltMax>.Value
            If par IsNot Nothing Then
                aStep.VoltMax = par
            End If
            par = params.<NomVoltage>.Value
            If par IsNot Nothing Then
                aStep.NomVoltage = par
            End If
        End Sub

        Private Sub _readPlainDQFilter(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Object), stepCounter As Integer)

        End Sub

        Private Sub _readPhasorCreationCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim phasors = From phasor In params.Elements Where phasor.Name = "phasor" Select phasor
            For Each phasor In phasors
                Dim magSignal = _signalMgr.SearchForSignalInTaggedSignals(phasor.<mag>.<PMU>.Value, phasor.<mag>.<Channel>.Value)
                If magSignal IsNot Nothing Then
                    aStep.InputChannels.Add(magSignal)
                Else
                    magSignal = New SignalSignatureViewModel("ErrorReadingMag")
                    magSignal.IsValid = False
                    _addLog("Error reading config file! Signal in step: " & stepCounter & ", channel name: " & phasor.<mag>.<Channel>.Value & " in PMU " & phasor.<mag>.<PMU>.Value & " not found!")
                End If
                Dim angSignal = _signalMgr.SearchForSignalInTaggedSignals(phasor.<ang>.<PMU>.Value, phasor.<ang>.<Channel>.Value)
                If angSignal IsNot Nothing Then
                    aStep.InputChannels.Add(angSignal)
                Else
                    angSignal = New SignalSignatureViewModel("ErrorReadingAng")
                    angSignal.IsValid = False
                    _addLog("Error reading config file! Signal in step: " & stepCounter & ", channel name: " & phasor.<ang>.<Channel>.Value & " in PMU " & phasor.<ang>.<PMU>.Value & " not found!")
                End If
                Dim custSignalName = phasor.<CustName>.Value
                If String.IsNullOrEmpty(custSignalName) Then
                    custSignalName = "CustomSignalNameRequired"
                End If
                Dim output = New SignalSignatureViewModel(custSignalName, aStep.CustPMUname, "OTHER")
                output.IsCustomSignal = True
                If magSignal.IsValid AndAlso angSignal.IsValid Then
                    Dim mtype = magSignal.TypeAbbreviation.ToArray
                    Dim atype = angSignal.TypeAbbreviation.ToArray
                    If mtype(0) = atype(0) AndAlso mtype(2) = atype(2) AndAlso mtype(1) = "M" AndAlso atype(1) = "A" Then
                        output.TypeAbbreviation = mtype(0) & "P" & mtype(2)
                    Else
                        _addLog("In step: " & stepCounter & ", type of input magnitude siganl: " & magSignal.SignalName & ", does not match angle signal: " & angSignal.SignalName & ".")
                    End If
                    output.SamplingRate = magSignal.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(magSignal)
                newPair.Value.Add(angSignal)
                aStep.OutputInputMappingPair.Add(newPair)
            Next
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readAngleCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim signals = From signal In params.Elements Where signal.Name = "signal" Select signal
            For Each signal In signals
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
                If input IsNot Nothing Then
                    If aStep.InputChannels.Contains(input) Then
                        _addLog("Duplicate input signal found in step: " & stepCounter & " ," & aStep.Name & ".")
                    Else
                        aStep.InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.<Channel>.Value & " in PMU " & signal.<PMU>.Value & " not found!")
                End If
                Dim custSignalName = signal.<CustName>.Value
                If String.IsNullOrEmpty(custSignalName) Then
                    If input.IsValid Then
                        custSignalName = input.SignalName
                    Else
                        custSignalName = "CustomSignalNameRequired"
                    End If
                End If
                Dim output = New SignalSignatureViewModel(custSignalName, aStep.CustPMUname, "OTHER")
                output.IsCustomSignal = True
                If input.IsValid AndAlso input.TypeAbbreviation.Length = 3 Then
                    Dim letter2 = input.TypeAbbreviation.ToArray(1)
                    If letter2 = "P" Then
                        output.TypeAbbreviation = input.TypeAbbreviation.Substring(0, 1) & "A" & input.TypeAbbreviation.Substring(2, 1)
                    End If
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                aStep.OutputInputMappingPair.Add(newPair)
            Next
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readUnaryCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim signals = From signal In params.Elements Where signal.Name = "signal" Select signal
            For Each signal In signals
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
                If input IsNot Nothing Then
                    If aStep.InputChannels.Contains(input) Then
                        _addLog("Duplicate input signal found in step: " & stepCounter & " ," & aStep.Name & ".")
                    Else
                        aStep.InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.<Channel>.Value & " in PMU " & signal.<PMU>.Value & " not found!")
                End If
                Dim custSignalName = signal.<CustName>.Value
                'If String.IsNullOrEmpty(custSignalName) Then
                '    If input.IsValid Then
                '        custSignalName = input.SignalName
                '    Else
                '        custSignalName = "NoCustomSignalNameSpecified"
                '    End If
                'End If
                Dim output = New SignalSignatureViewModel(custSignalName, aStep.CustPMUname)
                output.IsCustomSignal = True
                If input.IsValid Then
                    output.TypeAbbreviation = input.TypeAbbreviation
                    output.Unit = input.Unit
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                aStep.OutputInputMappingPair.Add(newPair)
            Next
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readSubtractionCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim outputName = params.<SignalName>.Value
            If outputName Is Nothing Then
                outputName = ""
            End If
            Dim minuend = _signalMgr.SearchForSignalInTaggedSignals(params.<minuend>.<PMU>.Value, params.<minuend>.<Channel>.Value)
            If minuend Is Nothing Then
                minuend = New SignalSignatureViewModel("MinuentNotFound")
                minuend.IsValid = False
                _addLog("Error reading config file! Minuend in step: " & stepCounter & " with PMU: " & params.<minuend>.<PMU>.Value & ", and Channel: " & params.<minuend>.<Channel>.Value & " not found!")
            Else
                aStep.InputChannels.Add(minuend)
            End If
            aStep.MinuendOrDividend = minuend
            Dim subtrahend = _signalMgr.SearchForSignalInTaggedSignals(params.<subtrahend>.<PMU>.Value, params.<subtrahend>.<Channel>.Value)
            If subtrahend Is Nothing Then
                subtrahend = New SignalSignatureViewModel("SubtrahendNotFound")
                subtrahend.IsValid = False
                _addLog("Error reading config file! Subtrahend in step: " & stepCounter & " with PMU: " & params.<subtrahend>.<PMU>.Value & ", and Channel: " & params.<subtrahend>.<Channel>.Value & " not found!")
            Else
                aStep.InputChannels.Add(subtrahend)
            End If
            aStep.SubtrahendOrDivisor = subtrahend
            Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname)
            If minuend.IsValid AndAlso subtrahend.IsValid Then
                If minuend.TypeAbbreviation = subtrahend.TypeAbbreviation Then
                    output.TypeAbbreviation = minuend.TypeAbbreviation
                    output.Unit = minuend.Unit
                    output.SamplingRate = minuend.SamplingRate
                Else
                    _addLog("In step: " & stepCounter & " ," & aStep.Name & ", the types of Minuend and Subtrahend or divisor and dividend do not match!")
                End If
            End If
            output.IsCustomSignal = True
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            aStep.OutputChannels.Add(output)
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readRaiseExpCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            aStep.Exponent = params.<exponent>.Value
            If aStep.Exponent Is Nothing Then
                aStep.Exponent = 1
            End If
            Dim signals = From signal In params.Elements Where signal.Name = "signal" Select signal
            For Each signal In signals
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
                If input IsNot Nothing Then
                    If aStep.InputChannels.Contains(input) Then
                        _addLog("Duplicate input signal found in step: " & stepCounter & " ," & aStep.Name & ".")
                    Else
                        aStep.InputChannels.Add(input)
                    End If
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & ", with channel name: " & signal.<Channel>.Value & " in PMU " & signal.<PMU>.Value & " not found!")
                End If
                Dim custSignalName = signal.<CustName>.Value
                'If String.IsNullOrEmpty(custSignalName) Then
                '    If input.IsValid Then
                '        custSignalName = input.SignalName
                '    Else
                '        custSignalName = "NoCustomSignalNameSpecified"
                '    End If
                'End If
                Dim output = New SignalSignatureViewModel(custSignalName, aStep.CustPMUname)
                output.IsCustomSignal = True
                'If input.IsValid And input.TypeAbbreviation = "SC" Then
                '    output.TypeAbbreviation = "SC"
                '    output.Unit = "SC"
                'End If
                If input.IsValid Then
                    If aStep.Exponent = 1 OrElse input.TypeAbbreviation = "SC" Then
                        output.TypeAbbreviation = input.TypeAbbreviation
                        output.Unit = input.Unit
                    Else
                        output.TypeAbbreviation = "OTHER"
                        output.Unit = "OTHER"
                    End If
                    output.SamplingRate = input.SamplingRate
                End If
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                aStep.OutputInputMappingPair.Add(newPair)
            Next
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readDivisionCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim outputName = params.<SignalName>.Value
            If outputName Is Nothing Then
                outputName = ""
            End If
            Dim Dividend = _signalMgr.SearchForSignalInTaggedSignals(params.<dividend>.<PMU>.Value, params.<dividend>.<Channel>.Value)
            If Dividend Is Nothing Then
                Dividend = New SignalSignatureViewModel("DividendNotFound")
                Dividend.IsValid = False
                _addLog("Error reading config file! Dividend in step: " & stepCounter & ", with PMU: " & params.<dividend>.<PMU>.Value & ", and Channel: " & params.<dividend>.<Channel>.Value & " not found!")
            Else
                aStep.InputChannels.Add(Dividend)
            End If
            aStep.MinuendOrDividend = Dividend
            Dim Divisor = _signalMgr.SearchForSignalInTaggedSignals(params.<divisor>.<PMU>.Value, params.<divisor>.<Channel>.Value)
            If Divisor Is Nothing Then
                Divisor = New SignalSignatureViewModel("DivisorNotFound")
                Divisor.IsValid = False
                _addLog("Error reading config file! Divisor in step: " & stepCounter & ", with PMU: " & params.<divisor>.<PMU>.Value & ", and Channel: " & params.<divisor>.<Channel>.Value & " not found!")
            Else
                aStep.InputChannels.Add(Divisor)
            End If
            aStep.SubtrahendOrDivisor = Divisor
            Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname, "OTHER")
            If Dividend.IsValid AndAlso Divisor.IsValid Then
                If Dividend.TypeAbbreviation = Divisor.TypeAbbreviation Then
                    output.TypeAbbreviation = "SC"
                    output.Unit = "SC"
                    output.SamplingRate = Dividend.SamplingRate
                ElseIf Divisor.TypeAbbreviation = "SC" Then
                    output.TypeAbbreviation = Dividend.TypeAbbreviation
                    output.Unit = Dividend.Unit
                    output.SamplingRate = Dividend.SamplingRate
                Else
                    _addLog("In step: " & stepCounter & " ," & aStep.Name & ", the types of divisor and dividend do not agree!")
                End If
            End If
            output.IsCustomSignal = True
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            aStep.OutputChannels.Add(output)
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Private Sub _readScalarRepetitionCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, ByRef stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim outputName = params.<SignalName>.Value
            If outputName Is Nothing Then
                outputName = ""
            End If
            aStep.Scalar = params.<scalar>.Value
            Try
                Double.Parse(aStep.Scalar)
            Catch ex As Exception
                _addLog("Scalar repetition customization step: " & stepCounter & " has invalid scalar input that cannot be converted to a scalar.")
            End Try
            Dim type = params.<SignalType>.Value
            If type Is Nothing Then
                type = "SC"
            End If
            Dim unit = params.<SignalUnit>.Value
            If unit Is Nothing Then
                unit = "SC"
            End If
            Dim pmu = params.<TimeSourcePMU>.Value
            aStep.TimeSourcePMU = SignalMgr.AllPMUs.Where(Function(x) x.PMU = pmu).FirstOrDefault()
            aStep.Unit = unit
            aStep.Type = type
            Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname, type)
            output.IsCustomSignal = True
            output.Unit = unit
            If aStep.TimeSourcePMU IsNot Nothing Then
                output.SamplingRate = aStep.TimeSourcePMU.SamplingRate
            End If
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            aStep.OutputChannels.Add(output)
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readMultiplicationCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, ByRef stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim type = ""
            Dim unit = ""
            Dim samplingRate = -1
            Dim countNonScalarType = 0
            Dim factors = From factor In params.Elements Where factor.Name = "factor" Select factor
            For Each factor In factors
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(factor.<PMU>.Value, factor.<Channel>.Value)
                If input IsNot Nothing Then
                    aStep.InputChannels.Add(input)
                    If input.TypeAbbreviation <> "SC" Then
                        countNonScalarType += 1
                        If String.IsNullOrEmpty(type) Then
                            type = input.TypeAbbreviation
                        End If
                        If String.IsNullOrEmpty(unit) Then
                            unit = input.Unit
                        End If
                    End If
                    If samplingRate = -1 Then
                        samplingRate = input.SamplingRate
                    ElseIf samplingRate <> input.SamplingRate Then
                        _addLog("All factors of multiplication customization have to have the same sampling rate! Different sampling rate found in addition customization step: " & stepCounter & ", with sampling rate: " & samplingRate & " and " & input.SamplingRate & ".")
                    End If
                Else
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & "with channel name: " & factor.<Channel>.Value & ", and in PMU " & factor.<PMU>.Value & " not found!")
                End If
            Next
            Dim outputName = params.<SignalName>.Value
            If outputName Is Nothing Then
                outputName = ""
            End If
            Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname)
            If countNonScalarType = 0 Then
                output.TypeAbbreviation = "SC"
                output.Unit = "SC"
            ElseIf countNonScalarType = 1 Then
                output.TypeAbbreviation = type
                output.Unit = unit
                'TODO: ElseIf countNonScalarType == 2 AndAlso one of them is current magnitude and one of the is voltage magnitude, then we should get power P
                'TODO: Are there any other multiplication result in legal signal?
            Else
                output.TypeAbbreviation = "OTHER"
                output.Unit = "OTHER"
            End If
            output.SamplingRate = samplingRate
            output.IsCustomSignal = True
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            aStep.OutputChannels.Add(output)
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readAdditionCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As Object, ByRef stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim type = ""
            Dim unit = ""
            Dim samplingRate = -1
            Dim outputName = params.<SignalName>.Value
            If outputName Is Nothing Then
                outputName = ""
            End If
            Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname, "OTHER")
            output.IsCustomSignal = True
            Dim terms = From term In params.Elements Where term.Name = "term" Select term
            For Each term In terms
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(term.<PMU>.Value, term.<Channel>.Value)
                If input IsNot Nothing Then
                    aStep.InputChannels.Add(input)
                    If String.IsNullOrEmpty(type) Then
                        type = input.TypeAbbreviation
                    ElseIf type <> input.TypeAbbreviation Then
                        _addLog("All terms of addition customization have to be the same signal type! Different signal type found in addition customization step: " & stepCounter & ", with types: " & type & " and " & input.TypeAbbreviation & ".")
                    End If
                    If String.IsNullOrEmpty(unit) Then
                        unit = input.Unit
                    ElseIf unit <> input.Unit Then
                        _addLog("All terms of addition customization have to have the same unit! Different unit found in addition customization step: " & stepCounter & ", with unit: " & unit & " and " & input.Unit & ".")
                    End If
                    If samplingRate = -1 Then
                        samplingRate = input.SamplingRate
                    ElseIf samplingRate <> input.SamplingRate Then
                        _addLog("All terms of addition customization have to have the same sampling rate! Different sampling rate found in addition customization step: " & stepCounter & ", with sampling rate: " & samplingRate & " and " & input.SamplingRate & ".")
                    End If
                Else
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & "with channel name: " & term.<Channel>.Value & ", and in PMU " & term.<PMU>.Value & " not found!")
                End If
            Next
            If Not String.IsNullOrEmpty(type) Then
                output.TypeAbbreviation = type
            End If
            If Not String.IsNullOrEmpty(unit) Then
                output.Unit = unit
            End If
            If samplingRate <> -1 Then
                output.SamplingRate = samplingRate
            End If
            output.OldUnit = output.Unit
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldSignalName = output.SignalName
            aStep.OutputChannels.Add(output)
            collectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private _powerTypeDictionary As Dictionary(Of String, String)
        Private Sub _readPowerCalculationCustomization(ByRef aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As Object, ByRef stepCounter As Integer, ByVal sectionFlag As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            aStep.PowType = [Enum].Parse(GetType(PowerType), _powerTypeDictionary(params.<PowType>.Value))
            Dim powers = From el In params.Elements Where el.Name = "power" Select el
            For index = 0 To powers.Count - 1
                'For Each power In powers
                If index > 0 Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(aStep.InputChannels)
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(aStep.OutputChannels)
                    If sectionFlag = 1 Then
                        _signalMgr.GroupedSignalByDataConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                        _signalMgr.GroupedSignalByDataConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                    ElseIf sectionFlag = 3 Then
                        _signalMgr.GroupedSignalByPostProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                        _signalMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    Dim oldStep = aStep
                    aStep = New Customization(DirectCast(oldStep, Customization))
                    stepCounter += 1
                    aStep.StepCounter = stepCounter
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
                End If
                Dim signalName = powers(index).<CustName>.Value
                If String.IsNullOrEmpty(signalName) Then
                    signalName = "CustomSignalNameRequired"
                End If
                Dim typeAbbre = aStep.PowType.ToString
                Dim output = New SignalSignatureViewModel(signalName, aStep.CustPMUname, typeAbbre)
                output.IsCustomSignal = True
                Dim samplingRate = -1
                Dim signals = From el In powers(index).Elements Where el.Name <> "CustName"
                For Each signal In signals
                    Dim input = _signalMgr.SearchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
                    If input Is Nothing Then
                        input = New SignalSignatureViewModel("SignalNotFound")
                        input.IsValid = False
                        input.TypeAbbreviation = "C"
                        _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
                    Else
                        If samplingRate = -1 Then
                            samplingRate = input.SamplingRate
                        End If
                    End If
                    aStep.InputChannels.Add(input)
                Next
                Select Case output.TypeAbbreviation
                    Case "CP", "S"
                        output.Unit = "MVA"
                    Case "Q"
                        output.Unit = "MVAR"
                    Case "P"
                        output.Unit = "MW"
                End Select
                output.SamplingRate = samplingRate
                output.OldUnit = output.Unit
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldSignalName = output.SignalName
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                For Each signal In aStep.InputChannels
                    newPair.Value.Add(signal)
                Next
                aStep.OutputInputMappingPair.Add(newPair)
                CollectionOfSteps.Add(aStep)
                'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
            Next
        End Sub
        Private Sub _readMetricPrefixCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As Object, ByRef stepCounter As Integer)
            Dim CustPMUname = params.<CustPMUname>.Value
            If CustPMUname Is Nothing Then
                aStep.CustPMUname = _lastCustPMUname
                'aStep.UseCustomPMU = False
            Else
                'aStep.UseCustomPMU = True
                aStep.CustPMUname = CustPMUname
                _lastCustPMUname = CustPMUname
            End If
            Dim toConvert = From convert In params.Elements Where convert.Name = "ToConvert" Select convert
            For Each convert In toConvert
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(convert.<PMU>.Value, convert.<Channel>.Value)
                If input IsNot Nothing Then
                    aStep.InputChannels.Add(input)
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    input.TypeAbbreviation = "C"
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
                End If
                Dim outputName = convert.<CustName>.Value
                If outputName Is Nothing Then
                    outputName = input.SignalName
                End If
                Dim newUnit = convert.<NewUnit>.Value
                Dim output = input
                'If aStep.UseCustomPMU Then
                output = New SignalSignatureViewModel(outputName, CustPMUname, input.TypeAbbreviation)
                output.SamplingRate = input.SamplingRate
                output.Unit = newUnit
                output.OldSignalName = output.SignalName
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldUnit = output.Unit
                'Else
                '    output.OldUnit = output.Unit
                '    output.Unit = newUnit
                'End If
                output.IsCustomSignal = True
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                aStep.OutputInputMappingPair.Add(newPair)
            Next
            CollectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readAngleConversionCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As Object, ByRef stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim toConvert = From convert In params.Elements Where convert.Name = "ToConvert" Select convert
            For Each convert In toConvert
                Dim input = _signalMgr.SearchForSignalInTaggedSignals(convert.<PMU>.Value, convert.<Channel>.Value)
                If input IsNot Nothing Then
                    aStep.InputChannels.Add(input)
                Else
                    input = New SignalSignatureViewModel("SignalNotFound")
                    input.IsValid = False
                    input.TypeAbbreviation = "C"
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
                End If
                Dim outputName = convert.<CustName>.Value
                If outputName Is Nothing Then
                    outputName = input.SignalName
                End If
                Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname, input.TypeAbbreviation)
                output.SamplingRate = input.SamplingRate
                output.IsCustomSignal = True
                If input.Unit.ToLower = "deg" Then
                    output.Unit = "RAD"
                Else
                    output.Unit = "DEG"
                End If
                output.OldSignalName = output.SignalName
                output.OldTypeAbbreviation = output.TypeAbbreviation
                output.OldUnit = output.Unit
                aStep.OutputChannels.Add(output)
                Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
                newPair.Value.Add(input)
                aStep.OutputInputMappingPair.Add(newPair)
            Next
            CollectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub

        Private Sub _readSpecTypeUnitCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As Object, ByRef stepCounter As Integer)
            aStep.CustPMUname = params.<CustPMUname>.Value
            If String.IsNullOrEmpty(aStep.CustPMUname) Then
                aStep.CustPMUname = _lastCustPMUname
            Else
                _lastCustPMUname = aStep.CustPMUname
            End If
            Dim inputSignal = _signalMgr.SearchForSignalInTaggedSignals(params.<PMU>.Value, params.<Channel>.Value)
            'Dim toConvert = From convert In params.Elements Where convert.Name = "ToConvert" Select convert
            'For Each convert In toConvert
            'Dim input = _signalMgr.SearchForSignalInTaggedSignals(convert.<PMU>.Value, convert.<Channel>.Value)
            Dim samplingRate = -1
            If inputSignal Is Nothing Then
                inputSignal = New SignalSignatureViewModel("SignalNotFound")
                inputSignal.IsValid = False
                inputSignal.TypeAbbreviation = "C"
                _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
            Else
                samplingRate = inputSignal.SamplingRate
            End If
            aStep.InputChannels.Add(inputSignal)
            Dim outputName = params.<CustName>.Value
            If outputName Is Nothing Then
                outputName = inputSignal.SignalName
            End If
            Dim output = New SignalSignatureViewModel(outputName, aStep.CustPMUname, params.<SigType>.Value)
            output.IsCustomSignal = True
            output.Unit = params.<SigUnit>.Value
            output.SamplingRate = samplingRate
            output.OldSignalName = output.SignalName
            output.OldTypeAbbreviation = output.TypeAbbreviation
            output.OldUnit = output.Unit
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatureViewModel, ObservableCollection(Of SignalSignatureViewModel))(output, New ObservableCollection(Of SignalSignatureViewModel))
            newPair.Value.Add(inputSignal)
            aStep.OutputInputMappingPair.Add(newPair)
            'Next
            CollectionOfSteps.Add(aStep)
            'aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            'GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            'GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        End Sub
        Private _lastCustPMUname As String
        ''' <summary>
        ''' Go through both signals read from file (csv or pdat) and custom created signals to find a target signal by matching both PMU name and channel name
        ''' </summary>
        ''' <param name="pmu"></param>
        ''' <param name="channel"></param>
        ''' <returns></returns>
#End Region
#Region "Read Process Config"
        Private Sub _readProcessConfig(configData As XDocument)
            _signalMgr.GroupedSignalByProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
            _signalMgr.GroupedSignalByProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
            ProcessConfigure.InitializationPath = configData.<Config>.<ProcessConfig>.<Configuration>.<InitializationPath>.Value
            _readUnwrap(configData)
            _readInterpolate(configData)
            _readProcessConfigStages(configData)
            _readWrap(configData)
            _readNameTypeUnit(configData)
        End Sub

        Private Sub _readUnwrap(configData As XDocument)
            Dim newUnWrapList = New ObservableCollection(Of Unwrap)
            Dim unWraps = From el In configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Unwrap" Select el
            For Each unWrap As XElement In unWraps
                Dim aUnwrap = New Unwrap
                aUnwrap.StepCounter = _signalMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
                'aUnwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aUnwrap.StepCounter.ToString & "-" & aUnwrap.Name
                aUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aUnwrap.StepCounter.ToString & "-" & aUnwrap.Name
                'aUnwrap.MaxNaN = unWrap.<MaxNaN>.Value()
                Try
                    aUnwrap.InputChannels = _readPMUElements(unWrap)
                Catch ex As Exception
                    _addLog("In unwrap processing step: " & aUnwrap.StepCounter.ToString & ". " & ex.Message)
                End Try
                For Each signal In aUnwrap.InputChannels
                    signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                    aUnwrap.OutputChannels.Add(signal)
                Next
                aUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(aUnwrap.OutputChannels)
                _signalMgr.GroupedSignalByProcessConfigStepsOutput.Add(aUnwrap.ThisStepOutputsAsSignalHierachyByPMU)
                newUnWrapList.Add(aUnwrap)
            Next
            ProcessConfigure.UnWrapList = newUnWrapList
        End Sub
        Private Sub _readInterpolate(configData As XDocument)
            Dim newInterpolateList = New ObservableCollection(Of Interpolate)
            Dim interpolates = From el In configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Interpolate" Select el
            For Each interpolate As XElement In interpolates
                Dim anInterpolate = New Interpolate
                anInterpolate.StepCounter = _signalMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
                anInterpolate.Limit = interpolate.<Parameters>.<Limit>.Value
                anInterpolate.Type = [Enum].Parse(GetType(InterpolateType), interpolate.<Parameters>.<Type>.Value)
                anInterpolate.FlagInterp = Convert.ToBoolean(interpolate.<Parameters>.<FlagInterp>.Value)
                anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & anInterpolate.StepCounter.ToString & " - " & anInterpolate.Type.ToString() & " " & anInterpolate.Name
                Try
                    anInterpolate.InputChannels = _readPMUElements(interpolate)
                Catch ex As Exception
                    _addLog("In an interpolate processing step: " & anInterpolate.StepCounter.ToString & ". " & ex.Message)
                End Try
                For Each signal In anInterpolate.InputChannels
                    signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                    anInterpolate.OutputChannels.Add(signal)
                Next
                anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(anInterpolate.OutputChannels)
                _signalMgr.GroupedSignalByProcessConfigStepsOutput.Add(anInterpolate.ThisStepOutputsAsSignalHierachyByPMU)
                newInterpolateList.Add(anInterpolate)
            Next
            ProcessConfigure.InterpolateList = newInterpolateList
        End Sub
        Private Sub _readProcessConfigStages(configData As XDocument)
            Dim CollectionOfSteps As New ObservableCollection(Of Object)
            Dim stepCounter = _signalMgr.GroupedSignalByProcessConfigStepsOutput.Count
            Dim stages = From el In configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Stages" Select el
            For Each stage In stages
                Dim steps = From element In stage.Elements Select element
                For Each stp In steps
                    Dim aStep As Object
                    If stp.Name = "Filter" Then
                        aStep = New TunableFilter
                        stepCounter += 1
                        aStep.StepCounter = stepCounter
                        aStep.Type = [Enum].Parse(GetType(TunableFilterType), stp.<Type>.Value)
                        Dim params = From ps In stp.<Parameters>.Elements Select ps
                        Select Case aStep.Type
                            Case TunableFilterType.HighPass
                                aStep.Order = stp.<Parameters>.<Order>.Value
                                aStep.Cutoff = stp.<Parameters>.<Cutoff>.Value
                            Case TunableFilterType.LowPass
                                aStep.PassRipple = stp.<Parameters>.<PassRipple>.Value
                                aStep.StopRipple = stp.<Parameters>.<StopRipple>.Value
                                aStep.PassCutoff = stp.<Parameters>.<PassCutoff>.Value
                                aStep.StopCutoff = stp.<Parameters>.<StopCutoff>.Value
                            Case TunableFilterType.Rational
                                aStep.Numerator = stp.<Parameters>.<Numerator>.Value
                                aStep.Denominator = stp.<Parameters>.<Denominator>.Value
                            Case Else
                                Throw New Exception("Unknown tunable filter type found in Config.xml.")
                        End Select
                    ElseIf stp.Name = "Multirate" Then
                        aStep = New Multirate
                        stepCounter += 1
                        aStep.StepCounter = stepCounter
                        Dim params = From ps In stp.<Parameters>.Elements Select ps
                        For Each pair In params
                            'Dim aPair As New ParameterValuePair
                            Dim paraName = pair.Name.ToString
                            'aPair.ParameterName = paraName
                            'aPair.Value = pair.Value
                            'aStep.FilterParameters.Add(aPair)
                            If paraName = "MultiRatePMU" Then
                                aStep.MultiRatePMU = pair.Value()
                            ElseIf paraName = "NewRate" Then
                                aStep.NewRate = pair.Value()
                                aStep.FilterChoice = 1
                            ElseIf paraName = "p" Then
                                aStep.PElement = pair.Value()
                                aStep.FilterChoice = 2
                            ElseIf paraName = "q" Then
                                aStep.QElement = pair.Value()
                                aStep.FilterChoice = 2
                            End If
                        Next
                        aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                        aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    End If
                    Try
                        aStep.InputChannels = _readPMUElements(stp)
                    Catch ex As Exception
                        _addLog("In a " & aStep.GetType.ToString & " processing step: " & aStep.StepCounter.ToString & ". " & ex.Message)
                    End Try
                    For Each signal In aStep.InputChannels
                        If TypeOf aStep Is Multirate Then
                            Dim output = New SignalSignatureViewModel(signal.SignalName, aStep.MultiRatePMU, signal.TypeAbbreviation)
                            output.SamplingRate = aStep.NewRate
                            output.Unit = signal.Unit
                            output.IsCustomSignal = True
                            output.OldSignalName = output.SignalName
                            output.OldTypeAbbreviation = output.TypeAbbreviation
                            output.OldUnit = output.Unit
                            aStep.OutputChannels.Add(output)
                        Else
                            signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                            aStep.OutputChannels.Add(signal)
                        End If
                    Next
                    If TypeOf aStep Is Multirate Then
                        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(aStep.InputChannels)
                        _signalMgr.GroupedSignalByProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                    End If
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(aStep.OutputChannels)
                    _signalMgr.GroupedSignalByProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                    CollectionOfSteps.Add(aStep)
                Next
            Next
            ProcessConfigure.CollectionOfSteps = CollectionOfSteps
        End Sub
        Private Sub _readWrap(configData As XDocument)
            Dim newWrapList = New ObservableCollection(Of Wrap)
            Dim Wraps = From el In configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Wrap" Select el
            For Each Wrap As XElement In Wraps
                Dim aWrap = New Wrap
                aWrap.StepCounter = _signalMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
                aWrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aWrap.StepCounter.ToString & "-" & aWrap.Name
                Try
                    aWrap.InputChannels = _readPMUElements(Wrap)
                Catch ex As Exception
                    _addLog("In a wrap processing step: " & aWrap.StepCounter.ToString & ". " & ex.Message)
                End Try
                For Each signal In aWrap.InputChannels
                    signal.PassedThroughProcessor = signal.PassedThroughProcessor + 1
                    aWrap.OutputChannels.Add(signal)
                Next
                aWrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(aWrap.OutputChannels)
                _signalMgr.GroupedSignalByProcessConfigStepsOutput.Add(aWrap.ThisStepOutputsAsSignalHierachyByPMU)
                newWrapList.Add(aWrap)
            Next
            ProcessConfigure.WrapList = newWrapList
        End Sub
        Private Sub _readNameTypeUnit(configData As XDocument)
            ProcessConfigure.NameTypeUnitElement = New NameTypeUnit
            Dim NameTypeUnitExist = From el In configData.<Config>.<ProcessConfig>.<Configuration>.Elements() Where el.Name = "NameTypeUnit" Select el
            If NameTypeUnitExist.Count <> 0 Then
                Dim pmus = From el In configData.<Config>.<ProcessConfig>.<Configuration>.<NameTypeUnit>.Elements() Where el.Name = "PMU" Select el
                If pmus.Count() = 0 Then
                    Dim newUnit = configData.<Config>.<ProcessConfig>.<Configuration>.<NameTypeUnit>.<NewUnit>.Value()
                    Dim newType = configData.<Config>.<ProcessConfig>.<Configuration>.<NameTypeUnit>.<NewType>.Value()
                    If Not newUnit Is Nothing OrElse Not newType Is Nothing Then
                        ProcessConfigure.NameTypeUnitElement.NewUnit = newUnit
                        ProcessConfigure.NameTypeUnitElement.NewType = newType
                        NameTypeUnitStatusFlag = 1
                    End If
                Else
                    Dim newPMU As NameTypeUnitPMU
                    For Each pmu In pmus
                        Dim pmuName = pmu.<Name>.Value
                        Dim CurrentChannel = pmu.<CurrentChannel>.Value
                        Dim NewChannel = pmu.<NewChannel>.Value
                        Dim NewUnit = pmu.<NewUnit>.Value
                        Dim NewType = pmu.<NewType>.Value

                        Dim pmuFound = False
                        If CurrentChannel = NewChannel Then
                            For Each pmuItem In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                                If pmuItem.NewType = NewType AndAlso pmuItem.NewUnit = NewUnit Then
                                    pmuFound = True
                                    newPMU = pmuItem
                                    newPMU.NewChannel = ""
                                    Exit For
                                End If
                            Next
                        End If

                        If Not pmuFound Then
                            newPMU = New NameTypeUnitPMU()
                            newPMU.NewType = NewType
                            newPMU.NewUnit = NewUnit
                            newPMU.NewChannel = NewChannel
                            ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Add(newPMU)
                        End If
                        Dim input = _signalMgr.SearchForSignalInTaggedSignals(pmuName, CurrentChannel)
                        If input IsNot Nothing Then
                            If input.IsNameTypeUnitChanged Then
                                _addLog("Error reading config file! Signal in a NameTypeUnit step : " & CurrentChannel & " in PMU " & pmuName & " has already gone through another NameTypeUnit step, a signal is not allow to go through NameTypeUnit step twice.")
                            Else
                                If Not String.IsNullOrEmpty(NewChannel) Then
                                    input.OldSignalName = input.SignalName
                                    input.SignalName = NewChannel
                                End If
                                input.OldTypeAbbreviation = input.TypeAbbreviation
                                input.TypeAbbreviation = NewType
                                input.OldUnit = input.Unit
                                input.Unit = NewUnit
                                input.PassedThroughProcessor = input.PassedThroughProcessor + 1
                                input.IsNameTypeUnitChanged = True
                                newPMU.InputChannels.Add(input)
                                newPMU.OutputChannels.Add(input)
                            End If
                        Else
                            _addLog("Error reading config file! Signal in a NameTypeUnit step of processing with channel name: " & CurrentChannel & " in PMU " & pmuName & " not found!")
                        End If
                    Next
                    For Each pmuItem In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
                        pmuItem.StepCounter = _signalMgr.GroupedSignalByProcessConfigStepsOutput.Count + 1
                        pmuItem.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & pmuItem.StepCounter.ToString & "-" & pmuItem.Name
                        pmuItem.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(pmuItem.OutputChannels)
                        _signalMgr.GroupedSignalByProcessConfigStepsOutput.Add(pmuItem.ThisStepOutputsAsSignalHierachyByPMU)
                    Next
                End If
            End If
        End Sub
#End Region
        Private Sub _readPostProcessConfig(configData As XDocument)
            _signalMgr.GroupedSignalByPostProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
            _signalMgr.GroupedSignalByPostProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
            Dim CollectionOfSteps As New ObservableCollection(Of Customization)
            Dim stepCounter As Integer = 0
            Dim stages = From el In configData.<Config>.<PostProcessCustomizationConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
            For Each stage In stages
                Dim steps = From element In stage.Elements Select element
                For Each stp In steps
                    Dim aStep = New Object
                    'aStep.Name = PostProcessConfigure.CustomizationReverseNameDictionary(stp.<Name>.Value)

                    Dim thisStepName = PostProcessConfigure.CustomizationReverseNameDictionary(stp.<Name>.Value)

                    'stepCounter += 1
                    'aStep.StepCounter = stepCounter
                    'aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    'aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name

                    'Dim signalForUnitTypeSpecificationCustomization As SignalSignatures = Nothing
                    Select Case thisStepName
                        Case "Scalar Repetition"
                            aStep = New ScalarRepCust
                            aStep.Name = thisStepName
                            _readScalarRepetitionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Addition"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readAdditionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Subtraction"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readSubtractionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Division"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readDivisionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Multiplication"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readMultiplicationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Exponential"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readRaiseExpCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Sign Reversal", "Absolute Value", "Real Component", "Imaginary Component", "Complex Conjugate"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readUnaryCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Angle Calculation"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readAngleCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Phasor Creation"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readPhasorCreationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Signal Type/Unit"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readSpecTypeUnitCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Power Calculation"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readPowerCalculationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter, 3)
                        Case "Metric Prefix"
                            aStep = New MetricPrefixCust
                            aStep.Name = thisStepName
                            _readMetricPrefixCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case "Angle Conversion"
                            aStep = New Customization
                            aStep.Name = thisStepName
                            _readAngleConversionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                        Case Else
                            _addLog("Cutomization not recognized in Post process configuration.")
                            Throw New Exception("Cutomization not recognized in Post process configuration.")
                    End Select


                    stepCounter += 1
                    aStep.StepCounter = stepCounter
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name

                    If TypeOf aStep Is Customization Then
                        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(aStep.InputChannels)
                        _signalMgr.GroupedSignalByPostProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                    End If
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = _signalMgr.SortSignalByPMU(aStep.OutputChannels)
                    _signalMgr.GroupedSignalByPostProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                Next
            Next
            PostProcessConfigure.CollectionOfSteps = CollectionOfSteps
        End Sub
#Region "Reading detector config"
        Private Sub _readDetectorConfig(ByRef configData As XDocument)
            _signalMgr.GroupedSignalByDetectorInput = New ObservableCollection(Of SignalTypeHierachy)
            Dim newDetectorList = New ObservableCollection(Of DetectorBase)
            Dim detectors = From el In configData.<Config>.<DetectorConfig>.<Configuration>.Elements Select el
            'Dim previousVoltageStabilityDetector As VoltageStability.Models.VoltageStabilityDetector
            For Each detector In detectors
                Select Case detector.Name
                    Case "EventPath"
                        DetectorConfigure.EventPath = detector.Value
                    Case "ResultUpdateInterval"
                        DetectorConfigure.ResultUpdateInterval = detector.Value
                    Case "OutOfRangeGeneral"
                        _readOutOfRangeFrequency(detector, newDetectorList)
                    '    _readOutOfRangeGeneral(detector, newDetectorList)
                    'Case "OutOfRangeFrequency"
                    '    _readOutOfRangeFrequency(detector, newDetectorList)
                    Case "Ringdown"
                        _readRingdown(detector, newDetectorList)
                    Case "WindRamp"
                        _readWindRamp(detector, newDetectorList)
                    Case "Periodogram"
                        _readPeriodogram(detector, newDetectorList)
                        DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible
                    Case "SpectralCoherence"
                        _readSpectralCoherence(detector, newDetectorList)
                        DetectorConfigure.ResultUpdateIntervalVisibility = Visibility.Visible
                    Case "Alarming"
                        _readAlarming(detector)
                        'Case "Thevenin"
                        '    Dim newVoltageStabilityDetector As VoltageStability.Models.VoltageStabilityDetector
                        '    Dim newDetectorGroupID = detector.<DetectorGroupID>.Value
                        '    If previousVoltageStabilityDetector IsNot Nothing AndAlso previousVoltageStabilityDetector.DetectorGroupID = newDetectorGroupID Then
                        '        Dim newMethod = detector.<Method>.Value
                        '        previousVoltageStabilityDetector.AddMethod(newMethod)
                        '    Else
                        '        newVoltageStabilityDetector = New VoltageStability.Models.VoltageStabilityDetectorGroupReader(detector).GetDetector()
                        '        newDetectorList.Add(New VoltageStability.ViewModels.VoltageStabilityDetectorViewModel(newVoltageStabilityDetector))
                        '        previousVoltageStabilityDetector = newVoltageStabilityDetector
                        'Else
                        '    If previousVoltageStabilityDetector.DetectorGroupID = newDetectorGroupID Then
                        '        Dim newMethod = detector.<Method>.Value
                        '        previousVoltageStabilityDetector.AddMethod(newMethod)
                        '    Else
                        '        newVoltageStabilityDetector = New VoltageStability.Models.VoltageStabilityDetectorGroupReader(detector).GetDetector()
                        '        newDetectorList.Add(newVoltageStabilityDetector)
                        '        previousVoltageStabilityDetector = newVoltageStabilityDetector
                        '    End If
                        'End If
                    Case Else
                        Throw New Exception("Unknown element found in DetectorConfig in config file.")
                End Select
            Next
            DetectorConfigure.DetectorList = newDetectorList
        End Sub

        Private Sub _readAlarming(detector As XElement)
            Dim newAlarmingList = New ObservableCollection(Of AlarmingDetectorBase)
            For Each alarm In detector.Elements
                Select Case alarm.Name
                    Case "SpectralCoherence"
                        Dim newDetector = New AlarmingSpectralCoherence
                        newDetector.CoherenceAlarm = alarm.<CoherenceAlarm>.Value
                        newDetector.CoherenceMin = alarm.<CoherenceMin>.Value
                        newDetector.TimeMin = alarm.<TimeMin>.Value
                        newDetector.CoherenceCorner = alarm.<CoherenceCorner>.Value
                        newDetector.TimeCorner = alarm.<TimeCorner>.Value
                        newAlarmingList.Add(newDetector)
                    Case "Periodogram"
                        Dim newDetector = New AlarmingPeriodogram
                        newDetector.SNRalarm = alarm.<SNRalarm>.Value
                        newDetector.SNRmin = alarm.<SNRmin>.Value
                        newDetector.TimeMin = alarm.<TimeMin>.Value
                        newDetector.SNRcorner = alarm.<SNRcorner>.Value
                        newDetector.TimeCorner = alarm.<TimeCorner>.Value
                        newAlarmingList.Add(newDetector)
                    Case "Ringdown"
                        Dim v = alarm.<MaxDuration>.Value
                        If Not String.IsNullOrEmpty(v) Then
                            Dim newDetector = New AlarmingRingdown
                            newDetector.MaxDuration = v
                            newAlarmingList.Add(newDetector)
                        End If
                    Case Else
                        Throw New Exception("Error! Unknown alarming detector elements found in config file.")
                End Select
                DetectorConfigure.AlarmingList = newAlarmingList
            Next
        End Sub

        Private Sub _readSpectralCoherence(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
            Dim newDetector = New SpectralCoherenceDetector
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & newDetector.Name
            newDetector.Mode = [Enum].Parse(GetType(DetectorModeType), detector.<Mode>.Value)
            newDetector.AnalysisLength = detector.<AnalysisLength>.Value
            newDetector.Delay = detector.<Delay>.Value
            newDetector.NumberDelays = detector.<NumberDelays>.Value
            newDetector.ThresholdScale = detector.<ThresholdScale>.Value
            newDetector.WindowType = [Enum].Parse(GetType(DetectorWindowType), detector.<WindowType>.Value)
            newDetector.FrequencyInterval = detector.<FrequencyInterval>.Value
            newDetector.WindowLength = detector.<WindowLength>.Value
            newDetector.WindowOverlap = detector.<WindowOverlap>.Value
            newDetector.FrequencyMin = detector.<FrequencyMin>.Value
            newDetector.FrequencyMax = detector.<FrequencyMax>.Value
            newDetector.FrequencyTolerance = detector.<FrequencyTolerance>.Value
            Try
                newDetector.InputChannels = _readPMUElements(detector)
            Catch ex As Exception
                _addLog("In Spectral Coherence detector: " & ex.Message)
            End Try
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
            _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            newList.Add(newDetector)
        End Sub

        Private Sub _readPeriodogram(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
            Dim newDetector = New PeriodogramDetector
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
            newDetector.Mode = [Enum].Parse(GetType(DetectorModeType), detector.<Mode>.Value)
            newDetector.AnalysisLength = detector.<AnalysisLength>.Value
            newDetector.WindowType = [Enum].Parse(GetType(DetectorWindowType), detector.<WindowType>.Value)
            newDetector.WindowLength = detector.<WindowLength>.Value
            newDetector.FrequencyInterval = detector.<FrequencyInterval>.Value
            newDetector.WindowOverlap = detector.<WindowOverlap>.Value
            newDetector.MedianFilterFrequencyWidth = detector.<MedianFilterFrequencyWidth>.Value
            newDetector.Pfa = detector.<Pfa>.Value
            newDetector.FrequencyMin = detector.<FrequencyMin>.Value
            newDetector.FrequencyMax = detector.<FrequencyMax>.Value
            newDetector.FrequencyTolerance = detector.<FrequencyTolerance>.Value
            Try
                newDetector.InputChannels = _readPMUElements(detector)
            Catch ex As Exception
                _addLog("In Periodogram detector: " & ex.Message)
            End Try
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
            _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            newList.Add(newDetector)
        End Sub

        Private Sub _readWindRamp(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
            Dim newDetector = New WindRampDetector
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & newDetector.Name
            newDetector.Fpass = detector.<Fpass>.Value
            'Determin whether it is long trend or short trend wind ramp detector by checking the value of Fpass
            'This value could be changed later
            If Not String.IsNullOrEmpty(newDetector.Fpass) AndAlso newDetector.Fpass = "0.00005" Then
                newDetector.IsLongTrend = True
            Else
                newDetector.IsLongTrend = False
            End If
            newDetector.Fstop = detector.<Fstop>.Value
            newDetector.Apass = detector.<Apass>.Value
            newDetector.Astop = detector.<Astop>.Value
            newDetector.ValMin = detector.<ValMin>.Value
            newDetector.TimeMin = detector.<TimeMin>.Value
            newDetector.ValMax = detector.<ValMax>.Value
            newDetector.TimeMax = detector.<TimeMax>.Value
            Try
                newDetector.InputChannels = _readPMUElements(detector)
            Catch ex As Exception
                _addLog("In Windramp detector" & ex.Message)
            End Try
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
            _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            newList.Add(newDetector)
        End Sub

        Private Sub _readRingdown(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
            Dim newDetector = New RingdownDetector
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & newDetector.Name
            newDetector.RMSlength = detector.<RMSlength>.Value
            'newDetector.ForgetFactor = detector.<ForgetFactor>.Value
            newDetector.RingThresholdScale = detector.<RingThresholdScale>.Value
            newDetector.RMSmedianFilterTime = detector.<RMSmedianFilterTime>.Value
            'newDetector.MaxDuration = detector.<MaxDuration>.Value
            Try
                newDetector.InputChannels = _readPMUElements(detector)
            Catch ex As Exception
                _addLog("In Ringdown detector" & ex.Message)
            End Try
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
            _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            newList.Add(newDetector)
        End Sub

        Private Sub _readOutOfRangeFrequency(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
            Dim newDetector = New OutOfRangeFrequencyDetector
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & newDetector.Name
            Dim type = From el In detector.Elements Where el.Name = "AverageWindow" Select el
            If type.Count = 0 Then
                newDetector.Type = OutOfRangeFrequencyDetectorType.Nominal
                newDetector.Nominal = type.Value
            Else
                newDetector.Type = OutOfRangeFrequencyDetectorType.AvergeWindow
                newDetector.AverageWindow = type.Value
            End If
            newDetector.DurationMax = detector.<DurationMax>.Value
            newDetector.DurationMin = detector.<DurationMin>.Value
            newDetector.Duration = detector.<Duration>.Value
            newDetector.AnalysisWindow = detector.<AnalysisWindow>.Value
            newDetector.RateOfChangeMax = detector.<RateOfChangeMax>.Value
            newDetector.RateOfChangeMin = detector.<RateOfChangeMin>.Value
            newDetector.RateOfChange = detector.<RateOfChange>.Value
            newDetector.EventMergeWindow = detector.<EventMergeWindow>.Value
            Try
                newDetector.InputChannels = _readPMUElements(detector)
            Catch ex As Exception
                _addLog("In out of range frequency detector" & ex.Message)
            End Try
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
            _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            newList.Add(newDetector)
        End Sub

        Private Sub _readOutOfRangeGeneral(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
            Dim newDetector = New OutOfRangeGeneralDetector
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & (_signalMgr.GroupedSignalByDetectorInput.Count + 1).ToString & " " & newDetector.Name
            newDetector.Max = detector.<Max>.Value
            newDetector.Min = detector.<Min>.Value
            newDetector.AnalysisWindow = detector.<AnalysisWindow>.Value
            newDetector.Duration = detector.<Duration>.Value
            Try
                newDetector.InputChannels = _readPMUElements(detector)
            Catch ex As Exception
                _addLog("In out of range general detector" & ex.Message)
            End Try
            newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = _signalMgr.SortSignalByType(newDetector.InputChannels)
            _signalMgr.GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
            newList.Add(newDetector)
        End Sub
#End Region

    End Class

End Namespace
