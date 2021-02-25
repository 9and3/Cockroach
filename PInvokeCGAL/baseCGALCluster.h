#pragma once
#define PINVOKE extern "C" __declspec(dllexport)


#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/Point_set_3.h>
#include <CGAL/Point_set_3/IO.h>
#include <CGAL/cluster_point_set.h>
#include <CGAL/compute_average_spacing.h>
#include <CGAL/Random.h>
#include <CGAL/Real_timer.h>
#include <fstream>
using Kernel = CGAL::Exact_predicates_inexact_constructions_kernel;
using Point_3 = Kernel::Point_3;
using Point_set = CGAL::Point_set_3<Point_3>;



PINVOKE void Cluster (
	double* p, size_t p_c,
	double* n, size_t n_c,
	double* c, size_t c_c,

	double radius,
	int iterations,
	int neighbours,


	int*& n_o, int& n_c_o,
	double*& c_o, int& c_c_o,
	int& numberOfClusters
	);