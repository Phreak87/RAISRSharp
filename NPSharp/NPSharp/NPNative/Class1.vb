Public Class NPNative

#Region "Zeros"
    <DebuggerStepThrough()>
    Shared Function Zeros(ByVal Rows As Integer, ByVal Cols As Integer) As Double()()
        Dim RET As New List(Of Double())
        For i As Integer = 0 To Rows - 1
            Dim RETA(Cols - 1) As Double
            RET.Add(RETA)
        Next
        Return RET.ToArray
    End Function
#End Region

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

#Region "Add"
    Shared Function Add(ByVal A As Double(), ByVal B As Double) As Double()
        Dim RET As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            RET.Add(A(i) + B)
        Next
        Return RET.ToArray
    End Function
    Shared Function Add(ByVal A As Double(), ByVal B As Double()) As Double()
        Dim RET As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            RET.Add(A(i) + B(i))
        Next
        Return RET.ToArray
    End Function
    Shared Function Add(ByVal A As Double()(), ByVal B As Double()) As Double()()
        Dim Ret As New List(Of Double())
        For i As Integer = 0 To A(0).Count - 1
            Ret.Add(Add(A(0), B(i)))
        Next
        Return Ret.ToArray
    End Function
#End Region

#Region "Multiply"
    Class MatrixExplain
        Property MatA
        Property MatB
        Property TypA
        Property TypB
        Property TypM
        Sub New(ByVal A As Object, ByVal B As Object)
            MatA = A
            MatB = B
        End Sub
    End Class
    Enum DataType
        Number = 0
        VectorH = 1
        VectorV = 2
    End Enum
    Enum MatrixTyp
        Sync = 0
        Async = 1
        Failed = 2

        N_N = 0         ' Number * Number
        VH_N = 0        ' Vector * Number
        VV_N = 0        ' Vector * Number
        M_N = 0         ' Matrix * Number

        VH_VH_SYM = 2       ' Vector * Vector (Horizonal)
        VV_VV_SYM = 2       ' Vector * Vector (Vertical)
        VH_VH_ASYM = 2      ' Vector * Vector (Horizonal)
        VV_VV_ASYM = 2      ' Vector * Vector (Vertical)
        VV_VH_SYM = 2       ' Vector * Vector (Horizonal)
        VH_VV_SYM = 2       ' Vector * Vector (Horizonal)
        VV_VH_ASYM = 2      ' Vector * Vector (Vertical)
        VH_VV_ASYM = 2      ' Vector * Vector (Vertical)

        M_VV = 1         ' Matrix * Vector
        M_VH = 1         ' Matrix * Vector
        M_M_SYNC
        M_M_ASYNC

    End Enum
    Private Shared Function MatrixType(ByVal A As Object, ByVal B As Object) As MatrixTyp
        If B.GetType = GetType(System.Double) Then Return MatrixTyp.Sync
        Select Case A.GetType
            Case GetType(System.Double)
                If A.Length = B.Length And A(0).Length = B(0).Length Then Return MatrixTyp.Sync '   A und B sind gleich, Synchron Multiplizieren
                If A(0).Length = B.Length Then Return MatrixTyp.Async '                             A und B sind unterschiedlich, Scalar Multiplizieren
                Return MatrixTyp.Failed '                                                           Arraytyp nicht Multiplizierbar
            Case GetType(System.Double())
                If B.GetType = GetType(System.Double) Then Return MatrixTyp.Sync
                If A.Length = B.Length Then Return MatrixTyp.Sync '                                 A und B sind gleich, Synchron Multiplizieren
                If A(0).Length = B.Length Then Return MatrixTyp.Async '                             A und B sind unterschiedlich, Scalar Multiplizieren
                Return MatrixTyp.Failed '                                                           Arraytyp nicht Multiplizierbar
            Case GetType(System.Double()())
                If B.GetType = GetType(System.Double) Then Return MatrixTyp.Sync
                If A.Length = B.Length And A(0).Length = B(0).Length Then Return MatrixTyp.Sync '   A und B sind gleich, Synchron Multiplizieren
                If A(0).Length = B.Length Then Return MatrixTyp.Async '                             A und B sind unterschiedlich, Scalar Multiplizieren
                Return MatrixTyp.Failed '                                                           Arraytyp nicht Multiplizierbar
        End Select
    End Function

    Shared Function Multiply(ByVal A As Double(), ByVal B As Double) As Double()
        Dim Ret As New List(Of Double)
        For Rows As Integer = 0 To A.Count - 1
            Ret.Add(A(Rows) * B)
        Next
        Return Ret.ToArray
    End Function
    Shared Function Multiply(ByVal A As Double(), ByVal B As Double()) As Double()
        Select Case MatrixType(A, B)
            Case MatrixTyp.Sync
                Dim Ret As New List(Of Double)
                For Rows As Integer = 0 To A.Count - 1
                    Ret.Add(A(Rows) * B(Rows))
                Next
                Return Ret.ToArray
            Case MatrixTyp.ASync : Throw New Exception("Not Implemented")
            Case MatrixTyp.Failed : Throw New Exception("Matrix can be: A.W = B.H (ASync) || A.W = B.W && A.H = B.H (Sync)")
        End Select
        Return Nothing
    End Function
    Shared Function Multiply(ByVal A As Double()(), ByVal B As Double()()) As Double()()
        Select Case MatrixType(A, B)
            Case MatrixTyp.Sync
                Dim Ret As New List(Of Double())
                For Rows As Integer = 0 To A.Count - 1
                    Dim RetD(A(0).Count - 1) As Double
                    For i2 As Integer = 0 To A(0).Count - 1
                        RetD(i2) = A(Rows)(i2) * B(Rows)(i2)
                    Next
                    Ret.Add(RetD)
                Next
                Return Ret.ToArray
            Case MatrixTyp.ASync : Throw New Exception("Not Implemented")
            Case MatrixTyp.Failed : Throw New Exception("Matrix can be: A.W = B.H (ASync) || A.W = B.W && A.H = B.H (Sync)")
        End Select
        Return Nothing
    End Function
    Shared Function Multiply(ByVal A As Integer()(), ByVal B As Integer()()) As Integer()()
        Select Case MatrixType(A, B)
            Case MatrixTyp.Sync
                Dim Ret As New List(Of Integer())
                For Rows As Integer = 0 To A.Count - 1
                    Dim RetD(A(0).Count - 1) As Integer
                    For i2 As Integer = 0 To A(0).Count - 1
                        RetD(i2) = A(Rows)(i2) * B(Rows)(i2)
                    Next
                    Ret.Add(RetD)
                Next
                Return Ret.ToArray
            Case MatrixTyp.ASync : Throw New Exception("Not Implemented")
            Case MatrixTyp.Failed : Throw New Exception("Matrix can be: A.W = B.H (ASync) || A.W = B.W && A.H = B.H (Sync)")
        End Select
        Return Nothing
    End Function

    Shared Function Multiply(ByVal A As Short(), ByVal B As Short) As Short()
        Dim L1 As New List(Of Short)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) * B)
        Next
        Return L1.ToArray
    End Function
    Shared Function Multiply(ByVal A As Short(), ByVal B As Short()) As Short()
        Dim L1 As New List(Of Short)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) * B(i))
        Next
        Return L1.ToArray
    End Function
    Shared Function Multiply(ByVal A As Short()(), ByVal B As Short()()) As Short()()
        Dim L0 As New List(Of Short())
        For i As Integer = 0 To A.Count - 1
            L0.Add(Multiply(A(i), B(i)))
        Next
        Return L0.ToArray
    End Function
