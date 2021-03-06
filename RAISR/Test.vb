﻿Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.Serialization
Imports System.Collections.Generic
Imports NPSharp.NPEmgu
Imports NPSharp.NPPublic
Imports System.IO
Imports System.Drawing

Class test

#Region "Test Arrays"
    ' Normal Numerics
    Dim _1 As Integer = 2
    Dim _1A() As Integer = {2}
    Dim _1B()() As Integer = {{2}.ToArray}
    ' Vectors
    Dim _1x3 As Integer()() = {{2, 0, 1}.ToArray}
    Dim _1x4 As Integer()() = {{2, 0, 1, 3}.ToArray}
    Dim _3x1 As Integer()() = {{2}.ToArray, {0}.ToArray, {1}.ToArray}
    Dim _4x1 As Integer()() = {{2}.ToArray, {0}.ToArray, {1}.ToArray, {3}.ToArray}
    ' Matrixes
    Dim _2x2 As Integer()() = {{1, 2}.ToArray, {3, 4}.ToArray}
    Dim _2x3 As Integer()() = {{0, 1, 2}.ToArray, {4, 5, 3}.ToArray}
    Dim _3x2 As Integer()() = {{0, 1}.ToArray, {8, 7}.ToArray, {4, 5}.ToArray}
    Dim _3x3 As Integer()() = {{5, 7, 1}.ToArray, {2, 9, 8}.ToArray, {8, 3, 4}.ToArray}
    Dim _4x3 As Integer()() = {{5, 7, 1}.ToArray, {2, 9, 8}.ToArray, {8, 3, 4}.ToArray, {8, 3, 4}.ToArray}
    Dim _3x4 As Integer()() = {{5, 7, 1, 3}.ToArray, {2, 9, 8, 5}.ToArray, {8, 3, 4, 0}.ToArray}

    ' Normal Numerics
    Dim MatSort0_1 As Mat = MatFromArray({0, 1})
    Dim MatSort1_0 As Mat = MatFromArray({1, 0})

    Dim Mat_1 As Mat = MatFromArray(_1)
    Dim Mat_1A As Mat = MatFromArray(_1A)
    Dim Mat_1B As Mat = MatFromArray(_1B)
    ' Vectors
    Dim Mat_1x3 As Mat = MatFromArray(_1x3)
    Dim Mat_3x1 As Mat = MatFromArray(_3x1)
    Dim Mat_1x4 As Mat = MatFromArray(_1x4)
    Dim Mat_4x1 As Mat = MatFromArray(_4x1)
    ' Matrixes
    Dim Mat_2x2 As Mat = MatFromArray(_2x2)
    Dim Mat_2x3 As Mat = MatFromArray(_2x3)
    Dim Mat_3x2 As Mat = MatFromArray(_3x2)
    Dim Mat_3x3 As Mat = MatFromArray(_3x3)
    Dim Mat_4x3 As Mat = MatFromArray(_4x3)
    Dim Mat_3x4 As Mat = MatFromArray(_3x4)
