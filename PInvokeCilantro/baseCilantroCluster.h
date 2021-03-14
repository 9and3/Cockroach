#pragma once


//using namespace std;

#define PINVOKE extern "C" __declspec(dllexport)
#define deletePtr(ptr, isArray) if (isArray) {delete[] arr;} else {delete arr;}


#include <cilantro/utilities/point_cloud.hpp>


#include <iostream>

// Cilantro
#include <cilantro/utilities/point_cloud.hpp>
#include <cilantro/utilities/io_utilities.hpp>
#include <cilantro/utilities/ply_io.hpp>
#include <cilantro/utilities/timer.hpp>
#include <cilantro/clustering/connected_component_extraction.hpp>
#include <cilantro/utilities/nearest_neighbor_graph_utilities.hpp>
#include <cilantro/clustering/spectral_clustering.hpp>
#include <cilantro/clustering/mean_shift.hpp>





PINVOKE void ReleaseInt (int* arr, bool isArray);
PINVOKE void ReleaseDouble (double* arr, bool isArray);

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///                                                                       CLUSTER TECHNIQUES                                                                             ///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// \summary HCS cluster: method based on normals and radius search.
/// \param voxel_size_search Size of voxel for point search, the smaller the more/smaller clusters it will produce.
/// \param normal_threshold_degree The threshold of tolerance for normal evaluation in degree angle.
/// \param min_cluster_size Minimum number of points in a cluster.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_connectedComponentRadius (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    float voxel_size_search = 0.1f,
    double normal_threshold_degree = 2.0,
    int min_cluster_size = 100,
    bool color_point_cloud = true);
