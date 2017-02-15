Imports System.ComponentModel

Public Class MainViewModel
    Inherits ViewModelBase

    Public Sub New()
        _showSettingsWindow = New DelegateCommand(AddressOf ShowSettings, AddressOf CanExecute)
    End Sub
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
        Dim settingsVM As New SettingsViewModel
        Dim settingsWin As New SettingsWindow
        settingsWin.DataContext = settingsVM
        settingsWin.Show()
    End Sub
End Class
