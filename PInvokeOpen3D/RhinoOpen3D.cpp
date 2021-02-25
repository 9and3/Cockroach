#include "RhinoOpen3D.h"


std::shared_ptr<open3d::geometry::PointCloud> Convert_RhinoCloudToOpen3D (const ON_PointCloud* cloud) {

    std::shared_ptr<open3d::geometry::PointCloud> Open3DCloud (new open3d::geometry::PointCloud);

    for ( int i = 0; i < cloud->m_P.Count (); i++ ) {
        auto p = cloud->m_P.At (i);
        Open3DCloud->points_.push_back (Eigen::Vector3d (p->x, p->y, p->z));

        if ( cloud->HasPointColors () ) {
            auto c = cloud->m_C.At (i);
            Open3DCloud->colors_.push_back (
                Eigen::Vector3d (c->Red (), c->Green (), c->Blue ()));
        }

        if ( cloud->HasPointNormals () ) {
            auto n = cloud->m_N.At (i);
            Open3DCloud->normals_.push_back (Eigen::Vector3d (n->x, n->y, n->z));
        } /* else {
             Open3DCloud->normals_.push_back(
                     Eigen::Vector3d(0, 0, 1));
         }*/
    }

    return Open3DCloud;
}

ON_PointCloud Convert_Open3DToRhinoCloud (std::shared_ptr<open3d::geometry::PointCloud> Open3DCloud) {
    ON_PointCloud cloudNew;

    for ( auto& p : Open3DCloud->points_ ) {
        ON_3dPoint pt (p.x (), p.y (), p.z ());
        cloudNew.m_P.Append (pt);
    }

    if ( Open3DCloud->colors_.size () == Open3DCloud->points_.size () ) {
        for ( auto& p : Open3DCloud->colors_ ) {
            ON_Color pt (p.x (), p.y (), p.z ());
            cloudNew.m_C.Append (pt);
        }
    }

    if ( Open3DCloud->normals_.size () == Open3DCloud->points_.size () ) {
        for ( auto& p : Open3DCloud->normals_ ) {
            ON_3dVector pt (p.x (), p.y (), p.z ());
            cloudNew.m_N.Append (pt);
        }
    }

    return cloudNew;
}


PINVOKE void Open3DDownsampleRhinoSDK (ON_PointCloud* c0, int numberOfPoints, ON_PointCloud& c1 ) {


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //ON_PointCloud to open3d::geometry::PointCloud
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        shared_ptr<open3d::geometry::PointCloud> Open3DCloud = Convert_RhinoCloudToOpen3D(c0);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Run Open3D Method
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        int nOfPoints = numberOfPoints;
        int b = (int)(Open3DCloud->points_.size () * (1.0 / numberOfPoints * 1.0));
        int nth = max (1, b);
        Open3DCloud = Open3DCloud->UniformDownSample (nth);

       ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //open3d::geometry::PointCloud to ON_PointCloud
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ON_PointCloud RhinoCloud = Convert_Open3DToRhinoCloud (Open3DCloud);
        c1 = RhinoCloud;
   

}
