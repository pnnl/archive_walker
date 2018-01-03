Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Forms
Imports BAWGUI
'Imports PDAT_Reader
Imports System.Linq
Imports Microsoft.Expression.Interactivity.Core
Partial Public Class SettingsViewModel
    Private Sub _readConfigFile(ByRef configData As XDocument)

        '''''''''''''''''''''' Read DataConfig''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim fileInfoList = New ObservableCollection(Of InputFileInfo)
        Dim inputInformation = From el In configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.Elements Where el.Name = "FilePath" Select el
        For Each el In inputInformation
            Dim info = New InputFileInfo
            info.FileDirectory = el.<FileDirectory>.Value
            Dim type = el.<FileType>.Value
            If type IsNot Nothing Then
                info.FileType = [Enum].Parse(GetType(DataFileType), type.ToLower)
            End If
            info.Mnemonic = el.<Mnemonic>.Value
            fileInfoList.Add(info)
            _buildInputFileFolderTree(info)
        Next

        DataConfigure.ReaderProperty.InputFileInfos = fileInfoList

        DataConfigure.ReaderProperty.ModeName = [Enum].Parse(GetType(ModeType), configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Name>.Value)
        Select Case DataConfigure.ReaderProperty.ModeName
            Case ModeType.Archive
                DataConfigure.ReaderProperty.DateTimeStart = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeStart>.Value
                DataConfigure.ReaderProperty.DateTimeEnd = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeEnd>.Value
            Case ModeType.Hybrid
                DataConfigure.ReaderProperty.DateTimeStart = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeStart>.Value
                DataConfigure.ReaderProperty.NoFutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<NoFutureWait>.Value
                DataConfigure.ReaderProperty.MaxNoFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxNoFutureCount>.Value
                DataConfigure.ReaderProperty.FutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<FutureWait>.Value
                DataConfigure.ReaderProperty.MaxFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxFutureCount>.Value
                DataConfigure.ReaderProperty.RealTimeRange = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<RealTimeRange>.Value
            Case ModeType.RealTime
                DataConfigure.ReaderProperty.NoFutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<NoFutureWait>.Value
                DataConfigure.ReaderProperty.MaxNoFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxNoFutureCount>.Value
                DataConfigure.ReaderProperty.FutureWait = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<FutureWait>.Value
                DataConfigure.ReaderProperty.MaxFutureCount = configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxFutureCount>.Value
            Case Else
                Throw New Exception("Error: invalid mode type found in config file.")
        End Select
        _readDataConfigStages(configData)
        '''''''''''''''''''''''''''''''''Read ProcessConfig and PostProcessConfig''''''''''''''''''''''''''''''''''''''''''''''''''''
        _readProcessConfig(configData)
        _readPostProcessConfig(configData)
        '''''''''''''''''''''''''''''''''Read DetectorConfig'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        _readDetectorConfig(configData)
    End Sub

    Private Function _readPMUElements(stp As XElement) As ObservableCollection(Of SignalSignatures)
        Dim inputSignalList = New ObservableCollection(Of SignalSignatures)
        Dim inputs = From el In stp.Elements Where el.Name = "PMU" Select el
        If inputs.ToList.Count > 0 Then

            For Each aInput In inputs
                Dim pmuName = aInput.<Name>.Value()
                Dim channels = From el In aInput.Elements Where el.Name = "Channel" Select el
                If channels.ToList.Count > 0 Then

                    For Each channel In channels
                        Dim signalName = channel.<Name>.Value
                        Dim signal = _searchForSignalInTaggedSignals(pmuName, signalName)
                        If signal IsNot Nothing Then
                            inputSignalList.Add(signal)
                        Else
                            Throw New Exception("Error reading config file! Signal with channel name: " & signalName & " in PMU " & pmuName & " not found!")
                        End If
                    Next
                Else
                    For Each group In GroupedRawSignalsByPMU
                        For Each subgroup In group.SignalList
                            If subgroup.SignalSignature.PMUName = pmuName Then

                                For Each signal In subgroup.SignalList
                                    inputSignalList.Add(signal.SignalSignature)
                                Next
                            End If
                        Next
                    Next
                End If
            Next
        Else
            Throw New Exception("Warning! No PMU specified, no channel or no PMU is included.")
        End If
        Return inputSignalList
    End Function

