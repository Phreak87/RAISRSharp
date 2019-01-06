
Module cgls
    Function cgls(ByVal A As Object, ByVal b As Object) As Object
        Dim _tup_1 = A.shape
        Dim height = _tup_1.Item1
        Dim width = _tup_1.Item2
        Dim x = NP.Zeros(height)

        While True
            Dim sumA = A.sum()
            If sumA < 100 Then
                Exit While
            End If

            If NP.linalg.det(A) < 1 Then
                A = A + NP.eye(height, width) * sumA * 0.000000005
            Else
                x = NP.linalg.inv(A).dot(b)
                Exit While
            End If
        End While

        Return x
    End Function
End Module