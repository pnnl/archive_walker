Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Forms
Imports BAWGUI
Imports PDAT_Reader
Imports System.Linq
Imports Microsoft.Expression.Interactivity.Core

'Imports BAWGUI.DataConfig

Partial Public Class SettingsViewModel
    Inherits ViewModelBase

    Public Sub New()
        _configFileName = ""
        _lastCustPMUname = ""
        '_sampleFile = ""

        _dataConfigure = New DataConfig
        _processConfigure = New ProcessConfig
        _postProcessConfigure = New PostProcessCustomizationConfig
        _detectorConfigure = New DetectorConfig
        _logs = New ObservableCollection(Of String)

        _openConfigFile = New DelegateCommand(AddressOf openConfigXMLFile, AddressOf CanExecute)
        _browseInputFileDir = New DelegateCommand(AddressOf _browseInputFileFolder, AddressOf CanExecute)
        _fileTypeChanged = New DelegateCommand(AddressOf _buildInputFileFolderTree, AddressOf CanExecute)
        _dqfilterSelected = New DelegateCommand(AddressOf _dqfilterSelection, AddressOf CanExecute)
        _customizationSelected = New DelegateCommand(AddressOf _customizationStepSelection, AddressOf CanExecute)
        _selectedSignalChanged = New DelegateCommand(AddressOf _signalSelected, AddressOf CanExecute)
        _dataConfigStepSelected = New DelegateCommand(AddressOf _stepSelectedToEdit, AddressOf CanExecute)
        _dataConfigStepDeSelected = New DelegateCommand(AddressOf _deSelectAllDataConfigSteps, AddressOf CanExecute)
        _setCurrentFocusedTextbox = New DelegateCommand(AddressOf _currentFocusedTextBoxChanged, AddressOf CanExecute)
        _setCurrentFocusedTextboxUnarySteps = New DelegateCommand(AddressOf _currentFocusedTextBoxForUnaryStepsChanged, AddressOf CanExecute)
        '_selectedOutputSignalChanged = New DelegateCommand(AddressOf _outputSignalSelectionChanged, AddressOf CanExecute)
        _textboxesLostFocus = New DelegateCommand(AddressOf _recoverCheckStatusOfCurrentStep, AddressOf CanExecute)
        _deleteDataConfigStep = New DelegateCommand(AddressOf _deleteADataConfigStep, AddressOf CanExecute)
        _powerPhasorTextBoxGotFocus = New DelegateCommand(AddressOf _powerPhasorCurrentFocusedTextbox, AddressOf CanExecute)
        _choosePhasorForPowerCalculation = New DelegateCommand(AddressOf _powerCalculationPhasorOption, AddressOf CanExecute)
        _chooseMagAngForPowerCalculation = New DelegateCommand(AddressOf _powerCalculationMagAngOption, AddressOf CanExecute)
        _addFileSource = New DelegateCommand(AddressOf _addAFileSource, AddressOf CanExecute)
        _deleteThisFileSource = New DelegateCommand(AddressOf _deleteAFileSource, AddressOf CanExecute)
        _saveConfigFile = New DelegateCommand(AddressOf _saveConfigureFile, AddressOf CanExecute)
        _saveConfigFileAs = New DelegateCommand(AddressOf _saveConfigureFileAs, AddressOf CanExecute)
        _walkerStageChanged = New DelegateCommand(AddressOf _changeSignalSelectionDropDownChoice, AddressOf CanExecute)
        _addUnwrap = New DelegateCommand(AddressOf _addAUnwrap, AddressOf CanExecute)
        _deleteUnwrapStep = New DelegateCommand(AddressOf _deleteAUnwrap, AddressOf CanExecute)
        _addInterpolate = New DelegateCommand(AddressOf _addAnInterpolate, AddressOf CanExecute)
        _deleteInterpolateStep = New DelegateCommand(AddressOf _deleteAnInterpolate, AddressOf CanExecute)
        _addWrap = New DelegateCommand(AddressOf _addAWrap, AddressOf CanExecute)
        _deleteWrap = New DelegateCommand(AddressOf _deleteAWrap, AddressOf CanExecute)
        _addTunableFilterOrMultirate = New DelegateCommand(AddressOf _addATunableFilterOrMultirate, AddressOf CanExecute)
        _deleteTunableFilterOrMultirate = New DelegateCommand(AddressOf _deleteATunableFilterOrMultirate, AddressOf CanExecute)
        _multirateParameterChoice = New DelegateCommand(AddressOf _chooseParameterForMultirate, AddressOf CanExecute)
        _processConfigStepSelected = New DelegateCommand(AddressOf _processStepSelectedToEdit, AddressOf CanExecute)
        _processConfigStepDeSelected = New DelegateCommand(AddressOf _deSelectAllProcessConfigSteps, AddressOf CanExecute)
        _deleteNameTypeUnit = New DelegateCommand(AddressOf _deleteANameTypeUnit, AddressOf CanExecute)
        _addNameTypeUnit = New DelegateCommand(AddressOf _addANameTypeUnit, AddressOf CanExecute)
        _postProcessConfigStepSelected = New DelegateCommand(AddressOf _postProcessConfigureStepSelected, AddressOf CanExecute)
        _postProcessConfigStepDeSelected = New DelegateCommand(AddressOf _deSelectAllPostProcessConfigSteps, AddressOf CanExecute)
        _deletePostProcessStep = New DelegateCommand(AddressOf _deleteAPostProcessStep, AddressOf CanExecute)
        '_postProcessingSelected = New DelegateCommand(AddressOf _selectPostProcessing, AddressOf CanExecute)

        '_inputFileDirTree = New ObservableCollection(Of Folder)
        _groupedRawSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
        _groupedRawSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByDataConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByDataConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)()
        _groupedSignalByProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)()
        _groupedSignalByPostProcessConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByPostProcessConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
        _allDataConfigOutputGroupedByPMU = New ObservableCollection(Of SignalTypeHierachy)
        _allDataConfigOutputGroupedByType = New ObservableCollection(Of SignalTypeHierachy)
        _allProcessConfigOutputGroupedByPMU = New ObservableCollection(Of SignalTypeHierachy)
        _allProcessConfigOutputGroupedByType = New ObservableCollection(Of SignalTypeHierachy)
        _allPostProcessOutputGroupedByPMU = New ObservableCollection(Of SignalTypeHierachy)
        _allPostProcessOutputGroupedByType = New ObservableCollection(Of SignalTypeHierachy)

        _allPMUs = New ObservableCollection(Of String)

        _timezoneList = TimeZoneInfo.GetSystemTimeZones
        _signalSelectionTreeViewVisibility = "Visible"
        _selectSignalMethods = {"All Initial Input Channels by Signal Type",
                                "All Initial Input Channels by PMU",
                                "Input Channels by Step",
                                "Output Channels by Step"}.ToList
        _selectedSelectionMethod = "All Initial Input Channels by Signal Type"
        '_powerTypeDictionary = New Dictionary(Of String, String) From {{"CP", "Complex"}, {"S", "Apparent"}, {"P", "Active"}, {"Q", "Reactive"}}
        _powerTypeDictionary = New Dictionary(Of String, String) From {{"Complex", "CP"}, {"Apparent", "S"}, {"Active", "P"}, {"Reactive", "Q"}}
        _nameTypeUnitStatusFlag = 0
    End Sub

    'Private _pmuSignalDictionary As Dictionary(Of String, List(Of SignalSignatures))
    'Public Property PMUSignalDictionary As Dictionary(Of String, List(Of SignalSignatures))
    '    Get
    '        Return _pmuSignalDictionary
    '    End Get
    '    Set(ByVal value As Dictionary(Of String, List(Of SignalSignatures)))
    '        _pmuSignalDictionary = value
    '        OnPropertyChanged()
    '    End Set
    'End Property
    Private _groupedRawSignalsByType As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedRawSignalsByType As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedRawSignalsByType
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedRawSignalsByType = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedRawSignalsByPMU As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedRawSignalsByPMU As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedRawSignalsByPMU
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedRawSignalsByPMU = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedSignalByDataConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByDataConfigStepsInput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByDataConfigStepsInput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByDataConfigStepsInput = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedSignalByDataConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByDataConfigStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByDataConfigStepsOutput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByDataConfigStepsOutput = value
            OnPropertyChanged()
        End Set
    End Property
    Private _allDataConfigOutputGroupedByType As ObservableCollection(Of SignalTypeHierachy)
    Public Property AllDataConfigOutputGroupedByType As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _allDataConfigOutputGroupedByType
        End Get
        Set(value As ObservableCollection(Of SignalTypeHierachy))
            _allDataConfigOutputGroupedByType = value
            OnPropertyChanged()
        End Set
    End Property
    Private _allDataConfigOutputGroupedByPMU As ObservableCollection(Of SignalTypeHierachy)
    Public Property AllDataConfigOutputGroupedByPMU As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _allDataConfigOutputGroupedByPMU
        End Get
        Set(value As ObservableCollection(Of SignalTypeHierachy))
            _allDataConfigOutputGroupedByPMU = value
            OnPropertyChanged()
        End Set
    End Property
    Private Sub _tagSignals(fileInfo As InputFileInfo, signalList As List(Of String))
        Dim newSignalList As New ObservableCollection(Of SignalSignatures)
        For Each name In signalList
            Dim signal As New SignalSignatures
            'signal.SignalName = name
            Dim nameParts = name.Split(".")
            signal.PMUName = nameParts(0)
            signal.SamplingRate = fileInfo.SamplingRate
            If nameParts.Length = 3 Then
                Select Case nameParts(2)
                    Case "F"
                        signal.TypeAbbreviation = "F"
                        signal.SignalName = nameParts(0) & ".frq"
                    Case "R"
                        signal.TypeAbbreviation = "RCF"
                        signal.SignalName = nameParts(0) & ".rocof"
                    Case "A"
                        signal.SignalName = nameParts(0) & "." & nameParts(1) & ".ANG"
                        Dim channel = nameParts(1).Substring(nameParts(1).Length - 2).ToArray
                        If channel(0) = "I" OrElse channel(0) = "V" Then
                            signal.TypeAbbreviation = channel(0) & "A" & channel(1)
                        Else
                            signal.TypeAbbreviation = "OTHER"
                            _addLog("Signal name " & signal.SignalName & " does not comply naming convention. Setting signal type to OTHER.")
                        End If
                    Case "M"
                        signal.SignalName = nameParts(0) & "." & nameParts(1) & ".MAG"
                        Dim channel = nameParts(1).Substring(nameParts(1).Length - 2).ToArray
                        If channel(0) = "I" OrElse channel(0) = "V" Then
                            signal.TypeAbbreviation = channel(0) & "M" & channel(1)
                        Else
                            signal.TypeAbbreviation = "OTHER"
                            _addLog("Signal name " & signal.SignalName & " does not comply naming convention. Setting signal type to OTHER.")
                        End If
                    Case Else
                        Throw New Exception("Error! Invalid signal name " & name & " found!")
                End Select
            ElseIf nameParts.Length = 2 Then
                If nameParts(1).Substring(0, 1) = "D" Then
                    signal.TypeAbbreviation = "D"
                    signal.SignalName = nameParts(0) & ".dig" & nameParts(1).Substring(1)
                Else
                    Dim lastLetter = nameParts(1).Last
                    Select Case lastLetter
                        Case "V"
                            signal.TypeAbbreviation = "Q"
                            signal.SignalName = name
                        Case "W"
                            signal.TypeAbbreviation = "P"
                            signal.SignalName = name
                            'Case "D"
                            '    signal.TypeAbbreviation = "D"
                            '    signal.SignalName = nameParts(0) & "dig"
                        Case Else
                            Throw New Exception("Error! Invalid signal name " & name & " found!")
                    End Select
                End If
            Else
                Throw New Exception("Error! Invalid signal name " & name & " found!")
            End If
            newSignalList.Add(signal)
        Next
        fileInfo.TaggedSignals = newSignalList
        fileInfo.GroupedSignalsByPMU = SortSignalByPMU(newSignalList)
        For Each group In fileInfo.GroupedSignalsByPMU
            If Not _allPMUs.Contains(group.SignalSignature.PMUName) Then
                _allPMUs.Add(group.SignalSignature.PMUName)
            End If
        Next
        Dim a = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
        a.SignalList = fileInfo.GroupedSignalsByPMU
        GroupedRawSignalsByPMU.Add(a)
        fileInfo.GroupedSignalsByType = SortSignalByType(newSignalList)
        Dim b = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
        b.SignalList = fileInfo.GroupedSignalsByType
        GroupedRawSignalsByType.Add(b)
    End Sub

    Private Function SortSignalByType(signalList As ObservableCollection(Of SignalSignatures)) As ObservableCollection(Of SignalTypeHierachy)
        Dim signalTypeTree As New ObservableCollection(Of SignalTypeHierachy)
        Dim signalTypeDictionary = signalList.GroupBy(Function(x) x.TypeAbbreviation.ToArray(0).ToString).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
        For Each signalType In signalTypeDictionary
            Select Case signalType.Key
                Case "S"
                    Dim groups = signalType.Value.GroupBy(Function(x) x.TypeAbbreviation)
                    For Each group In groups
                        Select Case group.Key
                            Case "S"
                                Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Apparent"))
                                newHierachy.SignalSignature.TypeAbbreviation = "S"
                                For Each signal In group
                                    newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                                Next
                                signalTypeTree.Add(newHierachy)
                            Case "SC"
                                Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Scalar"))
                                newHierachy.SignalSignature.TypeAbbreviation = "SC"
                                For Each signal In group
                                    newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                                Next
                                signalTypeTree.Add(newHierachy)
                            Case Else
                                _addLog("Unknown signal type: " & group.Key & "found!")
                        End Select
                    Next
                Case "O"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Other"))
                    newHierachy.SignalSignature.TypeAbbreviation = "OTHER"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "C"
                    'Dim groups = signalType.Value.GroupBy(Function(x) x.TypeAbbreviation).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
                    Dim groups = signalType.Value.GroupBy(Function(x) x.TypeAbbreviation)
                    For Each group In groups
                        Select Case group.Key
                            Case "C"
                                Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("CustomizedSignal"))
                                newHierachy.SignalSignature.TypeAbbreviation = "C"
                                For Each signal In group
                                    newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                                Next
                                signalTypeTree.Add(newHierachy)
                            Case "CP"
                                Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Complex"))
                                newHierachy.SignalSignature.TypeAbbreviation = "CP"
                                For Each signal In group
                                    newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                                Next
                                signalTypeTree.Add(newHierachy)
                            Case Else
                                _addLog("Unknown signal type: " & group.Key & "found!")
                        End Select
                    Next
                Case "D"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Digital"))
                    newHierachy.SignalSignature.TypeAbbreviation = "D"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "F"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Frequency"))
                    newHierachy.SignalSignature.TypeAbbreviation = "F"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "R"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Rate of Change of Frequency"))
                    newHierachy.SignalSignature.TypeAbbreviation = "R"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "Q"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Reactive Power"))
                    newHierachy.SignalSignature.TypeAbbreviation = "Q"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "P"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Active Power"))
                    newHierachy.SignalSignature.TypeAbbreviation = "P"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "V"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Voltage"))
                    newHierachy.SignalSignature.TypeAbbreviation = "V"
                    Dim voltageHierachy = signalType.Value.GroupBy(Function(y) y.TypeAbbreviation.ToArray(1).ToString)
                    For Each group In voltageHierachy
                        Select Case group.Key
                            Case "M"
                                Dim mGroup = New SignalTypeHierachy(New SignalSignatures("Magnitude"))
                                mGroup.SignalSignature.TypeAbbreviation = "VM"
                                Dim mGroupHierachky = group.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString)
                                For Each phase In mGroupHierachky
                                    Select Case phase.Key
                                        Case "P"
                                            Dim positiveGroup = New SignalTypeHierachy(New SignalSignatures("Positive Sequence"))
                                            positiveGroup.SignalSignature.TypeAbbreviation = "VMP"
                                            For Each signal In phase
                                                positiveGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(positiveGroup)
                                        Case "A"
                                            Dim AGroup = New SignalTypeHierachy(New SignalSignatures("Phase A"))
                                            AGroup.SignalSignature.TypeAbbreviation = "VMA"
                                            For Each signal In phase
                                                AGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(AGroup)
                                        Case "B"
                                            Dim BGroup = New SignalTypeHierachy(New SignalSignatures("Phase B"))
                                            BGroup.SignalSignature.TypeAbbreviation = "VMB"
                                            For Each signal In phase
                                                BGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(BGroup)
                                        Case "C"
                                            Dim CGroup = New SignalTypeHierachy(New SignalSignatures("Phase C"))
                                            CGroup.SignalSignature.TypeAbbreviation = "VMC"
                                            For Each signal In phase
                                                CGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(CGroup)
                                        Case Else
                                            Throw New Exception("Error! Invalid signal phase: " & phase.Key & " found in Voltage magnitude!")
                                    End Select
                                Next
                                newHierachy.SignalList.Add(mGroup)
                            Case "A"
                                Dim aGroup = New SignalTypeHierachy(New SignalSignatures("Angle"))
                                aGroup.SignalSignature.TypeAbbreviation = "VA"
                                Dim aGroupHierachky = group.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString)
                                For Each phase In aGroupHierachky
                                    Select Case phase.Key
                                        Case "P"
                                            Dim positiveGroup = New SignalTypeHierachy(New SignalSignatures("Positive Sequence"))
                                            positiveGroup.SignalSignature.TypeAbbreviation = "VAP"
                                            For Each signal In phase
                                                positiveGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(positiveGroup)
                                        Case "A"
                                            Dim GroupA = New SignalTypeHierachy(New SignalSignatures("Phase A"))
                                            GroupA.SignalSignature.TypeAbbreviation = "VAA"
                                            For Each signal In phase
                                                GroupA.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupA)
                                        Case "B"
                                            Dim GroupB = New SignalTypeHierachy(New SignalSignatures("Phase B"))
                                            GroupB.SignalSignature.TypeAbbreviation = "VAB"
                                            For Each signal In phase
                                                GroupB.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupB)
                                        Case "C"
                                            Dim GroupC = New SignalTypeHierachy(New SignalSignatures("Phase C"))
                                            GroupC.SignalSignature.TypeAbbreviation = "VAC"
                                            For Each signal In phase
                                                GroupC.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupC)
                                        Case Else
                                            Throw New Exception("Error! Invalid signal phase: " & phase.Key & " found in Voltage Angle!")
                                    End Select
                                Next
                                newHierachy.SignalList.Add(aGroup)
                            Case "P"
                                Dim aGroup = New SignalTypeHierachy(New SignalSignatures("Phasor"))
                                aGroup.SignalSignature.TypeAbbreviation = "VP"
                                Dim aGroupHierachky = group.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString)
                                For Each phase In aGroupHierachky
                                    Select Case phase.Key
                                        Case "P"
                                            Dim positiveGroup = New SignalTypeHierachy(New SignalSignatures("Positive Sequence"))
                                            positiveGroup.SignalSignature.TypeAbbreviation = "VPP"
                                            For Each signal In phase
                                                positiveGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(positiveGroup)
                                        Case "A"
                                            Dim GroupA = New SignalTypeHierachy(New SignalSignatures("Phase A"))
                                            GroupA.SignalSignature.TypeAbbreviation = "VPA"
                                            For Each signal In phase
                                                GroupA.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupA)
                                        Case "B"
                                            Dim GroupB = New SignalTypeHierachy(New SignalSignatures("Phase B"))
                                            GroupB.SignalSignature.TypeAbbreviation = "VPB"
                                            For Each signal In phase
                                                GroupB.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupB)
                                        Case "C"
                                            Dim GroupC = New SignalTypeHierachy(New SignalSignatures("Phase C"))
                                            GroupC.SignalSignature.TypeAbbreviation = "VPC"
                                            For Each signal In phase
                                                GroupC.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupC)
                                        Case Else
                                            Throw New Exception("Error! Invalid signal phase: " & phase.Key & " found in Voltage Angle!")
                                    End Select
                                Next
                                newHierachy.SignalList.Add(aGroup)
                            Case Else
                                Throw New Exception("Error! Invalid voltage signal type found: " & group.Key)
                        End Select
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "I"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Current"))
                    newHierachy.SignalSignature.TypeAbbreviation = "I"
                    Dim currentHierachy = signalType.Value.GroupBy(Function(y) y.TypeAbbreviation.ToArray(1).ToString)
                    For Each group In currentHierachy
                        Select Case group.Key
                            Case "M"
                                Dim mGroup = New SignalTypeHierachy(New SignalSignatures("Magnitude"))
                                mGroup.SignalSignature.TypeAbbreviation = "IM"
                                Dim mGroupHierachky = group.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString)
                                For Each phase In mGroupHierachky
                                    Select Case phase.Key
                                        Case "P"
                                            Dim positiveGroup = New SignalTypeHierachy(New SignalSignatures("Positive Sequence"))
                                            positiveGroup.SignalSignature.TypeAbbreviation = "IMP"
                                            For Each signal In phase
                                                positiveGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(positiveGroup)
                                        Case "A"
                                            Dim AGroup = New SignalTypeHierachy(New SignalSignatures("Phase A"))
                                            AGroup.SignalSignature.TypeAbbreviation = "IMA"
                                            For Each signal In phase
                                                AGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(AGroup)
                                        Case "B"
                                            Dim BGroup = New SignalTypeHierachy(New SignalSignatures("Phase B"))
                                            BGroup.SignalSignature.TypeAbbreviation = "IMB"
                                            For Each signal In phase
                                                BGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(BGroup)
                                        Case "C"
                                            Dim CGroup = New SignalTypeHierachy(New SignalSignatures("Phase C"))
                                            CGroup.SignalSignature.TypeAbbreviation = "IMC"
                                            For Each signal In phase
                                                CGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            mGroup.SignalList.Add(CGroup)
                                        Case Else
                                            Throw New Exception("Error! Invalid signal phase: " & phase.Key & " found in Voltage magnitude!")
                                    End Select
                                Next
                                newHierachy.SignalList.Add(mGroup)
                            Case "A"
                                Dim aGroup = New SignalTypeHierachy(New SignalSignatures("Angle"))
                                aGroup.SignalSignature.TypeAbbreviation = "IA"
                                Dim aGroupHierachky = group.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString)
                                For Each phase In aGroupHierachky
                                    Select Case phase.Key
                                        Case "P"
                                            Dim positiveGroup = New SignalTypeHierachy(New SignalSignatures("Positive Sequence"))
                                            positiveGroup.SignalSignature.TypeAbbreviation = "IAP"
                                            For Each signal In phase
                                                positiveGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(positiveGroup)
                                        Case "A"
                                            Dim GroupA = New SignalTypeHierachy(New SignalSignatures("Phase A"))
                                            GroupA.SignalSignature.TypeAbbreviation = "IAA"
                                            For Each signal In phase
                                                GroupA.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupA)
                                        Case "B"
                                            Dim GroupB = New SignalTypeHierachy(New SignalSignatures("Phase B"))
                                            GroupB.SignalSignature.TypeAbbreviation = "IAB"
                                            For Each signal In phase
                                                GroupB.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupB)
                                        Case "C"
                                            Dim GroupC = New SignalTypeHierachy(New SignalSignatures("Phase C"))
                                            GroupC.SignalSignature.TypeAbbreviation = "IAC"
                                            For Each signal In phase
                                                GroupC.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupC)
                                        Case Else
                                            Throw New Exception("Error! Invalid signal phase: " & phase.Key & " found in Voltage Angle!")
                                    End Select
                                Next
                                newHierachy.SignalList.Add(aGroup)
                            Case "P"
                                Dim aGroup = New SignalTypeHierachy(New SignalSignatures("Phasor"))
                                aGroup.SignalSignature.TypeAbbreviation = "IP"
                                Dim aGroupHierachky = group.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString)
                                For Each phase In aGroupHierachky
                                    Select Case phase.Key
                                        Case "P"
                                            Dim positiveGroup = New SignalTypeHierachy(New SignalSignatures("Positive Sequence"))
                                            positiveGroup.SignalSignature.TypeAbbreviation = "IPP"
                                            For Each signal In phase
                                                positiveGroup.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(positiveGroup)
                                        Case "A"
                                            Dim GroupA = New SignalTypeHierachy(New SignalSignatures("Phase A"))
                                            GroupA.SignalSignature.TypeAbbreviation = "IPA"
                                            For Each signal In phase
                                                GroupA.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupA)
                                        Case "B"
                                            Dim GroupB = New SignalTypeHierachy(New SignalSignatures("Phase B"))
                                            GroupB.SignalSignature.TypeAbbreviation = "IPB"
                                            For Each signal In phase
                                                GroupB.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupB)
                                        Case "C"
                                            Dim GroupC = New SignalTypeHierachy(New SignalSignatures("Phase C"))
                                            GroupC.SignalSignature.TypeAbbreviation = "IPC"
                                            For Each signal In phase
                                                GroupC.SignalList.Add(New SignalTypeHierachy(signal))
                                            Next
                                            aGroup.SignalList.Add(GroupC)
                                        Case Else
                                            Throw New Exception("Error! Invalid signal phase: " & phase.Key & " found in Voltage Angle!")
                                    End Select
                                Next
                                newHierachy.SignalList.Add(aGroup)
                            Case Else
                                Throw New Exception("Error! Invalid voltage signal type found: " & group.Key)
                        End Select
                    Next
                    signalTypeTree.Add(newHierachy)
                Case Else
                    Throw New Exception("Error! Invalid signal type found: " & signalType.Key)
            End Select
        Next
        Return signalTypeTree
    End Function

    Private Function SortSignalByPMU(signalList As ObservableCollection(Of SignalSignatures)) As ObservableCollection(Of SignalTypeHierachy)
        Dim PMUSignalDictionary = signalList.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
        Dim pmuSignalTree = New ObservableCollection(Of SignalTypeHierachy)
        For Each group In PMUSignalDictionary
            Dim newPMUSignature = New SignalSignatures(group.Key, group.Key)
            Dim newGroup = New SignalTypeHierachy(newPMUSignature)
            For Each signal In group.Value
                newGroup.SignalList.Add(New SignalTypeHierachy(signal))
            Next
            pmuSignalTree.Add(newGroup)
        Next
        Return pmuSignalTree
    End Function

    Private _lastInputFolderLocation As String
    Private _browseInputFileDir As ICommand
    Public Property BrowseInputFileDir As ICommand
        Get
            Return _browseInputFileDir
        End Get
        Set(ByVal value As ICommand)
            _browseInputFileDir = value
        End Set
    End Property

    Private Sub _browseInputFileFolder(obj As InputFileInfo)
        'Dim previousDir = New InputFileInfo(obj)
        Dim openDirectoryDialog As New FolderBrowserDialog()
        openDirectoryDialog.Description = "Select the directory that data files (.pdat or .csv) are located "
        If _lastInputFolderLocation Is Nothing Then
            openDirectoryDialog.SelectedPath = Environment.CurrentDirectory
        Else
            openDirectoryDialog.SelectedPath = _lastInputFolderLocation
        End If
        openDirectoryDialog.ShowNewFolderButton = False
        If (openDirectoryDialog.ShowDialog = DialogResult.OK) Then
            ' When a new directory is selected, we need to clean out everything that display contents of that directory
            obj.Mnemonic = ""
            obj.SamplingRate = ""
            obj.GroupedSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
            obj.GroupedSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
            ' clean out signals from that directory from all references since we display those signals in 4 different ways
            For Each group In GroupedRawSignalsByType
                If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                    GroupedRawSignalsByType.Remove(group)
                    Exit For
                End If
            Next
            For Each group In GroupedRawSignalsByPMU
                If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                    GroupedRawSignalsByPMU.Remove(group)
                    Exit For
                End If
            Next
            obj.TaggedSignals = New ObservableCollection(Of SignalSignatures)
            'For Each signal In obj.TaggedSignals
            '    signal.Dispose()
            'Next
            obj.SignalList = New List(Of String)
            'obj.GroupedSignalsByPMU
            _lastInputFolderLocation = openDirectoryDialog.SelectedPath
            obj.FileDirectory = _lastInputFolderLocation
            _buildInputFileFolderTree(obj)
            If _configData IsNot Nothing Then
                _readDataConfigStages()
            End If
        End If
    End Sub
    Private Sub _buildInputFileFolderTree(fileInfo As InputFileInfo)
        Dim _sampleFile = ""
        Try
            fileInfo.InputFileTree = New ObservableCollection(Of Folder)
            fileInfo.InputFileTree.Add(New Folder(fileInfo.FileDirectory, fileInfo.FileType.ToString, _sampleFile))
        Catch ex As Exception
            _addLog("Error reading input data directory! " & ex.Message)
        End Try
        If String.IsNullOrEmpty(_sampleFile) Then
            'MessageBox.Show("No file of type: " & fileInfo.FileType.ToString & vbCrLf & " is found in: " & fileInfo.FileDirectory, "Error!", MessageBoxButtons.OK)
            _addLog("No file of type: " & fileInfo.FileType.ToString & " is found in: " & fileInfo.FileDirectory)
        Else
            Try
                _readFirstDataFile(_sampleFile, fileInfo)
                If fileInfo.FileType.ToString = "pdat" Then
                    _tagSignals(fileInfo, fileInfo.SignalList)
                End If
            Catch ex As Exception
                'MessageBox.Show("Error sampling input data file!" & Environment.NewLine & ex.Message, "Error!", MessageBoxButtons.OK)
                _addLog("Error sampling input data file! " & ex.Message)
            End Try
        End If
    End Sub
    Private Sub _readFirstDataFile(sampleFile As String, fileInfo As InputFileInfo)
        If System.IO.Path.GetExtension(sampleFile).Substring(1) = "csv" Then
            'Dim CSVSampleFile As New JSIS_CSV_Reader.JSISCSV_Reader
            'Dim signals = CSVSampleFile.OpenCSV4row(_sampleFile)
            Dim fr As FileIO.TextFieldParser = New FileIO.TextFieldParser(sampleFile)
            fr.TextFieldType = FileIO.FieldType.Delimited
            fr.Delimiters = New String() {","}
            fr.HasFieldsEnclosedInQuotes = True
            'fileInfo.SamplingRate = System.IO.File.ReadAllLines(sampleFile).Length
            Dim pmuName = sampleFile.Split("\").Last.Split("_")(0)
            'Dim pmuName = sampleFile.Split("\").Last.Split(".")(0)
            If Not _allPMUs.Contains(pmuName) Then
                _allPMUs.Add(pmuName)
            End If
            Dim signalNames = fr.ReadFields.Skip(1).ToList
            Dim signalTypes = fr.ReadFields.Skip(1).ToList
            Dim signalUnits = fr.ReadFields.Skip(1).ToList
            fr.ReadLine()
            fr.ReadLine()
            Dim time1 = fr.ReadFields(0)
            Dim time2 = fr.ReadFields(0)
            Try
                Dim t1 = Convert.ToDouble(time1)
                Dim t2 = Convert.ToDouble(time2)
                fileInfo.SamplingRate = Math.Round((1 / (t2 - t1)) / 10) * 10.ToString
            Catch ex As Exception
                Dim t1 = DateTime.Parse(time1)
                Dim t2 = DateTime.Parse(time2)
                Dim dif = t2.Subtract(t1).TotalSeconds
                fileInfo.SamplingRate = Math.Round((1 / dif) / 10) * 10.ToString
            End Try
            Dim type = ""
            Dim signalName = ""
            Dim signalList = New List(Of String)
            Dim signalSignatureList = New ObservableCollection(Of SignalSignatures)
            For index = 0 To signalNames.Count - 1
                Dim newSignal = New SignalSignatures
                newSignal.PMUName = pmuName
                newSignal.Unit = signalUnits(index)
                newSignal.SignalName = signalNames(index)
                newSignal.SamplingRate = fileInfo.SamplingRate
                signalList.Add(signalNames(index))
                Select Case signalTypes(index)
                    Case "VPM"
                        'signalName = signalNames(index).Split(".")(0) & ".VMP"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "VMP"
                    Case "VPA"
                        'signalName = signalNames(index).Split(".")(0) & ".VAP"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "VAP"
                    Case "IPM"
                        'signalName = signalNames(index).Split(".")(0) & ".IMP"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "IMP"
                    Case "IPA"
                        'signalName = signalNames(index).Split(".")(0) & ".IAP"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "IAP"
                    Case "F"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "F"
                    Case "P"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "P"
                    Case "Q"
                        'signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "Q"
                    Case Else
                        Throw New Exception("Error! Invalid signal type " & signalTypes(index) & " found in file: " & sampleFile & " !")
                End Select
                signalSignatureList.Add(newSignal)
            Next
            fileInfo.SignalList = signalList
            fileInfo.TaggedSignals = signalSignatureList
            fileInfo.GroupedSignalsByPMU = SortSignalByPMU(signalSignatureList)
            Dim a = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
            a.SignalList = fileInfo.GroupedSignalsByPMU
            GroupedRawSignalsByPMU.Add(a)
            fileInfo.GroupedSignalsByType = SortSignalByType(signalSignatureList)
            Dim b = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
            b.SignalList = fileInfo.GroupedSignalsByType
            GroupedRawSignalsByType.Add(b)
        Else
            Dim PDATSampleFile As New PDATReader
            fileInfo.SignalList = PDATSampleFile.GetPDATSignalNameList(sampleFile)
            fileInfo.SamplingRate = PDATSampleFile.GetSamplingRate()
        End If
    End Sub

    Private _fileTypeChanged As ICommand
    Public Property FileTypeChanged As ICommand
        Get
            Return _fileTypeChanged
        End Get
        Set(ByVal value As ICommand)
            _fileTypeChanged = value
        End Set
    End Property

    Private _configFileName As String
    Public Property ConfigFileName As String
        Get
            Return _configFileName
        End Get
        Set(ByVal value As String)
            _configFileName = value
            OnPropertyChanged("ConfigFileName")
        End Set
    End Property

    Private _configData As XDocument

    Private _dataConfigure As DataConfig
    Public Property DataConfigure As DataConfig
        Get
            Return _dataConfigure
        End Get
        Set(ByVal value As DataConfig)
            _dataConfigure = value
            OnPropertyChanged()
        End Set
    End Property

    Private _processConfigure As ProcessConfig
    Public Property ProcessConfigure As ProcessConfig
        Get
            Return _processConfigure
        End Get
        Set(ByVal value As ProcessConfig)
            _processConfigure = value
            OnPropertyChanged()
        End Set
    End Property

    Private _postProcessConfigure As PostProcessCustomizationConfig
    Public Property PostProcessConfigure As PostProcessCustomizationConfig
        Get
            Return _postProcessConfigure
        End Get
        Set(ByVal value As PostProcessCustomizationConfig)
            _postProcessConfigure = value
            OnPropertyChanged()
        End Set
    End Property

    Private _detectorConfigure As DetectorConfig
    Public Property DetectorConfigure As DetectorConfig
        Get
            Return _detectorConfigure
        End Get
        Set(ByVal value As DetectorConfig)
            _detectorConfigure = value
            OnPropertyChanged()
        End Set
    End Property

#Region "Write XML Configure file"
    Private _saveConfigFile As ICommand
    Public Property SaveConfigFile() As ICommand
        Get
            Return _saveConfigFile
        End Get
        Set(ByVal value As ICommand)
            _saveConfigFile = value
        End Set
    End Property

    Private Sub _saveConfigureFile()
        If ConfigFileName Is Nothing Then
            _saveConfigureFileAs()
        Else
            Dim userchoice As Integer = MessageBox.Show("Do you want to over write current configure xml file?", "Warning!", MessageBoxButtons.YesNo)
            If userchoice = DialogResult.Yes Then
                If System.IO.File.Exists(ConfigFileName) Then
                    _writeXmlConfigFile(ConfigFileName)
                Else
                    MessageBox.Show("Specified file path " & ConfigFileName & " does not exist!", "Error!", MessageBoxButtons.OK)
                    _saveConfigureFileAs()
                End If
            Else
                _saveConfigureFileAs()
            End If
        End If
    End Sub

    Private _saveConfigFileAs As ICommand
    Public Property SaveConfigFileAs As ICommand
        Get
            Return _saveConfigFileAs
        End Get
        Set(ByVal value As ICommand)
            _saveConfigFileAs = value
        End Set
    End Property

    Private Sub _saveConfigureFileAs()
        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
        saveFileDialog.Title = "Save XML Config File"
        If ConfigFileName IsNot Nothing Then
            saveFileDialog.FileName = System.IO.Path.GetFileName(ConfigFileName)
        End If
        Dim userChoice As Integer = saveFileDialog.ShowDialog()
        If userChoice = DialogResult.OK Then
            Dim fileNameToSave = saveFileDialog.FileName
            'If System.IO.File.Exists(fileNameToSave) Then
            '    Dim userchoice2 As Integer = MessageBox.Show("Do you want to over write current configure xml file?", "Warning!", MessageBoxButtons.YesNo)
            '    If userchoice2 = DialogResult.Yes Then
            '        MessageBox.Show("Save file: " & fileNameToSave)
            '        _writeXmlConfigFile(fileNameToSave)
            '    Else
            '        _saveConfigureFileAs()
            '    End If
            'Else
            'MessageBox.Show("Save file: " & fileNameToSave)
            _writeXmlConfigFile(fileNameToSave)
            'End If
        End If
    End Sub

    Private Sub _writeXmlConfigFile(filename As String)
        Dim _configData As XElement = <Config></Config>
        Dim dataConfig As XElement = <DataConfig>
                                         <Configuration>
                                             <ReaderProperties></ReaderProperties>
                                         </Configuration>
                                     </DataConfig>
        Dim fileInfoCount = 0
        For Each fileInfo In DataConfigure.ReaderProperty.InputFileInfos
            If fileInfoCount = 0 Then
                Dim reader = (From c In dataConfig.<Configuration>.Elements Where c.Name = "ReaderProperties").FirstOrDefault
                reader.Add(<FileDirectory><%= fileInfo.FileDirectory %></FileDirectory>)
                reader.Add(<FileType><%= fileInfo.FileType %></FileType>)
                reader.Add(<Mnemonic><%= fileInfo.Mnemonic %></Mnemonic>)
            Else
                Dim info As XElement = <AdditionalFilePath>
                                           <FileDirectory><%= fileInfo.FileDirectory %></FileDirectory>
                                           <FileType><%= fileInfo.FileType %></FileType>
                                           <Mnemonic><%= fileInfo.Mnemonic %></Mnemonic>
                                       </AdditionalFilePath>
                dataConfig.<Configuration>.<ReaderProperties>.LastOrDefault.Add(info)
            End If
            fileInfoCount += 1
        Next
        Dim mode As XElement = <Mode>
                                   <Name><%= DataConfigure.ReaderProperty.ModeName %></Name>
                               </Mode>
        Select Case DataConfigure.ReaderProperty.ModeName
            Case ModeType.Archive
                Dim dtStart = DateTime.Parse(DataConfigure.ReaderProperty.DateTimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
                Dim dtStringStart = dtStart.ToString("yyyy-MM-dd HH:mm:ss") & " GMT"
                Dim dtEnd = DateTime.Parse(DataConfigure.ReaderProperty.DateTimeEnd, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
                Dim dtStringEnd = dtEnd.ToString("yyyy-MM-dd HH:mm:ss") & " GMT"
                Dim parameters As XElement = <Params>
                                                 <DateTimeStart><%= dtStringStart %></DateTimeStart>
                                                 <DateTimeEnd><%= dtStringEnd %></DateTimeEnd>
                                             </Params>
                mode.Add(parameters)
            Case ModeType.Hybrid
                Dim dtStart = DateTime.Parse(DataConfigure.ReaderProperty.DateTimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal)
                Dim dtStringStart = dtStart.ToString("yyyy-MM-dd HH:mm:ss") & " GMT"
                Dim parameters As XElement = <Params>
                                                 <DateTimeStart><%= dtStringStart %></DateTimeStart>
                                                 <NoFutureWait><%= DataConfigure.ReaderProperty.NoFutureWait %></NoFutureWait>
                                                 <MaxNoFutureCount><%= DataConfigure.ReaderProperty.MaxNoFutureCount %></MaxNoFutureCount>
                                                 <FutureWait><%= DataConfigure.ReaderProperty.FutureWait %></FutureWait>
                                                 <MaxFutureCount><%= DataConfigure.ReaderProperty.MaxFutureCount %></MaxFutureCount>
                                                 <RealTimeRange><%= DataConfigure.ReaderProperty.RealTimeRange %></RealTimeRange>
                                             </Params>
                mode.Add(parameters)
            Case ModeType.RealTime
                Dim parameters As XElement = <Params>
                                                 <NoFutureWait><%= DataConfigure.ReaderProperty.NoFutureWait %></NoFutureWait>
                                                 <MaxNoFutureCount><%= DataConfigure.ReaderProperty.MaxNoFutureCount %></MaxNoFutureCount>
                                                 <FutureWait><%= DataConfigure.ReaderProperty.FutureWait %></FutureWait>
                                                 <MaxFutureCount><%= DataConfigure.ReaderProperty.MaxFutureCount %></MaxFutureCount>
                                             </Params>
                mode.Add(parameters)
        End Select
        dataConfig.<Configuration>.<ReaderProperties>.LastOrDefault.Add(mode)
        Dim aStep As XElement
        Dim stage As XElement = <Stages></Stages>
        Dim newStageFlag = True
        For Each singleStep In DataConfigure.CollectionOfSteps
            If TypeOf singleStep Is Customization AndAlso newStageFlag Then
                newStageFlag = False
            ElseIf Not newStageFlag AndAlso TypeOf singleStep Is DQFilter Then
                dataConfig.<Configuration>.LastOrDefault.Add(stage)
                stage = <Stages></Stages>
                newStageFlag = True
            End If
            _writeAStep(stage, aStep, singleStep)
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
        If String.IsNullOrEmpty(ProcessConfigure.InitializationPath) Then
            processConfig.<Configuration>.FirstOrDefault.Add(<InitializationPath><%= ProcessConfigure.InitializationPath %>></InitializationPath>)
        Else
            processConfig.<Configuration>.FirstOrDefault.Add(<InitializationPath><%= CurDir() %>></InitializationPath>)
        End If
        Dim processing As XElement = <Processing></Processing>
        For Each unWrap In ProcessConfigure.UnWrapList
            aStep = <Unwrap>
                        <MaxNaN><%= unWrap.MaxNaN %></MaxNaN>
                    </Unwrap>
            Dim PMUSignalDictionary = unWrap.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
            _writePMUElements(aStep, PMUSignalDictionary)
            processing.Add(aStep)
        Next
        For Each itrplt In ProcessConfigure.InterpolateList
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
        For Each stp In ProcessConfigure.CollectionOfSteps
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
                            <Parameters></Parameters>
                        </Filter>
                For Each parameter In stp.FilterParameters
                    Dim para As XElement = New XElement(parameter.ParameterName.ToString, parameter.Value)
                    aStep.<Parameters>.LastOrDefault.Add(para)
                Next
                Dim PMUSignalDictionary = DirectCast(stp, TunableFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                _writePMUElements(aStep, PMUSignalDictionary)
            ElseIf TypeOf stp Is Multirate Then
                aStep = <Multirate>
                            <Parameters>
                                <MultiRatePMU><%= stp.MultiRatePMU %></MultiRatePMU>
                            </Parameters>
                        </Multirate>
                If stp.FilterParameters.Count = 2 Then
                    Dim newR = <NewRate><%= stp.NewRate %></NewRate>
                    aStep.<Parameters>.LastOrDefault.Add(newR)
                ElseIf stp.FilterParameters.Count = 3 Then
                    Dim p = <p><%= stp.PElement %></p>
                    Dim q = <q><%= stp.QElement %></q>
                    aStep.<Parameters>.LastOrDefault.Add(p)
                    aStep.<Parameters>.LastOrDefault.Add(q)
                End If
                Dim PMUSignalDictionary = DirectCast(stp, Multirate).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                _writePMUElements(aStep, PMUSignalDictionary)
            End If
            stage.Add(aStep)
        Next
        processing.Add(stage)
        For Each wrp In ProcessConfigure.WrapList
            aStep = <Wrap></Wrap>
            Dim PMUSignalDictionary = wrp.InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
            _writePMUElements(aStep, PMUSignalDictionary)
            processing.Add(aStep)
        Next
        processConfig.<Configuration>.LastOrDefault.Add(processing)
        Dim nameTypeUnit As XElement = <NameTypeUnit></NameTypeUnit>
        If ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList.Count > 0 Then

            For Each pmus In ProcessConfigure.NameTypeUnitElement.NameTypeUnitPMUList
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
            Dim unit = ProcessConfigure.NameTypeUnitElement.NewUnit
            If Not String.IsNullOrEmpty(unit) Then
                nameTypeUnit.Add(<NewUnit><%= unit %></NewUnit>)
            End If
            Dim type = ProcessConfigure.NameTypeUnitElement.NewType
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
        stage = <Stages></Stages>
        For Each singleStep In PostProcessConfigure.CollectionOfSteps
            _writeAStep(stage, aStep, singleStep)
            stage.Add(aStep)
        Next
        postProcessConfig.<Configuration>.LastOrDefault.Add(stage)
        'signalSelection = <SignalSelection></SignalSelection>
        'postProcessConfig.<Configuration>.LastOrDefault.Add(signalSelection)
        _configData.Add(postProcessConfig)
        Dim detectorConfig As XElement = <DetectorConfig><Configuration></Configuration></DetectorConfig>
        _configData.Add(detectorConfig)
        _configData.Save(filename)
    End Sub

    Private Sub _writeAStep(ByRef stage As XElement, ByRef aStep As XElement, ByRef singleStep As Object)
        Select Case singleStep.Name
            Case "Scalar Repetition Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                            <Parameters>
                                <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                <scalar><%= singleStep.Scalar %></scalar>
                                <SignalType><%= singleStep.OutputChannels(0).TypeAbbreviation %></SignalType>
                                <SignalUnit><%= singleStep.OutputChannels(0).Unit %></SignalUnit>
                            </Parameters>
                        </Customization>
                If Not String.IsNullOrEmpty(singleStep.TimeSourcePMU) Then
                    aStep.<Parameters>.LastOrDefault.Add(<TimeSourcePMU><%= singleStep.TimeSourcePMU %></TimeSourcePMU>)
                End If
            Case "Addition Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case "Subtraction Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                            <Parameters>
                                <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                <minuend>
                                    <PMU><%= singleStep.MinuendOrDividend.PMUName %></PMU>
                                    <Channel><%= singleStep.MinuendOrDividend.SignalName %></Channel>
                                </minuend>
                                <subtrahend>
                                    <PMU><%= singleStep.SubtrahendOrDivisor.PMUName %></PMU>
                                    <Channel><%= singleStep.SubtrahendOrDivisor.SignalName %></Channel>
                                </subtrahend>
                            </Parameters>
                        </Customization>
            Case "Multiplication Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case "Division Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                            <Parameters>
                                <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                <SignalName><%= singleStep.OutputChannels(0).SignalName %></SignalName>
                                <dividend>
                                    <PMU><%= singleStep.MinuendOrDividend.PMUName %></PMU>
                                    <Channel><%= singleStep.MinuendOrDividend.SignalName %></Channel>
                                </dividend>
                                <divisor>
                                    <PMU><%= singleStep.SubtrahendOrDivisor.PMUName %></PMU>
                                    <Channel><%= singleStep.SubtrahendOrDivisor.SignalName %></Channel>
                                </divisor>
                            </Parameters>
                        </Customization>
            Case "Raise signals to an exponent"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Take complex conjugate of signals", "Return angle of complex valued signals"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case "Phasor Creation Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case "Power Calculation Customization"
                Dim powerDict = _powerTypeDictionary.ToDictionary(Function(x) x.Value, Function(x) x.Key)
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case "Specify Signal Type and Unit Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                            <Parameters>
                                <SigType><%= singleStep.OutputChannels(0).TypeAbbreviation %></SigType>
                                <SigUnit><%= singleStep.OutputChannels(0).Unit %></SigUnit>
                                <PMU><%= singleStep.InputChannels(0).PMUName %></PMU>
                                <Channel><%= singleStep.InputChannels(0).SignalName %></Channel>
                                <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                                <CustName><%= singleStep.OutputChannels(0).SignalName %></CustName>
                            </Parameters>
                        </Customization>
            Case "Metric Prefix Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
                            <Parameters>
                                <CustPMUname><%= singleStep.CustPMUname %></CustPMUname>
                            </Parameters>
                        </Customization>
                For Each pair In singleStep.OutputInputMappingPair
                    Dim toConvert As XElement = <ToConvert>
                                                    <PMU><%= pair.Value(0).PMUName %></PMU>
                                                    <Channel><%= pair.Value(0).SignalName %></Channel>
                                                    <NewUnit><%= pair.Key.Unit %></NewUnit>
                                                    <CustName><%= pair.Key.SignalName %></CustName>
                                                </ToConvert>
                    aStep.<Parameters>.LastOrDefault.Add(toConvert)
                Next
            Case "Angle Conversion Customization"
                aStep = <Customization>
                            <Name><%= DataConfigure.CustomizationNameDictionary(singleStep.Name) %></Name>
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
            Case Else
                aStep = <Filter>
                            <Name><%= DataConfigure.DQFilterNameDictionary(singleStep.Name) %></Name>
                            <Parameters></Parameters>
                        </Filter>
                For Each parameter In singleStep.FilterParameters
                    'Dim a = {parameter}ParameterName
                    'Dim para As XElement = <<%= parameter.ParameterName.ToString %>><%= parameter.Value %></>
                    Dim para As XElement = New XElement(parameter.ParameterName.ToString, parameter.Value)
                    aStep.<Parameters>.LastOrDefault.Add(para)
                Next
                If singleStep.Name = "PMU Status Flags Data Quality Filter" Then

                    For Each signal In singleStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList
                        Dim PMU As XElement = <PMU>
                                                  <Name><%= signal.SignalSignature.PMUName %></Name>
                                              </PMU>
                        aStep.Add(PMU)
                    Next
                Else
                    Dim PMUSignalDictionary = DirectCast(singleStep, DQFilter).InputChannels.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
                    _writePMUElements(aStep, PMUSignalDictionary)
                End If
        End Select
    End Sub

    Private Sub _writePMUElements(aStep As XElement, pMUSignalDictionary As Dictionary(Of String, List(Of SignalSignatures)))
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


#End Region

    Private _openConfigFile As ICommand
    Public Property OpenConfigFile As ICommand
        Get
            Return _openConfigFile
        End Get
        Set(ByVal value As ICommand)
            _openConfigFile = value
        End Set
    End Property

    Private Sub openConfigXMLFile()
        Dim openFileDialog As New Microsoft.Win32.OpenFileDialog()
        openFileDialog.RestoreDirectory = True
        openFileDialog.FileName = ""
        openFileDialog.DefaultExt = ".xml"
        openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
        openFileDialog.InitialDirectory = CurDir() + "\ConfigFiles"

        Dim DialogResult? As Boolean = openFileDialog.ShowDialog
        If DialogResult Then
            ConfigFileName = openFileDialog.FileName
            _addLog("Open file: " & ConfigFileName & " successfully!")
            GroupedRawSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
            GroupedRawSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
            NameTypeUnitStatusFlag = 0
            Try
                _configData = XDocument.Load(_configFileName)
                _readConfigFile()
            Catch ex As Exception
                _addLog("Error reading config file!" & vbCrLf & ex.Message)
                MessageBox.Show("Error reading config file!" & vbCrLf & ex.Message & vbCrLf & "Please see logs below!", "Error!", MessageBoxButtons.OK)
            End Try
        End If
    End Sub
    Private Sub _readConfigFile()
        _addLog("Reading " & ConfigFileName)

        '''''''''''''''''''''' Read DataConfig''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim fileInfo = New InputFileInfo
        Dim fileInfoList = New ObservableCollection(Of InputFileInfo)
        fileInfo.FileDirectory = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<FileDirectory>.Value
        Dim fileType = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<FileType>.Value
        If fileType IsNot Nothing Then
            fileInfo.FileType = [Enum].Parse(GetType(DataFileType), fileType.ToLower())
        End If
        fileInfo.Mnemonic = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mnemonic>.Value
        fileInfoList.Add(fileInfo)
        _buildInputFileFolderTree(fileInfo)

        Dim inputInformation = From el In _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.Elements Where el.Name = "AdditionalFilePath" Select el
        For Each el In inputInformation
            Dim info = New InputFileInfo
            info.FileDirectory = el.<FileDirectory>.Value
            Dim type = el.<FileType>.Value.ToLower
            info.FileType = [Enum].Parse(GetType(DataFileType), type)
            info.Mnemonic = el.<Mnemonic>.Value
            fileInfoList.Add(info)
            _buildInputFileFolderTree(info)
        Next

        DataConfigure.ReaderProperty.InputFileInfos = fileInfoList

        DataConfigure.ReaderProperty.ModeName = [Enum].Parse(GetType(ModeType), _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Name>.Value)
        Select Case DataConfigure.ReaderProperty.ModeName
            Case ModeType.Archive
                DataConfigure.ReaderProperty.DateTimeStart = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeStart>.Value
                DataConfigure.ReaderProperty.DateTimeEnd = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeEnd>.Value
            Case ModeType.Hybrid
                DataConfigure.ReaderProperty.DateTimeStart = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<DateTimeStart>.Value
                DataConfigure.ReaderProperty.NoFutureWait = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<NoFutureWait>.Value
                DataConfigure.ReaderProperty.MaxNoFutureCount = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxNoFutureCount>.Value
                DataConfigure.ReaderProperty.FutureWait = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<FutureWait>.Value
                DataConfigure.ReaderProperty.MaxFutureCount = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxFutureCount>.Value
                DataConfigure.ReaderProperty.RealTimeRange = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<RealTimeRange>.Value
            Case ModeType.RealTime
                DataConfigure.ReaderProperty.NoFutureWait = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<NoFutureWait>.Value
                DataConfigure.ReaderProperty.MaxNoFutureCount = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxNoFutureCount>.Value
                DataConfigure.ReaderProperty.FutureWait = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<FutureWait>.Value
                DataConfigure.ReaderProperty.MaxFutureCount = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.<MaxFutureCount>.Value
            Case Else
                Throw New Exception("Error: invalid mode type found in config file.")
        End Select
        _readDataConfigStages()
        _groupAllDataConfigOutputSignal()
        '''''''''''''''''''''''''''''''''Read ProcessConfig''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        _readProcessConfig()
        _readPostProcessConfig()
        _addLog("Done reading " & ConfigFileName & " .")
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
                            _addLog("Error reading config file! Signal in a step of processing with channel name: " & signalName & " in PMU " & pmuName & " not found!")
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
            'For Each group In GroupedSignalsByType
            '    For Each subgroup In group.SignalList
            '        If subgroup.SignalSignature.TypeAbbreviation = "V" OrElse subgroup.SignalSignature.TypeAbbreviation = "I"
            '            For Each subsubgroup In subgroup.SignalList
            '                 if subsubgroup.SignalSignature.TypeAbbreviation.Substring(1,1) = "A"
            '                     For Each phase In subsubgroup.SignalList
            '                         For Each signal In phase.SignalList
            '                             aUnwrap.InputChannels.Add(signal.SignalSignature)
            '                         Next
            '                     Next
            '                 End If       
            '            Next
            '        End If
            '    Next
            'Next
            _addLog("Warning! No PMU specified in the step, no channel or no PMU is included in this step.")
        End If
        Return inputSignalList
    End Function

#Region "Read Data Config Customization Steps From XML Configure File"
    Private Sub _readDataConfigStages()
        Dim CollectionOfSteps As New ObservableCollection(Of Object)
        GroupedSignalByDataConfigStepsInput = New ObservableCollection(Of SignalTypeHierachy)
        GroupedSignalByDataConfigStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
        Dim stepCounter As Integer = 0
        Dim stages = From el In _configData.<Config>.<DataConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
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
                        aStep.InputChannels = _readPMUElements(stp)
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
            If String.IsNullOrEmpty(custSignalName) Then
                If input.IsValid Then
                    custSignalName = input.SignalName
                Else
                    custSignalName = "NoCustomSignalNameSpecified"
                End If
            End If
            Dim output = New SignalSignatures(custSignalName, aStep.CustPMUname, "OTHER")
            output.IsCustomSignal = True
            If input.IsValid Then
                output.TypeAbbreviation = input.TypeAbbreviation
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
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, "OTHER")
        If minuend.IsValid AndAlso subtrahend.IsValid Then
            If minuend.TypeAbbreviation = subtrahend.TypeAbbreviation Then
                output.TypeAbbreviation = minuend.TypeAbbreviation
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
            If String.IsNullOrEmpty(custSignalName) Then
                If input.IsValid Then
                    custSignalName = input.SignalName
                Else
                    custSignalName = "NoCustomSignalNameSpecified"
                End If
            End If
            Dim output = New SignalSignatures(custSignalName, aStep.CustPMUname, "OTHER")
            output.IsCustomSignal = True
            If input.IsValid And input.TypeAbbreviation = "SC" Then
                output.TypeAbbreviation = "SC"
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
            ElseIf Divisor.TypeAbbreviation = "SC" Then
                output.TypeAbbreviation = Dividend.TypeAbbreviation
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
    'Private Overloads Sub _readScalarRepetitionCustomization(aStep As Object, params As IEnumerable(Of XElement), collectionOfSteps As ObservableCollection(Of Customization), ByRef stepCounter As Integer)
    'End Sub
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
        aStep.TimeSourcePMU = params.<TimeSourcePMU>.Value

        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, type)
        output.IsCustomSignal = True
        output.Unit = unit
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
        ElseIf countNonScalarType = 1 Then
            output.TypeAbbreviation = type
        Else
            output.TypeAbbreviation = "OTHER"
        End If
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
            Else
                _addLog("Error reading config file! Input signal in step: " & stepCounter & "with channel name: " & term.<Channel>.Value & ", and in PMU " & term.<PMU>.Value & " not found!")
            End If
        Next
        If Not String.IsNullOrEmpty(type) Then
            output.TypeAbbreviation = type
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
            aStep.OutputChannels.Add(output)
            Dim signals = From el In powers(index).Elements Where el.Name <> "CustName"
            For Each signal In signals
                Dim input = _searchForSignalInTaggedSignals(signal.<PMU>.Value, signal.<Channel>.Value)
                If input Is Nothing Then
                    input = New SignalSignatures("SignalNotFound")
                    input.IsValid = False
                    input.TypeAbbreviation = "C"
                    _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
                End If
                aStep.InputChannels.Add(input)
            Next
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
        If inputSignal Is Nothing Then
            inputSignal = New SignalSignatures("SignalNotFound")
            inputSignal.IsValid = False
            inputSignal.TypeAbbreviation = "C"
            _addLog("Error reading config file! Input signal in step: " & stepCounter & " with PMU: " & params.<PMU>.Value & ", and Channel: " & params.<Channel>.Value & " not found!")
        End If
        aStep.InputChannels.Add(inputSignal)
        Dim outputName = params.<CustName>.Value
        If outputName Is Nothing Then
            outputName = inputSignal.SignalName
        End If
        Dim output = New SignalSignatures(outputName, aStep.CustPMUname, params.<SigType>.Value)
        output.IsCustomSignal = True
        output.Unit = params.<SigUnit>.Value
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
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.PMUName = pmu Then
                    For Each subsubgroup In subgroup.SignalList
                        If subsubgroup.SignalSignature.SignalName = channel Then
                            Return subsubgroup.SignalSignature
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsOutput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.PMUName = pmu Then
                    For Each subsubgroup In subgroup.SignalList
                        If subsubgroup.SignalSignature.SignalName = channel Then
                            Return subsubgroup.SignalSignature
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByProcessConfigStepsOutput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.PMUName = pmu Then
                    For Each subsubgroup In subgroup.SignalList
                        If subsubgroup.SignalSignature.SignalName = channel Then
                            Return subsubgroup.SignalSignature
                        End If
                    Next
                End If
            Next
        Next
        Return Nothing
    End Function
#End Region


#Region "Change signal selection"
    Private _selectedSignalChanged As ICommand
    Public Property SelectedSignalChanged As ICommand
        Get
            Return _selectedSignalChanged
        End Get
        Set(ByVal value As ICommand)
            _selectedSignalChanged = value
        End Set
    End Property
    Private Sub _signalSelected(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count < 1 And (obj.SignalSignature.PMUName Is Nothing Or obj.SignalSignature.TypeAbbreviation Is Nothing) Then
                _keepOriginalSelection(obj)
                MessageBox.Show("Clicked item is not a valid signal, or contains no valid signal!", "Error!", MessageBoxButtons.OK)
            Else
                If TypeOf _currentSelectedStep Is DQFilter OrElse TypeOf _currentSelectedStep Is TunableFilter OrElse TypeOf _currentSelectedStep Is Wrap OrElse TypeOf _currentSelectedStep Is Interpolate OrElse TypeOf _currentSelectedStep Is Unwrap OrElse TypeOf _currentSelectedStep Is NameTypeUnitPMU Then

                    Try
                        _changeSignalSelection(obj)
                    Catch ex As Exception
                        _keepOriginalSelection(obj)
                        MessageBox.Show("Error selecting signal(s) for DQfilter or data processing steps!" & vbCrLf & ex.Message, "Error!", MessageBoxButtons.OK)
                        _addLog("Error selecting signal(s) for DQfilter or data processing steps!" & ex.Message)
                    End Try
                ElseIf TypeOf _currentSelectedStep Is Multirate Then
                    If CurrentSelectedStep.FilterParameters.Count <> 0 Then
                        Try
                            _changeSignalSelection(obj)
                        Catch ex As Exception
                            _keepOriginalSelection(obj)
                            MessageBox.Show("Error selecting signal(s) for Multirate!" & vbCrLf & ex.Message, "Error!", MessageBoxButtons.OK)
                            _addLog("Error selecting signal(s) for Multirate!" & ex.Message)
                        End Try
                    Else
                        _keepOriginalSelection(obj)
                        MessageBox.Show("Please choose a way to specify sampling rate for Multirate!", "Error!", MessageBoxButtons.OK)
                        _addLog("Error selecting signal(s) for Multirate! No sampling rate specified!")
                    End If
                Else
                    Try
                        Select Case _currentSelectedStep.Name
                            Case "Scalar Repetition Customization"
                                Throw New Exception("Please do NOT select signals for Scalar Repetition Customization!")
                            Case "Addition Customization"
                                _changeSignalSelection(obj)
                                _checkAdditionCustomizationOutputType()
                            Case "Multiplication Customization"
                                _changeSignalSelection(obj)
                                _checkMultiplicationCustomizationOutputType()
                            Case "Subtraction Customization"
                                _setFocusedTextbox(obj)
                                _checkSubtractionCustomizationOutputType()
                            Case "Division Customization"
                                _setFocusedTextbox(obj)
                                _checkDivisionCustomizationOutputType()
                            Case "Raise signals to an exponent"
                                _changeSignalSelectionUnarySteps(obj)
                                _checkRaiseExpCustomizationOutputType()
                            Case "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Take complex conjugate of signals", "Metric Prefix Customization", "Angle Conversion Customization"
                                _changeSignalSelectionUnarySteps(obj)
                                ' For these 7 unary customization steps, the signal type and units of the input signal are applied to the output signal
                                ' So, the type are applied while signals are added in the _changeSignalSelectionUnarySteps() subroutine
                                ' Thus we don't need another subroutine to check output signal type
                                ' However, if we encounter bugs due to type for these steps, we might want to add a subroutine: _checkUnaryCustomizationOutputType()
                            Case "Return angle of complex valued signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _checkAngleForComplexSignalCustomizationOutputType()
                            Case "Phasor Creation Customization"
                                _changeSignalSelectionPhasorCreation(obj)
                                ' Type for output signal was set when signal selected, no need to check
                            Case "Power Calculation Customization"
                                If _currentSelectedStep.OutputInputMappingPair(0).Value.Count = 2 Then
                                    _changePhasorSignalForPowerCalculationCustomization(obj)
                                Else
                                    _changeMagAngSignalForPowerCalculationCustomization(obj)
                                End If
                                ' Here, type of output signal is not related to input signal, no need to check
                                ' It only relates to user choice and would only affect MATLAB calculation afterwards
                            Case "Specify Signal Type and Unit Customization"
                                _specifySignalTypeUnitSignalSelectionChanged(obj)
                            Case Else
                                Throw New Exception("Customization step not supported!")
                        End Select
                    Catch ex As Exception
                        _keepOriginalSelection(obj)
                        MessageBox.Show("Error selecting signal(s) for customization step!" & ex.Message, "Error!", MessageBoxButtons.OK)
                    End Try
                End If
            End If
        Else
            _keepOriginalSelection(obj)
            MessageBox.Show("Please select a step first!", "Error!", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub _checkAngleForComplexSignalCustomizationOutputType()
        For Each inputOutputPair In CurrentSelectedStep.OutputInputMappingPair
            If inputOutputPair.Value.Count > 0 Then
                Dim input = inputOutputPair.Value(0)
                If input.IsValid AndAlso input.TypeAbbreviation.Length = 3 Then
                    Dim letter2 = input.TypeAbbreviation.ToArray(1)
                    If letter2 = "P" Then
                        inputOutputPair.Key.TypeAbbreviation = input.TypeAbbreviation.Substring(0, 1) & "A" & input.TypeAbbreviation.Substring(2, 1)
                    End If
                Else
                    inputOutputPair.Key.TypeAbbreviation = "OTHER"
                End If
            End If
        Next
    End Sub

    Private Sub _checkRaiseExpCustomizationOutputType()
        For Each inputOutputPair In CurrentSelectedStep.OutputInputMappingPair
            If inputOutputPair.Value.Count > 0 Then
                If CurrentSelectedStep.Exponent = 1 OrElse inputOutputPair.Value(0).TypeAbbreviation = "SC" Then
                    inputOutputPair.Key.TypeAbbreviation = inputOutputPair.Value(0).TypeAbbreviation
                Else
                    inputOutputPair.Key.TypeAbbreviation = "OTHER"
                End If
            End If
        Next
    End Sub

    Private Sub _checkMultiplicationCustomizationOutputType()
        Dim type = ""
        Dim countNonScalarType = 0
        For Each signal In CurrentSelectedStep.InputChannels
            If signal.TypeAbbreviation <> "SC" Then
                countNonScalarType += 1
                If String.IsNullOrEmpty(type) Then
                    type = signal.TypeAbbreviation
                End If
            End If
        Next
        If countNonScalarType = 0 Then
            CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = "SC"
        ElseIf countNonScalarType = 1 Then
            CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = type
        Else
            CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = "OTHER"
        End If
    End Sub

    Private Sub _checkDivisionCustomizationOutputType()
        CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = "OTHER"
        If CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation IsNot Nothing AndAlso CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation IsNot Nothing Then
            If CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation <> CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation Then
                _addLog("Type of Divisor and Dividend should match! Different signal type found in Division customization step: " & CurrentSelectedStep.stepCounter & ", with types: " & CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation & " and " & CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation & ".")
                Throw New Exception("Type of Dividend and Divisor should match! Different signal type found in Division customization step: " & CurrentSelectedStep.stepCounter & ", with types: " & CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation & " and " & CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation & ".")
            End If
        End If
    End Sub

    Private Sub _checkSubtractionCustomizationOutputType()
        CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = "OTHER"
        If CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation IsNot Nothing AndAlso CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation IsNot Nothing Then
            If CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation <> CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation Then
                _addLog("Type of subtrahend and minuend should match! Different signal type found in subtraction customization step: " & CurrentSelectedStep.stepCounter & ", with types: " & CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation & " and " & CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation & ".")
                Throw New Exception("Type of subtrahend and minuend should match! Different signal type found in subtraction customization step: " & CurrentSelectedStep.stepCounter & ", with types: " & CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation & " and " & CurrentSelectedStep.MinuendOrDividend.TypeAbbreviation & ".")
            Else
                CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = CurrentSelectedStep.SubtrahendOrDivisor.TypeAbbreviation
            End If
        End If
    End Sub

    Private Sub _checkAdditionCustomizationOutputType()
        Dim type = ""
        For Each signal In CurrentSelectedStep.InputChannels
            If String.IsNullOrEmpty(type) Then
                type = signal.TypeAbbreviation
            ElseIf type <> signal.TypeAbbreviation Then
                _addLog("All terms of addition customization have to be the same signal type! Different signal type found in addition customization step: " & CurrentSelectedStep.stepCounter & ", with types: " & type & " and " & signal.TypeAbbreviation & ".")
                CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = "OTHER"
                Throw New Exception("All terms of addition customization have to be the same signal type! Different signal type found in addition customization step: " & CurrentSelectedStep.stepCounter & ", with types: " & type & " and " & signal.TypeAbbreviation & ".")
                Exit Sub
            End If
        Next
        If Not String.IsNullOrEmpty(type) Then
            CurrentSelectedStep.OutputChannels(0).TypeAbbreviation = type
        End If
    End Sub

    Private Sub _keepOriginalSelection(obj As SignalTypeHierachy)
        If obj.SignalList.Count = 0 AndAlso Not String.IsNullOrEmpty(obj.SignalSignature.PMUName) AndAlso Not String.IsNullOrEmpty(obj.SignalSignature.TypeAbbreviation) Then
            If obj.SignalSignature.IsChecked Then
                obj.SignalSignature.IsChecked = False
            Else
                obj.SignalSignature.IsChecked = True
            End If
        Else
            If obj.SignalSignature.IsChecked Then
                _checkAllChildren(obj, False)
            Else
                _checkAllChildren(obj, True)
            End If
            _dataConfigDetermineAllParentNodeStatus()
        End If
    End Sub
    Private _textboxesLostFocus As ICommand
    Public Property TextboxesLostFocus As ICommand
        Get
            Return _textboxesLostFocus
        End Get
        Set(ByVal value As ICommand)
            _textboxesLostFocus = value
        End Set
    End Property
    Private Sub _recoverCheckStatusOfCurrentStep(obj As Object)
        For Each signal In obj.InputChannels
            signal.IsChecked = True
        Next
        _dataConfigDetermineAllParentNodeStatus()
        If _currentSelectedStep IsNot Nothing Then
            _currentSelectedStep.CurrentCursor = ""
        End If
        _currentInputOutputPair = Nothing
        _currentFocusedPhasorSignalForPowerCalculation = Nothing
    End Sub

    ''' <summary>
    ''' This method is for the subtraction or division cutomization steps
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _setFocusedTextbox(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 OrElse obj.SignalSignature.PMUName Is Nothing OrElse obj.SignalSignature.TypeAbbreviation Is Nothing Then    'if selected a group of signal
            Throw New Exception("Error! Please select valid signal for this textbox! We need a single signal, cannot be group of signals!")
        Else
            If _currentSelectedStep.CurrentCursor = "" Then ' if no textbox selected, textbox lost it focus right after a click any where else, so only click immediate follow a textbox selection would work
                Throw New Exception("Error! Please select a valid text box for this input signal!")
            ElseIf _currentSelectedStep.CurrentCursor = "MinuendOrDividend" Then
                If _currentSelectedStep.SubtrahendOrDivisor IsNot Nothing AndAlso obj.SignalSignature = _currentSelectedStep.SubtrahendOrDivisor Then
                    Throw New Exception("Minuend Or divident cannot be the same as the subtrahend or divisor!")
                End If
                If obj.SignalSignature.IsChecked Then       ' check box checked
                    If _currentSelectedStep.MinuendOrDividend IsNot Nothing And _currentSelectedStep.MinuendOrDividend IsNot _currentSelectedStep.SubtrahendOrDivisor Then  ' if the current text box has content and not equal to the divisor
                        _currentSelectedStep.MinuendOrDividend.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(_currentSelectedStep.MinuendOrDividend)
                    End If
                    _currentSelectedStep.MinuendOrDividend = obj.SignalSignature
                    If Not _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    End If
                Else                                        ' check box unchecked
                    If _currentSelectedStep.MinuendOrDividend Is obj.SignalSignature Then   ' if the content of the text box is the same as the clicked item and the checkbox is unchecked, means user wants to delete the content in the textbox
                        If _currentSelectedStep.SubtrahendOrDivisor Is obj.SignalSignature Then     ' however, if the textbox has the same contect as the divisor or subtrahend, we cannot uncheck the clicked item
                            obj.SignalSignature.IsChecked = True
                        Else
                            _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                        End If
                        Dim dummy = New SignalSignatures("PleaseAddASignal", "PleaseAddASignal")
                        dummy.IsValid = False
                        _currentSelectedStep.MinuendOrDividend = dummy
                    End If
                End If
                _currentSelectedStep.CurrentCursor = ""
                _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
                _dataConfigDetermineAllParentNodeStatus()
            ElseIf _currentSelectedStep.CurrentCursor = "SubtrahendOrDivisor" Then
                If _currentSelectedStep.MinuendOrDividend IsNot Nothing AndAlso obj.SignalSignature = _currentSelectedStep.MinuendOrDividend Then
                    Throw New Exception("Subtrahend Or divisor cannot be the same as the minuend or divident!")
                End If
                If obj.SignalSignature.IsChecked Then
                    If _currentSelectedStep.SubtrahendOrDivisor IsNot Nothing And _currentSelectedStep.SubtrahendOrDivisor IsNot _currentSelectedStep.MinuendOrDividend Then
                        _currentSelectedStep.SubtrahendOrDivisor.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(_currentSelectedStep.SubtrahendOrDivisor)
                    End If
                    _currentSelectedStep.SubtrahendOrDivisor = obj.SignalSignature
                    If Not _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    End If
                Else
                    If _currentSelectedStep.SubtrahendOrDivisor Is obj.SignalSignature Then
                        If _currentSelectedStep.MinuendOrDividend Is obj.SignalSignature Then
                            obj.SignalSignature.IsChecked = True
                        Else
                            _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                        End If
                        Dim dummy = New SignalSignatures("PleaseAddASignal", "PleaseAddASignal")
                        dummy.IsValid = False
                        _currentSelectedStep.SubtrahendOrDivisor = dummy
                    End If
                End If
                _currentSelectedStep.CurrentCursor = ""
                _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
                _dataConfigDetermineAllParentNodeStatus()
            End If
        End If
        _determineFileDirCheckableStatus()
    End Sub
    Private Sub _changeSignalSelectionUnarySteps(obj As SignalTypeHierachy)
        If Not _currentInputOutputPair.HasValue Then
            If obj.SignalSignature.IsChecked Then
                '_checkAllChildren(obj, obj.SignalSignature.IsChecked)
                _addOrDeleteInputSignal(obj, obj.SignalSignature.IsChecked)
                _addOuputSignalsForUnaryCustomizationStep(obj)
            Else
                _removeMatchingInputOutputSignalsUnary(obj)
            End If
        Else
            If obj.SignalList.Count > 0 Or String.IsNullOrEmpty(obj.SignalSignature.PMUName) Or String.IsNullOrEmpty(obj.SignalSignature.TypeAbbreviation) Then
                _keepOriginalSelection(obj)
                Throw New Exception("Please select a valid signal!")
            ElseIf obj.SignalSignature.IsChecked AndAlso _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                Throw New Exception("Selected signal already in this step!")
            Else
                Dim targetPairs = (From x In DirectCast(_currentSelectedStep, Customization).OutputInputMappingPair Where x.Key = _currentInputOutputPair.Value.Key Select x).ToList

                If targetPairs.Count = 1 Then
                    Dim oldInput = targetPairs.FirstOrDefault.Value.FirstOrDefault
                    If Not String.IsNullOrEmpty(oldInput.PMUName) AndAlso Not String.IsNullOrEmpty(oldInput.SignalName) AndAlso Not String.IsNullOrEmpty(oldInput.TypeAbbreviation) Then
                        _currentSelectedStep.InputChannels.Remove(oldInput)
                        oldInput.IsChecked = False
                    End If
                    targetPairs.FirstOrDefault.Value.Clear()
                    If obj.SignalSignature.IsChecked Then
                        targetPairs.FirstOrDefault.Value.Add(obj.SignalSignature)
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                        targetPairs.FirstOrDefault.Key.TypeAbbreviation = obj.SignalSignature.TypeAbbreviation
                    Else
                        _currentSelectedStep.OutputChannels.Remove(targetPairs.FirstOrDefault.Key)
                        _currentSelectedStep.OutputInputMappingPair.Remove(targetPairs.FirstOrDefault)
                    End If
                    _currentInputOutputPair = Nothing
                Else
                    Throw New Exception("Error adding/deleting selected item to the step!")
                End If
            End If
        End If

        _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        If TypeOf (_currentSelectedStep) Is Customization Then
            _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
        End If
        _dataConfigDetermineAllParentNodeStatus()
        _determineFileDirCheckableStatus()
    End Sub

    Private Sub _changeSignalSelection(obj As SignalTypeHierachy)
        _checkAllChildren(obj, obj.SignalSignature.IsChecked)
        _addOrDeleteInputSignal(obj, obj.SignalSignature.IsChecked)
        If TypeOf _currentSelectedStep Is DQFilter OrElse TypeOf _currentSelectedStep Is TunableFilter OrElse TypeOf _currentSelectedStep Is Wrap OrElse TypeOf _currentSelectedStep Is Interpolate OrElse TypeOf _currentSelectedStep Is Unwrap OrElse TypeOf _currentSelectedStep Is NameTypeUnitPMU Then
            _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
        ElseIf TypeOf _currentSelectedStep Is Multirate Then
            _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
            _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        Else
            _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        End If
        _dataConfigDetermineAllParentNodeStatus()
        _processConfigDetermineAllParentNodeStatus()
        _determineFileDirCheckableStatus()
    End Sub
    Private Sub _changeSignalSelectionPhasorCreation(obj As SignalTypeHierachy)
        If Not _currentInputOutputPair.HasValue Then
            If obj.SignalSignature.IsChecked Then
                _addMatchingInputOutputSignalsPhasor(obj)
            Else
                _removeMatchingInputOutputSignalsPhasor(obj)
            End If
        Else
            If obj.SignalList.Count > 0 Or String.IsNullOrEmpty(obj.SignalSignature.PMUName) Or String.IsNullOrEmpty(obj.SignalSignature.TypeAbbreviation) Then
                _keepOriginalSelection(obj)
                Throw New Exception("Please select a valid signal!")
            ElseIf obj.SignalSignature.IsChecked AndAlso _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                Throw New Exception("Selected signal already in this step!")
            Else
                Dim targetPairs = (From x In DirectCast(_currentSelectedStep, Customization).OutputInputMappingPair Where x.Key = _currentInputOutputPair.Value.Key Select x).ToList

                If targetPairs.Count = 1 Then
                    Dim oldInputMag = targetPairs.FirstOrDefault.Value.FirstOrDefault
                    Dim oldInputAng = targetPairs.FirstOrDefault.Value(1)
                    If Not String.IsNullOrEmpty(oldInputMag.PMUName) AndAlso Not String.IsNullOrEmpty(oldInputMag.SignalName) AndAlso Not String.IsNullOrEmpty(oldInputMag.TypeAbbreviation) Then
                        _currentSelectedStep.InputChannels.Remove(oldInputMag)
                        oldInputMag.IsChecked = False
                    End If
                    If Not String.IsNullOrEmpty(oldInputAng.PMUName) AndAlso Not String.IsNullOrEmpty(oldInputAng.SignalName) AndAlso Not String.IsNullOrEmpty(oldInputAng.TypeAbbreviation) Then
                        _currentSelectedStep.InputChannels.Remove(oldInputAng)
                        oldInputAng.IsChecked = False
                    End If
                    targetPairs.FirstOrDefault.Value.Clear()
                    If obj.SignalSignature.IsChecked Then
                        Dim ang = _findMatchingAng(obj.SignalSignature)
                        If ang IsNot Nothing Then
                            ang.IsChecked = True
                            targetPairs.FirstOrDefault.Value.Add(obj.SignalSignature)
                            _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                            targetPairs.FirstOrDefault.Value.Add(ang)
                            _currentSelectedStep.InputChannels.Add(ang)
                            targetPairs.FirstOrDefault.Key.TypeAbbreviation = ang.TypeAbbreviation.Substring(0, 1) & "P" & ang.TypeAbbreviation.Substring(2, 1)
                        Else
                            _currentInputOutputPair = Nothing
                            Throw New Exception("Cannot find matching angle signal for selected magnitude signal: " & obj.SignalSignature.SignalName)
                        End If
                    Else
                        _currentSelectedStep.OutputChannels.Remove(targetPairs.FirstOrDefault.Key)
                        _currentSelectedStep.OutputInputMappingPair.Remove(targetPairs.FirstOrDefault)
                    End If
                    _currentInputOutputPair = Nothing
                Else
                    _currentInputOutputPair = Nothing
                    Throw New Exception("Error adding/deleting selected item to the step!")
                End If
            End If
        End If

        _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        If TypeOf (_currentSelectedStep) Is Customization Then
            _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
        End If
        _dataConfigDetermineAllParentNodeStatus()
        _determineFileDirCheckableStatus()
    End Sub
    ''' <summary>
    ''' find matching Ang signal given a Mag signal
    ''' </summary>
    ''' <param name="mag"></param>
    Private Function _findMatchingAng(mag As SignalSignatures) As SignalSignatures
        Dim type = mag.TypeAbbreviation
        Dim signalName = mag.SignalName
        Dim parts = signalName.Split(".")
        If parts.Length <> 3 Then
            signalName = parts(0)
        Else
            signalName = parts(1)
        End If
        Dim pmu = mag.PMUName
        If mag.IsCustomSignal Then
            For Each group In GroupedSignalByDataConfigStepsOutput
                For Each subgroup In group.SignalList
                    If subgroup.SignalSignature.PMUName = pmu Then
                        For Each signal In subgroup.SignalList
                            Dim target = signal.SignalSignature.SignalName.Split(".")
                            Dim foundSignalName = ""
                            If target.Length <> 3 Then
                                foundSignalName = target(0)
                            Else
                                foundSignalName = target(1)
                            End If
                            If foundSignalName = signalName Then
                                Return signal.SignalSignature
                            End If
                        Next
                    End If
                Next
            Next
        Else
            For Each group In GroupedRawSignalsByType
                If group.SignalSignature.IsEnabled Then
                    For Each subgroup In group.SignalList
                        If subgroup.SignalSignature.TypeAbbreviation = type.Substring(0, 1) Then
                            For Each subsubgroup In subgroup.SignalList
                                If subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) = "A" Then
                                    For Each subsubsubgroup In subsubgroup.SignalList
                                        If subsubsubgroup.SignalSignature.TypeAbbreviation.Substring(2, 1) = type.Substring(2, 1) Then
                                            For Each signal In subsubsubgroup.SignalList
                                                If signal.SignalSignature.PMUName = pmu Then
                                                    Dim target = signal.SignalSignature.SignalName.Split(".")
                                                    Dim foundSignalName = ""
                                                    If target.Length <> 3 Then
                                                        foundSignalName = target(0)
                                                    Else
                                                        foundSignalName = target(1)
                                                    End If
                                                    If foundSignalName = signalName Then
                                                        Return signal.SignalSignature
                                                    End If
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        End If
        Return Nothing
    End Function

    Private Sub _changePhasorSignalForPowerCalculationCustomization(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 OrElse obj.SignalSignature.PMUName Is Nothing OrElse obj.SignalSignature.TypeAbbreviation Is Nothing Then    'if selected a group of signal
            Throw New Exception("Error! Please select valid signal for this textbox! We need a single signal, cannot be group of signals!")
        Else

            If obj.SignalSignature.TypeAbbreviation.Length <> 3 OrElse obj.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "P" Then
                '_keepOriginalSelection(obj)
                _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not a phasor signal.")
                Throw New Exception("Signal selection is not Valid! Please select a signal of type phasor.")
            ElseIf _currentFocusedPhasorSignalForPowerCalculation Is Nothing Then
                '_keepOriginalSelection(obj)
                Throw New Exception("No textbox selected!")
                'ElseIf _currentFocusedPhasorSignalForPowerCalculation.IsValid AndAlso (_currentFocusedPhasorSignalForPowerCalculation.TypeAbbreviation.Substring(0) <> obj.SignalSignature.TypeAbbreviation.Substring(0)) Then
                '    _keepOriginalSelection(obj)
                '    _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type: " & _currentFocusedPhasorSignalForPowerCalculation.TypeAbbreviation)
                '    Throw New Exception("Signal selection is not Valid! Please select a signal of type: " & _currentFocusedPhasorSignalForPowerCalculation.TypeAbbreviation)
            ElseIf _currentSelectedStep.OutputInputMappingPair(0).Value(0) = _currentFocusedPhasorSignalForPowerCalculation Then
                If obj.SignalSignature.TypeAbbreviation.Substring(0, 1) = "V" Then
                    Dim oldPhasor = _currentSelectedStep.OutputInputMappingPair(0).Value(0)
                    If _currentSelectedStep.InputChannels.Contains(oldPhasor) Then
                        oldPhasor.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(oldPhasor)
                    End If
                    _currentSelectedStep.OutputInputMappingPair(0).Value.Remove(oldPhasor)
                    If obj.SignalSignature.IsChecked Then
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(0, obj.SignalSignature)
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    Else
                        Dim dummy = New SignalSignatures("PleaseAddVoltagePhasor", "PleaseAddVoltagePhasor")
                        dummy.IsValid = False
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(0, dummy)
                    End If
                Else
                    _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type Voltage phasor ")
                    Throw New Exception("Signal selection is not Valid! Please select a signal of voltage phasor")
                End If
            ElseIf _currentSelectedStep.OutputInputMappingPair(0).Value(1) = _currentFocusedPhasorSignalForPowerCalculation Then
                If obj.SignalSignature.TypeAbbreviation.Substring(0, 1) = "I" Then
                    Dim oldPhasor = _currentSelectedStep.OutputInputMappingPair(0).Value(1)
                    If _currentSelectedStep.InputChannels.Contains(oldPhasor) Then
                        oldPhasor.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(oldPhasor)
                    End If
                    _currentSelectedStep.OutputInputMappingPair(0).Value.Remove(oldPhasor)
                    If obj.SignalSignature.IsChecked Then
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(1, obj.SignalSignature)
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    Else
                        Dim dummy = New SignalSignatures("PleaseAddCurrentPhasor", "PleaseAddVoltagePhasor")
                        dummy.IsValid = False
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(1, dummy)
                    End If
                Else
                    _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type current phasor.")
                    Throw New Exception("Signal selection is not Valid! Please select a signal of current phasor.")
                End If
            Else
                Throw New Exception("Error changing signal for this power calculation step!")
            End If
        End If
        _currentFocusedPhasorSignalForPowerCalculation = Nothing
        _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
        _dataConfigDetermineAllParentNodeStatus()
        _determineFileDirCheckableStatus()
    End Sub
    Private Sub _changeMagAngSignalForPowerCalculationCustomization(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 Then
            _addLog("Selected a group of signals! Signal group: " & obj.SignalSignature.SignalName & ", number of signals: " & obj.SignalList.Count & " .")
            Throw New Exception("Please only select a signal valid signal instead of a group of signals!")
        Else
            If obj.SignalSignature.TypeAbbreviation.Length <> 3 OrElse obj.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not a magnitude signal.")
                Throw New Exception("Signal selection is not Valid! Please select a signal of magnitude type.")
            ElseIf _currentFocusedPhasorSignalForPowerCalculation Is Nothing Then
                Throw New Exception("No textbox selected!")
            ElseIf _currentSelectedStep.OutputInputMappingPair(0).Value(0) = _currentFocusedPhasorSignalForPowerCalculation OrElse _currentSelectedStep.OutputInputMappingPair(0).Value(1) = _currentFocusedPhasorSignalForPowerCalculation Then
                If obj.SignalSignature.TypeAbbreviation.Substring(0, 1) = "V" Then
                    Dim oldVM = _currentSelectedStep.OutputInputMappingPair(0).Value(0)
                    Dim oldVA = _currentSelectedStep.OutputInputMappingPair(0).Value(1)
                    If _currentSelectedStep.InputChannels.Contains(oldVM) Then
                        oldVM.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(oldVM)
                    End If
                    If _currentSelectedStep.InputChannels.Contains(oldVA) Then
                        oldVA.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(oldVA)
                    End If
                    _currentSelectedStep.OutputInputMappingPair(0).Value.Remove(oldVM)
                    _currentSelectedStep.OutputInputMappingPair(0).Value.Remove(oldVA)
                    If obj.SignalSignature.IsChecked Then
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(0, obj.SignalSignature)
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                        Dim newVA = _findMatchingAng(obj.SignalSignature)
                        If newVA Is Nothing Then
                            newVA = New SignalSignatures("NoMatchingAnglefound")
                            newVA.IsValid = False
                        Else
                            newVA.IsChecked = True
                        End If
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(1, newVA)
                        _currentSelectedStep.InputChannels.Add(newVA)
                    Else
                        Dim dummyVM = New SignalSignatures("PleaseAddVoltageMag")
                        dummyVM.IsValid = False
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(0, dummyVM)
                        Dim dummyVA = New SignalSignatures("PleaseAddVoltageAng")
                        dummyVA.IsValid = False
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(1, dummyVA)
                    End If
                Else
                    _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type Voltage Magnitude.")
                    Throw New Exception("Signal selection is not Valid! Please select a signal of voltage Magnitude.")
                End If
            ElseIf _currentSelectedStep.OutputInputMappingPair(0).Value(2) = _currentFocusedPhasorSignalForPowerCalculation OrElse _currentSelectedStep.OutputInputMappingPair(0).Value(3) = _currentFocusedPhasorSignalForPowerCalculation Then
                If obj.SignalSignature.TypeAbbreviation.Substring(0, 1) = "I" Then
                    Dim oldIM = _currentSelectedStep.OutputInputMappingPair(0).Value(2)
                    Dim oldIA = _currentSelectedStep.OutputInputMappingPair(0).Value(3)
                    If _currentSelectedStep.InputChannels.Contains(oldIM) Then
                        oldIM.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(oldIM)
                    End If
                    If _currentSelectedStep.InputChannels.Contains(oldIA) Then
                        oldIA.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(oldIA)
                    End If
                    _currentSelectedStep.OutputInputMappingPair(0).Value.Remove(oldIM)
                    _currentSelectedStep.OutputInputMappingPair(0).Value.Remove(oldIA)
                    If obj.SignalSignature.IsChecked Then
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(2, obj.SignalSignature)
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                        Dim newIA = _findMatchingAng(obj.SignalSignature)
                        If newIA Is Nothing Then
                            newIA = New SignalSignatures("NoMatchingAnglefound")
                            newIA.IsValid = False
                        Else
                            newIA.IsChecked = True
                        End If
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(3, newIA)
                        _currentSelectedStep.InputChannels.Add(newIA)
                    Else
                        Dim dummyIM = New SignalSignatures("PleaseAddCurrentMag")
                        dummyIM.IsValid = False
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(2, dummyIM)
                        Dim dummyIA = New SignalSignatures("PleaseAddCurrentAng")
                        dummyIA.IsValid = False
                        _currentSelectedStep.OutputInputMappingPair(0).Value.Insert(3, dummyIA)
                    End If
                Else
                    _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type current magnitude.")
                    Throw New Exception("Signal selection is not Valid! Please select a signal of current magnitude.")
                End If
            Else
                Throw New Exception("Error changing signal for this power calculation step!")
            End If
            _currentFocusedPhasorSignalForPowerCalculation = Nothing
            _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
            _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
            _dataConfigDetermineAllParentNodeStatus()
            _determineFileDirCheckableStatus()
        End If
    End Sub
    Private Sub _specifySignalTypeUnitSignalSelectionChanged(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 OrElse String.IsNullOrEmpty(obj.SignalSignature.PMUName) OrElse String.IsNullOrEmpty(obj.SignalSignature.TypeAbbreviation) Then
            _keepOriginalSelection(obj)
            Throw New Exception("Signal selection is not Valid! Please select a single valid signal.")
        Else
            If _currentSelectedStep.InputChannels.Count > 0 Then
                _currentSelectedStep.InputChannels(0).IsChecked = False
                _currentSelectedStep.InputChannels.Clear
            End If
            If obj.SignalSignature.IsChecked Then
                _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                If String.IsNullOrEmpty(_currentSelectedStep.OutputChannels(0).SignalName) Then
                    _currentSelectedStep.OutputChannels(0).SignalName = obj.SignalSignature.SignalName
                End If
            End If
            _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
            _dataConfigDetermineAllParentNodeStatus()
            _determineFileDirCheckableStatus()
        End If
    End Sub

    Private Sub _removeMatchingInputOutputSignalsUnary(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 Then
            For Each child In obj.SignalList
                _removeMatchingInputOutputSignalsUnary(child)
            Next
        Else
            Dim targetToRemove = (From x In DirectCast(_currentSelectedStep, Customization).OutputInputMappingPair Where x.Value(0).SignalName = obj.SignalSignature.SignalName Select x).ToList
            For Each target In targetToRemove
                _currentSelectedStep.OutputChannels.Remove(target.Key)
                _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                _currentSelectedStep.OutputInputMappingPair.Remove(target)
                obj.SignalSignature.IsChecked = False
            Next
        End If
    End Sub
    Private Sub _removeMatchingInputOutputSignalsPhasor(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 Then
            For Each signal In obj.SignalList
                _removeMatchingInputOutputSignalsPhasor(signal)
            Next
        Else
            If obj.SignalSignature.TypeAbbreviation.Length = 3 AndAlso obj.SignalSignature.TypeAbbreviation.Substring(1, 1) = "M" Then
                Dim targetToRemove = (From x In DirectCast(_currentSelectedStep, Customization).OutputInputMappingPair Where x.Value(0).SignalName = obj.SignalSignature.SignalName Select x).ToList
                For Each target In targetToRemove
                    _currentSelectedStep.OutputChannels.Remove(target.Key)
                    _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                    _currentSelectedStep.InputChannels.Remove(target.Value(1))
                    _currentSelectedStep.OutputInputMappingPair.Remove(target)
                    obj.SignalSignature.IsChecked = False
                    target.Value(1).IsChecked = False
                Next
            End If
        End If
    End Sub
    ''' <summary>
    ''' This subroutine adds output signal for each selected input signal for unary customization steps
    ''' It also pairs the output to the input
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _addOuputSignalsForUnaryCustomizationStep(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count > 0 Then
                For Each child In obj.SignalList
                    _addOuputSignalsForUnaryCustomizationStep(child)
                Next
            Else
                Dim newOutput = New SignalSignatures(obj.SignalSignature.SignalName, _currentSelectedStep.CustPMUname, obj.SignalSignature.TypeAbbreviation)
                newOutput.IsCustomSignal = True
                _currentSelectedStep.outputChannels.Add(newOutput)
                Dim targetkey = (From kvp In DirectCast(_currentSelectedStep, Customization).OutputInputMappingPair Where kvp.Key = newOutput Select kvp Distinct).ToList()
                If targetkey.Count = 0 Then
                    Dim kvp = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(newOutput, New ObservableCollection(Of SignalSignatures))
                    kvp.Value.Add(obj.SignalSignature)
                    _currentSelectedStep.OutputInputMappingPair.Add(kvp)
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' add or remove selected signals to the inputChannels of customization steps except power calculation and phasor creation
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="isChecked"></param>
    Private Sub _addOrDeleteInputSignal(obj As SignalTypeHierachy, isChecked As Boolean)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count > 0 Then
                For Each signal In obj.SignalList
                    If signal.SignalSignature.IsEnabled Then
                        _addOrDeleteInputSignal(signal, isChecked)
                    End If
                Next
            Else
                ' This happens when user check a input grouped by steps, but that step has no input signals thus has no child
                If (obj.SignalSignature.PMUName Is Nothing Or obj.SignalSignature.TypeAbbreviation Is Nothing) Then
                    Throw New Exception("Item is not a valid signal, or contains no valid signal, nothing to be added or removed!")
                Else
                    If isChecked Then
                        If _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                            Throw New Exception("Selected item " & obj.SignalSignature.SignalName & " already exist in this step!")
                        End If
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                        If TypeOf _currentSelectedStep Is DQFilter Then
                            obj.SignalSignature.PassedThroughDQFilter = True
                            _currentSelectedStep.OutputChannels.Add(obj.SignalSignature)
                        End If
                        If TypeOf _currentSelectedStep Is TunableFilter OrElse TypeOf _currentSelectedStep Is Wrap OrElse TypeOf _currentSelectedStep Is Interpolate OrElse TypeOf _currentSelectedStep Is Unwrap Then
                            obj.SignalSignature.PassedThroughProcessor = True
                            _currentSelectedStep.OutputChannels.Add(obj.SignalSignature)
                        End If
                        If TypeOf _currentSelectedStep Is Multirate Then
                            Dim newOutput = New SignalSignatures(obj.SignalSignature.SignalName)
                            newOutput.PMUName = _currentSelectedStep.FilterParameters(0).Value
                            newOutput.TypeAbbreviation = obj.SignalSignature.TypeAbbreviation
                            newOutput.IsCustomSignal = True
                            newOutput.Unit = obj.SignalSignature.Unit
                            If _currentSelectedStep.FilterParameters.Count = 2 Then
                                newOutput.SamplingRate = _currentSelectedStep.NewRate
                            ElseIf _currentSelectedStep.FilterParameters.Count = 3 Then
                                Dim p = 0
                                Integer.TryParse(_currentSelectedStep.PElement, p)
                                Dim q = 0
                                Integer.TryParse(_currentSelectedStep.QElement, q)
                                If q <> 0 Then
                                    newOutput.SamplingRate = obj.SignalSignature.SamplingRate * p / q
                                End If
                            End If
                            _currentSelectedStep.OutputChannels.Add(newOutput)
                        End If
                        If TypeOf _currentSelectedStep Is NameTypeUnitPMU Then
                            Dim newOutput = New SignalSignatures()
                            If CurrentSelectedStep.InputChannels.Count > 1 Then
                                newOutput.SignalName = obj.SignalSignature.SignalName
                                CurrentSelectedStep.NewChannel = ""
                            Else
                                CurrentSelectedStep.NewChannel = obj.SignalSignature.SignalName
                                newOutput.SignalName = CurrentSelectedStep.NewChannel
                            End If
                            newOutput.PMUName = obj.SignalSignature.PMUName
                            newOutput.TypeAbbreviation = CurrentSelectedStep.NewType
                            newOutput.Unit = CurrentSelectedStep.NewUnit
                            newOutput.IsCustomSignal = True
                            newOutput.SamplingRate = obj.SignalSignature.SamplingRate
                            _currentSelectedStep.OutputChannels.Add(newOutput)
                        End If
                    Else
                        _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                        If TypeOf _currentSelectedStep Is DQFilter Then
                            obj.SignalSignature.PassedThroughDQFilter = False
                            _currentSelectedStep.OutputChannels.Remove(obj.SignalSignature)
                        End If
                        If TypeOf _currentSelectedStep Is TunableFilter OrElse TypeOf _currentSelectedStep Is Wrap OrElse TypeOf _currentSelectedStep Is Interpolate OrElse TypeOf _currentSelectedStep Is Unwrap Then
                            obj.SignalSignature.PassedThroughProcessor = False
                            _currentSelectedStep.OutputChannels.Remove(obj.SignalSignature)
                        End If
                        If TypeOf _currentSelectedStep Is Multirate Then

                            For Each output In _currentSelectedStep.OutputChannels
                                If output.SignalName = obj.SignalSignature.SignalName AndAlso output.TypeAbbreviation = obj.SignalSignature.TypeAbbreviation Then
                                    _currentSelectedStep.OutputChannels.Remove(output)
                                    Exit For
                                End If
                            Next
                        End If
                        If TypeOf _currentSelectedStep Is NameTypeUnitPMU Then

                            If CurrentSelectedStep.OutputChannels.Count = 1 Then
                                CurrentSelectedStep.OutputChannels.Clear
                            Else
                                For Each output In _currentSelectedStep.OutputChannels
                                    If output.SignalName = obj.SignalSignature.SignalName AndAlso output.PMUName = obj.SignalSignature.PMUName Then
                                        _currentSelectedStep.OutputChannels.Remove(output)
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                    End If
                    obj.SignalSignature.IsChecked = isChecked
                End If
            End If
        End If
    End Sub
    Private Sub _addMatchingInputOutputSignalsPhasor(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 Then
            For Each signal In obj.SignalList
                If signal.SignalSignature.IsEnabled Then
                    _addMatchingInputOutputSignalsPhasor(signal)
                End If
            Next
        Else
            If (obj.SignalSignature.PMUName Is Nothing Or obj.SignalSignature.TypeAbbreviation Is Nothing) Then
                Throw New Exception("Item is not a valid signal, or contains no valid signal, nothing to be added or removed!")
            ElseIf _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                Throw New Exception("Selected signal: " & obj.SignalSignature.SignalName & " already exists in this step, duplication not allowed!")
            ElseIf obj.SignalSignature.TypeAbbreviation.Length = 3 AndAlso obj.SignalSignature.TypeAbbreviation.Substring(1, 1) = "M" Then
                Dim ang = _findMatchingAng(obj.SignalSignature)
                If ang IsNot Nothing Then
                    ang.IsChecked = True
                    Dim pmu = _currentSelectedStep.CustPMUname
                    If pmu Is Nothing Then
                        pmu = _lastCustPMUname
                    End If
                    Dim name = ang.SignalName
                    Dim nameParts = name.Split(".")
                    If nameParts.Length <> 3 Then
                        name = nameParts(0)
                    Else
                        name = nameParts(0) & nameParts(1)
                    End If
                    Dim type = ang.TypeAbbreviation.Substring(0, 1) & "P" & ang.TypeAbbreviation.Substring(2, 1)
                    Dim newOutput = New SignalSignatures(name, pmu, type)
                    newOutput.IsCustomSignal = True
                    Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(newOutput, New ObservableCollection(Of SignalSignatures))
                    newPair.Value.Add(obj.SignalSignature)
                    newPair.Value.Add(ang)
                    _currentSelectedStep.OutputInputMappingPair.Add(newPair)
                    _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    _currentSelectedStep.InputChannels.Add(ang)
                    _currentSelectedStep.OutputChannels.Add(newOutput)
                Else
                    Throw New Exception("Cannot find matching angle signal for selected magnitude signal: " & obj.SignalSignature.SignalName)
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' This sub checks/unchecks of all children of a node in the signal grouped by type parent tree
    ''' </summary>
    ''' <param name="node"></param>
    ''' <param name="isChecked"></param>
    Private Sub _checkAllChildren(ByRef node As SignalTypeHierachy, ByVal isChecked As Boolean)
        If node.SignalList.Count > 0 Then
            ' if not a leaf node, call itself recursively to check/uncheck all children
            For Each child In node.SignalList
                If child.SignalSignature.IsEnabled Then
                    child.SignalSignature.IsChecked = isChecked
                    _checkAllChildren(child, isChecked)
                End If
            Next
        End If
    End Sub


    Private _setCurrentFocusedTextbox As ICommand
    Public Property SetCurrentFocusedTextbox As ICommand
        Get
            Return _setCurrentFocusedTextbox
        End Get
        Set(ByVal value As ICommand)
            _setCurrentFocusedTextbox = value
        End Set
    End Property
    ''' <summary>
    ''' This method is called when a textbox is clicked in subtraction, division, exponent, unary.... customization
    ''' where we want signal to be put in individual textboxes.
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _currentFocusedTextBoxChanged(obj As SignalSignatures)
        For Each signal In _currentSelectedStep.InputChannels
            signal.IsChecked = False
        Next
        If obj IsNot Nothing AndAlso Not String.IsNullOrEmpty(obj.TypeAbbreviation) AndAlso Not String.IsNullOrEmpty(obj.PMUName) Then
            obj.IsChecked = True
        End If
        _dataConfigDetermineAllParentNodeStatus()
    End Sub

    ''' <summary>
    ''' This points to the current selected textbox of a Unary operation step which is a pair of input output.
    ''' </summary>
    Private _currentInputOutputPair As KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures)) ? = Nothing
    'Private _currentMultipleInputOutputPair As KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures)) ? = Nothing
    Private _setCurrentFocusedTextboxUnarySteps As ICommand
    Public Property SetCurrentFocusedTextboxUnarySteps As ICommand
        Get
            Return _setCurrentFocusedTextboxUnarySteps
        End Get
        Set(ByVal value As ICommand)
            _setCurrentFocusedTextboxUnarySteps = value
        End Set
    End Property
    Private Sub _currentFocusedTextBoxForUnaryStepsChanged(obj As Object)
        _currentInputOutputPair = obj
        _currentFocusedTextBoxChanged(obj.Value(0))
    End Sub
    Private _powerPhasorTextBoxGotFocus As ICommand
    Public Property PowerPhasorTextBoxGotFocus As ICommand
        Get
            Return _powerPhasorTextBoxGotFocus
        End Get
        Set(ByVal value As ICommand)
            _powerPhasorTextBoxGotFocus = value
        End Set
    End Property
    Private Sub _powerPhasorCurrentFocusedTextbox(obj As Object)
        _currentFocusedTextBoxChanged(obj)
        _currentFocusedPhasorSignalForPowerCalculation = obj
    End Sub
    Private _currentFocusedPhasorSignalForPowerCalculation As SignalSignatures

#End Region

    Private Sub _dataConfigDetermineAllParentNodeStatus()
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByType)
        _determineParentGroupedByTypeNodeStatus(GroupedRawSignalsByPMU)
        'For Each group In GroupedSignalsByPMU
        '    _determineParentCheckStatus(group)
        'Next
        For Each stepInput In GroupedSignalByDataConfigStepsInput
            If stepInput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
                _determineParentCheckStatus(stepInput)
            Else
                stepInput.SignalSignature.IsChecked = False
            End If
        Next
        For Each stepOutput In GroupedSignalByDataConfigStepsOutput
            If stepOutput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList)
                _determineParentCheckStatus(stepOutput)
            Else
                stepOutput.SignalSignature.IsChecked = False
            End If
        Next
    End Sub
    ''' <summary>
    ''' Go down a tree to determine nodes checking status
    ''' </summary>
    ''' <param name="groups"></param>
    Private Sub _determineParentGroupedByTypeNodeStatus(groups As ObservableCollection(Of SignalTypeHierachy))
        If groups.Count > 0 Then
            For Each group In groups
                ' if has children, then its status depends on children status
                If group.SignalList.Count > 0 Then
                    For Each subgroup In group.SignalList
                        If subgroup.SignalList.Count > 0 Then
                            For Each subsubgroup In subgroup.SignalList
                                If subsubgroup.SignalList.Count > 0 Then
                                    For Each subsubsubgroup In subsubgroup.SignalList
                                        If subsubsubgroup.SignalList.Count > 0 Then
                                            _determineParentCheckStatus(subsubsubgroup)
                                        End If
                                    Next
                                    _determineParentCheckStatus(subsubgroup)
                                End If
                            Next
                            _determineParentCheckStatus(subgroup)
                        End If
                    Next
                    _determineParentCheckStatus(group)
                Else
                    ' else, no children, status must be false, this only applies top level nodes, since leaf node won't have children at all
                    group.SignalSignature.IsChecked = False
                End If
            Next
        End If
    End Sub
    ''' <summary>
    ''' This sub loop through all children of a hierachy node to determine the node's status of checked/unchecked/indeterminate
    ''' </summary>
    ''' <param name="group"></param>
    Private Sub _determineParentCheckStatus(group As SignalTypeHierachy)
        If group.SignalList.Count > 0 Then
            Dim hasCheckedItem = False
            Dim hasUnCheckedItem = False
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.IsChecked Is Nothing Then
                    hasCheckedItem = True
                    hasUnCheckedItem = True
                    Exit For
                End If
                If subgroup.SignalSignature.IsChecked And Not hasCheckedItem Then
                    hasCheckedItem = True
                    Continue For
                End If
                If subgroup.SignalSignature.IsChecked = False And Not hasUnCheckedItem Then
                    hasUnCheckedItem = True
                End If
                If hasCheckedItem And hasUnCheckedItem Then
                    Exit For
                End If
            Next
            If hasCheckedItem And hasUnCheckedItem Then
                group.SignalSignature.IsChecked = Nothing
            Else
                group.SignalSignature.IsChecked = hasCheckedItem
            End If
        End If
    End Sub

    ''' <summary>
    ''' Check and decide if a file directory and its sub grouped signal is checkable or not depends on other file directory check status
    ''' </summary>
    Private Sub _determineFileDirCheckableStatus()
        Dim disableOthers = False
        For Each group In GroupedRawSignalsByType
            If group.SignalSignature.IsChecked Or group.SignalSignature.IsChecked Is Nothing Then
                disableOthers = True
            End If
        Next
        If disableOthers Then
            For Each group In GroupedRawSignalsByType
                If Not group.SignalSignature.IsChecked Then
                    group.SignalSignature.IsEnabled = False
                Else
                    group.SignalSignature.IsEnabled = True
                End If
            Next
            For Each group In GroupedRawSignalsByPMU
                If Not group.SignalSignature.IsChecked Then
                    group.SignalSignature.IsEnabled = False
                Else
                    group.SignalSignature.IsEnabled = True
                End If
            Next
        Else
            For Each group In GroupedRawSignalsByType
                group.SignalSignature.IsEnabled = True
            Next
            For Each group In GroupedRawSignalsByPMU
                group.SignalSignature.IsEnabled = True
            Next
        End If
    End Sub

#Region "Step manipulation: Add a step"
    Private _dqfilterSelected As ICommand
    Public Property DQFilterSelected As ICommand
        Get
            Return _dqfilterSelected
        End Get
        Set(ByVal value As ICommand)
            _dqfilterSelected = value
        End Set
    End Property
    Private Sub _dqfilterSelection(obj As Object)
        Dim newFilter As New DQFilter
        newFilter.Name = obj.ToString
        newFilter.StepCounter = DataConfigure.CollectionOfSteps.Count + 1
        newFilter.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newFilter.StepCounter.ToString & "-" & newFilter.Name
        newFilter.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newFilter.StepCounter.ToString & "-" & newFilter.Name
        'GroupedSignalByStepsInput.Add(newFilter.ThisStepInputsAsSignalHerachyByType)
        'GroupedSignalByDataConfigStepsOutput.Add(newFilter.ThisStepOutputsAsSignalHierachyByPMU)

        For Each parameter In DataConfigure.DQFilterNameParametersDictionary(newFilter.Name)
            If parameter = "SetToNaN" Or parameter = "FlagAllByFreq" Then
                newFilter.FilterParameters.Add(New ParameterValuePair(parameter, False))
            ElseIf newFilter.Name = "Nominal-Value Frequency Data Quality Filter" And parameter = "FlagBit" Then
                newFilter.FilterParameters.Add(New ParameterValuePair(parameter, False, False))
            Else
                newFilter.FilterParameters.Add(New ParameterValuePair(parameter, ""))
            End If
        Next
        'newFilter.Parameters
        DataConfigure.CollectionOfSteps.Add(newFilter)
        _stepSelectedToEdit(newFilter)
    End Sub
    Private _customizationSelected As ICommand
    Public Property CustomizationSelected As ICommand
        Get
            Return _customizationSelected
        End Get
        Set(ByVal value As ICommand)
            _customizationSelected = value
        End Set
    End Property
    Private Sub _customizationStepSelection(obj As Object)
        Dim newCustomization As New Customization
        newCustomization.Name = obj(0).ToString
        newCustomization.CustPMUname = _lastCustPMUname
        Try
            Select Case newCustomization.Name
                Case "Scalar Repetition Customization"
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, "C")
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                Case "Addition Customization", "Multiplication Customization"
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, "C")
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                Case "Subtraction Customization", "Division Customization"
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, "C")
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                    Dim dummy = New SignalSignatures("PleaseAddASignal", "PleaseAddASignal")
                    dummy.IsValid = False
                    newCustomization.MinuendOrDividend = dummy
                    newCustomization.SubtrahendOrDivisor = dummy
                Case "Raise signals to an exponent"
                    newCustomization.Exponent = "1"
                Case "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Return angle of complex valued signals", "Take complex conjugate of signals", "Phasor Creation Customization", "Metric Prefix Customization", "Angle Conversion Customization"
                    'PASS
                Case "Power Calculation Customization"
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, newCustomization.PowType.ToString)
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                    Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(newSignal, New ObservableCollection(Of SignalSignatures))
                    newCustomization.OutputInputMappingPair.Add(newPair)
                Case "Specify Signal Type and Unit Customization"
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, "C")
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                    Dim dummy = New SignalSignatures("NeedInputSignal", newCustomization.CustPMUname, "OTHER")
                    dummy.IsValid = False
                    newCustomization.InputChannels.Add(dummy)
                Case Else
                    Throw New Exception("Customization step not supported!")
            End Select
        Catch ex As Exception
            MessageBox.Show("Error selecting signal(s) for customization step!" & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
        newCustomization.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(newCustomization.OutputChannels)
        'GroupedSignalByDataConfigStepsInput.Add(newCustomization.ThisStepInputsAsSignalHerachyByType)
        'GroupedSignalByDataConfigStepsOutput.Add(newCustomization.ThisStepOutputsAsSignalHierachyByPMU)
        'newCustomization.IsStepSelected = True
        If obj(1) = "DataConfig" Then
            newCustomization.StepCounter = DataConfigure.CollectionOfSteps.Count + 1
            DataConfigure.CollectionOfSteps.Add(newCustomization)
            newCustomization.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newCustomization.StepCounter.ToString & "-" & newCustomization.Name
            newCustomization.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newCustomization.StepCounter.ToString & "-" & newCustomization.Name
            _stepSelectedToEdit(newCustomization)
        Else
            newCustomization.StepCounter = PostProcessConfigure.CollectionOfSteps.Count + 1
            PostProcessConfigure.CollectionOfSteps.Add(newCustomization)
            newCustomization.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newCustomization.StepCounter.ToString & "-" & newCustomization.Name
            newCustomization.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newCustomization.StepCounter.ToString & "-" & newCustomization.Name
            _postProcessConfigureStepSelected(newCustomization)
        End If
    End Sub
    Private _choosePhasorForPowerCalculation As ICommand
    Public Property ChoosePhasorForPowerCalculation As ICommand
        Get
            Return _choosePhasorForPowerCalculation
        End Get
        Set(ByVal value As ICommand)
            _choosePhasorForPowerCalculation = value
            OnPropertyChanged()
        End Set
    End Property
    Private Sub _powerCalculationPhasorOption(obj As Object)
        Dim vPhasor = New SignalSignatures("NeedVoltagePhasor", _currentSelectedStep.CustPMUname)
        vPhasor.IsValid = False
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(vPhasor)
        Dim iPhasor = New SignalSignatures("NeedCurrentPhasor", _currentSelectedStep.CustPMUname)
        iPhasor.IsValid = False
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(iPhasor)
        _disableEnableAllButPhasorSignalsInDataConfig(False)
    End Sub
    Private _chooseMagAngForPowerCalculation As ICommand
    Public Property ChooseMagAngForPowerCalculation As ICommand
        Get
            Return _chooseMagAngForPowerCalculation
        End Get
        Set(ByVal value As ICommand)
            _chooseMagAngForPowerCalculation = value
            OnPropertyChanged()
        End Set
    End Property
    Private Sub _powerCalculationMagAngOption(obj As Object)
        Dim vMag = New SignalSignatures("NeedVoltageMag", _currentSelectedStep.CustPMUname)
        vMag.IsValid = False
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(vMag)
        Dim vAng = New SignalSignatures("NeedVoltageAng", _currentSelectedStep.CustPMUname)
        vAng.IsValid = False
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(vAng)
        Dim iMag = New SignalSignatures("NeedCurrentMag", _currentSelectedStep.CustPMUname)
        iMag.IsValid = False
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(iMag)
        Dim iAng = New SignalSignatures("NeedCurrentAng", _currentSelectedStep.CustPMUname)
        iAng.IsValid = False
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(iAng)
        _disableEnableAllButMagnitudeSignalsInDataConfig(False)
    End Sub

#End Region

#Region "Step Manipulations: Delete, Select or DeSelect"
    Private _currentSelectedStep As Object
    Public Property CurrentSelectedStep As Object
        Get
            Return _currentSelectedStep
        End Get
        Set(ByVal value As Object)
            _currentSelectedStep = value
            OnPropertyChanged()
        End Set
    End Property

    Private _dataConfigStepSelected As ICommand
    Public Property DataConfigStepSelected As ICommand
        Get
            Return _dataConfigStepSelected
        End Get
        Set(ByVal value As ICommand)
            _dataConfigStepSelected = value
        End Set
    End Property
    Private Sub _stepSelectedToEdit(processStep As Object)
        ' if processStep is already selected, then the selection is not changed, nothing needs to be done.
        ' however, if processStep is not selected, which means a new selection, we need to find the old selection, unselect it and all it's input signal
        If Not processStep.IsStepSelected Then
            Try
                Dim lastNumberOfSteps = processStep.StepCounter
                Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
                Dim selectedFound = False
                For Each stp In DataConfigure.CollectionOfSteps
                    If stp.IsStepSelected Then
                        stp.IsStepSelected = False
                        For Each signal In stp.InputChannels
                            signal.IsChecked = False
                        Next
                        'If TypeOf (stp) Is Customization Then
                        '    For Each signal In stp.OutputChannels
                        '        signal.IsChecked = False
                        '    Next
                        'End If
                        'Exit For
                        selectedFound = True
                    End If
                    If stp.StepCounter < lastNumberOfSteps Then
                        If TypeOf (stp) Is Customization Then
                            stp.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(stp.InputChannels)
                            stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                        End If
                        stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(stp.OutputChannels)
                        stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    If stp.StepCounter >= lastNumberOfSteps AndAlso selectedFound Then
                        Exit For
                    End If
                Next
                _determineFileDirCheckableStatus()
                processStep.IsStepSelected = True

                For Each signal In processStep.InputChannels
                    signal.IsChecked = True
                Next

                If processStep.Name = "Scalar Repetition Customization" Then
                    SignalSelectionTreeViewVisibility = "Collapsed"
                Else
                    SignalSelectionTreeViewVisibility = "Visible"
                End If

                'If CurrentSelectedStep.Name = "Power Calculation Customization" AndAlso CurrentSelectedStep.OutputInputMappingPair(0).Value.Count = 2 Then
                '    _disableEnableAllButPhasorSignals(True)
                'End If
                If CurrentSelectedStep IsNot Nothing Then
                    If CurrentSelectedStep.Name = "Phasor Creation Customization" Then
                        _disableEnableAllButMagnitudeSignalsInDataConfig(True)
                    ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
                        If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
                            Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
                            If situation = 4 Then
                                _disableEnableAllButMagnitudeSignalsInDataConfig(True)
                            ElseIf situation = 2 Then
                                _disableEnableAllButPhasorSignalsInDataConfig(True)
                            End If
                        End If
                    ElseIf CurrentSelectedStep.Name = "Metric Prefix Customization" Then
                        _disableEnableAllButMagnitudeFrequencyROCOFSignalsInDataConfig(True)
                    ElseIf CurrentSelectedStep.Name = "Angle Conversion Customization" Then
                        _disableEnableAllButAngleSignalsInDataConfig(True)
                    End If
                End If

                GroupedSignalByDataConfigStepsInput = stepsInputAsSignalHierachy
                GroupedSignalByDataConfigStepsOutput = stepsOutputAsSignalHierachy
                _dataConfigDetermineAllParentNodeStatus()
                _determineFileDirCheckableStatus()

                If processStep.Name = "Phasor Creation Customization" Then
                    '_disableEnableGroupForPhasorCreationCustomization(GroupedSignalsByType, False)
                    '_disableEnableGroupForPhasorCreationCustomization(GroupedSignalByStepsInput, False)
                    '_disableEnableGroupForPhasorCreationCustomization(GroupedSignalByStepsOutput, False)
                    _disableEnableAllButMagnitudeSignalsInDataConfig(False)
                ElseIf processStep.Name = "Power Calculation Customization" Then
                    If processStep.OutputInputMappingPair.Count > 0 Then
                        Dim situation = processStep.OutputInputMappingPair(0).Value.Count
                        If situation = 4 Then
                            _disableEnableAllButMagnitudeSignalsInDataConfig(False)
                        ElseIf situation = 2 Then
                            _disableEnableAllButPhasorSignalsInDataConfig(False)
                        End If
                    End If
                ElseIf processStep.Name = "Metric Prefix Customization" Then
                    _disableEnableAllButMagnitudeFrequencyROCOFSignalsInDataConfig(False)
                ElseIf processStep.Name = "Angle Conversion Customization" Then
                    _disableEnableAllButAngleSignalsInDataConfig(False)
                End If
                CurrentSelectedStep = processStep
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' disable all but magnitude of current or magnitude of voltage signals
    ''' </summary>
    ''' <param name="isEnabled"></param>
    Private Sub _disableEnableAllButMagnitudeSignalsInDataConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedRawSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next

        'If CurrentTabIndex = 1 Then
        For Each group In GroupedSignalByDataConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
        'ElseIf CurrentTabIndex = 3 Then
        '    For Each group In GroupedSignalByPostProcessConfigStepsInput
        '        For Each subgroup In group.SignalList
        '            If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
        '                subgroup.SignalSignature.IsEnabled = isEnabled
        '            Else
        '                For Each subsubgroup In subgroup.SignalList
        '                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
        '                        subsubgroup.SignalSignature.IsEnabled = isEnabled
        '                    End If
        '                Next
        '            End If
        '        Next
        '    Next
        '    For Each group In GroupedSignalByPostProcessConfigStepsOutput
        '        For Each subgroup In group.SignalList
        '            For Each subsubgroup In subgroup.SignalList
        '                If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
        '                    subsubgroup.SignalSignature.IsEnabled = isEnabled
        '                End If
        '            Next
        '        Next
        '    Next


        'End If
    End Sub
    Private Sub _disableEnableAllButPhasorSignalsInDataConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            group.SignalSignature.IsEnabled = isEnabled
        Next
        For Each group In GroupedRawSignalsByPMU
            group.SignalSignature.IsEnabled = isEnabled
        Next
        For Each group In GroupedSignalByDataConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "P" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "P" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
    End Sub
    Private Sub _disableEnableAllButAngleSignalsInDataConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedRawSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "A" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "A" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
    End Sub
    Private Sub _disableEnableAllButMagnitudeFrequencyROCOFSignalsInDataConfig(isEnabled As Boolean)
        For Each group In GroupedRawSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedRawSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R" Then
                    subgroup.SignalSignature.IsEnabled = isEnabled
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
                            subsubgroup.SignalSignature.IsEnabled = isEnabled
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByDataConfigStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
                        subsubgroup.SignalSignature.IsEnabled = isEnabled
                    End If
                Next
            Next
        Next
    End Sub
    'Private Sub _disableEnableAllButCurrentVoltageSignals(isEnabled As Boolean)
    '    For Each group In GroupedRawSignalsByType
    '        For Each subgroup In group.SignalList
    '            If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V"
    '                subgroup.SignalSignature.IsEnabled = isEnabled
    '            End If
    '        Next
    '    Next
    '    For Each group In GroupedRawSignalsByPMU
    '        For Each subgroup In group.SignalList
    '            For Each subsubgroup In subgroup.SignalList
    '                If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Substring(0,1) <> "V" AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(0,1) <> "I") Then
    '                    subsubgroup.SignalSignature.IsEnabled = isEnabled
    '                End If
    '            Next
    '        Next
    '    Next
    '    For Each group In GroupedSignalByProcessConfigStepsInput
    '        For Each subgroup In group.SignalList
    '            If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "R" Then
    '                subgroup.SignalSignature.IsEnabled = isEnabled
    '            Else
    '                For Each subsubgroup In subgroup.SignalList
    '                    If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 2 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M") Then
    '                        subsubgroup.SignalSignature.IsEnabled = isEnabled
    '                    End If
    '                Next
    '            End If
    '        Next
    '    Next
    '    For Each group In GroupedSignalByProcessConfigStepsOutput
    '        For Each subgroup In group.SignalList
    '            For Each subsubgroup In subgroup.SignalList
    '                If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "F" AndAlso subsubgroup.SignalSignature.TypeAbbreviation <> "R") OrElse (subsubgroup.SignalSignature.TypeAbbreviation.Length = 3 AndAlso subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M") Then
    '                    subsubgroup.SignalSignature.IsEnabled = isEnabled
    '                End If
    '            Next
    '        Next
    '    Next
    'End Sub

    Private _signalSelectionTreeViewVisibility As String
    Public Property SignalSelectionTreeViewVisibility As String
        Get
            Return _signalSelectionTreeViewVisibility
        End Get
        Set(ByVal value As String)
            _signalSelectionTreeViewVisibility = value
            OnPropertyChanged()
        End Set
    End Property

    Private _dataConfigStepDeSelected As ICommand
    Public Property DataConfigStepDeSelected As ICommand
        Get
            Return _dataConfigStepDeSelected
        End Get
        Set(ByVal value As ICommand)
            _dataConfigStepDeSelected = value
        End Set
    End Property
    ''' <summary>
    ''' When user click outside the step list and none of the steps should be selected, then we need to uncheck all checkboxes
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _deSelectAllDataConfigSteps()
        If _currentSelectedStep IsNot Nothing Then
            For Each signal In _currentSelectedStep.InputChannels
                signal.IsChecked = False
            Next
            'If _currentSelectedStep.OutputChannels IsNot Nothing Then
            '    For Each signal In _currentSelectedStep.OutputChannels
            '        signal.IsChecked = False
            '    Next
            'End If
            Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            For Each stp In DataConfigure.CollectionOfSteps
                If TypeOf (stp) Is Customization Then
                    stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                End If
                stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
            Next
            GroupedSignalByDataConfigStepsInput = stepsInputAsSignalHierachy
            GroupedSignalByDataConfigStepsOutput = stepsOutputAsSignalHierachy
            If CurrentSelectedStep.Name = "Phasor Creation Customization" Then
                _disableEnableAllButMagnitudeSignalsInDataConfig(True)
            ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
                If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
                    Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
                    If situation = 4 Then
                        _disableEnableAllButMagnitudeSignalsInDataConfig(True)
                    Else
                        _disableEnableAllButPhasorSignalsInDataConfig(True)
                    End If
                End If
            ElseIf CurrentSelectedStep.Name = "Metric Prefix Customization" Then
                _disableEnableAllButMagnitudeFrequencyROCOFSignalsInDataConfig(True)
            ElseIf CurrentSelectedStep.Name = "Angle Conversion Customization" Then
                _disableEnableAllButAngleSignalsInDataConfig(True)
            End If
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByDataConfigStepsInput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByDataConfigStepsOutput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedRawSignalsByType, False)
            _currentSelectedStep.IsStepSelected = False
            CurrentSelectedStep = Nothing

            '_dataConfigDetermineAllParentNodeStatus()

            _determineFileDirCheckableStatus()
        End If
        SignalSelectionTreeViewVisibility = "Visible"
    End Sub
    Private Sub _changeCheckStatusAllParentsOfGroupedSignal(groups As ObservableCollection(Of SignalTypeHierachy), checkStatus As Boolean)
        For Each node In groups
            node.SignalSignature.IsChecked = checkStatus
            For Each child In node.SignalList
                child.SignalSignature.IsChecked = checkStatus
                For Each grandChild In child.SignalList
                    grandChild.SignalSignature.IsChecked = checkStatus
                    For Each greatGrandChild In grandChild.SignalList
                        greatGrandChild.SignalSignature.IsChecked = checkStatus
                    Next
                Next
            Next
        Next
    End Sub

    Private _deleteDataConfigStep As ICommand
    Public Property DeleteDataConfigStep As ICommand
        Get
            Return _deleteDataConfigStep
        End Get
        Set(ByVal value As ICommand)
            _deleteDataConfigStep = value
        End Set
    End Property
    Private Sub _deleteADataConfigStep(obj As Object)
        'Dim toBeDeleted = obj
        Try
            DataConfigure.CollectionOfSteps.Remove(obj)
            Dim steps = New ObservableCollection(Of Object)(DataConfigure.CollectionOfSteps)
            ' First find the step to be deleted
            'For Each stp In steps
            '    If stp.StepCounter = obj.StepCounter Then
            '        toBeDeleted = stp
            '        Exit For
            '    End If
            'Next
            ' if the step is found
            'If toBeDeleted IsNot Nothing Then
            'Dim stepInputHierachy = New ObservableCollection(Of SignalTypeHierachy)
            'Dim stepOutputHierachy = New ObservableCollection(Of SignalTypeHierachy)

            ' go through each step to change names that affected by the deleted step
            For Each stp In steps
                If stp.StepCounter > obj.StepCounter Then
                    stp.StepCounter -= 1
                    If TypeOf (stp) Is Customization Then
                        stp.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stp.StepCounter.ToString & "-" & stp.Name
                    End If
                    stp.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stp.StepCounter.ToString & "-" & stp.Name
                End If
            Next
            'GroupedSignalByDataConfigStepsInput = stepInputHierachy
            'GroupedSignalByDataConfigStepsOutput = stepOutputHierachy
            _deSelectAllDataConfigSteps()
            'If CurrentSelectedStep.StepCounter = obj.StepCounter Then
            '    For Each signal In obj.InputChannels
            '        signal.IsChecked = False
            '    Next
            '    If CurrentSelectedStep.Name = "Phasor Creation Customization" Then
            '        _disableEnableAllButMagnitudeSignals(True)
            '    ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
            '        If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
            '            Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
            '            If situation = 4 Then
            '                _disableEnableAllButMagnitudeSignals(True)
            '            Else
            '                _disableEnableAllButPhasorSignals(True)
            '            End If
            '        End If
            '    ElseIf CurrentSelectedStep.Name = "Metric Prefix Customization" Then
            '        _disableEnableAllButMagnitudeFrequencyROCOFSignals(True)
            '    ElseIf CurrentSelectedStep.Name = "Angle Conversion Customization" Then
            '        _disableEnableAllButAngleSignalsInDataConfig(True)
            '    End If
            '    CurrentSelectedStep = Nothing
            '    _dataConfigDetermineAllParentNodeStatus()
            '    _determineFileDirCheckableStatus()
            'End If
            _addLog("Step " & obj.StepCounter & ", " & obj.Name & " is deleted!")
            DataConfigure.CollectionOfSteps = steps
            'toBeDeleted = Nothing
            SignalSelectionTreeViewVisibility = "Visible"
        Catch ex As Exception
            MessageBox.Show("Error deleting step " & obj.StepCounter.ToString & " in Data Configuration, " & obj.Name & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
        'Else
        '    MessageBox.Show("Step " & toBeDeleted.StepCounter.ToString & ", " & toBeDeleted.Name & " is not found!", "Error!", MessageBoxButtons.OK)
        'End If
    End Sub

#End Region

    Private _selectSignalMethods As List(Of String)
    Public Property SelectSignalMethods As List(Of String)
        Get
            Return _selectSignalMethods
        End Get
        Set(ByVal value As List(Of String))
            _selectSignalMethods = value
            OnPropertyChanged()
        End Set
    End Property

    Private _selectedSelectionMethod As String
    Public Property SelectedSelectionMethod As String
        Get
            Return _selectedSelectionMethod
        End Get
        Set(ByVal value As String)
            _selectedSelectionMethod = value
            OnPropertyChanged()
        End Set
    End Property

    Private _allPMUs As ObservableCollection(Of String)
    Public Property AllPMUs As ObservableCollection(Of String)
        Get
            Return _allPMUs
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            _allPMUs = value
            OnPropertyChanged()
        End Set
    End Property

    Private _timezoneList As ReadOnlyCollection(Of TimeZoneInfo)
    Public ReadOnly Property TimeZoneList As ReadOnlyCollection(Of TimeZoneInfo)
        Get
            Return _timezoneList
        End Get
    End Property

    Private _logs As ObservableCollection(Of String)
    Public Property Logs As ObservableCollection(Of String)
        Get
            Return _logs
        End Get
        Set(ByVal value As ObservableCollection(Of String))
            _logs = value
            OnPropertyChanged()
        End Set
    End Property
    Private Sub _addLog(log As String)
        Dim timeDate = DateTime.Now
        Logs.Add(timeDate.ToString & ": " & log)
    End Sub


    Private _addFileSource As ICommand
    Public Property AddFileSource As ICommand
        Get
            Return _addFileSource
        End Get
        Set(ByVal value As ICommand)
            _addFileSource = value
        End Set
    End Property
    Private Sub _addAFileSource(obj As Object)
        DataConfigure.ReaderProperty.InputFileInfos.Add(New InputFileInfo)
    End Sub
    Private _deleteThisFileSource As ICommand
    Public Property DeleteThisFileSource As ICommand
        Get
            Return _deleteThisFileSource
        End Get
        Set(ByVal value As ICommand)
            _deleteThisFileSource = value
        End Set
    End Property

    Private Sub _deleteAFileSource(obj As InputFileInfo)
        For Each source In DataConfigure.ReaderProperty.InputFileInfos
            If obj Is source Then

                For Each group In GroupedRawSignalsByType
                    If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                        GroupedRawSignalsByType.Remove(group)
                        Exit For
                    End If
                Next
                For Each group In GroupedRawSignalsByPMU
                    If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                        GroupedRawSignalsByPMU.Remove(group)
                        Exit For
                    End If
                Next
                'For Each stp In DataConfigure.CollectionOfSteps
                '    If stp.InputChannels.Count > 0 Then
                '        If obj.TaggedSignals.Contains(stp.InputChannels(0)) Then
                '            stp.InputChannels = New ObservableCollection(Of SignalSignatures)
                '            stp.ThisStepInputsAsSignalHerachyByType.SignalList = New ObservableCollection(Of SignalTypeHierachy)
                '            stp.ThisStepInputsAsSignalHerachyByType.SignalSignature.IsChecked = False
                '            'stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = New ObservableCollection(Of SignalTypeHierachy)
                '            'stp.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.IsChecked = False
                '            If TypeOf stp Is Customization Then
                '                For Each pair In stp.OutputInputMappingPair
                '                    Dim numberOfInput = pair.Value.Count
                '                    pair.Value.Clear()
                '                    For index = 0 To numberOfInput - 1
                '                        Dim empty = New SignalSignatures("NeedInput")
                '                        empty.IsValid = False
                '                        pair.Value.Add(empty)
                '                    Next
                '                Next
                '                stp.MinuendOrDividend = New SignalSignatures("NeedMinuendOrDivident")
                '                stp.MinuendOrDividend.IsValid = False
                '                stp.SubtrahendOrDivisor = New SignalSignatures("NeedSubtrahendOrDivisor")
                '                stp.SubtrahendOrDivisor.IsValid = False
                '            End If
                '            AllPMUs = New ObservableCollection(Of String)
                '            For Each group In GroupedSignalsByPMU
                '                For Each subgroup In group.SignalList
                '                    If Not AllPMUs.Contains(subgroup.SignalSignature.PMUName) Then
                '                        AllPMUs.Add(subgroup.SignalSignature.PMUName)
                '                    End If
                '                Next
                '            Next
                '        End If
                '    End If
                'Next
                DataConfigure.ReaderProperty.InputFileInfos.Remove(obj)
                Exit For
            End If
        Next
        If _configData IsNot Nothing Then
            _readDataConfigStages()
        End If
    End Sub

    Private _walkerStageChanged As ICommand
    Public Property WalkerStageChanged As ICommand
        Get
            Return _walkerStageChanged
        End Get
        Set(value As ICommand)
            _walkerStageChanged = value
        End Set
    End Property


    Private Sub _changeSignalSelectionDropDownChoice(index As Integer)
        Select Case index
            Case 0
                CurrentTabIndex = 0
            Case 1
                SelectSignalMethods = {"All Initial Input Channels by Signal Type",
                                       "All Initial Input Channels by PMU",
                                       "Input Channels by Step",
                                       "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "All Initial Input Channels by Signal Type"
                CurrentTabIndex = 1
            Case 2
                SelectSignalMethods = {"All Initial Input Channels by Signal Type",
                                       "All Initial Input Channels by PMU",
                                       "Output from SignalSelectionAndManipulation by Signal Type",
                                       "Output from SignalSelectionAndManipulation by PMU",
                                       "Input to MultiRate steps",
                                       "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "All Initial Input Channels by Signal Type"
                CurrentTabIndex = 2
            Case 3
                SelectSignalMethods = {"NOT Implemented Yet!",
                                        "All Initial Input Channels by Signal Type",
                                        "All Initial Input Channels by PMU",
                                        "Output from SignalSelectionAndManipulation by Signal Type",
                                        "Output from SignalSelectionAndManipulation by PMU",
                                        "Input to MultiRate steps",
                                        "Output Channels by Step"}.ToList
                'SelectedSelectionMethod = "NOT Implemented Yet!"
                CurrentTabIndex = 3
        End Select
    End Sub
    Private _oldTabIndex as Integer
    private _currentTabIndex as Integer
    Public Property CurrentTabIndex As Integer
        Get
            Return _currentTabIndex
        End Get
        Set(value As Integer)
            _currentTabIndex = value
            If _oldTabIndex = 1 And _currentTabIndex <> 1 Then
                _groupAllDataConfigOutputSignal()
                _deSelectAllDataConfigSteps()
            End If
            If _oldTabIndex = 2 And _currentTabIndex <> 2 Then
                _groupAllProcessConfigOutputSignal()
                _deSelectAllProcessConfigSteps()
            End If
            If _oldTabIndex = 3 And _currentTabIndex <> 3 Then
                _groupAllPostProcessConfigOutputSignal()
                _deSelectAllPostProcessConfigSteps()
            End If
            _oldTabIndex = _currentTabIndex
            OnPropertyChanged()
        End Set
    End Property

    Private Sub _groupAllDataConfigOutputSignal()
        Dim allOutputSignals = New ObservableCollection(Of SignalSignatures)
        For Each stp In DataConfigure.CollectionOfSteps
            For Each signal In stp.OutputChannels
                If Not allOutputSignals.Contains(signal) Then
                    allOutputSignals.Add(signal)
                End If
            Next
        Next
        AllDataConfigOutputGroupedByType = SortSignalByType(allOutputSignals)
        AllDataConfigOutputGroupedByPMU = SortSignalByPMU(allOutputSignals)
    End Sub


    'Private _postProcessingSelected As ICommand
    'Public Property PostProcessingSelected As ICommand
    '    Get
    '        Return _postProcessingSelected
    '    End Get
    '    Set(value As ICommand)
    '        _postProcessingSelected = value
    '    End Set
    'End Property
    'Private Sub _selectPostProcessing(obj As Object)

    'End Sub
End Class