#Region "Read Data Config Customization Steps From XML Configure File"
    Private Sub _readDataConfigStages(configData As XDocument)
        Dim CollectionOfSteps As New ObservableCollection(Of Object)
        GroupedSignalByDataConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)
        GroupedSignalByDataConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
        Dim stepCounter As Integer = 0
        Dim stages = From el In configData.<Config>.<DataConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
        For Each stage In stages
            Dim steps = From element In stage.Elements Select element
            For Each stp In steps
                Dim aStep As Object

                If stp.Name = "Filter" Then
                    aStep = New DQFilter
                    aStep.Name = DataConfigure.DQFilterReverseNameDictionary(stp.<Name>.Value)
                    'necessaryParams.AddRange(DataConfigure.DQFilterNameParametersDictionary(aStep.Name))
                ElseIf stp.Name = "Customization" Then
                    aStep = New Customization
                    aStep.Name = DataConfigure.CustomizationReverseNameDictionary(stp.<Name>.Value)
                    'necessaryParams.AddRange(DataConfigure.CustomizationNameParemetersDictionary(aStep.Name))
                End If
                stepCounter += 1
                aStep.StepCounter = stepCounter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name

                'Dim signalForUnitTypeSpecificationCustomization As SignalSignatures = Nothing
                Select Case aStep.Name
                    Case "Scalar Repetition Customization"
                        _readScalarRepetitionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Addition Customization"
                        _readAdditionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Subtraction Customization"
                        _readSubtractionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Division Customization"
                        _readDivisionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Multiplication Customization"
                        _readMultiplicationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Raise signals to an exponent"
                        _readRaiseExpCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Take complex conjugate of signals"
                        _readUnaryCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Return angle of complex valued signals"
                        _readAngleCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Phasor Creation Customization"
                        _readPhasorCreationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Specify Signal Type and Unit Customization"
                        _readSpecTypeUnitCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Power Calculation Customization"
                        _readPowerCalculationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter, 1)
                    Case "Metric Prefix Customization", "Angle Conversion Customization"
                        _readMetricPrefixOrAngleConversionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case Else
                        Dim params = From ps In stp.<Parameters>.Elements Select ps
                        For Each pair In params
                            Dim aPair As New ParameterValuePair
                            Dim paraName = pair.Name.ToString
                            aPair.ParameterName = paraName
                            If pair.Value.ToLower = "false" Then
                                aPair.Value = False
                            ElseIf pair.Value.ToLower = "true" Then
                                aPair.Value = True
                            ElseIf aStep.Name = "Nominal-Value Frequency Data Quality Filter" And paraName = "FlagBit" Then
                                aPair.IsRequired = False
                                aPair.Value = pair.Value
                            Else
                                aPair.Value = pair.Value
                            End If
                            aStep.FilterParameters.Add(aPair)
                        Next
                        Try
                            aStep.InputChannels = _readPMUElements(stp)
                        Catch ex As Exception
                            _addLog("In a data quality filter step: " & aStep.StepCounter.ToString & " of data config. " & ex.Message)
                        End Try
                        For Each signal In aStep.InputChannels
                            signal.PassedThroughDQFilter = True
                            aStep.OutputChannels.Add(signal)
                        Next
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        ' to add this step that is for sure a DQ filter to the list of step for signal manipulation and customization.
                        ' Don't not move it outside the select case loop!!!!!
                        ' Or it will cause customization steps being added twice.
                        CollectionOfSteps.Add(aStep)
                        ' Leave this line here! Don't move it!
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                End Select
                If TypeOf aStep Is Customization Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                    GroupedSignalByDataConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                End If
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                GroupedSignalByDataConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
            Next
        Next
        DataConfigure.CollectionOfSteps = CollectionOfSteps
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
            Dim magSignal = _searchForSignalInTaggedSignals(phasor.<mag>.<PMU>.Value, phasor.<mag>.<Channel>.Value)
            If magSignal IsNot Nothing Then
                aStep.InputChannels.Add(magSignal)
            Else
                magSignal = New SignalSignatures("ErrorReadingMag")
                magSignal.IsValid = False
                _addLog("Error reading config file! Signal in step: " & stepCounter & ", channel name: " & phasor.<mag>.<Channel>.Value & " in PMU " & phasor.<mag>.<PMU>.Value & " not found!")
            End If
            Dim angSignal = _searchForSignalInTaggedSignals(phasor.<ang>.<PMU>.Value, phasor.<ang>.<Channel>.Value)
            If angSignal IsNot Nothing Then
                aStep.InputChannels.Add(angSignal)
            Else
                angSignal = New SignalSignatures("ErrorReadingAng")
                angSignal.IsValid = False
                _addLog("Error reading config file! Signal in step: " & stepCounter & ", channel name: " & phasor.<ang>.<Channel>.Value & " in PMU " & phasor.<ang>.<PMU>.Value & " not found!")
            End If
            Dim custSignalName = phasor.<CustName>.Value
            If String.IsNullOrEmpty(custSignalName) Then
                custSignalName = "CustomSignalNameRequired"
            End If
            Dim output = New SignalSignatures(custSignalName, aStep.CustPMUname, "OTHER")
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
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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
            Dim input = _searchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
            If input IsNot Nothing Then
                If aStep.InputChannels.Contains(input) Then
                    _addLog("Duplicate input signal found in step: " & stepCounter & " ," & aStep.Name & ".")
                Else
                    aStep.InputChannels.Add(input)
                End If
            Else
                input = New SignalSignatures("SignalNotFound")
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
            Dim output = New SignalSignatures(custSignalName, aStep.CustPMUname, "OTHER")
            output.IsCustomSignal = True
            If input.IsValid AndAlso input.TypeAbbreviation.Length = 3 Then
                Dim letter2 = input.TypeAbbreviation.ToArray(1)
                If letter2 = "P" Then
                    output.TypeAbbreviation = input.TypeAbbreviation.Substring(0, 1) & "A" & input.TypeAbbreviation.Substring(2, 1)
                End If
                output.SamplingRate = input.SamplingRate
            End If
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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
            Dim input = _searchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
            If input IsNot Nothing Then
                If aStep.InputChannels.Contains(input) Then
                    _addLog("Duplicate input signal found in step: " & stepCounter & " ," & aStep.Name & ".")
                Else
                    aStep.InputChannels.Add(input)
                End If
            Else
                input = New SignalSignatures("SignalNotFound")
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
            Dim output = New SignalSignatures(custSignalName, aStep.CustPMUname)
            output.IsCustomSignal = True
            If input.IsValid Then
                output.TypeAbbreviation = input.TypeAbbreviation
                output.Unit = input.Unit
                output.SamplingRate = input.SamplingRate
            End If
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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
        Dim minuend = _searchForSignalInTaggedSignals(params.<minuend>.<PMU>.Value, params.<minuend>.<Channel>.Value)
        If minuend Is Nothing Then
            minuend = New SignalSignatures("MinuentNotFound")
            minuend.IsValid = False
            _addLog("Error reading config file! Minuend in step: " & stepCounter & " with PMU: " & params.<minuend>.<PMU>.Value & ", and Channel: " & params.<minuend>.<Channel>.Value & " not found!")
        Else
            aStep.InputChannels.Add(minuend)
        End If
        aStep.MinuendOrDividend = minuend
        Dim subtrahend = _searchForSignalInTaggedSignals(params.<subtrahend>.<PMU>.Value, params.<subtrahend>.<Channel>.Value)
        If subtrahend Is Nothing Then
            subtrahend = New SignalSignatures("SubtrahendNotFound")
            subtrahend.IsValid = False
            _addLog("Error reading config file! Subtrahend in step: " & stepCounter & " with PMU: " & params.<subtrahend>.<PMU>.Value & ", and Channel: " & params.<subtrahend>.<Channel>.Value & " not found!")
        Else
            aStep.InputChannels.Add(subtrahend)
        End If
        aStep.SubtrahendOrDivisor = subtrahend
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname)
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
            Dim input = _searchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
            If input IsNot Nothing Then
                If aStep.InputChannels.Contains(input) Then
                    _addLog("Duplicate input signal found in step: " & stepCounter & " ," & aStep.Name & ".")
                Else
                    aStep.InputChannels.Add(input)
                End If
            Else
                input = New SignalSignatures("SignalNotFound")
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
            Dim output = New SignalSignatures(custSignalName, aStep.CustPMUname)
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
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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
        Dim Dividend = _searchForSignalInTaggedSignals(params.<dividend>.<PMU>.Value, params.<dividend>.<Channel>.Value)
        If Dividend Is Nothing Then
            Dividend = New SignalSignatures("DividendNotFound")
            Dividend.IsValid = False
            _addLog("Error reading config file! Dividend in step: " & stepCounter & ", with PMU: " & params.<dividend>.<PMU>.Value & ", and Channel: " & params.<dividend>.<Channel>.Value & " not found!")
        Else
            aStep.InputChannels.Add(Dividend)
        End If
        aStep.MinuendOrDividend = Dividend
        Dim Divisor = _searchForSignalInTaggedSignals(params.<divisor>.<PMU>.Value, params.<divisor>.<Channel>.Value)
        If Divisor Is Nothing Then
            Divisor = New SignalSignatures("DivisorNotFound")
            Divisor.IsValid = False
            _addLog("Error reading config file! Divisor in step: " & stepCounter & ", with PMU: " & params.<divisor>.<PMU>.Value & ", and Channel: " & params.<divisor>.<Channel>.Value & " not found!")
        Else
            aStep.InputChannels.Add(Divisor)
        End If
        aStep.SubtrahendOrDivisor = Divisor
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, "OTHER")
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
        aStep.TimeSourcePMU.PMU = params.<TimeSourcePMU>.Value

        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, type)
        output.IsCustomSignal = True
        output.Unit = unit
        output.SamplingRate = GroupedRawSignalsByPMU.SelectMany(Function(x) x.SignalList).Distinct.Select(Function(y) y.SignalSignature).Where(Function(z) z.PMUName = aStep.TimeSourcePMU.PMU).Select(Function(n) n.SamplingRate).FirstOrDefault()
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
            Dim input = _searchForSignalInTaggedSignals(factor.<PMU>.Value, factor.<Channel>.Value)
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
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname)
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
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, "OTHER")
        output.IsCustomSignal = True
        Dim terms = From term In params.Elements Where term.Name = "term" Select term
        For Each term In terms
            Dim input = _searchForSignalInTaggedSignals(term.<PMU>.Value, term.<Channel>.Value)
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
                aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                If sectionFlag = 1 Then
                    GroupedSignalByDataConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                    GroupedSignalByDataConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                ElseIf sectionFlag = 3 Then
                    GroupedSignalByPostProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                    GroupedSignalByPostProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
                End If
                Dim oldStep = aStep
                aStep = New Customization(oldStep)
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
            Dim output = New SignalSignatures(signalName, aStep.CustPMUname, typeAbbre)
            output.IsCustomSignal = True
            Dim samplingRate = -1
            Dim signals = From el In powers(index).Elements Where el.Name <> "CustName"
            For Each signal In signals
                Dim input = _searchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
                If input Is Nothing Then
                    input = New SignalSignatures("SignalNotFound")
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
            output.SamplingRate = samplingRate
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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

    Private Sub _readMetricPrefixOrAngleConversionCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As Object, ByRef stepCounter As Integer)
        aStep.CustPMUname = params.<CustPMUname>.Value
        If String.IsNullOrEmpty(aStep.CustPMUname) Then
            aStep.CustPMUname = _lastCustPMUname
        Else
            _lastCustPMUname = aStep.CustPMUname
        End If
        Dim toConvert = From convert In params.Elements Where convert.Name = "ToConvert" Select convert
        For Each convert In toConvert
            Dim input = _searchForSignalInTaggedSignals(convert.<PMU>.Value, convert.<Channel>.Value)
            If input IsNot Nothing Then
                aStep.InputChannels.Add(input)
            Else
                input = New SignalSignatures("SignalNotFound")
                input.IsValid = False
                input.TypeAbbreviation = "C"
                _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
            End If
            Dim outputName = convert.<CustName>.Value
            If outputName Is Nothing Then
                outputName = input.SignalName
            End If
            Dim output = New SignalSignatures(outputName, aStep.CustPMUname, input.TypeAbbreviation)
            output.SamplingRate = input.SamplingRate
            output.IsCustomSignal = True
            output.Unit = convert.<NewUnit>.Value
            aStep.OutputChannels.Add(output)
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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
        Dim inputSignal = _searchForSignalInTaggedSignals(params.<PMU>.Value, params.<Channel>.Value)
        'Dim toConvert = From convert In params.Elements Where convert.Name = "ToConvert" Select convert
        'For Each convert In toConvert
        'Dim input = _searchForSignalInTaggedSignals(convert.<PMU>.Value, convert.<Channel>.Value)
        Dim samplingRate = -1
        If inputSignal Is Nothing Then
            inputSignal = New SignalSignatures("SignalNotFound")
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
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, params.<SigType>.Value)
        output.IsCustomSignal = True
        output.Unit = params.<SigUnit>.Value
        output.SamplingRate = samplingRate
        aStep.OutputChannels.Add(output)
        Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
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
    Private Function _searchForSignalInTaggedSignals(pmu As String, channel As String) As SignalSignatures
        For Each group In GroupedRawSignalsByPMU
            For Each samplingRateSubgroup In group.SignalList
                For Each subgroup In samplingRateSubgroup.SignalList
                    If subgroup.SignalSignature.PMUName = pmu Then
                        For Each subsubgroup In subgroup.SignalList
                            If subsubgroup.SignalSignature.SignalName = channel Then
                                Return subsubgroup.SignalSignature
                            End If
                        Next
                    End If
                Next
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsOutput
            For Each samplingRateSubgroup In group.SignalList
                For Each subgroup In samplingRateSubgroup.SignalList
                    If subgroup.SignalSignature.PMUName = pmu Then
                        For Each subsubgroup In subgroup.SignalList
                            If subsubgroup.SignalSignature.SignalName = channel Then
                                Return subsubgroup.SignalSignature
                            End If
                        Next
                    End If
                Next
            Next
        Next
        For Each group In GroupedSignalByProcessConfigStepsOutput
            For Each samplingRateSubgroup In group.SignalList
                For Each subgroup In samplingRateSubgroup.SignalList
                    If subgroup.SignalSignature.PMUName = pmu Then
                        For Each subsubgroup In subgroup.SignalList
                            If subsubgroup.SignalSignature.SignalName = channel Then
                                Return subsubgroup.SignalSignature
                            End If
                        Next
                    End If
                Next
            Next
        Next
        For Each group In GroupedSignalByPostProcessConfigStepsOutput
            For Each samplingRateSubgroup In group.SignalList
                For Each subgroup In samplingRateSubgroup.SignalList
                    If subgroup.SignalSignature.PMUName = pmu Then
                        For Each subsubgroup In subgroup.SignalList
                            If subsubgroup.SignalSignature.SignalName = channel Then
                                Return subsubgroup.SignalSignature
                            End If
                        Next
                    End If
                Next
            Next
        Next
        Return Nothing
    End Function
