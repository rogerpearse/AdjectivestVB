
Namespace Phonemes
    Public NotInheritable Class VowelPhoneme
        Inherits Phoneme

        Private _Length As Phonemes.VowelLength

        Public Sub New(ByVal symbol As String, ByVal example As String, ByVal vowelLength As VowelLength)
            Me.Symbol = symbol
            PronunciationExample = example
            Length = vowelLength
            PhonemeType = PhonemeType.Vowel
        End Sub

        Public Property Length As VowelLength
            Get
                Return _Length
            End Get
            Private Set(ByVal value As VowelLength)
                _Length = value
            End Set
        End Property
    End Class
End Namespace
