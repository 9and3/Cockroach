#pragma once

#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}

PINVOKE void ReleaseInt (int* arr, bool isArray);
PINVOKE void ReleaseDouble (double* arr, bool isArray);



struct vec3 {
	double x = 0, y = 0, z = 0;
	vec3 (double, double, double);
};

#include <CGAL/Polyhedron_items_with_id_3.h>


#include <CGAL/Exact_predicates_inexact_constructions_kernel.h>
#include <CGAL/IO/Polyhedron_iostream.h>
#include <CGAL/Polyhedron_3.h>
#include <CGAL/Polyhedron_incremental_builder_3.h>
#include <CGAL/Simple_cartesian.h>
#include <CGAL/Surface_mesh.h>
#include <CGAL/boost/graph/split_graph_into_polylines.h>
#include <CGAL/extract_mean_curvature_flow_skeleton.h>
#include <CGAL/Polygon_mesh_processing/corefinement.h>
#include <CGAL/Polyhedron_items_with_id_3.h>

#include <CGAL/compute_average_spacing.h>
#include <CGAL/pca_estimate_normals.h>
#include <CGAL/mst_orient_normals.h>
#include <CGAL/property_map.h>
#include <utility> // defines std::pair
#include <list>

#include <CGAL/poisson_surface_reconstruction.h>

#include <iostream>
#include <fstream>




#include <CGAL/Point_set_3.h>
#include <CGAL/Point_set_3/IO.h>
#include <CGAL/cluster_point_set.h>
#include <CGAL/Random.h>
#include <CGAL/Real_timer.h>



//Convex hull case: input array of point, return face vertex indexing
//Mesh boolean: array of points, array of faces id,
PINVOKE void MeshBoolean_Create (
	double* coord_mesh1, size_t n_coord_mesh1,
	int* faces_mesh1, size_t n_faces_mesh1,

	double* coord_mesh2, size_t n_coord_mesh2,
	int* faces_mesh2, size_t n_faces_mesh2,

	size_t Difference_Union_Intersection,

	double*& coord_out, int& n_coord_out,
	int*& faces_out, int& n_faces_out
);

PINVOKE void MeshBoolean_CreateArray (


	double* coord_mesh, int* n_coord_meshArray,//Flat array of coordinates / 0 256 512 flat array of vertices array / number of meshes
	int* faces_mesh, int* n_faces_meshArray,
	size_t n_mesh,

	size_t Difference_Union_Intersection,

	double*& coord_out, int& n_coord_out,
	int*& faces_out, int& n_faces_out,
	int& nValidMeshes

);



PINVOKE void MeshSkeleton_Create (


	double* coord_mesh1, size_t n_coord_mesh1,
	int* faces_mesh1, size_t n_faces_mesh1,

	//List of Polylines
	double*& coord_out, int& n_coord_out,
	int*& coord_ID, int& n_coord_ID


);


PINVOKE void ComputeNormals (

	double* p, size_t p_c,
	double* n, size_t n_c,
	double* c, size_t c_c,

	double radius,// = 0.1,
	int iterations,// = 30,
	int neighbours,// = 100, //reorientation
	bool erase,

	double*& p_o, int& p_c_o,
	double*& n_o, int& n_c_o,
	double*& c_o, int& c_c_o
);





#include <boost/container/flat_map.hpp>

PINVOKE void MeshBoolean_CreateArrayTrackColors (


	double* coord_mesh, int* n_coord_meshArray,//Flat array of coordinates / 0 256 512 flat array of vertices array / number of meshes
	int* faces_mesh, int* n_faces_meshArray,
	size_t n_mesh,

	size_t Difference_Union_Intersection,

	double*& coord_out, int& n_coord_out,
	int*& faces_out, int& n_faces_out,
	int*& facesColors_out, int& n_facesColors_out,
	int& nValidMeshes

);