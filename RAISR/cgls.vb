
Module cgls
    Function cgls(ByVal A As Object, ByVal b As Object) As Object
        Dim _tup_1 = A.shape
        Dim height = _tup_1.Item1
        Dim width = _tup_1.Item2
        Dim x = NPSharp.NPNative.Zeros(height, 1)

        While True
            Dim sumA = A.sum()
            If sumA < 100 Then
                Exit While
            End If

            If NPSharp.NPNative.linalg.det(A) < 1 Then
                A = A + NPSharp.NPNative.eye(height, width) * sumA * 0.000000005
            Else
                'x = NPSharp.NPEmgu.Linalg.Inv(A).Dot(b)
                Exit While
            End If
        End While

        Return x
    End Function
End Module