#pragma once
//#include "stdafx.h"



#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}

#include <iostream>
#include <fstream>
#include <limits>
#include <vector>
#include <algorithm>
#include "open3d/Open3D.h"

PINVOKE int Test_GetSquare(int n);




PINVOKE void Open3DDownsample (

    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double voxelSize,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o
);

PINVOKE void Open3DNormals (

    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double radius,// = 0.1,
    int iterations,// = 30,
    int neighbours,// = 100, //reorientation

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o
);

PINVOKE void Open3DPoisson (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    size_t PoisonMaxDepth,
    size_t PoisonMinDepth,
    float PoisonScale,
    bool Linear,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o,
    double*& density, int& density_c);


PINVOKE void Open3DMeshPopulate (
    double* p, size_t p_c,
    int* f, size_t f_c,

    int type,
    int n,

    double*& p_o, int& p_c_o
);


PINVOKE void RANSACPlane (//0 //100 //1000000
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double  distanceThreshold,
    int neighbours,
    int iterations,
    bool InliersOrOutliers,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o

);



PINVOKE void ReleaseInt (int* arr, bool isArray);
PINVOKE void ReleaseDouble (double* arr, bool isArray);

