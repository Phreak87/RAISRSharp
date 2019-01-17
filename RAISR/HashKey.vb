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

        Dim G As Mat = gx.NP_VStack(gy).T '         {2,81}
        Dim GTWG1 As Mat = G.T.dot(WIn, False) '  
        Dim GTWG As Mat = GTWG1.NP_dot(G, False)
        Dim GTWGD As Double()() = GetData(GTWG)

        Dim Eigen As New Eigen(GTWG)
        Dim W = Eigen.Values : Dim WD As Double()() = GetData(W)
        Dim V = Eigen.Vectors : Dim VD As Double()() = GetData(V)

        Dim nonzerow = NPSharp.NPEmgu.Count_nonzero(IsReal(W)) : If nonzerow <> 0 Then W = real(W)
        Dim nonzerov = NPSharp.NPEmgu.Count_nonzero(IsReal(V)) : If nonzerov <> 0 Then V = real(V)

        Dim idx = NPSharp.Python.Slice(W.NP_ArgSort, ":", ":", "-1")

        W = W.Row(0)
        ' V = V(":", idx)
        Dim theta = Math.Atan2(1.0, 0.0)
        If theta < 0 Then theta = theta + pi

        Dim lamda = WD(0)(0)
        Dim sqrtlamda1 = Math.Sqrt(WD(0)(0))
        Dim sqrtlamda2 = Math.Sqrt(WD(1)(0))

        Dim u As Double = 0 : If sqrtlamda1 + sqrtlamda2 > 0 Then u = (sqrtlamda1 - sqrtlamda2) / (sqrtlamda1 + sqrtlamda2)
        angle = Math.Floor(theta / pi * Qangle)

        strength = 0
        If lamda < 0.0001 Then
        ElseIf lamda > 0.001 Then : strength = 2
        Else : strength = 1
        End If

        coherence = 0
        If u < 0.25 Then
        ElseIf u > 0.5 Then : coherence = 2
        Else : coherence = 1
        End If

        If angle > 23 Then : angle = 23
        ElseIf angle < 0 Then : angle = 0
        End If


    End Sub
End Class
