Imports NPSharp.NPEmgu

Module gaussian2d
    Function gaussian2d(Optional ByVal shape As Integer() = Nothing,
                        Optional ByVal sigma As Double = 0.5) As Mat

        If IsNothing(shape) Then shape = {3, 3}
        Dim m As Integer = (shape(0) - 1) / 2
        Dim n As Integer = (shape(1) - 1) / 2

        Dim Y_X As Tuple(Of Double(), Double()()) = NPSharp.NPEmgu.OGrid({-m, m + 1}, {-n, n + 1})
        Dim Y = New Mat(Y_X.Item1) ' VectorV
        Dim X = New Mat(Y_X.Item2) ' VectorH

        Dim M2 As Mat = ((X * X + Y * Y) / (2 * sigma * sigma)) * -1 ' VectorH * VectorH
        Dim h = M2.Exp

        'h(h < NP.finfo(h.dtype).eps * h.max()) = 0
        Dim sumh = NPSharp.NPEmgu.Sum(h)
        If sumh <> 0 Then h = h / sumh
        Return h.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv32F)
    End Function
End Module
