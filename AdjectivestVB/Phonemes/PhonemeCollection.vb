Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace Adjectivest.Phonemes
    Public Class PhonemeCollection
        Implements IDisposable

        Public Sub New()
            Initialize()
        End Sub

        Private Const vowelsFileName As String = "vowels.txt"
        Private Const consonantsFileName As String = "consonants.txt"
        Private Const resourcesFolder As String = "AdjectivestResources"
        Private Const phoneticVowels As String = "aeiou"
        Private Const commaDelimiter As Char = ","c
        Private Vowels As List(Of VowelPhoneme) = New List(Of VowelPhoneme)()
        Private ConsonantPhonemes As List(Of ConsonantPhoneme) = New List(Of ConsonantPhoneme)()
        Private phonemeIndex As Dictionary(Of String, Phoneme) = New Dictionary(Of String, Phoneme)()
        Private disposedValue As Boolean

        Public Function GetPhoneme(ByVal symbol As String) As Phoneme
            Return phonemeIndex(symbol)
        End Function

        Private Sub Initialize()
            Dim vowels = File.ReadAllLines(Path.Combine(resourcesFolder, vowelsFileName))

            For i = 0 To vowels.Length - 1
                Dim vowelPhonemeRaw = vowels(i).Split(commaDelimiter)
                Dim symbol = vowelPhonemeRaw(0)
                Dim length = vowelPhonemeRaw(1)
                Dim example = vowelPhonemeRaw(2)
                Dim vowelLength As VowelLength = CType([Enum].Parse(GetType(VowelLength), length), Phonemes.VowelLength)
                Dim vowelPhoneme As VowelPhoneme = New VowelPhoneme(symbol, example, vowelLength)
                Me.Vowels.Add(vowelPhoneme)
                phonemeIndex.Add(symbol, vowelPhoneme)
            Next

            Dim consonants = File.ReadAllLines(Path.Combine(resourcesFolder, consonantsFileName))

            For i = 0 To consonants.Length - 1
                Dim consonantsPhonemeRaw = consonants(i).Split(commaDelimiter)
                Dim symbol = consonantsPhonemeRaw(0)
                Dim doubleRaw = consonantsPhonemeRaw(1)
                Dim example = consonantsPhonemeRaw(2)
                Dim formation = consonantsPhonemeRaw(3)
                Dim doubleOnShort = Boolean.Parse(doubleRaw)
                Dim consonantFormation As ConsonantFormation = CType([Enum].Parse(GetType(ConsonantFormation), formation), Phonemes.ConsonantFormation)
                Dim consonantPhoneme As ConsonantPhoneme = New ConsonantPhoneme(symbol, consonantFormation, doubleOnShort, example)
                ConsonantPhonemes.Add(consonantPhoneme)
                phonemeIndex.Add(symbol, consonantPhoneme)
            Next
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                End If

                Vowels = Nothing
                ConsonantPhonemes = Nothing
                phonemeIndex = Nothing
                disposedValue = True
            End If
        End Sub

        ' // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ' ~PhonemeCollection()
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
End Namespace
