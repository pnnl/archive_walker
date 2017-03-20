Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Forms
Imports BAWGUI
Imports PDAT_Reader
'Imports BAWGUI.DataConfig

Public Class SettingsViewModel
    Inherits ViewModelBase

    Public Sub New()
        _configFileName = ""
        '_sampleFile = ""

        _dataConfigure = New DataConfig
        _processConfigure = New ProcessConfig
        _detectorConfigure = New DetectorConfig
        '_currentSelectedStep = New DQFilter

        _openConfigFile = New DelegateCommand(AddressOf openConfigXMLFile, AddressOf CanExecute)
        _browseInputFileDir = New DelegateCommand(AddressOf _browseInputFileFolder, AddressOf CanExecute)
        _fileTypeChanged = New DelegateCommand(AddressOf _buildInputFileFolderTree, AddressOf CanExecute)
        _dqfilterSelected = New DelegateCommand(AddressOf _dqfilterSelection, AddressOf CanExecute)
        _customizationSelected = New DelegateCommand(AddressOf _customizationStepSelection, AddressOf CanExecute)
        _selectedSignalChanged = New DelegateCommand(AddressOf _signalSelected, AddressOf CanExecute)
        _stepSelected = New DelegateCommand(AddressOf _stepSelectedToEdit, AddressOf CanExecute)
        _stepDeSelected = New DelegateCommand(AddressOf _deSelectAllSteps, AddressOf CanExecute)
        _setCurrentFocusedTextbox = New DelegateCommand(AddressOf _curentFocusedTextBoxChanged, AddressOf CanExecute)
        _selectedOutputSignalChanged = New DelegateCommand(AddressOf _outputSignalSelectionChanged, AddressOf CanExecute)
        _textboxesLostFocus = New DelegateCommand(AddressOf _recoverCheckStatusOfCurrentStep, AddressOf CanExecute)
        '_inputFileDirTree = New ObservableCollection(Of Folder)
        _groupedSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
        _groupedSignalByStepsOutput = New ObservableCollection(Of SignalTypeHierachy)
        _allPMUs = New ObservableCollection(Of String)

        _timezoneList = TimeZoneInfo.GetSystemTimeZones
        _signalSelectionTreeViewVisibility = "Visible"
        _selectSignalMethods = {"All Raw Input Channels by Signal Type",
                                "All Raw Input Channels by PMU",
                                "Input Channels by Step",
                                "OutPut Channels by Step"}.ToList
        _selectedSelectionMethod = "All Raw Input Channels by Signal Type"
    End Sub

    'Private _sampleFile As String

    'Private _taggedSignals As ObservableCollection(Of SignalSignatures)
    'Public Property TaggedSignals As ObservableCollection(Of SignalSignatures)
    '    Get
    '        Return _taggedSignals
    '    End Get
    '    Set(ByVal value As ObservableCollection(Of SignalSignatures))
    '        _taggedSignals = value
    '        OnPropertyChanged()
    '    End Set
    'End Property

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
                        signal.TypeAbbreviation = "R"
                        signal.SignalName = nameParts(0) & ".rocof"
                    Case "A"
                        Dim channel = nameParts(1).Substring(nameParts(1).Length - 2).ToArray
                        signal.TypeAbbreviation = channel(0) & "A" & channel(1)
                        signal.SignalName = nameParts(0) & "." & nameParts(1) & ".ANG"
                    Case "M"
                        Dim channel = nameParts(1).Substring(nameParts(1).Length - 2).ToArray
                        signal.TypeAbbreviation = channel(0) & "M" & channel(1)
                        signal.SignalName = nameParts(0) & "." & nameParts(1) & ".MAG"
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
                Case "O"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("Other"))
                    newHierachy.SignalSignature.TypeAbbreviation = "O"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
                Case "C"
                    Dim newHierachy = New SignalTypeHierachy(New SignalSignatures("CustomizedSignal"))
                    newHierachy.SignalSignature.TypeAbbreviation = "C"
                    For Each signal In signalType.Value
                        newHierachy.SignalList.Add(New SignalTypeHierachy(signal))
                    Next
                    signalTypeTree.Add(newHierachy)
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
            GroupedSignalsByType = New ObservableCollection(Of SignalTypeHierachy)
            GroupedSignalsByPMU = New ObservableCollection(Of SignalTypeHierachy)
            Try
                _configData = XDocument.Load(_configFileName)
                _readConfigFile()
            Catch ex As Exception
                MessageBox.Show("Error reading config file!" & vbCrLf & ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    Private Sub _readConfigFile()
        'Dim _sampleFile = ""
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
        'Dim CollectionOfSteps As New ObservableCollection(Of Object)
        'Dim stepsAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
        ''Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
        'Dim stepCounter As Integer = 0
        'Dim stages = From el In _configData.<Config>.<DataConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
        'For Each el In stages
        '    Dim steps = From element In _configData.<Config>.<DataConfig>.<Configuration>.<Stages>.Elements Select element
        '    For Each element In steps
        '        Dim aStep As Object
        '        Dim necessaryParams As New List(Of String)
        '        Dim outputInputNameDictionary = New Dictionary(Of String, List(Of SignalSignatures))

        '        If element.Name = "Filter" Then
        '            aStep = New DQFilter
        '            aStep.Name = DataConfigure.DQFilterReverseNameDictionary(element.<Name>.Value)
        '            necessaryParams.AddRange(DataConfigure.DQFilterNameParametersDictionary(aStep.Name))
        '        ElseIf element.Name = "Customization" Then
        '            aStep = New Customization
        '            aStep.Name = DataConfigure.CustomizationReverseNameDictionary(element.<Name>.Value)
        '            necessaryParams.AddRange(DataConfigure.CustomizationNameParemetersDictionary(aStep.Name))
        '        End If
        '        stepCounter += 1
        '        aStep.StepCounter = stepCounter
        '        aStep.ThisStepInputsAsSignalHerachyByType.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
        '        If TypeOf (aStep) Is Customization Then
        '            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalSignature.SignalName = "Step " & aStep.StepCounter.ToString & "-" & aStep.Name
        '        End If
        '        Dim params = From ps In element.<Parameters>.Elements Select ps
        '        For Each pair In params
        '            Dim aPair As New ParameterValuePair
        '            Dim paraName = pair.Name.ToString
        '            If TypeOf (aStep) Is Customization AndAlso (paraName = "term" Or paraName = "factor" Or paraName = "signal") Then
        '                Dim signal = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
        '                If signal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(signal) Then
        '                    aStep.InputChannels.Add(signal)
        '                End If
        '                Dim custSignalName = pair.<CustName>.Value
        '                If Not String.IsNullOrEmpty(custSignalName) Then
        '                    'Dim ext = custSignalName.LastIndexOf(".")
        '                    'If custSignalName.Substring(ext + 1).ToLower = "mag" Then
        '                    '    custSignalName = custSignalName.Substring(0, custSignalName.Length - 2)
        '                    'End If
        '                    'Dim parts = custSignalName.Split(".")
        '                    'Dim last2Letters = parts(-2).Substring(-2).ToArray
        '                    'If parts(-1).ToLower = "mag" Then
        '                    '    Dim signalType = last2Letters(0) & "M" & last2Letters(1)
        '                    'End If                                
        '                    If Not outputInputNameDictionary.ContainsKey(custSignalName) Then
        '                        outputInputNameDictionary(custSignalName) = New List(Of SignalSignatures)
        '                        outputInputNameDictionary(custSignalName).Add(signal)
        '                    Else
        '                        Throw New Exception("Duplicate custom signal name " & custSignalName & " found in this step!")
        '                    End If
        '                    'aStep.CustSignalName.Add(custSignalName)
        '                End If
        '            ElseIf TypeOf (aStep) Is Customization And (paraName = "minuend" Or paraName = "dividend") Then
        '                aStep.MinuendOrDivident = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
        '                If aStep.MinuendOrDivident IsNot Nothing Then
        '                    aStep.InputChannels.Add(aStep.MinuendOrDivident)
        '                End If
        '            ElseIf TypeOf (aStep) Is Customization And (paraName = "subtrahend" Or paraName = "divisor") Then
        '                aStep.SubtrahendOrDivisor = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
        '                If aStep.SubtrahendOrDivisor IsNot Nothing Then
        '                    aStep.InputChannels.Add(aStep.SubtrahendOrDivisor)
        '                End If
        '            ElseIf TypeOf (aStep) Is Customization And paraName = "CustPMUname" Then
        '                aStep.CustPMUname = pair.Value
        '            ElseIf TypeOf (aStep) Is Customization And paraName = "SignalName" Then
        '                aStep.CustSignalName.Add(pair.Value)
        '            ElseIf TypeOf (aStep) Is Customization And paraName = "CustName" Then
        '                aStep.CustSignalName.Add(pair.Value)
        '            ElseIf TypeOf aStep Is Customization And paraName = "phasor" Then
        '                'Dim phasors = From x In pair.<phasor>.Elements Select x
        '                'For Each phasor In phasors
        '                Dim magSignal = _searchForSignalInTaggedSignals(pair.<mag>.<PMU>.Value, pair.<mag>.<Channel>.Value)
        '                If magSignal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(magSignal) Then
        '                    aStep.InputChannels.Add(magSignal)
        '                End If
        '                Dim angSignal = _searchForSignalInTaggedSignals(pair.<ang>.<PMU>.Value, pair.<ang>.<Channel>.Value)
        '                If angSignal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(angSignal) Then
        '                    aStep.InputChannels.Add(angSignal)
        '                End If
        '                Dim custSignalName = pair.<CustName>.Value
        '                If Not String.IsNullOrEmpty(custSignalName) Then
        '                    'Dim ext = custSignalName.LastIndexOf(".")
        '                    'If custSignalName.Substring(ext + 1).ToLower = "mag" Then
        '                    '    custSignalName = custSignalName.Substring(0, custSignalName.Length - 2)
        '                    'End If
        '                    'Dim parts = custSignalName.Split(".")
        '                    'Dim last2Letters = parts(-2).Substring(-2).ToArray
        '                    'If parts(-1).ToLower = "mag" Then
        '                    '    Dim signalType = last2Letters(0) & "M" & last2Letters(1)
        '                    'End If
        '                    If Not outputInputNameDictionary.ContainsKey(custSignalName) Then
        '                        outputInputNameDictionary(custSignalName) = New List(Of SignalSignatures)
        '                        outputInputNameDictionary(custSignalName).Add(magSignal)
        '                        outputInputNameDictionary(custSignalName).Add(angSignal)
        '                    Else
        '                        Throw New Exception("Duplicate custom signal name " & custSignalName & " found in this step!")
        '                    End If
        '                    'aStep.CustSignalName.Add(custSignalName)
        '                End If
        '                'Next
        '            Else
        '                aPair.ParameterName = paraName
        '                If pair.Value.ToLower = "false" Then
        '                    aPair.Value = False
        '                ElseIf pair.Value.ToLower = "true" Then
        '                    aPair.Value = True
        '                ElseIf aStep.Name = "Nominal-Value Frequency Data Quality Filter" And pair.Value = "FlagBit" Then
        '                    aPair.IsRequired = False
        '                    aPair.Value = pair.Value
        '                Else
        '                    aPair.Value = pair.Value
        '                End If
        '                aStep.Parameters.Add(aPair)
        '            End If
        '            necessaryParams.Remove(paraName)
        '        Next
        '        'If TypeOf (aStep) Is Customization Then
        '        '    aStep.CustPMUname = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "CustPMUname" Select x.Value).FirstOrDefault
        '        '    aStep.CustSignalName = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "SignalName" Select x.Value).FirstOrDefault
        '        '    If element.<Parameters>.<CustName>.Value Then
        '        '        aStep.CustSignalName = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "CustName" Select x.Value).FirstOrDefault
        '        '    End If
        '        If TypeOf (aStep) Is Customization AndAlso Not String.IsNullOrEmpty(aStep.CustPMUname) AndAlso aStep.CustSignalName.Count > 0 Then
        '            For Each name In aStep.CustSignalName
        '                aStep.OutputChannels.Add(New SignalSignatures(name, aStep.CustPMUname, "C"))
        '            Next
        '        End If
        '        If TypeOf aStep Is Customization AndAlso outputInputNameDictionary.Count > 0 AndAlso Not String.IsNullOrEmpty(aStep.CustPMUname) Then
        '            For Each pair In outputInputNameDictionary
        '                Dim newOutputSignal = New SignalSignatures(pair.Key, aStep.CustPMUname, "C")
        '                aStep.OutputChannels.Add(newOutputSignal)
        '                If Not aStep.OutputInputMappingDictionary.containsKey(newOutputSignal) Then
        '                    aStep.OutputInputMappingDictionary(newOutputSignal) = New ObservableCollection(Of SignalSignatures)
        '                    For Each signal In pair.Value
        '                        aStep.OutputInputMappingDictionary(newOutputSignal).Add(signal)
        '                    Next
        '                Else
        '                    Throw New Exception("Duplicate custom signal name " & pair.Key & " found in this step!")
        '                End If
        '            Next
        '        End If
        '        'End If
        '        For Each parameter In necessaryParams
        '            aStep.Parameters.Add(New ParameterValuePair(parameter, ""))
        '        Next
        '        'stepCounter += 1
        '        'aStep.StepCounter = stepCounter
        '        CollectionOfSteps.Add(aStep)
        '        aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
        '        stepsAsSignalHierachy.Add(aStep.ThisStepInputsAsSignalHerachyByType)
        '        If TypeOf (aStep) Is Customization Then
        '            aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
        '        End If
        '        'stepsOutputAsSignalHierachy.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
        '    Next
        'Next
        'DataConfigure.CollectionOfSteps = CollectionOfSteps
        'GroupedSignalByStepsInput = stepsAsSignalHierachy
        ''GroupedSignalByStepsOutput = stepsOutputAsSignalHierachy
    End Sub
    Private Sub _readStages()
        Dim CollectionOfSteps As New ObservableCollection(Of Object)
        Dim stepsAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
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
                Dim params = From ps In element.<Parameters>.Elements Select ps
                For Each pair In params
                    Dim aPair As New ParameterValuePair
                    Dim paraName = pair.Name.ToString
                    If TypeOf (aStep) Is Customization AndAlso (paraName = "term" Or paraName = "factor" Or paraName = "signal") Then
                        Dim signal = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
                        If signal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(signal) Then
                            aStep.InputChannels.Add(signal)
                        ElseIf paraName = "signal" Then
                            signal = New SignalSignatures("", "")
                        End If
                        Dim custSignalName = pair.<CustName>.Value
                        If Not String.IsNullOrEmpty(custSignalName) Then
                            'Dim ext = custSignalName.LastIndexOf(".")
                            'If custSignalName.Substring(ext + 1).ToLower = "mag" Then
                            '    custSignalName = custSignalName.Substring(0, custSignalName.Length - 2)
                            'End If
                            'Dim parts = custSignalName.Split(".")
                            'Dim last2Letters = parts(-2).Substring(-2).ToArray
                            'If parts(-1).ToLower = "mag" Then
                            '    Dim signalType = last2Letters(0) & "M" & last2Letters(1)
                            'End If                                
                            If Not outputInputNameDictionary.ContainsKey(custSignalName) Then
                                outputInputNameDictionary(custSignalName) = New List(Of SignalSignatures)
                                If signal IsNot Nothing Then
                                    outputInputNameDictionary(custSignalName).Add(signal)
                                    'Else
                                    '    outputInputNameDictionary(custSignalName).Add(New SignalSignatures())
                                End If
                            Else
                                Throw New Exception("Duplicate custom signal name " & custSignalName & " found in this step!")
                            End If
                            'aStep.CustSignalName.Add(custSignalName)
                        End If
                    ElseIf TypeOf (aStep) Is Customization And (paraName = "minuend" Or paraName = "dividend") Then
                        aStep.MinuendOrDivident = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
                        If aStep.MinuendOrDivident IsNot Nothing Then
                            aStep.InputChannels.Add(aStep.MinuendOrDivident)
                        End If
                    ElseIf TypeOf (aStep) Is Customization And (paraName = "subtrahend" Or paraName = "divisor") Then
                        aStep.SubtrahendOrDivisor = _searchForSignalInTaggedSignals(pair.<PMU>.Value, pair.<Channel>.Value)
                        If aStep.SubtrahendOrDivisor IsNot Nothing Then
                            aStep.InputChannels.Add(aStep.SubtrahendOrDivisor)
                        End If
                    ElseIf TypeOf (aStep) Is Customization And paraName = "CustPMUname" Then
                        aStep.CustPMUname = pair.Value
                        _lastCustPMUname = pair.Value
                    ElseIf TypeOf (aStep) Is Customization And paraName = "SignalName" Then
                        aStep.CustSignalName.Add(pair.Value)
                    ElseIf TypeOf (aStep) Is Customization And paraName = "CustName" Then
                        aStep.CustSignalName.Add(pair.Value)
                    ElseIf TypeOf aStep Is Customization And paraName = "phasor" Then
                        'Dim phasors = From x In pair.<phasor>.Elements Select x
                        'For Each phasor In phasors
                        Dim magSignal = _searchForSignalInTaggedSignals(pair.<mag>.<PMU>.Value, pair.<mag>.<Channel>.Value)
                        If magSignal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(magSignal) Then
                            aStep.InputChannels.Add(magSignal)
                        End If
                        Dim angSignal = _searchForSignalInTaggedSignals(pair.<ang>.<PMU>.Value, pair.<ang>.<Channel>.Value)
                        If angSignal IsNot Nothing AndAlso Not aStep.InputChannels.Contains(angSignal) Then
                            aStep.InputChannels.Add(angSignal)
                        End If
                        Dim custSignalName = pair.<CustName>.Value
                        If Not String.IsNullOrEmpty(custSignalName) Then
                            'Dim ext = custSignalName.LastIndexOf(".")
                            'If custSignalName.Substring(ext + 1).ToLower = "mag" Then
                            '    custSignalName = custSignalName.Substring(0, custSignalName.Length - 2)
                            'End If
                            'Dim parts = custSignalName.Split(".")
                            'Dim last2Letters = parts(-2).Substring(-2).ToArray
                            'If parts(-1).ToLower = "mag" Then
                            '    Dim signalType = last2Letters(0) & "M" & last2Letters(1)
                            'End If
                            If Not outputInputNameDictionary.ContainsKey(custSignalName) Then
                                outputInputNameDictionary(custSignalName) = New List(Of SignalSignatures)
                                outputInputNameDictionary(custSignalName).Add(magSignal)
                                outputInputNameDictionary(custSignalName).Add(angSignal)
                            Else
                                Throw New Exception("Duplicate custom signal name " & custSignalName & " found in this step!")
                            End If
                            'aStep.CustSignalName.Add(custSignalName)
                        End If
                        'Next
                    Else
                        aPair.ParameterName = paraName
                        If pair.Value.ToLower = "false" Then
                            aPair.Value = False
                        ElseIf pair.Value.ToLower = "true" Then
                            aPair.Value = True
                        ElseIf aStep.Name = "Nominal-Value Frequency Data Quality Filter" And pair.Value = "FlagBit" Then
                            aPair.IsRequired = False
                            aPair.Value = pair.Value
                        Else
                            aPair.Value = pair.Value
                        End If
                        aStep.Parameters.Add(aPair)
                    End If
                    necessaryParams.Remove(paraName)
                Next
                'If TypeOf (aStep) Is Customization Then
                '    aStep.CustPMUname = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "CustPMUname" Select x.Value).FirstOrDefault
                '    aStep.CustSignalName = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "SignalName" Select x.Value).FirstOrDefault
                '    If element.<Parameters>.<CustName>.Value Then
                '        aStep.CustSignalName = (From x In DirectCast(aStep, Customization).Parameters Where x.ParameterName = "CustName" Select x.Value).FirstOrDefault
                '    End If
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
                            aStep.OutputChannels.Add(New SignalSignatures(name, aStep.CustPMUname, type))
                        Else
                            aStep.OutputChannels.Add(New SignalSignatures(name, aStep.CustPMUname))
                        End If
                    Next
                End If
                If TypeOf aStep Is Customization AndAlso outputInputNameDictionary.Count > 0 AndAlso Not String.IsNullOrEmpty(aStep.CustPMUname) Then
                    For Each pair In outputInputNameDictionary
                        Dim newOutputSignal = New SignalSignatures(pair.Key, aStep.CustPMUname, "C")
                        Select Case aStep.Name
                            Case "Addition Customization"
                                newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                            Case "Subtraction Customization"
                                newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                            Case "Multiplication Customization"
                                newOutputSignal.TypeAbbreviation = "O"
                            Case "Division Customization"
                                newOutputSignal.TypeAbbreviation = "O"
                            Case "Raise signals to an exponent"
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

                            Case "Power Calculation Customization"

                            Case "Specify Signal Type and Unit Customization"

                            Case "Metric Prefix Customization"
                                newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                            Case "Angle Conversion Customization"
                                newOutputSignal.TypeAbbreviation = pair.Value.FirstOrDefault.TypeAbbreviation
                            Case Else
                                Throw New Exception("Customization step not supported!")
                        End Select
                        aStep.OutputChannels.Add(newOutputSignal)
                        If Not aStep.OutputInputMappingDictionary.containsKey(newOutputSignal) Then
                            aStep.OutputInputMappingDictionary(newOutputSignal) = New ObservableCollection(Of SignalSignatures)
                            For Each signal In pair.Value
                                aStep.OutputInputMappingDictionary(newOutputSignal).Add(signal)
                            Next
                        Else
                            Throw New Exception("Duplicate custom signal name " & pair.Key & " found in this step!")
                        End If
                    Next
                End If
                'End If
                For Each parameter In necessaryParams
                    If parameter = "CustPMUname" Then
                        aStep.Parameters.Add(New ParameterValuePair(parameter, _lastCustPMUname))
                    Else
                        aStep.Parameters.Add(New ParameterValuePair(parameter, ""))
                    End If
                Next
                'stepCounter += 1
                'aStep.StepCounter = stepCounter
                CollectionOfSteps.Add(aStep)
                aStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(aStep.InputChannels)
                stepsAsSignalHierachy.Add(aStep.ThisStepInputsAsSignalHerachyByType)
                If TypeOf (aStep) Is Customization Then
                    aStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(aStep.OutputChannels)
                End If
                'stepsOutputAsSignalHierachy.Add(aStep.ThisStepOutputsAsSignalHierachyByPMU)
            Next
        Next
        DataConfigure.CollectionOfSteps = CollectionOfSteps
        GroupedSignalByStepsInput = stepsAsSignalHierachy

    End Sub

    Private _lastCustPMUname As String

    Private Function _searchForSignalInTaggedSignals(pmu As String, channel As String) As SignalSignatures
        Dim target = pmu & "." & channel.Split(".")(1) & "." & channel.Split(".")(2)
        For Each group In GroupedSignalsByPMU
            For Each p In group.SignalList
                If p.SignalSignature.PMUName = pmu Then
                    For Each signal In p.SignalList
                        If signal.SignalSignature.SignalName = target Then
                            Return signal.SignalSignature
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
            _readStages()
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
            MessageBox.Show("Error reading input data directory!" & Environment.NewLine & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
        If String.IsNullOrEmpty(_sampleFile) Then
            MessageBox.Show("No file of type: " & fileInfo.FileType.ToString & vbCrLf & " is found in: " & fileInfo.FileDirectory, "Error!", MessageBoxButtons.OK)
        Else
            Try
                _readFirstDataFile(_sampleFile, fileInfo)
                If fileInfo.FileType.ToString = "pdat" Then
                    _tagSignals(fileInfo, fileInfo.SignalList)
                End If
            Catch ex As Exception
                MessageBox.Show("Error sampling input data file!" & Environment.NewLine & ex.Message, "Error!", MessageBoxButtons.OK)
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


        For Each parameter In DataConfigure.CustomizationNameParemetersDictionary(newCustomization.Name)
            'If parameter = "SetToNaN" Or parameter = "FlagAllByFreq" Then
            '    newCustomization.Parameters.Add(New ParameterValuePair(parameter, False))
            'ElseIf newCustomization.Name = "Nominal-Value Frequency Data Quality Filter" And parameter = "FlagBit" Then
            '    newCustomization.Parameters.Add(New ParameterValuePair(parameter, False, False))
            'Else
            newCustomization.Parameters.Add(New ParameterValuePair(parameter, ""))
            'End If
        Next


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
    'Private Sub _changeSignalSelectionByType(obj As SignalSignatures)
    '    If _currentSelectedStep IsNot Nothing Then
    '        If Not String.IsNullOrEmpty(obj.PMUName) Then
    '            ' test for all parent, this must be leaf node in the tree, so check both pmu parent tree and type parent tree to change parent's check status
    '            Try
    '                _checkParentStatus(obj)
    '                _checkPMUParentStaus(obj)

    '                _addOrDeleteSignal(obj, obj.IsChecked)
    '                'If _currentSelectedStep IsNot Nothing Then
    '                '    _currentSelectedStep.InputChannels.Add(obj)
    '                'End If
    '            Catch ex As Exception
    '                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
    '            End Try
    '        ElseIf obj.TypeAbbreviation.Length = 1 Then
    '            ' check all children, this must be the top most node of the type tree, so only need to check children, no parent
    '            Try
    '                For Each group In GroupedSignalsByType
    '                    If group.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation Then
    '                        _checkAllChildren(group, obj.IsChecked)
    '                    End If
    '                Next
    '            Catch ex As Exception
    '                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
    '            End Try
    '        ElseIf obj.TypeAbbreviation.Length = 2 Then
    '            ' check all children and test all parents, this must be the 2nd level (M or A) node of V and I
    '            Try
    '                For Each group In GroupedSignalsByType
    '                    If group.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation.First Then
    '                        For Each subgroup In group.SignalList
    '                            If subgroup.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation Then
    '                                _checkAllChildren(subgroup, obj.IsChecked)
    '                            End If
    '                        Next
    '                    End If
    '                Next
    '                ' after check/uncheck all children recursively, need to change its parent check/uncheck status only in the type tree
    '                _checkParentStatus(obj)
    '            Catch ex As Exception
    '                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
    '            End Try
    '        ElseIf obj.TypeAbbreviation.Length = 3 Then
    '            ' check all children and test all parents, this is the 3rd level (P, A, B or C) node of V and I
    '            Try
    '                For Each group In GroupedSignalsByType
    '                    If group.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation.First Then
    '                        For Each subgroup In group.SignalList
    '                            If subgroup.SignalSignature.TypeAbbreviation.ToArray(1) = obj.TypeAbbreviation.ToArray(1) Then
    '                                For Each subsubgroup In subgroup.SignalList
    '                                    If subsubgroup.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation Then
    '                                        _checkAllChildren(subsubgroup, obj.IsChecked)
    '                                    End If
    '                                Next
    '                            End If
    '                        Next
    '                    End If
    '                Next
    '                _checkParentStatus(obj)
    '            Catch ex As Exception
    '                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
    '            End Try
    '        Else
    '            MessageBox.Show("Error! Unknown information about the checked item!" & Environment.NewLine & "Name: " & obj.SignalName & Environment.NewLine & "Type: " & obj.TypeAbbreviation & Environment.NewLine & "PMU: " & obj.PMUName, "Error!", MessageBoxButtons.OK)
    '        End If
    '    Else
    '        obj.IsChecked = False
    '        MessageBox.Show("Please select a step first!", "Error!", MessageBoxButtons.OK)
    '    End If
    'End Sub

    Private _subtractionFocus As Object
    Public Property SubtractionFocus As Object
        Get
            Return _subtractionFocus
        End Get
        Set(ByVal value As Object)
            _subtractionFocus = value
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
    Private _textboxesLostFocus As ICommand
    Public Property TextboxesLostFocus As ICommand
        Get
            Return _textboxesLostFocus
        End Get
        Set(ByVal value As ICommand)
            _textboxesLostFocus = value
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
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Reverse sign of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Take absolute value of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Return real component of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Return imaginary component of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Return angle of complex valued signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Take complex conjugate of signals"
                    For Each inputOutputPair In _currentSelectedStep.OutputInputMappingDictionary
                        If inputOutputPair.Value.Count > 0 Then
                            type = inputOutputPair.Value(0).TypeAbbreviation
                        End If
                        inputOutputPair.Key.TypeAbbreviation = type
                    Next
                Case "Phasor Creation Customization"

                Case "Power Calculation Customization"
                Case "Specify Signal Type and Unit Customization"
                Case "Metric Prefix Customization"
                Case "Angle Conversion Customization"
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
    Private Sub _recoverCheckStatusOfCurrentStep(obj As Object)
        For Each signal In obj.InputChannels
            signal.IsChecked = True
        Next
        _determineAllParentNodeStatus()
    End Sub
    Private Sub _setFocusedTextbox(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 And (obj.SignalSignature.PMUName Is Nothing Or obj.SignalSignature.TypeAbbreviation Is Nothing) Then    'if selected a group of signal
            _determineParentCheckStatus(obj)
            Throw New Exception("Error! Please select valid signal for this textbox! We need a single signal, cannot be group of signals!")
        Else
            If _currentSelectedStep.CurrentCursor = "" Then ' if no textbox selected, textbox lost it focus right after a click any where else, so only click immediate follow a textbox selection would work
                If obj.SignalSignature.IsChecked Then       ' reverse the click status to go back to the original checkbox status
                    obj.SignalSignature.IsChecked = False
                Else
                    obj.SignalSignature.IsChecked = True
                End If
                For Each signal In _currentSelectedStep.InputChannels
                    signal.IsChecked = True
                Next
                _determineAllParentNodeStatus()
                Throw New Exception("Error! Please select a valid text box for this input signal!")
            ElseIf _currentSelectedStep.CurrentCursor = "MinuendOrDivident" Then
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
                        '_currentSelectedStep.MinuendOrDivident.IsChecked = False
                        _currentSelectedStep.MinuendOrDivident = Nothing
                        'Else                                                                    ' if the content is not the same as the clicked item, then the clicked item must be the same as the divisor or subtrahend, so we cannot uncheck it
                        '    If _currentSelectedStep.MinuendOrDivident IsNot Nothing Then        ' if the content of the text box is not empty, we need to set it old item's check mark to be false
                        '        _currentSelectedStep.MinuendOrDivident.IsChecked = False
                        '    End If
                        '    obj.SignalSignature.IsChecked = True
                        '    _currentSelectedStep.MinuendOrDivident = obj.SignalSignature
                        '    If Not _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                        '        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                        '    End If
                    End If
                End If
                _currentSelectedStep.CurrentCursor = ""
                _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
                _determineAllParentNodeStatus()
            ElseIf _currentSelectedStep.CurrentCursor = "SubtrahendOrDivisor" Then
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
                        '_currentSelectedStep.SubtrahendOrDivisor.IsChecked = False
                        _currentSelectedStep.SubtrahendOrDivisor = Nothing
                        '_currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                        'Else
                        '    If _currentSelectedStep.SubtrahendOrDivisor IsNot Nothing Then
                        '        _currentSelectedStep.SubtrahendOrDivisor.IsChecked = False
                        '    End If
                        '    obj.SignalSignature.IsChecked = True
                        '    _currentSelectedStep.SubtrahendOrDivisor = obj.SignalSignature
                        '    If Not _currentSelectedStep.InputChannels.Contains(obj.SignalSignature) Then
                        '        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                        '    End If
                    End If
                End If
                _currentSelectedStep.CurrentCursor = ""
                _currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
                _determineAllParentNodeStatus()
            End If
        End If
    End Sub
    Private Sub _curentFocusedTextBoxChanged(obj As SignalSignatures)
        For Each signal In _currentSelectedStep.InputChannels
            signal.IsChecked = False
        Next
        If obj IsNot Nothing Then
            obj.IsChecked = True
        End If
        '_currentSelectedStep.ThisStepInputsAsSignalHerachyByType.SignalList = SortSignalByType(_currentSelectedStep.InputChannels)
        'If TypeOf (_currentSelectedStep) Is Customization Then
        '    _currentSelectedStep.ThisStepOutputsAsSignalHierachyByPMU.SignalList = SortSignalByPMU(_currentSelectedStep.OutputChannels)
        'End If
        _determineAllParentNodeStatus()
        '_determineFileDirCheckableStatus()
    End Sub
    Private Sub _signalSelected(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count < 1 And (obj.SignalSignature.PMUName Is Nothing Or obj.SignalSignature.TypeAbbreviation Is Nothing) Then
                obj.SignalSignature.IsChecked = False
                MessageBox.Show("Clicked item is not a valid signal, or contains no valid signal!", "Error!", MessageBoxButtons.OK)
            Else
                If TypeOf _currentSelectedStep Is DQFilter Then
                    Try
                        _changeSignalSelection(obj)
                    Catch ex As Exception
                        obj.SignalSignature.IsChecked = False
                        MessageBox.Show("Error selecting signal(s) for data quality filter!" & vbCrLf & ex.Message, "Error!", MessageBoxButtons.OK)
                    End Try
                Else
                    Try
                        Select Case _currentSelectedStep.Name
                            Case "Scalar Repetition Customization"
                                obj.SignalSignature.IsChecked = False
                                Throw New Exception("Please do NOT select signals for Scalar Repetition Customization!")
                            Case "Addition Customization"
                                _changeSignalSelection(obj)
                                _CheckOutputType()
                            Case "Subtraction Customization"
                                _setFocusedTextbox(obj)
                                _CheckOutputType()
                            Case "Multiplication Customization"
                                _changeSignalSelection(obj)
                                _CheckOutputType()
                            Case "Division Customization"
                                _setFocusedTextbox(obj)
                                _CheckOutputType()
                            Case "Raise signals to an exponent"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Reverse sign of signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Take absolute value of signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Return real component of signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Return imaginary component of signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Return angle of complex valued signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Take complex conjugate of signals"
                                _changeSignalSelectionUnarySteps(obj)
                                _CheckOutputType()
                            Case "Phasor Creation Customization"

                            Case "Power Calculation Customization"
                            Case "Specify Signal Type and Unit Customization"
                            Case "Metric Prefix Customization"
                            Case "Angle Conversion Customization"
                            Case Else
                                Throw New Exception("Customization step not supported!")
                        End Select
                    Catch ex As Exception
                        MessageBox.Show("Error selecting signal(s) for customization step!" & ex.Message, "Error!", MessageBoxButtons.OK)
                    End Try
                End If
            End If
        Else
            obj.SignalSignature.IsChecked = False
            MessageBox.Show("Please select a step first!", "Error!", MessageBoxButtons.OK)
        End If
    End Sub

    Private Sub _changeSignalSelectionUnarySteps(obj As SignalTypeHierachy)
        If obj.SignalSignature.IsChecked Then
            _checkAllChildren(obj, obj.SignalSignature.IsChecked)
            _addOrDeleteInputSignal(obj, obj.SignalSignature.IsChecked)
            _addOuputSignals(obj)
        Else
            _removeMatchingInputOutputSignals(obj)
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
        Else ' if a leaf node, then check the pmu parent tree
            '_checkPMUParentStaus(node.SignalSignature)

            '_addOrDeleteSignal(node.SignalSignature, isChecked)
            'If _currentSelectedStep IsNot Nothing Then
            '    _currentSelectedStep.InputChannels.Add(node.SignalSignature)
            'End If
        End If
    End Sub

    ''' <summary>
    ''' This sub check the signal grouped by type parent tree
    ''' the status of each node is decided by its children
    ''' so need to decide lower level node's status then go up the tree
    ''' </summary>
    ''' <param name="node"></param>
    'Private Sub _checkParentStatus(ByRef node As SignalSignatures)
    '    If node.TypeAbbreviation.Length <> 3 Then
    '        ' 1 parent, could be leaf of F, R, P, Q, D, or first level children (M or A) of V and I
    '        For Each group In GroupedSignalsByType
    '            If group.SignalSignature.TypeAbbreviation.First = node.TypeAbbreviation.First Then
    '                _determineParentCheckStatus(group)
    '            End If
    '        Next
    '    Else
    '        If String.IsNullOrEmpty(node.PMUName) Then
    '            ' 2 parents, P, A, B, C under V and I
    '            For Each group In GroupedSignalsByType
    '                If group.SignalSignature.TypeAbbreviation.First = node.TypeAbbreviation.First Then
    '                    For Each subgroup In group.SignalList
    '                        If subgroup.SignalSignature.TypeAbbreviation = node.TypeAbbreviation.Substring(0, 2) Then
    '                            _determineParentCheckStatus(subgroup)
    '                        End If
    '                    Next
    '                    _determineParentCheckStatus(group)
    '                End If
    '            Next
    '        Else
    '            ' 3 parents, leaf nodes of V and I
    '            For Each group In GroupedSignalsByType
    '                If group.SignalSignature.TypeAbbreviation.First = node.TypeAbbreviation.First Then
    '                    For Each subgroup In group.SignalList
    '                        If subgroup.SignalSignature.TypeAbbreviation = node.TypeAbbreviation.Substring(0, 2) Then
    '                            For Each subsubgroup In subgroup.SignalList
    '                                If subsubgroup.SignalSignature.TypeAbbreviation = node.TypeAbbreviation Then
    '                                    _determineParentCheckStatus(subsubgroup)
    '                                End If
    '                            Next
    '                            _determineParentCheckStatus(subgroup)
    '                        End If
    '                    Next
    '                    _determineParentCheckStatus(group)
    '                End If
    '            Next
    '        End If
    '    End If
    'End Sub
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

    Private _stepSelected As ICommand
    Public Property StepSelected As ICommand
        Get
            Return _stepSelected
        End Get
        Set(ByVal value As ICommand)
            _stepSelected = value
        End Set
    End Property

    'Private _currentSelectedStep As SignalProcessStep
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
    Private Sub _stepSelectedToEdit(processStep As Object)
        ' if processStep is already selected, then the selection is not changed, nothing needs to be done.
        ' however, if processStep is not selected, which means a new selection, we need to find the old selection, unselect it and all it's input signal
        If Not processStep.IsStepSelected Then
            'Dim isFirstSelection = True
            Dim lastNumberOfSteps = processStep.StepCounter
            Dim stepsOutputAsSignalHierachy As New ObservableCollection(Of SignalTypeHierachy)
            For Each stp In DataConfigure.CollectionOfSteps
                If stp.IsStepSelected Then
                    'isFirstSelection = False
                    stp.IsStepSelected = False
                    For Each signal In stp.InputChannels
                        signal.IsChecked = False
                        ' these might not be necessary
                        '_checkParentStatus(signal)
                        '_checkPMUParentStaus(signal)
                    Next
                    If TypeOf (stp) Is Customization Then
                        For Each signal In stp.OutputChannels
                            signal.IsChecked = False
                        Next
                    End If
                End If
                If TypeOf (stp) Is Customization And stp.StepCounter < lastNumberOfSteps Then
                    stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsAsSignalHierachyByPMU)
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

            GroupedSignalByStepsOutput = stepsOutputAsSignalHierachy
            _determineAllParentNodeStatus()
            _determineFileDirCheckableStatus()
            CurrentSelectedStep = processStep
        End If
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
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByStepsInput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalByStepsOutput, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalsByPMU, False)
            _changeCheckStatusAllParentsOfGroupedSignal(GroupedSignalsByType, False)
            _currentSelectedStep.IsStepSelected = False
            CurrentSelectedStep = Nothing
            _determineFileDirCheckableStatus()
        End If
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

    Private Sub _removeMatchingInputOutputSignals(obj As SignalTypeHierachy)
        If obj.SignalList.Count > 0 Then
            'obj.SignalSignature.IsChecked = False
            For Each child In obj.SignalList
                _removeMatchingInputOutputSignals(child)
            Next
        Else
            obj.SignalSignature.IsChecked = True
            For Each pair In _currentSelectedStep.OutputInputMappingDictionary
                If pair.Value(0).SignalName = obj.SignalSignature.SignalName Then
                    _currentSelectedStep.OutputChannels.Remove(pair.Key)
                    _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                    _currentSelectedStep.OutputInputMappingDictionary.Remove(pair.Key)
                    obj.SignalSignature.IsChecked = False
                    Exit For
                End If
            Next
            'For Each signal In _currentSelectedStep.OutputChannels
            '    If signal.SignalName = obj.SignalSignature.SignalName Then
            '        _currentSelectedStep.OutputChannels.Remove(signal)
            '        _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
            '        obj.SignalSignature.IsChecked = False
            '        Exit For
            '    End If
            'Next
        End If
    End Sub

    Private _selectedOutputSignalChanged As ICommand
    Public Property SelectedOutputSignalChanged As ICommand
        Get
            Return _selectedOutputSignalChanged
        End Get
        Set(ByVal value As ICommand)
            _selectedOutputSignalChanged = value
        End Set
    End Property

    Private Sub _outputSignalSelectionChanged(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count = 0 AndAlso (String.IsNullOrEmpty(obj.SignalSignature.PMUName) Or obj.SignalSignature.PMUName = obj.SignalSignature.SignalName) Then
                obj.SignalSignature.IsChecked = False
                MessageBox.Show("Ivalid signal selection", "Error!", MessageBoxButtons.OK)
            Else

            End If
        Else
            obj.SignalSignature.IsChecked = False
            MessageBox.Show("Please select a step first!", "Error!", MessageBoxButtons.OK)
        End If
        ' if current step is not null
        ' if is checking
        ' or if is un-checking.................

    End Sub

    Private Sub _addOuputSignals(obj As SignalTypeHierachy)
        If _currentSelectedStep IsNot Nothing Then
            If obj.SignalList.Count > 0 Then
                For Each child In obj.SignalList
                    _addOuputSignals(child)
                Next
            Else
                'If isChecked Then
                Dim newOutput = New SignalSignatures(obj.SignalSignature.SignalName, _currentSelectedStep.CustPMUname, "C")
                newOutput.IsChecked = True
                _currentSelectedStep.outputChannels.Add(newOutput)
                If Not _currentSelectedStep.OutputInputMappingDictionary.ContainsKey(newOutput) Then
                    _currentSelectedStep.OutputInputMappingDictionary(newOutput) = New ObservableCollection(Of SignalSignatures)
                    _currentSelectedStep.OutputInputMappingDictionary(newOutput).Add(obj.SignalSignature)
                End If
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
                        _currentSelectedStep.InputChannels.Add(obj.SignalSignature)
                    Else
                        _currentSelectedStep.InputChannels.Remove(obj.SignalSignature)
                    End If
                End If
            End If
        End If
    End Sub

    'Private _signalSelectedFromThisStep As ICommand
    'Public Property SignalSelectedFromThisStep As ICommand
    '    Get
    '        Return _signalSelectedFromThisStep
    '    End Get
    '    Set(ByVal value As ICommand)
    '        _signalSelectedFromThisStep = value
    '    End Set
    'End Property

    'Private Sub _addSignalFromThisStepToCurrentStep(obj As SignalProcessStep)
    '    For Each hierachy In obj.InputChannelsSortedByType
    '        hierachy.SignalSignature.IsChecked = obj.AreSignalSelected
    '        _changeSignalSelection(hierachy)
    '    Next
    'End Sub

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

End Class
