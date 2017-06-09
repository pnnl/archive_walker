Imports System.ComponentModel
Imports Microsoft.Win32
Imports System.Windows.Forms.VisualStyles

Public Class MainViewModel
    Inherits ViewModelBase

    Public Sub New()
        _showSettingsWindow = New DelegateCommand(AddressOf ShowSettings, AddressOf CanExecute)
        _openFile = New DelegateCommand(AddressOf OpenFileFunc, AddressOf CanExecute)
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
            Dim settingsVM As New SettingsViewModel
            _settingsWin = New SettingsWindow
            _settingsWin.DataContext = settingsVM
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
End Class
