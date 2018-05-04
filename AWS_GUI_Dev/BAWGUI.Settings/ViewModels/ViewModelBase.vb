Imports System.ComponentModel

Namespace ViewModels
    Public Class ViewModelBase
        Implements INotifyPropertyChanged
        ''' <summary>
        ''' Raise property changed event
        ''' </summary>
        ''' <param name="sender">The event sender</param>
        ''' <param name="e">The event</param>
        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
        Protected Overridable Sub OnPropertyChanged(<Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
        Protected Overridable Function CanExecute(ByVal param As Object) As Boolean
            Return True
        End Function
    End Class
End Namespace
