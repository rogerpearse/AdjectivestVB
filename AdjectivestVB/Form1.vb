Option Strict On
Option Explicit On

Imports Adjectivest.Adjectivest

Public Class Form1

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim inflector As AdjectiveInflector = New AdjectiveInflector(True)
        MsgBox(inflector.GetAdjectiveInflections("good"))
    End Sub
End Class
