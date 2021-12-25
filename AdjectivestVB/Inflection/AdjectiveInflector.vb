Imports System
Imports System.IO
Imports Adjectivest.Adjectivest.WordProcessor
Imports Adjectivest.Adjectivest.Phonemes
Imports Adjectivest.Adjectivest.Core.AdjectiveDictionary
Imports System.Runtime.InteropServices

Namespace Adjectivest
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

            '-- If nothing can be found to work with, just return the word
            If inflectionBase.Phonemes Is Nothing Then
                Return word
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

        ''' <summary>
        ''' This does not appear to be called from anywhere.
        ''' </summary>
        Public Function IsKnownIrregular(ByVal word As String, _
                                         <Out()> ByRef comparative As String, _
                                         <Out()> ByRef superlative As String) As Boolean
            comparative = Nothing
            superlative = Nothing

            For i = 0 To irregularAdjectives.Length - 1
                Dim irregular = irregularAdjectives(i)

                If irregular.StartsWith(word) Then
                    Dim splitIrregular = irregular.Split(commaDelimiter)
                    comparative = splitIrregular(1)
                    superlative = splitIrregular(2)
                    Return True
                End If
            Next

            Return False
        End Function

        Public Function IsKnownIrregular(ByVal word As String, _
                                         <Out()> ByRef form As AdjectiveForm, _
                                         <Out()> ByRef baseForm As String, _
                                         <Out()> ByRef comparative As String, _
                                         <Out()> ByRef superlative As String) As Boolean
            baseForm = Nothing
            comparative = Nothing
            superlative = Nothing
            form = AdjectiveForm.None

            For i = 0 To irregularAdjectives.Length - 1
                Dim irregular = irregularAdjectives(i)

                If irregular.Contains(word) Then
                    Dim splitIrregular = irregular.Split(commaDelimiter)
                    baseForm = splitIrregular(0)
                    comparative = splitIrregular(1)
                    superlative = splitIrregular(2)

                    For j = 0 To splitIrregular.Length - 1
                        If splitIrregular(j) = word Then
                            Select Case j
                                Case 0
                                    form = Adjectivest.AdjectiveForm.Base
                                Case 1
                                    form = Adjectivest.AdjectiveForm.Comparative
                                Case 2
                                    form = Adjectivest.AdjectiveForm.Superlative
                                Case Else
                                    form = Adjectivest.AdjectiveForm.None
                            End Select
                        End If
                    Next

                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' This does not appear to be called from anywhere.
        ''' </summary>
        Public Function IsKnownIrregular(ByVal word As String) As Boolean
            Dim comparative As String = Nothing
            Dim superlative As String = Nothing
            Return IsKnownIrregular(word, comparative, superlative)
        End Function

        ''' <summary>
        ''' Do we use more/most or -er/-est?
        ''' </summary>
        Public Function GetInflectionType(ByVal wordText As String, _
                                          ByVal syllableCount As Integer, _
                                          ByVal doesWordEndInEd As Boolean) As InflectionType

            If (syllableCount < 2 _
                OrElse wordText.EndsWith(ySuffix) _
                OrElse (syllableCount < 3 _
                        And wordText.EndsWith(leSuffix))) _
            AndAlso Not doesWordEndInEd Then
                Return InflectionType.ErEst
            End If
            Return InflectionType.MoreMost

        End Function

        Public Function GetComparative(ByVal word As WordObj, ByVal inflectionType As InflectionType) As String
            If inflectionType = Adjectivest.InflectionType.MoreMost Then
                Return GetMultiSyllabicComparative(word, AdjectiveForm.Comparative)
            End If
            ' InflectionType.ErEst
            Return GetBasicComparative(word, AdjectiveForm.Comparative)
        End Function

        Public Function GetSuperlative(ByVal word As WordObj, ByVal inflectionType As InflectionType) As String
            If inflectionType = Adjectivest.InflectionType.MoreMost Then
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

            Dim suffix As String = Adjectivest.AdjectiveInflector.basicSuperlative
            If comparisonType = Adjectivest.AdjectiveForm.Comparative Then
                suffix = Adjectivest.AdjectiveInflector.basicComparative
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

                        ' This is the original code, which should return true for "tall" but in fact returns false.
                        ' It also fails for rich and other digraphs
                        'Dim endsInNonDigraphicDualConsonant As Boolean = TypeOf wordObj.GetPenultimatePhoneme() Is ConsonantPhoneme

                        ' A digraph is two letters that combine together to correspond to one sound (phoneme). 
                        ' Examples of consonant digraphs are ‘ch, sh, th, ng’.

                        Dim lastPhoneme As Phoneme = wordObj.GetLastPhoneme()
                        Dim penultimateConsonant As Phoneme = wordObj.GetPenultimatePhoneme()

                        ' Rough and ready fix for "tall"
                        Dim endsInNonDigraphicDualConsonant As Boolean = False
                        Dim lastChar As Char = word(word.Length - 1)
                        Dim penultimateChar As Char = word(word.Length - 2)
                        If lastChar = penultimateChar Then
                            endsInNonDigraphicDualConsonant = True
                        End If

                        If lastConsonant.DoubleOnShortVowel _
                            And Not (TypeOf lastPhoneme Is ConsonantPhoneme And lastConsonant.Formation = ConsonantFormation.Digraph) _
                            AndAlso endsInNonDigraphicDualConsonant = False _
                            And finalValue.ToLower.EndsWith("nd") = False _
                            And finalValue.ToLower.EndsWith("rt") = False Then '-- Bodging nd and rt - almost certainly wrong way to do this
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

        Private Function GetMultiSyllabicComparative(ByVal wordObj As WordObj, ByVal comparisonType As AdjectiveForm) As String
            Dim finalValue = wordObj.Word
            Dim prefix As String = ""

            Select Case comparisonType
                Case Adjectivest.AdjectiveForm.Comparative
                    prefix = Adjectivest.AdjectiveInflector.multiSyllabicComparative
                Case Adjectivest.AdjectiveForm.Superlative
                    prefix = Adjectivest.AdjectiveInflector.multiSyllabicSuperlative
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
End Namespace
