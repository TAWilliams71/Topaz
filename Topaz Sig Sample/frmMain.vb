Public Class frmMain
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim fSig As New frmSignature
        If fSig.ShowDialog(Me) = DialogResult.Cancel Then Exit Sub
        Dim Signature As clsSignature = fSig.Signature

        'show the signature

    End Sub
End Class
