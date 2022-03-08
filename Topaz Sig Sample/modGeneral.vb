Module modGeneral

    <Runtime.CompilerServices.Extension()>
    Friend Function ToByteArray(ByVal value As String) As Byte()
        Return Text.Encoding.ASCII.GetBytes(value)
    End Function

    <Runtime.CompilerServices.Extension()>
    Friend Function ByteArrayToString(ByVal value As Byte()) As String
        If value Is Nothing Then Return Nothing
        Return Text.Encoding.ASCII.GetString(value)
    End Function

End Module
