Imports System.Text
Imports Adjectivest.Adjectivest

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
    Public Sub TestGarbageWordsJustReturnInputString()
        '-- This is the failure path.  Something that can't be inflected just returns the same string input
        Assert.AreEqual("zesty", inflector.GetAdjectiveInflections("zesty"))
        Assert.AreEqual("crungus", inflector.GetAdjectiveInflections("crungus"))
        Assert.AreEqual("woogle", inflector.GetAdjectiveInflections("woogle"))
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
        Assert.AreEqual("wide|wider|widest", inflector.GetAdjectiveInflections("wide", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllableEndingInDigraph()
        Assert.AreEqual("rich|richer|richest", inflector.GetAdjectiveInflections("rich", AdjectiveForm.Base))
        Assert.AreEqual("lush|lusher|lushest", inflector.GetAdjectiveInflections("lush", AdjectiveForm.Base))
        Assert.AreEqual("sick|sicker|sickest", inflector.GetAdjectiveInflections("sick", AdjectiveForm.Base))
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
    End Sub

    <TestMethod()>
    Public Sub TestTwoSyllableExceptions()
        ' TODO quiet and narrow do not work.
        'Assert.AreEqual("quiet|quieter|quietest", inflector.GetAdjectiveInflections("quiet", AdjectiveForm.Base))
        'Assert.AreEqual("narrow|narrower|narrowest", inflector.GetAdjectiveInflections("narrow", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestThreeSyllableHandling()
        Assert.AreEqual("important|more important|most important", inflector.GetAdjectiveInflections("important", AdjectiveForm.Base))
        Assert.AreEqual("expensive|more expensive|most expensive", inflector.GetAdjectiveInflections("expensive", AdjectiveForm.Base))
    End Sub

    <TestMethod()>
    Public Sub TestAFewIrregulars()
        Assert.AreEqual("little|less|least", inflector.GetAdjectiveInflections("little", AdjectiveForm.Base))
        Assert.AreEqual("much|more|most", inflector.GetAdjectiveInflections("much", AdjectiveForm.Base))
        Assert.AreEqual("far|farther|farthest", inflector.GetAdjectiveInflections("far", AdjectiveForm.Base))
    End Sub
End Class
