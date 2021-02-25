#include "pch.h"
#include "baseCilantroCluster.h"

PINVOKE void ReleaseInt (int* arr, bool isArray) {
    deletePtr (arr, isArray);
}

PINVOKE void ReleaseDouble (double* arr, bool isArray) {
    deletePtr (arr, isArray);
}












///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///                                                                       CLUSTER TECHNIQUES                                                                             ///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/* The goal of point cloud segmentation is to identify points in a cloud with similar features for clustering into their respective regions */

///////////////////////////////////////////
///             HCS CLUSTER             ///
///////////////////////////////////////////

// Source:
/* https://github.com/kzampog/cilantro/blob/master/examples/connected_component_extraction.cpp */

/* "Highly Connected Component Cluster"(?) or "connected-component-extractio-cluster" divides the pointcloud in regions separeted by one particular geometric descriptor.
 * To our best knowledge this is the fastest and unique function in all PC processing libraries.
 * It supports arbitrary point-wise similarity functions and it allows connected components to be found within organized point cloud data, given a comparison function:
 * in this case is based on normal evaluation. This register different cluster regions with abrupt change in normal orientation or similar orientation.
 */

 /* NORMALS are needed for this particular cluster*/

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_connectedComponentRadius (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    float voxel_size_search,
    double normal_threshold_degree,
    int min_cluster_size,
    bool color_point_cloud) {
    // Point search by radius
    cilantro::RadiusNeighborhoodSpecification<float> nh (voxel_size_search * voxel_size_search);

    // Normal evaluate
    cilantro::NormalsProximityEvaluator<float, 3> ev (cilantro_cloud_f->normals, (float)(normal_threshold_degree * (M_PI / 180)));
    // Perform clustering/segmentation
    cilantro::ConnectedComponentExtraction3f<> cce (cilantro_cloud_f->points);
    cce.segment (nh, ev, min_cluster_size, cilantro_cloud_f->size ());

    // Build a color map
    size_t num_labels = cce.getNumberOfClusters ();
    const auto& labels = cce.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = cce.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = cce.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
};

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_connectedComponentKSearch (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    int n_search,
    double normal_threshold_degree,
    int min_cluster_size,
    bool color_point_cloud) {
    // Point search by k neighbours
    cilantro::KNNNeighborhoodSpecification<int> nk_search (n_search);

    // Normal evaluate
    cilantro::NormalsProximityEvaluator<float, 3> ev (cilantro_cloud_f->normals, (float)(normal_threshold_degree * (M_PI / 180)));

    // Perform clustering/segmentation
    cilantro::ConnectedComponentExtraction3f<> cce (cilantro_cloud_f->points);
    cce.segment (nk_search, ev, min_cluster_size, cilantro_cloud_f->size ());

    // Build a color map
    size_t num_labels = cce.getNumberOfClusters ();
    const auto& labels = cce.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = cce.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = cce.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
};
///////////////////////////////////////////
///         EUCLIDEAN CLUSTER           ///
///////////////////////////////////////////

// Source:
/* https://github.com/kzampog/cilantro/blob/master/examples/connected_component_extraction.cpp */

/* From [cilantro: A Lean, Versatile, and Efficient Library for Point Cloud Data Processing, 2018]:
 * We note that common segmentation tasks such as extracting Euclidean (see PCL’s EuclideanClusterExtraction) or smooth (see PCL’s
RegionGrowing) segments can be straightforwardly cast as connected component segmentation under different similarity metrics.

is a greedy region growing algorithm based on nearest neighbors. Cluster affinity is based on a distance to any point of a cluster (cluster tolerance parameter).*/

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_euclideanKsearch (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    int n_search,
    float dist_thresh,
    int min_cluster_size,
    bool color_point_cloud) {
    // Point search by k neighbours
    cilantro::KNNNeighborhoodSpecification<int> nk_search (n_search);

    // Distance evaluator
    cilantro::PointsProximityEvaluator<float> pe (dist_thresh);

    // Perform clustering/segmentation
    cilantro::ConnectedComponentExtraction3f<> cce (cilantro_cloud_f->points);
    cce.segment (nk_search, pe, min_cluster_size, cilantro_cloud_f->size ());

    // Build a color map
    size_t num_labels = cce.getNumberOfClusters ();
    const auto& labels = cce.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = cce.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = cce.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
};

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_euclideanRadius (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    float voxel_size_search,
    float dist_thresh,
    int min_cluster_size,
    bool color_point_cloud) {
    // Point search by radius
    cilantro::RadiusNeighborhoodSpecification<float> nh (voxel_size_search * voxel_size_search);

    // Distance evaluator
    cilantro::PointsProximityEvaluator<float> pe (dist_thresh);

    // Perform clustering/segmentation
    cilantro::ConnectedComponentExtraction3f<> cce (cilantro_cloud_f->points);
    cce.segment (nh, pe, min_cluster_size, cilantro_cloud_f->size ());

    // Build a color map
    size_t num_labels = cce.getNumberOfClusters ();
    const auto& labels = cce.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = cce.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = cce.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
};

