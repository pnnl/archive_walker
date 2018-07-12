Imports System.Windows.Input


Namespace ViewModels
    ''' <summary>
    ''' This class provides a simple ICommand implementation
    ''' </summary>
    Public Class DelegateCommand
        Implements ICommand

        Private m_canExecute As Func(Of Object, Boolean)
        Private m_executeAction As Action(Of Object)
        Private m_canExecuteCache As Boolean
        ''' <summary>
        ''' Occurs when changes occur that affect whether the command should execute.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Event CanExecuteChanged(ByVal sender As Object, ByVal e As System.EventArgs) Implements ICommand.CanExecuteChanged
        ''' <summary>
        ''' Initializes a new instance of the DelegateCommand class
        ''' </summary>
        ''' <param name="executeAction">The execute action</param>
        ''' <param name="canExecute">The can execute predicate</param>
        Public Sub New(ByVal executeAction As Action(Of Object), ByVal canExecute As Func(Of Object, Boolean))
            Me.m_executeAction = executeAction
            Me.m_canExecute = canExecute
        End Sub
        ''' <summary>
        ''' Defines the method that determines whether the command can execute in its current state
        ''' </summary>
        ''' <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        ''' <returns>True if this command can be executed, otherwise false.</returns>
        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
            Dim temp As Boolean = m_canExecute(parameter)
            If m_canExecuteCache <> temp Then
                m_canExecuteCache = temp
                RaiseEvent CanExecuteChanged(Me, New EventArgs())
            End If
            Return m_canExecuteCache
        End Function
        ''' <summary>
        ''' Defines the method to be called when the command is invoked.
        ''' </summary>
        ''' <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
            m_executeAction(parameter)
        End Sub
    End Class
End Namespace
