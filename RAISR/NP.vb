Public Class NP

    Shared Function Zeros(Count As Integer) As Double()
        Dim I(Count - 1) As Double : Return I
    End Function

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
    Shared Function OGrid(_From As Double, _To As Double, Count As Integer) As Double()
        Dim Min As Double = Math.Min(_From, _To)
        Dim Max As Double = Math.Max(_From, _To)
        Dim Sum As Double = (Max - Min)

        If Count = 0 Then Count = Sum
        Dim Steps As Double = Sum / Count
        Dim RET(Count - 1) As Double

        Dim I As Integer = 0
        For Val As Double = Min To Max Step Steps
            If Val < Max Then RET(I) = Val : I += 1
        Next

        Return RET
    End Function
    Shared Function OGrid(_From As Double, _To As Double) As Tuple(Of Double(), Double()())
        Dim RET As Double() = OGrid(_From, _To, 0)
        Return New Tuple(Of Double(), Double()())(RET, {RET})
    End Function
    Shared Function OGrid(A1 As Double(), B1 As Double()) As Tuple(Of Double(), Double()())
        Dim RETA As Double() = OGrid(A1(0), A1(1), 0)
        Dim RETB As Double() = OGrid(B1(0), B1(1), 0)
        Return New Tuple(Of Double(), Double()())(RETA, {RETb})
    End Function
#End Region

#Region "Add"
    Shared Function Add(A As Double(), B As Double) As Double()
        Dim RET As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            RET.Add(A(i) + B)
        Next
        Return RET.ToArray
    End Function
    Shared Function Add(A As Double(), B As Double()) As Double()
        Dim RET As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            RET.Add(A(i) + B(i))
        Next
        Return RET.ToArray
    End Function
    Shared Function Add(A As Double()(), B As Double()) As Double()()
        Dim Ret As New List(Of Double())
        For i As Integer = 0 To A(0).Count - 1
            Ret.Add(NP.Add(A(0), B(i)))
        Next
        Return Ret.ToArray
    End Function
#End Region

#Region "Multiply"
    Shared Function Multiply(A As Double(), B As Double) As Double()
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) * B)
        Next
        Return L1.ToArray
    End Function
    Shared Function Multiply(A As Double(), B As Double()) As Double()
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) * B(i))
        Next
        Return L1.ToArray
    End Function
    Shared Function Multiply(A As Double()(), B As Double()()) As Double()()
        Dim L0 As New List(Of Double())
        For i As Integer = 0 To A.Count - 1
            L0.Add(NP.Multiply(A(i), B(i)))
        Next
        Return L0.ToArray
    End Function
    Shared Function Multiply(A As Double()(), B As Double) As Double()()
        Dim L0 As New List(Of Double())
        For i As Integer = 0 To A.Count - 1
            L0.Add(NP.Multiply(A(i), B))
        Next
        Return L0.ToArray
    End Function
#End Region

#Region "Divide"
    Shared Function Divide(A As Double(), B As Double) As Double()
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) / B)
        Next
        Return L1.ToArray
    End Function
    Shared Function Divide(A As Double(), B As Double()) As Double()
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L1.Add(A(i) / B(i))
        Next
        Return L1.ToArray
    End Function
    Shared Function Divide(A As Double()(), B As Double()()) As Double()()
        Dim L0 As New List(Of Double())
        Dim L1 As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            L0.Add(NP.Divide(A(i), B(i)))
        Next
        Return L0.ToArray
    End Function
    Shared Function Divide(A As Double()(), B As Double) As Double()()
        Dim L0 As New List(Of Double())
        For i As Integer = 0 To A.Count - 1
            L0.Add(NP.Divide(A(i), B))
        Next
        Return L0.ToArray
    End Function
#End Region

#Region "Negate"
    Shared Function Negate(A As Double()) As Double()
        Dim RES(A.Count - 1) As Double
        For i As Integer = 0 To A.Count - 1
            RES(i) = A(i) * -1
        Next
        Return RES
    End Function
    Shared Function Negate(A As Double()()) As Double()()
        Dim RES As New List(Of Double())
        For i As Integer = 0 To A.Count - 1
            RES.Add(Negate(A(i)))
        Next
        Return RES.ToArray
    End Function
#End Region

#Region "Sum"
    Shared Function Sum(A As Double()) As Double
        Dim Res As Double = 0
        For Each entry In A
            Res += entry
        Next
        Return Res
    End Function
    Shared Function Sum(A As Double()()) As Double
        Dim Res As Double = 0
        For Each Entry In A
            Res += Sum(Entry)
        Next
        Return Res
    End Function
#End Region

#Region "Helper"
    Shared Function Show(A As Double()()) As String
        Dim S As New System.Text.StringBuilder
        S.Append("<HTML><HEAD></HEADY><BODY>")
        S.Append("<Table Border=1>")
        For Each Entry In A
            S.Append(AddHTMLLine(Entry))
        Next
        S.Append("</BODY></HTML>")
        Return S.ToString
    End Function
    Shared Function Show(A As Double()) As String
        Dim S As New System.Text.StringBuilder
        S.Append("<HTML><HEAD></HEADY><BODY>")
        S.Append("<Table Border=1>")
        S.Append(AddHTMLLine(A))
        S.Append("</BODY></HTML>")
        Return S.ToString
    End Function
    Private Shared Function AddHTMLLine(A As Double()) As String
        Return ("<TR><TD>" & String.Join("</TD><TD>", A) & "</TD>")
    End Function
#End Region
End Class