#End Region

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

    Sub TestArrays()
        Dim Res1 = New NPSharp.NPPublic.MatrixExplain(_1B, _3x3)
        Dim Res2 = Mat_3x3.NP_ArgSort
        Dim Res3 = NPSharp.Python.Slice(Res2, ":", ":", "-1")
    End Sub
    Sub TestMul()
        Dim A As Mat
        Dim B As MatrixExplain
        B = New MatrixExplain(Mat_1, Mat_1) : A = Mat_1 * Mat_1 ' N_N (2 * 2)
        B = New MatrixExplain(Mat_3x1, Mat_3x1) : A = Mat_3x1 * Mat_3x1 'Case CalcType.VV_VV_SYM (2,0,1 * 2,0,1)    => OK
        B = New MatrixExplain(Mat_1x3, Mat_1x3) : A = Mat_1x3 * Mat_1x3 'Case CalcType.VH_VH_SYM (2,0,1 * 2,0,1)    => OK
        B = New MatrixExplain(Mat_3x1, Mat_1x3) : A = Mat_3x1 * Mat_1x3 'Case CalcType.VV_VH_SYM (2,0,1 * 2,0,1)    => OK
        B = New MatrixExplain(Mat_1x3, Mat_3x1) : A = Mat_1x3 * Mat_3x1 'Case CalcType.VH_VV_SYM (2,0,1 * 2,0,1)    => OK
        B = New MatrixExplain(Mat_3x1, Mat_1x4) : A = Mat_3x1 * Mat_1x4 'Case CalcType.VV_VH_ASYM (2,0,1 * 2,0,1,3) => OK
        B = New MatrixExplain(Mat_1x3, Mat_4x1) : A = Mat_1x3 * Mat_4x1 'Case CalcType.VH_VV_ASYM (2,0,1 * 2,0,1,3) => OK
        B = New MatrixExplain(Mat_3x3, Mat_1x3) : A = Mat_3x3 * Mat_1x3 'Case CalcType.M_VH_SYM                     => OK
        B = New MatrixExplain(Mat_3x3, Mat_3x1) : A = Mat_3x3 * Mat_3x1 'Case CalcType.M_VV_SYM                     => OK
        B = New MatrixExplain(Mat_3x3, Mat_1) : A = Mat_3x3 * Mat_1 '    Case CalcType.M_N                          => OK
        B = New MatrixExplain(Mat_3x3, Mat_3x3) : A = Mat_3x3 * Mat_3x3 'Case CalcType.M_M_SYM                      => OK
        B = New MatrixExplain(Mat_3x3, Mat_3x4) : A = Mat_3x3 * Mat_3x4 'Case CalcType.M_M_ASYM                     => OK
        B = New MatrixExplain(Mat_3x1, Mat_4x1) 'A = Mat_3x1 * Mat_4x1 'Case CalcType.VV_VV_ASYM                    => Nicht möglich
        B = New MatrixExplain(Mat_1x3, Mat_1x4) 'A = Mat_1x3 * Mat_1x4 'Case CalcType.VH_VH_ASYM                    => Nicht möglich
        B = New MatrixExplain(Mat_3x3, Mat_4x3) 'A = Mat_3x3 * Mat_3x4 'Case CalcType.M_M_ASYM                      => Nicht möglich
    End Sub
    Sub TestDot()
        ' Internet Example => OK
        Dim A1 = New Mat({{1, 2}.ToArray,
                         {3, 4}.ToArray})
        Dim B1 = New Mat({{11, 12}.ToArray,
                         {13, 14}.ToArray})
        Dim C1 As Mat = A1.Dot(B1, True)

        ' My Example => OK
        Dim A2 = New Mat({{0.0, 2.2}.ToArray,
                          {0.0, 2.2}.ToArray})
        Dim B2 = New Mat({{2.2, 0.0}.ToArray,
                          {0.0, 2.2}.ToArray})
        Dim C2 As Mat = A2.Dot(B2, True)
    End Sub
    Sub TestSort()
        Dim _3x3M As Mat = Mat_3x3.NP_ArgSort
        Dim _3x3T As Double()() = _3x3M.NP_GetData
        Dim _3x3I As Double()() = _3x3M.PY_Slice(":", ":", "-1").NP_GetData
    End Sub

    Sub New()
        TestDot()

        If Not IsNothing(args.filter) Then filtername = args.filter()
        h = Pickle.Load(filtername).h  ' Blob needs to be converted!

        Dim weightingO As Mat = gaussian2d.gaussian2d({gradientsize, gradientsize}, 2)
        Dim weightingR As Mat = weightingO.NP_Ravel
        Dim weighting As Mat = NPSharp.NPEmgu.Diag(weightingR)

        For Each File In My.Computer.FileSystem.GetFiles(trainpath)
            If File.EndsWith(".jpg") = True Or
                File.EndsWith(".tif") = True Or
                File.EndsWith(".jpeg") = True Then
                imagelist.Add(File)
            End If
        Next

        Dim imagecount As Integer = 1
        Dim InterPolate As Object = Nothing

        For Each image In imagelist
            ' ------------------------------------------
            ' Part 1 - Read and Convert to Grayscale
            ' ------------------------------------------
            Dim origin As Mat = New Mat(image)
            Dim ycrcvorigin As Mat = origin.CvtColor(Emgu.CV.CvEnum.ColorConversion.Bgr2YCrCb)
            Dim grayorigin As Mat = NPSharp.Python.Slice(ycrcvorigin, ":", ":", "0")

            ' ------------------------------------------
            ' Part 2 - Normalisieren
            ' ------------------------------------------
            Dim minorig As Double = grayorigin.NP_Min / 255
            Dim maxorig As Double = grayorigin.NP_Max / 255
            grayorigin = grayorigin.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv32F).Normalize(minorig, maxorig, Emgu.CV.CvEnum.NormType.MinMax)

            ' ------------------------------------------
            ' Part 3 - Low Resolution Grid
            ' ------------------------------------------
            Dim heightLR = grayorigin.Height
            Dim widthLR = grayorigin.Width
            Dim heightgridLR = Mat.NP_linspace(0, heightLR - 1, heightLR)
            Dim widthgridLR = Mat.NP_linspace(0, widthLR - 1, widthLR)

            ' ------------------------------------------
            ' Part 4 - biliearinterp ist eine Funktionsklasse!
            ' ------------------------------------------
            Dim bilinearinterp As NPSharp.NPEmgu.Interp2d =
                New NPSharp.NPEmgu.Interp2d(widthgridLR,
                                            heightgridLR,
                                            grayorigin,
                                            Emgu.CV.CvEnum.Inter.Linear)

            ' ------------------------------------------
            ' Part 5 - High Resolution Grid
            ' ------------------------------------------
            Dim heightgridHR As Mat = Mat.NP_linspace(0, heightLR - 0.5, heightLR * 2)
            Dim widthgridHR As Mat = Mat.NP_linspace(0, widthLR - 0.5, widthLR * 2)
            Dim upscaledLR As Mat = bilinearinterp.Run(widthgridHR, heightgridHR)

            ' ------------------------------------------
            ' Part 6 - Calculate predictHR pixels
            ' ------------------------------------------
            Dim heightHR = upscaledLR.Height
            Dim widthHR = upscaledLR.Width
            Dim predictHR As Mat = Mat.Zeros(heightHR - 2 * margin, widthHR - 2 * margin, Emgu.CV.CvEnum.DepthType.Cv32F)

            Dim operationcount As Integer = 0
            Dim totaloperations As Integer = (heightHR - 2 * margin) * (widthHR - 2 * margin)

            For row As Integer = margin To heightHR - margin
                For col As Integer = margin To widthHR - margin
                    operationcount += 1

                    If operationcount Mod 100 = 0 Then Console.WriteLine(operationcount & "/" & totaloperations)
                    ' ------------------------------------------
                    ' Part 7 - Patch
                    ' ------------------------------------------
                    Dim Patch As Mat = New Mat(upscaledLR,
                                               NPSharp.Python.Rectangle(
                                                   row - patchmargin,
                                                   row + patchmargin + 1,
                                                   col - patchmargin,
                                                   col + patchmargin + 1)).Clone
                    Patch = Patch.NP_Ravel

                    Dim gradientblock = New Mat(upscaledLR,
                                                NPSharp.Python.Rectangle(
                                                    row - gradientmargin,
                                                    row + gradientmargin + 1,
                                                    col - gradientmargin,
                                                    col + gradientmargin + 1)).Clone

                    Dim HK = New hashkey(gradientblock, Qangle, weighting)
                    Dim angle = HK.angle
                    Dim strength = HK.strength
                    Dim coherence = HK.coherence

                    gradientblock.Dispose() : HK = Nothing

                    Dim pixeltype As Integer = (row - margin) Mod R * R + (col - margin) Mod R

                    Dim PatchMat As Mat ' = h(angle)(strength)(coherence)(pixeltype)
                    Dim PatchMCV As Mat '= New Emgu.CV.Structure.MCvScalar(PatchMat.Dot(PatchMat, True))
                    'predictHR.Row(row - margin).Col(col - margin).SetTo(PatchMCV)

                Next
            Next

            predictHR = NPSharp.NPEmgu.Clip(Multiply(NPSharp.NPEmgu.AsType(predictHR, DataType._Float), 255), 0, 255) ' predictHR.astype("float") * 255

            Dim Result = Mat.Zeros(heightHR, widthHR, 3)

            Dim y As Object = ExtractChannel(ycrcvorigin, 0)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, y, kind:="linear")
            Dim Result1 = bilinearinterp.Run(widthgridHR, heightgridHR)
            'result(":", ":", 0) = bilinearinterp(widthgridHR, heightgridHR) => Test following 2 Lines
            'Dim result1 As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Width, heightgridHR.Height), , , Emgu.CV.CvEnum.Inter.Linear)
            Result.Row(0).Col(0).Data(0).SetTo(result1)
            Dim cr As Object = ExtractChannel(ycrcvorigin, 1)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cr, kind:="linear")
            'result(":", ":", 1) = bilinearinterp(widthgridHR, heightgridHR) => Test following 2 Lines
            Dim result2 As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Width, heightgridHR.Height), , , Emgu.CV.CvEnum.Inter.Linear)
            Result.Row(0).Col(0).Data(1).SetTo(result2)
            Dim cv As Object = ExtractChannel(ycrcvorigin, 2)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cv, kind:="linear")
            'result(":", ":", 2) = bilinearinterp(widthgridHR, heightgridHR) => Test following 2 Lines
            Dim result3 As New Mat : Emgu.CV.CvInvoke.Resize(grayorigin, upscaledLR, New System.Drawing.Size(widthgridHR.Width, heightgridHR.Height), , , Emgu.CV.CvEnum.Inter.Linear)
            Result.Row(0).Col(0).Data(2).SetTo(result3)

            Result.Row(heightHR - margin).Col(widthHR - margin).Data(0).setto(predictHR)
            Dim ResU8 As Mat : Result.ConvertTo(ResU8, Emgu.CV.CvEnum.DepthType.Cv8U)
            Emgu.CV.CvInvoke.CvtColor(ResU8, Result, Emgu.CV.CvEnum.ColorConversion.YCrCb2Rgb)

            Dim Resu As Mat : Emgu.CV.CvInvoke.CvtColor(Result, Resu, Emgu.CV.CvEnum.ColorConversion.Rgb2Bgr)
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

    Function ExtractChannel(ByVal Mat As Mat, ByVal Channel As Integer) As Mat
        Dim Ret As New Mat
        Emgu.CV.CvInvoke.ExtractChannel(Mat.OrgMat, Ret.OrgMat, Channel)
        Return Ret
    End Function

End Class

Class Pickle
    Shared Function Load(ByVal File As String) As PickleData
        If My.Computer.FileSystem.FileExists(File) Then
            Dim fs As Stream = New FileStream(File, FileMode.Open)
            Dim bf As BinaryFormatter = New BinaryFormatter()
            'Dim Data = CType(bf.Deserialize(fs), FilterData)
            fs.Close()
            'Return Data
        End If
        Return New PickleData
    End Function
    Shared Sub Save(ByVal File As String, ByVal Data As PickleData)
        If My.Computer.FileSystem.FileExists(File) = True Then
            My.Computer.FileSystem.DeleteFile(File)
        End If
        Dim fs As Stream = New FileStream(File, FileMode.Create)
        Dim bf As BinaryFormatter = New BinaryFormatter()
        bf.Serialize(fs, Data)
        fs.Close()
    End Sub
    Shared Sub Dump(ByVal A As Object, ByVal B As Object)

    End Sub
End Class

<Serializable()>
Class PickleData
    Property h As Mat()()()() = {{{{New Mat(30, 30, Emgu.CV.CvEnum.DepthType.Cv32S, 3)}.ToArray}.ToArray}.ToArray}.ToArray
    Sub New()

    End Sub
End Class