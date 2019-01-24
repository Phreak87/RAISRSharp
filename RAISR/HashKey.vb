Imports NPSharp.NPEmgu
Imports NPSharp.NPPublic

Class hashkey
    Const pi = Math.PI

    Property angle = 0.0
    Property strength = 0.0
    Property coherence = 0.0

    Sub New(ByVal Block As Mat,
            ByVal Qangle As Integer,
            ByVal WIn As Mat)

        Dim _tup_1 As New Gradient(Block) '          x9,9 y9,9
        Dim gy = _tup_1.Y.NP_Ravel  '                81,1 (W,H)
        Dim gx = _tup_1.X.NP_Ravel  '                81,1 (W,H)

        Dim G As Mat = gx.NP_VStack(gy).T '          {81,2} => Vertikal Stapeln (1x1) + (1x1) => (1x2)
        Dim GTWG1 As Mat = G.T.Dot(WIn, True) '      W Multiplizieren mit Transponierter Matrix G
        Dim GTWG As Mat = GTWG1.Dot(G, False) '      GTWG1 Multiplizieren mit G
        G.Dispose() : GTWG1.Dispose()

        Dim Eigen As New Eigen(GTWG)
        GTWG.Dispose()
        Dim W = Eigen.Values '                                                              ([0,0])
        Dim V = Eigen.Vectors '                                                             ([1,0],[0,1])

        'Dim nonzerow = NPSharp.NPEmgu.Count_nonzero(IsReal(W)) : If nonzerow <> 0 Then W = real(W)
        'Dim nonzerov = NPSharp.NPEmgu.Count_nonzero(IsReal(V)) : If nonzerov <> 0 Then V = real(V)

        Dim idx = W.NP_ArgSort '                            SortierREIHENFOLGE (Nicht Daten) => z.B. (Index 0,1)
        idx = NPSharp.Python.Slice(idx, ":", ":", "-1") '   Invertieren (Zeilen)             => z.B. (Index 1,0)
        W = NPSharp.Python.Slice(W, idx) '                                                   => z.B. (W = 0,0)
        V = NPSharp.Python.Slice(V, ":", idx) '                                              => ([0,1],[1,0])

        Dim tv1 = V.NP_GetData(1)(0)
        Dim theta = Math.Atan2(tv1, V.NP_GetValue(0, 0))
        If theta < 0 Then theta = theta + pi

        Dim lamda = W.NP_GetValue(0, 0)
        Dim sqrtlamda1 = Math.Sqrt(W.NP_GetValue(0, 0))
        Dim sqrtlamda2 = Math.Sqrt(W.NP_GetValue(1)(0))

        Dim u As Double = 0
        If sqrtlamda1 + sqrtlamda2 = 0 Then
            u = 0
        Else
            u = (sqrtlamda1 - sqrtlamda2) / (sqrtlamda1 + sqrtlamda2)
        End If

        angle = Math.Floor(theta / pi * Qangle)

        strength = 0
        If lamda < 0.0001 Then
            strength = 0
        ElseIf lamda > 0.001 Then
            strength = 2
        Else
            strength = 1
        End If

        coherence = 0
        If u < 0.25 Then
            coherence = 0
        ElseIf u > 0.5 Then
            coherence = 2
        Else
            coherence = 1
        End If

        If angle > 23 Then
            angle = 23
        ElseIf angle < 0 Then
            angle = 0
        End If


    End Sub
End Class
