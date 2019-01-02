Public Class RAISR
    Dim Trained As Boolean
    Dim imageMatList As List(Of Emgu.CV.Mat)
    Dim Scale, patchLength, gradientLength
    Dim FilterBuckets As List(Of Emgu.CV.Mat()) '     std::vector<std::vector<cv::Mat>>  filterBuckets; // contains trained filter

    Public Sub New(ByVal _imageMatList As List(Of Emgu.CV.Mat),
                   ByVal _scale As Integer,
                   ByVal _patchLength As Integer,
                   ByVal _gradientLength As Integer)

        Trained = False
        imageMatList = _imageMatList
        Scale = _scale
        patchLength = _patchLength
        gradientLength = _gradientLength
        FilterBuckets = New List(Of Emgu.CV.Mat())(24 * 3 * 3) ' (HashBuckets.NumOfAngle * HashBuckets.NumOfCoherence * HashBuckets.NumOfStrength)
        For i As Integer = 0 To 24 * 3 * 3 : FilterBuckets.Add(Nothing) : Next

        Debug.Assert(patchLength Mod 2 = 1 AndAlso
                     gradientLength Mod 2 = 1 AndAlso
                     patchLength > 0 AndAlso
                     gradientLength > 0 AndAlso
                     patchLength >= gradientLength)

        Dim numberOfFilters As Integer = Scale * Scale

        For i As Integer = 0 To FilterBuckets.Count - 1
            Array.Resize(FilterBuckets(i), numberOfFilters)
        Next
    End Sub

    Sub Train()

        '    void RAISR::train() {
        '    // initialize the calculation buckets
        Dim numberOfFilters As Integer = Scale * Scale
        Dim margin As Integer = patchLength / 2

        Dim ATA(FilterBuckets.Count)() As Emgu.CV.Mat  '    vector<vector<Mat>> ATA(filterBuckets.size());
        Dim ATB(FilterBuckets.Count)() As Emgu.CV.Mat '    vector<vector<Mat>> ATb(filterBuckets.size());

        For i As Integer = 0 To FilterBuckets.Count - 1
            Array.Resize(ATA(i), numberOfFilters)
            Array.Resize(ATB(i), numberOfFilters)
        Next

        For i As Integer = 0 To imageMatList.Count - 1
            Dim HRImage As Emgu.CV.Mat = imageMatList(i)
            Dim LRImage As Emgu.CV.Mat = DownGrade(HRImage, Scale)

            Dim Buckets As New HashBuckets(LRImage.Clone, Scale, gradientLength)    '        HashBuckets buckets(LRImage.clone(), (unsigned) scale,(unsigned) gradientLength);
            LRImage.ConvertTo(LRImage, Emgu.CV.CvEnum.DepthType.Cv64F)              '        LRImage.convertTo(LRImage, CV_64F);
            HRImage.ConvertTo(LRImage, Emgu.CV.CvEnum.DepthType.Cv64F)              '        HRImage.convertTo(HRImage, CV_64F);

            Dim rows As Integer = LRImage.Rows '        int rows = LRImage.rows;
            Dim cols As Integer = LRImage.Cols '        int cols = LRImage.cols;

            '        // loop each High Resolution pixel
            '        for (int r = margin; r<= rows - margin -1; r++){
            '            for (int c = margin ; c <= cols - margin -1 ; c++){

            '                // find the type of the pixel
            '                int pixelType = ((r-margin) % scale) * scale  + ((c-margin) % scale);

            '                // get the value of current High Resolution pixel
            '                double HRPixel = HRImage.at<double>(r,c);

            '                // find the corresponding Low Resolution patch
            '                // Range is left inclusive and right exclusive function
            '                Mat patch = LRImage(
            '                        Range(r - margin, r + margin + 1),
            '                        Range(c - margin, c + margin + 1)
            '                ).clone();

            '                // for each patch, we can mirror it and rotate it 90/180/270 degree at the same time
            '                // to get 7 more training patches, which gives us total 8 patches at each pixel
            '                for (Mirror mirrorFlag: {Mirror::NO_MIRROR, Mirror::MIRROR}){
            '                    if (mirrorFlag == MIRROR) flip(patch, patch, 1);
            '                    for (Rotation rotateFlag: {Rotation::NO_ROTATION, Rotation::ROTATE_90, Rotation::ROTATE_180, Rotation::ROTATE_270}) {
            '                        Mat rotatedPatch;
            '                        if (rotateFlag == Rotation::NO_ROTATION) rotatedPatch = patch.clone();
            '                        else rotate(patch, rotatedPatch, rotateFlag);
            '                        int hashValue = getHashValue(buckets, r, c, rotateFlag, mirrorFlag);

            '                        // use the current patch to fill the calculation matrix
            '                        fillBucketsMatrix(ATA, ATb, hashValue, rotatedPatch, HRPixel, pixelType);
            '                    }
            '                }
            '            }
            '        }
            '    }
        Next


        '    // loop each buckets and loop every calculation matrix inside
        '    //  to solve least square to get corresponding filter
        '    for (int i = 0 ; i< filterBuckets.size(); i++){
        '        for (int j = 0 ; j< numberOfFilters; j++){
        '            Mat currentEntryFilter;
        '            if (ATA[i][j].empty()) continue;
        '            solve(ATA[i][j], ATb[i][j], currentEntryFilter, DECOMP_SVD);
        '            filterBuckets[i][j] = currentEntryFilter;
        '        }
        '    }

        '    // when training stage is done, we should set flag to true
        '    trained = true;

        '    cout << "training process done "<< endl << flush;
        '}

    End Sub
    Sub Test(DownScale As Boolean,
             ByRef ImageMatList As List(Of Emgu.CV.Mat),
             ByRef DownScaledImageList As List(Of Emgu.CV.Mat),
             ByRef RAISRImageList As List(Of Emgu.CV.Mat),
             ByRef CheapScaledImageList As List(Of Emgu.CV.Mat),
             CTBlendingType As String)

        '        void RAISR::test(bool downScale, vector<Mat> &imageMatList, vector<Mat> & downScaledImageList, vector<Mat>& RAISRImageList, vector<Mat> &cheapScaledImageList, string CTBlendingType) {

        If Trained = False Then
            Throw New Exception("you must train the model before test the model")
            Exit Sub
        End If

        '    cout << "test process start "<< endl << flush;

        '    for (int i= 0 ; i < imageMatList.size(); i++){
        '        Mat image = imageMatList[i];
        '        Mat LRImage;
        '        int rows = image.rows;
        '        int cols = image.cols;
        '        int margin = patchLength/2;

        '        if (downScale){
        '            // downscale image to generate the true test sample
        '            Mat downScaledImage;
        '            Size ImageSize = Size(cols/scale, rows/scale);
        '            resize(image, downScaledImage, ImageSize, 0, 0, INTER_CUBIC);
        '            downScaledImageList.push_back(downScaledImage);

        '            // cheap upscale the image

        '            ImageSize = Size(cols, rows);
        '            resize(downScaledImage, LRImage, ImageSize, 0, 0, INTER_LINEAR);
        '            cheapScaledImageList.push_back(LRImage.clone());
        '        }else{

        '            // just cheap upscale the image
        '            rows *=scale;
        '            cols *=scale;
        '            Size ImageSize = Size(cols, rows);
        '            resize(image, LRImage, ImageSize, 0, 0, INTER_LINEAR);
        '            cheapScaledImageList.push_back(LRImage.clone());

        '        }

        '        // construct the HashBuckets
        '        HashBuckets buckets(LRImage.clone(), (unsigned) scale,(unsigned) gradientLength);

        '        // now convert the LRImage into modifiable one
        '        LRImage.convertTo(LRImage, CV_64F);

        '        // construct the container to hold RAISR image by copy that cheap upscaled image
        '        Mat HRImage = LRImage.clone();

        '        // loop each pixel
        '        for (int r = margin; r<= rows - margin -1; r++) {
        '            for (int c = margin; c <= cols - margin - 1; c++) {
        '                int pixelType = ((r-margin) % scale) * scale  + ((c-margin) % scale);

        '                // get each pixel's corresponding patch
        '                Mat patch = LRImage(
        '                        Range(r - margin, r + margin + 1),
        '                        Range(c - margin, c + margin + 1)
        '                ).clone();

        '                // flatten the patch
        '                Mat flattedPatch = patch.reshape(0,1);

        '                // find the hashValue for that patch to find
        '                // the corresponding filter in the filterBuckets
        '                int hashValue = getHashValue(buckets, r, c, NO_ROTATION, NO_MIRROR);
        '                if (filterBuckets[hashValue][pixelType].empty()){
        '                    continue;
        '                }

        '                // map the patch to a new pixel value by apply trained filters
        '                Mat filteredPixel = flattedPatch*filterBuckets[hashValue][pixelType];

        '                // assign that calculated pixel value back to the image
        '                HRImage.at<double>(r,c) = filteredPixel.at<double>(0,0);

        '            }
        '        }

        '        // CTBlending process. The CT-patch is 3x3 square
        '        margin = 3;
        '        Mat HRImageCopy = HRImage.clone();
        '        if (CTBlendingType != "None"){
        '            for (int r = margin; r<= rows - margin -1; r++) {
        '                for (int c = margin; c <= cols - margin - 1; c++) {
        '                    // get each pixel's corresponding patch
        '                    Mat LRPatch = LRImage(
        '                            Range(r - margin, r + margin + 1),
        '                            Range(c - margin, c + margin + 1)
        '                    ).clone();

        '                    Mat HRPatch =  HRImage(
        '                            Range(r - margin, r + margin + 1),
        '                            Range(c - margin, c + margin + 1)
        '                    ).clone();

        '                    // Census transform
        '                    for (int i = 0 ; i < margin; i++){
        '                        for (int j = 0 ; j < margin ; j++){
        '                            if (i == 1 and j == 1) continue;
        '                            LRPatch.at<double>(i,j) = LRPatch.at<double>(i,j) > LRPatch.at<double>(1,1) ? 1.0:0.0;
        '                            HRPatch.at<double>(i,j) = HRPatch.at<double>(i,j) > HRPatch.at<double>(1,1) ? 1.0:0.0;
        '                        }
        '                    }

        '                    double maxCount = (double)(margin*margin-4);
        '                    if (CTBlendingType=="Randomness"){
        '                        double LRCount = (double)getLeastConnectedComponents(LRPatch);
        '                        double weight = LRCount*2.0/maxCount;
        '                        HRImageCopy.at<double>(r,c) = LRImage.at<double>(r,c) + weight*(HRImage.at<double>(r,c) - LRImage.at<double>(r,c));

        '                    } else if (CTBlendingType == "CountOfBitsChanged"){
        '                        vector<double>HRFlattenPatch;
        '                        vector<double>LRFlattenPatch;
        '                        flattenPatchBoundary(HRPatch, HRFlattenPatch);
        '                        flattenPatchBoundary(LRPatch, LRFlattenPatch);
        '                        double countOfBitsChanged = 0.0 ;
        '                        for (int k = 0 ; k<HRFlattenPatch.size(); k++){
        '                            if (HRFlattenPatch[k] != LRFlattenPatch[k]) countOfBitsChanged+=1.0;
        '                        }
        '                        double weight = countOfBitsChanged/maxCount;

        '                        HRImageCopy.at<double>(r,c) = LRImage.at<double>(r,c) + weight*(HRImage.at<double>(r,c) - LRImage.at<double>(r,c));

        '                    }else{
        '                        cout<<"invalid blending type, so no blending applied" << endl;
        '                        break;
        '                    }
        '                }
        '            }
        '        }

        '        Mat resultHRImage;
        '        convertScaleAbs(HRImageCopy, resultHRImage);
        '        RAISRImageList.push_back(resultHRImage);
        '    }

        '    cout << "test process done "<< endl << flush;

        '}

    End Sub
    Sub WriteOutFilter(outPath As String)
        '        void RAISR::writeOutFilter(string& outPath){

        If Trained = False Then
            Throw New Exception("model is not trained, you cannot serialize current filter")
            Exit Sub ' Programm beenden
        End If

        Dim outfile As System.IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(outPath & Now.ToString.Replace(":", "_") & ".Filter", False)

        '    if (!trained ){
        '        cout<< "model is not trained, you cannot serialize current filter" << endl;
        '        exit(-1);
        '    }
        '    ofstream outfile;
        '    string outFilePath;
        '    std::time_t t = std::time(0);   // get time now
        '    std::tm* now = std::localtime(&t);
        '    outFilePath = outPath + "/"+ to_string(now->tm_year+1900) + "_" + to_string(now->tm_mon + 1) + "_"
        '                  + to_string(now->tm_mday) + "_"
        '                  + to_string(now->tm_hour) + "_"
        '                  + to_string(now->tm_min)  + "_"
        '                  + to_string(now->tm_sec)  + ".filter";
        '    cout<< " file wil be stored at "<< outFilePath << endl;
        '    outfile.open (outFilePath.c_str());

        '    int size = (int) filterBuckets.size();
        '    int inner_size = (int) filterBuckets[0].size();
        '    int rows = filterBuckets[0][0].rows;
        '    int cols = filterBuckets[0][0].cols;

        '    outfile << to_string(size) << " "<< to_string(inner_size) << " " << to_string(rows) << " " << to_string(cols) << " "<< endl;
        '    for(int i = 0 ; i < size; i++){
        '        for (int j = 0 ; j< inner_size ; j++){
        '            for (int r = 0 ; r < rows; r++){
        '                for (int c = 0; c < cols; c++){
        '                    outfile << to_string(filterBuckets[i][j].at<double>(r, c))<< " ";
        '                }
        '            }
        '            outfile << endl;
        '        }
        '    }
        '//    outfile << "Writing this to a file.\n";
        '    outfile.close();

        '}
    End Sub
    Function DownGrade(Image As Emgu.CV.Mat, Scale As Integer) As Emgu.CV.Mat

        Dim ImageSize = Image.Size
        Dim Rows As Integer = Image.Rows
        Dim Cols As Integer = Image.Cols
        Dim TempImageSize As New Drawing.Size(Cols / Scale, Rows / Scale)
        Dim TempImage As New Emgu.CV.Mat
        Dim ResultImage As New Emgu.CV.Mat

        Emgu.CV.CvInvoke.Resize(Image, TempImage, TempImageSize, 0, 0, Emgu.CV.CvEnum.Inter.Cubic)  '    resize(image, tempImage, tempImageSize, 0, 0, INTER_CUBIC);
        Emgu.CV.CvInvoke.Resize(TempImage, ResultImage, ImageSize, 0, 0, Emgu.CV.CvEnum.Inter.Linear)  '    resize(tempImage, resultImage, imageSize, 0,0, INTER_LINEAR);

        Return ResultImage
    End Function
End Class