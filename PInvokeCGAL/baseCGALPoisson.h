#pragma once

#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}

#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Point_set_3/IO.h>
#include <CGAL/Point_set_3.h>
#include <CGAL/compute_average_spacing.h>
#include <CGAL/poisson_surface_reconstruction.h>
#include <CGAL/Polyhedron_3.h>
#include <CGAL/Polyhedron_items_with_id_3.h>
#include <vector>
#include <string>

typedef CGAL::Exact_predicates_inexact_constructions_kernel Kernel2;
typedef Kernel2::Point_3 Point2;
typedef Kernel2::Vector_3 Vector2;
//typedef CGAL::Polyhedron_3<Kernel2> Polyhedron2;
typedef CGAL::Polyhedron_items_with_id_3 Polyhedron2;

//typedef CGAL::Polyhedron_items_with_id_3<Kernel2> Polyhedron2ID;


PINVOKE void ComputePoissonSurfaceReconstruction (

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