///////////////////////////////////////////
///          SPECTRAL CLUSTER           ///
///////////////////////////////////////////

// Source:
/* https://github.com/kzampog/cilantro/blob/master/examples/spectral_clustering.cpp */

/* It is based on Laplacian functions*/

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_spectralCluster (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    size_t max_num_clusters,
    int k_search,
    bool color_point_cloud) {
    // Variables
    cilantro::VectorSet3f points = cilantro_cloud_f->points;
    Eigen::SparseMatrix<float> affinities;

    // Build neighbourhood graph
    auto nn = cilantro::KDTree3f<> (points).search (points, cilantro::KNNNeighborhoodSpecification<> (k_search));
    affinities = cilantro::getNNGraphFunctionValueSparseMatrix (nn, cilantro::RBFKernelWeightEvaluator<float> (), true);

    // Execute spectral cluster
    cilantro::SpectralClustering<float> sc (affinities, max_num_clusters, true, cilantro::GraphLaplacianType::NORMALIZED_RANDOM_WALK);

    // Build a color map
    size_t num_labels = sc.getNumberOfClusters ();
    const auto& labels = sc.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = sc.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = sc.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
}

///////////////////////////////////////////
///           KMEANS CLUSTER            ///
///////////////////////////////////////////

// Source:
/* https://github.com/kzampog/cilantro/blob/master/examples/kmeans.cpp */

/* iteratively finds spherical clusters of points (usually high-dimensional), where cluster affinity is based on a distance to the cluster center.
 * From a math point of view: it chooses centroids that will minimize the squared distances of cluster points to the centroid -
 * and as mentioned, each points belongs to the cluster with the nearest centroid. */

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_kMeans (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    size_t n_cluster,
    size_t max_iter,
    bool use_k_tree,
    bool color_point_cloud) {
    // Variables
    float tol = std::numeric_limits<float>::epsilon ();

    // k-means on point coordinates
    cilantro::KMeans3f<> kmc (cilantro_cloud_f->points);

    // Call k-means cluster
    kmc.cluster (n_cluster, max_iter, tol, use_k_tree);

    // Build a color map
    size_t num_labels = kmc.getNumberOfClusters ();
    const auto& labels = kmc.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = kmc.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = kmc.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
}

///////////////////////////////////////////
///         MEAN-SHIFT CLUSTER          ///
///////////////////////////////////////////

// Source:
/* https://github.com/kzampog/cilantro/blob/master/examples/mean_shift.cpp */

/* From: https://scikit-learn.org/stable/modules/generated/sklearn.cluster.MeanShift.html#sklearn.cluster.MeanShift
 * Mean shift clustering aims to discover “blobs” in a smooth density of samples. It is a centroid-based algorithm, which works by updating candidates for centroids
 * to be the mean of the points within a given region. These candidates are then filtered in a post-processing stage to eliminate near-duplicates to form the final set of centroids.
 */

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_meanShift (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    float kernel_radius,
    int max_iter,
    float cluster_tol,
    bool color_point_cloud) {
    // Execute k-mean shift
    cilantro::MeanShift3f<> ms (cilantro_cloud_f->points);
    ms.cluster (kernel_radius,
        max_iter,
        cluster_tol,
        1E-07,
        cilantro::UnityWeightEvaluator<float> ());

    // Build a color map
    size_t num_labels = ms.getNumberOfClusters ();
    const auto& labels = ms.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

    // Create different cloud per cluster
    num_labels = ms.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = ms.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
};

///////////////////////////////////////////
///            COLOR CLUSTER            ///
///////////////////////////////////////////

