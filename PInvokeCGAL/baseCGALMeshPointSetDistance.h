#pragma once

#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}

PINVOKE void ReleaseInt (int* arr, bool isArray);
PINVOKE void ReleaseDouble (double* arr, bool isArray);


#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Surface_mesh.h>
#include <CGAL/Polygon_mesh_processing/distance.h>
#include <CGAL/Polygon_mesh_processing/remesh.h>
#define TAG CGAL::Parallel_if_available_tag
typedef CGAL::Exact_predicates_inexact_constructions_kernel K;
typedef K::Point_3                     Point;
typedef CGAL::Surface_mesh<K::Point_3> Mesh;
namespace PMP = CGAL::Polygon_mesh_processing;


PINVOKE void MeshToCloudDistance (

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
