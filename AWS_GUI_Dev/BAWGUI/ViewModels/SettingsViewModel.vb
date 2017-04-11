Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Forms
Imports BAWGUI
Imports PDAT_Reader
Imports System.Linq

'Imports BAWGUI.DataConfig

Public Class SettingsViewModel
    Inherits ViewModelBase

    Public Sub New()
        _configFileName = ""
        _lastCustPMUname = ""
        '_sampleFile = ""

        _dataConfigure = New DataConfig
        _processConfigure = New ProcessConfig
        _detectorConfigure = New DetectorConfig
        _logs = New ObservableCollection(Of String)

        _openConfigFile = New DelegateCommand(AddressOf openConfigXMLFile, AddressOf CanExecute)
        _browseInputFileDir = New DelegateCommand(AddressOf _browseInputFileFolder, AddressOf CanExecute)
        _fileTypeChanged = New DelegateCommand(AddressOf _buildInputFileFolderTree, AddressOf CanExecute)
        _dqfilterSelected = New DelegateCommand(AddressOf _dqfilterSelection, AddressOf CanExecute)
        _customizationSelected = New DelegateCommand(AddressOf _customizationStepSelection, AddressOf CanExecute)
        _selectedSignalChanged = New DelegateCommand(AddressOf _signalSelected, AddressOf CanExecute)
        _stepSelected = New DelegateCommand(AddressOf _stepSelectedToEdit, AddressOf CanExecute)
        _stepDeSelected = New DelegateCommand(AddressOf _deSelectAllSteps, AddressOf CanExecute)
        _setCurrentFocusedTextbox = New DelegateCommand(AddressOf _currentFocusedTextBoxChanged, AddressOf CanExecute)
        _setCurrentFocusedTextboxUnarySteps = New DelegateCommand(AddressOf _currentFocusedTextBoxForUnaryStepsChanged, AddressOf CanExecute)
        '_selectedOutputSignalChanged = New DelegateCommand(AddressOf _outputSignalSelectionChanged, AddressOf CanExecute)
        _textboxesLostFocus = New DelegateCommand(AddressOf _recoverCheckStatusOfCurrentStep, AddressOf CanExecute)
        _deleteThisStep = New DelegateCommand(AddressOf _deleteAStep, AddressOf CanExecute)
        _powerPhasorTextBoxGotFocus = New DelegateCommand(AddressOf _powerPhasorCurrentFocusedTextbox, AddressOf CanExecute)
        _choosePhasorForPowerCalculation = New DelegateCommand(AddressOf _powerCalculationPhasorOption, AddressOf CanExecute)
        _chooseMagAngForPowerCalculation = New DelegateCommand(AddressOf _powerCalculationMagAngOption, AddressOf CanExecute)
        _addFileSource = New DelegateCommand(AddressOf _addAFileSource, AddressOf CanExecute)
        _deleteThisFileSource = New DelegateCommand(AddressOf _deleteAFileSource, AddressOf CanExecute)

        '_inputFileDirTree = New ObservableCollection(Of Folder)
        _groupedSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByStepsInput = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
        _allPMUs = New ObservableCollection(Of String)

        _timezoneList = TimeZoneInfo.GetSystemTimeZones
        _signalSelectionTreeViewVisibility = "Visible"
        _selectSignalMethods = {"All Raw Input Channels by Signal Type",
                                "All Raw Input Channels by PMU",
                                "Input Channels by Step",
                                "OutPut Channels by Step"}.ToList
        _selectedSelectionMethod = "All Raw Input Channels by Signal Type"
        _powerTypeDictionary = New Dictionary(Of String, String) From {{"Complex", "CP"}, {"Apparent", "S"}, {"Active", "P"}, {"Reactive", "Q"}}

    End Sub


    Private _groupedSignalsByType As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalsByType As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalsByType
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalsByType = value
            OnPropertyChanged()
        End Set
    End Property

    Private _pmuSignalDictionary As Dictionary(Of String, List(Of SignalSignatures))
    Public Property PMUSignalDictionary As Dictionary(Of String, List(Of SignalSignatures))
        Get
            Return _pmuSignalDictionary
        End Get
        Set(ByVal value As Dictionary(Of String, List(Of SignalSignatures)))
            _pmuSignalDictionary = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedSignalsByPMU As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalsByPMU As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalsByPMU
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalsByPMU = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedSignalByStepsInput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByStepsInput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByStepsInput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByStepsInput = value
            OnPropertyChanged()
        End Set
    End Property
    Private _groupedSignalByStepsOutput As ObservableCollection(Of SignalTypeHierachy)
    Public Property GroupedSignalByStepsOutput As ObservableCollection(Of SignalTypeHierachy)
        Get
            Return _groupedSignalByStepsOutput
        End Get
        Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
            _groupedSignalByStepsOutput = value
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
        GroupedSignalsByPMU.Add(a)
        fileInfo.GroupedSignalsByType = SortSignalByType(newSignalList)
        Dim b = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
        b.SignalList = fileInfo.GroupedSignalsByType
        GroupedSignalsByType.Add(b)
    End Sub

    Private Function SortSignalByType(signalList As ObservableCollection(Of SignalSignatures)) As ObservableCollection(Of SignalTypeHierachy)
        Dim signalTypeTree As New ObservableCollection(Of SignalTypeHierachy)
        Dim signalTypeDictionary = signalList.GroupBy(Function(x) x.TypeAbbreviation.ToArray(0).ToString).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
        For Each signalType In signalTypeDictionary
            Select Case signalType.Key
                Case "S"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Apparent"))
                    newHierachy.SignalSignature.TypeAbbreviation = "S"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "O"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Other"))
                    newHierachy.SignalSignature.TypeAbbreviation = "O"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "C"
                    Dim groups = signalType.Value.GroupBy(Function(x) x.TypeAbbreviation).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
                    For Each group In groups
                        Select Case group.Key
                            Case "C"
                                Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("CustomizedSignal"))
                                newHierachy.SignalSignature.TypeAbbreviation = "C"
                                For Each signal In signalType.Value
                                    newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                                Next
                                signalTypeTree.Add(newHierachy)
                            Case "CP"
                                Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Complex"))
                                newHierachy.SignalSignature.TypeAbbreviation = "CP"
                                For Each signal In signalType.Value
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
        PMUSignalDictionary = signalList.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
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
            Dim type = ""
            Dim signalName = ""
            Dim signalList = New List(Of String)
            Dim signalSignatureList = New ObservableCollection(Of SignalSignatures)
            For index = 0 To signalNames.Count - 1
                Dim newSignal = New SignalSignatures
                newSignal.PMUName = pmuName
                Select Case signalTypes(index)
                    Case "VPM"
                        'signalName = signalNames(index).Split(".")(0) & ".VMP"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "VMP"
                    Case "VPA"
                        'signalName = signalNames(index).Split(".")(0) & ".VAP"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "VAP"
                    Case "IPM"
                        'signalName = signalNames(index).Split(".")(0) & ".IMP"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "IMP"
                    Case "IPA"
                        'signalName = signalNames(index).Split(".")(0) & ".IAP"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "IAP"
                    Case "F"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "F"
                    Case "P"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "P"
                    Case "Q"
                        signalName = signalNames(index)
                        newSignal.TypeAbbreviation = "Q"
                    Case Else
                        Throw New Exception("Error! Invalid signal type " & signalTypes(index) & " found in file: " & sampleFile & " !")
                End Select
                newSignal.SignalName = signalName
                signalList.Add(signalName)
                signalSignatureList.Add(newSignal)
            Next
            fileInfo.SignalList = signalList
            fileInfo.TaggedSignals = signalSignatureList
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
            fileInfo.GroupedSignalsByPMU = SortSignalByPMU(signalSignatureList)
            Dim a = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
            a.SignalList = fileInfo.GroupedSignalsByPMU
            GroupedSignalsByPMU.Add(a)
            fileInfo.GroupedSignalsByType = SortSignalByType(signalSignatureList)
            Dim b = New SignalTypeHierachy(New SignalSignatures(fileInfo.FileDirectory & ", Sampling Rate: " & fileInfo.SamplingRate & "/Second"))
            b.SignalList = fileInfo.GroupedSignalsByType
            GroupedSignalsByType.Add(b)
        Else
            Dim PDATSampleFile As New PDATReader
            fileInfo.SignalList = PDATSampleFile.GetPDATSignalNameList(sampleFile)
            fileInfo.SamplingRate = PDATSampleFile.GetSamplingRate()
        End If
    End Sub

    'Private _signalList As List(Of String)
    'Public Property SignalList As List(Of String)
    '    Get
    '        Return _signalList
    '    End Get
    '    Set(value As List(Of String))
    '        _signalList = value
    '        OnPropertyChanged("SignalList")
    '    End Set
    'End Property

    Private _lastInputFolderLocation As String

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
            OnPropertyChanged("DataConfigure")
        End Set
    End Property

    Private _processConfigure As ProcessConfig
    Public Property ProcessConfigure As ProcessConfig
        Get
            Return _processConfigure
        End Get
        Set(ByVal value As ProcessConfig)
            _processConfigure = value
            OnPropertyChanged("ProcessConfigure")
        End Set
    End Property

    Private _detectorConfigure As DetectorConfig
    Public Property DetectorConfigure As DetectorConfig
        Get
            Return _detectorConfigure
        End Get
        Set(ByVal value As DetectorConfig)
            _detectorConfigure = value
            OnPropertyChanged("Detectorconfigure")
        End Set
    End Property

    Private _inputFileDirTree As ObservableCollection(Of Folder)
    Public Property InputFileDirTree As ObservableCollection(Of Folder)
        Get
            Return _inputFileDirTree
        End Get
        Set(ByVal value As ObservableCollection(Of Folder))
            _inputFileDirTree = value
            OnPropertyChanged("InputFileDirTree")
        End Set
    End Property

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
            GroupedSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
            GroupedSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
            Try
                _configData = XDocument.Load(_configFileName)
                _readConfigFile()
            Catch ex As Exception
                MessageBox.Show("Error reading config file!" & vbCrLf & ex.Message & vbCrLf & "Please see logs below!", "Error!", MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    Private Sub _readConfigFile()
        'Dim _sampleFile = ""
        _addLog("Reading " & ConfigFileName)
        Dim fileInfo = New InputFileInfo
        Dim fileInfoList = New ObservableCollection(Of InputFileInfo)
        fileInfo.FileDirectory = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<FileDirectory>.Value
        Dim fileType = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<FileType>.Value.ToLower
        fileInfo.FileType = [Enum].Parse(GetType(DataFileType), fileType)
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
        _readStages()

        _addLog("Done reading " & ConfigFileName & " .")
    End Sub
    Private Sub _readStages()
        Dim CollectionOfSteps As New ObservableCollection(Of Object)
        'Dim stepsAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
        'Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
        Dim stepCounter As Integer = 0
        Dim stages = From el In _configData.<Config>.<DataConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
        For Each el In stages
            Dim steps = From element In _configData.<Config>.<DataConfig>.<Configuration>.<Stages>.Elements Select element
            For Each element In steps
                Dim aStep As Object
                Dim necessaryParams As New List(Of String)
                Dim outputInputNameDictionary = New Dictionary(Of String, List(Of SignalSignatures))

                If element.Name = "Filter" Then
                    aStep = New DQFilter
                    aStep.Name = DataConfigure.DQFilterReverseNameDictionary(element.<Name>.Value)
                    necessaryParams.AddRange(DataConfigure.DQFilterNameParametersDictionary(aStep.Name))
                ElseIf element.Name = "Customization" Then
                    aStep = New Customization
                    aStep.Name = DataConfigure.CustomizationReverseNameDictionary(element.<Name>.Value)
                    necessaryParams.AddRange(DataConfigure.CustomizationNameParemetersDictionary(aStep.Name))
                End If
                stepCounter += 1
                aStep.StepCounter = stepCounter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
                If TypeOf (aStep) Is Customization Then
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
                End If

                'Dim signalForUnitTypeSpecificationCustomization As SignalSignatures = Nothing
                Select Case aStep.Name
                    Case "Specify Signal Type and Unit Customization"
                        _readSpecTypeUnitCustomization(aStep, element.<Parameters>, CollectionOfSteps)
                    Case "Power Calculation Customization"
                        _readPowerCalculationCustomization(aStep, element.<Parameters>, CollectionOfSteps, stepCounter)
                    Case "Metric Prefix Customization", "Angle Conversion Customization"
                        _readMetricPrefixOrAngleConversionCustomization(aStep, element.<Parameters>, CollectionOfSteps)
                    Case Else
                        Dim params = From ps In element.<Parameters>.Elements Select ps
                        For Each pair In params
                            Dim aPair As New ParameterValuePair
                            Dim paraName = pair.Name.ToString
                            If TypeOf (aStep) Is Customization AndAlso (paraName = "term" Or paraName = "factor" Or paraName = "signal") Then
                                Dim signal = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
                                If signal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(signal) Then
                                    aStep.InputChannels.Add(signal)
                                ElseIf paraName = "signal" Then
                                    signal = New SignalSignatures("SignalNotFound", "")
                                    signal.IsValid = False
                                    _addLog("Error reading config file! Signal " & pair.<Channel>.Value & " in PMU " & pair.<PMU>.Value & " not found!")
                                End If
                                Dim custSignalName = pair.<CustName>.Value
                                If Not String.IsNullOrEmpty(custSignalName) Then
                                    If Not outputInputNameDictionary.ContainsKey(custSignalName) Then
                                        outputInputNameDictionary(custSignalName) = New List(Of SignalSignatures)
                                        If signal IsNot Nothing Then
                                            outputInputNameDictionary(custSignalName).Add(signal)
                                        End If
                                    Else
                                        Throw New Exception("Duplicate custom signal name " & custSignalName & " found in this step!")
                                    End If
                                End If
                            ElseIf TypeOf (aStep) Is Customization And (paraName = "minuend" Or paraName = "dividend") Then
                                Dim minuendOrDivident = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
                                If minuendOrDivident Is Nothing Then
                                    minuendOrDivident = New SignalSignatures("MinuentOrDividentNotFound")
                                    minuendOrDivident.IsValid = False
                                Else
                                    aStep.InputChannels.Add(minuendOrDivident)
                                End If
                                aStep.MinuendOrDivident = minuendOrDivident
                            ElseIf TypeOf (aStep) Is Customization And (paraName = "subtrahend" Or paraName = "divisor") Then
                                Dim subtrahendOrDivisor = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
                                If subtrahendOrDivisor Is Nothing Then
                                    subtrahendOrDivisor = New SignalSignatures("SubtrahendOrDivisorNotFound")
                                    subtrahendOrDivisor.IsValid = False
                                Else
                                    aStep.InputChannels.Add(subtrahendOrDivisor)
                                End If
                                aStep.SubtrahendOrDivisor = subtrahendOrDivisor
                            ElseIf TypeOf (aStep) Is Customization And paraName = "CustPMUname" Then
                                aStep.CustPMUname = pair.Value
                                _lastCustPMUname = pair.Value
                            ElseIf TypeOf (aStep) Is Customization And paraName = "SignalName" Then
                                aStep.CustSignalName.Add(pair.Value)
                            ElseIf TypeOf (aStep) Is Customization And paraName = "CustName" Then
                                aStep.CustSignalName.Add(pair.Value)
                            ElseIf TypeOf aStep Is Customization And paraName = "phasor" Then
                                Dim magSignal = _searchForSignalInTaggedSignals(pair.<mag>.<PMU>.Value, pair.<mag>.<Channel>.Value)
                                If magSignal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(magSignal) Then
                                    aStep.InputChannels.Add(magSignal)
                                Else
                                    magSignal = New SignalSignatures("ErrorReadingMag")
                                    magSignal.IsValid = False
                                    _addLog("Error reading config file! Signal " & pair.<mag>.<Channel>.Value & " in PMU " & pair.<mag>.<PMU>.Value & " not found!")
                                End If
                                Dim angSignal = _searchForSignalInTaggedSignals(pair.<ang>.<PMU>.Value, pair.<ang>.<Channel>.Value)
                                If angSignal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(angSignal) Then
                                    aStep.InputChannels.Add(angSignal)
                                Else
                                    angSignal = New SignalSignatures("ErrorReadingAng")
                                    angSignal.IsValid = False
                                    _addLog("Error reading config file! Signal " & pair.<ang>.<Channel>.Value & " in PMU " & pair.<ang>.<PMU>.Value & " not found!")
                                End If
                                Dim custSignalName = pair.<CustName>.Value
                                If Not String.IsNullOrEmpty(custSignalName) Then
                                    If Not outputInputNameDictionary.ContainsKey(custSignalName) Then
                                        outputInputNameDictionary(custSignalName) = New List(Of SignalSignatures)
                                        outputInputNameDictionary(custSignalName).Add(magSignal)
                                        outputInputNameDictionary(custSignalName).Add(angSignal)
                                    Else
                                        _addLog("Duplicate custom signal name " & custSignalName & " found in this step!")
                                        Throw New Exception("Duplicate custom signal name " & custSignalName & " found in this step!")
                                    End If
                                End If
                                'ElseIf TypeOf aStep Is Customization And paraName = "PMU" Then
                                '    If signalForUnitTypeSpecificationCustomization Is Nothing Then
                                '        signalForUnitTypeSpecificationCustomization = New SignalSignatures
                                '    End If
                                '    signalForUnitTypeSpecificationCustomization.PMUName = pair.Value
                                'ElseIf TypeOf aStep Is Customization And paraName = "Channel" Then
                                '    If signalForUnitTypeSpecificationCustomization Is Nothing Then
                                '        signalForUnitTypeSpecificationCustomization = New SignalSignatures
                                '    End If
                                '    signalForUnitTypeSpecificationCustomization.SignalName = pair.Value
                            ElseIf TypeOf aStep Is Customization And paraName = "exponent" Then
                                aStep.Exponent = pair.Value
                            Else
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
                                aStep.Parameters.Add(aPair)
                            End If
                            necessaryParams.Remove(paraName)
                        Next
                        'If signalForUnitTypeSpecificationCustomization IsNot Nothing Then
                        '    Dim signal = _searchForSignalInTaggedSignals(signalForUnitTypeSpecificationCustomization.PMUName, signalForUnitTypeSpecificationCustomization.SignalName)
                        '    If signal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(signal) Then
                        '        aStep.InputChannels.Add(signal)
                        '    Else
                        '        signal = New SignalSignatures("ErrorReadingMag")
                        '        signal.IsValid = False
                        '        _addLog("Error reading config file! Signal " & signalForUnitTypeSpecificationCustomization.SignalName & " in PMU " & signalForUnitTypeSpecificationCustomization.PMUName & " not found!")
                        '    End If
                        'End If
                        For Each parameter In necessaryParams
                            If parameter = "CustPMUname" Then
                                'aStep.Parameters.Add(New ParameterValuePair(parameter, _lastCustPMUname))
                                aStep.CustPMUname = _lastCustPMUname
                                'ElseIf parameter = "CustName" AndAlso signalForUnitTypeSpecificationCustomization IsNot Nothing Then
                                'aStep.CustSignalName.Add(signalForUnitTypeSpecificationCustomization.SignalName)
                                'ElseIf parameter = "PMU" Then
                            Else
                                aStep.Parameters.Add(New ParameterValuePair(parameter, ""))
                            End If
                        Next
                        If TypeOf (aStep) Is Customization AndAlso Not String.IsNullOrEmpty(aStep.CustPMUname) AndAlso aStep.CustSignalName.Count > 0 Then
                            For Each name In aStep.CustSignalName
                                Dim type = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "SignalType" Select x.Value).FirstOrDefault
                                If type Is Nothing Then
                                    type = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "SigType" Select x.Value).FirstOrDefault
                                End If
                                If type Is Nothing AndAlso aStep.InputChannels.count > 0 Then
                                    type = aStep.InputChannels(0).TypeAbbreviation
                                End If
                                If type IsNot Nothing Then
                                    Dim s = New SignalSignatures(name, aStep.CustPMUname, type)
                                    s.IsCustomSignal = True
                                    aStep.OutputChannels.Add(s)
                                Else
                                    Dim s = New SignalSignatures(name, aStep.CustPMUname)
                                    s.IsCustomSignal = True
                                    aStep.OutputChannels.Add(s)
                                End If
                            Next
                        End If
                        If TypeOf aStep Is Customization AndAlso outputInputNameDictionary.Count > 0 AndAlso Not String.IsNullOrEmpty(aStep.CustPMUname) Then
                            For Each pair In outputInputNameDictionary
                                Dim newOutputSignal = New SignalSignatures(pair.Key, aStep.CustPMUname, "C")
                                newOutputSignal.IsCustomSignal = True
                                Select Case aStep.Name
                                    Case "Addition Customization"
                                        newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case "Subtraction Customization"
                                        newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case "Multiplication Customization", "Division Customization", "Raise signals to an exponent"
                                        newOutputSignal.TypeAbbreviation = "O"
                                    Case "Reverse sign of signals"
                                        newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case "Take absolute value of signals"
                                        If pair.Value.Count > 0 Then
                                            newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                        End If
                                    Case "Return real component of signals"
                                        newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case "Return imaginary component of signals"
                                        newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case "Return angle of complex valued signals"

                                    Case "Take complex conjugate of signals"
                                        newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case "Phasor Creation Customization"
                                        If pair.Value.FirstOrDefault.IsValid Then
                                            newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation.Substring(0, 1) & "P" & pair.Value.FirstOrDefault.TypeAbbreviation.Substring(2, 1)
                                        End If
                                        'Case "Power Calculation Customization"

                                        'Case "Specify Signal Type and Unit Customization"

                                        'Case "Metric Prefix Customization"
                                        '    newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                        'Case "Angle Conversion Customization"
                                        '    newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                                    Case Else
                                        Throw New Exception("Customization step not supported!")
                                End Select
                                aStep.OutputChannels.Add(newOutputSignal)
                                Dim targetKey = (From y In DirectCast(aStep, Customization).OutputInputMappingPair Where y.Key = newOutputSignal Select y Distinct).ToList
                                If targetKey.Count = 0 Then
                                    Dim kvp = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(newOutputSignal, New ObservableCollection(Of SignalSignatures))
                                    'aStep.OutputInputMappingPair.Add(New KeyValuePair(Of 
                                    For Each signal In pair.Value
                                        kvp.Value.Add(signal)
                                    Next
                                    aStep.OutputInputMappingPair.Add(kvp)
                                Else
                                    Throw New Exception("Duplicate custom signal name " & pair.Key & " found in this step!")
                                End If
                            Next
                        End If
                        CollectionOfSteps.Add(aStep)
                        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                        'stepsAsSignalHierachy.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                        If TypeOf (aStep) Is Customization Then
                            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                        End If

                End Select
                GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
            Next
        Next
        DataConfigure.CollectionOfSteps = CollectionOfSteps
    End Sub

    Private _powerTypeDictionary As Dictionary(Of String, String)
    Private Sub _readPowerCalculationCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As ObservableCollection(Of Object), ByRef stepCounter As Integer)
        aStep.CustPMUname = params.<CustPMUname>.Value
        If aStep.CustPMUname Is Nothing Then
            aStep.CustPMUname = _lastCustPMUname
        End If
        aStep.PowType = [Enum].Parse(GetType(PowerType), params.<PowType>.Value)
        Dim powers = From el In params.Elements Where el.Name = "power" Select el
        For index = 0 To powers.Count - 1
            'For Each power In powers
            If index > 0 Then
                Dim oldStep = aStep
                aStep = New Customization(oldStep)
                stepCounter += 1
                aStep.StepCounter = stepCounter
                aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
                aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
            End If
            Dim signalName = powers(index).<CustName>.Value
            Dim typeAbbre = _powerTypeDictionary(aStep.PowType.ToString)
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
                End If
                aStep.InputChannels.Add(input)
            Next
            Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(output, New ObservableCollection(Of SignalSignatures))
            For Each signal In aStep.InputChannels
                newPair.Value.Add(signal)
            Next
            aStep.OutputInputMappingPair.Add(newPair)
            CollectionOfSteps.Add(aStep)
            aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
            GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
            GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        Next
    End Sub

    Private Sub _readMetricPrefixOrAngleConversionCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As ObservableCollection(Of Object))
        aStep.CustPMUname = params.<CustPMUname>.Value
        If aStep.CustPMUname Is Nothing Then
            aStep.CustPMUname = _lastCustPMUname
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
            End If
            Dim outputName = convert.<Custname>.Value
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
        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
        aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
        GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
        GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
    End Sub

    Private Sub _readSpecTypeUnitCustomization(aStep As Object, params As IEnumerable(Of XElement), CollectionOfSteps As ObservableCollection(Of Object))
        aStep.CustPMUname = params.<CustPMUname>.Value
        If aStep.CustPMUname Is Nothing Then
            aStep.CustPMUname = _lastCustPMUname
        End If
        Dim inputSignal = _searchForSignalInTaggedSignals(params.<PMU>.Value, params.<Channel>.Value)
        'Dim toConvert = From convert In params.Elements Where convert.Name = "ToConvert" Select convert
        'For Each convert In toConvert
        'Dim input = _searchForSignalInTaggedSignals(convert.<PMU>.Value, convert.<Channel>.Value)
        If inputSignal Is Nothing Then
            inputSignal = New SignalSignatures("SignalNotFound")
            inputSignal.IsValid = False
            inputSignal.TypeAbbreviation = "C"
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
        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
        aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
        GroupedSignalByStepsInput.Add(aStep.ThisStepInputsAsSignalHerachyByType)
        GroupedSignalByStepsOutput.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
    End Sub
    Private _lastCustPMUname As String

    Private Function _searchForSignalInTaggedSignals(pmu As String, channel As String) As SignalSignatures
        'Dim target = pmu & "." & channel.Split(".")(1) & "." & channel.Split(".")(2)
        For Each group In GroupedSignalsByPMU
            For Each p In group.SignalList
                If p.SignalSignature.PMUName = pmu Then
                    For Each signal In p.SignalList
                        If signal.SignalSignature.SignalName = channel Then
                            Return signal.SignalSignature
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByStepsOutput
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
            For Each group In GroupedSignalsByType
                If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                    GroupedSignalsByType.Remove(group)
                    Exit For
                End If
            Next
            For Each group In GroupedSignalsByPMU
                If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                    GroupedSignalsByPMU.Remove(group)
                    Exit For
                End If
            Next
            For Each stp In DataConfigure.CollectionOfSteps
                If stp.InputChannels.Count > 0 Then
                    If obj.TaggedSignals.Contains(stp.InputChannels(0)) Then
                        stp.InputChannels = New ObservableCollection(Of SignalSignatures)
                        stp.ThisStepInputsAsSignalHerachyByType.SignalList = New ObservableCollection(Of SignalTypeHierachy)
                        stp.ThisStepInputsAsSignalHerachyByType.SignalSignature.IsChecked = False
                        'stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = New ObservableCollection(Of SignalTypeHierachy)
                        'stp.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.IsChecked = False
                    End If
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
                _readStages()
            End If
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
        For Each parameter In DataConfigure.DQFilterNameParametersDictionary(newFilter.Name)
            If parameter = "SetToNaN" Or parameter = "FlagAllByFreq" Then
                newFilter.Parameters.Add(New ParameterValuePair(parameter, False))
            ElseIf newFilter.Name = "Nominal-Value Frequency Data Quality Filter" And parameter = "FlagBit" Then
                newFilter.Parameters.Add(New ParameterValuePair(parameter, False, False))
            Else
                newFilter.Parameters.Add(New ParameterValuePair(parameter, ""))
            End If
        Next
        'newFilter.Parameters
        DataConfigure.CollectionOfSteps.Add(newFilter)
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
        newCustomization.Name = obj.ToString
        newCustomization.StepCounter = DataConfigure.CollectionOfSteps.Count + 1
        newCustomization.CustPMUname = _lastCustPMUname

        Try
            Select Case newCustomization.Name
                Case "Scalar Repetition Customization"
                    For Each parameter In DataConfigure.CustomizationNameParemetersDictionary(newCustomization.Name)
                        If parameter <> "CustPMUname" AndAlso parameter <> "SignalName" Then
                            newCustomization.Parameters.Add(New ParameterValuePair(parameter, ""))
                        End If
                    Next
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
                    newCustomization.MinuendOrDivident = dummy
                    newCustomization.SubtrahendOrDivisor = dummy
                Case "Raise signals to an exponent"
                    newCustomization.Exponent = "1"
                Case "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Return angle of complex valued signals", "Take complex conjugate of signals", "Phasor Creation Customization", "Metric Prefix Customization", "Angle Conversion Customization"
                    'PASS
                Case "Power Calculation Customization"
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, "C")
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                    Dim newPair = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(newSignal, New ObservableCollection(Of SignalSignatures))
                    newCustomization.OutputInputMappingPair.Add(newPair)
                Case "Specify Signal Type and Unit Customization"
                    For Each parameter In DataConfigure.CustomizationNameParemetersDictionary(newCustomization.Name)
                        If parameter <> "CustPMUname" AndAlso parameter <> "CustName" AndAlso parameter <> "PMU" AndAlso parameter <> "Channel" Then
                            newCustomization.Parameters.Add(New ParameterValuePair(parameter, ""))
                        End If
                    Next
                    Dim newSignal = New SignalSignatures("", newCustomization.CustPMUname, "C")
                    newSignal.IsCustomSignal = True
                    newCustomization.OutputChannels.Add(newSignal)
                Case Else
                    Throw New Exception("Customization step not supported!")
            End Select
        Catch ex As Exception
            MessageBox.Show("Error selecting signal(s) for customization step!" & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
        newCustomization.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & newCustomization.StepCounter.ToString & "-" & newCustomization.Name
        newCustomization.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & newCustomization.StepCounter.ToString & "-" & newCustomization.Name
        newCustomization.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(newCustomization.OutputChannels)
        DataConfigure.CollectionOfSteps.Add(newCustomization)
    End Sub

    Private _selectedSignalChanged As ICommand
    Public Property SelectedSignalChanged As ICommand
        Get
            Return _selectedSignalChanged
        End Get
        Set(ByVal value As ICommand)
            _selectedSignalChanged = value
        End Set
    End Property
    Private Sub _CheckOutputType()
        If TypeOf (_currentSelectedStep) Is Customization AndAlso Not String.IsNullOrEmpty(_currentSelectedStep.CustPMUname) Then
            Dim type = "O"
            Select Case _currentSelectedStep.Name
                Case "Scalar Repetition Customization"
                    type = (From x In DirectCast(_currentSelectedStep, Customization).Parameters Where x.ParameterName = "SignalType" Select x.Value).FirstOrDefault
                    _currentSelectedStep.OutputChannels(0).TypeAbbreviation = type
                Case "Addition Customization"
                    If _currentSelectedStep.InputChannels.Count > 0 Then
                        type = _currentSelectedStep.InputChannels(0).TypeAbbreviation
                    End If
                    _currentSelectedStep.OutputChannels(0).TypeAbbreviation = type
                Case "Subtraction Customization"
                    If _currentSelectedStep.InputChannels.Count > 0 Then
                        type = _currentSelectedStep.InputChannels(0).TypeAbbreviation
                    End If
                    _currentSelectedStep.OutputChannels(0).TypeAbbreviation = type
                Case "Multiplication Customization"
                    If _currentSelectedStep.InputChannels.Count > 0 Then
                        type = "O"
                    End If
                    _currentSelectedStep.OutputChannels(0).TypeAbbreviation = type
                Case "Division Customization"
                    If _currentSelectedStep.InputChannels.Count > 0 Then
                        type = "O"
                    End If
                    _currentSelectedStep.OutputChannels(0).TypeAbbreviation = type
                Case "Raise signals to an exponent"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Reverse sign of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Take absolute value of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Return real component of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Return imaginary component of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Return angle of complex valued signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Take complex conjugate of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Phasor Creation Customization"

                Case "Power Calculation Customization"
                Case "Specify Signal Type and Unit Customization"
                    'type = (From x In DirectCast(_currentSelectedStep, Customization).Parameters Where x.ParameterName = "SigType" Select x.Value).FirstOrDefault
                    '_currentSelectedStep.OutputChannels(0).TypeAbbreviation = type
                Case "Metric Prefix Customization"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Angle Conversion Customization"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingPair
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case Else
                    Throw New Exception("Customization step not supported!")
            End Select
            'Dim 
            'If type Is Nothing Then
            '    type = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "SigType" Select x.Value).FirstOrDefault
            'End If

            'If type IsNot Nothing Then
            '    aStep.OutputChannels.Add(New SignalSignatures(name, aStep.CustPMUname, type))
            'Else
            '    aStep.OutputChannels.Add(New SignalSignatures(name, aStep.CustPMUname))
            'End If
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
            _determineAllParentNodeStatus()
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
        _determineAllParentNodeStatus()
        If _currentSelectedStep IsNot Nothing Then
            _currentSelectedStep.CurrentCursor = ""
        End If
        _currentInputOutputPair = Nothing
        _currentFocusedPhasorSignalForPowerCalculation = Nothing
    End Sub

    Private Sub _signalSelected(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count < 1 And (obj.SignalSignature.PMUName Is Nothing Or obj.SignalSignature.TypeAbbreviation Is Nothing) Then
                _keepOriginalSelection(obj)
                MessageBox.Show("Clicked item is not a valid signal, or contains no valid signal!", "Error!", MessageBoxButtons.OK)
            Else
                If TypeOf _currentSelectedStep Is DQFilter Then
                    Try
                        _changeSignalSelection(obj)
                    Catch ex As Exception
                        _keepOriginalSelection(obj)
                        MessageBox.Show("Error selecting signal(s) for data quality filter!" & vbCrLf & ex.Message, "Error!", MessageBoxButtons.OK)
                    End Try
                Else
                    Try
                        Select Case _currentSelectedStep.Name
                            Case "Scalar Repetition Customization"
                                Throw New Exception("Please do NOT select signals for Scalar Repetition Customization!")
                            Case "Addition Customization", "Multiplication Customization"
                                _changeSignalSelection(obj)
                                _CheckOutputType()
                            Case "Subtraction Customization", "Division Customization"
                                _setFocusedTextbox(obj)
                                _CheckOutputType()
                            Case "Raise signals to an exponent", "Reverse sign of signals", "Take absolute value of signals", "Return real component of signals", "Return imaginary component of signals", "Return angle of complex valued signals", "Take complex conjugate of signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Phasor Creation Customization"
                                _changeSignalSelectionPhasorCreation(obj)
                                '_CheckOutputType()
                            Case "Power Calculation Customization"
                                If _currentSelectedStep.OutputInputMappingPair(0).Value.Count = 2 Then
                                    _changePhasorSignalForPowerCalculationCustomization(obj)
                                Else
                                    _changeMagAngSignalForPowerCalculationCustomization(obj)
                                End If
                                '_CheckOutputType()
                            Case "Specify Signal Type and Unit Customization"
                                _specifySignalTypeUnitSignalSelectionChanged(obj)
                                '_CheckOutputType()
                            Case "Metric Prefix Customization"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Angle Conversion Customization"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
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
            ElseIf _currentSelectedStep.CurrentCursor = "MinuendOrDivident" Then
                If _currentSelectedStep.SubtrahendOrDivisor IsNot Nothing AndAlso obj.SignalSignature = _currentSelectedStep.SubtrahendOrDivisor Then
                    Throw New Exception("Minuend Or divident cannot be the same as the subtrahend or divisor!")
                End If
                If obj.SignalSignature.IsChecked Then       ' check box checked
                    If _currentSelectedStep.MinuendOrDivident IsNot Nothing And _currentSelectedStep.MinuendOrDivident IsNot _currentSelectedStep.SubtrahendOrDivisor Then  ' if the current text box has content and not equal to the divisor
                        _currentSelectedStep.MinuendOrDivident.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(_currentSelectedStep.MinuendOrDivident)
                    End If
                    _currentSelectedStep.MinuendOrDivident = obj.SignalSignature
                    If Not _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    End If
                Else                                        ' check box unchecked
                    If _currentSelectedStep.MinuendOrDivident Is obj.SignalSignature Then   ' if the content of the text box is the same as the clicked item and the checkbox is unchecked, means user wants to delete the content in the textbox
                        If _currentSelectedStep.SubtrahendOrDivisor Is obj.SignalSignature Then     ' however, if the textbox has the same contect as the divisor or subtrahend, we cannot uncheck the clicked item
                            obj.SignalSignature.IsChecked = True
                        Else
                            _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                        End If
                        Dim dummy = New SignalSignatures("PleaseAddASignal", "PleaseAddASignal")
                        dummy.IsValid = False
                        _currentSelectedStep.MinuendOrDivident = dummy
                    End If
                End If
                _currentSelectedStep.CurrentCursor = ""
                _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
                _determineAllParentNodeStatus()
            ElseIf _currentSelectedStep.CurrentCursor = "SubtrahendOrDivisor" Then
                If _currentSelectedStep.MinuendOrDivident IsNot Nothing AndAlso obj.SignalSignature = _currentSelectedStep.MinuendOrDivident Then
                    Throw New Exception("Subtrahend Or divisor cannot be the same as the minuend or divident!")
                End If
                If obj.SignalSignature.IsChecked Then
                    If _currentSelectedStep.SubtrahendOrDivisor IsNot Nothing And _currentSelectedStep.SubtrahendOrDivisor IsNot _currentSelectedStep.MinuendOrDivident Then
                        _currentSelectedStep.SubtrahendOrDivisor.IsChecked = False
                        _currentSelectedStep.InputChannels.Remove(_currentSelectedStep.SubtrahendOrDivisor)
                    End If
                    _currentSelectedStep.SubtrahendOrDivisor = obj.SignalSignature
                    If Not _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    End If
                Else
                    If _currentSelectedStep.SubtrahendOrDivisor Is obj.SignalSignature Then
                        If _currentSelectedStep.MinuendOrDivident Is obj.SignalSignature Then
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
                _determineAllParentNodeStatus()
            End If
        End If
    End Sub
    Private Sub _changeSignalSelectionUnarySteps(obj As SignalTypeHierachy)
        If Not _currentInputOutputPair.HasValue Then
            If obj.SignalSignature.IsChecked Then
                '_checkAllChildren(obj, obj.SignalSignature.IsChecked)
                _addOrDeleteInputSignal(obj, obj.SignalSignature.IsChecked)
                _addOuputSignals(obj)
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
        _determineAllParentNodeStatus()
        _determineFileDirCheckableStatus()
    End Sub

    Private Sub _changeSignalSelection(obj As SignalTypeHierachy)
        _checkAllChildren(obj, obj.SignalSignature.IsChecked)
        _addOrDeleteInputSignal(obj, obj.SignalSignature.IsChecked)
        _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        _determineAllParentNodeStatus()
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
        _determineAllParentNodeStatus()
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
            For Each group In GroupedSignalByStepsOutput
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
            For Each group In GroupedSignalsByType
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
                    '_keepOriginalSelection(obj)
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
                    '_keepOriginalSelection(obj)
                    _addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type current phasor.")
                    Throw New Exception("Signal selection is not Valid! Please select a signal of current phasor.")
                End If
            Else
                '_keepOriginalSelection(obj)
                '_addLog("Selected signal: " & obj.SignalSignature.SignalName & " is not of signal type current phasor.")
                Throw New Exception("Error changing signal for this power calculation step!")
            End If
        End If
        _currentFocusedPhasorSignalForPowerCalculation = Nothing
        _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)

        _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)

        _determineAllParentNodeStatus()
        _determineFileDirCheckableStatus()

    End Sub
    Private Sub _changeMagAngSignalForPowerCalculationCustomization(obj As SignalTypeHierachy)
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

        _determineAllParentNodeStatus()
        _determineFileDirCheckableStatus()


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
            _determineAllParentNodeStatus()
            _determineFileDirCheckableStatus()
        End If
    End Sub

    ''' <summary>
    ''' Check and decide if a file directory and its sub grouped signal is checkable or not depends on other file directory check status
    ''' </summary>
    Private Sub _determineFileDirCheckableStatus()
        Dim disableOthers = False
        For Each group In GroupedSignalsByType
            If group.SignalSignature.IsChecked Or group.SignalSignature.IsChecked Is Nothing Then
                disableOthers = True
            End If
        Next
        If disableOthers Then
            For Each group In GroupedSignalsByType
                If Not group.SignalSignature.IsChecked Then
                    group.SignalSignature.IsEnabled = False
                Else
                    group.SignalSignature.IsEnabled = True
                End If
            Next
            For Each group In GroupedSignalsByPMU
                If Not group.SignalSignature.IsChecked Then
                    group.SignalSignature.IsEnabled = False
                Else
                    group.SignalSignature.IsEnabled = True
                End If
            Next
        Else
            For Each group In GroupedSignalsByType
                group.SignalSignature.IsEnabled = True
            Next
            For Each group In GroupedSignalsByPMU
                group.SignalSignature.IsEnabled = True
            Next
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
                child.SignalSignature.IsChecked = isChecked
                _checkAllChildren(child, isChecked)
            Next
        Else

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
    ''' This sub check the pmu parent tree
    ''' </summary>
    ''' <param name="node"></param>
    'Private Sub _checkPMUParentStaus(ByRef node As SignalSignatures)
    '    For Each group In GroupedSignalsByPMU
    '        If group.SignalSignature.SignalName = node.PMUName Then
    '            _determineParentCheckStatus(group)
    '        End If
    '    Next
    'End Sub

    'Private _addSelectedSignalToStep As ICommand
    'Public Property AddSelectedSignalToStep As ICommand
    '    Get
    '        Return _addSelectedSignalToStep
    '    End Get
    '    Set(ByVal value As ICommand)
    '        _addSelectedSignalToStep = value
    '    End Set
    'End Property

    'Private Sub _addSelectedSignal(obj As Object)
    '    If _currentSelectedStep IsNot Nothing Then
    '        '_currentSelectedStep.InputChannels.Clear()
    '        Dim a = New ObservableCollection(Of SignalSignatures)
    '        For Each signal In TaggedSignals
    '            If signal.IsChecked Then
    '                a.Add(signal)
    '            End If
    '        Next
    '        _currentSelectedStep.InputChannels = a
    '    End If
    'End Sub
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

    Private _stepSelected As ICommand
    Public Property StepSelected As ICommand
        Get
            Return _stepSelected
        End Get
        Set(ByVal value As ICommand)
            _stepSelected = value
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
                For Each stp In DataConfigure.CollectionOfSteps
                    If stp.IsStepSelected Then
                        stp.IsStepSelected = False
                        For Each signal In stp.InputChannels
                            signal.IsChecked = False
                        Next
                        If TypeOf (stp) Is Customization Then
                            For Each signal In stp.OutputChannels
                                signal.IsChecked = False
                            Next
                        End If
                    End If
                    If stp.StepCounter < lastNumberOfSteps Then
                        stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                        If TypeOf (stp) Is Customization Then
                            stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                        End If
                    End If
                Next
                _determineFileDirCheckableStatus()
                processStep.IsStepSelected = True
                'TODO: disable all signal selection tree if step name is "Scalar Repetition Customization"
                For Each signal In processStep.InputChannels
                    signal.IsChecked = True
                    '_checkParentStatus(signal)
                    '_checkPMUParentStaus(signal)
                Next
                'If TypeOf (processStep) Is Customization AndAlso processStep.OutputChannels IsNot Nothing Then
                '    For Each signal In processStep.OutputChannels
                '        signal.IsChecked = True
                '    Next
                'End If

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
                        _disableEnableAllButMagnitudeSignals(True)
                    ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
                        If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
                            Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
                            If situation = 4 Then
                                _disableEnableAllButMagnitudeSignals(True)
                            ElseIf situation = 2 Then
                                _disableEnableAllButPhasorSignals(True)
                            End If
                        End If
                    End If
                End If

                GroupedSignalByStepsInput = stepsInputAsSignalHierachy
                GroupedSignalByStepsOutput = stepsOutputAsSignalHierachy

                _determineAllParentNodeStatus()

                _determineFileDirCheckableStatus()

                If processStep.Name = "Phasor Creation Customization" Then
                    '_disableEnableGroupForPhasorCreationCustomization(GroupedSignalsByType, False)
                    '_disableEnableGroupForPhasorCreationCustomization(GroupedSignalByStepsInput, False)
                    '_disableEnableGroupForPhasorCreationCustomization(GroupedSignalByStepsOutput, False)
                    _disableEnableAllButMagnitudeSignals(False)
                ElseIf processStep.Name = "Power Calculation Customization" Then
                    If processStep.OutputInputMappingPair.Count > 0 Then
                        Dim situation = processStep.OutputInputMappingPair(0).Value.Count
                        If situation = 4 Then
                            _disableEnableAllButMagnitudeSignals(False)
                        ElseIf situation = 2 Then
                            _disableEnableAllButPhasorSignals(False)
                        End If
                    End If
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
    ''' <param name="isEnable"></param>
    Private Sub _disableEnableAllButMagnitudeSignals(isEnable As Boolean)
        For Each group In GroupedSignalsByType
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnable
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If String.IsNullOrEmpty(subsubgroup.SignalSignature.TypeAbbreviation) OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnable
                            'Else
                            '    If subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            '        subsubgroup.SignalSignature.IsEnabled = isEnable
                            '    End If
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalsByPMU
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnable
                    End If
                Next
            Next
        Next
        For Each group In GroupedSignalByStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnable
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnable
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "M" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnable
                    End If
                Next
            Next
        Next
    End Sub
    ''' <summary>
    ''' disable all signal type except voltage magnitude or current magnitude
    ''' </summary>
    ''' <param name="parentGroup"></param>
    ''' <param name="isEnable"></param>
    'Private Sub _disableEnableGroupForPhasorCreationCustomization(parentGroup As ObservableCollection(Of SignalTypeHierachy), isEnable As Boolean)
    '    For Each group In parentGroup
    '        If group.SignalSignature.IsEnabled Then
    '            For Each subgroup In group.SignalList
    '                If subgroup.SignalSignature.TypeAbbreviation Is Nothing Then
    '                    For Each subsubgroup In subgroup.SignalList
    '                        Dim type = subsubgroup.SignalSignature.TypeAbbreviation
    '                        If type Is Nothing OrElse type.Length <> 3 OrElse type.Substring(1, 1) <> "M" Then
    '                            subsubgroup.SignalSignature.IsEnabled = isEnable
    '                        End If
    '                    Next
    '                ElseIf subgroup.SignalSignature.TypeAbbreviation <> "V" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "I" Then
    '                    subgroup.SignalSignature.IsEnabled = isEnable
    '                    For Each subsubgroup In subgroup.SignalList
    '                        subsubgroup.SignalSignature.IsEnabled = isEnable
    '                    Next
    '                Else
    '                    For Each subsubgroup In subgroup.SignalList
    '                        If subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "M" Then
    '                            subsubgroup.SignalSignature.IsEnabled = isEnable
    '                            For Each subsubsubgroup In subsubgroup.SignalList
    '                                subsubsubgroup.SignalSignature.IsEnabled = isEnable
    '                                For Each subsubsubsubgroup In subsubsubgroup.SignalList
    '                                    subsubsubsubgroup.SignalSignature.IsEnabled = isEnable
    '                                Next
    '                            Next
    '                        End If
    '                    Next
    '                End If
    '            Next
    '        End If
    '    Next
    'End Sub
    Private Sub _disableEnableAllButPhasorSignals(isEnable As Boolean)
        For Each group In GroupedSignalsByType
            group.SignalSignature.IsEnabled = isEnable
        Next
        For Each group In GroupedSignalsByPMU
            group.SignalSignature.IsEnabled = isEnable
        Next
        For Each group In GroupedSignalByStepsInput
            For Each subgroup In group.SignalList
                If subgroup.SignalSignature.TypeAbbreviation <> "I" AndAlso subgroup.SignalSignature.TypeAbbreviation <> "V" Then
                    subgroup.SignalSignature.IsEnabled = isEnable
                Else
                    For Each subsubgroup In subgroup.SignalList
                        If subsubgroup.SignalSignature.TypeAbbreviation.Substring(1) <> "P" Then
                            subsubgroup.SignalSignature.IsEnabled = isEnable
                        End If
                    Next
                End If
            Next
        Next
        For Each group In GroupedSignalByStepsOutput
            For Each subgroup In group.SignalList
                For Each subsubgroup In subgroup.SignalList
                    If subsubgroup.SignalSignature.TypeAbbreviation.Length <> 3 OrElse subsubgroup.SignalSignature.TypeAbbreviation.Substring(1, 1) <> "P" Then
                        subsubgroup.SignalSignature.IsEnabled = isEnable
                    End If
                Next
            Next
        Next
    End Sub
    ''' <summary>
    ''' Go down a tree to determine nodes checking status
    ''' </summary>
    ''' <param name="groups"></param>
    Private Sub _determineParentGroupedByTypeNodeStatus(groups As ObservableCollection(Of SignalTypeHierachy))
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
    End Sub

    Private Sub _determineAllParentNodeStatus()
        _determineParentGroupedByTypeNodeStatus(GroupedSignalsByType)
        _determineParentGroupedByTypeNodeStatus(GroupedSignalsByPMU)
        'For Each group In GroupedSignalsByPMU
        '    _determineParentCheckStatus(group)
        'Next
        For Each stepInput In GroupedSignalByStepsInput
            If stepInput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepInput.SignalList)
                _determineParentCheckStatus(stepInput)
            Else
                stepInput.SignalSignature.IsChecked = False
            End If
        Next
        For Each stepOutput In GroupedSignalByStepsOutput
            If stepOutput.SignalList.Count > 0 Then
                _determineParentGroupedByTypeNodeStatus(stepOutput.SignalList)
                _determineParentCheckStatus(stepOutput)
            Else
                stepOutput.SignalSignature.IsChecked = False
            End If
        Next
    End Sub

    Private _stepDeSelected As ICommand
    Public Property StepDeSelected As ICommand
        Get
            Return _stepDeSelected
        End Get
        Set(ByVal value As ICommand)
            _stepDeSelected = value
        End Set
    End Property
    ''' <summary>
    ''' When user click outside the step list and none of the steps should be selected, then we need to uncheck all checkboxes
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _deSelectAllSteps(obj As Object)
        If _currentSelectedStep IsNot Nothing Then
            For Each signal In _currentSelectedStep.InputChannels
                signal.IsChecked = False
            Next
            If TypeOf (_currentSelectedStep) Is Customization AndAlso _currentSelectedStep.OutputChannels IsNot Nothing Then
                For Each signal In _currentSelectedStep.OutputChannels
                    signal.IsChecked = False
                Next
            End If
            Dim stepsInputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            For Each stp In DataConfigure.CollectionOfSteps
                stepsInputAsSignalHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                If TypeOf (stp) Is Customization Then
                    stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                End If
            Next
            GroupedSignalByStepsInput = stepsInputAsSignalHierachy
            GroupedSignalByStepsOutput = stepsOutputAsSignalHierachy
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByStepsInput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByStepsOutput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalsByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalsByType, False)
            _currentSelectedStep.IsStepSelected = False
            CurrentSelectedStep = Nothing
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


    'Private Sub _addOrDeleteSignal(obj As SignalSignatures, isChecked As Boolean)
    '    If isChecked Then
    '        If _currentSelectedStep IsNot Nothing Then
    '            _currentSelectedStep.InputChannels.Add(obj)
    '            _currentSelectedStep.InputChannelsSortedByType = SortSignalByType(_currentSelectedStep.InputChannels)
    '        End If
    '    Else
    '        _currentSelectedStep.InputChannels.Remove(obj)
    '        _currentSelectedStep.InputChannelsSortedByType = SortSignalByType(_currentSelectedStep.InputChannels)
    '    End If
    'End Sub

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

    Private Sub _addOuputSignals(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count > 0 Then
                For Each child In obj.SignalList
                    _addOuputSignals(child)
                Next
            Else
                'If isChecked Then
                Dim newOutput = New SignalSignatures(obj.SignalSignature.SignalName, _currentSelectedStep.CustPMUname, obj.SignalSignature.TypeAbbreviation)
                newOutput.IsChecked = True
                _currentSelectedStep.outputChannels.Add(newOutput)
                Dim targetkey = (From kvp In DirectCast(_currentSelectedStep, Customization).OutputInputMappingPair Where kvp.Key = newOutput Select kvp Distinct).ToList()
                If targetkey.Count = 0 Then
                    Dim kvp = New KeyValuePair(Of SignalSignatures, ObservableCollection(Of SignalSignatures))(newOutput, New ObservableCollection(Of SignalSignatures))
                    kvp.Value.Add(obj.SignalSignature)
                    _currentSelectedStep.OutputInputMappingPair.Add(kvp)
                End If
                'Dim tempDict = _currentSelectedStep.OutputInputMappingPair.ToDictionary(Function(x) x.Value.Key, Function(x) x.Value.Value)
                'If Not tempDict.ContainsKey(newOutput) Then
                '    tempDict(newOutput) = New ObservableCollection(Of SignalSignatures)
                '    tempDict(newOutput).Add(obj.SignalSignature)
                'End If
                '_currentSelectedStep.OutputInputMappingPair = tempDict.ToList()
                'Else

                '    For Each signal In _currentSelectedStep.OutputChannels
                '        If signal.SignalName = obj.SignalSignature.SignalName Then
                '            _currentSelectedStep.OutputChannels.Remove(signal)
                '            Exit For
                '        End If
                '    Next
                'End If
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
                    _addOrDeleteInputSignal(signal, isChecked)
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
                    Else
                        _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                    End If
                    obj.SignalSignature.IsChecked = isChecked
                End If
            End If
        End If
    End Sub
    Private Sub _addMatchingInputOutputSignalsPhasor(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 Then
            For Each signal In obj.SignalList
                _addMatchingInputOutputSignalsPhasor(signal)
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
        _determineAllParentNodeStatus()
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
    Private _deleteThisStep As ICommand
    Public Property DeleteThisStep As ICommand
        Get
            Return _deleteThisStep
        End Get
        Set(ByVal value As ICommand)
            _deleteThisStep = value
        End Set
    End Property
    Private Sub _deleteAStep(obj As Object)
        Dim toBeDeleted As Object
        Dim steps = New ObservableCollection(Of Object)(DataConfigure.CollectionOfSteps)
        ' First find the step to be deleted
        For Each stp In steps
            If stp.StepCounter = obj.StepCounter Then
                toBeDeleted = stp
                Exit For
            End If
        Next
        ' if the step is found
        If toBeDeleted IsNot Nothing Then
            Dim stepInputHierachy = New ObservableCollection(Of SignalTypeHierachy)
            Dim stepOutputHierachy = New ObservableCollection(Of SignalTypeHierachy)
            Try
                ' go through each step to change names that affected by the deleted step
                For Each stp In steps
                    If stp.StepCounter < toBeDeleted.StepCounter Then
                        stepInputHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                        stepOutputHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                    End If
                    If stp.StepCounter > toBeDeleted.StepCounter Then
                        stp.StepCounter -= 1
                        stp.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & stp.StepCounter.ToString & "-" & stp.Name
                        stepInputHierachy.Add(stp.ThisStepInputsAsSignalHerachyByType)
                        If TypeOf (stp) Is Customization Then
                            stp.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & stp.StepCounter.ToString & "-" & stp.Name
                            stepOutputHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
                        End If
                    End If
                Next
                GroupedSignalByStepsInput = stepInputHierachy
                GroupedSignalByStepsOutput = stepOutputHierachy
                If CurrentSelectedStep.StepCounter = toBeDeleted.StepCounter Then
                    For Each signal In toBeDeleted.InputChannels
                        signal.IsChecked = False
                    Next
                    If CurrentSelectedStep.Name = "Phasor Creation Customization" Then
                        _disableEnableAllButMagnitudeSignals(True)
                    ElseIf CurrentSelectedStep.Name = "Power Calculation Customization" Then
                        If CurrentSelectedStep.OutputInputMappingPair.Count > 0 Then
                            Dim situation = CurrentSelectedStep.OutputInputMappingPair(0).Value.Count
                            If situation = 4 Then
                                _disableEnableAllButMagnitudeSignals(True)
                            Else
                                _disableEnableAllButPhasorSignals(True)
                            End If
                        End If
                    End If
                    CurrentSelectedStep = Nothing
                    _determineAllParentNodeStatus()
                    _determineFileDirCheckableStatus()
                End If
                steps.Remove(toBeDeleted)
                _addLog("Step " & toBeDeleted.StepCounter & ", " & toBeDeleted.Name & " is deleted!")
                DataConfigure.CollectionOfSteps = steps
                toBeDeleted = Nothing
                SignalSelectionTreeViewVisibility = "Visible"
            Catch ex As Exception
                MessageBox.Show("Error deleting step " & toBeDeleted.StepCounter.ToString & ", " & toBeDeleted.Name, "Error!", MessageBoxButtons.OK)
            End Try
        Else
            MessageBox.Show("Step " & toBeDeleted.StepCounter.ToString & ", " & toBeDeleted.Name & " is not found!", "Error!", MessageBoxButtons.OK)
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
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(vPhasor)
        Dim iPhasor = New SignalSignatures("NeedCurrentPhasor", _currentSelectedStep.CustPMUname)
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(iPhasor)
        _disableEnableAllButPhasorSignals(False)
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
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(vMag)
        Dim vAng = New SignalSignatures("NeedVoltageAng", _currentSelectedStep.CustPMUname)
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(vAng)
        Dim iMag = New SignalSignatures("NeedCurrentMag", _currentSelectedStep.CustPMUname)
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(iMag)
        Dim iAng = New SignalSignatures("NeedCurrentAng", _currentSelectedStep.CustPMUname)
        _currentSelectedStep.OutputInputMappingPair(0).Value.Add(iAng)
        _disableEnableAllButMagnitudeSignals(False)
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

                For Each group In GroupedSignalsByType
                    If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                        GroupedSignalsByType.Remove(group)
                        Exit For
                    End If
                Next
                For Each group In GroupedSignalsByPMU
                    If group.SignalSignature.SignalName.Split(",")(0) = obj.FileDirectory Then
                        GroupedSignalsByPMU.Remove(group)
                        Exit For
                    End If
                Next
                For Each stp In DataConfigure.CollectionOfSteps
                    If stp.InputChannels.Count > 0 Then
                        If obj.TaggedSignals.Contains(stp.InputChannels(0)) Then
                            stp.InputChannels = New ObservableCollection(Of SignalSignatures)
                            stp.ThisStepInputsAsSignalHerachyByType.SignalList = New ObservableCollection(Of SignalTypeHierachy)
                            stp.ThisStepInputsAsSignalHerachyByType.SignalSignature.IsChecked = False
                            'stp.ThisStepOutputsAsSignalHierachyByPMU.SignalList = New ObservableCollection(Of SignalTypeHierachy)
                            'stp.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.IsChecked = False
                            If TypeOf stp Is Customization Then
                                For Each pair In stp.OutputInputMappingPair
                                    Dim numberOfInput = pair.Value.Count
                                    pair.Value.Clear()
                                    For index = 0 To numberOfInput - 1
                                        Dim empty = New SignalSignatures("NeedInput")
                                        empty.IsValid = False
                                        pair.Value.Add(empty)
                                    Next
                                Next
                                stp.MinuendOrDivident = New SignalSignatures("NeedMinuendOrDivident")
                                stp.MinuendOrDivident.IsValid = False
                                stp.SubtrahendOrDivisor = New SignalSignatures("NeedSubtrahendOrDivisor")
                                stp.SubtrahendOrDivisor.IsValid = False
                            End If
                            AllPMUs = New ObservableCollection(Of String)
                            For Each group In GroupedSignalsByPMU
                                For Each subgroup In group.SignalList
                                    If Not AllPMUs.Contains(subgroup.SignalSignature.PMUName) Then
                                        AllPMUs.Add(subgroup.SignalSignature.PMUName)
                                    End If
                                Next
                            Next
                        End If
                    End If
                Next
                DataConfigure.ReaderProperty.InputFileInfos.Remove(obj)
                Exit For
            End If
        Next
    End Sub
End Class
