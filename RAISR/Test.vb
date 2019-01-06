Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.Serialization
Imports System.Collections.Generic
Imports Emgu.CV
Imports System.IO

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
    Public h As Object
    Public weighting As Object

    Sub New()

        If Not IsNothing(args.filter) Then filtername = args.filter()
        h = Pickle.Load(filtername)
        weighting = gaussian2d.gaussian2d({gradientsize, gradientsize}, 2)
        weighting = NP.Diag(NP.Ravel(weighting))

        For Each File In My.Computer.FileSystem.GetFiles(trainpath) : imagelist.Add(File) : Next
        Dim imagecount As Integer = 1

        Dim InterPolate As Object

        For Each image In imagelist

            If image.EndsWith(".jpg") = False Then Continue For
            Dim origin As Mat = Emgu.CV.CvInvoke.Imread(image)
            Dim ycrcvorigin As New Mat : Emgu.CV.CvInvoke.CvtColor(origin, ycrcvorigin, CvEnum.ColorConversion.Bgr2YCrCb)

            Dim grayorigin As Mat = ExtractChannel(ycrcvorigin, 0)
            'grayorigin = (Emgu.CV.CvInvoke.Normalize(grayorigin.astype("float"), Nothing, grayorigin.min() / 255, grayorigin.max() / 255, CvEnum.NormType.MinMax))

            Dim heightLR = grayorigin.Height
            Dim widthLR = grayorigin.Width
            Dim heightgridLR As Object = NP.linspace(0, heightLR - 1, heightLR - 1)
            Dim widthgridLR As Object = NP.linspace(0, widthLR - 1, widthLR - 1)

            Dim bilinearinterp As Object = InterPolate.interp2d(widthgridLR, heightgridLR, grayorigin, kind:="linear")
            Dim heightgridHR As Object = NP.linspace(0, heightLR, heightLR * 2)
            Dim widthgridHR As Object = NP.linspace(0, widthLR, widthLR * 2)
            'Dim upscaledLR As Object = bilinearinterp(widthgridHR, heightgridHR)
            Dim upscaledLR As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Count, heightgridHR.Count), , , CvEnum.Inter.Linear)

            Dim heightHR = upscaledLR.Height
            Dim widthHR = upscaledLR.Width
            'Dim predictHR As Object = NP.Zeros(Tuple.Create(heightHR - 2 * margin, widthHR - 2 * margin))
            Dim predictHR As Mat = Mat.Zeros(heightHR - 2 * margin, widthHR - 2 * margin, CvEnum.DepthType.Cv64F, 1)

            Dim operationcount As Integer = 0
            Dim totaloperations As Integer = (heightHR - 2 * margin) * (widthHR - 2 * margin)

            For row As Integer = 1 To Range(margin, heightHR - margin)
                For col As Integer = 1 To Range(margin, widthHR - margin)
                    operationcount += 1

                    Dim patch As Object ' = 'upscaledLR((row - patchmargin)((row + patchmargin) + 1), (col - patchmargin)((col + patchmargin) + 1)).ravel
                    Dim gradientblock As Object '= upscaledLR((row - gradientmargin)((row + gradientmargin) + 1), (col - gradientmargin)((col + gradientmargin) + 1))

                    Dim HK = hashkey.hashkey(gradientblock, Qangle, weighting)
                    Dim angle = HK.angle
                    Dim strength = HK.strength
                    Dim coherence = HK.coherence

                    Dim pixeltype As Object = (row - margin) Mod R * R + (col - margin) Mod R
                    'predictHR(row - margin, col - margin) = NP.Clip(predictHR.astype("float") * 255.0, 0.0, 255.0)
                Next
            Next

            'predictHR = NP.Clip(predictHR.astype("float") * 255, 0, 255)
            Dim result As Object ' = NP.Zeros(Tuple.Create(heightHR, widthHR, 3))
            Dim y As Object = ExtractChannel(ycrcvorigin, 0)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, y, kind:="linear")
            result(":", ":", 0) = bilinearinterp(widthgridHR, heightgridHR)
            Dim cr As Object = ExtractChannel(ycrcvorigin, 1)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cr, kind:="linear")
            result(":", ":", 1) = bilinearinterp(widthgridHR, heightgridHR)
            Dim cv As Object = ExtractChannel(ycrcvorigin, 2)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cv, kind:="linear")
            result(":", ":", 2) = bilinearinterp(widthgridHR, heightgridHR)
            result = Nothing ' (margin:heightHR-margin,margin:widthHR-margin,0) =  predictHR
            result = Nothing ' cv2.cvtColor(NP.uint8(result), cv2.COLOR_YCrCb2RGB)

            Dim Resu As Mat : Emgu.CV.CvInvoke.CvtColor(result, Resu, CvEnum.ColorConversion.Rgb2Bgr)
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
        Dim Ret As Mat = Mat.Clone
        Emgu.CV.CvInvoke.ExtractChannel(Mat, Ret, Channel)
        Dim Test = Ret.GetData
        Return Ret
    End Function
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
            Dim Data = CType(bf.Deserialize(fs), FilterData)
            fs.Close()
            Return Data
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