#End Region

#Region "Divide"
    Shared Function Divide(ByVal A As Double(), ByVal B As Double) As Double()
        Select Case MatrixType(A, B)
            Case MatrixTyp.Sync
                Dim L1 As New List(Of Double)
                For i As Integer = 0 To A.Count - 1
                    L1.Add(A(i) / B)
                Next
                Return L1.ToArray
        End Select
        Return Nothing
    End Function
    Shared Function Divide(ByVal A As Double(), ByVal B As Double()) As Double()
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) / B(i))
        Next
        Return L1.ToArray
    End Function
    Shared Function Divide(ByVal A As Double()(), ByVal B As Double()()) As Double()()
        Dim L0 As New List(Of Double())
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L0.Add(Divide(A(i), B(i)))
        Next
        Return L0.ToArray
    End Function
    Shared Function Divide(ByVal A As Double()(), ByVal B As Double) As Double()()
        Select Case MatrixType(A, B)
            Case MatrixTyp.Sync
                Dim L0 As New List(Of Double())
                For i As Integer = 0 To A.Count - 1
                    L0.Add(Divide(A(i), B))
                Next
                Return L0.ToArray
        End Select
        Return Nothing
    End Function
#End Region

#Region "Negate"
    Shared Function Negate(ByVal A As Double()) As Double()
        Dim RES(A.Count - 1) As Double
        For i As Integer = 0 To A.Count - 1
            RES(i) = A(i) * -1
        Next
        Return RES
    End Function
    Shared Function Negate(ByVal A As Double()()) As Double()()
        Dim RES As New List(Of Double())
        For i As Integer = 0 To A.Count - 1
            RES.Add(Negate(A(i)))
        Next
        Return RES.ToArray
    End Function
