Public Class clsSignature
    Public Enum eEncryptionMode
        ''' <summary>Clear text mode</summary>
        None = 0
        ''' <summary>40-bit DES. If a longer key is set, only 40 bits used</summary>
        Medium = 1
        ''' <summary>Higher security encryption mode</summary>
        High = 2
    End Enum

    Public Property ID As Guid
    Public Property Signature As Byte()
    Public Property Receipt As Int64

    Public Function GetSignatureAsImage() As Image
        Dim SigImageSize As New Size(2000, 700) 'this is usually a stored setting i create so the size can be changed
        If Signature Is Nothing Then Return Nothing
        Dim SigPlusNET As New Topaz.SigPlusNET
        SigPlusNET.AutoKeyStart()
        SigPlusNET.SetAutoKeyData(ID.ToString)
        SigPlusNET.AutoKeyFinish()

        SigPlusNET.SetEncryptionMode(eEncryptionMode.High)
        SigPlusNET.SetSigString(Signature.ByteArrayToString)

        SigPlusNET.BackgroundImage = My.Resources.Void_banner ' i do this when showing the user the stored signature so it cant be used, but not in the real one im creating
        SigPlusNET.SetImageFileFormat(4) '4=JPG Q=100
        SigPlusNET.SetImageXSize(SigImageSize.Width) 'sets image width in pixels (Default: 2000)
        SigPlusNET.SetImageYSize(SigImageSize.Height) 'sets image height in pixels (Default: 700)
        SigPlusNET.SetJustifyMode(5) '5-justify and zoom signature (center of control).
        Return SigPlusNET.GetSigImage

    End Function
End Class