#End Region
#Region "Read Process Config"
    Private Sub _readProcessConfig(configData As XDocument)
        GroupedSignalByProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
        GroupedSignalByProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
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
            aUnwrap.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
            'aUnwrap.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aUnwrap.StepCounter.ToString & "-" & aUnwrap.Name
            aUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aUnwrap.StepCounter.ToString & "-" & aUnwrap.Name
            aUnwrap.MaxNaN = unWrap.<MaxNaN>.Value()
            Try
                aUnwrap.InputChannels = _readPMUElements(unWrap)
            Catch ex As Exception
                _addLog("In unwrap processing step: " & aUnwrap.StepCounter.ToString & ". " & ex.Message)
            End Try
            For Each signal In aUnwrap.InputChannels
                signal.PassedThroughProcessor = True
                aUnwrap.OutputChannels.Add(signal)
            Next
            aUnwrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aUnwrap.OutputChannels)
            GroupedSignalByProcessConfigStepsOutput.Add(aUnwrap.ThisStepOutputsAsSignalHierachyByPMU)
            newUnWrapList.Add(aUnwrap)
        Next
        ProcessConfigure.UnWrapList = newUnWrapList
    End Sub
    Private Sub _readInterpolate(configData As XDocument)
        Dim newInterpolateList = New ObservableCollection(Of Interpolate)
        Dim interpolates = From el In configData.<Config>.<ProcessConfig>.<Configuration>.<Processing>.Elements Where el.Name = "Interpolate" Select el
        For Each interpolate As XElement In interpolates
            Dim anInterpolate = New Interpolate
            anInterpolate.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
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
                signal.PassedThroughProcessor = True
                anInterpolate.OutputChannels.Add(signal)
            Next
            anInterpolate.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(anInterpolate.OutputChannels)
            GroupedSignalByProcessConfigStepsOutput.Add(anInterpolate.ThisStepOutputsAsSignalHierachyByPMU)
            newInterpolateList.Add(anInterpolate)
        Next
        ProcessConfigure.InterpolateList = newInterpolateList
    End Sub
    Private Sub _readProcessConfigStages(configData As XDocument)
        Dim CollectionOfSteps As New ObservableCollection(Of Object)
        Dim stepCounter = GroupedSignalByProcessConfigStepsOutput.Count
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
                    For Each pair In params
                        'Dim aPair As New ParameterValuePair
                        Dim paraName = pair.Name.ToString
                        Dim aPair = (From x In DirectCast(aStep, TunableFilter).FilterParameters Where x.ParameterName = paraName Select x).FirstOrDefault
                        If pair.Value.ToLower = "false" Then
                            aPair.Value = False
                        ElseIf pair.Value.ToLower = "true" Then
                            aPair.Value = True
                        ElseIf paraName = "Endpoints" Then
                            aPair.Value = [Enum].Parse(GetType(EndpointsType), pair.Value)
                        ElseIf paraName = "HandleNaN" Then
                            aPair.Value = [Enum].Parse(GetType(HandleNaNType), pair.Value)
                            'ElseIf aStep.Name = "Nominal-Value Frequency Data Quality Filter" And paraName = "FlagBit" Then
                            '    aPair.IsRequired = False
                            '    aPair.Value = pair.Value
                        Else
                            aPair.Value = pair.Value
                        End If
                    Next
                ElseIf stp.Name = "Multirate" Then
                    aStep = New Multirate
                    stepCounter += 1
                    aStep.StepCounter = stepCounter
                    Dim params = From ps In stp.<Parameters>.Elements Select ps
                    For Each pair In params
                        Dim aPair As New ParameterValuePair
                        Dim paraName = pair.Name.ToString
                        aPair.ParameterName = paraName
                        aPair.Value = pair.Value
                        aStep.FilterParameters.Add(aPair)
                        If paraName = "MultiRatePMU" Then
                            aStep.MultiRatePMU = pair.Value()
                        ElseIf paraName = "NewRate" Then
                            aStep.NewRate = pair.Value()
                        ElseIf paraName = "p" Then
                            aStep.PElement = pair.Value()
                        ElseIf paraName = "q" Then
                            aStep.QElement = pair.Value()
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
                    signal.PassedThroughProcessor = True
                    If TypeOf aStep Is Multirate Then
                        Dim output = New SignalSignatures(signal.SignalName, aStep.MultiRatePMU, signal.TypeAbbreviation)
                        output.SamplingRate = signal.SamplingRate
                        output.Unit = signal.Unit
                        output.IsCustomSignal = True
                        aStep.OutputChannels.Add(output)
                    Else
                        aStep.OutputChannels.Add(signal)
                    End If
                Next
                If TypeOf aStep Is Multirate Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                    GroupedSignalByProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                End If
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                GroupedSignalByProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
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
            aWrap.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
            aWrap.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aWrap.StepCounter.ToString & "-" & aWrap.Name
            Try
                aWrap.InputChannels = _readPMUElements(Wrap)
            Catch ex As Exception
                _addLog("In a wrap processing step: " & aWrap.StepCounter.ToString & ". " & ex.Message)
            End Try
            For Each signal In aWrap.InputChannels
                signal.PassedThroughProcessor = True
                aWrap.OutputChannels.Add(signal)
            Next
            aWrap.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aWrap.OutputChannels)
            GroupedSignalByProcessConfigStepsOutput.Add(aWrap.ThisStepOutputsAsSignalHierachyByPMU)
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
                ProcessConfigure.NameTypeUnitElement.NewUnit = configData.<Config>.<ProcessConfig>.<Configuration>.<NameTypeUnit>.<NewUnit>.Value()
                ProcessConfigure.NameTypeUnitElement.NewType = configData.<Config>.<ProcessConfig>.<Configuration>.<NameTypeUnit>.<NewType>.Value()
                NameTypeUnitStatusFlag = 1
            Else
                Dim newPMU As NameTypeUnitPMU
                'newPMU.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
                'newPMU.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newPMU.StepCounter.ToString & "-" & newPMU.Name
                For Each pmu In pmus
                    Dim pmuName = pmu.<Name>.Value
                    Dim CurrentChannel = pmu.<CurrentChannel>.Value
                    Dim NewChannel = pmu.<NewChannel>.Value
                    Dim NewUnit = pmu.<NewUnit>.Value
                    Dim NewType = pmu.<NewType>.Value

                    If newPMU Is Nothing OrElse newPMU.NewType <> NewType OrElse newPMU.NewUnit <> NewUnit OrElse CurrentChannel <> NewChannel Then

                        If newPMU IsNot Nothing Then
                            newPMU.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(newPMU.OutputChannels)
                            GroupedSignalByProcessConfigStepsOutput.Add(newPMU.ThisStepOutputsAsSignalHierachyByPMU)
                            ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Add(newPMU)
                        End If
                        newPMU = New NameTypeUnitPMU()
                        newPMU.StepCounter = GroupedSignalByProcessConfigStepsOutput.Count + 1
                        newPMU.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newPMU.StepCounter.ToString & "-" & newPMU.Name

                        newPMU.NewType = NewType
                        newPMU.NewUnit = NewUnit
                        newPMU.NewChannel = NewChannel

                    End If
                    If newPMU.InputChannels.Count > 0 Then
                        newPMU.NewChannel = ""
                    End If
                    Dim input = _searchForSignalInTaggedSignals(pmuName, CurrentChannel)
                    If input IsNot Nothing Then
                        newPMU.InputChannels.Add(input)
                        Dim output = New SignalSignatures(NewChannel, pmuName, NewType)
                        output.Unit = NewUnit
                        output.IsCustomSignal = True
                        newPMU.OutputChannels.Add(output)
                    Else
                        _addLog("Error reading config file! Signal in a step of processing with channel name: " & CurrentChannel & " in PMU " & pmuName & " not found!")
                    End If

                Next
                newPMU.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(newPMU.OutputChannels)
                GroupedSignalByProcessConfigStepsOutput.Add(newPMU.ThisStepOutputsAsSignalHierachyByPMU)
                ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Add(newPMU)
            End If
        End If
    End Sub
