Module gaussian2d
    Function gaussian2d(Optional ByVal shape As Object = Nothing,
                        Optional ByVal sigma As Object = 0.5) As Object

        If Not IsNothing(shape) Then shape = Tuple.Create(3, 3)
        Dim _tup_1 = shape.[Select](Function(ss) (ss - 1.0) / 2.0)
        Dim m = _tup_1.Item1
        Dim n = _tup_1.Item2
        Dim _tup_2 As Object ' Dim _tup_2 = np.ogrid(-m._(m + 1), -n._(n + 1))
        Dim y = _tup_2.Item1
        Dim x = _tup_2.Item2
        Dim h = NP.exp(-x * x + y * y / (2.0 * sigma * sigma))
        h(h < NP.finfo(h.dtype).eps * h.max()) = 0
        Dim sumh = h.sum()

        If sumh <> 0 Then h /= sumh
        Return h
    End Function
End Module
