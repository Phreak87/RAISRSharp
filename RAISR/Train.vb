﻿Imports NPSharp.NPEmgu
Imports System.Collections.Generic
Imports System

Class train

    Public args As Object ' = gettrainargs()
    Public R As Integer = 2
    Public patchsize As Integer = 11
    Public gradientsize As Integer = 9
    Public Qangle As Integer = 24
    Public Qstrength As Integer = 3
    Public Qcoherence As Integer = 3
    Public trainpath As String = "train"
    Public maxblocksize As Object = Math.Max(patchsize, gradientsize)
    Public margin As Object = Math.Floor(maxblocksize / 2)
    Public patchmargin As Object = Math.Floor(patchsize / 2)
    Public gradientmargin As Object = Math.Floor(gradientsize / 2)
    Public Q = Mat.Zeros(Qangle, Qstrength, Qcoherence, R * R, patchsize * patchsize, patchsize * patchsize)
    Public V = Mat.Zeros(Qangle, Qstrength, Qcoherence, R * R, patchsize * patchsize)
    Public h = Mat.Zeros(Qangle, Qstrength, Qcoherence, R * R, patchsize * patchsize)
    'Public Q As RAISR.FilterData = Pickle.Load(fp)
    'Public V As RAISR.FilterData = Pickle.Load(fp)
    Public weightingA As Mat = gaussian2d.gaussian2d({gradientsize, gradientsize}, 2)
    Public weighting As Mat = NPSharp.NPEmgu.Diag(weightingA.NP_Ravel)
    Public imagelist As Object = New List(Of Object)()

    Sub New()

        For Each File In My.Computer.FileSystem.GetFiles(trainpath)
            If File.EndsWith(".jpg") = True Or
                File.EndsWith(".tif") = True Or
                File.EndsWith(".jpeg") = True Then
                imagelist.Add(File)
            End If
        Next
        Dim imagecount As Integer = 1

        For Each Image As String In imagelist
            ' ------------------------------------------
            ' Part 1 - Read and Convert to Grayscale
            ' ------------------------------------------
            Dim origin = New Mat(CStr(Image))
            Dim ycrcvorigin As Mat = origin.CvtColor(Emgu.CV.CvEnum.ColorConversion.Bgr2YCrCb)
            Dim grayorigin As Mat = NPSharp.Python.Slice(ycrcvorigin, ":", ":", "0")

            ' ------------------------------------------
            ' Part 2 - Normalisieren
            ' ------------------------------------------
            grayorigin = grayorigin.NP_AsType(Emgu.CV.CvEnum.DepthType.Cv64F).Normalize(
                grayorigin.NP_Min / 255,
                grayorigin.NP_Max / 255,
                Emgu.CV.CvEnum.NormType.MinMax)

            ' ------------------------------------------
            ' Part 3 - Resize
            ' ------------------------------------------
            Dim height = grayorigin.Height
            Dim width = grayorigin.Width
            Dim LR As Mat = Transform.Resize(grayorigin,
                                             NPSharp.Python.Size(Math.Floor((height + 1) / 2),
                                                                 Math.Floor((width + 1) / 2)),
                                                                 Emgu.CV.CvEnum.BorderType.Reflect,
                                                                 False)

            ' ------------------------------------------
            ' Part 4 - Upscale
            ' ------------------------------------------
            height = LR.Height : width = LR.Width
            Dim heightgrid As Mat = Mat.NP_linspace(0, height - 1, height)
            Dim widthgrid As Mat = Mat.NP_linspace(0, width - 1, width)
            Dim bilinearinterp = New NPSharp.NPEmgu.Interp2d(widthgrid, heightgrid, LR, Emgu.CV.CvEnum.Inter.Linear)
            heightgrid = Mat.NP_linspace(0, height - 1, height * 2 - 1)
            widthgrid = Mat.NP_linspace(0, width - 1, width * 2 - 1)

            Dim upscaledLR As Mat = bilinearinterp.Run(widthgrid, heightgrid)
            height = upscaledLR.Height
            width = upscaledLR.Width

            Dim OperationCount As Integer = 0
            Dim TotalOperations As Integer = (height - 2 * margin) * (width - 2 * margin)

            For row = margin To height - margin
                For col = margin To width - margin
                    OperationCount += 1
                    Dim Patch As Mat = New Mat(upscaledLR,
                                               NPSharp.Python.Rectangle(
                                                   row - patchmargin,
                                                   row + patchmargin + 1,
                                                   col - patchmargin,
                                                   col + patchmargin + 1)).Clone
                    Patch = Patch.Matrix(Patch.NP_Ravel)

                    Dim gradientblock As Mat = New Mat(upscaledLR, NPSharp.Python.Rectangle(
                                                       row - gradientmargin,
                                                       (row + gradientmargin) + 1,
                                                       col - gradientmargin,
                                                       (col + gradientmargin) + 1))

                    Dim HK = New hashkey(gradientblock, Qangle, weighting)
                    Dim angle = HK.angle
                    Dim strength = HK.strength
                    Dim coherence = HK.coherence

                    Dim pixeltype As Integer = (row - margin) Mod R * R + (col - margin) Mod R
                    Dim pixelHR As Mat = New Mat(grayorigin.NP_GetValue(row, col))

                    Dim ATA As Mat = Patch.T.Dot(Patch, True)
                    Dim ATb As Mat = Patch.T.Dot(pixelHR, True)
                    ' ATb = ATb.array(ATb).ravel()

                    Q(angle)(strength)(coherence)(pixeltype) = ATA.NP_GetData
                    V(angle)(strength)(coherence)(pixeltype) = ATb.NP_GetData(0)

                Next
            Next
            imagecount += 1
        Next

        MsgBox(1)
        ' sys.stdout.flush()
        ' Pickle.Dump(Q, fp)
        ' Pickle.Dump(V, fp)
        'sys.stdout.flush()
        'rotate(j, i) = 1
        'flip(k, i) = 1
        'P(":", ":", i - 1) = np.linalg.matrix_power(flip, i2).dot(np.linalg.matrix_power(rotate, i1))
        'Qextended(newangleslot, strength, coherence, pixeltype) = newQ
        'Vextended(newangleslot, strength, coherence, pixeltype) = newV
        'sys.stdout.flush()
        'sys.stdout.flush()
        'h(angle, strength, coherence, pixeltype) = cgls(Q(angle, strength, coherence, pixeltype), V(angle, strength, coherence, pixeltype))
        'pickle.dump(h, fp)
        'filterplot(h, R, Qangle, Qstrength, Qcoherence, patchsize)
    End Sub

    Sub Start()

        'Dim totaloperations As Object = (height - 2 * margin) * (width - 2 * margin)
        'operationcount = 1
        'Dim patch As Mat = upscaledLR((row - patchmargin)((row + patchmargin) + 1), (col - patchmargin)((col + patchmargin) + 1))
        'patch = NPSharp.NPEmgu.matrix(patch.ravel())
        'Dim gradientblock As Object = upscaledLR((row - gradientmargin)((row + gradientmargin) + 1), (col - gradientmargin)((col + gradientmargin) + 1))
        'Dim pixeltype As Object = (row - margin) Mod R * R + (col - margin) Mod R
        'Dim pixelHR As Object = grayorigin(row, col)
        'Dim ATA As Object = NPSharp.NPEmgu.dot(patch.T, patch)
        'Dim ATb As Object = NPSharp.NPEmgu.dot(patch.T, pixelHR)
        'ATb = NPSharp.NPNative.array(ATb).ravel()
        'imagecount = 1
        'Dim P As Object = NPSharp.NPEmgu.Zeros(Tuple.Create(patchsize * patchsize, patchsize * patchsize, 7))
        'Dim rotate As Object = NPSharp.NPEmgu.Zeros(Tuple.Create(patchsize * patchsize, patchsize * patchsize))
        'Dim flip As Object = NPSharp.NPEmgu.Zeros(Tuple.Create(patchsize * patchsize, patchsize * patchsize))
        'Dim i1 As Object = i Mod patchsize
        'Dim i2 As Object = Math.Floor(i / patchsize)
        'Dim j As Object = patchsize * patchsize - patchsize + i2 - patchsize * i1
        'Dim k As Object = patchsize * (i2 + 1) - i1 - 1
        'Dim i1 As Object = i Mod 4
        'Dim i2 As Object = floor(i / 4)
        'Dim Qextended As Object = NPSharp.NPEmgu.Zeros(Tuple.Create(Qangle, Qstrength, Qcoherence, R * R, patchsize * patchsize, patchsize * patchsize))
        'Dim Vextended As Object = NPSharp.NPEmgu.Zeros(Tuple.Create(Qangle, Qstrength, Qcoherence, R * R, patchsize * patchsize))
        'Dim m1 As Object = m Mod 4
        'Dim m2 As Object = Math.Floor(m / 4)
        'Dim newangleslot As Object = angle
        'newangleslot = Qangle - angle - 1
        'newangleslot = Convert.ToInt32(newangleslot - Qangle / 2 * m1)
        'newangleslot = Qangle
        'Dim newQ As Object = P(":", ":", m - 1).T.dot(Q(angle, strength, coherence, pixeltype)).dot(P(":", ":", m - 1))
        'Dim newV As Object = P(":", ":", m - 1).T.dot(V(angle, strength, coherence, pixeltype))
        'Dim Q As Object = Qextended
        'Dim V As Object = Vextended
        'operationcount = 0
        'totaloperations = R * R * Qangle * Qstrength * Qcoherence
        'operationcount = 1
    End Sub

End Class
