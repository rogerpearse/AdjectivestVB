Option Strict On
Option Explicit On

Imports Adjectivest

Public Class Form1

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.TextBox2.Text = ""
        Dim inflector As AdjectiveInflector = New AdjectiveInflector(True)
        Me.TextBox2.Text = inflector.GetAdjectiveInflections(TextBox1.Text.Trim)
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.AcceptButton = Button1

    End Sub

End Class
