Imports System.Collections.Generic
Imports System.IO
Imports Adjectivest.Adjectivest.WordProcessor

Namespace Adjectivest
    Public Class UnidentifiedWordBuilder
        Private Const vowels As String = "aeiou"

        Public Function BuildWordObject(ByVal input As String) As WordObj
            Return Nothing
        End Function

        Public Shared Function GetDictionaryIndices() As Dictionary(Of Char, Integer)
            Dim returnDict = New Dictionary(Of Char, Integer)()
            Dim lines = File.ReadAllLines("AdjectivestResources/adjectivesList.txt")
            returnDict.Add("a"c, 0)
            Dim [next] = "b"c

            For i = 1 To lines.Length - 1
                Dim line = lines(i)
                Dim lineStart = line(0)

                If lineStart = [next] Then
                    returnDict.Add([next], i)
                    [next] = Microsoft.VisualBasic.ChrW(AscW([next]) + 1)
                End If
            Next

            Dim format = ""

            For Each entry In returnDict
                format += entry.Key & "," & entry.Value & Microsoft.VisualBasic.Constants.vbLf
            Next

            Return returnDict
        End Function
    End Class
End Namespace