std::vector<std::shared_ptr<cilantro::PointCloud3f>> cluster_color (std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f,
    int n_search,
    float dist_tresh,
    int min_cluster_size,
    bool color_point_cloud) {
    // PC colors
    cilantro::ConstDataMatrixMap<float, 3> pc_col (cilantro_cloud_f->colors);

    // Point search by k neighbours
    cilantro::KNNNeighborhoodSpecification<int> nk_search (n_search);

    // Normal evaluate
    cilantro::ColorsProximityEvaluator<float> ce (pc_col, dist_tresh);

    // Perform clustering/segmentation
    cilantro::ConnectedComponentExtraction3f<> cce (cilantro_cloud_f->points);
    cce.segment (nk_search, ce, min_cluster_size, cilantro_cloud_f->size ());

    // Build a color map
    size_t num_labels = cce.getNumberOfClusters ();
    const auto& labels = cce.getPointToClusterIndexMap ();
    cilantro::VectorSet3f color_map (3, num_labels + 1);
    for ( size_t i = 0; i < num_labels; i++ ) {
        color_map.col (i) = Eigen::Vector3f::Random ().cwiseAbs ();
    }

    // No label
    color_map.col (num_labels).setZero ();
    cilantro::VectorSet3f colors (3, labels.size ());
    for ( size_t i = 0; i < colors.cols (); i++ ) {
        colors.col (i) = color_map.col (labels[i]);
    }

    // Create a new unifed colored cloud
    std::shared_ptr<cilantro::PointCloud3f> cloud_seg (new cilantro::PointCloud3f (cilantro_cloud_f->points, cilantro_cloud_f->normals, colors));

    ///////////////////////

// Create different cloud per cluster
    num_labels = cce.getNumberOfClusters ();
    std::cout << "the number of clusters " + std::to_string (num_labels) << std::endl;

    // Get index of clister per point
    auto pt_index_cluster = cce.getPointToClusterIndexMap ();

    // Vector to host individual PCs
    std::vector<std::shared_ptr<cilantro::PointCloud3f>> ptr_clouds_list;

    // Iterate through cloud and create seperate cluster
    auto ptt = cilantro_cloud_f->points;
    int n_pt = (int)ptt.cols ();
    auto col = cilantro_cloud_f->colors;
    auto col_seg = cloud_seg->colors;
    auto nor = cilantro_cloud_f->normals;

    for ( int i = 0; i < num_labels; i++ ) {
        std::shared_ptr<cilantro::PointCloud3f> single_cloud (new cilantro::PointCloud3f);
        for ( int k = 0; k < labels.size (); k++ ) {
            if ( i == labels[k] ) {
                // Push point
                single_cloud->points.conservativeResize (single_cloud->points.rows (), single_cloud->points.cols () + 1);
                single_cloud->points.col (single_cloud->points.cols () - 1) = ptt.col (k);

                // Push color
                if ( color_point_cloud ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col_seg.col (k);
                }
                if ( (color_point_cloud == false) && cilantro_cloud_f->hasColors () ) {
                    single_cloud->colors.conservativeResize (single_cloud->colors.rows (), single_cloud->colors.cols () + 1);
                    single_cloud->colors.col (single_cloud->colors.cols () - 1) = col.col (k);
                }

                // Push normal
                if ( cilantro_cloud_f->hasNormals () ) {
                    single_cloud->normals.conservativeResize (single_cloud->normals.rows (), single_cloud->normals.cols () + 1);
                    single_cloud->normals.col (single_cloud->normals.cols () - 1) = nor.col (k);
                }
            }
        }
        ptr_clouds_list.push_back (single_cloud);
    }

    return ptr_clouds_list;
};


















