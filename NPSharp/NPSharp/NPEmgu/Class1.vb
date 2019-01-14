Imports Emgu.CV
Imports Emgu.CV.Structure
Imports System.Runtime.InteropServices

Public Class NPEmgu
    Shared Function Ravel(A As Mat) As Mat
        Return A.Reshape(1, 1)
    End Function
    Shared Function Ones(ByVal Cols As Integer) As Mat
        Return Mat.Ones(1, Cols, CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Ones(ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Return Mat.Ones(Rows, Cols, CvEnum.DepthType.Cv32S, 1)
    End Function

    Shared Function Zeros(ByVal Cols As Integer) As Mat
        Return Emgu.CV.Mat.Zeros(1, Cols, CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Return Mat.Zeros(Rows, Cols, CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer, Depth As Integer) As Mat
        Return Mat.Zeros(Rows, Cols, CvEnum.DepthType.Cv32S, Depth)
    End Function

    Shared Function Multiply(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.Multiply(A, B, Ret) : Return Ret
    End Function
    Shared Function Divide(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.Divide(A, B, Ret) : Return Ret
    End Function
    Shared Function Add(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.Add(A, B, Ret) : Return Ret
    End Function
    Shared Function Substract(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.Subtract(A, B, Ret) : Return Ret
    End Function

    Shared Function eye(ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Return Mat.Eye(Rows, Cols, CvEnum.DepthType.Default, 1)
    End Function
    Shared Function exp(ByVal A As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.Exp(A, Ret) : Return Ret
    End Function
    Shared Function isreal(A As Emgu.CV.Mat)
        Return A
    End Function
    Shared Function Diag(ByVal A As Mat, Optional ByVal D As Integer = 0) As Mat
        Return A.Diag(D)
    End Function
    Shared Function VStack(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.VConcat(A, B, Ret) : Return Ret
    End Function
    Shared Function HStack(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.HConcat(A, B, Ret) : Return Ret
    End Function
    Shared Function count_nonzero(A As Mat)
        Return Emgu.CV.CvInvoke.CountNonZero(A)
    End Function
    Shared Function dot(A As Mat, B As Mat) As Mat
        Dim Sum As Short()() = (NPNative.Multiply(GetData(A.Row(0)), GetData(B.Row(0))))
        NPEmgu.SetData(A, Sum.ToArray)
        Return A
    End Function

    'Default	-1	default
    'Cv8U	0	Byte
    'Cv8S	1	SByte
    'Cv16U	2	UInt16
    'Cv16S	3	Int16
    'Cv32S	4	Int32
    'Cv32F	5	float
    'Cv64F	6	double
    Shared Function GetData(ByVal A As Mat) As Object
        Select Case A.Depth
            Case CvEnum.DepthType.Cv8U : Return GetCV8U(A) '    Byte
            Case CvEnum.DepthType.Cv16S : Return GetCV16S(A) '  Int16
            Case CvEnum.DepthType.Cv32S : Return GetCV32S(A) '  Integer
            Case CvEnum.DepthType.Cv64F : Return GetCV64F(A) '  Double
            Case Else : Throw New NotImplementedException
        End Select
        Return Nothing
    End Function
    Shared Function GetData(A As Mat, Row As Integer) As Object
        Return GetData(A.Row(Row))
    End Function
    Shared Function GetData(A As Mat, Row As Integer, Col As Integer) As Object
        Return GetData(A.Row(Row).Col(Col))
    End Function
    Shared Function GetData(A As Mat, Row As Integer, Col As Integer, Index As Integer) As Object
        Return A.Row(Row).Col(Col).GetData(Index)
    End Function
    Private Shared Function GetCV8U(ByVal A As Mat) As Byte()()
        Dim RET As New List(Of Byte())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Byte
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    Private Shared Function GetCV16S(ByVal A As Mat) As Int16()()
        Dim RET As New List(Of Int16())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Int16
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    Private Shared Function GetCV32S(ByVal A As Mat) As Integer()()
        Dim RET As New List(Of Integer())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Integer
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function
    Private Shared Function GetCV64F(ByVal A As Mat) As Double()()
        Dim RET As New List(Of Double())
        For i As Integer = 0 To A.Rows - 1
            Dim MDat(A.Width - 1) As Double
            Marshal.Copy(A.Row(i).DataPointer, MDat, 0, A.Row(i).Width)
            RET.Add(MDat)
        Next
        Return RET.ToArray
    End Function

    Shared Sub SetData(ByVal A As Mat, ByVal Values As Object)
        Select Case A.Depth
            Case CvEnum.DepthType.Cv8U : SetCV8U(A, Values) '   Byte
            Case CvEnum.DepthType.Cv16S : SetCV16S(A, Values) ' Int16
            Case CvEnum.DepthType.Cv32S : SetCV32S(A, Values) ' Integer
            Case CvEnum.DepthType.Cv64F : SetCV64F(A, Values) ' Double
            Case Else : Throw New NotImplementedException
        End Select
    End Sub
    Private Shared Sub SetCV8U(ByVal A As Mat, ByVal Values As Byte()())
        For i As Integer = 0 To A.Rows - 1
            Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
        Next
    End Sub
    Private Shared Sub SetCV16S(ByVal A As Mat, ByVal Values As Int16()())
        For i As Integer = 0 To A.Rows - 1
            Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
        Next
    End Sub
    Private Shared Sub SetCV32S(ByVal A As Mat, ByVal Values As Integer()())
        For i As Integer = 0 To A.Rows - 1
            Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
        Next
    End Sub
    Private Shared Sub SetCV64F(ByVal A As Mat, ByVal Values As Double()())
        For i As Integer = 0 To A.Rows - 1
            Marshal.Copy(Values(i), 0, A.Row(i).DataPointer, A.Row(i).Width)
        Next
    End Sub



    Shared Function cvtColor(Origin As Mat, Conversation As CvEnum.ColorConversion) As Mat
        Dim RES As New Mat
        Emgu.CV.CvInvoke.CvtColor(Origin, RES, Conversation)
        Return RES
    End Function
    Shared Function ConvertTo(ByVal Mat As Mat, ByVal Depth As CvEnum.DepthType)
        Dim RET As New Mat : Mat.ConvertTo(RET, Depth) : Return RET
    End Function
    Shared Function Remap(ByVal Mat As Mat) As Mat
        Mat.Reshape(3)
    End Function

    Class eigen
        Property Values As New Mat
        Property Vectors As New Mat
        Sub New(A As Mat)
            Emgu.CV.CvInvoke.Eigen(A, Values, Vectors)
        End Sub
    End Class

    Class Interp2d
        Property X
        Property Y
        Property Z
        Property Kind As Emgu.CV.CvEnum.Inter
        Public Function Run(widthgridHR, heightgridHR)
            ' Emgu.CV.CvInvoke.Remap(grayorigin, bilinearinterp, widthgridLR, widthgridLR, CvEnum.Inter.Linear)
            Dim upscaledLR As New Mat
            Emgu.CV.CvInvoke.Resize(Z, upscaledLR, New System.Drawing.Size(widthgridHR.Length, heightgridHR.Length), , , CvEnum.Inter.Linear)
            Return upscaledLR
        End Function
        Sub New(_X As Matrix(Of Single), _Y As Matrix(Of Single), _Z As Mat, _Kind As Emgu.CV.CvEnum.Inter)
            X = _X
            Y = _Y
            Z = _Z
            Kind = _Kind
        End Sub
    End Class

    Class Gradient
        Property X As New Mat
        Property Y As New Mat
        Sub New(A As Mat)
            Emgu.CV.CvInvoke.SpatialGradient(ConvertTo(A, CvEnum.DepthType.Cv8U), X, Y)
        End Sub
    End Class

    Shared Function MatFromArray(Array As Object) As Mat
        Select Case (Array.GetType)
            Case GetType(System.Double()())
                Dim Ret = New Mat(Array.length, Array(0).length, CvEnum.DepthType.Cv64F, 1)
                SetData(Ret, Array) : Return Ret
        End Select

        ' Dim Ret As New Mat(Array.count, Array(0).count, )

    End Function
End Class