#End Region
    Private Sub _readPostProcessConfig(configData As XDocument)
        GroupedSignalByPostProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
        GroupedSignalByPostProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
        Dim CollectionOfSteps As New ObservableCollection(Of Customization)
        Dim stepCounter As Integer = 0
        Dim stages = From el In configData.<Config>.<PostProcessCustomizationConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
        For Each stage In stages
            Dim steps = From element In stage.Elements Select element
            For Each stp In steps
                Dim aStep = New Customization
                aStep.Name = PostProcessConfigure.CustomizationReverseNameDictionary(stp.<Name>.Value)
                stepCounter += 1
                aStep.StepCounter = stepCounter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & " - " & aStep.Name

                'Dim signalForUnitTypeSpecificationCustomization As SignalSignatures = Nothing
                Select Case aStep.Name
                    Case "Scalar Repetition Customization"
                        _readScalarRepetitionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Addition Customization"
                        _readAdditionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Subtraction Customization"
                        _readSubtractionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Division Customization"
                        _readDivisionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Multiplication Customization"
                        _readMultiplicationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Raise signals to an exponent"
                        _readRaiseExpCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Take complex conjugate of signals"
                        _readUnaryCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Return angle of complex valued signals"
                        _readAngleCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Phasor Creation Customization"
                        _readPhasorCreationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Specify Signal Type and Unit Customization"
                        _readSpecTypeUnitCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Power Calculation Customization"
                        _readPowerCalculationCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter, 3)
                    Case "Metric Prefix Customization", "Angle Conversion Customization"
                        _readMetricPrefixOrAngleConversionCustomization(aStep, stp.<Parameters>, CollectionOfSteps, stepCounter)
                    Case Else
                        _addLog("Cutomization not recognized in Post process configuration.")
                        Throw New Exception("Cutomization not recognized in Post process configuration.")
                End Select
                If TypeOf aStep Is Customization Then
                    aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                    GroupedSignalByPostProcessConfigStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                End If
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                GroupedSignalByPostProcessConfigStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
            Next
        Next
        PostProcessConfigure.CollectionOfSteps = CollectionOfSteps
    End Sub
