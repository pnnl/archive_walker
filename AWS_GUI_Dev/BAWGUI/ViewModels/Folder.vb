Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO

Public Class Folder
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
    Public Sub New(filename, filetype)
        _name = Path.GetFileName(filename)
        'what If type Is nothing?
        _type = filetype
        _buildDirTree(filename)
    End Sub
    Private _type As String
    Private _name As String
    Public Property Name As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
            OnPropertyChanged("Name")
        End Set
    End Property
    Private _subFolders As ObservableCollection(Of Folder)
    Public Property SubFolders As ObservableCollection(Of Folder)
        Get
            Return _subFolders
        End Get
        Set(ByVal value As ObservableCollection(Of Folder))
            _subFolders = value
            OnPropertyChanged("SubFolders")
        End Set
    End Property
    Private Sub _buildDirTree(filename)
        If File.Exists(filename) Then

        ElseIf Directory.Exists(filename) Then
            _subFolders = New ObservableCollection(Of Folder)
            For Each path In Directory.GetDirectories(filename)
                _subFolders.Add(New Folder(path, _type))
            Next
            For Each file In Directory.GetFiles(filename)
                'what If type Is nothing?
                If Path.GetExtension(file).Substring(1) = _type Then
                    _subFolders.Add(New Folder(file, _type))
                End If
            Next
        Else
            Throw New Exception(vbCrLf + "Error: data file path """ + filename + """ does not exists!")
        End If
    End Sub
End Class
