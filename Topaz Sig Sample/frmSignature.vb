Public Class frmSignature
#Region " Private Enumeration "
    Private Enum eTabletState
        Inactive
        Active
    End Enum

    Private Enum eRefreshMode As Byte
        ''' <summary>The Display is cleared at the specified location.</summary>
        Clear = 0
        ''' <summary>The Display is complemented at the specified location.</summary>
        Complement = 1
        ''' <summary>The contents of the background memory in the tablet are transferred to the LCD display, overwriting the contents of the LCD display.</summary>
        WriteOpaque = 2
        ''' <summary>The contents of the background memory in the tablet are transferred to the LCD display, and combined with the contents of the LCD display.</summary>
        WriteTransparent = 3
    End Enum

    Private Enum eSendGraphicDest
        Foreground = 0
        ''' <summary>Background memory in tablet</summary>
        Background = 1
    End Enum

    Private Enum eCaptureMode
        ''' <summary>no LCD commands are sent to the tablet</summary>
        None = 0
        ''' <summary>sets capture mode to be active with Autoerase in the tablet</summary>
        Autoerase = 1
        ''' <summary>sets the tablet to persistent ink capture without autoerase</summary>
        NoAutoerase = 2
        ''' <summary>signature ink is displayed inverted on a suitable dark background set using the Graphic functions.</summary>
        Inverted = 3
    End Enum
#End Region

    Private PenWidth As Integer = 5

    Private LCDSize As Size

#Region " Properties "
    Public Property Signature As clsSignature
#End Region

#Region " Private Methods "

    Private Sub frmSignature_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            SigPlusNET1.SetImagePenWidth(PenWidth) : SigPlusNET1.SetDisplayPenWidth(PenWidth)

            'get the LCD Size
            Dim iLCDSize As Integer = SigPlusNET1.LCDGetLCDSize
            LCDSize = New Size(iLCDSize And &HFFFF, (iLCDSize >> 16) And &HFFFF)

            SigPlusNET1.SetTabletState(eTabletState.Active)
            If SigPlusNET1.GetTabletState = eTabletState.Inactive Then
                MsgBox("Unable to activate the signature tablet, make sure it is connected to the PC and is operational.")
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End If

            SigPlusNET1.LCDRefresh(eRefreshMode.Clear, 0, 0, LCDSize.Width, LCDSize.Height)
            SigPlusNET1.SetTranslateBitmapEnable(False)
            SigPlusNET1.LCDSendGraphic(eSendGraphicDest.Background, eRefreshMode.WriteOpaque, 0, 20, My.Resources.Sign)
            SigPlusNET1.LCDSendGraphic(eSendGraphicDest.Background, eRefreshMode.WriteOpaque, 207, 4, My.Resources.OK)
            SigPlusNET1.LCDSendGraphic(eSendGraphicDest.Background, eRefreshMode.WriteOpaque, 15, 4, My.Resources.CLEAR)

            SigPlusNET1.SetLCDCaptureMode(eCaptureMode.NoAutoerase)


            SigPlusNET1.ClearSigWindow(1)
            SigPlusNET1.LCDRefresh(eRefreshMode.WriteOpaque, 0, 0, LCDSize.Width, LCDSize.Height) 'Brings the background image already loaded into foreground
            SigPlusNET1.ClearTablet()
            SigPlusNET1.KeyPadAddHotSpot(1, 1, 10, 5, 53, 17) 'For CLEAR button
            SigPlusNET1.KeyPadAddHotSpot(2, 1, 197, 5, 19, 17) 'For OK button
            SigPlusNET1.LCDSetWindow(0, 22, 238, 40)
            SigPlusNET1.SetSigWindow(1, 0, 22, 240, 40) 'Sets the area where ink is permitted in the SigPlus object

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub frmSignature_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub frmSignature_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        SigPlusNET1.LCDRefresh(eRefreshMode.Clear, 0, 0, 240, 64)
        SigPlusNET1.LCDSetWindow(0, 0, 240, 64)
        SigPlusNET1.SetSigWindow(1, 0, 0, 240, 64)
        SigPlusNET1.KeyPadClearHotSpotList()
        SigPlusNET1.SetLCDCaptureMode(eCaptureMode.Autoerase)
        SigPlusNET1.SetTabletState(eTabletState.Inactive)
    End Sub

    Private Sub btnAccept_Click(sender As Object, e As EventArgs) Handles btnAccept.Click
        Try
            Cursor.Current = Cursors.WaitCursor
            If Signature.ID.Equals(Guid.Empty) Then Signature.ID = Guid.NewGuid

            SigPlusNET1.AutoKeyStart()
            SigPlusNET1.SetAutoKeyData(Signature.ID.ToString) 'use the id as a encryption key
            SigPlusNET1.AutoKeyFinish()

            SigPlusNET1.SetEncryptionMode(clsSignature.eEncryptionMode.High)
            Signature.Signature = SigPlusNET1.GetSigString.ToByteArray
            Signature.Receipt = SigPlusNET1.GetSigReceipt 'similar to the key receipt. Forms receipt by using the auto key generation algorithm on the signature file and the result can be used to verify that the signature has not been modified.

            SigPlusNET1.LCDRefresh(0, 0, 0, 240, 64)
            Dim f As Font = New System.Drawing.Font("Arial", 9.0F, System.Drawing.FontStyle.Regular)
            SigPlusNET1.LCDWriteString(0, 2, 35, 25, f, "Signature capture complete.")
            System.Threading.Thread.Sleep(2000)

        Catch ex As Exception
            Cursor.Current = Cursors.Default
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        SigPlusNET1.ClearSigWindow(1)
        SigPlusNET1.LCDRefresh(eRefreshMode.Complement, 10, 0, 53, 17) 'Refresh LCD at 'CLEAR' to indicate to user that this option has been sucessfully chosen
        SigPlusNET1.LCDRefresh(eRefreshMode.WriteOpaque, 0, 0, LCDSize.Width, LCDSize.Height) 'Brings the background image already loaded into foreground
        SigPlusNET1.ClearTablet()
        btnAccept.Enabled = False
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        'nothing to do
    End Sub

    Private Sub SigPlusNET1_PenUp(sender As Object, e As EventArgs) Handles SigPlusNET1.PenUp
        btnAccept.Enabled = True
        Select Case True
            Case SigPlusNET1.KeyPadQueryHotSpot(1) 'CLEAR
                btnClear.PerformClick()
            Case SigPlusNET1.KeyPadQueryHotSpot(2) 'OK
                SigPlusNET1.ClearSigWindow(1)
                SigPlusNET1.LCDRefresh(1, 210, 3, 14, 14) 'Refresh LCD at 'OK' to indicate to user that this option has been successfully chosen
                If SigPlusNET1.NumberOfTabletPoints = 0 Then
                    SigPlusNET1.LCDRefresh(0, 0, 0, 240, 64)
                    SigPlusNET1.LCDSendGraphic(eSendGraphicDest.Foreground, eRefreshMode.WriteOpaque, 4, 20, My.Resources.please)
                    System.Threading.Thread.Sleep(750)
                    SigPlusNET1.ClearTablet()
                    SigPlusNET1.LCDRefresh(eRefreshMode.WriteOpaque, 0, 0, LCDSize.Width, LCDSize.Height)
                    SigPlusNET1.SetLCDCaptureMode(eCaptureMode.NoAutoerase)   'Sets mode so ink will not disappear after a few seconds
                    btnAccept.Enabled = False
                Else
                    btnAccept.PerformClick()
                End If
        End Select
    End Sub

#End Region
End Class