#Region "Reading detector config"
    Private Sub _readDetectorConfig(ByRef configData As XDocument)
        GroupedSignalByDetectorInput = New ObservableCollection(Of SignalTypeHierachy)
        Dim newDetectorList = New ObservableCollection(Of DetectorBase)
        Dim detectors = From el In configData.<Config>.<DetectorConfig>.<Configuration>.Elements Select el
        For Each detector In detectors
            Select Case detector.Name
                Case "EventPath"
                    DetectorConfigure.EventPath = detector.Value
                Case "ResultUpdateInterval"
                    DetectorConfigure.ResultUpdateInterval = detector.Value
                Case "OutOfRangeGeneral"
                    _readOutOfRangeGeneral(detector, newDetectorList)
                Case "OutOfRangeFrequency"
                    _readOutOfRangeFrequency(detector, newDetectorList)
                Case "Ringdown"
                    _readRingdown(detector, newDetectorList)
                Case "WindRamp"
                    _readWindRamp(detector, newDetectorList)
                Case "Periodogram"
                    _readPeriodogram(detector, newDetectorList)
                    ResultUpdateIntervalVisibility = Visibility.Visible
                Case "SpectralCoherence"
                    _readSpectralCoherence(detector, newDetectorList)
                    ResultUpdateIntervalVisibility = Visibility.Visible
                Case "Alarming"
                    _readAlarming(detector)
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
                    Dim newDetector = New AlarmingRingdown
                    newDetector.MaxDuration = alarm.<MaxDuration>.Value
                    newAlarmingList.Add(newDetector)
                Case Else
                    Throw New Exception("Error! Unknown alarming detector elements found in config file.")
            End Select
            DetectorConfigure.AlarmingList = newAlarmingList
        Next
    End Sub

    Private Sub _readSpectralCoherence(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New SpectralCoherenceDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
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
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readPeriodogram(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New PeriodogramDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
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
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readWindRamp(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New WindRampDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.Fpass = detector.<Fpass>.Value
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
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readRingdown(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New RingdownDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.RMSlength = detector.<RMSlength>.Value
        newDetector.ForgetFactor = detector.<ForgetFactor>.Value
        newDetector.RingThresholdScale = detector.<RingThresholdScale>.Value
        'newDetector.MaxDuration = detector.<MaxDuration>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In Ringdown detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readOutOfRangeFrequency(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New OutOfRangeFrequencyDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
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
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In out of range frequency detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub

    Private Sub _readOutOfRangeGeneral(detector As XElement, ByRef newList As ObservableCollection(Of DetectorBase))
        Dim newDetector = New OutOfRangeGeneralDetector
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = (GroupedSignalByDetectorInput.Count + 1).ToString & " Detector " & newDetector.Name
        newDetector.Max = detector.<Max>.Value
        newDetector.Min = detector.<Min>.Value
        newDetector.AnalysisWindow = detector.<AnalysisWindow>.Value
        newDetector.Duration = detector.<Duration>.Value
        Try
            newDetector.InputChannels = _readPMUElements(detector)
        Catch ex As Exception
            _addLog("In out of range general detector" & ex.Message)
        End Try
        newDetector.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(newDetector.InputChannels)
        GroupedSignalByDetectorInput.Add(newDetector.ThisStepInputsAsSignalHerachyByType)
        newList.Add(newDetector)
    End Sub
#End Region

End Class
