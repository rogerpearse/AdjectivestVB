Imports System.Collections.Generic
Imports System.IO
Imports Adjectivest.Phonemes

Public Class InMemoryResourceDictionary
    Inherits ResourceDictionary

    Private cmuDictLines As String()
    Private adjectivesLines As String()

    Public Sub New()
        cmuDictLines = File.ReadAllLines(dictionaryPath)
        dictionaryLetterIndices = GetDictionaryLetterIndices()
        adjectivesLines = File.ReadAllLines(adjectivesListPath)
        adjectivesListLetterIndices = GetAdjectivesListLetterIndices()
        PhonemeCollection = New PhonemeCollection()
    End Sub

    ''' <summary>
    ''' Used only for test
    ''' </summary>
    Public Function getAdjectivesLines() As String()
        Return adjectivesLines
    End Function

    Public Overrides Function AdjectivesListContainsWord(ByVal word As String) As Boolean
        Dim first As Char = word(0)
        Dim startIndex As Integer = adjectivesListLetterIndices(first)
        Try

            Dim maxIndex As Integer = adjectivesListLetterIndices(Microsoft.VisualBasic.ChrW(AscW(first) + 1))

            For i = startIndex To maxIndex - 1

                If adjectivesLines(i).Equals(word) Then
                    Return True
                End If
            Next

            Return False
        Catch ex As Exception
            ' TODO ought to have better
            Return False
        End Try

    End Function

    Public Overrides Function DictContainsWord(ByVal word As String) As Boolean
        Dim upperFirst As Char = Char.ToUpper(word(0))
        Dim startIndex As Integer = dictionaryLetterIndices(upperFirst)
        Dim maxIndex As Integer = If(upperFirst <> "Z"c, dictionaryLetterIndices(Microsoft.VisualBasic.ChrW(AscW(upperFirst) + 1)), cmuDictLines.Length - 1)
        'int startIndex = 0;
        'int maxIndex = cmuDictLines.Length - 1;

        Return Not Equals(GetDictLine(word.ToUpper(), startIndex, maxIndex), Nothing)
    End Function

    Protected Overrides Function GetDictLine(ByVal word As String, ByVal startIndex As Integer, ByVal maxIndex As Integer) As String
        For i = startIndex To maxIndex - 1
            Dim line = cmuDictLines(i)

            If LineIdentifiedAsWord(word, line) Then
                Return line
            End If
        Next

        Return Nothing ' TODO returning nothing as a normal outcome and testing for it is an antipattern
    End Function

    Protected Overrides Function GetAdjectivesListLetterIndices() As Dictionary(Of Char, Integer)
        Dim returnDict = New Dictionary(Of Char, Integer)()
        Const indicesStartIndex = 5
        Const delimiter = ","c

        For i = indicesStartIndex To indicesStartIndex + 25 - 1
            Dim split = adjectivesLines(i).Split(delimiter)
            Dim character = Char.Parse(split(0))
            Dim index = Integer.Parse(split(1))
            returnDict.Add(character, index)
        Next

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
            cmuDictLines = Nothing
            adjectivesLines = Nothing
        End If
    End Sub
End Class
