Imports System.Collections.Generic

Class test

    Public args As Object '= gettestargs()
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
        If IsNothing(args.filter) Then filtername = args.filter()
        h = Pickle.Load(filtername)
        weighting = gaussian2d.gaussian2d(New List(Of Object) From {
        gradientsize,
        gradientsize
        }, 2)
        weighting = NP.diag(weighting.ravel())

        For Each File In My.Computer.FileSystem.GetFiles(trainpath) : imagelist.Add(File) : Next
        Dim imagecount As Integer = 1

        Dim cv2 As Object
        Dim InterPolate As Object

        For Each image In imagelist

            Dim origin As Object = cv2.imread(image)
            Dim ycrcvorigin As Object = cv2.cvtColor(origin, cv2.COLOR_BGR2YCrCb)

            Dim grayorigin As Object = ycrcvorigin(":", ":", 0)
            grayorigin = (cv2.normalize(grayorigin.astype("float"), Nothing, grayorigin.min() / 255, grayorigin.max() / 255, cv2.NORM_MINMAX))

            Dim heightLR = 0 ' GreyOrigin.shape(height)
            Dim widthLR = 0 ' GreyOrigin.shape(width)
            Dim heightgridLR As Object = NP.linspace(0, heightLR - 1, heightLR)
            Dim widthgridLR As Object = NP.linspace(0, widthLR - 1, widthLR)

            Dim bilinearinterp As Object = interpolate.interp2d(widthgridLR, heightgridLR, grayorigin, kind:="linear")
            Dim heightgridHR As Object = NP.linspace(0, heightLR - 0.5, heightLR * 2)
            Dim widthgridHR As Object = NP.linspace(0, widthLR - 0.5, widthLR * 2)
            Dim upscaledLR As Object = bilinearinterp(widthgridHR, heightgridHR)

            Dim heightHR = 0 ' upscaledLR.shape(height)
            Dim widthHR = 0 ' upscaledLR.shape(width)
            Dim predictHR As Object = NP.Zeros(Tuple.Create(heightHR - 2 * margin, widthHR - 2 * margin))

            Dim operationcount As Object = 0
            Dim totaloperations As Object = (heightHR - 2 * margin) * (widthHR - 2 * margin)

            For row As Integer = 1 To Range(margin, heightHR - margin)
                For col As Integer = 1 To Range(margin, widthHR - margin)
                    operationcount += 1

                    Dim patch As Object '= upscaledLR((row - patchmargin)((row + patchmargin) + 1), (col - patchmargin)((col + patchmargin) + 1)).ravel
                    Dim gradientblock As Object '= upscaledLR((row - gradientmargin)((row + gradientmargin) + 1), (col - gradientmargin)((col + gradientmargin) + 1))

                    Dim HK = hashkey.hashkey(gradientblock, Qangle, weighting)
                    Dim angle = HK.angle
                    Dim strength = HK.strength
                    Dim coherence = HK.coherence

                    Dim pixeltype As Object = (row - margin) Mod R * R + (col - margin) Mod R
                    predictHR(row - margin, col - margin) = NP.Clip(predictHR.astype("float") * 255.0, 0.0, 255.0)
                Next
            Next

            predictHR = NP.Clip(predictHR.astype("float") * 255, 0, 255)
            Dim result As Object = NP.Zeros(Tuple.Create(heightHR, widthHR, 3))
            Dim y As Object = ycrcvorigin(":", ":", 0)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, y, kind:="linear")
            result(":", ":", 0) = bilinearinterp(widthgridHR, heightgridHR)
            Dim cr As Object = ycrcvorigin(":", ":", 1)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cr, kind:="linear")
            result(":", ":", 1) = bilinearinterp(widthgridHR, heightgridHR)
            Dim cv As Object = ycrcvorigin(":", ":", 2)

            bilinearinterp = InterPolate.interp2d(widthgridLR, heightgridLR, cv, kind:="linear")
            result(":", ":", 2) = bilinearinterp(widthgridHR, heightgridHR)
            result = Nothing ' (margin:heightHR-margin,margin:widthHR-margin,0) =  predictHR
            result = Nothing ' cv2.cvtColor(NP.uint8(result), cv2.COLOR_YCrCb2RGB)

            cv2.imwrite("results/" & image & "_result.bmp", cv2.cvtColor(result, cv2.COLOR_RGB2BGR))
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

End Class

Class Pickle
    Shared Function Load()

    End Function
End Class