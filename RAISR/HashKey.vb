
Module hashkey
    Const pi = Math.PI
    Function hashkey(ByVal Block As Emgu.CV.Mat,
                     ByVal Qangle As Integer,
                     ByVal W As Emgu.CV.Mat) As Object

        Dim _tup_1 As New NPSharp.NPEmgu.Gradient(Block)
        Dim gy = NPSharp.NPEmgu.Ravel(_tup_1.Y)
        Dim gx = NPSharp.NPEmgu.Ravel(_tup_1.X)

        Dim G As Emgu.CV.Mat = NPSharp.NPEmgu.VStack(gx, gy).T
        Dim GTWG1 As Emgu.CV.Mat = NPSharp.NPEmgu.dot(gx, gy)
        Dim GTWG = NPSharp.NPEmgu.dot(GTWG1, G)

        Dim _tup_2 As New NPSharp.NPEmgu.eigen(gx)
        Dim w1 = _tup_2.Values
        Dim v = _tup_2.Vectors
        Dim nonzerow = NPSharp.NPEmgu.count_nonzero(NPSharp.NPEmgu.isreal(W))
        Dim nonzerov = NPSharp.NPEmgu.count_nonzero(NPSharp.NPEmgu.isreal(v))

        If nonzerow <> 0 Then W = NP.real(W)
        If nonzerov <> 0 Then v = NP.real(v)

        'Dim idx = W.argsort()() ' [' - 1:]

        'W = W.Row(idx)
        'v = v(":", idx)
        'Dim theta '= Math.Atan2(v(1, 0), v(0, 0))

        'If theta < 0 Then theta = theta + pi

        'Dim lamda = W.Row(0)
        'Dim sqrtlamda1 = Math.Sqrt(W.Row(0)(0))
        'Dim sqrtlamda2 = Math.Sqrt(W.Row(1)(0))

        'Dim u As Double = 0 : If sqrtlamda1 + sqrtlamda2 > 0 Then u = (sqrtlamda1 - sqrtlamda2) / (sqrtlamda1 + sqrtlamda2)
        'Dim angle = Math.Floor(theta / pi * Qangle)

        'Dim strength = 0
        'If lamda < 0.0001 Then
        'ElseIf lamda > 0.001 Then : strength = 2
        'Else : strength = 1
        'End If

        'Dim coherence = 0
        'If u < 0.25 Then
        'ElseIf u > 0.5 Then : coherence = 2
        'Else : coherence = 1
        'End If

        'If angle > 23 Then : angle = 23
        'ElseIf angle < 0 Then : angle = 0
        'End If

        'Return Tuple.Create(angle, strength, coherence)
    End Function
End Module
