Imports Emgu.CV
Imports Emgu.CV.Structure
Imports System.Runtime.InteropServices
Imports NPSharp.NPEmgu

Public Module Main
    Sub Main()
        Dim Zeros As Mat = NPEmgu.Zeros(5, 5)
        Dim Ones As Mat = NPEmgu.Ones(5, 5)

        Dim Arr As Integer()() = NPEmgu.GetData(Ones) : Arr(0)(2) = 1000
        NPEmgu.SetData(Ones, Arr)
        Dim Arr2 = NPEmgu.GetData(Ones)

        Dim NewMat As New Mat()
        NewMat.PushBack(Ones)
        NewMat.PushBack(Ones)
        NewMat.PushBack(Ones)
        Emgu.CV.UI.ImageViewer.Show(NewMat.Reshape(3))
    End Sub

End Module
