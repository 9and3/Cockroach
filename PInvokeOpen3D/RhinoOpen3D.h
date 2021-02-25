#pragma once



#include "stdafx.h"



#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}

#include <iostream>
#include <fstream>

#include <limits>
#include <vector>
#include <algorithm>
#include "open3d/Open3D.h"

using namespace std;

PINVOKE void Open3DDownsampleRhinoSDK (

    ON_PointCloud* c0,
    int numberOfPoints,
    ON_PointCloud& c1
);

std::shared_ptr<open3d::geometry::PointCloud> Convert_RhinoCloudToOpen3D (const ON_PointCloud * cloud);
ON_PointCloud Convert_Open3DToRhinoCloud (std::shared_ptr<open3d::geometry::PointCloud> Open3DCloud);




