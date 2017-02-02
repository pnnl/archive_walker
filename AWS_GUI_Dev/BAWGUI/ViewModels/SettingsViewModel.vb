Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Forms
'Imports BAWGUI.DataConfig

Public Class SettingsViewModel
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Raise property changed event
    ''' </summary>
    ''' <param name="sender">The event sender</param>
    ''' <param name="e">The event</param>
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    Private Sub OnPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub

    Public Sub New()
        _configFileName = ""

        _dataConfigure = New DataConfig
        _processConfigure = New ProcessConfig
        _detectorConfigure = New DetectorConfig

        _openConfigFile = New DelegateCommand(AddressOf openConfigXMLFile, AddressOf CanExecute)
        _browseInputFileDir = New DelegateCommand(AddressOf _browseInputFileFolder, AddressOf CanExecute)
        _fileTypeChanged = New DelegateCommand(AddressOf _rebuildInputFileFolderTree, AddressOf CanExecute)
        _inputFileDirTree = New ObservableCollection(Of Folder)

        _timezoneList = TimeZoneInfo.GetSystemTimeZones
        '_selectedTimeZone = TimeZoneInfo.Utc.ToString
    End Sub

    Private Sub _rebuildInputFileFolderTree()
        Try
            InputFileDirTree = New ObservableCollection(Of Folder)
            InputFileDirTree.Add(New Folder(DataConfigure.ReaderProperty.FileDirectory, DataConfigure.ReaderProperty.FileType.ToString))
        Catch ex As Exception
            MessageBox.Show("Error reading input data directory!" & Environment.NewLine & ex.Message, "Error!", MessageBoxButtons.OK)
        End Try
    End Sub

    Private _lastInputFolderLocation As String

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
            _rebuildInputFileFolderTree()
        End If
    End Sub

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

    Private _openConfigFile As ICommand
    Public Property OpenConfigFile As ICommand
        Get
            Return _openConfigFile
        End Get
        Set(ByVal value As ICommand)
            _openConfigFile = value
        End Set
    End Property

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
        InputFileDirTree = New ObservableCollection(Of Folder)
        InputFileDirTree.Add(New Folder(DataConfigure.ReaderProperty.FileDirectory, type))
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

    Private _fileTypeChanged As ICommand
    Public Property FileTypeChanged As ICommand
        Get
            Return _fileTypeChanged
        End Get
        Set(ByVal value As ICommand)
            _fileTypeChanged = value
        End Set
    End Property

    Private Function CanExecute(ByVal param As Object) As Boolean
        Return True
    End Function
End Class
