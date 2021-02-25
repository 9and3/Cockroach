#include "base.h"


typedef open3d::geometry::PointCloud PC;

using namespace std;

PINVOKE int Test_GetSquare (int n) {
	return n*n;
}


PINVOKE void ReleaseInt (int* arr, bool isArray) {
    deletePtr (arr, isArray);
}

PINVOKE void ReleaseDouble (double* arr, bool isArray) {
    deletePtr (arr, isArray);
}





PINVOKE void Open3DDownsample (
    double* p, size_t p_c, 
    double* n, size_t n_c, 
    double* c, size_t c_c,

    double voxelsize,

    double*& p_o, int& p_c_o, 
    double*& n_o, int& n_c_o, 
    double*& c_o, int& c_c_o) {
 

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to Open3D PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    shared_ptr<PC> Open3DCloud (new PC);
    //ofstream myfile;
    //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_MeshSkeleton.txt");

    //ofstream myfile0;
    //myfile0.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\PInvokeCSharp\\bin\\x64\\Debug1.txt");
    //myfile0 << p_c;
    //myfile0 << n_c;
    //myfile0 << c_c;
    //myfile0.close ();

    //ofstream myfile1;
    //myfile1.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\PInvokeCSharp\\bin\\x64\\Debug2.txt");
    Open3DCloud->points_.resize (p_c);
    Open3DCloud->normals_.resize (p_c);
    Open3DCloud->colors_.resize (p_c);



    for ( size_t i = 0; i < p_c; i++ ) {

        Open3DCloud->points_[i] = Eigen::Vector3d (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]);
        Open3DCloud->normals_[i] = Eigen::Vector3d (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2]);
        Open3DCloud->colors_[i] = Eigen::Vector3d (c[3 * i + 0], c[3 * i + 1], c[3 * i + 2]);

        //Open3DCloud->points_.push_back (Eigen::Vector3d (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]));
        //Open3DCloud->normals_.push_back (Eigen::Vector3d (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2]));
        //Open3DCloud->colors_.push_back (Eigen::Vector3d (c[3 * i + 0], c[3 * i + 1], c[3 * i + 2]));



        //myfile1 << "\n";
        //myfile1 << p[3 * i + 0];
        //myfile1 << p[3 * i + 1];
        //myfile1 << p[3 * i + 2];

        //myfile1 << n[3 * i + 0];
        //myfile1 << n[3 * i + 1];
        //myfile1 << n[3 * i + 2];

        //myfile1 << c[3 * i + 0];
        //myfile1 << c[3 * i + 1];
        //myfile1 << c[3 * i + 2];
    }

    //myfile1.close ();


    //ofstream myfile2;
    //myfile2.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\PInvokeCSharp\\bin\\x64\\Debug3.txt");
    //myfile2 << "\n Number of Points: ";
    //myfile2 << Open3DCloud->points_.size ();
    //myfile2.close ();







    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //int nOfPoints = numberOfPoints;
    //int b = (int)(Open3DCloud->points_.size () * (1.0 / numberOfPoints *1.0));
    //int nth = max (1,b );

    //ofstream myfile3;
    //myfile3.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\PInvokeCSharp\\bin\\x64\\Debug4.txt");
    //myfile3 << "\n Number of Points: ";
    //myfile3 << Open3DCloud->points_.size ();
    //myfile3.close ();


   // Open3DCloud = Open3DCloud->UniformDownSample (nth);

    Open3DCloud = Open3DCloud->VoxelDownSample (voxelsize);
    //ofstream myfile4;
    //myfile4.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\PInvokeCSharp\\bin\\x64\\Debug5.txt");
    //myfile4 << "\n Number of Points: ";
    //myfile4 << Open3DCloud->points_.size ();
    //myfile4.close ();


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    p_c_o = Open3DCloud->points_.size ()*3;
    p_o = new double[p_c_o];

    n_c_o = Open3DCloud->normals_.size ()*3 ;
    n_o = new double[n_c_o];

    c_c_o = Open3DCloud->colors_.size ()*3 ;
    c_o = new double[c_c_o];

    int i = 0;
    for ( auto& p : Open3DCloud->points_ ) {
        p_o[i++] = p.x ();
        p_o[i++] = p.y ();
        p_o[i++] = p.z ();

        //myfile << p.x ();
        //myfile << p.y ();
        //myfile << p.z ();
    }

    i = 0;
    for ( auto& p : Open3DCloud->normals_ ) {
        n_o[i++] = p.x ();
        n_o[i++] = p.y ();
        n_o[i++] = p.z ();

        //myfile << p.x ();
        //myfile << p.y ();
        //myfile << p.z ();
    }

    i = 0;
    for ( auto& p : Open3DCloud->colors_ ) {
        c_o[i++] = p.x ();
        c_o[i++] = p.y ();
        c_o[i++] = p.z ();

        //myfile << p.x ();
        //myfile << p.y ();
        //myfile << p.z ();
    }


    //myfile.close ();

}














