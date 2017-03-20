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

    Private _settingsWin As SettingsWindow

    Private Sub ShowSettings()
        Dim isWindowOpen = False
        For Each w In Application.Current.Windows
            If w Is _settingsWin Then
                isWindowOpen = True
                w.Activate
            End If
        Next
        If Not isWindowOpen Then
            Dim settingsVM As New SettingsViewModel
            _settingsWin = New SettingsWindow
            _settingsWin.DataContext = settingsVM
            _settingsWin.Show()
        End If
    End Sub
End Class
