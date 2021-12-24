Imports System
Imports System.Collections.Generic
Imports Adjectivest.Adjectivest.Phonemes

Namespace Adjectivest.WordProcessor
    Public Class WordObj
        Private _Word As String, _Phonemes As System.Collections.Generic.List(Of Adjectivest.Phonemes.Phoneme), _SyllableCount As Integer, _FirstSyllableType As Adjectivest.Phonemes.PhonemeType, _LastSyllableType As Adjectivest.Phonemes.PhonemeType

        Public Sub New(ByVal wordContent As String, ByVal phonemes As List(Of Phoneme))
            Word = wordContent
            Me.Phonemes = phonemes
            SyllableCount = GetSyllableCount(phonemes)
            SetPhonemeTypes(phonemes)
        End Sub

        Public Property Word As String
            Get
                Return _Word
            End Get
            Private Set(ByVal value As String)
                _Word = value
            End Set
        End Property

        Public Property Phonemes As List(Of Phoneme)
            Get
                Return _Phonemes
            End Get
            Private Set(ByVal value As List(Of Phoneme))
                _Phonemes = value
            End Set
        End Property

        Public Property SyllableCount As Integer
            Get
                Return _SyllableCount
            End Get
            Private Set(ByVal value As Integer)
                _SyllableCount = value
            End Set
        End Property

        Public Property FirstSyllableType As PhonemeType
            Get
                Return _FirstSyllableType
            End Get
            Private Set(ByVal value As PhonemeType)
                _FirstSyllableType = value
            End Set
        End Property

        Public Property LastSyllableType As PhonemeType
            Get
                Return _LastSyllableType
            End Get
            Private Set(ByVal value As PhonemeType)
                _LastSyllableType = value
            End Set
        End Property

        Private Function GetSyllableCount(ByVal phonemes As List(Of Phoneme)) As Integer
            Dim value = 0

            If phonemes IsNot Nothing Then
                For Each p In phonemes

                    If TypeOf p Is VowelPhoneme Then
                        value += 1
                    End If
                Next
            End If

            Return value
        End Function

        'Phoneme Helpers

        Public Function GetLastPhoneme() As Phoneme
            Return Phonemes(Phonemes.Count - 1)
        End Function

        Public Function GetPenultimatePhoneme() As Phoneme
            Return Phonemes(Phonemes.Count - 2)
        End Function

        Public Function GetFirstVowel() As VowelPhoneme
            Dim v As VowelPhoneme = Nothing

            For i = 0 To Phonemes.Count - 1

                If TypeOf Phonemes(i) Is VowelPhoneme Then
                    v = CType(Phonemes(i), VowelPhoneme)
                    Return v
                End If
            Next

            Return Nothing
        End Function

        Private Sub SetPhonemeTypes(ByVal phonemes As List(Of Phoneme))
            If phonemes IsNot Nothing Then
                FirstSyllableType = Me.Phonemes(0).PhonemeType
                Dim last = GetLastPhoneme()
                LastSyllableType = GetLastPhoneme().PhonemeType
            End If
        End Sub

    End Class
End Namespace
