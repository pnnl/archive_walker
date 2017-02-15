Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Forms
Imports PDAT_Reader
'Imports BAWGUI.DataConfig

Public Class SettingsViewModel
    Inherits ViewModelBase

    Public Sub New()
        _configFileName = ""
        _sampleFile = ""

        _dataConfigure = New DataConfig
        _processConfigure = New ProcessConfig
        _detectorConfigure = New DetectorConfig

        _openConfigFile = New DelegateCommand(AddressOf openConfigXMLFile, AddressOf CanExecute)
        _browseInputFileDir = New DelegateCommand(AddressOf _browseInputFileFolder, AddressOf CanExecute)
        _fileTypeChanged = New DelegateCommand(AddressOf _buildInputFileFolderTree, AddressOf CanExecute)
        _dqfilterSelected = New DelegateCommand(AddressOf _dqfilterSelection, AddressOf CanExecute)
        _selectedTypeSignalChanged = New DelegateCommand(AddressOf _changeSignalSelectionByType, AddressOf CanExecute)
        _pmuSignalSelectionChanged = New DelegateCommand(AddressOf _changeSignalSelectionByPMU, AddressOf CanExecute)

        _inputFileDirTree = New ObservableCollection(Of Folder)

        _timezoneList = TimeZoneInfo.GetSystemTimeZones
        '_selectedTimeZone = TimeZoneInfo.Utc.ToString
    End Sub

    Private _sampleFile As String

    Private _taggedSignals As ObservableCollection(Of SignalSignatures)
    Public Property TaggedSignals As ObservableCollection(Of SignalSignatures)
        Get
            Return _taggedSignals
        End Get
        Set(ByVal value As ObservableCollection(Of SignalSignatures))
            _taggedSignals = value
            'PMUSignalDictionary = value.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
            'GroupedSignalsByType = value.GroupBy(Function(x) x.TypeAbbreviation.ToArray(0).ToString).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList)) 'GroupBy(Function(y) y.TypeAbbreviation.ToArray(1).ToString).ToDictionary(Function(y) y.Key, Function(y) y.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString).ToDictionary(Function(z) z.Key, Function(z) z.ToList)
            'Dim a = GroupedSignalsByType("V").GroupBy(Function(x) x.TypeAbbreviation.ToArray(1).ToString).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
            'Dim signalTypeDictionary = value.GroupBy(Function(x) x.TypeAbbreviation.ToArray(0).ToString).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList)) 'GroupBy(Function(y) y.TypeAbbreviation.ToArray(1).ToString).ToDictionary(Function(y) y.Key, Function(y) y.GroupBy(Function(z) z.TypeAbbreviation.ToArray(2).ToString).ToDictionary(Function(z) z.Key, Function(z) z.ToList)
            'For Each signalType In signalTypeDictionary
            '    Select Case signalType
            '        Case "D"

            '        Case 2

            '        Case Else

            '    End Select signalType.Key
            'Next
            OnPropertyChanged()
        End Set
    End Property

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
    Private Sub _sortSignals()
        Dim signalList As New ObservableCollection(Of SignalSignatures)
        Dim signalTypeTree As New ObservableCollection(Of SignalTypeHierachy)
        For Each name In _signalList
            Dim signal As New SignalSignatures
            signal.SignalName = name
            Dim nameParts = name.Split(".")
            signal.PMUName = nameParts(0)
            If nameParts.Length = 3 Then
                Select Case nameParts(2)
                    Case "F"
                        signal.TypeAbbreviation = "F"
                    Case "R"
                        signal.TypeAbbreviation = "R"
                    Case "A"
                        Dim channel = nameParts(1).Substring(nameParts(1).Length - 2).ToArray
                        signal.TypeAbbreviation = channel(0) & "A" & channel(1)
                    Case "M"
                        Dim channel = nameParts(1).Substring(nameParts(1).Length - 2).ToArray
                        signal.TypeAbbreviation = channel(0) & "M" & channel(1)
                    Case Else
                        Throw New Exception("Error! Invalid signal name " & name & " found!")
                End Select
            ElseIf nameParts.Length = 2 Then
                Dim lastLetter = nameParts(1).Last
                Select Case lastLetter
                    Case "V"
                        signal.TypeAbbreviation = "Q"
                    Case "W"
                        signal.TypeAbbreviation = "P"
                    Case "D"
                        signal.TypeAbbreviation = "D"
                    Case Else
                        Throw New Exception("Error! Invalid signal name " & name & " found!")
                End Select
            Else
                Throw New Exception("Error! Invalid signal name " & name & " found!")
            End If
            signalList.Add(signal)
        Next
        TaggedSignals = signalList
        PMUSignalDictionary = signalList.GroupBy(Function(x) x.PMUName).ToDictionary(Function(x) x.Key, Function(x) x.ToList)
        Dim pmuSignalTree = New ObservableCollection(Of SignalTypeHierachy)
        For Each group In PMUSignalDictionary
            Dim newGroup = New SignalTypeHierachy(New SignalSignatures(group.Key))
            'newGroup.SignalSignature.PMUName = group.Key
            For Each signal In group.Value
                newGroup.SignalList.Add(New SignalTypeHierachy(signal))
            Next
            pmuSignalTree.Add(newGroup)
        Next
        GroupedSignalsByPMU = pmuSignalTree
        Dim signalTypeDictionary = signalList.GroupBy(Function(x) x.TypeAbbreviation.ToArray(0).ToString).ToDictionary(Function(x) x.Key, Function(x) New ObservableCollection(Of SignalSignatures)(x.ToList))
        For Each signalType In signalTypeDictionary
            Select Case signalType.Key
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
        GroupedSignalsByType = signalTypeTree
    End Sub

    Private Function _readFirstDataFile() As List(Of String)
        If System.IO.Path.GetExtension(_sampleFile).Substring(1) = "csv" Then
            'Dim CSVSampleFile As New JSIS_CSV_Reader.JSISCSV_Reader
            'Dim signals = CSVSampleFile.OpenCSV4row(_sampleFile)
            Dim fr As FileIO.TextFieldParser = New FileIO.TextFieldParser(_sampleFile)
            fr.TextFieldType = FileIO.FieldType.Delimited
            fr.Delimiters = New String() {","}
            fr.HasFieldsEnclosedInQuotes = True
            fr.ReadLine()
            fr.ReadLine()
            fr.ReadLine()
            Return fr.ReadFields.Skip(1).ToList
        Else
            Dim PDATSampleFile As New PDATReader
            Return PDATSampleFile.GetPDATSignalNameList(_sampleFile)
        End If
    End Function

    Private _signalList As List(Of String)
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
            Try
                _configData = XDocument.Load(_configFileName)
                _readConfigFile()
            Catch ex As Exception
                MessageBox.Show("Error reading config file!" & ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        End If
    End Sub

    Private Sub _readConfigFile()
        'Throw New NotImplementedException()
        DataConfigure.ReaderProperty.FileDirectory = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<FileDirectory>.Value
        Dim type = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<FileType>.Value.ToLower
        DataConfigure.ReaderProperty.FileType = [Enum].Parse(GetType(DataFileType), type)
        'InputFileDirTree = New ObservableCollection(Of Folder)
        'InputFileDirTree.Add(New Folder(DataConfigure.ReaderProperty.FileDirectory, type))
        _buildInputFileFolderTree()

        DataConfigure.ReaderProperty.Mnemonic = _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mnemonic>.Value
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

        Dim CollectionOfSteps As New ObservableCollection(Of SignalProcessStep)
        Dim stepCounter As Integer = 0
        Dim stages = From el In _configData.<Config>.<DataConfig>.<Configuration>.Elements Where el.Name = "Stages" Select el
        For Each el In stages
            Dim steps = From element In _configData.<Config>.<DataConfig>.<Configuration>.<Stages>.Elements Select element
            For Each element In steps
                Dim aStep As SignalProcessStep
                If element.Name = "Filter" Then
                    aStep = New DQFilter
                ElseIf el.Name = "Customization " Then
                    aStep = New Customization
                End If
                aStep.Name = DataConfigure.DQFilterReverseNameDictionary(element.<Name>.Value)
                Dim params = From ps In element.<Parameters>.Elements Select ps
                For Each pair In params
                    Dim aPair As New ParameterValuePair
                    aPair.ParameterName = pair.Name.ToString
                    If pair.Value.ToLower = "false" Then
                        aPair.Value = False
                    ElseIf pair.Value.ToLower = "true" Then
                        aPair.Value = True
                    Else
                        aPair.Value = pair.Value
                    End If
                    aStep.Parameters.Add(aPair)
                Next
                stepCounter += 1
                aStep.StepCounter = stepCounter
                CollectionOfSteps.Add(aStep)
            Next
        Next
        DataConfigure.CollectionOfSteps = CollectionOfSteps
        'Dim results = From el In _configData.<Config>.<DataConfig>.<Configuration>.<ReaderProperties>.<Mode>.<Params>.Elements Select el
        'Dim newParams = New ObservableCollection(Of ParameterValuePair)
        'For Each el In results
        '    newParams.Add(New ParameterValuePair(el.Name.ToString, el.Value))
        'Next
        'DataConfigure.ReaderProperty.ModeParams = newParams
    End Sub
    'Private _lastConfigFileLocation As String

    Private _timezoneList As ReadOnlyCollection(Of TimeZoneInfo)
    Public ReadOnly Property TimeZoneList As ReadOnlyCollection(Of TimeZoneInfo)
        Get
            Return _timezoneList
        End Get
    End Property

    Private _browseInputFileDir As ICommand
    Public Property BrowseInputFileDir As ICommand
        Get
            Return _browseInputFileDir
        End Get
        Set(ByVal value As ICommand)
            _browseInputFileDir = value
        End Set
    End Property

    Private Sub _browseInputFileFolder()
        Dim openDirectoryDialog As New FolderBrowserDialog()
        openDirectoryDialog.Description = "Select the directory that data files (.pdat or .csv) are located "
        If _lastInputFolderLocation Is Nothing Then
            openDirectoryDialog.SelectedPath = Environment.CurrentDirectory
        Else
            openDirectoryDialog.SelectedPath = _lastInputFolderLocation
        End If
        openDirectoryDialog.ShowNewFolderButton = False
        If (openDirectoryDialog.ShowDialog = DialogResult.OK) Then
            _lastInputFolderLocation = openDirectoryDialog.SelectedPath
            DataConfigure.ReaderProperty.FileDirectory = _lastInputFolderLocation
            'InputFileDirTree = New ObservableCollection(Of Folder)
            'InputFileDirTree.Add(New Folder(DataConfigure.ReaderProperty.FileDirectory, DataConfigure.ReaderProperty.FileType.ToString))
            _buildInputFileFolderTree()
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

    Private Sub _buildInputFileFolderTree()
        _sampleFile = ""
        Try
            InputFileDirTree = New ObservableCollection(Of Folder)
            InputFileDirTree.Add(New Folder(DataConfigure.ReaderProperty.FileDirectory, DataConfigure.ReaderProperty.FileType.ToString, _sampleFile))
        Catch ex As Exception
            MessageBox.Show("Error reading input data directory!" & Environment.NewLine & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
        Try
            _signalList = _readFirstDataFile()
            _sortSignals()
        Catch ex As Exception
            MessageBox.Show("Error sampling input data file!" & Environment.NewLine & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
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
            Else
                newFilter.Parameters.Add(New ParameterValuePair(parameter, ""))
            End If
        Next
        'newFilter.Parameters
        DataConfigure.CollectionOfSteps.Add(newFilter)
    End Sub

    Private _selectedTypeSignalChanged As ICommand
    Public Property SelectedTypeSignalChanged As ICommand
        Get
            Return _selectedTypeSignalChanged
        End Get
        Set(ByVal value As ICommand)
            _selectedTypeSignalChanged = value
        End Set
    End Property
    ''' <summary>
    ''' This sub is called when user select signals in the group by type signal tree
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _changeSignalSelectionByType(obj As SignalSignatures)
        If Not String.IsNullOrEmpty(obj.PMUName) Then
            ' test for all parent, this must be leaf node in the tree, so check both pmu parent tree and type parent tree to change parent's check status
            Try
                _checkParentStatus(obj)
                _checkPMUParentStaus(obj)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        ElseIf obj.TypeAbbreviation.Length = 1 Then
            ' check all children, this must be the top most node of the type tree, so only need to check children, no parent
            Try
                For Each group In GroupedSignalsByType
                    If group.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation Then
                        _checkAllChildren(group, obj.IsChecked)
                    End If
                Next
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        ElseIf obj.TypeAbbreviation.Length = 2 Then
            ' check all children and test all parents, this must be the 2nd level (M or A) node of V and I
            Try
                For Each group In GroupedSignalsByType
                    If group.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation.First Then
                        For Each subgroup In group.SignalList
                            If subgroup.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation Then
                                _checkAllChildren(subgroup, obj.IsChecked)
                            End If
                        Next
                    End If
                Next
                ' after check/uncheck all children recursively, need to change its parent check/uncheck status only in the type tree
                _checkParentStatus(obj)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        ElseIf obj.TypeAbbreviation.Length = 3 Then
            ' check all children and test all parents, this is the 3rd level (P, A, B or C) node of V and I
            Try
                For Each group In GroupedSignalsByType
                    If group.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation.First Then
                        For Each subgroup In group.SignalList
                            If subgroup.SignalSignature.TypeAbbreviation.ToArray(1) = obj.TypeAbbreviation.ToArray(1) Then
                                For Each subsubgroup In subgroup.SignalList
                                    If subsubgroup.SignalSignature.TypeAbbreviation = obj.TypeAbbreviation Then
                                        _checkAllChildren(subsubgroup, obj.IsChecked)
                                    End If
                                Next
                            End If
                        Next
                    End If
                Next
                _checkParentStatus(obj)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK)
            End Try
        Else
            MessageBox.Show("Error! Unknown information about the checked item!" & Environment.NewLine & "Name: " & obj.SignalName & Environment.NewLine & "Type: " & obj.TypeAbbreviation & Environment.NewLine & "PMU: " & obj.PMUName, "Error!", MessageBoxButtons.OK)
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
            _checkPMUParentStaus(node.SignalSignature)
        End If
    End Sub
    ''' <summary>
    ''' This sub check the signal grouped by type parent tree
    ''' the status of each node is decided by its children
    ''' so need to decide lower level node's status then go up the tree
    ''' </summary>
    ''' <param name="node"></param>
    Private Sub _checkParentStatus(ByRef node As SignalSignatures)
        If node.TypeAbbreviation.Length <> 3 Then
            ' 1 parent, could be leaf of F, R, P, Q, D, or first level children (M or A) of V and I
            For Each group In GroupedSignalsByType
                If group.SignalSignature.TypeAbbreviation.First = node.TypeAbbreviation.First Then
                    _determineParentCheckStatus(group)
                End If
            Next
        Else
            If String.IsNullOrEmpty(node.PMUName) Then
                ' 2 parents, P, A, B, C under V and I
                For Each group In GroupedSignalsByType
                    If group.SignalSignature.TypeAbbreviation.First = node.TypeAbbreviation.First Then
                        For Each subgroup In group.SignalList
                            If subgroup.SignalSignature.TypeAbbreviation = node.TypeAbbreviation.Substring(0, 2) Then
                                _determineParentCheckStatus(subgroup)
                            End If
                        Next
                        _determineParentCheckStatus(group)
                    End If
                Next
            Else
                ' 3 parents, leaf nodes of V and I
                For Each group In GroupedSignalsByType
                    If group.SignalSignature.TypeAbbreviation.First = node.TypeAbbreviation.First Then
                        For Each subgroup In group.SignalList
                            If subgroup.SignalSignature.TypeAbbreviation = node.TypeAbbreviation.Substring(0, 2) Then
                                For Each subsubgroup In subgroup.SignalList
                                    If subsubgroup.SignalSignature.TypeAbbreviation = node.TypeAbbreviation Then
                                        _determineParentCheckStatus(subsubgroup)
                                    End If
                                Next
                                _determineParentCheckStatus(subgroup)
                            End If
                        Next
                        _determineParentCheckStatus(group)
                    End If
                Next
            End If
        End If
    End Sub
    ''' <summary>
    ''' This sub loop through all children of a hierachy node to determine the node's status of checked/unchecked/indeterminate
    ''' </summary>
    ''' <param name="group"></param>
    Private Sub _determineParentCheckStatus(group As SignalTypeHierachy)
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
    End Sub

    Private _pmuSignalSelectionChanged As ICommand
    Public Property PMUSignalSelectionChanged As ICommand
        Get
            Return _pmuSignalSelectionChanged
        End Get
        Set(ByVal value As ICommand)
            _pmuSignalSelectionChanged = value
        End Set
    End Property
    ''' <summary>
    ''' This sub is called when user change signal selection using the signals grouped by PMU.
    ''' </summary>
    ''' <param name="obj"></param>
    Private Sub _changeSignalSelectionByPMU(obj As SignalSignatures)
        If String.IsNullOrEmpty(obj.TypeAbbreviation) Then
            ' check all children, but do not call the recursive function to avoid infinite loop, check the type parent tree after check each children
            For Each group In GroupedSignalsByPMU
                If group.SignalSignature.SignalName = obj.SignalName Then
                    For Each child In group.SignalList
                        child.SignalSignature.IsChecked = obj.IsChecked
                        _checkParentStatus(child.SignalSignature)
                    Next
                End If
            Next
        Else
            ' test parent, need to test both parent tree
            _checkPMUParentStaus(obj)
            _checkParentStatus(obj)
        End If
    End Sub
    ''' <summary>
    ''' This sub check the pmu parent tree
    ''' </summary>
    ''' <param name="node"></param>
    Private Sub _checkPMUParentStaus(ByRef node As SignalSignatures)
        For Each group In GroupedSignalsByPMU
            If group.SignalSignature.SignalName = node.PMUName Then
                _determineParentCheckStatus(group)
            End If
        Next
    End Sub

End Class
