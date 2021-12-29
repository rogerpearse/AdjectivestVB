Imports System.Text
Imports Adjectivest

<TestClass()>
<DeploymentItem("AdjectivestResources", "AdjectivestResources")> _
Public Class DoesWordEndInEd

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
    Public Sub TestIsPastParticiple()
        Assert.AreEqual(True, inflector.DoesWordEndInEd("ended"))
        Assert.AreEqual(False, inflector.DoesWordEndInEd("done"))
        Assert.AreEqual(False, inflector.DoesWordEndInEd("red"))
        Assert.AreEqual(False, inflector.DoesWordEndInEd(""))
    End Sub

End Class
