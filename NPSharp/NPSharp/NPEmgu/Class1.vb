Imports System.Runtime.InteropServices
Imports NPSharp.NPPublic
Imports NPSharp.NPPublic.MatrixExplain
Imports System.Drawing


Public Class NPEmgu

    Enum NP_DataType
        Cv8U_Byte = Emgu.CV.CvEnum.DepthType.Cv8U
        Cv8S_SByte = Emgu.CV.CvEnum.DepthType.Cv8S
        Cv16S_Int16 = Emgu.CV.CvEnum.DepthType.Cv16S
        Cv16U_UInt16 = Emgu.CV.CvEnum.DepthType.Cv16U
        Cv32S_Int32 = Emgu.CV.CvEnum.DepthType.Cv32S
        Cv32F_Single = Emgu.CV.CvEnum.DepthType.Cv32F
        Cv64F_Double = Emgu.CV.CvEnum.DepthType.Cv64F
    End Enum

    Class Matrix(Of T)
        Sub New(ByVal Array As T())

        End Sub
    End Class

    Class Mat
        Property OrgMat As Emgu.CV.Mat

#Region "Operators"
        Public Shared Operator *(ByVal Mat1 As Mat, ByVal Num As Double) As Mat
            Return Multiply(Mat1, Num)
        End Operator
        Public Shared Operator *(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Multiply(Mat1, Mat2)
        End Operator

        Public Shared Operator +(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Add(Mat1, Mat2)
        End Operator
        Public Shared Operator -(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Substract(Mat1, Mat2)
        End Operator

        Public Shared Operator /(ByVal Mat1 As Mat, ByVal Num As Double) As Mat
            Return Divide(Mat1, Num)
        End Operator
        Public Shared Operator /(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Divide(Mat1, Mat2)
        End Operator
#End Region

#Region "Mat Props"
        ReadOnly Property Bitmap As Bitmap
            Get
                Return OrgMat.Bitmap
            End Get
        End Property
        ReadOnly Property Data
            Get
                Return OrgMat.Data
            End Get
        End Property
        ReadOnly Property DataPointer As IntPtr
            Get
                Return OrgMat.DataPointer
            End Get
        End Property
        ReadOnly Property Cols As Integer
            Get
                Return OrgMat.Cols
            End Get
        End Property
        ReadOnly Property Rows As Integer
            Get
                Return OrgMat.Rows
            End Get
        End Property
        ReadOnly Property Depth As Emgu.CV.CvEnum.DepthType
            Get
                Return OrgMat.Depth
            End Get
        End Property
        ReadOnly Property DepthS As NP_DataType
            Get
                Select Case Depth

                    Case Emgu.CV.CvEnum.DepthType.Cv64F : Return NP_DataType.Cv64F_Double
                End Select
            End Get
        End Property
        ReadOnly Property Height As Integer
            Get
                Return OrgMat.Height
            End Get
        End Property
        ReadOnly Property Width As Integer
            Get
                Return OrgMat.Width
            End Get
        End Property
        ReadOnly Property Channels As Integer
            Get
                Return OrgMat.NumberOfChannels
            End Get
        End Property
#End Region

#Region "Constructor"
        Sub New()
            OrgMat = New Emgu.CV.Mat
        End Sub
        Sub New(File As String)
            OrgMat = New Emgu.CV.Mat(File)
        End Sub
        Sub New(ByVal Mat As Emgu.CV.Mat)
            OrgMat = Mat
        End Sub
        Sub New(ByVal Mat As Mat, ByVal Rect As System.Drawing.Rectangle)
            OrgMat = New Emgu.CV.Mat(Mat.OrgMat, Rect)
        End Sub
        Sub New(ByVal Rows As Integer, ByVal Cols As Integer, ByVal Depth As Emgu.CV.CvEnum.DepthType, ByVal Channels As Integer)
            OrgMat = New Emgu.CV.Mat(Rows, Cols, Depth, Channels)
        End Sub
        Sub New(ByVal Array As Object)
            Dim Desc = New MatrixExplain(Array)
            Select Case Desc.MatTypA
                Case MatrixExplain.DataType.Number : OrgMat = New Emgu.CV.Mat(1, 1, Desc.NumTypeA, 1)
                Case MatrixExplain.DataType.Matrix : OrgMat = New Emgu.CV.Mat(UBound(Array) + 1, UBound(Array(0)) + 1, Desc.NumTypeA, 1)
                Case MatrixExplain.DataType.VectorH : OrgMat = New Emgu.CV.Mat(1, UBound(Array(0)) + 1, Desc.NumTypeA, 1)
                Case MatrixExplain.DataType.VectorV : OrgMat = New Emgu.CV.Mat(UBound(Array) + 1, 1, Desc.NumTypeA, 1)
            End Select
            SetData(Me, Array)
        End Sub
#End Region

        Public Overrides Function ToString() As String
            Return "H: " & Height & ", W:" & Width & " D:" & DepthS.ToString
        End Function

#Region "MY Extensions"
        ReadOnly Property ShowTable() As String
            Get
                Return NPPublic.Show(Me.NP_GetData)
            End Get
        End Property

#End Region

#Region "NP Extensions"
        ReadOnly Property NP_GetData()
            Get
                Return NPPublic.GetData(Me)
            End Get
        End Property
        ReadOnly Property NP_Max As Double
            Get
                Dim Max() As Double
                OrgMat.MinMax(Nothing, Max, Nothing, Nothing)
                Return Max(0)
            End Get
        End Property
        ReadOnly Property NP_Min As Double
            Get
                Dim Min() As Double
                OrgMat.MinMax(Min, Nothing, Nothing, Nothing)
                Return Min(0)
            End Get
        End Property
        Function NP_Ravel() As Mat
            Return New Mat(OrgMat.Reshape(1, 1))
        End Function
        Function NP_VStack(ByVal Mat2 As Mat) As Mat
            Return VStack(Me, Mat2)
        End Function
        Function NP_HStack(ByVal Mat2 As Mat) As Mat
            Return VStack(Me, Mat2)
        End Function
        Function NP_AsType(ByVal DataType As Emgu.CV.CvEnum.DepthType) As Mat
            Dim RET As New Mat
            OrgMat.ConvertTo(RET.OrgMat, DataType)
            Return RET
        End Function

        Shared Function NP_linspace(ByVal Start As Double, ByVal Ends As Double, ByVal Cnt As Integer) As Mat
            Dim StepWidth As Double = (Ends - Start) / (Cnt - 1)
            Dim Ret As New List(Of Double)
            For i As Double = Start To Ends Step StepWidth
                Ret.Add(Math.Round(i, 1))
            Next
            Return New Mat(Ret.ToArray)
        End Function
#Region "Dot"
        Function NP_dot(ByVal B As Mat, Optional ByVal NonZero As Boolean = True) As Mat
            If NonZero = True Then
                Dim RES As New Mat
                Dim AMin As Mat = ZerosToOne(Me) ' NumPy replaces Zeros with Ones
                Dim BMin As Mat = ZerosToOne(B) ' NumPy replaces Zeros with Ones
                Emgu.CV.CvInvoke.Multiply(AMin, BMin, RES)
            Else
                Return Me * B
            End If
            Return Nothing
        End Function
        Private Function ZerosToOne(ByVal A As Mat) As Mat
            Dim RES As New Mat(A.Rows, A.Cols, A.Depth, A.Channels)
            Dim ONES As Mat = Mat.Ones(A.Rows, A.Cols, A.Depth, A.Channels)
            Emgu.CV.CvInvoke.Max(A, ONES, RES)
            Return New Mat(RES.OrgMat)
        End Function
#End Region


#End Region

#Region "PY Extensions"
        Function PY_Slice(ByVal Start As String, ByVal Stops As String, ByVal Steps As String) As Mat
            Return Python.Slice(Me, Start, Stops, Steps)
        End Function
#End Region

#Region "SP Extensions"

#End Region

        Sub PushBack(ByVal Mat As Mat)
            OrgMat.PushBack(Mat.OrgMat)
        End Sub
        Sub PopBack(ByVal Mat As Object)
            OrgMat.PopBack(Mat.OrgMat)
        End Sub
        <DebuggerStepThrough()>
        Function Row(ByVal Index As Integer) As Mat
            Return New Mat(OrgMat.Row(Index))
        End Function
        Function Col(ByVal Index As Integer) As Mat
            Return New Mat(OrgMat.Col(Index))
        End Function
        Function T()
            Return New Mat(OrgMat.T)
        End Function

        Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer,
                              Optional ByVal DataType As Emgu.CV.CvEnum.DepthType = Emgu.CV.CvEnum.DepthType.Cv32S,
                              Optional ByVal Channels As Integer = 1)
            Return New Mat(Emgu.CV.Mat.Zeros(Rows, Cols, DataType, Channels))
        End Function
        Shared Function Ones()

        End Function
        Shared Function Eye()

        End Function
        Function Exp() As Mat
            Dim Ret As New Mat : Emgu.CV.CvInvoke.Exp(OrgMat, Ret.OrgMat) : Return Ret
        End Function
        Function Dot(ByVal B As Mat) As Double
            Return OrgMat.Dot(B)
        End Function
        Function ConvertTo(ByVal A As Object, ByVal B As Object)

        End Function
        Function Clone() As Mat
            Return New Mat(OrgMat.Clone)
        End Function
        Function Reshape(ByVal A As Integer)

        End Function
        Function Reshape(ByVal A As Integer, ByVal B As Integer)

        End Function
        Function CvtColor(ByVal Conversation As Emgu.CV.CvEnum.ColorConversion) As Mat
            Dim RES As New Mat
            Emgu.CV.CvInvoke.CvtColor(OrgMat, RES.OrgMat, Conversation)
            Return RES
        End Function
        Function Normalize(ByVal Min As Double, ByVal Max As Double, ByVal Norm As Emgu.CV.CvEnum.NormType)
            Dim RET As New Mat
            Emgu.CV.CvInvoke.Normalize(OrgMat, RET.OrgMat, , , Norm)
            Return RET
        End Function
        Sub ShowCV()
            Dim MaxB As New Emgu.CV.Mat
            If OrgMat.Width < 100 And OrgMat.Height < 100 Then
                Emgu.CV.CvInvoke.Resize(OrgMat, MaxB, New Drawing.Size(OrgMat.Height * 30, OrgMat.Width * 30), , , Emgu.CV.CvEnum.Inter.Nearest)
            Else
                MaxB = OrgMat
            End If

            Emgu.CV.CvInvoke.Imshow("Test", MaxB)
            Emgu.CV.CvInvoke.WaitKey(3000)
            MaxB.Dispose()
        End Sub
#Region "ArgSort"
        Public Function NP_ArgSort() As Mat
            Dim M As Double()() = NPNative.Zeros(Rows, Cols)
            For i As Integer = 0 To Rows - 1
                Dim ORG As Object = Me.Row(i).NP_GetData(0)
                Dim DIF As Object = SortAsc(Row(i)).NP_GetData(0)
                Dim IDXP As Integer = 0
                For i2 As Integer = 0 To UBound(ORG)
                    Dim IDX As Integer = Array.IndexOf(DIF, ORG(i2))
                    If IDXP = IDX And IDX > 0 Then IDX += 1
                    M(i)(i2) = IDX : IDXP = IDX
                Next
            Next
            Return Python.Slice(ArrayToMat(M), ":", ":", "-1")
        End Function
        Private Shared Function SortRows(ByVal Mat As Mat) As Mat
            Dim Ret As New Mat
            Emgu.CV.CvInvoke.Sort(Mat, Ret, Emgu.CV.CvEnum.SortFlags.SortEveryRow)
            Return Ret
        End Function
        Private Shared Function SortCols(ByVal Mat As Mat) As Mat
            Dim Ret As New Mat
            Emgu.CV.CvInvoke.Sort(Mat, Ret, Emgu.CV.CvEnum.SortFlags.SortEveryColumn)
            Return Ret
        End Function
        Private Shared Function SortAsc(ByVal Mat As Mat) As Mat
            Dim Ret As New Mat
            Emgu.CV.CvInvoke.Sort(Mat.OrgMat, Ret.OrgMat, Emgu.CV.CvEnum.SortFlags.SortAscending)
            Return Ret
        End Function
        Private Shared Function SortDesc(ByVal Mat As Mat) As Mat
            Dim Ret As New Mat
            Emgu.CV.CvInvoke.Sort(Mat, Ret, Emgu.CV.CvEnum.SortFlags.SortDescending)
            Return Ret
        End Function
#End Region

    End Class

#Region "Ones"
    Shared Function Ones(ByVal Cols As Integer) As Mat
        Return Mat.Ones(1, Cols, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Ones(ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Return Mat.Ones(Rows, Cols, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Ones(ByVal Rows As Integer, ByVal Cols As Integer, ByVal Depth As Integer) As Mat
        Return Mat.Ones(Rows, Cols, Emgu.CV.CvEnum.DepthType.Cv32S, Depth)
    End Function
#End Region
#Region "Zeros"
    Shared Function Zeros(ByVal Cols As Integer) As Mat
        Return Mat.Zeros(1, Cols, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer) As Mat
        Return Mat.Zeros(Rows, Cols, Emgu.CV.CvEnum.DepthType.Cv32S, 1)
    End Function
    Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer, ByVal Depth As Integer) As Mat
        Return Mat.Zeros(Rows, Cols, Emgu.CV.CvEnum.DepthType.Cv32S, Depth)
    End Function
#End Region

#Region "Main-Matrix-Math"
    Shared Function Multiply(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim AMat As Mat = CType(A, Mat)
        Dim Ret As New Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : Emgu.CV.CvInvoke.Multiply(A.OrgMat, B.OrgMat, Ret.OrgMat) : Return Ret
            Case CalcType.VV_VV_SYM : Emgu.CV.CvInvoke.Multiply(A.OrgMat, B.OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_SYM : Emgu.CV.CvInvoke.Multiply(A.orgmat, B.orgmat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_SYM : Emgu.CV.CvInvoke.Multiply(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_SYM : Emgu.CV.CvInvoke.Multiply(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_ASYM : Emgu.CV.CvInvoke.Multiply(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_ASYM : Emgu.CV.CvInvoke.Multiply(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VV_ASYM : Emgu.CV.CvInvoke.Multiply(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_ASYM : Emgu.CV.CvInvoke.Multiply(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VH_SYM : Emgu.CV.CvInvoke.Multiply(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VV_SYM : Emgu.CV.CvInvoke.Multiply(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_N : Emgu.CV.CvInvoke.Multiply(A.OrgMat, New Mat(B).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_M_SYM : Emgu.CV.CvInvoke.Multiply(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.M_M_ASYM
                Dim R As New List(Of Double())
                For Row As Integer = 0 To A.Height - 1
                    Dim C(B.Width - 1) As Double
                    For Col As Integer = 0 To B.Width - 1
                        C(Col) = A.Row(Row).NP_dot(B.Col(Col).T, False)
                    Next
                    R.Add(C)
                Next
                Return ArrayToMat(R.ToArray)
            Case Else
                Return Nothing
        End Select
        Return Nothing
    End Function
    Shared Function Substract(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim AMat As Mat = CType(A, Mat)
        Dim Ret As New Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : Emgu.CV.CvInvoke.Subtract(A, B, Ret) : Return Ret
            Case CalcType.VV_VV_SYM : Emgu.CV.CvInvoke.Subtract(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_SYM : Emgu.CV.CvInvoke.Subtract(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_SYM : Emgu.CV.CvInvoke.Subtract(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_SYM : Emgu.CV.CvInvoke.Subtract(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_ASYM : Emgu.CV.CvInvoke.Subtract(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_ASYM : Emgu.CV.CvInvoke.Subtract(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VV_ASYM : Emgu.CV.CvInvoke.Subtract(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_ASYM : Emgu.CV.CvInvoke.Subtract(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VH_SYM : Emgu.CV.CvInvoke.Subtract(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VV_SYM : Emgu.CV.CvInvoke.Subtract(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_N : Emgu.CV.CvInvoke.Subtract(A.OrgMat, New Mat(B).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_M_SYM : Emgu.CV.CvInvoke.Subtract(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.M_M_ASYM
            Case Else : Return Nothing
        End Select
        Return Nothing
    End Function
    Shared Function Divide(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim AMat As Mat = CType(A, Mat)
        Dim Ret As New Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : Emgu.CV.CvInvoke.Divide(A, B, Ret) : Return Ret
            Case CalcType.VV_VV_SYM : Emgu.CV.CvInvoke.Divide(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_SYM : Emgu.CV.CvInvoke.Divide(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_SYM : Emgu.CV.CvInvoke.Divide(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_SYM : Emgu.CV.CvInvoke.Divide(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_ASYM : Emgu.CV.CvInvoke.Divide(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_ASYM : Emgu.CV.CvInvoke.Divide(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VV_ASYM : Emgu.CV.CvInvoke.Divide(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_ASYM : Emgu.CV.CvInvoke.Divide(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VH_SYM : Emgu.CV.CvInvoke.Divide(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VV_SYM : Emgu.CV.CvInvoke.Divide(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_N : Emgu.CV.CvInvoke.Divide(A.OrgMat, New Mat(B).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_M_SYM : Emgu.CV.CvInvoke.Divide(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.M_M_ASYM
            Case Else : Return Nothing
        End Select
        Return Nothing
    End Function
    Shared Function Add(ByVal A As Object, ByVal B As Object) As Mat
        Dim MatDsc = New NPSharp.NPPublic.MatrixExplain(A, B)
        Dim AMat As Mat = CType(A, Mat)
        Dim Ret As New Mat
        Select Case MatDsc.CalcTyp
            Case CalcType.N_N : Emgu.CV.CvInvoke.Add(A, B, Ret) : Return Ret
            Case CalcType.VV_VV_SYM : Emgu.CV.CvInvoke.Add(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_SYM : Emgu.CV.CvInvoke.Add(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_SYM : Emgu.CV.CvInvoke.Add(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_SYM : Emgu.CV.CvInvoke.Add(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VH_ASYM : Emgu.CV.CvInvoke.Add(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VV_ASYM : Emgu.CV.CvInvoke.Add(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VV_VV_ASYM : Emgu.CV.CvInvoke.Add(ExtendVector(A, B.Width).OrgMat, ExtendVector(B, A.Width).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.VH_VH_ASYM : Emgu.CV.CvInvoke.Add(ExtendVector(A, B.Height).OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VH_SYM : Emgu.CV.CvInvoke.Add(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_VV_SYM : Emgu.CV.CvInvoke.Add(AMat.OrgMat, ExtendVector(B, A.Height).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_N : Emgu.CV.CvInvoke.Add(A.OrgMat, New Mat(B).OrgMat, Ret.OrgMat, , AMat.Depth) : Return Ret
            Case CalcType.M_M_SYM : Emgu.CV.CvInvoke.Add(A, B, Ret, , AMat.Depth) : Return Ret
            Case CalcType.M_M_ASYM
            Case Else : Return Nothing
        End Select
        Return Nothing
    End Function
    Shared Function Sum(ByVal A As Mat) As Integer
        Dim Ret As New Emgu.CV.Structure.MCvScalar : Ret = Emgu.CV.CvInvoke.Sum(A.OrgMat) : Return Ret.V0
    End Function

#End Region


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
            Case DataType._Float : depth = Emgu.CV.CvEnum.DepthType.Cv32F
            Case DataType._Single : Throw New Exception("???")
            Case DataType._Double : depth = Emgu.CV.CvEnum.DepthType.Cv64F
        End Select
        Return ConvertTo(Mat, depth)
    End Function

    Shared Function Diag(ByVal A As Mat, Optional ByVal D As Integer = 0) As Mat
        Select Case A.Depth
            Case Emgu.CV.CvEnum.DepthType.Cv16S
                Dim ADat As Int16()() = GetData(A)
                Dim RESA As Int16()() = NPNative.Diag(GetData(A.Row(0))(0))
                Dim RES As Mat = MatFromArray(RESA) : Return RES
            Case Emgu.CV.CvEnum.DepthType.Cv64F
                Dim ADat As Double()() = GetData(A)
                Dim RESA As Double()() = NPNative.Diag(GetData(A.Row(0))(0))
                Dim RES As Mat = MatFromArray(RESA) : Return RES
            Case Else

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
        Dim Ret As New Mat : Emgu.CV.CvInvoke.VConcat(A.OrgMat, B.OrgMat, Ret.OrgMat) : Return Ret
    End Function
    Shared Function HStack(ByVal A As Mat, ByVal B As Mat) As Mat
        Dim Ret As New Mat : Emgu.CV.CvInvoke.HConcat(A, B, Ret) : Return Ret
    End Function

    Shared Function IsReal(ByVal A As Mat) As Mat
        Return A
    End Function
    Shared Function real(ByVal A As Mat) As Mat
        Return A
    End Function

    Shared Function Remap(ByVal Mat As Mat) As Mat
        Mat.Reshape(3)
    End Function
    Shared Function Count_nonzero(ByVal A As Mat)
        Return Emgu.CV.CvInvoke.CountNonZero(A)
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
    Private Shared Function ExtendVector(ByVal A As Mat, ByVal TargetSize As Integer) As Mat
        Dim Ret As New Mat
        Dim Types = New MatrixExplain(A, Nothing)
        If Types.MatTypA = MatrixExplain.DataType.Number Then
            Ret.PopBack(A)
        End If
        If Types.MatTypA = MatrixExplain.DataType.VectorH Then
            For i As Integer = 0 To TargetSize - 1
                Ret.PushBack(A)
            Next
            Return Ret
        End If
        If Types.MatTypA = MatrixExplain.DataType.VectorV Then
            Ret = ExtendVector(A.T, TargetSize).T
            Return Ret
        End If
        Return Ret
    End Function


#Region "Classes (Multi-Return)"
    Class Eigen
        Property Values As New Mat
        Property Vectors As New Mat
        Property ValuesD As Object ' Can be any numeric type
        Property VectorsD As Object ' Can be any numeric type
        Sub New(ByVal A As Mat)
            Emgu.CV.CvInvoke.Eigen(A, Values, Vectors)
            ValuesD = GetData(Values)
            VectorsD = GetData(Vectors)
        End Sub
    End Class
    Class Interp2d
        Property X As Mat
        Property Y As Mat
        Property Z As Mat
        Property Kind As Emgu.CV.CvEnum.Inter
        Public Function Run(ByVal widthgridHR, ByVal heightgridHR)
            ' Emgu.CV.CvInvoke.Remap(grayorigin, bilinearinterp, widthgridLR, widthgridLR, CvEnum.Inter.Linear)
            Dim upscaledLR As New Mat
            Emgu.CV.CvInvoke.Resize(Z.OrgMat, upscaledLR.OrgMat, New System.Drawing.Size(widthgridHR.width, heightgridHR.Height), , , Emgu.CV.CvEnum.Inter.Linear)
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
        Property X As New Mat
        Property Y As New Mat
        Property XD As Object
        Property YD As Object
        Sub New(ByVal A As Mat)
            Emgu.CV.CvInvoke.SpatialGradient(A.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv8U).OrgMat, X.OrgMat, Y.OrgMat)
            XD = GetData(X)
            YD = GetData(Y)
        End Sub
    End Class
    Class Linalg
        Sub New()

        End Sub
        Public Shared Function Det(ByVal A As Object) As Integer

        End Function
        Public Shared Function Inv(ByVal A As Double) As Mat

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

End Class