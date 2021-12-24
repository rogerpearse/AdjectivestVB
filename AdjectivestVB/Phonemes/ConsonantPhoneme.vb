
Namespace Adjectivest.Phonemes
    Public NotInheritable Class ConsonantPhoneme
        Inherits Phoneme

        Private _Formation As Adjectivest.Phonemes.ConsonantFormation, _DoubleOnShortVowel As Boolean

        Public Sub New(ByVal symbol As String, ByVal formation As ConsonantFormation, ByVal doubleOnShort As Boolean, ByVal example As String)
            MyBase.Symbol = symbol
            Me.Formation = formation
            DoubleOnShortVowel = doubleOnShort
            PronunciationExample = example
            PhonemeType = PhonemeType.Consonant
        End Sub

        Public Property Formation As ConsonantFormation
            Get
                Return _Formation
            End Get
            Private Set(ByVal value As ConsonantFormation)
                _Formation = value
            End Set
        End Property

        Public Property DoubleOnShortVowel As Boolean
            Get
                Return _DoubleOnShortVowel
            End Get
            Private Set(ByVal value As Boolean)
                _DoubleOnShortVowel = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return MyBase.ToString() & " | " & Formation & " | DoubleOnShort: " & DoubleOnShortVowel
        End Function
    End Class
End Namespace
