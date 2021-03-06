Option Explicit On
Option Strict On
Imports System
Imports System.IO
Imports Adjectivest.Phonemes
Imports System.Runtime.InteropServices

Public Enum InflectionType
    MoreMost
    ErEst
End Enum

Public Enum AdjectiveForm
    None
    Base
    Comparative
    Superlative
End Enum

Public Class AdjectiveInflector
    Implements IDisposable

    Private Const irregularAdjectivesFileName As String = "AdjectivestResources/irregularAdjectives.txt"
    Private Const commaDelimiter As Char = ","c
    Public Const leSuffix As String = "le"
    Public Const erSuffix As String = "er"
    Public Const ySuffix As String = "y"
    Private Const basicComparative As String = "er"
    Private Const basicSuperlative As String = "est"
    Private Const pastParticipleEnding As String = "ed"
    Private Const multiSyllabicComparative As String = "more"
    Private Const multiSyllabicSuperlative As String = "most"

    'exception rules
    Private Const eSuffix As String = "e"
    Public Shared irregularAdjectives As String()
    Private disposedValue As Boolean
    Public Property ResourceDictionary As ResourceDictionary

    ''' <summary>
    ''' If parameter=true, the data loads into memory for greater speed. If false, it uses the file each time.
    ''' </summary>
    Public Sub New(ByVal loadDictionaryIntoMemory As Boolean)

        irregularAdjectives = File.ReadAllLines(irregularAdjectivesFileName)

        If loadDictionaryIntoMemory Then
            ResourceDictionary = New InMemoryResourceDictionary()
        Else
            ResourceDictionary = New BasicResourceDictionary()
        End If
    End Sub

    ''' <summary>
    ''' The main entry point to the code.  Pass in the word to be inflected, whether "sad", "sadder" or "saddest" - all will return the same string.  
    ''' But if you already know what form your input adjective is, setting adjectiveForm to the corresponding value will speed up the process and be less error-prone. 
    ''' Leaving it as AdjectiveForm.None, the program will attempt to infer its form. Output is the same either way
    ''' </summary>
    Public Function GetAdjectiveInflections(ByVal word As String, _
                                            Optional ByVal adjectiveForm As AdjectiveForm = AdjectiveForm.None) As String
        Dim baseForm = ""
        Dim comparative = ""
        Dim superlative = ""

        If IsKnownIrregular(word, adjectiveForm, baseForm, comparative, superlative) Then
            Return Format(baseForm, comparative, superlative)
        End If

        Dim wordObj As WordObj = Nothing
        Dim phonemes = ResourceDictionary.GetPhonemesFromWord(word)
        wordObj = New WordObj(word, phonemes)
        Dim baseWordObj As WordObj = Nothing

        If adjectiveForm = adjectiveForm.None Then
            adjectiveForm = InferAdjectiveFormAndBaseForm(wordObj, baseForm, baseWordObj)
        End If

        Dim inflectionBase = If(baseWordObj Is Nothing, wordObj, baseWordObj)

        '-- If nothing can be found to work with, just return the word with more/most
        If inflectionBase.Phonemes Is Nothing Then
            Return Format(word, GetComparative(inflectionBase, Adjectivest.InflectionType.MoreMost), GetSuperlative(inflectionBase, Adjectivest.InflectionType.MoreMost))
        End If

        ' Do we use more/most or -er/est?
        Dim endsInEd As Boolean = DoesWordEndInEd(inflectionBase.Word)
        Dim inflectionType As InflectionType = GetInflectionType(inflectionBase.Word, _
                                                                 inflectionBase.SyllableCount, _
                                                                 endsInEd)

        Select Case adjectiveForm
            Case adjectiveForm.Base
                baseForm = word
                comparative = GetComparative(inflectionBase, inflectionType)
                superlative = GetSuperlative(inflectionBase, inflectionType)
            Case adjectiveForm.Comparative
                comparative = word
                superlative = GetSuperlative(inflectionBase, inflectionType)
            Case adjectiveForm.Superlative
                superlative = word
                comparative = GetComparative(inflectionBase, inflectionType)
        End Select

        Return Format(baseForm, comparative, superlative)
    End Function

    Private Function Format(ByVal word As String, ByVal comparative As String, ByVal superlative As String) As String
        Return word + "|" + comparative + "|" + superlative
    End Function

    Public Function IsKnownIrregular(ByVal word As String, _
                                     ByRef form As AdjectiveForm, _
                                     ByRef baseForm As String, _
                                     ByRef comparative As String, _
                                     ByRef superlative As String) As Boolean
        baseForm = Nothing
        comparative = Nothing
        superlative = Nothing
        form = AdjectiveForm.None

        For i = 0 To irregularAdjectives.Length - 1
            Dim irregular As String = irregularAdjectives(i)

            ' Ignore comments
            If irregular.StartsWith("#") Then Continue For

            If Not irregular.Contains(word) Then Continue For

            Dim splitIrregular As String() = irregular.Split(commaDelimiter)
            baseForm = splitIrregular(0)
            comparative = splitIrregular(1)
            superlative = splitIrregular(2)

            For j = 0 To splitIrregular.Length - 1
                If splitIrregular(j) = word Then
                    Select Case j
                        Case 0
                            form = AdjectiveForm.Base
                        Case 1
                            form = AdjectiveForm.Comparative
                        Case 2
                            form = AdjectiveForm.Superlative
                        Case Else
                            form = AdjectiveForm.None
                    End Select
                End If
            Next

            Return True
        Next

        Return False
    End Function

    ''' <summary>
    ''' Do we use more/most or -er/-est?
    ''' </summary>
    Public Function GetInflectionType(ByVal wordText As String, _
                                      ByVal syllableCount As Integer, _
                                      ByVal doesWordEndInEd As Boolean) As InflectionType

        If doesWordEndInEd Then Return InflectionType.MoreMost

        If syllableCount < 2 Then Return InflectionType.ErEst

        If syllableCount < 3 And wordText.EndsWith(ySuffix) Then Return InflectionType.ErEst

        If (syllableCount < 3 And wordText.EndsWith(leSuffix)) Then
            Return InflectionType.ErEst
        End If

        ' Otherwise
        Return InflectionType.MoreMost

    End Function

    Public Function GetComparative(ByVal word As WordObj, ByVal inflection_type As InflectionType) As String
        If inflection_type = InflectionType.MoreMost Then
            Return GetMultiSyllabicComparative(word, AdjectiveForm.Comparative)
        End If
        ' InflectionType.ErEst
        Return GetBasicComparative(word, AdjectiveForm.Comparative)
    End Function

    Public Function GetSuperlative(ByVal word As WordObj, ByVal inflection_type As InflectionType) As String
        If inflection_type = InflectionType.MoreMost Then
            Return GetMultiSyllabicComparative(word, AdjectiveForm.Superlative)
        End If
        ' InflectionType.ErEst
        Return GetBasicComparative(word, AdjectiveForm.Superlative)
    End Function

    ''' <summary>
    ''' Gets the base-form of an adjective.  Input can be any form.  But if you already know what form your input adjective is, setting word to the corresponding value will speed up the process and be less error-prone. Leaving it as AdjectiveForm.None, the program will attempt to infer its form.
    ''' </summary>
    Public Function GetBaseForm(ByVal wordObj As WordObj, Optional ByVal knownAdjectiveForm As AdjectiveForm = AdjectiveForm.None) As String
        Dim baseForm As String = Nothing

        Select Case knownAdjectiveForm
            Case AdjectiveForm.Base
                Return wordObj.Word
            Case AdjectiveForm.None
                Dim baseFormObj As WordObj = Nothing
                knownAdjectiveForm = InferAdjectiveFormAndBaseForm(wordObj, baseForm, baseFormObj)

                If Equals(baseForm, Nothing) Then
                    Return GetBaseForm(wordObj, knownAdjectiveForm)
                End If

                Return baseForm
            Case Else

                If IsMultiSyllabicComparativeOrSuperlative(wordObj.Word, knownAdjectiveForm, baseForm) Then
                    Return baseForm
                End If

                Return GetBaseFormFromSimpleComparativeOrSuperlative(wordObj)
        End Select
    End Function

    Private Function IsMultiSyllabicComparativeOrSuperlative(ByVal input As String, <Out()> ByRef form As AdjectiveForm, <Out()> ByRef baseForm As String) As Boolean

        Const spaceSeparator = " "c
        Dim splitArr = input.Trim().Split(spaceSeparator)
        baseForm = Nothing
        form = AdjectiveForm.None

        If splitArr.Length > 1 Then
            Select Case splitArr(0)
                Case multiSyllabicComparative
                    form = AdjectiveForm.Comparative
                Case multiSyllabicSuperlative
                    form = AdjectiveForm.Superlative
            End Select

            baseForm = splitArr(1)
            Return True
        End If

        Return False
    End Function

    Public Function InferAdjectiveFormAndBaseForm(ByVal wordObj As WordObj, <Out()> ByRef baseForm As String, <Out()> ByRef baseFormObj As WordObj) As AdjectiveForm
        Dim likelyForm = AdjectiveForm.None
        baseFormObj = Nothing
        Dim isMultiSyllabic = IsMultiSyllabicComparativeOrSuperlative(wordObj.Word, likelyForm, baseForm)

        If Not isMultiSyllabic Then
            Dim comparative As String = ""
            Dim superlative As String = ""

            If IsKnownIrregular(wordObj.Word, likelyForm, baseForm, comparative, superlative) Then
                Return likelyForm
            End If

            If wordObj.Word.EndsWith(basicComparative) Then
                likelyForm = AdjectiveForm.Comparative
            ElseIf wordObj.Word.EndsWith(basicSuperlative) Then
                likelyForm = AdjectiveForm.Superlative
            Else
                likelyForm = AdjectiveForm.Base
            End If
        End If

        If likelyForm <> AdjectiveForm.Base Then
            baseForm = GetBaseForm(wordObj, likelyForm)
            Dim baseFormFound = Not Equals(baseForm, Nothing) AndAlso ResourceDictionary.AdjectivesListContainsWord(baseForm)

            If Not baseFormFound Then
                baseForm = wordObj.Word
                likelyForm = AdjectiveForm.Base
            Else
                baseFormObj = BuildWordObject(baseForm)
            End If
        End If

        Return likelyForm
    End Function

    Private Function GetBasicComparative(ByVal wordObj As WordObj, ByVal comparisonType As AdjectiveForm) As String
        Dim finalValue As String = Nothing
        Dim word = wordObj.Word

        Dim suffix As String = basicSuperlative
        If comparisonType = AdjectiveForm.Comparative Then
            suffix = basicComparative
        End If

        If word.EndsWith(eSuffix) Then
            suffix = suffix.Substring(1)
            finalValue = word & suffix
            Return finalValue
        End If

        If word.EndsWith(ySuffix) AndAlso wordObj.SyllableCount > 1 Then
            finalValue = word.Substring(0, word.Length - 1)
            finalValue += "i" & suffix
            Return finalValue
        End If

        finalValue = word

        'Should be only vowel by this point?
        Dim firstVowel As VowelPhoneme = wordObj.GetFirstVowel()
        Dim endsInConsonant As Boolean = False
        If wordObj.LastSyllableType = PhonemeType.Consonant Then
            endsInConsonant = True
        End If

        If endsInConsonant Then
            Dim lastConsonant As ConsonantPhoneme = CType(wordObj.GetLastPhoneme(), ConsonantPhoneme)

            Select Case firstVowel.Length
                Case VowelLength.Short

                    Dim lastPhoneme As Phoneme = wordObj.GetLastPhoneme()
                    Dim lastChar As Char = word(word.Length - 1)

                    '-- Test moved to function, and bodged
                    If doWeDoubleLastConsonant(finalValue.ToLower, _
                                               lastConsonant.DoubleOnShortVowel, _
                                               lastConsonant.Formation, _
                                               TypeOf lastPhoneme Is ConsonantPhoneme) Then
                        ' Double last consonant, but not if already doubled, or a digraph
                        finalValue += lastChar
                        finalValue += suffix
                    Else
                        finalValue = word & suffix
                    End If

                    Return finalValue
                Case VowelLength.Long
                    Return finalValue & suffix
            End Select
        Else

            If wordObj.SyllableCount = 1 Then
                Return finalValue & suffix
            End If
        End If

        Return finalValue
    End Function

    ''' <summary>
    ''' Decide if we need to double the last consonstant.
    ''' </summary>        
    Public Function doWeDoubleLastConsonant(finalValue As String, _
                                            lastConsonantDoubleOnShortVowel As Boolean, _
                                            lastConsonantFormation As ConsonantFormation, _
                                            isLastConsonantPhoneme As Boolean) As Boolean

        ' TODO remove original Adjectivest code that didn't really work.
        'If lastConsonantDoubleOnShortVowel = False Then Return False
        'If isLastConsonantPhoneme AndAlso lastConsonantFormation = ConsonantFormation.Digraph Then Return False
        'If endsInNonDigraphicDualConsonant Then Return False

        '-- Last three chars.  If shorter then we can't do much.  Coded from
        ' https://speakspeak.com/resources/english-grammar-rules/english-spelling-rules/double-consonant-adjective-before-er-est
        If finalValue.Length < 3 Then Return True
        Dim char3 As Char = finalValue(finalValue.Length - 1)
        Dim char2 As Char = finalValue(finalValue.Length - 2)
        Dim char1 As Char = finalValue(finalValue.Length - 3)

        '-- We DO double the final letter when the adjective ends in consonant + vowel + consonant
        If Not isVowel(char1) And isVowel(char2) And Not isVowel(char3) Then
            Return True
        End If

        '-- We don't double the final letter when the adjective ends in vowel + vowel + consonant
        If isVowel(char1) And isVowel(char2) And Not isVowel(char3) Then
            Return False
        End If

        '-- We don't double the final letter when the adjective ends in vowel + consonant + consonant
        If isVowel(char1) And Not isVowel(char2) And Not isVowel(char3) Then
            Return False
        End If

        '-- Otherwise do not double the consonant
        Return False

    End Function

    Public Function isVowel(c As Char) As Boolean
        Return "aeiou".Contains(c)
    End Function

    ''' <summary>
    ''' Put "more" or "most" on the front of the word
    ''' </summary>
    Private Function GetMultiSyllabicComparative(ByVal wordObj As WordObj, ByVal comparisonType As AdjectiveForm) As String

        Dim finalValue = wordObj.Word
        Dim prefix As String = ""

        Select Case comparisonType
            Case AdjectiveForm.Comparative
                prefix = multiSyllabicComparative
            Case AdjectiveForm.Superlative
                prefix = multiSyllabicSuperlative
            Case Else
                Throw New System.NotImplementedException
        End Select

        finalValue = prefix & " " & finalValue
        Return finalValue
    End Function

    Private Function GetBaseFormFromSimpleComparativeOrSuperlative(ByVal wordObj As WordObj) As String
        Dim suffix = If(wordObj.Word.EndsWith(basicComparative), basicComparative, basicSuperlative)
        Dim trimmedWord = wordObj.Word.Substring(0, wordObj.Word.Length - suffix.Length)
        Dim potentialForm As String = Nothing

        If trimmedWord.EndsWith("i"c) Then
            potentialForm = trimmedWord.Substring(0, trimmedWord.Length - 1) & "y"c
        End If

        If Not Equals(potentialForm, Nothing) AndAlso ResourceDictionary.DictContainsWord(potentialForm) Then
            Return potentialForm
        End If

        Return trimmedWord
    End Function

    Private Function BuildWordObject(ByVal word As String) As WordObj
        Dim phonemes = ResourceDictionary.GetPhonemesFromWord(word)
        Return New WordObj(word, phonemes)
    End Function

    ''' <summary>
    ''' Does word end in "ed"?   Renamed from IsPastParticiple, which it is not.
    ''' </summary>
    Public Function DoesWordEndInEd(ByVal word As String) As Boolean
        Return word.Length > 3 AndAlso word.EndsWith(pastParticipleEnding)
    End Function

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ResourceDictionary.Dispose()
                ResourceDictionary = Nothing
            End If

            irregularAdjectives = Nothing
            disposedValue = True
        End If
    End Sub

    ' // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ' ~AdjectiveInflector()
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
