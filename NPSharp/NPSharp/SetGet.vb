Imports System.Runtime.InteropServices
Imports NPSharp.NPEmgu

Public Class NPPublic

#Region "Set/Get"
    <DebuggerStepThrough()>
    Shared Function ArrayToMat(ByVal Array As Object) As Mat
        Return MatFromArray(Array)
    End Function
    '<DebuggerStepThrough()>
    Shared Function MatFromArray(ByVal Array As Object) As Mat
        If IsNothing(Array) Then Return Nothing
        Select Case (Array.GetType)

            Case GetType(System.Double())
                Dim Ret = New Mat(1, Array.length, Emgu.CV.CvEnum.DepthType.Cv64F, 1)
                SetData(Ret, Array) : Return Ret
            Case GetType(System.Double()())
                Dim Ret = New Mat(Array.length, Array(0).length, Emgu.CV.CvEnum.DepthType.Cv64F, 1)
                Dim Test = Ret.NP_GetData
                SetData(Ret, Array) : Return Ret
            Case GetType(Double()()())
                Throw New Exception

            Case GetType(Integer)
                Dim Ret = New Mat(1, 1, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
                SetData(Ret, Array) : Return Ret
            Case GetType(Integer())
                Dim Ret = New Mat(1, Array.length, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
                SetData(Ret, Array) : Return Ret
            Case GetType(Integer()())
                Dim Ret = New Mat(Array.length, Array(0).length, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
                SetData(Ret, Array) : Return Ret
            Case GetType(Integer()()())
                Throw New Exception

        End Select
        Return Nothing
    End Function

    <DebuggerStepThrough()>
    Shared Function MatToArray(ByVal Mat As Mat) As Object
        Return GetData(Mat)
    End Function
    <DebuggerStepThrough()>
    Shared Function ArrayFromMat(ByVal Mat As Mat) As Object
        Return GetData(Mat)
    End Function

    '<DebuggerStepThrough()>
    Shared Function GetData(ByVal A As Mat) As Object
        If IsNothing(A) Then Return Nothing

        Select Case A.Depth
            Case Emgu.CV.CvEnum.DepthType.Cv8U : Return GetCV8U(A) '    Byte
            Case Emgu.CV.CvEnum.DepthType.Cv16S : Return GetCV16S(A) '  Int16
            Case Emgu.CV.CvEnum.DepthType.Cv32S : Return GetCV32S(A) '  Integer
            Case Emgu.CV.CvEnum.DepthType.Cv32F : Return GetCV32F(A) '  Float
            Case Emgu.CV.CvEnum.DepthType.Cv64F : Return GetCV64F(A) '  Double
            Case Else : Throw New NotImplementedException
        End Select
    End Function

    <DebuggerStepThrough()>
    Private Shared Function GetCV8U(ByVal A As Mat) As Byte()()
        Dim RET As New List(Of Byte())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Byte
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    <DebuggerStepThrough()>
    Private Shared Function GetCV16S(ByVal A As Mat) As Int16()()
        Dim RET As New List(Of Int16())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Int16
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    <DebuggerStepThrough()>
    Private Shared Function GetCV32S(ByVal A As Mat) As Integer()()
        Dim RET As New List(Of Integer())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Integer
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    <DebuggerStepThrough()>
    Private Shared Function GetCV32F(ByVal A As Mat) As Single()()
        Dim RET As New List(Of Single())
        For i As Single = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Single
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    <DebuggerStepThrough()>
    Private Shared Function GetCV64F(ByVal A As Mat) As Double()()
        Dim RET As New List(Of Double())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Double
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function

    '<DebuggerStepThrough()>
    Shared Sub SetData(ByVal A As Mat, ByVal Values As Object)
        Select Case A.Depth
            Case Emgu.CV.CvEnum.DepthType.Cv8U : SetCV8U(A, Values) '   Byte
            Case Emgu.CV.CvEnum.DepthType.Cv16S : SetCV16S(A, Values) ' Int16
            Case Emgu.CV.CvEnum.DepthType.Cv32S : SetCV32S(A, Values) ' Int32/Integer
            Case Emgu.CV.CvEnum.DepthType.Cv64F : SetCV64F(A, Values) ' Double
            Case Else : Throw New NotImplementedException

        End Select
    End Sub

    <DebuggerStepThrough()>
    Private Shared Sub SetCV8U(ByVal A As Mat, ByVal Values As Object)
        Select Case Values.GetType
            Case GetType(Byte()())
                For i As Integer = 0 To A.Rows - 1
                    Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
                Next
            Case GetType(Byte())
                Marshal.Copy(Values, 0, A.Row(0).DataPointer, A.Width)
        End Select
    End Sub
    <DebuggerStepThrough()>
    Private Shared Sub SetCV16S(ByVal A As Mat, ByVal Values As Object)
        Select Case Values.GetType
            Case GetType(Int16()())
                For i As Integer = 0 To A.Rows - 1
                    Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
                Next
            Case GetType(Int16())
                Marshal.Copy(Values, 0, A.Row(0).DataPointer, A.Width)
        End Select
    End Sub
    '<DebuggerStepThrough()>
    Private Shared Sub SetCV32S(ByVal A As Mat, ByVal Values As Object)
        Select Case Values.GetType
            Case GetType(Integer)
                Marshal.WriteInt32(A.Row(0).DataPointer, Values)
            Case GetType(Integer())
                Marshal.Copy(Values, 0, A.Row(0).DataPointer, A.Width)
            Case GetType(Integer()())
                For i As Integer = 0 To A.Rows - 1
                    Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
                Next
        End Select
    End Sub
    <DebuggerStepThrough()>
    Private Shared Sub SetCV64F(ByVal A As Mat, ByVal Values As Object)
        Select Case Values.GetType
            Case GetType(Double()())
                For i As Integer = 0 To A.Rows - 1
                    Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
                Next
            Case GetType(Double())
                Marshal.Copy(Values, 0, A.Row(0).DataPointer, A.Height)
            Case GetType(Double)
                Marshal.Copy({CDbl(Values)}, 0, A.Row(0).DataPointer, 1)
        End Select
    End Sub
#End Region

#Region "Html"
    Shared Function Show(ByVal A As Double()()) As String
        Dim S As New System.Text.StringBuilder
        S.Append("<HTML><HEAD></HEADY><BODY>")
        S.Append("<Table Border=1>")
        For Each Entry In A
            S.Append(AddHTMLLine(Entry))
        Next
        S.Append("</BODY></HTML>")
        Return S.ToString
    End Function
    Shared Function Show(ByVal A As Double()) As String
        Dim S As New System.Text.StringBuilder
        S.Append("<HTML><HEAD></HEADY><BODY>")
        S.Append("<Table Border=1>")
        S.Append(AddHTMLLine(A))
        S.Append("</BODY></HTML>")
        Return S.ToString
    End Function
    Private Shared Function AddHTMLLine(ByVal A As Double()) As String
        Return ("<TR><TD>" & String.Join("</TD><TD>", A) & "</TD>")
    End Function
#End Region

End Class
