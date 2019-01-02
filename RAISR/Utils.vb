Module Utils

    Function Type2Str(Type As Integer) As String
        '    string type2str(int type) {
        '    string r;

        '    uchar depth = type & CV_MAT_DEPTH_MASK;
        '    uchar chans = 1 + (type >> CV_CN_SHIFT);

        '    switch ( depth ) {
        '        case CV_8U:  r = "8U"; break;
        '        case CV_8S:  r = "8S"; break;
        '        case CV_16U: r = "16U"; break;
        '        case CV_16S: r = "16S"; break;
        '        case CV_32S: r = "32S"; break;
        '        case CV_32F: r = "32F"; break;
        '        case CV_64F: r = "64F"; break;
        '        default:     r = "User"; break;
        '    }

        '    r += "C";
        '    r += (chans+'0');

        '    return r;
        '}
    End Function
    Sub DebugMat()
        'void debugMat(Mat m) {
        '    cout << "Rows: " << m.rows << ", Cols: " << m.cols << endl;
        '    cout << "M = "<< endl << " "  << m << endl << endl;
        '    waitKey();
        '}
    End Sub
    Sub MatIsEqual(mat1 As Object, mat2 As Object)
        '    bool matIsEqual(const cv::Mat mat1, const cv::Mat mat2) {
        '    // treat two empty mat as identical as well
        '    if (mat1.empty() && mat2.empty()) {
        '        return true;
        '    }
        '    // if dimensionality of two mat is not identical, these two mat is not identical
        '    if (mat1.cols != mat2.cols || mat1.rows != mat2.rows || mat1.dims != mat2.dims) {
        '        return false;
        '    }
        '    cv::Mat diff;
        '    cv::compare(mat1, mat2, diff, cv::CMP_NE);
        '    int nz = cv::countNonZero(diff);
        '    return nz == 0;
        '}
    End Sub
    Sub readListofImage(dirPath As String,
                        ByRef imageMatList As List(Of Emgu.CV.Mat),
                        ByRef ImageNameList As List(Of String))

        'void readListOfImage(string& dirPath, vector<Mat>& imageMatList, vector<string>& imageNameList) {

        '    DIR *dir;
        '    struct dirent *entry;
        '    string filePath;
        '    struct stat fileStat;

        For Each File In My.Computer.FileSystem.GetFiles(dirPath)
            ImageNameList.Add(File)
            imageMatList.Add(New Emgu.CV.Mat(File))
        Next
        '    if ((dir = opendir (dirPath.c_str())) != NULL) {
        '        //print all the files and directories within directory
        '        while ((entry = readdir (dir)) != NULL) {
        '            filePath= dirPath + "/" + entry->d_name;


        '            // check if file is valid and check if file is not a directory
        '            if (stat( filePath.c_str(), &fileStat )) continue;
        '            if (S_ISDIR( fileStat.st_mode ))         continue;

        '            cout<< "read image: " << filePath.c_str() << endl;
        '            imageNameList.push_back(string(entry->d_name));

        '            Mat currentImage = imread(filePath.c_str(), 0);
        '            if (!currentImage.data) {
        '                cout << "Read current image error!" << endl;
        '                exit (EXIT_FAILURE);
        '            }

        '            imageMatList.push_back(currentImage);
        '        }
        '        closedir (dir);
        '    } else {
        '        cout << "Error opening directory" << endl;
        '        exit (EXIT_FAILURE);
        '    }
        '}
    End Sub

End Module
