Imports Microsoft.Win32
Imports BAWGUI.Results.ViewModels

Public Class MainViewModel
    Inherits ViewModelBase

    Public Sub New()
        _showSettingsWindow = New DelegateCommand(AddressOf ShowSettings, AddressOf CanExecute)
        _openFile = New DelegateCommand(AddressOf OpenFileFunc, AddressOf CanExecute)
        _settingsView = New SettingsViewModel
        _resultsView = New ResultsViewModel
        _currentView = _settingsView
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
            _settingsWin.DataContext = SettingsView
            _settingsWin.Show()
        End If
    End Sub
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
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"

        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

        End If
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
    Private _resultsView As ResultsViewModel
    Public Property ResultsView As ResultsViewModel
        Get
            Return _resultsView
        End Get
        Set(ByVal value As ResultsViewModel)
            _resultsView = value
            OnPropertyChanged()
        End Set
    End Property
    Private _settingsView As SettingsViewModel
    Public Property SettingsView As SettingsViewModel
        Get
            Return _settingsView
        End Get
        Set(ByVal value As SettingsViewModel)
            _settingsView = value
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
            CurrentView = ResultsView
        Else
            CurrentViewName = "Settings"
            CurrentView = SettingsView
        End If
    End Sub
End Class
