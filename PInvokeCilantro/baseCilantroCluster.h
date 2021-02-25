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

/// \summary HCS cluster : method based on normals and neighbour search.
/// \param n_search Number of neighbours to be included in the k-search, the more the better, but also the more expensive.
/// \param normal_threshold_degree The threshold of tolerance for normal evaluation in degree angle.
/// \param min_cluster_size Minimum number of points in a cluster.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_connectedComponentKSearch (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    int n_search = 30,
    double normal_threshold_degree = 2,
    int min_cluster_size = 100,
    bool color_point_cloud = true);

/// \summary Spectral cluster : method based on Laplacian function [description to be compelted].
/// \param max_num_clusters Minimum number of clusters to be created.
/// \param k_search Number of neighbours to be searched.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_spectralCluster (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    size_t max_num_clusters = 5,
    int k_search = 30,
    bool color_point_cloud = true);

/// \summary Euclidean cluster : method based on HCS cluster with distance function as main descriptor.
/// \param n_search Number of neighbours to be included in the k-search, the more the better, but also the more expensive.
/// \param dist_thresh The distance threshold of points to be cluster together.
/// \param min_cluster_size Minimum number of points in a cluster.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_euclideanKsearch (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    int n_search = 30,
    float dist_thresh = 0.02,
    int min_cluster_size = 30,
    bool color_point_cloud = true);

/// \summary Euclidean cluster : method based on HCS cluster with distance function as main descriptor.
/// \param voxel_size_search Size of voxel for point search, the smaller the more/smaller clusters it will produce.
/// \param dist_thresh The distance threshold of points to be cluster together.
/// \param min_cluster_size Minimum number of points in a cluster.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_euclideanRadius (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    float voxel_size_search,
    float dist_thresh,
    int min_cluster_size,
    bool color_point_cloud = true);

/// \summary K-means cluster : iteratively finds spherical clusters of points (usually high-dimensional), where cluster affinity is based on a distance to the cluster center.
/// \param n_cluster Number of cluster to cast.
/// \param max_iter Maximal iterations.
/// \param use_k_tree Use or not the k-tree search
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_kMeans (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    size_t n_cluster = 250,
    size_t max_iter = 100,
    bool use_k_tree = true,
    bool color_point_cloud = true);

/// \summary Mean shift clustering aims to discover “blobs” in a smooth density of samples.
/// \param kernel_radius Radius of the kernel search.
/// \param max_iter Max number of iterations.
/// \param cluster_tol Tolerance of the clustering.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_meanShift (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    float kernel_radius = 2.0f,
    int max_iter = 5000,
    float cluster_tol = 0.2f,
    bool color_point_cloud = true);

/// \summary Cluster points with the same color.
/// \param n_search Number of neighbours to be included in the k-search, the more the better, but also the more expensive.
/// \param dist_thresh The color distance threshold of points to be cluster together.
/// \param min_cluster_size Minimum number of points in a cluster.
/// \param color_point_cloud Apply random color to new cluster or leave original colors.
std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_color (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    int n_search = 30,
    float dist_tresh = 0.01f,
    int min_cluster_size = 30,
    bool color_point_cloud = true);



PINVOKE void ClusterConnectedComponentRadius (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,


    double voxelSizeSearch_,
    double normalThresholdDegree_,
    int minClusterSize_,
    bool colorPointCloud_,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    int*& c_o, int& c_c_o,
    int*& cluster_o, int& cluster_c_o
);