PINVOKE void ClusterConnectedComponentRadius (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,


    double voxelSizeSearch_,
    double normalThresholdDegree_,
    int minClusterSize_ ,
    bool colorPointCloud_ ,


    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,// track id
    int*& c_o, int& c_c_o,
    int*& cluster_o, int& cluster_c_o
) {


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to CGAL PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   // std::list<PointVectorPair> points;
    std::shared_ptr<cilantro::PointCloud3f> cilantro_cloud_f (new cilantro::PointCloud3f);


    auto ptt = cilantro_cloud_f->points;
    auto nor = cilantro_cloud_f->normals;
    auto col = cilantro_cloud_f->colors;

    //ofstream myfile0;
    //myfile0.open ("C:\\Users\\petra\\AppData\\Roaming\\Grasshopper\\6\\Libraries\\Cockroach\\Step0.txt");
    //myfile0 << p_c << "\n";

    for ( int i = 0; i < p_c; i++ ) {

        Eigen::Vector3f pt_f (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]);
        Eigen::Vector3f nl_f (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2]);
        Eigen::Vector3f cl_f (c[3 * i + 0], c[3 * i + 1], c[3 * i + 2]);

        //myfile0 << pt_f << "\n";
        //myfile0 << nl_f << "\n";
        //myfile0 << cl_f << "\n";

        //myfile0 << "\n";

        ptt.conservativeResize (ptt.rows (), ptt.cols () + 1);
        ptt.col (ptt.cols () - 1) = pt_f;

        nor.conservativeResize (nor.rows (), nor.cols () + 1);
        nor.col (nor.cols () - 1) = nl_f;

        col.conservativeResize (col.rows (), col.cols () + 1);
        col.col (col.cols () - 1) = cl_f;
    }

    cilantro_cloud_f->points = ptt;
    cilantro_cloud_f->normals = nor;
    cilantro_cloud_f->colors = col;


    //myfile0 << cilantro_cloud_f->points.cols () << "\n";
    //myfile0 << cilantro_cloud_f->normals.cols ()  << "\n";
    //myfile0 << cilantro_cloud_f->colors.cols () << "\n";


    //    auto ptt_ = cilantro_cloud_f->points;
    //    int n_pt_ = (int)ptt.cols ();
    //    auto col_ = cilantro_cloud_f->colors;
    //    auto nor_ = cilantro_cloud_f->normals;
    //for ( int i = 0; i < n_pt_; i++ ) {

    //            Eigen::Vector3d pt_d_ = ptt.col (i).cast<double> ();
    //    Eigen::Vector3d no_d_ = nor.col (i).cast <double> ();
    //    Eigen::Vector3d cl_d_ = col.col (i).cast <double> ();

    //    myfile0 << pt_d_ << "\n";
    //  myfile0 << no_d_ << "\n";
    //  myfile0 << cl_d_  << "\n";
    //  myfile0 << "\n";
    //}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run CILANTRO Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
     // Pre-defined values
    double voxelSizeSearch = voxelSizeSearch_ == 0 ?  0.1 : voxelSizeSearch_;
    double normalThresholdDegree = normalThresholdDegree_ == 0 ? 2.0 : normalThresholdDegree_;
    int minClusterSize = minClusterSize_ == 0 ? 100 : minClusterSize_;
    bool colorPointCloud = true;



    //// Run the cluster

    std::vector<std::shared_ptr<cilantro::PointCloud3f>> cilantro_list;
    cilantro_list = cluster_connectedComponentRadius (cilantro_cloud_f, voxelSizeSearch, normalThresholdDegree,   minClusterSize,  colorPointCloud);


   //myfile0 << voxelSizeSearch << "\n";
   //myfile0 << normalThresholdDegree << "\n";
   //myfile0 << minClusterSize << "\n";
   //myfile0 << colorPointCloud << "\n";
   // myfile0 << "Number of clusters:"<<"\n";
   // myfile0 << cilantro_list.size();
 

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////Run Open3D Method
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    if ( cilantro_list.size () > 0 ){

        int a = 0;
        int b = 0;
        int cc = 0;
        //int clusterc = 0;

        p_c_o = 0;
        n_c_o = 0;
        c_c_o = 0;


        cluster_c_o = cilantro_list.size ();
        cluster_o = new int[cluster_c_o];

        for ( int i = 0; i < cilantro_list.size (); i++ ) {
            p_c_o += cilantro_list[i]->points.cols () * 3;
            n_c_o += cilantro_list[i]->normals.cols () * 3;
            c_c_o += cilantro_list[i]->colors.cols () * 3;

            cluster_o[i] = cilantro_list[i]->points.cols ();
          
        }
        p_o = new double[p_c_o];
        n_o = new double[n_c_o];
        c_o = new int[c_c_o];
 

        //myfile0 << "\n count" << p_c_o << "\n";
        //myfile0 << "\n count" << n_c_o << "\n";
        //myfile0 << "\n count" << c_c_o << "\n";
        //myfile0 << "\n count" << cluster_c_o << "\n";

        for ( int i = 0; i < cilantro_list.size (); i++ ) {


            auto ptt_ = cilantro_list[i]->points;
            int n_pt_ = (int)ptt_.cols ();
            auto col_ = cilantro_list[i]->colors;
            auto nor_ = cilantro_list[i]->normals;

            //myfile0 << "\n count" << (int)ptt_.cols () << "\n";
            //myfile0 << "\n count" << (int)col_.cols () << "\n";
            //myfile0 << "\n count" << (int)nor_.cols () << "\n";

            int random_integer_red = rand () % 255;
            int random_integer_green = rand () % 255;
            int random_integer_blu = rand () % 255;
            for ( int j = 0; j < n_pt_; j++ ) {

                // Convert points
                Eigen::Vector3d pt_d = ptt_.col (j).cast<double> ();
                Eigen::Vector3d no_d = nor_.col (j).cast <double> ();
                Eigen::Vector3d cl_d = col_.col (j).cast <double> ();

               // cluster_o[clusterc++] = i;

                p_o[a++] = pt_d.x ();
                p_o[a++] = pt_d.y ();
                p_o[a++] = pt_d.z ();

                n_o[b++] = no_d.x ();
                n_o[b++] = no_d.y ();
                n_o[b++] = no_d.z ();

           /*     c_o[cc++] = cl_d.x ();
                c_o[cc++] = cl_d.y ();
                c_o[cc++] = cl_d.z ();*/

                c_o[cc++] = random_integer_red;
                c_o[cc++] = random_integer_green;
                c_o[cc++] = random_integer_blu;


                //myfile0 << "\n position" << pt_d.x () ;
                //myfile0 << "\n position" << pt_d.y () ;
                //myfile0 << "\n position" << pt_d.z ();


                //myfile0 << "\n normal" << no_d.x () ;
                //myfile0 << "\n normal" << no_d.y ()  ;
                //myfile0 << "\n normal" << no_d.z ()  ;


  /*              myfile0 << " color" << random_integer_red;
                myfile0 << " color" << random_integer_green;
                myfile0 << " color" << random_integer_blu;*/
            }

           // myfile0 << "\n count" << n_pt_ << "\n";



        }









    //for(int i = 0; i<1; i++){

    //

    //    p_c_o = cilantro_list[0]->points.cols()*3 ;
    //    p_o = new double[p_c_o];

    //    n_c_o = cilantro_list[0]->normals.cols () * 3;
    //    n_o = new double[n_c_o];

    //    c_c_o = cilantro_list[0]->colors.cols () * 3;
    //    c_o = new double[c_c_o];

    //    int a = 0;
    //    int b = 0;
    //    int cc = 0;

    //    auto ptt_ = cilantro_list[0]->points;
    //    int n_pt_ = (int)ptt_.cols ();
    //    auto col_ = cilantro_list[0]->colors;
    //    auto nor_ = cilantro_list[0] ->normals;

    //    myfile0 << "\n count" << (int)ptt_.cols () << "\n";
    //    myfile0 << "\n count" << (int)col_.cols () << "\n";
    //    myfile0 << "\n count" << (int)nor_.cols () << "\n";
    //    for ( int i = 0; i < n_pt_; i++ ) {

    //        // Convert points
    //        Eigen::Vector3d pt_d = ptt_.col (i).cast<double> ();
    //        Eigen::Vector3d no_d = nor_.col (i).cast <double> ();
    //        Eigen::Vector3d cl_d = col_.col (i).cast <double> ();


    //        p_o[a++] = pt_d.x();
    //        p_o[a++] = pt_d.y ();
    //        p_o[a++] = pt_d.z ();

    //        n_o[b++] = no_d.x ();
    //        n_o[b++] = no_d.y ();
    //        n_o[b++] = no_d.z ();

    //        c_o[cc++] = cl_d.x ();
    //        c_o[cc++] = cl_d.y ();
    //        c_o[cc++] = cl_d.z ();
    //        

    //       
    //    }

    //    myfile0 << "\n count" << n_pt_ << "\n";



    //}

    }else{

        p_c_o = 3;
        p_o = new double[p_c_o];

        n_c_o =  3;
        n_o = new double[n_c_o];

        c_c_o = 3;
        c_o = new int[c_c_o];

        cluster_c_o = 1;
        cluster_o = new int[1]{1};

            p_o[0] = 0;
            p_o[1] = 0;
            p_o[2] = 0;

            n_o[0] = 0;
            n_o[1] = 0;
            n_o[2] = 1;

            c_o[0] = 0;
            c_o[1] = 0;
            c_o[2] = 0;



       
    }
    //myfile0.close ();
}