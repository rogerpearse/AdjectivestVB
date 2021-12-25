Imports System.Text
Imports Adjectivest.Adjectivest

<TestClass()>
<DeploymentItem("AdjectivestResources", "AdjectivestResources")> _
Public Class GetInflectionType

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
    Public Sub TestIrregularOneSyllable()
        Assert.AreEqual(InflectionType.ErEst, inflector.GetInflectionType("good", 1, False))
    End Sub

    <TestMethod()>
    Public Sub TestOneSyllable()
        Assert.AreEqual(InflectionType.ErEst, inflector.GetInflectionType("edgy", 1, False))
        Assert.AreEqual(InflectionType.ErEst, inflector.GetInflectionType("red", 1, False))
        Assert.AreEqual(InflectionType.ErEst, inflector.GetInflectionType("able", 1, False))
    End Sub

    <TestMethod()>
    Public Sub TestMore()
        'Assert.AreEqual(InflectionType.MoreMost, inflector.GetInflectionType("elated", 3, True))
        Assert.AreEqual(InflectionType.MoreMost, inflector.GetInflectionType("cowardly", 3, False))
    End Sub

End Class
