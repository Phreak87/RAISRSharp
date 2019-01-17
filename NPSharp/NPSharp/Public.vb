Imports System.Runtime.InteropServices
Imports NPSharp.NPEmgu

Public Class NPPublic
    '<DebuggerStepThrough()>
    Class MatrixExplain
        Private A As Object
        Private B As Object
        ReadOnly Property MatAData As Object
            Get
                If ObTypeA = ObjType.Mat Then Return GetData(A)
                If ObTypeA = ObjType.Array Then Return A
                Return Nothing
            End Get
        End Property
        ReadOnly Property MatBData As Object
            Get
                If ObTypeB = ObjType.Mat Then Return GetData(B)
                If ObTypeB = ObjType.Array Then Return B
                Return Nothing
            End Get
        End Property
        ReadOnly Property NumTypeA As NP_DataType
            Get
                Return NumType(A)
            End Get
        End Property
        ReadOnly Property NumTypeB As NP_DataType
            Get
                Return NumType(B)
            End Get
        End Property
        ReadOnly Property ObTypeA As ObjType
            Get
                Return ObjectType(A)
            End Get
        End Property
        ReadOnly Property ObTypeB As ObjType
            Get
                Return ObjectType(B)
            End Get
        End Property
        ReadOnly Property MatTypA As DataType
            Get
                Return MatrixType(A)
            End Get
        End Property
        ReadOnly Property MatTypB As DataType
            Get
                Return MatrixType(B)
            End Get
        End Property
        ReadOnly Property CalcTyp As CalcType
            Get
                Return CalculationType(A, B)
            End Get
        End Property
        Property ErrDesc As String

        Sub New(MatA As Object, Optional MatB As Object = Nothing)
            A = MatA
            B = MatB
        End Sub

        Enum ObjType
            Mat = 0
            Numb = 1
            Array = 2
            Failed = 99
        End Enum
        Private Function ObjectType(ByVal A As Object) As ObjType
            If IsNothing(A) Then Return ObjType.Failed
            If A.GetType = GetType(String) Then Return ObjType.Failed

            If A.GetType = GetType(Mat) Then Return ObjType.Mat
            If IsNumeric(A) Then Return ObjType.Numb
            Return ObjType.Array
        End Function

        Enum NP_DataType
            Cv8U_Byte = Emgu.CV.CvEnum.DepthType.Cv8U
            Cv8S_SByte = Emgu.CV.CvEnum.DepthType.Cv8S
            Cv16S_Int16 = Emgu.CV.CvEnum.DepthType.Cv16S
            Cv16U_UInt16 = Emgu.CV.CvEnum.DepthType.Cv16U
            Cv32S_Int32 = Emgu.CV.CvEnum.DepthType.Cv32S
            Cv32F_Single = Emgu.CV.CvEnum.DepthType.Cv32F
            Cv64F_Double = Emgu.CV.CvEnum.DepthType.Cv64F
            Unknown = 99
        End Enum
        Function NumType(ByVal Array As Object) As NP_DataType
            If IsNothing(Array) Then Return NP_DataType.Unknown
            Select Case Array.GetType
                Case GetType(Integer) : Return NP_DataType.Cv32S_Int32
                Case GetType(Integer()) : Return NP_DataType.Cv32S_Int32
                Case GetType(Integer()()) : Return NP_DataType.Cv32S_Int32

                Case GetType(Double) : Return NP_DataType.Cv64F_Double
                Case GetType(Double()) : Return NP_DataType.Cv64F_Double
                Case GetType(Double()()) : Return NP_DataType.Cv64F_Double

            End Select
        End Function

        Enum DataType
            Number = 0
            VectorH = 1
            VectorV = 2
            Matrix = 3
            Failed = 99
        End Enum
        Private Function MatrixType(ByVal A As Object) As DataType
            Select Case ObjectType(A)
                Case ObjType.Numb : Return DataType.Number
                Case ObjType.Failed : Return DataType.Failed
                Case ObjType.Mat
                    Dim AMat As Mat = CType(A, Mat)
                    If AMat.Rows = 1 And AMat.Cols = 1 Then Return DataType.Number
                    If AMat.Rows = 1 And AMat.Cols > 1 Then Return DataType.VectorH
                    If AMat.Rows > 1 And AMat.Cols = 1 Then Return DataType.VectorV
                    If AMat.Rows > 1 And AMat.Cols > 1 Then Return DataType.Matrix
                Case ObjType.Array
                    Dim ABound1 As Integer = UBound(A)
                    Dim ABound2 As Integer = 0 : If IsNumeric(A(0)) = False Then ABound2 = UBound(A(0))
                    Select Case ABound1
                        Case 0
                            If ABound2 = 0 Then Return DataType.Number
                            If ABound2 > 0 Then Return DataType.VectorH
                        Case Else
                            If ABound2 = 0 Then Return DataType.VectorV
                            If ABound2 > 0 Then Return DataType.Matrix
                    End Select
            End Select
            Return DataType.Failed
        End Function

        Enum CalcType
            N_N = 0         ' Number * Number .
            VH_N = 1        ' Vector * Number .
            VV_N = 2        ' Vector * Number .
            M_N = 3         ' Matrix * Number . 

            VH_VH_SYM = 4       ' Vector * Vector (Horizonal, Gleiche Anzahl)
            VV_VV_SYM = 5       ' Vector * Vector (Vertical,  Gleiche Anzahl)
            VH_VH_ASYM = 6      ' Vector * Vector (Horizonal, Differente Anzahl)
            VV_VV_ASYM = 7      ' Vector * Vector (Vertical,  Differente Anzahl)

            VV_VH_SYM = 8       ' Vector * Vector (Horizonal)
            VH_VV_SYM = 9       ' Vector * Vector (Horizonal)
            VV_VH_ASYM = 10      ' Vector * Vector (Vertical)
            VH_VV_ASYM = 11      ' Vector * Vector (Vertical)

            M_VV_SYM = 12         ' Matrix * Vector
            M_VH_SYM = 13         ' Matrix * Vector
            M_M_SYM = 14
            M_M_ASYM = 15

            FAIL = 99
        End Enum
        Private Function CalculationType(ByVal A As Object, ByVal B As Object) As CalcType
            If IsNothing(A) Then Return CalcType.FAIL
            If IsNothing(B) Then Return CalcType.FAIL
            If MatTypA = DataType.Number And MatTypB = DataType.Number Then Return CalcType.N_N
            If MatTypA = DataType.Number And MatTypB = DataType.VectorH Then Return CalcType.VH_N
            If MatTypA = DataType.Number And MatTypB = DataType.VectorV Then Return CalcType.VV_N
            If MatTypA = DataType.VectorH And MatTypB = DataType.Number Then Return CalcType.VH_N
            If MatTypA = DataType.VectorV And MatTypB = DataType.Number Then Return CalcType.VV_N
            If MatTypA = DataType.Number And MatTypB = DataType.Matrix Then Return CalcType.M_N
            If MatTypA = DataType.Matrix And MatTypB = DataType.Number Then Return CalcType.M_N

            If A.GetType = GetType(Mat) Then
                Dim AM As Emgu.CV.Mat = CType(A.OrgMat, Emgu.CV.Mat)
                Dim BM As Emgu.CV.Mat = CType(B.OrgMat, Emgu.CV.Mat)
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorH Then If AM.Cols = BM.Cols Then Return CalcType.VH_VH_SYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorV Then If AM.Cols = BM.Cols Then Return CalcType.VV_VV_SYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorH Then If AM.Cols <> BM.Cols Then Return CalcType.VH_VH_ASYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorV Then If AM.Cols <> BM.Cols Then Return CalcType.VV_VV_ASYM

                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorV Then If AM.Cols = BM.Rows Then Return CalcType.VH_VV_SYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorH Then If AM.Cols = BM.Rows Then Return CalcType.VV_VH_SYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorV Then If AM.Cols <> BM.Rows Then Return CalcType.VH_VV_ASYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorH Then If AM.Cols <> BM.Rows Then Return CalcType.VV_VH_ASYM
                If MatTypA = DataType.Matrix And MatTypB = DataType.VectorH Then Return CalcType.M_VH_SYM
                If MatTypA = DataType.Matrix And MatTypB = DataType.VectorV Then Return CalcType.M_VV_SYM
                If MatTypA = DataType.Matrix Then
                    If A.rows = B.rows And A.cols = B.cols Then
                        Return CalcType.M_M_SYM
                    Else
                        If A.cols = B.Rows Then
                            Return CalcType.M_M_ASYM
                        Else
                            ErrDesc = "A.Cols must be B.Rows"
                        End If
                    End If
                End If
            Else
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorH Then If A(0).length = B(0).length Then Return CalcType.VH_VH_SYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorV Then If A(0).length = B(0).length Then Return CalcType.VV_VV_SYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorH Then If A(0).length <> B(0).length Then Return CalcType.VH_VH_ASYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorV Then If A(0).length <> B(0).length Then Return CalcType.VV_VV_ASYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorV Then If A(0).length = B.length Then Return CalcType.VH_VV_SYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorH Then If A(0).length = B.length Then Return CalcType.VV_VH_SYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorV Then If A(0).length <> B.length Then Return CalcType.VH_VV_ASYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorH Then If A(0).length <> B.length Then Return CalcType.VV_VH_ASYM
                If MatTypA = DataType.Matrix And MatTypB = DataType.VectorH Then Return CalcType.M_VH_SYM
                If MatTypA = DataType.Matrix And MatTypB = DataType.VectorV Then Return CalcType.M_VV_SYM
                If MatTypA = DataType.Matrix And MatTypB = DataType.Matrix Then
                    If A.length = B.length And A(0).Length = B(0).length Then
                        Return CalcType.M_M_SYM
                    Else
                        If A(0).length = B.length Then
                            Return CalcType.M_M_ASYM
                        Else
                            ErrDesc = "A.Cols must be B.Rows"
                        End If
                    End If
                End If
            End If

            Return CalcType.FAIL
        End Function
    End Class

