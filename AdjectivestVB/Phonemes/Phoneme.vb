
Namespace Phonemes
    Public MustInherit Class Phoneme
        Private _Symbol As String, _PronunciationExample As String, _PhonemeType As Adjectivest.Phonemes.PhonemeType

        Public Property Symbol As String
            Get
                Return _Symbol
            End Get
            Protected Set(ByVal value As String)
                _Symbol = value
            End Set
        End Property

        Public Property PronunciationExample As String
            Get
                Return _PronunciationExample
            End Get
            Protected Set(ByVal value As String)
                _PronunciationExample = value
            End Set
        End Property

        Public Property PhonemeType As PhonemeType
            Get
                Return _PhonemeType
            End Get
            Protected Set(ByVal value As PhonemeType)
                _PhonemeType = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Symbol & " | " & PronunciationExample
        End Function
    End Class
End Namespace
