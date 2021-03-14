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
