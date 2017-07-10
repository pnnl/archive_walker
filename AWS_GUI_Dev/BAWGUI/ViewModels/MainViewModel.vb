Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles
Imports BAWGUI.Results.ViewModels

Public Class MainViewModel
    Inherits ViewModelBase

    Public Sub New()
        _resultsViewModel = New ResultsViewModel()
        _showSettingsWindow = New DelegateCommand(AddressOf ShowSettings, AddressOf CanExecute)
        _openFile = New DelegateCommand(AddressOf OpenFileFunc, AddressOf CanExecute)
        _settingsViewModel = New SettingsViewModel
        _currentView = _resultsViewModel
        _currentViewName = "Settings"
        _toggleResultsSettings = New DelegateCommand(AddressOf _switchView, AddressOf CanExecute)
    End Sub

    Private _settingsWin As SettingsWindow

    Private _showSettingsWindow As ICommand
    Public Property ShowSettingsWindow As ICommand
        Get
            Return _showSettingsWindow
        End Get
        Set(ByVal value As ICommand)
            _showSettingsWindow = value
        End Set
    End Property
    Private Sub ShowSettings()
        Dim isWindowOpen = False
        For Each w In Application.Current.Windows
            If w Is _settingsWin Then
                isWindowOpen = True
                w.Activate()
            End If
        Next
        If Not isWindowOpen Then
            _settingsWin = New SettingsWindow
            _settingsWin.DataContext = SettingsViewModel
            _settingsWin.Show()
        End If
    End Sub
    Private Property _resultsViewModel As ResultsViewModel
    Public Property ResultsViewModel As ResultsViewModel
        Get
            Return _resultsViewModel
        End Get
        Set(value As ResultsViewModel)
            _resultsViewModel = value
        End Set
    End Property


    Private _openFile As ICommand
    Public Property OpenFile As ICommand
        Get
            Return _openFile
        End Get
        Set(ByVal value As ICommand)
            _openFile = value
        End Set
    End Property
    Private Sub OpenFileFunc()
        Dim openDirectoryDialog As New FolderBrowserDialog()
        openDirectoryDialog.Description = "Select the directory that result xml files are located "
        Dim folderName = SettingsViewModel.DetectorConfigure.EventPath
        If Directory.Exists(folderName) Then
            openDirectoryDialog.SelectedPath = folderName
        Else
            openDirectoryDialog.SelectedPath = Environment.CurrentDirectory
        End If
        openDirectoryDialog.ShowNewFolderButton = False
        If (openDirectoryDialog.ShowDialog = DialogResult.OK) Then
            Dim selectedInputFolderInfo As New DirectoryInfo(openDirectoryDialog.SelectedPath)
            Dim filenames = selectedInputFolderInfo.GetFiles
            filenames = filenames.OrderBy(Function(x) x.Name).ToArray
            Dim files = New List(Of String)
            Dim dates = New List(Of String)
            For Each file In filenames
                Dim nameFragment = file.Name.Split(New Char() {".", "_"})
                If file.Extension.ToLower = ".xml" And nameFragment.Length = 3 Then
                    Dim datestr = nameFragment(1)
                    Try
                        Date.ParseExact(datestr, "yyMMdd", CultureInfo.InvariantCulture)
                        dates.Add(datestr)
                        files.Add(file.FullName)
                    Catch ex As Exception
                        If file.Name.ToLower = "eventlist_current.xml" Then
                            files.Add(file.FullName)
                        End If
                    End Try
                End If
            Next
            If Not files.Count = dates.Count Then
                dates.Sort()
                Dim lastDate = dates.LastOrDefault
                dates.Add((Convert.ToInt32(lastDate) + 1).ToString)
            End If
            'dates.Sort()
            'Dim startDate As String
            'Dim endDate As String
            'If Not String.IsNullOrEmpty(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeStart) Then
            '    startDate = Date.Parse(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeStart).ToString("yyMMdd")
            'Else
            '    startDate = dates.FirstOrDefault
            'End If
            'If Not String.IsNullOrEmpty(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeEnd) Then
            '    endDate = Date.Parse(SettingsViewModel.DataConfigure.ReaderProperty.DateTimeEnd).ToString("yyMMdd")
            'Else
            '    endDate = dates.LastOrDefault
            'End If
            Try
                '_resultsViewModel.LoadResults(files, startDate, endDate)
                _resultsViewModel.LoadResults(files, dates)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
        'Dim openFileDialog As New System.Windows.Forms.OpenFileDialog()
        'openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"

        'If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
        '    Try
        '        _resultsViewModel.LoadResults(openFileDialog.FileName)
        '    Catch ex As Exception
        '        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    End Try
        'End If
    End Sub
    Private _currentView As Object
    Public Property CurrentView As Object
        Get
            Return _currentView
        End Get
        Set(ByVal value As Object)
            _currentView = value
            OnPropertyChanged()
        End Set
    End Property
    Private _settingsViewModel As SettingsViewModel
    Public Property SettingsViewModel As SettingsViewModel
        Get
            Return _settingsViewModel
        End Get
        Set(ByVal value As SettingsViewModel)
            _settingsViewModel = value
            OnPropertyChanged()
        End Set
    End Property
    Private _currentViewName As String
    Public Property CurrentViewName As String
        Get
            Return _currentViewName
        End Get
        Set(ByVal value As String)
            _currentViewName = value
            OnPropertyChanged()
        End Set
    End Property
    Private _toggleResultsSettings As ICommand
    Public Property ToggleResultsSettings As ICommand
        Get
            Return _toggleResultsSettings
        End Get
        Set(ByVal value As ICommand)
            _toggleResultsSettings = value
        End Set
    End Property
    Private Sub _switchView(obj As Object)
        If CurrentViewName = "Settings" Then
            CurrentViewName = "Results"
            CurrentView = SettingsViewModel
        Else
            CurrentViewName = "Settings"
            CurrentView = ResultsViewModel
        End If
    End Sub
End Class
