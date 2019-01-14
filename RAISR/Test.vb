Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.Serialization
Imports System.Collections.Generic
Imports NPSharp.NPEmgu
Imports Emgu.CV
Imports System.IO
Imports System.Drawing

Class test

    Public args = New gettestargs()
    Public R As Integer = 2
    Public patchsize As Integer = 11
    Public gradientsize As Integer = 9
    Public Qangle As Integer = 24
    Public Qstrength As Integer = 3
    Public Qcoherence As Integer = 3
    Public trainpath As String = "test"
    Public maxblocksize As Integer = Math.Max(patchsize, gradientsize)
    Public margin As Integer = Math.Floor(maxblocksize / 2)
    Public patchmargin As Integer = Math.Floor(patchsize / 2)
    Public gradientmargin As Integer = Math.Floor(gradientsize / 2)
    Public filtername As String = "filters\filter_BSDS500"
    Public imagelist As New List(Of String)
    Public h As Mat()()()()
    Public weighting As Object

    Sub New()

        If Not IsNothing(args.filter) Then filtername = args.filter()
        'h = Pickle.Load(filtername) ' Blob needs to be converted!
        weighting = gaussian2d.gaussian2d({gradientsize, gradientsize}, 2)
        weighting = NP.Diag(NP.Ravel(weighting))
        Dim WMat As Emgu.CV.Mat = NPSharp.NPEmgu.MatFromArray(weighting)

        For Each File In My.Computer.FileSystem.GetFiles(trainpath)
            If File.EndsWith(".jpg") = True Then imagelist.Add(File)
        Next

        Dim imagecount As Integer = 1
        Dim InterPolate As Object = Nothing

        For Each image In imagelist
            ' ------------------------------------------
            ' Part 1 - Read and Convert to Grayscale
            ' ------------------------------------------
            Dim origin As Mat = Emgu.CV.CvInvoke.Imread(image)
            Dim ycrcvorigin As Mat = NPSharp.NPEmgu.cvtColor(origin, CvEnum.ColorConversion.Bgr2YCrCb)
            Dim grayorigin As Mat = ExtractChannel(ycrcvorigin, 0)

            ' ------------------------------------------
            ' Part 2 - Normalisieren
            ' ------------------------------------------
            Dim minorig() As Double = {0} : Dim maxorig() As Double = {0}
            grayorigin.MinMax(minorig, maxorig, Nothing, Nothing)
            minorig = {minorig(0) / 255} : maxorig = {maxorig(0) / 255}

            'grayorigin = (Emgu.CV.CvInvoke.Normalize(grayorigin.astype("float"), Nothing, grayorigin.min() / 255, grayorigin.max() / 255, CvEnum.NormType.MinMax))
            Emgu.CV.CvInvoke.Normalize(NPSharp.NPEmgu.ConvertTo(grayorigin, CvEnum.DepthType.Cv64F),
                                       grayorigin,
                                       minorig(0),
                                       maxorig(0),
                                       CvEnum.NormType.MinMax)
            ' Debug(grayorigin) => OK

            ' ------------------------------------------
            ' Part 3 - Low Resolution Grid
            ' ------------------------------------------
            Dim heightLR = grayorigin.Height
            Dim widthLR = grayorigin.Width
            Dim heightgridLR = New Matrix(Of Single)(NP.linspace(0, heightLR - 1, heightLR))
            Dim widthgridLR = New Matrix(Of Single)(NP.linspace(0, widthLR - 1, widthLR))

            ' ------------------------------------------
            ' Part 4 - biliearinterp ist eine Funktionsklasse!
            ' ------------------------------------------
            Dim bilinearinterp As NPSharp.NPEmgu.Interp2d =
                New NPSharp.NPEmgu.Interp2d(widthgridLR,
                                            heightgridLR,
                                            grayorigin,
                                            CvEnum.Inter.Linear)

            ' ------------------------------------------
            ' Part 5 - High Resolution Grid
            ' ------------------------------------------
            Dim heightgridHR As Single() = NP.linspace(0, heightLR - 0.5, heightLR * 2)
            Dim widthgridHR As Single() = NP.linspace(0, widthLR - 0.5, widthLR * 2)
            Dim upscaledLR As Mat = bilinearinterp.Run(widthgridHR, heightgridHR)

            ' ------------------------------------------
            ' Part 6 - Calculate predictHR pixels
            ' ------------------------------------------
            Dim heightHR = upscaledLR.Height
            Dim widthHR = upscaledLR.Width
            Dim predictHR As Mat = Mat.Zeros(heightHR - 2 * margin, widthHR - 2 * margin, CvEnum.DepthType.Cv64F, 1)

            Dim operationcount As Integer = 0
            Dim totaloperations As Integer = (heightHR - 2 * margin) * (widthHR - 2 * margin)

            For row As Integer = margin To heightHR - margin
                For col As Integer = margin To widthHR - margin
                    operationcount += 1

                    Dim Patch As Emgu.CV.Mat = New Emgu.CV.Mat(upscaledLR, New Rectangle(row - patchmargin, col - patchmargin, row + patchmargin + 1, col + patchmargin + 1)).Clone
                    Patch = NPSharp.NPEmgu.Ravel(Patch)

                    Dim gradientblock As Mat = New Emgu.CV.Mat(upscaledLR, New Rectangle(row - gradientmargin,
                                                                                         col - gradientmargin,
                                                                                         row + gradientmargin,
                                                                                         col + gradientmargin)).Clone

                    Dim HK = hashkey.hashkey(gradientblock, Qangle, WMat)
                    Dim angle = HK.angle
                    Dim strength = HK.strength
                    Dim coherence = HK.coherence

                    Dim pixeltype As Object = (row - margin) Mod R * R + (col - margin) Mod R

                    ' predictHR(row - margin, col - margin) = patch.dot(h{angle,strength,coherence,pixeltype}) => same as 3 Lines ...
                    Dim PatchMat As Mat = h(0)(0)(0)(0)
                    Dim PatchMCV As New Emgu.CV.Structure.MCvScalar(PatchMat.Dot(PatchMat))
                    predictHR.Row(row - margin).Col(col - margin).SetTo(PatchMCV)

                Next
            Next

            Dim Test2 As Mat = h(0)(0)(0)(0)

            'predictHR = NP.Clip(predictHR.astype("float") * 255, 0, 255)

            'Dim result As Object = NP.Zeros(heightHR, widthHR, 3) => Same as ...
            Dim Result As Mat = Emgu.CV.Mat.Zeros(heightHR, widthHR, CvEnum.DepthType.Cv64F, 3)

            Dim y As Object = ExtractChannel(ycrcvorigin, 0)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, y, kind:="linear")
            'result(":", ":", 0) = bilinearinterp(widthgridHR, heightgridHR) => Test following 2 Lines
            Dim result1 As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Length, heightgridHR.Length), , , CvEnum.Inter.Linear)
            Result.Row(0).Col(0).Data(0).SetTo(result1)
            Dim cr As Object = ExtractChannel(ycrcvorigin, 1)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cr, kind:="linear")
            'result(":", ":", 1) = bilinearinterp(widthgridHR, heightgridHR) => Test following 2 Lines
            Dim result2 As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Length, heightgridHR.Length), , , CvEnum.Inter.Linear)
            Result.Row(0).Col(0).Data(1).SetTo(result2)
            Dim cv As Object = ExtractChannel(ycrcvorigin, 2)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cv, kind:="linear")
            'result(":", ":", 2) = bilinearinterp(widthgridHR, heightgridHR) => Test following 2 Lines
            Dim result3 As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Length, heightgridHR.Length), , , CvEnum.Inter.Linear)
            Result.Row(0).Col(0).Data(2).SetTo(result3)

            Result.Row(heightHR - margin).Col(widthHR - margin).Data(0).setto(predictHR)
            Dim ResU8 As Mat : Result.ConvertTo(ResU8, CvEnum.DepthType.Cv8U)
            Emgu.CV.CvInvoke.CvtColor(ResU8, Result, CvEnum.ColorConversion.YCrCb2Rgb)

            Dim Resu As Mat : Emgu.CV.CvInvoke.CvtColor(Result, Resu, CvEnum.ColorConversion.Rgb2Bgr)
            Emgu.CV.CvInvoke.Imwrite("results/" & image & "_result.bmp", Resu)
            imagecount += 1

            If args.plot Then
                'Dim fig As Object '= plt.figure()
                'Dim ax As Object '= fig.add_subplot(1, 4, 1)
                'ax = fig.add_subplot(1, 4, 2)
                'ax = fig.add_subplot(1, 4, 3)
                'ax = fig.add_subplot(1, 4, 4)
            End If

        Next

    End Sub

    Function ExtractChannel(Mat As Mat, Channel As Integer) As Mat
        Dim Ret As New Mat
        Emgu.CV.CvInvoke.ExtractChannel(Mat, Ret, Channel)
        Return Ret
    End Function
    Sub Debug(A As Mat)
        Dim Test = NPSharp.NPEmgu.GetData(A)
        Emgu.CV.CvInvoke.Imshow("Test", A)
        Emgu.CV.CvInvoke.WaitKey()
    End Sub
End Class

<Serializable()>
Class FilterData

End Class

Class Pickle
    Shared Function Load(File As String) As FilterData
        If My.Computer.FileSystem.FileExists(File) Then
            Dim fs As Stream = New FileStream(File, FileMode.Open)
            Dim bf As BinaryFormatter = New BinaryFormatter()
            Dim fd As New FilterData
            'Dim Data = CType(bf.Deserialize(fs), FilterData)
            fs.Close()
            'Return Data
        End If
        Return Nothing
    End Function
    Shared Sub Save(File As String, Data As FilterData)
        If My.Computer.FileSystem.FileExists(File) = True Then
            My.Computer.FileSystem.DeleteFile(File)
        End If
        Dim fs As Stream = New FileStream(File, FileMode.Create)
        Dim bf As BinaryFormatter = New BinaryFormatter()
        bf.Serialize(fs, Data)
        fs.Close()
    End Sub

End Class