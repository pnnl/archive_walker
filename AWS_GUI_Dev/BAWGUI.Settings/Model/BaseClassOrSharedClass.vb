Imports System.Collections.ObjectModel
Imports BAWGUI.Core
Imports BAWGUI.Settings.ViewModel
Imports BAWGUI.Settings.ViewModels

Namespace Model
    Public Class ParameterValuePair
        Inherits ViewModelBase
        Public Sub New()
            _isRequired = True
        End Sub
        Public Sub New(para As String, value As Object, required As Boolean)
            _parameterName = para
            _value = value
            _isRequired = required
        End Sub
        Public Sub New(para As String, value As Object)
            _parameterName = para
            _value = value
            _isRequired = True
        End Sub
        Private _parameterName As String
        Public Property ParameterName As String
            Get
                Return _parameterName
            End Get
            Set(ByVal value As String)
                _parameterName = value
                OnPropertyChanged()
            End Set
        End Property
        Private _value As Object
        Public Property Value As Object
            Get
                Return _value
            End Get
            Set(ByVal value As Object)
                _value = value
                OnPropertyChanged()
            End Set
        End Property
        Private _isRequired As Boolean
        Public Property IsRequired As Boolean
            Get
                Return _isRequired
            End Get
            Set(ByVal value As Boolean)
                _isRequired = value
                OnPropertyChanged()
            End Set
        End Property
        Private _toolTip As String
        Public Property ToolTip As String
            Get
                Return _toolTip
            End Get
            Set(ByVal value As String)
                _toolTip = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''Class PMU''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Public Class PMU
    '    Inherits ViewModelBase
    '    Public Sub New()
    '        _channels = New ObservableCollection(Of String)
    '    End Sub
    '    Private _PMUName As String
    '    Public Property PMUName As String
    '        Get
    '            Return _PMUName
    '        End Get
    '        Set(ByVal value As String)
    '            _PMUName = value
    '            OnPropertyChanged()
    '        End Set
    '    End Property
    '    Private _channels As ObservableCollection(Of String)
    '    Public Property Channels As ObservableCollection(Of String)
    '        Get
    '            Return _channels
    '        End Get
    '        Set(ByVal value As ObservableCollection(Of String))
    '            _channels = value
    '            OnPropertyChanged()
    '        End Set
    '    End Property
    'End Class

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''Class Filter''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''Class SignalProcessStep''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>
    ''' This is the base class that all signal process steps inherits, including
    ''' filters (DQFilter and TunableFilter), data config customization, unwrap, wrap, interpolate, etc.
    ''' </summary>
    Public MustInherit Class SignalProcessStep
        Inherits ViewModelBase

        Public MustOverride Property Name As String
        Private _stepCounter As Integer
        Public Property StepCounter As Integer
            Get
                Return _stepCounter
            End Get
            Set(ByVal value As Integer)
                _stepCounter = value

                OnPropertyChanged()
            End Set
        End Property

        Private _isStepSelected As Boolean
        Public Property IsStepSelected As Boolean
            Get
                Return _isStepSelected
            End Get
            Set(ByVal value As Boolean)
                _isStepSelected = value
                OnPropertyChanged()
            End Set
        End Property

        Private _inputChannels As ObservableCollection(Of SignalSignatureViewModel)
        Public Property InputChannels As ObservableCollection(Of SignalSignatureViewModel)
            Get
                Return _inputChannels
            End Get
            Set(ByVal value As ObservableCollection(Of SignalSignatureViewModel))
                _inputChannels = value
                OnPropertyChanged()
            End Set
        End Property

        Private _thisStepInputsAsSignalHierachyByType As SignalTypeHierachy
        Public Property ThisStepInputsAsSignalHerachyByType As SignalTypeHierachy
            Get
                Return _thisStepInputsAsSignalHierachyByType
            End Get
            Set(ByVal value As SignalTypeHierachy)
                _thisStepInputsAsSignalHierachyByType = value
                OnPropertyChanged()
            End Set
        End Property

        Private _outputChannels As ObservableCollection(Of SignalSignatureViewModel)
        Public Property OutputChannels As ObservableCollection(Of SignalSignatureViewModel)
            Get
                Return _outputChannels
            End Get
            Set(value As ObservableCollection(Of SignalSignatureViewModel))
                _outputChannels = value
                OnPropertyChanged()
            End Set
        End Property

        Private _thisStepOutputsAsSignalHierachyByPMU As SignalTypeHierachy
        Public Property ThisStepOutputsAsSignalHierachyByPMU As SignalTypeHierachy
            Get
                Return _thisStepOutputsAsSignalHierachyByPMU
            End Get
            Set(ByVal value As SignalTypeHierachy)
                _thisStepOutputsAsSignalHierachyByPMU = value
                OnPropertyChanged()
            End Set
        End Property

        Private _isExpanded As Boolean
        Public Property IsExpanded As Boolean
            Get
                Return _isExpanded
            End Get
            Set(ByVal value As Boolean)
                _isExpanded = value
                OnPropertyChanged()
            End Set
        End Property

        Public MustOverride Function CheckStepIsComplete() As Boolean
    End Class
    Public Class Filter
        Inherits SignalProcessStep
        Public Sub New()
            '_filterParameters = New ObservableCollection(Of ParameterValuePair)
            _fileterParameters = New ObservableCollection(Of ParameterValuePair)
            InputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepInputsAsSignalHerachyByType = New SignalTypeHierachy(New SignalSignatureViewModel)

            OutputChannels = New ObservableCollection(Of SignalSignatureViewModel)
            ThisStepOutputsAsSignalHierachyByPMU = New SignalTypeHierachy(New SignalSignatureViewModel)
            IsExpanded = False
        End Sub

        Public Overrides Function CheckStepIsComplete() As Boolean
            Return True
        End Function

        Private _name As String
        Public Overrides Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
                OnPropertyChanged()
            End Set
        End Property

        'Private _filterName As String
        'Public Property FilterName As String
        '    Get
        '        Return _filterName
        '    End Get
        '    Set(ByVal value As String)
        '        _filterName = value
        '        OnPropertyChanged()
        '    End Set
        'End Property

        Private _fileterParameters As ObservableCollection(Of ParameterValuePair)
        Public Property FilterParameters As ObservableCollection(Of ParameterValuePair)
            Get
                Return _fileterParameters
            End Get
            Set(ByVal value As ObservableCollection(Of ParameterValuePair))
                _fileterParameters = value
                OnPropertyChanged()
            End Set
        End Property

        'Private _filterParameters As ObservableCollection(Of ParameterValuePair)
        'Public Property FilterParameters As ObservableCollection(Of ParameterValuePair)
        '    Get
        '        Return _filterParameters
        '    End Get
        '    Set(ByVal value As ObservableCollection(Of ParameterValuePair))
        '        _filterParameters = value
        '        OnPropertyChanged()
        '    End Set
        'End Property
    End Class

    Public MustInherit Class DetectorBase
        Inherits ViewModelBase

        Public MustOverride ReadOnly Property Name As String

        Private _isStepSelected As Boolean
        Public Property IsStepSelected As Boolean
            Get
                Return _isStepSelected
            End Get
            Set(ByVal value As Boolean)
                _isStepSelected = value
                OnPropertyChanged()
            End Set
        End Property

        Private _inputChannels As ObservableCollection(Of SignalSignatureViewModel)
        Public Property InputChannels As ObservableCollection(Of SignalSignatureViewModel)
            Get
                Return _inputChannels
            End Get
            Set(ByVal value As ObservableCollection(Of SignalSignatureViewModel))
                _inputChannels = value
                OnPropertyChanged()
            End Set
        End Property
        Private _thisStepInputsAsSignalHierachyByType As SignalTypeHierachy
        Public Property ThisStepInputsAsSignalHerachyByType As SignalTypeHierachy
            Get
                Return _thisStepInputsAsSignalHierachyByType
            End Get
            Set(ByVal value As SignalTypeHierachy)
                _thisStepInputsAsSignalHierachyByType = value
                OnPropertyChanged()
            End Set
        End Property
        Private _isExpanded As Boolean
        Public Property IsExpanded As Boolean
            Get
                Return _isExpanded
            End Get
            Set(ByVal value As Boolean)
                _isExpanded = value
                OnPropertyChanged()
            End Set
        End Property
    End Class

    Public MustInherit Class AlarmingDetectorBase
        Inherits ViewModelBase

        Public MustOverride ReadOnly Property Name As String

        Private _isStepSelected As Boolean
        Public Property IsStepSelected As Boolean
            Get
                Return _isStepSelected
            End Get
            Set(ByVal value As Boolean)
                _isStepSelected = value
                OnPropertyChanged()
            End Set
        End Property
        Private _isExpanded As Boolean
        Public Property IsExpanded As Boolean
            Get
                Return _isExpanded
            End Get
            Set(ByVal value As Boolean)
                _isExpanded = value
                OnPropertyChanged()
            End Set
        End Property
    End Class
End Namespace
