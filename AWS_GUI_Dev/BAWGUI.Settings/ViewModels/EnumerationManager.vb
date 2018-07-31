Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Linq
Imports System.Windows.Data
Imports System.Windows
Imports System.Drawing
Namespace ViewModels
    Public Class EnumerationManager
        Public Shared Function GetValues(enumeration As Type) As Array
            Dim wArray As Array = [Enum].GetValues(enumeration)
            Dim wFinalArray As New ArrayList()
            For Each wValue As [Enum] In wArray
                Dim fi As FieldInfo = enumeration.GetField(wValue.ToString())
                If fi IsNot Nothing Then
                    Dim wBrowsableAttributes As BrowsableAttribute() = TryCast(fi.GetCustomAttributes(GetType(BrowsableAttribute), True), BrowsableAttribute())
                    If wBrowsableAttributes.Length > 0 Then
                        '  If the Browsable attribute is false
                        If wBrowsableAttributes(0).Browsable = False Then
                            ' Do not add the enumeration to the list.
                            Continue For
                        End If
                    End If

                    Dim wDescriptions As DescriptionAttribute() = TryCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), True), DescriptionAttribute())
                    If wDescriptions.Length > 0 Then
                        wFinalArray.Add(wDescriptions(0).Description)
                    Else
                        wFinalArray.Add(wValue.ToString())
                    End If
                End If
            Next
            Return wFinalArray.ToArray()
        End Function
    End Class
End Namespace
