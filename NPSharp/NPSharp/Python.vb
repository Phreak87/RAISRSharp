Imports NPSharp.NPEmgu
Imports NPSharp.NPPublic

Public Class Python
    Shared Function Slice(ByVal Array As Mat,
                          ByVal _From As String,
                          ByVal _To As String,
                          ByVal _Step As String) As Mat

        If _From = ":" And _To = ":" And _Step = "-1" Then  Return SwapRows(Array)
        Throw New NotImplementedException

    End Function
    Private Shared Function SwapRows(A As Mat) As Mat
        Dim RET As New Mat(0, A.Cols, A.Depth, A.Channels)
        For i As Integer = A.Rows - 1 To 0 Step -1
            RET.PushBack(A.Row(i))
        Next
        Return RET
    End Function
    Private Shared Function SwapRows(ByVal A As Double()()) As Double()()
        Dim RET As New List(Of Double())
        For Each Line In A
            RET.Insert(0, Line)
        Next
        Return RET.ToArray
    End Function
    Private Shared Function SwapCol(ByVal A As Double()) As Double()
        Dim Ret(A.Length - 1) As Double
        Dim ICol As Integer = 0
        For Col As Integer = A.Length - 1 To 0 Step -1
            Ret(ICol) = A(Col)
            ICol += 1
        Next
        Return Ret
    End Function
End Class