PINVOKE void Open3DNormals (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double radius,
    int iterations,
    int neighbours,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o) {


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to Open3D PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    shared_ptr<PC> Open3DCloud (new PC);

    Open3DCloud->points_.resize (p_c);
    Open3DCloud->normals_.resize (p_c);
    Open3DCloud->colors_.resize (p_c);



    for ( size_t i = 0; i < p_c; i++ ) {

        Open3DCloud->points_[i] = Eigen::Vector3d (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]);
        Open3DCloud->normals_[i] = Eigen::Vector3d (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2]);
        Open3DCloud->colors_[i] = Eigen::Vector3d (c[3 * i + 0], c[3 * i + 1], c[3 * i + 2]);
    }

 




    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


  
    Open3DCloud->EstimateNormals (open3d::geometry::KDTreeSearchParamHybrid (radius, iterations), true);
    int numberOfCP = min (1000, max (10, (int)(neighbours)));
    Open3DCloud->OrientNormalsConsistentTangentPlane (numberOfCP);




    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    p_c_o = Open3DCloud->points_.size () * 3;
    p_o = new double[p_c_o];

    n_c_o = Open3DCloud->normals_.size () * 3;
    n_o = new double[n_c_o];

    c_c_o = Open3DCloud->colors_.size () * 3;
    c_o = new double[c_c_o];

    int i = 0;
    for ( auto& p : Open3DCloud->points_ ) {
        p_o[i++] = p.x ();
        p_o[i++] = p.y ();
        p_o[i++] = p.z ();
    }

    i = 0;
    for ( auto& p : Open3DCloud->normals_ ) {
        n_o[i++] = p.x ();
        n_o[i++] = p.y ();
        n_o[i++] = p.z ();
    }

    i = 0;
    for ( auto& p : Open3DCloud->colors_ ) {
        c_o[i++] = p.x ();
        c_o[i++] = p.y ();
        c_o[i++] = p.z ();
    }











}


typedef open3d::geometry::PointCloud PC;
typedef open3d::geometry::TriangleMesh Open3DMesh;


PINVOKE void Open3DPoisson (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    size_t PoisonMaxDepth,
    size_t PoisonMinDepth,
    float PoisonScale,
    bool Linear,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o,
    double*& density, int& density_c
    ) {


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to Open3D PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    shared_ptr<PC> Open3DCloud (new PC);

    Open3DCloud->points_.resize (p_c);
    Open3DCloud->normals_.resize (p_c);
    Open3DCloud->colors_.resize (p_c);



    for ( size_t i = 0; i < p_c; i++ ) {

        Open3DCloud->points_[i] = Eigen::Vector3d (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]);
        Open3DCloud->normals_[i] = Eigen::Vector3d (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2]);
        Open3DCloud->colors_[i] = Eigen::Vector3d (c[3 * i + 0], c[3 * i + 1], c[3 * i + 2]);
    }






    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    auto poissonReconstruction = open3d::geometry::TriangleMesh::CreateFromPointCloudPoisson (*Open3DCloud, PoisonMaxDepth, PoisonMinDepth, PoisonScale, false, -1);
    std::shared_ptr<open3d::geometry::TriangleMesh> meshOpen3D (new open3d::geometry::TriangleMesh);  

    meshOpen3D = std::get<0> (poissonReconstruction);
    auto densityVector = std::get<1> (poissonReconstruction);






    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        p_c_o = meshOpen3D->vertices_.size () * 3;
        p_o = new double[p_c_o];

        n_c_o = meshOpen3D->triangles_.size () * 3;
        n_o = new double[n_c_o];

        c_c_o = meshOpen3D->vertices_.size () * 3;
        c_o = new double[c_c_o];

        density_c = densityVector.size();
        density = new double[density_c];



        int i = 0;
        for ( auto v = meshOpen3D->vertices_.begin (); v != meshOpen3D->vertices_.end (); ++v ) {
            p_o[i++] = v->x ();
            p_o[i++] = v->y ();
            p_o[i++] = v->z ();
        }

        i = 0;
        for ( auto v = meshOpen3D->triangles_.begin (); v != meshOpen3D->triangles_.end (); ++v ) {
            n_o[i++] = v->x ();
            n_o[i++] = v->y ();
            n_o[i++] = v->z ();
        }

        i = 0;
        if ( meshOpen3D->HasVertexColors () ) {
            for ( auto v = meshOpen3D->vertex_colors_.begin (); v != meshOpen3D->vertex_colors_.end (); ++v ) {
                c_o[i++] = v->x ();
                c_o[i++] = v->y ();
                c_o[i++] = v->z ();
            }
        }

        i = 0;
        for ( auto& v : densityVector ) {
            density[i++] = v;
        }




}






