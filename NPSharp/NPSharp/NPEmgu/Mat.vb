Imports System.Drawing
Imports NPSharp.NPPublic
Imports System.Runtime.InteropServices

Partial Public Class NPEmgu
    Class Mat
        Property OrgMat As Emgu.CV.Mat
        Property Cache As New List(Of Object)

        Public Overrides Function ToString() As String
            Return "H: " & Height & ", W:" & Width & " D:" & DepthS.ToString & " : " & ArrayToString(Me)
        End Function

        <DebuggerStepThrough()>
        Sub Dispose()
            OrgMat.Dispose()
            OrgMat = Nothing
        End Sub
#Region "Operators"
        Public Shared Operator *(ByVal Mat1 As Mat, ByVal Num As Double) As Mat
            Return Multiply(Mat1, Num)
        End Operator
        Public Shared Operator *(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Multiply(Mat1, Mat2)
        End Operator
        Public Shared Operator /(ByVal Mat1 As Mat, ByVal Num As Double) As Mat
            Return Divide(Mat1, Num)
        End Operator
        Public Shared Operator /(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Divide(Mat1, Mat2)
        End Operator
        Public Shared Operator +(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Add(Mat1, Mat2)
        End Operator
        Public Shared Operator -(ByVal Mat1 As Mat, ByVal Mat2 As Mat) As Mat
            Return Substract(Mat1, Mat2)
        End Operator
#End Region

#Region "Mat Props"
        ReadOnly Property Shape As Integer()
            Get
                Return {Height, Width}
            End Get
        End Property
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
                    Case Emgu.CV.CvEnum.DepthType.Cv32S : Return NP_DataType.Cv32S_Int32
                    Case Emgu.CV.CvEnum.DepthType.Cv32F : Return NP_DataType.Cv32F_Single
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
        <DebuggerStepThrough()>
        Sub New()
            OrgMat = New Emgu.CV.Mat()
        End Sub
        <DebuggerStepThrough()>
        Sub New(ByVal File As String)
            OrgMat = New Emgu.CV.Mat(File)
        End Sub
        <DebuggerStepThrough()>
        Sub New(ByVal Mat As Emgu.CV.Mat)
            OrgMat = Mat
        End Sub
        <DebuggerStepThrough()>
        Sub New(ByVal Mat As Mat, ByVal Rect As System.Drawing.Rectangle)
            OrgMat = New Emgu.CV.Mat(Mat.OrgMat, Rect)
        End Sub
        <DebuggerStepThrough()>
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

#Region "CV Private"
        <DebuggerStepThrough()>
        Function Repeat(ByVal Cols, ByVal Rows)
            Return NPSharp.NPEmgu.Repeat(Me, Rows, Cols)
        End Function
        <DebuggerStepThrough()>
        Sub PushBack(ByVal Mat As Mat)
            OrgMat.PushBack(Mat.OrgMat)
        End Sub
        <DebuggerStepThrough()>
        Sub PopBack(ByVal Mat As Object)
            OrgMat.PopBack(Mat.OrgMat)
        End Sub
        <DebuggerStepThrough()>
        Function Row(ByVal Index As Integer) As Mat
            Return New Mat(OrgMat.Row(Index))
        End Function
        <DebuggerStepThrough()>
        Function Col(ByVal Index As Integer) As Mat
            Return New Mat(OrgMat.Col(Index))
        End Function
        <DebuggerStepThrough()>
        Function T() As Mat
            Return New Mat(OrgMat.T)
        End Function
        <DebuggerStepThrough()>
        Function Matrix(ByVal A As Mat) As Mat
            ' Converts to A-Matrix (But is already here)
            Return A
        End Function
        Function Exp() As Mat
            Dim Ret As New Mat : Emgu.CV.CvInvoke.Exp(OrgMat, Ret.OrgMat) : Return Ret
        End Function
        Function Dot(ByVal B As Mat, ByVal UseNPDot As Boolean) As Mat
            If UseNPDot = True Then
                Dim MX As New MatrixExplain(Me, B)
                Select Case MX.CalcTyp
                    Case MatrixExplain.CalcType.M_M_ASYM : Return Multiply_N_M(Me, B)
                    Case MatrixExplain.CalcType.M_M_SYM : Return Multiply_N_M(Me, B)
                    Case Else : Throw New Exception
                End Select
            Else
                Return Me * B
            End If
        End Function

        Enum EnumType
            Mat = 0
            Num = 1
            Arr = 2
        End Enum
        '<DebuggerStepThrough()>
        Private Shared Function RawType(ByVal A As Object) As EnumType
            Select Case A.GetType
                Case GetType(Mat) : Return EnumType.Mat

                Case GetType(Short) : Return EnumType.Num
                Case GetType(Integer) : Return EnumType.Num
                Case GetType(Single) : Return EnumType.Num
                Case GetType(Double) : Return EnumType.Num

                Case GetType(Short()) : Return EnumType.Arr
                Case GetType(Integer()) : Return EnumType.Arr
                Case GetType(Single()) : Return EnumType.Arr
                Case GetType(Double()) : Return EnumType.Arr

                Case GetType(Short()()) : Return EnumType.Arr
                Case GetType(Integer()()) : Return EnumType.Arr
                Case GetType(Single()()) : Return EnumType.Arr
                Case GetType(Double()()) : Return EnumType.Arr
            End Select
            Return Nothing
        End Function
        Shared Function ArrayToString(ByVal NumArray As Object) As String
            Dim S As New System.Text.StringBuilder
            Select Case RawType(NumArray)
                Case EnumType.Num : S.Append(NumArray)
                Case EnumType.Arr
                    S.Append("{")
                    For i As Integer = 0 To Math.Min(UBound(NumArray), 5)
                        S.Append(ArrayToString(NumArray(i)) & ";")
                        If (UBound(NumArray) > i) And i >= 5 Then S.Append("...;")
                    Next
                    S.Remove(S.Length - 1, 1)
                    S.AppendLine("}")
                Case EnumType.Mat
                    Try
                        Return ArrayToString(NumArray.NP_GetData)
                    Catch
                    End Try
            End Select
            Return S.ToString
        End Function

        Function ConvertTo(ByVal A As Object, ByVal B As Object)
            Return NPSharp.NPEmgu.ConvertTo(A, B)
        End Function
        Function Clone() As Mat
            Return New Mat(OrgMat.Clone)
        End Function
        Function Reshape(ByVal Cn As Integer, Optional ByVal Rows As Integer = 0) As Mat
            Return New Mat(OrgMat.Reshape(Cn, Rows))
        End Function

        Function CvtColor(ByVal Conversation As Emgu.CV.CvEnum.ColorConversion) As Mat
            Dim RET As New Mat : Emgu.CV.CvInvoke.CvtColor(OrgMat, RET.OrgMat, Conversation) : Return RET
        End Function
        Function Normalize(ByVal Min As Double, ByVal Max As Double, ByVal Norm As Emgu.CV.CvEnum.NormType)
            Dim RET As New Mat : Emgu.CV.CvInvoke.Normalize(OrgMat, RET.OrgMat, Min, Max, Norm) : Return RET
        End Function

#End Region
#Region "CV Shared"
        Shared Function Eye()
            Throw New Exception
        End Function

        Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer,
                        Optional ByVal DataType As Emgu.CV.CvEnum.DepthType = Emgu.CV.CvEnum.DepthType.Cv32S,
                        Optional ByVal Channels As Integer = 1)
            Return New Mat(Emgu.CV.Mat.Zeros(Rows, Cols, DataType, Channels))
        End Function
        Shared Function Ones(ByVal Rows As Integer, ByVal Cols As Integer,
                                Optional ByVal DataType As Emgu.CV.CvEnum.DepthType = Emgu.CV.CvEnum.DepthType.Cv32S,
                                Optional ByVal Channels As Integer = 1)
            Return New Mat(Emgu.CV.Mat.Ones(Rows, Cols, DataType, Channels))
        End Function

        Shared Function Zeros(ByVal A As Integer, ByVal B As Integer, ByVal C As Integer)
            Dim RET(A - 1)()() As Double
            For IA As Integer = 0 To A - 1
                Dim _B(B - 1)() As Double : RET(IA) = _B
                For IB = 0 To B - 1
                    Dim _C(C - 1) As Double : RET(IA)(IB) = _C
                    For IC = 0 To C - 1
                        Dim _D As Double : RET(IA)(IB)(IC) = _D
                    Next
                Next
            Next
            Return RET
        End Function
        Shared Function Zeros(ByVal A As Integer, ByVal B As Integer, ByVal C As Integer, ByVal D As Integer)
            Dim RET(A - 1)()()() As Double
            For IA As Integer = 0 To A - 1
                Dim _B(B - 1)()() As Double : RET(IA) = _B
                For IB = 0 To B - 1
                    Dim _C(C - 1)() As Double : RET(IA)(IB) = _C
                    For IC = 0 To C - 1
                        Dim _D(D - 1) As Double : RET(IA)(IB)(IC) = _D
                        For ID = 0 To D - 1
                            Dim _E As Double : RET(IA)(IB)(IC)(ID) = _E
                        Next
                    Next
                Next
            Next
            Return RET
        End Function
        Shared Function Zeros(ByVal A As Integer, ByVal B As Integer, ByVal C As Integer, ByVal D As Integer, ByVal E As Integer)
            Dim RET(A - 1)()()()() As Double
            For IA As Integer = 0 To A - 1
                Dim _B(B - 1)()()() As Double : RET(IA) = _B
                For IB = 0 To B - 1
                    Dim _C(C - 1)()() As Double : RET(IA)(IB) = _C
                    For IC = 0 To C - 1
                        Dim _D(D - 1)() As Double : RET(IA)(IB)(IC) = _D
                        For ID = 0 To D - 1
                            Dim _E(E - 1) As Double : RET(IA)(IB)(IC)(ID) = _E
                        Next
                    Next
                Next
            Next
            Return RET
        End Function
        Shared Function Zeros(ByVal A As Integer, ByVal B As Integer, ByVal C As Integer, ByVal D As Integer, ByVal E As Integer, ByVal F As Integer)
            Dim RET(A - 1)()()()()() As Double
            For IA As Integer = 0 To A - 1
                Dim _B(B - 1)()()()() As Double : RET(IA) = _B
                For IB = 0 To B - 1
                    Dim _C(C - 1)()()() As Double : RET(IA)(IB) = _C
                    For IC = 0 To C - 1
                        Dim _D(D - 1)()() As Double : RET(IA)(IB)(IC) = _D
                        For ID = 0 To D - 1
                            Dim _E(E - 1)() As Double : RET(IA)(IB)(IC)(ID) = _E
                            For IE = 0 To E - 1
                                Dim _F(F - 1) As Double : RET(IA)(IB)(IC)(ID)(IE) = _F
                                For FI = 0 To F - 1
                                    Dim _G As Double : RET(IA)(IB)(IC)(ID)(IE)(FI) = _G
                                Next
                            Next
                        Next
                    Next
                Next
            Next
            Return RET
        End Function
        Shared Function Zeros(ByVal A As Integer, ByVal B As Integer, ByVal C As Integer, ByVal D As Integer, ByVal E As Integer, ByVal F As Integer, ByVal G As Integer)
            Dim RET(A - 1)()()()()()() As Double
            For IA As Integer = 0 To A - 1
                Dim _B(B - 1)()()()()() As Double : RET(IA) = _B
                For IB = 0 To B - 1
                    Dim _C(C - 1)()()()() As Double : RET(IA)(IB) = _C
                    For IC = 0 To C - 1
                        Dim _D(D - 1)()()() As Double : RET(IA)(IB)(IC) = _D
                        For ID = 0 To D - 1
                            Dim _E(E - 1)()() As Double : RET(IA)(IB)(IC)(ID) = _E
                            For IE = 0 To E - 1
                                Dim _F(F - 1)() As Double : RET(IA)(IB)(IC)(ID)(IE) = _F
                                For FI = 0 To F - 1
                                    Dim _G As Double() : RET(IA)(IB)(IC)(ID)(IE)(FI) = _G
                                Next
                            Next
                        Next
                    Next
                Next
            Next
            Return RET
        End Function

#End Region

#Region "MY Extensions"
        ReadOnly Property ShowTable() As String
            Get
                Return NPPublic.Show(Me.NP_GetData)
            End Get
        End Property
        Sub ShowCV()
            Dim MaxB As New Emgu.CV.Mat
            If OrgMat.Width < 100 And OrgMat.Height < 100 Then
                Emgu.CV.CvInvoke.Resize(OrgMat, MaxB, New Drawing.Size(OrgMat.Height * 30, OrgMat.Width * 30), , , Emgu.CV.CvEnum.Inter.Nearest)
            Else
                MaxB = OrgMat
            End If

            Emgu.CV.CvInvoke.Imshow("Test", MaxB)
            Emgu.CV.CvInvoke.WaitKey(100)
            MaxB.Dispose()
        End Sub
#End Region

#Region "NP Extensions"
        ReadOnly Property NP_GetValue(ByVal Row As Integer) As Double()
            Get
                Dim Dbl As Double
                Dim Value(Me.OrgMat.Cols - 1) As Double
                Marshal.Copy(Me.OrgMat.DataPointer + (Row * Me.OrgMat.Cols) * Marshal.SizeOf(Dbl), Value, 0, Value.Length)
                Return Value
            End Get
        End Property
        ReadOnly Property NP_GetValue(ByVal Row As Integer, ByVal Col As Integer) As Double
            Get
                Dim Value As Double = 0
                Marshal.Copy(Me.OrgMat.DataPointer + (Row * Me.OrgMat.Cols + Col) * Marshal.SizeOf(Value), {Value}, 0, 1)
                Return Value
            End Get
        End Property
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
            Return New Mat(OrgMat.Reshape(1, OrgMat.Width * OrgMat.Height))
        End Function
        Function NP_VStack(ByVal Mat2 As Mat) As Mat
            Return VStack(Me, Mat2)
        End Function
        Function NP_HStack(ByVal Mat2 As Mat) As Mat
            Return VStack(Me, Mat2)
        End Function
        <DebuggerStepThrough()>
        Function NP_AsType(ByVal DataType As Emgu.CV.CvEnum.DepthType) As Mat
            Dim RET As New Mat : OrgMat.ConvertTo(RET.OrgMat, DataType) : Return RET
        End Function

        Shared Function NP_linspace(ByVal Start As Double, ByVal Ends As Double, ByVal Cnt As Integer) As Mat
            Dim StepWidth As Double = (Ends - Start) / (Cnt - 1)
            Dim Ret As New List(Of Double)
            For i As Double = Start To Ends Step StepWidth
                Ret.Add(Math.Round(i, 1))
            Next
            Return New Mat(Ret.ToArray).NP_AsType(Emgu.CV.CvEnum.DepthType.Cv32F)
        End Function

#End Region

#Region "PY Extensions"
        Function PY_Slice(ByVal Start As String, ByVal Stops As String, ByVal Steps As String) As Mat
            Return Python.Slice(Me, Start, Stops, Steps)
        End Function
#End Region

#Region "ArgSort"
        Public Function NP_ArgSort() As Mat
            Dim Desc As New MatrixExplain(Me)
            Select Case Desc.MatTypA
                Case MatrixExplain.DataType.VectorV
                    Dim A = Me.NP_GetData
                    Dim N As New Dictionary(Of Integer, Object)
                    For i As Integer = 0 To Me.Width
                        N.Add(i, A(i)(0))
                    Next
                    Dim T = N.OrderBy(Function(s) s.Value).Select(Function(s) s.Key).ToArray
                    Return New Mat(T).NP_AsType(Emgu.CV.CvEnum.DepthType.Cv16S)
            End Select
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
End Class
