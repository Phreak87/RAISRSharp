Imports NPSharp.NPEmgu
Imports NPSharp.NPPublic

Public Class Python
    ''' <summary>
    ''' Rectangle with Changed Parameters.
    ''' Instead of X,Y, Width, Height use
    ''' X,Y, LastXPoint, LastYPoint
    ''' </summary>
    ''' <param name="XMin"></param>
    ''' <param name="YMin"></param>
    ''' <param name="XMax"></param>
    ''' <param name="YMax"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Shared Function Rectangle(ByVal XMin As Integer, ByVal YMin As Integer, ByVal XMax As Integer, ByVal YMax As Integer) As System.Drawing.Rectangle
        Dim n As New Drawing.Rectangle(XMin, YMin, XMax - (XMin - 1), (YMax - (YMin - 1)))
        Return n
    End Function

    Shared Function Slice(ByVal Array As Mat,
                          ByVal _From As String,
                          ByVal _To As String,
                          ByVal _Step As String) As Mat

        If _From = ":" And _To = ":" And _Step = "-1" Then Return SwapRows(Array)
        If _From = ":" And _To = ":" And _Step = "0" Then
            Dim Ret As New Mat
            Emgu.CV.CvInvoke.ExtractChannel(Array.OrgMat, Ret.OrgMat, _Step)
            Return Ret
        End If

        Throw New NotImplementedException

    End Function
    Shared Function Slice(ByVal Array As Mat,
                                ByVal SortRow As String,
                                ByVal SortCol As Mat) As Mat

        ' Sort (Done via Row-Sort):
        ' [ 1 0 ] => [ 1 ]
        '            [ 0 ]

        ' Array
        ' [ 1 2 ] Trans [ 1 3 ]
        ' [ 3 4 ]       [ 2 4 ]

        ' Array in Sort Order (Fast Row-Sort)
        ' [ 1 3 ] Sort [ 1 ] = [ 2 4 ]
        ' [ 2 4 ]      [ 0 ]   [ 1 3 ]

        ' Transpose again
        ' [ 2 4 ] Trans [ 2 1 ]
        ' [ 1 3 ]       [ 4 3 ]

        If SortRow = ":" Then
            Return Slice(Array.T, SortCol).T
        Else
            Throw New Exception
        End If
    End Function
    Shared Function Slice(ByVal Array As Mat,
                          ByVal SortRow As Mat) As Mat
        ' We have a Indexing Element Sort (e.g. [1,0])

        'Dim Test As New Mat
        'Emgu.CV.CvInvoke.MixChannels(Array.OrgMat, Test.OrgMat, {1, 0})

        Dim DESC As New MatrixExplain(SortRow) : If DESC.MatTypA = MatrixExplain.DataType.VectorH Then SortRow = SortRow.T ' Swap to Rows instead of Cols
        Dim DATA As Int16()() = SortRow.NP_GetData
        Dim RET As New Mat()

        For i As Integer = 0 To Array.Height - 1
            RET.PushBack(Array.Row(DATA(i)(0)))
        Next
        Return RET
    End Function

    Private Shared Function SwapRows(ByVal A As Mat) As Mat
        Dim RET As New Mat()
        Emgu.CV.CvInvoke.Flip(A.OrgMat, RET.OrgMat, Emgu.CV.CvEnum.FlipType.Vertical)
        Return RET
    End Function
    Private Shared Function SwapCol(ByVal A As Mat) As Mat
        Dim RET As New Mat()
        Emgu.CV.CvInvoke.Flip(A.OrgMat, RET.OrgMat, Emgu.CV.CvEnum.FlipType.Horizontal)
        Return RET
    End Function
End Class