#Region "Set/Get"
    'Default	-1	default
    'Cv8U	0	Byte
    'Cv8S	1	SByte
    'Cv16U	2	UInt16
    'Cv16S	3	Int16
    'Cv32S	4	Int32/Integer
    'Cv32F	5	float
    'Cv64F	6	double

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
    Shared Function GetData(ByVal A As Mat,
                            Optional ByVal Row As String = ":",
                            Optional ByVal Col As String = ":",
                            Optional ByVal Idx As String = ":") As Object
        If IsNothing(A) Then Return Nothing

        If Row = ":" And
           Col = ":" And
           Idx = ":" Then
            Select Case A.Depth
                Case Emgu.CV.CvEnum.DepthType.Cv8U : Return GetCV8U(A) '    Byte
                Case Emgu.CV.CvEnum.DepthType.Cv16S : Return GetCV16S(A) '  Int16
                Case Emgu.CV.CvEnum.DepthType.Cv32S : Return GetCV32S(A) '  Integer
                Case Emgu.CV.CvEnum.DepthType.Cv32F : Return GetCV32F(A) '  Float
                Case Emgu.CV.CvEnum.DepthType.Cv64F : Return GetCV64F(A) '  Double
                Case Else : Throw New NotImplementedException
            End Select
        Else
            Dim IRow As Integer() = {Row, Row} : If Row = ":" Then IRow = {0, A.Height}
            Dim ICol As Integer() = {Col, Col} : If Col = ":" Then ICol = {0, A.Width}
            Dim IIdx As Integer() = {Idx, Idx} : If Idx = ":" Then IIdx = {0, A.Channels}
            Return A.Row(Row).Col(Col).NP_GetData(Idx)
        End If
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
    '<DebuggerStepThrough()>
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

#Region "Helper"
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
