Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Adjectivest.Adjectivest.Phonemes

Namespace Adjectivest.Core.AdjectiveDictionary
    Public Class BasicResourceDictionary
        Inherits ResourceDictionary

        Public Sub New()
            dictionaryLetterIndices = GetDictionaryLetterIndices()
            adjectivesListLetterIndices = GetAdjectivesListLetterIndices()
            PhonemeCollection = New PhonemeCollection()
        End Sub

        Public Overrides Function AdjectivesListContainsWord(ByVal word As String) As Boolean
            Dim first As Char = word(0)
            Dim startIndex As Integer = adjectivesListLetterIndices(first)
            Dim maxIndex As Integer = adjectivesListLetterIndices(Microsoft.VisualBasic.ChrW(AscW(first) + 1))

            Using streamReader As StreamReader = New StreamReader(adjectivesListPath)

                For i = 0 To maxIndex - 1
                    Dim adj As String = streamReader.ReadLine()

                    If adj.Equals(word) Then
                        Return True
                    End If
                Next
            End Using

            Return False
        End Function

        Public Overrides Function DictContainsWord(ByVal word As String) As Boolean
            Dim upperFirst As Char = Char.ToUpper(word(0))
            Dim maxIndex As Integer = If(upperFirst <> "Z"c, dictionaryLetterIndices(Microsoft.VisualBasic.ChrW(AscW(upperFirst) + 1)), 133904)
            Return Not Equals(GetDictLine(word, 0, maxIndex), Nothing)
        End Function

        Protected Overrides Function GetDictLine(ByVal word As String, ByVal startIndex As Integer, ByVal maxIndex As Integer) As String
            Using streamReader As StreamReader = New StreamReader(dictionaryPath)

                For i = 0 To maxIndex - 1
                    Dim line As String = streamReader.ReadLine()

                    If LineIdentifiedAsWord(word, line) Then
                        Return line
                    End If
                Next
            End Using

            Return Nothing  '-- TODO antipattern
        End Function

        Protected Overrides Function GetAdjectivesListLetterIndices() As Dictionary(Of Char, Integer)
            Const delimiter = ","c
            Dim returnDict = New Dictionary(Of Char, Integer)()

            Using streamReader As StreamReader = New StreamReader(adjectivesListPath)
                Dim readingIndices = False

                For i = 0 To 29 - 1
                    Dim line As String = streamReader.ReadLine()

                    If line.StartsWith("a,") Then
                        readingIndices = True
                    End If

                    If readingIndices Then
                        Dim split = line.Split(delimiter)
                        Dim character = Char.Parse(split(0))
                        Dim index = Integer.Parse(split(1))
                        returnDict.Add(character, index)
                    End If
                Next
            End Using

            Return returnDict
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    PhonemeCollection.Dispose()
                    PhonemeCollection = Nothing
                End If

                dictionaryLetterIndices = Nothing
                adjectivesListLetterIndices = Nothing
                disposedValue = True
                GC.Collect()
            End If
        End Sub
    End Class
End Namespace
