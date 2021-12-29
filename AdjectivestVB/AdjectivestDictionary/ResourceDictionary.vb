Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text.RegularExpressions
Imports Adjectivest.Phonemes

Public MustInherit Class ResourceDictionary
    Implements IDisposable

    Protected Const resourcesFolder As String = "AdjectivestResources"
    Protected Const cmuDictFileName As String = "cmudict-0.7b.txt"
    Protected Const dictLetterIndicesFileName As String = "dictLetterIndices.txt"
    Protected Const adjectivesListFileName As String = "adjectivesList.txt"
    Protected dictionaryLetterIndices As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer)()
    Protected dictionaryPath As String = Path.Combine(resourcesFolder, cmuDictFileName)
    Protected adjectivesListLetterIndices As Dictionary(Of Char, Integer) = New Dictionary(Of Char, Integer)()
    Protected adjectivesListPath As String = Path.Combine(resourcesFolder, adjectivesListFileName)
    Protected disposedValue As Boolean
    Protected Property PhonemeCollection As PhonemeCollection

    Public Function GetPhonemesFromWord(ByVal word As String) As List(Of Phoneme)
        Dim phonemes As List(Of Phoneme) = New List(Of Phoneme)()
        Dim upperFirst As Char = Char.ToUpper(word(0))
        Dim wordUpper As String = word.ToUpper()
        Dim startIndex As Integer = dictionaryLetterIndices(upperFirst)
        Dim maxIndex As Integer = If(upperFirst <> "Z"c, dictionaryLetterIndices(Microsoft.VisualBasic.ChrW(AscW(upperFirst) + 1)), 133904)
        Dim line As String = GetDictLine(wordUpper, startIndex, maxIndex)

        If line Is Nothing Then
            Return Nothing
        End If

        phonemes = GetPhonemesFromLine(word, line)
        Return phonemes
    End Function

    Public MustOverride Function DictContainsWord(ByVal word As String) As Boolean
    Public MustOverride Function AdjectivesListContainsWord(ByVal word As String) As Boolean
    Protected MustOverride Function GetDictLine(ByVal word As String, ByVal startIndex As Integer, ByVal maxIndex As Integer) As String

    Protected Shared Function GetDictionaryLetterIndices() As Dictionary(Of Char, Integer)
        Const delimiter = ","c
        Dim keyValuePairs = New Dictionary(Of Char, Integer)()
        Dim lines = File.ReadAllLines(Path.Combine(resourcesFolder, dictLetterIndicesFileName))

        For i = 0 To lines.Length - 1
            Dim values = lines(i).Split(delimiter)
            Dim letter = values(0)(0)
            Dim index = Integer.Parse(values(1))
            keyValuePairs.Add(letter, index)
        Next

        Return keyValuePairs
    End Function

    Protected Function GetPhonemesFromLine(ByVal word As String, ByVal line As String) As List(Of Phoneme)
        Const spaceSeparator = " "c
        Dim phonemes As List(Of Phoneme) = New List(Of Phoneme)()
        Dim phonemeSubString As String = line.Substring(word.Length).Trim()
        Dim phonemesRaw = phonemeSubString.Split(spaceSeparator)

        For j = 0 To phonemesRaw.Length - 1
            Dim phoneme = Regex.Replace(phonemesRaw(j), "[\d-]", String.Empty)
            phonemes.Add(PhonemeCollection.GetPhoneme(phoneme))
        Next

        Return phonemes
    End Function

    Protected Function LineIdentifiedAsWord(ByVal word As String, ByVal line As String) As Boolean
        Const spaceSeparator = " "c
        Return line.StartsWith(word) AndAlso Equals(line.Split(spaceSeparator)(0), word)
    End Function

    Protected MustOverride Function GetAdjectivesListLetterIndices() As Dictionary(Of Char, Integer)
    Protected MustOverride Sub Dispose(ByVal disposing As Boolean)

    ' // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ' ~ResourceDictionary()
    ' {
    '     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    '     Dispose(disposing: false);
    ' }

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
