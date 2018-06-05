Imports System.Collections.ObjectModel
Imports BAWGUI.Core
Imports BAWGUI.Settings.ViewModel
Imports BAWGUI.Settings.ViewModels

Namespace Model
    Public Class SignalTypeHierachy
        Inherits ViewModelBase
        Public Sub New()
            _signalSignature = New SignalSignatureViewModel
            _signalList = New ObservableCollection(Of SignalTypeHierachy)
        End Sub
        Public Sub New(signature As SignalSignatureViewModel)
            _signalSignature = signature
            _signalList = New ObservableCollection(Of SignalTypeHierachy)
        End Sub
        Public Sub New(signature As SignalSignatureViewModel, list As ObservableCollection(Of SignalTypeHierachy))
            _signalSignature = signature
            _signalList = list
        End Sub
        Private _signalSignature As SignalSignatureViewModel
        Public Property SignalSignature As SignalSignatureViewModel
            Get
                Return _signalSignature
            End Get
            Set(ByVal value As SignalSignatureViewModel)
                _signalSignature = value
                OnPropertyChanged()
            End Set
        End Property
        Private _signalList As ObservableCollection(Of SignalTypeHierachy)
        Public Property SignalList As ObservableCollection(Of SignalTypeHierachy)
            Get
                Return _signalList
            End Get
            Set(ByVal value As ObservableCollection(Of SignalTypeHierachy))
                _signalList = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

End Namespace