#End Region

#Region "Sum"
    Shared Function Sum(ByVal A As Double()) As Double
        Dim Res As Double = 0
        For Each entry In A
            Res += entry
        Next
        Return Res
    End Function
    Shared Function Sum(ByVal A As Double()()) As Double
        Dim Res As Double = 0
        For Each Entry In A
            Res += Sum(Entry)
        Next
        Return Res
    End Function
#End Region

    Shared Function eye()

    End Function
    Shared Function exp(ByVal A As Double()()) As Double()()
        Dim RES As New List(Of Double())
        For i As Integer = 0 To A.Count - 1
            Dim L1 As New List(Of Double)
            For i2 As Integer = 0 To A(i).Count - 1
                L1.Add(Math.Exp(A(i)(i2) * -1))
            Next
            RES.Add(L1.ToArray)
        Next
        Return RES.ToArray
    End Function

    Class Linalg
        Public Shared Function Det(ByVal A As Object) As Integer

        End Function
        Public Shared Function Inv(ByVal A As Object) As Double()()

        End Function
    End Class

    Shared Function Ravel(ByVal A As Double()()) As Double()
        Dim RES As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            RES.AddRange(A(i))
        Next
        Return RES.ToArray
    End Function


    Shared Function Reshape()

    End Function

    Shared Function Diag(ByVal A As Int16(), Optional ByVal k As Double = 0) As Int16()()
        Dim Res As New List(Of Int16())
        If k = 0 Then
            For i As Integer = 0 To A.Count - 1
                Dim L1(A.Count - 1) As Int16
                L1(i) = (A(i))
                Res.Add(L1)
            Next
        Else
            Throw New NotImplementedException
        End If
        Return Res.ToArray
    End Function
    Shared Function Diag(ByVal A As Integer(), Optional ByVal k As Double = 0) As Integer()()
        Dim Res As New List(Of Integer())
        If k = 0 Then
            For i As Integer = 0 To A.Count - 1
                Dim L1(A.Count - 1) As Integer
                L1(i) = (A(i))
                Res.Add(L1)
            Next
        Else
            Throw New NotImplementedException
        End If
        Return Res.ToArray
    End Function
    Shared Function Diag(ByVal A As Double(), Optional ByVal k As Double = 0) As Double()()
        Dim Res As New List(Of Double())
        If k = 0 Then
            For i As Integer = 0 To A.Count - 1
                Dim L1(A.Count - 1) As Double
                L1(i) = (A(i))
                Res.Add(L1)
            Next
        Else
            Throw New NotImplementedException
        End If
        Return Res.ToArray
    End Function

    Shared Function T(ByVal A()() As Integer) As Integer()()
        Dim Ret As New List(Of Integer())
        For Col As Integer = 0 To A(0).Length - 1
            Dim ARow(A.Length - 1) As Integer
            For Row As Integer = 0 To A.Length - 1
                ARow(Row) = A(Row)(Col)
            Next
            Ret.Add(ARow)
        Next
        Return Ret.ToArray()
    End Function
    Shared Function T(ByVal A()() As Double) As Double()()
        Dim Ret As New List(Of Double())
        For Col As Double = 0 To A(0).Length - 1
            Dim ARow(A.Length - 1) As Double
            For Row As Double = 0 To A.Length - 1
                ARow(Row) = A(Row)(Col)
            Next
            Ret.Add(ARow)
        Next
        Return Ret.ToArray()
    End Function
    Shared Function T(ByVal A()() As Single) As Single()()
        Dim Ret As New List(Of Single())
        For Col As Single = 0 To A(0).Length - 1
            Dim ARow(A.Length - 1) As Single
            For Row As Single = 0 To A.Length - 1
                ARow(Row) = A(Row)(Col)
            Next
            Ret.Add(ARow)
        Next
        Return Ret.ToArray()
    End Function


End Class