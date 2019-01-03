Module Utils

    Function Type2Str(Type As Integer) As String
        Dim r As String = ""

        Dim depth As Emgu.CV.CvEnum.IplDepth

        '    uchar depth = type & CV_MAT_DEPTH_MASK;
        '    uchar chans = 1 + (type >> CV_CN_SHIFT);

        Select Case depth
            Case Emgu.CV.CvEnum.IplDepth.IplDepth_8U : r = "8U"
            Case Emgu.CV.CvEnum.IplDepth.IplDepth_8S : r = "8S"
            Case Emgu.CV.CvEnum.IplDepth.IplDepth16U : r = "16U"
            Case Emgu.CV.CvEnum.IplDepth.IplDepth16S : r = "16S"
            Case Emgu.CV.CvEnum.IplDepth.IplDepth32S : r = "32S"
            Case Emgu.CV.CvEnum.IplDepth.IplDepth32F : r = "32F"
            Case Emgu.CV.CvEnum.IplDepth.IplDepth64F : r = "64F"
            Case Else : r = "User"
        End Select

        '    r += "C";
        '    r += (chans+'0');

        Return r
    End Function

    Sub DebugMat(m As Emgu.CV.Mat)
        Console.WriteLine("Cols:" & m.Cols & ", Rows:" & m.Rows)
        Console.WriteLine("m:" & m.ToString)
        Console.ReadLine()
    End Sub
    Function MatIsEqual(mat1 As Emgu.CV.Mat, mat2 As Emgu.CV.Mat) As Boolean
        If IsNothing(mat1) And IsNothing(mat2) Then
            Return True
        End If

        If mat1.Rows <> mat2.Rows Or mat1.Cols <> mat2.Cols Then
            Return False
        End If

        Dim Diff As New Emgu.CV.Mat
        Emgu.CV.CvInvoke.Compare(mat1, mat2, Diff, Emgu.CV.CvEnum.CmpType.NotEqual)
        Dim nz As Integer = Emgu.CV.CvInvoke.CountNonZero(Diff)
        Return nz = 0
    End Function
    Sub readListofImage(dirPath As String,
                        ByRef imageMatList As List(Of Emgu.CV.Mat),
                        ByRef ImageNameList As List(Of String))

        For Each File In My.Computer.FileSystem.GetFiles(dirPath)
            ImageNameList.Add(File)
            imageMatList.Add(New Emgu.CV.Mat(File))
        Next

    End Sub

End Module
