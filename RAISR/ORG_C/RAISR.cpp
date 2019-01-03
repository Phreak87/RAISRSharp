#include <algorithm>
#include <assert.h>
#include <ctime>
#include <fstream>
#include <iostream>
#include <iterator>
#include "RAISR.h"
#include <sstream>

using namespace std;
using namespace cv;


void RAISR::testPrivateModuleMethod() {
    double dummy_query_data[] = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
    double another[] = { 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0};
    cv::Mat dummy_query = cv::Mat(3, 3, CV_64F, dummy_query_data);
    cv::Mat another_mat = cv::Mat(3, 3, CV_64F, another);
    cout << another_mat << endl;

    vector<double> flatten;
    flattenPatchBoundary(another_mat, flatten);
    for (int i = 0 ; i< flatten.size(); i++){
        cout << flatten[i] << endl;
    }

    cout << getLeastConnectedComponents(another_mat);


}

void RAISR::readInFilter(string& inPath){

    string line;
    ifstream infile (inPath);
    if (infile.is_open()) {

        getline(infile, line);
        istringstream iss(line);
        vector<string> tokens{istream_iterator<string>{iss},
                              istream_iterator<string>{}};
        int size = stoi(tokens[0].c_str());
        int inner_size = stoi(tokens[1].c_str());
        int rows = stoi(tokens[2].c_str());
        int cols = stoi(tokens[3].c_str());
        if (rows!= patchLength*patchLength){
            cout << "filter file is not compatible with current patchLength, please check"<<endl;
            exit(-1);
        }

        int i = 0;
        while ( getline (infile,line) ) {
            int k = 0;
            istringstream new_iss(line);
            vector<string> new_tokens{istream_iterator<string>{new_iss},
                                      istream_iterator<string>{}};
            Mat currentMat(rows, cols, CV_64F);
            for (int r = 0 ; r < rows; r++){
                for (int c = 0 ; c < cols; c++){
                    currentMat.at<double>(r,c) = stod(new_tokens[k].c_str());
                    k++;
                }
            }
            filterBuckets[i/inner_size][i%inner_size] = currentMat;
            i++;

        }
        infile.close();
    }
    else cout << "Unable to open file";

    trained = true;
}



Rotation& operator++( Rotation &c ) {
    using IntType = typename std::underlying_type<Rotation>::type;
    if(c == Rotation::ROTATE_270)
        c = Rotation ::NO_ROTATION;
    else
        c = static_cast<Rotation >( static_cast<IntType>(c) + 1 );
    return c;
}

Rotation operator++( Rotation &c, int ) {
    Rotation result = c;
    ++c;
    return result;
}

/************************************************************
 *  This function is a least square solver implementaion with
 *  using conjugate Gradient algorithm
 *  Note: It is trying to find an X that minimize |AX-b| most
 *
 *  params: A, b : matrices used in the calculation
 *  return: x
 */
Mat conjugateGradientSolver(Mat A, Mat b){
    int rows = A.rows;
    int cols = A.cols;
    double sumOfA = sum(A)[0];

    Mat result = Mat(rows ,1, CV_64F, double(0));
    while (sumOfA >= 100){
        if (determinant(A) < 1){
            A = A + Mat::eye(rows, cols, CV_64F)* sumOfA*0.000000005;
        }else{
            result += A.inv() * b;
            break;
        }
    }
    return result;
}

void flattenPatchBoundary(Mat patch, vector<double>& flattenPatch){
    int rows = patch.rows;
    int cols = patch.cols;
    int dr[] = {0, 1, 0, -1};
    int dc[] = {1, 0, -1, 0};
    int numberOfSteps = (rows*2 + cols*2 -4);
    int r = 0;
    int c = 0;
    int dir_index = 0;

    // flatten the patch
    for (int i = 0; i< numberOfSteps; i++){
        double value = patch.at<double>(r,c);
        flattenPatch.push_back(value);
        int nr = r+ dr[dir_index%4];
        int nc = c+ dc[dir_index%4];
        if (nr<0 || nr >= rows || nc < 0 || nc >= cols) {
            dir_index += 1;
        }
        r = r+ dr[dir_index%4];
        c = c+ dc[dir_index%4];
    }
}

int getLeastConnectedComponents(Mat patch){

    int rows = patch.rows;
    int cols = patch.cols;
    int numberOfSteps = (rows*2 + cols*2 -4);
    vector<double> flattenPatch;
    int i =0;

    flattenPatchBoundary(patch, flattenPatch);
    i = 0;
    for (; i< numberOfSteps; i++){
        if (flattenPatch[i] != flattenPatch[(i+1)%numberOfSteps]) break;
    }
    if(i == numberOfSteps) return 0;
    int count = numberOfSteps;
    i+=1;
    int j = 0;
    while (j < numberOfSteps){
        int tempCount = 1;
        while (j < numberOfSteps && flattenPatch[i%numberOfSteps] == flattenPatch[(i+1)%numberOfSteps]){
            tempCount +=1;
            i++;
            j++;
        }
         count = count > tempCount ? tempCount : count;
        i++;
        j++;
    }

    return count;
}
