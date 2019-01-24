Imports NPSharp.NPEmgu

Partial Public Class NPPublic
    '<DebuggerStepThrough()>
    Class MatrixExplain
        Private A As Object
        Private B As Object

        Public Overrides Function ToString() As String
            Return "A:" & NumTypeA.ToString & ":" & ObTypeA.ToString & ":" & MatTypA.ToString & " ... " &
                   "B:" & NumTypeB.ToString & ":" & ObTypeB.ToString & ":" & MatTypB.ToString
        End Function

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

        <DebuggerStepThrough()>
        Sub New(ByVal MatA As Object, Optional ByVal MatB As Object = Nothing)
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
            Failed = 99
        End Enum
        Function NumType(ByVal Array As Object) As NP_DataType
            If IsNothing(Array) Then Return NP_DataType.Failed
            Dim Check As String = Array.GetType.ToString
            Select Case Array.GetType
                Case GetType(Mat) : Return Array.depth

                Case GetType(Byte) : Return NP_DataType.Cv8U_Byte
                Case GetType(Byte()) : Return NP_DataType.Cv8U_Byte
                Case GetType(Byte()()) : Return NP_DataType.Cv8U_Byte
                Case GetType(Byte()()()) : Return NP_DataType.Cv8U_Byte

                Case GetType(Integer) : Return NP_DataType.Cv32S_Int32
                Case GetType(Integer()) : Return NP_DataType.Cv32S_Int32
                Case GetType(Integer()()) : Return NP_DataType.Cv32S_Int32

                Case GetType(Single) : Return NP_DataType.Cv32F_Single
                Case GetType(Single()) : Return NP_DataType.Cv32F_Single
                Case GetType(Single()()) : Return NP_DataType.Cv32F_Single

                Case GetType(Double) : Return NP_DataType.Cv64F_Double
                Case GetType(Double()) : Return NP_DataType.Cv64F_Double
                Case GetType(Double()()) : Return NP_DataType.Cv64F_Double

                Case Else : Throw New Exception("Not implemented yet")
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
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorV Then If AM.Rows = BM.Rows Then Return CalcType.VV_VV_SYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorH Then If AM.Cols <> BM.Cols Then Return CalcType.VH_VH_ASYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorV Then If AM.Rows <> BM.Rows Then Return CalcType.VV_VV_ASYM

                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorV Then If AM.Cols = BM.Rows Then Return CalcType.VH_VV_SYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorH Then If AM.Rows = BM.Cols Then Return CalcType.VV_VH_SYM
                If MatTypA = DataType.VectorH And MatTypB = DataType.VectorV Then If AM.Cols <> BM.Rows Then Return CalcType.VH_VV_ASYM
                If MatTypA = DataType.VectorV And MatTypB = DataType.VectorH Then If AM.Rows <> BM.Cols Then Return CalcType.VV_VH_ASYM

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
End Class