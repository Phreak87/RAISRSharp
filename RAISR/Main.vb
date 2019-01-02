Module Main

    Sub Main()
        Dim dirPath As String = "./train_images"
        Dim outPath As String = "./result_images"
        Dim filterPath As String = "./filters"

        Dim ImageList As New List(Of Emgu.CV.Mat)           '    vector<Mat> imageList;
        Dim ImageNameList As New List(Of String)            '    vector<string> imageNameList;
        readListofImage(dirPath, ImageList, ImageNameList)  '    readListOfImage(dirPath, imageList, imageNameList);

        Dim model As New RAISR(ImageList, 4, 11, 9)         '    RAISR model(imageList, 4, 11, 9);

        '//    string filterFilePath = "./filters/2018_5_31_19_40_15_scale_2.filter";
        '//    model.readInFilter(filterFilePath);

        model.Train()                       '    model.train();
        model.WriteOutFilter(filterPath)    '    model.writeOutFilter(filterPath);
        dirPath = "./test_images"           '    dirPath = "./test_images";

        ImageList.Clear()
        ImageNameList.Clear()
        readListofImage(dirPath, ImageList, ImageNameList)
        Dim downScaledImageList As New List(Of Emgu.CV.Mat)   '    vector<Mat> downScaledImageList;
        Dim cheapScaledImageList As New List(Of Emgu.CV.Mat)  '    vector<Mat> cheapScaledImageList;
        Dim RAISRImageList As New List(Of Emgu.CV.Mat)        '    vector<Mat> RAISRImageList;

        model.Test(True, ImageList, downScaledImageList, RAISRImageList, cheapScaledImageList, "None")
        For i As Integer = 0 To ImageList.Count
            Dim CurrentOutPath As String = ""
            CurrentOutPath = outPath + "/cheapScale_" & ImageNameList(i)
            cheapScaledImageList(i).Save(CurrentOutPath) '          imwrite(currentOutPath, cheapScaledImageList[i]);

            CurrentOutPath = outPath + "/RAISR_" & ImageNameList(i)
            RAISRImageList(i).Save(CurrentOutPath) '                imwrite(currentOutPath, RAISRImageList[i]);

            CurrentOutPath = outPath + "/downScaled_" & ImageNameList(i)
            downScaledImageList(i).Save(CurrentOutPath) '           imwrite(currentOutPath, downScaledImageList[i]);
        Next
    End Sub

End Module
