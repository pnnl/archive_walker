Imports System.ComponentModel

Public Class MainViewModel
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
    Private Function CanExecute(ByVal param As Object) As Boolean
        Return True
    End Function
End Class
