Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports BAWGUI.Settings.ViewModels

Namespace Model
    Public Class Folder
        Inherits ViewModelBase
        Public Sub New(filename As String, filetype As String, ByRef firstFile As String)
            _name = Path.GetFileName(filename)
            _fullName = filename
            'what If type Is nothing?
            _type = filetype
            'firstFile = Nothing
            '_buildDirTree(filename, firstFile)
            _findFirstFile(filename, filetype, firstFile)
        End Sub
        Private _type As String
        Private _name As String
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
                OnPropertyChanged()
            End Set
        End Property
        Private _fullName As String
        Public Property FullName As String
            Get
                Return _fullName
            End Get
            Set(ByVal value As String)
                _fullName = value
                OnPropertyChanged()
            End Set
        End Property
        Private _subFolders As ObservableCollection(Of Folder)
        Public Property SubFolders As ObservableCollection(Of Folder)
            Get
                Return _subFolders
            End Get
            Set(ByVal value As ObservableCollection(Of Folder))
                _subFolders = value
                OnPropertyChanged()
            End Set
        End Property
        Private Sub _buildDirTree(filename As String, ByRef firstFile As String)
            If File.Exists(filename) Then
                If String.IsNullOrEmpty(firstFile) Then
                    firstFile = filename
                End If
                'If firstFile IsNot Nothing Then
                '    firstFile = filename
                'End If
            ElseIf Directory.Exists(filename) Then
                _subFolders = New ObservableCollection(Of Folder)
                For Each path In Directory.GetDirectories(filename)
                    _subFolders.Add(New Folder(path, _type, firstFile))
                Next
                For Each file In Directory.GetFiles(filename)
                    'what If type Is nothing?
                    If Path.GetExtension(file).Substring(1) = _type Then
                        If String.IsNullOrEmpty(firstFile) Then
                            _subFolders.Add(New Folder(file, _type, firstFile))
                        End If
                    End If
                Next
            Else
                Throw New Exception(vbCrLf + "Error: data file path """ + filename + """ does not exists!")
            End If
        End Sub
        Private Sub _findFirstFile(filename As String, filetype As String, ByRef firstFile As String)
            If File.Exists(filename) Then
                If Path.GetExtension(filename).Substring(1).ToLower() = filetype Then
                    firstFile = filename
                End If
            ElseIf Directory.Exists(filename) Then
                For Each file In Directory.GetFiles(filename)
                    _findFirstFile(file, filetype, firstFile)
                    If Not String.IsNullOrEmpty(firstFile) Then
                        Exit Sub
                    End If
                Next
                For Each path In Directory.GetDirectories(filename)
                    _findFirstFile(path, filetype, firstFile)
                    If Not String.IsNullOrEmpty(firstFile) Then
                        Exit Sub
                    End If
                Next
            Else
                Throw New Exception(vbCrLf + "Error: data file path """ + filename + """ does not exists!")
            End If
        End Sub
    End Class
End Namespace