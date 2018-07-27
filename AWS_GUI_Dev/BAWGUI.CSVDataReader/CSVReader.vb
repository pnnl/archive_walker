Namespace CSVDataReader
    Public Class CSVReader
        Public Sub New(fileName As String)
            Dim fr As FileIO.TextFieldParser = New FileIO.TextFieldParser(fileName)
            fr.TextFieldType = FileIO.FieldType.Delimited
            fr.Delimiters = New String() {","}
            fr.HasFieldsEnclosedInQuotes = True
            pmuName = fileName.Split("\").Last.Split("_")(0)
            signalNames = fr.ReadFields.Skip(1).ToList
            signalTypes = fr.ReadFields.Skip(1).ToList
            signalUnits = fr.ReadFields.Skip(1).ToList
            fr.ReadLine()
            fr.ReadLine()
            Dim time1 = fr.ReadFields(0)
            Dim time2 = fr.ReadFields(0)
            Try
                Dim t1 = Convert.ToDouble(time1)
                Dim t2 = Convert.ToDouble(time2)
                SamplingRate = Math.Round((1 / (t2 - t1)) / 10) * 10
            Catch ex As Exception
                Dim t1 = DateTime.Parse(time1)
                Dim t2 = DateTime.Parse(time2)
                Dim dif = t2.Subtract(t1).TotalSeconds
                SamplingRate = Math.Round((1 / dif) / 10) * 10
            End Try
        End Sub

        Public Property pmuName As String
        Public Property signalNames As List(Of String)
        Public Property signalTypes As List(Of String)
        Public Property signalUnits As List(Of String)
        Public Property SamplingRate As Double
    End Class
End Namespace
