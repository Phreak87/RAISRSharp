Module gaussian2d
    Function gaussian2d(Optional ByVal shape As Integer() = Nothing,
                        Optional ByVal sigma As Double = 0.5) As Object

        If IsNothing(shape) Then shape = {3, 3}

        Dim m = (shape(0) - 1) / 2
        Dim n = (shape(1) - 1) / 2

        Dim Y_X As Tuple(Of Double(), Double()()) = NP.OGrid({-m, m}, {-n, n}) ' Dont add +1 like in the Original
        Dim y = Y_X.Item1
        Dim x = Y_X.Item2

        Dim MX_ = NP.Multiply(x, x)
        Dim MY_ = NP.Multiply(y, y)
        Dim A_ = NP.Add(MX_, MY_)
        Dim N_ = NP.Negate(A_)
        Dim D_ = NP.Divide(A_, 2.0 * sigma * sigma)
        Dim h = NP.exp(D_)
        'h(h < NP.finfo(h.dtype).eps * h.max()) = 0
        Dim sumh = NP.Sum(h)
        If sumh <> 0 Then h = NP.Divide(h, sumh)
        Dim Test As String = NP.Show(h)
        Return h
    End Function
End Module
