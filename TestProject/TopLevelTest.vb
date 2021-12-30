Imports System.Text
Imports Adjectivest

' This is all setup for Visual Studio 2010 Ultimate.
' Test setup notes.
' If the test cannot find the files in ActivestResources, despite each file property being set to "Copy Always"
' then try this.
' 1. Set the DeploymentItem on the test to copy a folder to the TestResults/~/Out
' 2. In VS2010, look for the Local.testsettings file in the Solution.
' Open this, look at Deployment tab.  Make sure the "Enable Deployment"  is checked 
' and add the ActivestResources folder to it.  Rebuild and run the test again.
' Coverage can be enabled from Local.testsettings, Data and Diagnostic tab, enable coverage and double-click on the row to select the main project.
<TestClass()>
<DeploymentItem("AdjectivestResources", "AdjectivestResources")> _
Public Class TopLevelTest

    '-- The entry point to the code
    Private inflector As AdjectiveInflector

    ' Use TestInitialize to run code before running each test
    <TestInitialize()> _
    Public Sub setUp()
        inflector = New AdjectiveInflector(True)
        Debug.Print("-------------------- New Test ----------------------------------------------------")
    End Sub

    <TestCleanup()> _
    Public Sub tearDown()
    End Sub

    '----------------------------------------------------------
    '--  ** TESTS **
    '----------------------------------------------------------

    <TestMethod()>
    Public Sub TestGood()
        Assert.AreEqual("good|better|best", inflector.GetAdjectiveInflections("good"))
    End Sub

    ''' <summary>
    ''' Try the words from the C# text project
    ''' </summary>
    <TestMethod()>
    Public Sub TestStandardData()
        Assert.AreEqual("sad|sadder|saddest", inflector.GetAdjectiveInflections("sad"))
        Assert.AreEqual("gigantic|more gigantic|most gigantic", inflector.GetAdjectiveInflections("gigantic"))
        Assert.AreEqual("elegant|more elegant|most elegant", inflector.GetAdjectiveInflections("elegant"))
        Assert.AreEqual("red|redder|reddest", inflector.GetAdjectiveInflections("red"))
        Assert.AreEqual("pissed|more pissed|most pissed", inflector.GetAdjectiveInflections("pissed"))
        Assert.AreEqual("exciting|more exciting|most exciting", inflector.GetAdjectiveInflections("exciting"))
        Assert.AreEqual("good|better|best", inflector.GetAdjectiveInflections("better"))
        Assert.AreEqual("cool|cooler|coolest", inflector.GetAdjectiveInflections("coolest"))
        Assert.AreEqual("round|rounder|roundest", inflector.GetAdjectiveInflections("round"))
        Assert.AreEqual("fantastic|more fantastic|most fantastic", inflector.GetAdjectiveInflections("fantastic"))
        Assert.AreEqual("large|larger|largest", inflector.GetAdjectiveInflections("large"))
        Assert.AreEqual("big|bigger|biggest", inflector.GetAdjectiveInflections("big"))
        Assert.AreEqual("bad|worse|worst", inflector.GetAdjectiveInflections("bad"))
        Assert.AreEqual("good|better|best", inflector.GetAdjectiveInflections("good"))
        Assert.AreEqual("zany|zanier|zaniest", inflector.GetAdjectiveInflections("zany"))
        Assert.AreEqual("aggravating|more aggravating|most aggravating", inflector.GetAdjectiveInflections("aggravating"))
    End Sub

    <TestMethod()>
    Public Sub TestLeSuffix()
        Assert.AreEqual("able|abler|ablest", inflector.GetAdjectiveInflections("able"))
        Assert.AreEqual("capable|more capable|most capable", inflector.GetAdjectiveInflections("capable"))
        Assert.AreEqual("unbelievable|more unbelievable|most unbelievable", inflector.GetAdjectiveInflections("unbelievable"))
    End Sub

    <TestMethod()>
    Public Sub TestGarbageWordsJustReturnMoreMost()
        Assert.AreEqual("zesty|more zesty|most zesty", inflector.GetAdjectiveInflections("zesty"))
        Assert.AreEqual("crungus|more crungus|most crungus", inflector.GetAdjectiveInflections("crungus"))
        Assert.AreEqual("woogle|more woogle|most woogle", inflector.GetAdjectiveInflections("woogle"))
    End Sub

    <TestMethod()>
    Public Sub TestPassingArgumentHasNoEffectOnOutput()
        ' If you already know what form your input adjective is, setting word to the corresponding value will speed up the process and be less error-prone. 
        ' Leaving it as AdjectiveForm.None, the program will attempt to infer its form.
        ' Output is the same either way
        ' The C# code has timings in its test routines.
        Assert.AreEqual("sad|sadder|saddest", inflector.GetAdjectiveInflections("sad", AdjectiveForm.Comparative))
    End Sub

    <TestMethod()>
    Public Sub TestCapitalHandling()
        '-- This looks a bit eccentric, but is fine.  We're not going to have capitals normally.
        Assert.AreEqual("Sad|Sadder|Saddest", inflector.GetAdjectiveInflections("Sad"))
        Assert.AreEqual("SAD|SADDer|SADDest", inflector.GetAdjectiveInflections("SAD"))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllableHandling()
        Assert.AreEqual("fat|fatter|fattest", inflector.GetAdjectiveInflections("fat", AdjectiveForm.Base))
        Assert.AreEqual("slim|slimmer|slimmest", inflector.GetAdjectiveInflections("slim", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllableEndingInE()
        Assert.AreEqual("wide|wider|widest", inflector.GetAdjectiveInflections("wide", AdjectiveForm.Base))
        Assert.AreEqual("rare|rarer|rarest", inflector.GetAdjectiveInflections("rare", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllableEndingInDigraph()
        Assert.AreEqual("rich|richer|richest", inflector.GetAdjectiveInflections("rich", AdjectiveForm.Base))
        Assert.AreEqual("lush|lusher|lushest", inflector.GetAdjectiveInflections("lush", AdjectiveForm.Base))
        Assert.AreEqual("sick|sicker|sickest", inflector.GetAdjectiveInflections("sick", AdjectiveForm.Base))
        Assert.AreEqual("round|rounder|roundest", inflector.GetAdjectiveInflections("round", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestGrand()
        ' Bodged this
        Assert.AreEqual("grand|grander|grandest", inflector.GetAdjectiveInflections("grand", AdjectiveForm.Base))
        Assert.AreEqual("short|shorter|shortest", inflector.GetAdjectiveInflections("short", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllableWithDoubledConsonantHandling()
        Assert.AreEqual("tall|taller|tallest", inflector.GetAdjectiveInflections("tall"))
    End Sub

    <TestMethod()>
    Public Sub TestTwoSyllableHandling()
        Assert.AreEqual("happy|happier|happiest", inflector.GetAdjectiveInflections("happy", AdjectiveForm.Base))
        Assert.AreEqual("simple|simpler|simplest", inflector.GetAdjectiveInflections("simple", AdjectiveForm.Base))
        Assert.AreEqual("busy|busier|busiest", inflector.GetAdjectiveInflections("busy", AdjectiveForm.Base))
        Assert.AreEqual("tilted|more tilted|most tilted", inflector.GetAdjectiveInflections("tilted", AdjectiveForm.Base))
        Assert.AreEqual("useful|more useful|most useful", inflector.GetAdjectiveInflections("useful", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestTwoSyllableExceptions()
        ' TODO Find a way to code for these two syllable words. Currently "quiet", "narrow", "clever" use more/most
        'Assert.AreEqual("quiet|quieter|quietest", inflector.GetAdjectiveInflections("quiet", AdjectiveForm.Base))
        'Assert.AreEqual("narrow|narrower|narrowest", inflector.GetAdjectiveInflections("narrow", AdjectiveForm.Base))
        'Assert.AreEqual("clever|cleverer|cleverest", inflector.GetAdjectiveInflections("clever", AdjectiveForm.Base))
        'Assert.AreEqual("handsome|handsomer|handsomest", inflector.GetAdjectiveInflections("handsome", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestThreeSyllableHandling()
        Assert.AreEqual("important|more important|most important", inflector.GetAdjectiveInflections("important", AdjectiveForm.Base))
        Assert.AreEqual("expensive|more expensive|most expensive", inflector.GetAdjectiveInflections("expensive", AdjectiveForm.Base))
        Assert.AreEqual("popular|more popular|most popular", inflector.GetAdjectiveInflections("popular", AdjectiveForm.Base))
        Assert.AreEqual("valiant|more valiant|most valiant", inflector.GetAdjectiveInflections("valiant", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestAFewIrregulars()
        Assert.AreEqual("little|less|least", inflector.GetAdjectiveInflections("little", AdjectiveForm.Base))
        Assert.AreEqual("much|more|most", inflector.GetAdjectiveInflections("much", AdjectiveForm.Base))
        Assert.AreEqual("far|farther|farthest", inflector.GetAdjectiveInflections("far", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOthers()
        Assert.AreEqual("orange|more orange|most orange", inflector.GetAdjectiveInflections("orange", AdjectiveForm.Base))  ' colour
        Assert.AreEqual("pious|more pious|most pious", inflector.GetAdjectiveInflections("pious", AdjectiveForm.Base))
        Assert.AreEqual("soon|sooner|soonest", inflector.GetAdjectiveInflections("soon", AdjectiveForm.Base))
        Assert.AreEqual("high|higher|highest", inflector.GetAdjectiveInflections("high", AdjectiveForm.Base))
        Assert.AreEqual("gentle|gentler|gentlest", inflector.GetAdjectiveInflections("gentle", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllableWords()
        Assert.AreEqual("bald|balder|baldest", inflector.GetAdjectiveInflections("bald", AdjectiveForm.Base))
        Assert.AreEqual("bent|benter|bentest", inflector.GetAdjectiveInflections("bent", AdjectiveForm.Base))
        Assert.AreEqual("blunt|blunter|bluntest", inflector.GetAdjectiveInflections("blunt", AdjectiveForm.Base))
        Assert.AreEqual("calm|calmer|calmest", inflector.GetAdjectiveInflections("calm", AdjectiveForm.Base))
        Assert.AreEqual("daft|dafter|daftest", inflector.GetAdjectiveInflections("daft", AdjectiveForm.Base))
        Assert.AreEqual("damp|damper|dampest", inflector.GetAdjectiveInflections("damp", AdjectiveForm.Base))
        Assert.AreEqual("fast|faster|fastest", inflector.GetAdjectiveInflections("fast", AdjectiveForm.Base))
        Assert.AreEqual("fast|faster|fastest", inflector.GetAdjectiveInflections("fast", AdjectiveForm.Base))
        Assert.AreEqual("gaunt|gaunter|gauntest", inflector.GetAdjectiveInflections("gaunt", AdjectiveForm.Base))
        Assert.AreEqual("gold|golder|goldest", inflector.GetAdjectiveInflections("gold", AdjectiveForm.Base))
        Assert.AreEqual("hard|harder|hardest", inflector.GetAdjectiveInflections("hard", AdjectiveForm.Base))
        Assert.AreEqual("numb|number|numbest", inflector.GetAdjectiveInflections("numb", AdjectiveForm.Base))
        Assert.AreEqual("pert|perter|pertest", inflector.GetAdjectiveInflections("pert", AdjectiveForm.Base))
        Assert.AreEqual("plump|plumper|plumpest", inflector.GetAdjectiveInflections("plump", AdjectiveForm.Base))
        Assert.AreEqual("scant|scanter|scantest", inflector.GetAdjectiveInflections("scant", AdjectiveForm.Base))
        Assert.AreEqual("sharp|sharper|sharpest", inflector.GetAdjectiveInflections("sharp", AdjectiveForm.Base))
        Assert.AreEqual("soft|softer|softest", inflector.GetAdjectiveInflections("soft", AdjectiveForm.Base))
        Assert.AreEqual("strict|stricter|strictest", inflector.GetAdjectiveInflections("strict", AdjectiveForm.Base))
        Assert.AreEqual("swift|swifter|swiftest", inflector.GetAdjectiveInflections("swift", AdjectiveForm.Base))
        Assert.AreEqual("warm|warmer|warmest", inflector.GetAdjectiveInflections("warm", AdjectiveForm.Base))
        Assert.AreEqual("vast|vaster|vastest", inflector.GetAdjectiveInflections("vast", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestTwoSyllableEndingInIle()
        Assert.AreEqual("fertile|fertiler|fertilest", inflector.GetAdjectiveInflections("fertile", AdjectiveForm.Base))
        Assert.AreEqual("fragile|fragiler|fragilest", inflector.GetAdjectiveInflections("fragile", AdjectiveForm.Base))
        Assert.AreEqual("futile|futiler|futilest", inflector.GetAdjectiveInflections("futile", AdjectiveForm.Base))
        Assert.AreEqual("hostile|more hostile|most hostile", inflector.GetAdjectiveInflections("hostile", AdjectiveForm.Base)) '-- irregular
        Assert.AreEqual("sterile|more sterile|most sterile", inflector.GetAdjectiveInflections("sterile", AdjectiveForm.Base)) '-- irregular
        Assert.AreEqual("worthwhile|more worthwhile|most worthwhile", inflector.GetAdjectiveInflections("worthwhile", AdjectiveForm.Base)) '-- irregular
    End Sub

    <TestMethod()>
    Public Sub TestTwoSyllableEndingInAle()
        Assert.AreEqual("female|femaler|femalest", inflector.GetAdjectiveInflections("female", AdjectiveForm.Base))
        Assert.AreEqual("male|maler|malest", inflector.GetAdjectiveInflections("male", AdjectiveForm.Base))

    End Sub

    <TestMethod()>
    Public Sub TestConsonantVowelConsonantDoesNotDouble()
        ' https://speakspeak.com/resources/english-grammar-rules/english-spelling-rules/double-consonant-adjective-before-er-est
        Assert.AreEqual("thin|thinner|thinnest", inflector.GetAdjectiveInflections("thin", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestEndsInYorWDoesNotDouble()
        ' https://speakspeak.com/resources/english-grammar-rules/english-spelling-rules/double-consonant-adjective-before-er-est
        Assert.AreEqual("grey|greyer|greyest", inflector.GetAdjectiveInflections("grey", AdjectiveForm.Base))
        Assert.AreEqual("slow|slower|slowest", inflector.GetAdjectiveInflections("slow", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestVowelVowelConsonantDoesNotDouble()
        ' https://speakspeak.com/resources/english-grammar-rules/english-spelling-rules/double-consonant-adjective-before-er-est
        Assert.AreEqual("cheap|cheaper|cheapest", inflector.GetAdjectiveInflections("cheap", AdjectiveForm.Base))
        Assert.AreEqual("clear|clearer|clearest", inflector.GetAdjectiveInflections("clear", AdjectiveForm.Base))
        Assert.AreEqual("dear|dearer|dearest", inflector.GetAdjectiveInflections("dear", AdjectiveForm.Base))
        Assert.AreEqual("fair|fairer|fairest", inflector.GetAdjectiveInflections("fair", AdjectiveForm.Base))
        Assert.AreEqual("near|nearer|nearest", inflector.GetAdjectiveInflections("near", AdjectiveForm.Base))
        Assert.AreEqual("sheer|sheerer|sheerest", inflector.GetAdjectiveInflections("sheer", AdjectiveForm.Base))
        Assert.AreEqual("taut|tauter|tautest", inflector.GetAdjectiveInflections("taut", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestVowelConsonantConsonantDoesNotDouble()
        ' https://speakspeak.com/resources/english-grammar-rules/english-spelling-rules/double-consonant-adjective-before-er-est
        Assert.AreEqual("old|older|oldest", inflector.GetAdjectiveInflections("old", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestNorth()
        Assert.AreEqual("north|norther|northest", inflector.GetAdjectiveInflections("north", AdjectiveForm.Base))
        Assert.AreEqual("goodlooking|more goodlooking|most goodlooking", inflector.GetAdjectiveInflections("goodlooking", AdjectiveForm.Base))
        Assert.AreEqual("wholesale|more wholesale|most wholesale", inflector.GetAdjectiveInflections("wholesale", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllablePastParticiplesMoonlightingAsAdjectives()
        ' TODO "hurt", "lost", "worn", "grown", "spent" don't work.  They're really past participles.  No idea how to code for these.
        'Assert.AreEqual("hurt|more hurt|most hurt", inflector.GetAdjectiveInflections("hurt", AdjectiveForm.Base))
        'Assert.AreEqual("lost|more lost|most lost", inflector.GetAdjectiveInflections("lost", AdjectiveForm.Base))
        'Assert.AreEqual("worn|more worn|most worn", inflector.GetAdjectiveInflections("worn", AdjectiveForm.Base))
        Assert.AreEqual("bored|more bored|most bored", inflector.GetAdjectiveInflections("bored", AdjectiveForm.Base))
        Assert.AreEqual("broken|more broken|most broken", inflector.GetAdjectiveInflections("broken", AdjectiveForm.Base))
        Assert.AreEqual("cooked|more cooked|most cooked", inflector.GetAdjectiveInflections("cooked", AdjectiveForm.Base))
        Assert.AreEqual("flawed|more flawed|most flawed", inflector.GetAdjectiveInflections("flawed", AdjectiveForm.Base))
        Assert.AreEqual("known|better known|best known", inflector.GetAdjectiveInflections("known", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestThreeSyllableEndingInY()
        Assert.AreEqual("willingly|more willingly|most willingly", inflector.GetAdjectiveInflections("willingly", AdjectiveForm.Base))
        Assert.AreEqual("ambulatory|more ambulatory|most ambulatory", inflector.GetAdjectiveInflections("ambulatory", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestManyAndMuch()
        Assert.AreEqual("many|more|most", inflector.GetAdjectiveInflections("many", AdjectiveForm.Base))
        Assert.AreEqual("much|more|most", inflector.GetAdjectiveInflections("much", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestMore()
        ' This can fail badly if irregularAdjectives.txt has a line with "more" in it above the lines for many|more|most etc.
        ' Check the file order
        Assert.AreEqual("many|more|most", inflector.GetAdjectiveInflections("more", AdjectiveForm.Comparative))
        Assert.AreEqual("many|more|most", inflector.GetAdjectiveInflections("more"))
    End Sub

    ' Run against the whole list of adjectives.  Then paste the output into MS Word and let spell-checker look for funnies.
    '<TestMethod()>
    Public Sub TestAll()

        Dim Adjectives As String() = CType(inflector.ResourceDictionary, InMemoryResourceDictionary).getAdjectivesLines()

        For i = 32 To Adjectives.Count - 1
            Dim s As String = Adjectives(i).Trim & " => " & inflector.GetAdjectiveInflections(Adjectives(i).Trim)
            Debug.Print(s.Replace("|", ", "))
        Next

    End Sub
End Class
