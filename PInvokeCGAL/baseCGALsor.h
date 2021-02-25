#pragma once
#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/property_map.h>
#include <CGAL/compute_average_spacing.h>
#include <CGAL/remove_outliers.h>
#include <CGAL/IO/read_xyz_points.h>
#include <vector>
#include <fstream>
#include <iostream>
#include <CGAL/Point_set_3.h>
#include <CGAL/Point_set_3/IO.h>

//typedef CGAL::Exact_predicates_inexact_constructions_kernel Kernel;
//typedef Kernel::Point_3 Point;

//using Kernel = CGAL::Exact_predicates_inexact_constructions_kernel;
//using Point_3 = Kernel::Point_3;

using Kernel = CGAL::Exact_predicates_inexact_constructions_kernel;
using Point_3 = Kernel::Point_3;
using Point_set = CGAL::Point_set_3<Point_3>;

#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}

PINVOKE void ReleaseInt (int* arr, bool isArray);
PINVOKE void ReleaseDouble (double* arr, bool isArray);

PINVOKE void SOR (

    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double radius,
    double percent,
    int neighbours,
    int type,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,//user normals to track id
    double*& c_o, int& c_c_o

) ;