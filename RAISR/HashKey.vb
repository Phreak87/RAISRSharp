
Module hashkey
    Const pi = Math.PI
    Function hashkey(ByVal block As Object,
                     ByVal Qangle As Object,
                     ByVal W As Object) As Object

        Dim _tup_1 = NP.gradient(block)
        Dim gy = _tup_1.Item1
        Dim gx = _tup_1.Item2
        gx = gx.ravel()
        gy = gy.ravel()
        Dim G = NP.vstack(Tuple.Create(gx, gy)).T
        Dim GTWG = G.T.dot(W).dot(G)
        Dim _tup_2 = NP.linalg.eig(GTWG)
        Dim w1 = _tup_2.Item1
        Dim v = _tup_2.Item2
        Dim nonzerow = NP.count_nonzero(NP.isreal(w))
        Dim nonzerov = NP.count_nonzero(NP.isreal(v))

        If nonzerow <> 0 Then w = NP.real(w)
        If nonzerov <> 0 Then v = NP.real(v)

        Dim idx = w.argsort()() ' [' - 1:]

        w = w(idx)
        v = v(":", idx)
        Dim theta = Math.Atan2(v(1, 0), v(0, 0))

        If theta < 0 Then theta = theta + pi

        Dim lamda = w(0)
        Dim sqrtlamda1 = Math.Sqrt(w(0))
        Dim sqrtlamda2 = Math.Sqrt(w(1))

        Dim u As Double = 0 : If sqrtlamda1 + sqrtlamda2 > 0 Then u = (sqrtlamda1 - sqrtlamda2) / (sqrtlamda1 + sqrtlamda2)
        Dim angle = Math.Floor(theta / pi * Qangle)

        Dim strength = 0
        If lamda < 0.0001 Then
        ElseIf lamda > 0.001 Then : strength = 2
        Else : strength = 1
        End If

        Dim coherence = 0
        If u < 0.25 Then
        ElseIf u > 0.5 Then : coherence = 2
        Else : coherence = 1
        End If

        If angle > 23 Then : angle = 23
        ElseIf angle < 0 Then : angle = 0
        End If

        Return Tuple.Create(angle, strength, coherence)
    End Function
End Module
