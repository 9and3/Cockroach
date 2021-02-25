#include "baseCGALMeshPointSetDistance.h"



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
){

	
	//CGAL::Polygon_mesh_processing::approximate_Hausdorff_distance

	//double max_dist =
	//	CGAL::Polygon_mesh_processing::approximate_max_distance_to_point_set
	//	(output_mesh,
	//		CGAL::make_range (boost::make_transform_iterator
	//		(points.begin (), CGAL::Property_map_to_unary_function<Point_map> ()),
	//			boost::make_transform_iterator
	//			(points.end (), CGAL::Property_map_to_unary_function<Point_map> ())),
	//		4000);
	//std::cout << "Max distance to point_set: " << max_dist << std::endl;

}
