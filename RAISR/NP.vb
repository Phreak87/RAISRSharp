﻿Public Class NP

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
        Dim RET(Count) As Double

        Dim I As Integer = 0
        For Val As Double = Min To Max Step Steps
            RET(I) = Val : I += 1
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

    Shared Function Ravel(A As Double()()) As Double()
        Dim RES As New List(Of Double)
        For i As Integer = 0 To A.Count - 1
            RES.AddRange(A(i))
        Next
        Return RES.ToArray
    End Function

    Shared Function eye()

    End Function
    Shared Function exp(A As Double()()) As Double()()
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
    Shared Function linalg()

    End Function
    Shared Function finfo()

    End Function
    Shared Function gradient()

    End Function
    Shared Function vstack()

    End Function
    Shared Function isreal()

    End Function
    Shared Function real()

    End Function
    Shared Function count_nonzero()

    End Function
    Shared Function linspace(a As Integer, b As Integer, c As Integer)
        Dim Steps As Double = (b - a) / c
        Dim Ret As New List(Of Double)
        For i As Double = a To b Step Steps
            Ret.Add(Math.Round(i, 0))
        Next
        Return Ret
    End Function
    Shared Function Reshape()

    End Function

    Shared Function Diag(A As Double(), Optional k As Double = 0)
        Dim Res As New List(Of Double())
        If k = 0 Then
            Dim L1(A.Count) As Double
            For i As Integer = 0 To A.Count - 1
                L1(i) = (A(i))
                Res.Add(L1)
            Next
        Else
            Throw New NotImplementedException
        End If
        Return Res.ToList
    End Function

    Shared Function Clip()

    End Function

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