PINVOKE void Open3DMeshPopulate (
    double* p, size_t p_c,
    int* f, size_t f_c,

    int type,
    int n,

    double*& p_o, int& p_c_o) {




    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to Open3D Mesh
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    auto meshOpen3D = std::make_shared<open3d::geometry::TriangleMesh> ();

    for ( size_t i = 0; i < p_c; i++ ) {

   
        auto v = Eigen::Vector3d (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]);
        meshOpen3D->vertices_.push_back (v);
    }
        for ( size_t i = 0; i < f_c; i++ ) {
        meshOpen3D->triangles_.push_back (Eigen::Vector3i (f[3 * i + 0], f[3 * i + 1], f[3 * i + 2]));
    }


    ////////////////////////////////////////////////////////////////
    // Convert Rhino Mesh --> Populate points
    ////////////////////////////////////////////////////////////////
    bool TriangleNormal = type == 0 ? false : true;
    auto Open3DCloud = type>1 ? meshOpen3D->SamplePointsPoissonDisk (n) :  meshOpen3D->SamplePointsUniformly (n, TriangleNormal, 1);


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   //Output
   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    p_c_o = Open3DCloud->points_.size () * 3;
    p_o = new double[p_c_o];


    int i = 0;
    for ( auto& p : Open3DCloud->points_ ) {
        p_o[i++] = p.x ();
        p_o[i++] = p.y ();
        p_o[i++] = p.z ();
    }





}



PINVOKE void RANSACPlane (//0 //100 //1000000
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double  distanceThreshold_,
    int neighbours_,
    int iterations_,
    bool InliersOrOutliers,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o

){


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to Open3D PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    shared_ptr<PC> Open3DCloud (new PC);

    Open3DCloud->points_.resize (p_c);
    Open3DCloud->normals_.resize (p_c);
    Open3DCloud->colors_.resize (p_c);


    for ( size_t i = 0; i < p_c; i++ ) {
        Open3DCloud->points_[i] = Eigen::Vector3d (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]);
        Open3DCloud->normals_[i] = Eigen::Vector3d (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2]);
        Open3DCloud->colors_[i] = Eigen::Vector3d (c[3 * i + 0], c[3 * i + 1], c[3 * i + 2]);
    }


    ////////////////////////////////////////////////////////////////
    // Plane Segmentation
    ////////////////////////////////////////////////////////////////
    double distanceThreshold = distanceThreshold_<0 ? 0 : distanceThreshold_;
    int neighbours = neighbours_ < 3 ? 3 : neighbours_;
    int iterations = iterations_<1 ? 1 : iterations_;

    std::shared_ptr<PC> Open3DCloud_inliers (new PC ());
    std::shared_ptr<PC> Open3DCloud_outliers (new PC ());
    std::tuple<Eigen::Vector4d, std::vector<size_t>> segmentation = Open3DCloud->SegmentPlane (distanceThreshold, neighbours, iterations);
    Eigen::Vector4d plane = std::get<0> (segmentation);
    std::vector<size_t> inliersIndex = std::get<1> (segmentation);

    Open3DCloud_outliers = Open3DCloud->SelectByIndex (inliersIndex, true); // Select out of plane points
    Open3DCloud_inliers = Open3DCloud->SelectByIndex (inliersIndex, false); // Select in plane points
    //RhinoApp ().Print (L"Cockroach: NumberofPointsOpen3D out-of-plane = %g\n", Open3DCloud_outliers->points_.size ());
    //RhinoApp ().Print (L"Cockroach: NumberofPointsOpen3D in-plane = %g\n", Open3DCloud_inliers->points_.size ());

    
    Open3DCloud = InliersOrOutliers ?  Open3DCloud_inliers : Open3DCloud_outliers;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   //Output
   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    p_c_o = Open3DCloud->points_.size () * 3;
    p_o = new double[p_c_o];

    n_c_o = Open3DCloud->normals_.size () * 3;
    n_o = new double[n_c_o];

    c_c_o = Open3DCloud->colors_.size () * 3;
    c_o = new double[c_c_o];

    int i = 0;
    for ( auto& p : Open3DCloud->points_ ) {
        p_o[i++] = p.x ();
        p_o[i++] = p.y ();
        p_o[i++] = p.z ();
    }

    i = 0;
    for ( auto& p : Open3DCloud->normals_ ) {
        n_o[i++] = p.x ();
        n_o[i++] = p.y ();
        n_o[i++] = p.z ();
    }

    i = 0;
    for ( auto& p : Open3DCloud->colors_ ) {
        c_o[i++] = p.x ();
        c_o[i++] = p.y ();
        c_o[i++] = p.z ();
    }




}