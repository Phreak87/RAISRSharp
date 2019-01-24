Imports System.Runtime.InteropServices
Imports NPSharp.NPPublic
Imports NPSharp.NPPublic.MatrixExplain
Imports System.Drawing


Public Class NPEmgu
    Enum DataType
        _Byte = 0
        _SByte = 1
        _Int16 = 2
        _UInt16 = 3
        _Int32 = 4
        _UInt32 = 5
        _Float = 6
        _Single = 7
        _Double = 8
    End Enum
    Enum NP_DataType
        Cv8U_Byte = Emgu.CV.CvEnum.DepthType.Cv8U
        Cv8S_SByte = Emgu.CV.CvEnum.DepthType.Cv8S '    Signed
        Cv16S_Int16 = Emgu.CV.CvEnum.DepthType.Cv16S '  Signed
        Cv16U_UInt16 = Emgu.CV.CvEnum.DepthType.Cv16U
        Cv32S_Int32 = Emgu.CV.CvEnum.DepthType.Cv32S '  Signed
        Cv32F_Single = Emgu.CV.CvEnum.DepthType.Cv32F
        Cv64F_Double = Emgu.CV.CvEnum.DepthType.Cv64F
    End Enum

#Region "Main-Matrix-Math"
    Shared Function Multiply(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim Ret As New Mat
        Dim AMat As Mat
        Dim BMat As Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : AMat = A : BMat = B

            Case CalcType.VV_VV_SYM : AMat = A : BMat = B
            Case CalcType.VH_VH_SYM : AMat = A : BMat = B
            Case CalcType.VV_VH_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)

            Case CalcType.VV_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VV_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)

            Case CalcType.M_VH_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_VV_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_N : AMat = A : BMat = Repeat(New Mat(B), A.rows, A.cols)
            Case CalcType.M_M_SYM : AMat = A : BMat = B
            Case CalcType.M_M_ASYM : Return Multiply_N_M(A, B)
            Case Else
                Return Nothing
        End Select
        Emgu.CV.CvInvoke.Multiply(AMat.OrgMat, BMat.OrgMat, Ret.OrgMat, , AMat.Depth)
        Return Ret
    End Function
    Shared Function Multiply_N_M(ByVal A As Object, ByVal B As Object) As Mat
        Dim R(A.height - 1)() As Double
        For Row As Integer = 0 To A.Height - 1
            Dim CC(B.width - 1) As Double
            For Col As Integer = 0 To B.Width - 1
                Dim VecA As Mat = A.Row(Row) '       Zeile von Array A
                Dim VecB As Mat = B.Col(Col).T '     Transponierte Spalte von Array B
                Dim SumAB As Mat = VecA * VecB
                Dim AddAB = Sum(SumAB)
                VecA.Dispose()
                VecB.Dispose()
                CC(Col) = AddAB
            Next
            R(Row) = CC
        Next
        Return New Mat(R)
    End Function
    Shared Function Substract(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim Ret As New Mat
        Dim AMat As Mat
        Dim BMat As Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : AMat = A : BMat = B

            Case CalcType.VV_VV_SYM : AMat = A : BMat = B
            Case CalcType.VH_VH_SYM : AMat = A : BMat = B
            Case CalcType.VV_VH_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)

            Case CalcType.VV_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VV_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)

            Case CalcType.M_VH_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_VV_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_N : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_M_SYM : AMat = A : BMat = B
            Case CalcType.M_M_ASYM
                Dim R As New List(Of Double())
                For Row As Integer = 0 To A.Height - 1
                    Dim CC(B.width - 1) As Double
                    For Col As Integer = 0 To B.Width - 1
                        Dim VecA As Mat = A.Row(Row) '       Zeile von Array A
                        Dim VecB As Mat = B.Col(Col).T '     Transponierte Spalte von Array B
                        Dim SumAB As Mat = VecA - VecB
                        Dim AddAB = Sum(SumAB)
                        VecA.Dispose()
                        VecB.Dispose()
                        CC(Col) = AddAB
                    Next
                    R.Add(CC)
                Next
                Return New Mat(R.ToArray)
            Case Else
                Return Nothing
        End Select
        Emgu.CV.CvInvoke.Subtract(AMat.OrgMat, BMat.OrgMat, Ret.OrgMat, , AMat.Depth)
        Return Ret
    End Function
    Shared Function Divide(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim Ret As New Mat
        Dim AMat As Mat
        Dim BMat As Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : AMat = A : BMat = B
            Case CalcType.VV_VV_SYM : AMat = A : BMat = B
            Case CalcType.VH_VH_SYM : AMat = A : BMat = B
            Case CalcType.VV_VH_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VV_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VV_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_VH_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_VV_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_N : AMat = A : BMat = Repeat(New Mat(B), A.rows, A.cols)
            Case CalcType.M_M_SYM : AMat = A : BMat = B
            Case CalcType.M_M_ASYM
                Dim R As New List(Of Double())
                For Row As Integer = 0 To A.Height - 1
                    Dim CC(B.width - 1) As Double
                    For Col As Integer = 0 To B.Width - 1
                        Dim VecA As Mat = A.Row(Row) '       Zeile von Array A
                        Dim VecB As Mat = B.Col(Col).T '     Transponierte Spalte von Array B
                        Dim SumAB As Mat = VecA / VecB
                        Dim AddAB = Sum(SumAB)
                        VecA.Dispose()
                        VecB.Dispose()
                        CC(Col) = AddAB
                    Next
                    R.Add(CC)
                Next
                Return New Mat(R.ToArray)
            Case Else
                Return Nothing
        End Select
        Emgu.CV.CvInvoke.Divide(AMat.OrgMat, BMat.OrgMat, Ret.OrgMat, , AMat.Depth)
        Return Ret
    End Function
    Shared Function Add(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim Ret As New Mat
        Dim AMat As Mat
        Dim BMat As Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : AMat = A : BMat = B

            Case CalcType.VV_VV_SYM : AMat = A : BMat = B
            Case CalcType.VH_VH_SYM : AMat = A : BMat = B
            Case CalcType.VV_VH_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_SYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)

            Case CalcType.VV_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VV_VV_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.VH_VH_ASYM : AMat = Repeat(A, B.rows, B.cols) : BMat = Repeat(B, A.rows, A.cols)

            Case CalcType.M_VH_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_VV_SYM : AMat = A : BMat = Repeat(B, A.rows, A.cols)
            Case CalcType.M_N : AMat = A : BMat = Repeat(New Mat(B), A.rows, A.cols)
            Case CalcType.M_M_SYM : AMat = A : BMat = B
            Case CalcType.M_M_ASYM
                Dim R As New List(Of Double())
                For Row As Integer = 0 To A.Height - 1
                    Dim CC(B.width - 1) As Double
                    For Col As Integer = 0 To B.Width - 1
                        Dim VecA As Mat = A.Row(Row) '       Zeile von Array A
                        Dim VecB As Mat = B.Col(Col).T '     Transponierte Spalte von Array B
                        Dim SumAB As Mat = VecA + VecB
                        Dim AddAB = Sum(SumAB)
                        VecA.Dispose()
                        VecB.Dispose()
                        CC(Col) = AddAB
                    Next
                    R.Add(CC)
                Next
                Return New Mat(R.ToArray)
            Case Else
                Return Nothing
        End Select
        Emgu.CV.CvInvoke.Add(AMat.OrgMat, BMat.OrgMat, Ret.OrgMat, , AMat.Depth)
        Return Ret
    End Function
    Shared Function Sum(ByVal A As Mat) As Double
        Dim Ret As New Emgu.CV.Structure.MCvScalar : Ret = Emgu.CV.CvInvoke.Sum(A.OrgMat) : Return Ret.V0
    End Function
#End Region

    <DebuggerStepThrough()>
    Shared Function Repeat(ByVal A As Mat, ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Dim Ret As New Mat
        Dim Types = New MatrixExplain(A, Nothing)
        If Types.MatTypA = MatrixExplain.DataType.Number Then Emgu.CV.CvInvoke.Repeat(A.OrgMat, Rows, Cols, Ret.OrgMat) : Return Ret
        If Types.MatTypA = MatrixExplain.DataType.VectorH Then Emgu.CV.CvInvoke.Repeat(A.OrgMat, Rows, 1, Ret.OrgMat) : Return Ret
        If Types.MatTypA = MatrixExplain.DataType.VectorV Then Emgu.CV.CvInvoke.Repeat(A.OrgMat, 1, Cols, Ret.OrgMat) : Return Ret
        Return Ret
    End Function
    Shared Function ConvertTo(ByVal Mat As Mat, ByVal Depth As Emgu.CV.CvEnum.DepthType)
        Dim RET As New Mat : Mat.ConvertTo(RET, Depth) : Return RET
    End Function
    Shared Function AsType(ByVal Mat As Mat, ByVal Type As DataType)
        Dim depth As Emgu.CV.CvEnum.DepthType
        Select Case Type
            Case DataType._Byte : depth = Emgu.CV.CvEnum.DepthType.Cv8U
            Case DataType._SByte : depth = Emgu.CV.CvEnum.DepthType.Cv8S
            Case DataType._Int16 : depth = Emgu.CV.CvEnum.DepthType.Cv16S
            Case DataType._UInt16 : depth = Emgu.CV.CvEnum.DepthType.Cv16U
            Case DataType._Int32 : depth = Emgu.CV.CvEnum.DepthType.Cv32S
            Case DataType._Single : depth = Emgu.CV.CvEnum.DepthType.Cv32F
            Case DataType._Double : depth = Emgu.CV.CvEnum.DepthType.Cv64F
        End Select
        Return ConvertTo(Mat, depth)
    End Function
    Shared Function Diag(ByVal A As Mat, Optional ByVal k As Integer = 0) As Mat
        Select Case A.Depth
            Case Emgu.CV.CvEnum.DepthType.Cv32F
                Dim Res As New List(Of Single())
                Dim Org = A.NP_GetData
                If k = 0 Then
                    For i As Integer = 0 To A.Height - 1
                        Dim L1(A.Height - 1) As Single
                        L1(i) = (Org(i)(0))
                        Res.Add(L1)
                    Next
                End If
                Return New Mat(Res.ToArray)
            Case Emgu.CV.CvEnum.DepthType.Cv16S
                Dim Res As New List(Of Int16())
                Dim Org = A.NP_GetData
                If k = 0 Then
                    For i As Integer = 0 To A.Height - 1
                        Dim L1(A.Height - 1) As Int16
                        Res(i) = (Org(i)(0))
                        Res.Add(L1)
                    Next
                End If
                Return New Mat(Res.ToArray)
            Case Emgu.CV.CvEnum.DepthType.Cv64F
                Dim Res As New List(Of Double())
                Dim Org = A.NP_GetData
                If k = 0 Then
                    For i As Integer = 0 To A.Height - 1
                        Dim L1(A.Height - 1) As Double
                        L1(i) = (Org(i)(0))
                        Res.Add(L1)
                    Next
                End If
                Return New Mat(Res.ToArray)
            Case Else
                Throw New Exception("Datatype not implemented for Diag (CV16S + CV64F implemented)")
        End Select
    End Function
    Shared Function Arange(ByVal Rows As Integer) As Mat
        Dim RET As New Mat
        For i As Integer = 0 To Rows
            RET.PopBack(i)
        Next
        Return RET
    End Function
    Shared Function Eye(ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Return Mat.Eye(Rows, Cols, Emgu.CV.CvEnum.DepthType.Default, 1)
    End Function
    Shared Function VStack(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret2 As New Mat(0, 0, Emgu.CV.CvEnum.DepthType.Cv64F, 1)
        If A.Width = B.Width And A.Height = B.Height And A.Width = 1 Then
            For Ai As Integer = 0 To A.Cols - 1
                Ret2.PushBack(A.Col(Ai).T)
            Next
            For Ai As Integer = 0 To B.Cols - 1
                Ret2.PushBack(B.Col(Ai).T)
            Next
            Return Ret2
        End If
        Emgu.CV.CvInvoke.VConcat(A.OrgMat, B.OrgMat, Ret2.OrgMat) : Return Ret2.Reshape(A.Width * 2)
    End Function
    Shared Function HStack(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.VConcat(A, B, Ret) : Return Ret
    End Function
    Shared Function IsReal(ByVal A As Mat) As Mat
        Return A
    End Function
    Shared Function real(ByVal A As Mat) As Mat
        Dim Ret As Double()() = A.NP_GetData
        For Row As Integer = 0 To A.Rows - 1
            For Col As Integer = 0 To A.Cols - 1
                Ret(Row)(Col) = Math.Truncate(Ret(Row)(Col))
            Next
        Next
        Return New Mat(Ret)
    End Function
    Shared Function Count_nonzero(ByVal A As Mat)
        Return Emgu.CV.CvInvoke.CountNonZero(A.OrgMat)
    End Function
    Shared Function Clip(ByVal A As Mat, ByVal ClipLower As Double, ByVal ClipUpper As Double) As Mat
        Dim Ret As Mat = A.Clone
        Dim Ones As Mat = Mat.Ones(A.Rows, A.Cols, A.Depth, A.Channels)
        Dim MaxC As New Mat : Emgu.CV.CvInvoke.Threshold(Ones, MaxC, 0, ClipUpper, Emgu.CV.CvEnum.ThresholdType.Binary)
        Dim MinC As New Mat : Emgu.CV.CvInvoke.Threshold(Ones, MinC, 0, ClipLower, Emgu.CV.CvEnum.ThresholdType.Binary)
        Emgu.CV.CvInvoke.Min(Ret, MaxC, Ret)
        Emgu.CV.CvInvoke.Max(Ret, MinC, Ret)
        Return Ret
    End Function
    Shared Function T(ByVal A As Mat) As Mat
        Return A.T
    End Function
    Private Shared Function SetMaxValue(ByVal A As Mat, ByVal Value As Integer) As Mat
        Dim Ret As New Mat
        Dim Ones As Mat = Mat.Ones(A.Rows, A.Cols, A.Depth, A.Channels)
        Dim Values As New Mat : Emgu.CV.CvInvoke.Threshold(Ones, Values, 0, Value, Emgu.CV.CvEnum.ThresholdType.Binary)
        Emgu.CV.CvInvoke.Min(A, Values, Ret) : Return Ret
    End Function
    Private Shared Function SetMinValue(ByVal A As Mat, ByVal Value As Integer) As Mat
        Dim Ret As New Mat
        Dim Values As New Mat(A.Rows, A.Cols, A.Depth, A.Channels)
        Dim Ones As Mat = Mat.Ones(A.Rows, A.Cols, A.Depth, A.Channels)
        Emgu.CV.CvInvoke.Threshold(Ones, Values, 0, Value, Emgu.CV.CvEnum.ThresholdType.Binary)
        Emgu.CV.CvInvoke.Max(A, Values, Ret) : Return Ret
    End Function

    Shared Function Remap(ByVal Mat As Mat) As Mat
        Mat.Reshape(3)
    End Function



#Region "Classes (Multi-Return)"
    Class Eigen
        Property Values As New Mat
        Property Vectors As New Mat
        Sub New(ByVal A As Mat)
            Emgu.CV.CvInvoke.Eigen(A.OrgMat, Values.OrgMat, Vectors.OrgMat)
        End Sub
    End Class
    Class Interp2d
        Property X As Mat
        Property Y As Mat
        Property Z As Mat
        Property Kind As Emgu.CV.CvEnum.Inter
        Public Function Run(ByVal widthgridHR, ByVal heightgridHR)
            Dim upscaledLR As New Mat(heightgridHR.Height, widthgridHR.Height, Emgu.CV.CvEnum.DepthType.Cv64F, 1)
            'Emgu.CV.CvInvoke.Remap(Z.OrgMat, upscaledLR.OrgMat, Y.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv32S).OrgMat, X.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv32S).OrgMat, Kind)
            Emgu.CV.CvInvoke.Resize(Z.OrgMat, upscaledLR.OrgMat, New System.Drawing.Size(widthgridHR.t.width, heightgridHR.Height), , , Kind)
            Return upscaledLR
        End Function
        Sub New(ByVal _X As Mat, ByVal _Y As Mat, ByVal _Z As Mat, ByVal _Kind As Emgu.CV.CvEnum.Inter)
            X = _X
            Y = _Y
            Z = _Z
            Kind = _Kind
        End Sub
    End Class
    Class Gradient
        Property X As New Mat()
        Property Y As New Mat()
        Sub New(ByVal A As Mat)
            Emgu.CV.CvInvoke.SpatialGradient(A.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv8U).OrgMat, X.OrgMat, Y.OrgMat)
            X = X.NP_AsType(A.Depth)
            Y = Y.NP_AsType(A.Depth)
        End Sub
    End Class
    Class Linalg
        Sub New()

        End Sub
        Public Shared Function matrix_power() As Mat

        End Function
        Public Shared Function Det(ByVal A As Object) As Integer

        End Function
        Public Shared Function Inv(ByVal A As Double) As Mat

        End Function
    End Class
    Class Transform
        Shared Function Resize(ByVal Mat As Mat, ByVal Size As Drawing.Size, ByVal Mode As Emgu.CV.CvEnum.BorderType, ByVal AntiAlias As Boolean)
            Dim RET As New Mat
            Dim TF = Emgu.CV.CvInvoke.GetAffineTransform(
                                             {New Drawing.PointF(0, 0),
                                              New Drawing.PointF(Mat.Width - 1, 0),
                                              New Drawing.PointF(0, Mat.Height - 1)},
 _
                                             {New Drawing.PointF(0, 0),
                                              New Drawing.PointF(Size.Width, 0),
                                              New Drawing.PointF(0, Size.Height)})

            ' In OpenCV theres no way to Diable Interpolation. Using Nearest Neigbor as alternative
            'Emgu.CV.CvInvoke.Resize(Mat, RET, Size, , , Emgu.CV.CvEnum.Inter.Nearest)
            'Emgu.CV.CvInvoke.ResizeForFrame(Mat.OrgMat, RET.OrgMat, Size, Emgu.CV.CvEnum.Inter.Nearest, True)
            Emgu.CV.CvInvoke.WarpAffine(Mat.OrgMat, RET.OrgMat, TF, Size, Emgu.CV.CvEnum.Inter.Nearest, Emgu.CV.CvEnum.Warp.Default, Mode)
            Return RET
        End Function
    End Class
#End Region

    Shared Sub Debug(ByVal A As Mat)
        If IsNothing(A) Then Exit Sub
        Dim Test = NPSharp.NPPublic.GetData(A)
        Dim MaxB As New Mat
        If A.Width < 100 And A.Height < 100 Then
            Emgu.CV.CvInvoke.Resize(A, MaxB, New Drawing.Size(A.Height * 30, A.Width * 30), , , Emgu.CV.CvEnum.Inter.Nearest)
        Else
            MaxB = A
        End If
        Emgu.CV.CvInvoke.Imshow("Test", MaxB)
        Emgu.CV.CvInvoke.WaitKey(3000)
    End Sub


#Region "OGrid"
    ''' <summary>
    ''' >>> from numpy import ogrid
    '''>>> ogrid[-1:1:5j]
    '''array([-1. , -0.5,  0. ,  0.5,  1. ])
    '''>>> ogrid[0:5,0:5]
    '''[
    ''' array([[0],
    '''        [1],
    '''        [2],
    '''        [3],
    '''        [4]]), 
    ''' array([[0, 1, 2, 3, 4]])]
    ''' </summary>
    ''' <param name="_From"></param>
    ''' <param name="_To"></param>
    ''' <param name="Count"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function OGrid(ByVal _From As Double, ByVal _To As Double, ByVal Count As Integer) As Double()
        Dim Min As Double = Math.Min(_From, _To)
        Dim Max As Double = Math.Max(_From, _To)
        Dim Sum As Double = (Max - Min)

        If Count = 0 Then Count = Sum
        Dim Steps As Double = Sum / Count
        Dim RET(Count) As Double

        Dim I As Integer = 0
        For Val As Double = Min To Max Step Steps
            RET(I) = Val : I += 1
        Next

        Return RET
    End Function
    Shared Function OGrid(ByVal _From As Double, ByVal _To As Double) As Tuple(Of Double(), Double()())
        Dim RET As Double() = OGrid(_From, _To, 0)
        Return New Tuple(Of Double(), Double()())(RET, {RET})
    End Function
    Shared Function OGrid(ByVal A1 As Double(), ByVal B1 As Double()) As Tuple(Of Double(), Double()())
        Dim RETA As Double() = OGrid(A1(0), A1(1) - 1, 0)
        Dim RETB As Double() = OGrid(B1(0), B1(1) - 1, 0)
        Return New Tuple(Of Double(), Double()())(RETA, {RETB})
    End Function
#End Region


End Class