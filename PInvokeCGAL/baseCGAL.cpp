#include "baseCGAL.h"


PINVOKE void ReleaseInt (int* arr, bool isArray) {
	deletePtr (arr, isArray);
}

PINVOKE void ReleaseDouble (double* arr, bool isArray) {
	deletePtr (arr, isArray);
}


vec3::vec3 (double a, double b, double c)
    : x (a), y (b), z (c) {}








typedef CGAL::Simple_cartesian<double> Kernel;
typedef Kernel::Point_3 Point;
typedef CGAL::Polyhedron_3<Kernel> Polyhedron;

typedef boost::graph_traits<Polyhedron>::vertex_descriptor vertex_descriptor;

typedef CGAL::Mean_curvature_flow_skeletonization<Polyhedron> Skeletonization;
typedef Skeletonization::Skeleton Skeleton;

typedef Skeleton::vertex_descriptor Skeleton_vertex;
typedef Skeleton::edge_descriptor Skeleton_edge;
typedef Polyhedron::HalfedgeDS HalfedgeDS;

// Normal Estimation Types
typedef CGAL::Exact_predicates_inexact_constructions_kernel Kernel2;
typedef Kernel::Point_3 Point2;
typedef Kernel::Vector_3 Vector2;
typedef std::pair<Kernel2::Point_3, Kernel2::Vector_3> PointVectorPair; // Point with normal vector stored in a std::pair.
typedef CGAL::Parallel_if_available_tag Concurrency_tag;// Concurrency
typedef CGAL::Polyhedron_3<Kernel2> Polyhedron2;
typedef CGAL::Polyhedron_3<Kernel2, CGAL::Polyhedron_items_with_id_3>  Polyhedron3;

typedef CGAL::Point_set_3<Point> Point_set;

PINVOKE void MeshBoolean_Create (

    double* coord_mesh1, size_t n_coord_mesh1,//i.e. [x0, y0, z0, x1, y1, z1] , 2
    int* faces_mesh1, size_t n_faces_mesh1, //[v0, v1, v2, v0, v1, v2], 2

    double* coord_mesh2, size_t n_coord_mesh2,
    int* faces_mesh2, size_t n_faces_mesh2,

    size_t Difference_Union_Intersection,

    double*& coord_out, int& n_coord_out,
    int*& faces_out, int& n_faces_out

) {

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Create Mesh
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3> mesh1, mesh2, out;
    mesh1.clear ();
    mesh2.clear ();
    mesh1.reserve (n_coord_mesh1, n_coord_mesh1 * 3, n_faces_mesh1);
    mesh2.reserve (n_coord_mesh2, n_coord_mesh2 * 3, n_faces_mesh2);

    for ( size_t i = 0; i < n_coord_mesh1; i++ ) {
        mesh1.add_vertex (CGAL::Epick::Point_3 (coord_mesh1[3 * i], coord_mesh1[3 * i + 1], coord_mesh1[3 * i + 2]));
    }


    for ( size_t i = 0; i < n_faces_mesh1; i++ ) {
        mesh1.add_face (CGAL::SM_Vertex_index (faces_mesh1[3 * i + 0]), CGAL::SM_Vertex_index (faces_mesh1[3 * i + 1]), CGAL::SM_Vertex_index (faces_mesh1[3 * i + 2]));
    }



    for ( size_t i = 0; i < n_coord_mesh2; i++ ) {
        mesh2.add_vertex (CGAL::Epick::Point_3 (coord_mesh2[3 * i], coord_mesh2[3 * i + 1], coord_mesh2[3 * i + 2]));
    }


    for ( size_t i = 0; i < n_faces_mesh2; i++ ) {
        mesh2.add_face (CGAL::SM_Vertex_index (faces_mesh2[3 * i + 0]), CGAL::SM_Vertex_index (faces_mesh2[3 * i + 1]), CGAL::SM_Vertex_index (faces_mesh2[3 * i + 2]));
    }

    ////////////////////////////////////////////////////////////////
    // Check if Converted meshes are valid
    ////////////////////////////////////////////////////////////////
    if ( !mesh1.is_valid (false) || !mesh2.is_valid (false) ) {
        //RhinoApp ().Print (L"Cockroach: Invalid Mesh\n");
        std::ofstream myfile;
        myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP//%g_DLL\\bin\\x64\\Step1.txt");
        myfile << "Check if Converted meshes are valid - Not valid .\n";
        myfile.close ();

        return;
    } else {
        //std::ofstream myfile;
        //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\Step1.txt");
        // auto  string = "Check if Converted meshes are valid - Valid.";
        // myfile << string << "\n";
        // myfile << mesh1.number_of_vertices () << "\n";
        // myfile << mesh1.number_of_faces () << "\n";
        // myfile << mesh2.number_of_vertices () << "\n";
        // myfile << mesh2.number_of_faces () << "\n";
        //myfile.close ();
    }


    ////////////////////////////////////////////////////////////////
    // Perform CGAL Boolean
    ////////////////////////////////////////////////////////////////
    bool valid_union = false;
    try {
        if ( Difference_Union_Intersection == 1 ) {
            valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_union (mesh1, mesh2, out, CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));
        } else if ( Difference_Union_Intersection == 2 ) {
            valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_intersection (mesh1, mesh2, out, CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));
        } else {
            valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_difference (mesh1, mesh2, out, CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));
        }
    } catch ( const std::exception& e ) {

    }



    if ( valid_union ) {
        //RhinoApp ().Print (L"Cockroach: Union was successfully computed\n");

        //std::ofstream myfile;
        //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\Step2.txt");
        //myfile << "Cockroach: Union was successfully computed\n";
        //myfile << out.number_of_vertices () << "\n";
        //myfile << out.number_of_faces () << "\n";;
        //myfile.close ();
    } else {
        std::ofstream myfile;
        myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\Step2.txt");
        myfile << "Cockroach: Union could not be computed\n";
        myfile.close ();
        //RhinoApp ().Print (L"Cockroach: Union could not be computed\n");
        return;
    }


    ////////////////////////////////////////////////////////////////
    // Return vertex and face array
    ////////////////////////////////////////////////////////////////

    //convex_hull hull (coord, nPts);
    //nFaces = hull.num_faces ();
    //hull.copy_faces (faceIndices);


    //Get vertices coordinates
    coord_out = new double[out.number_of_vertices () * 3];


    int i = 0;
    int c = 0;
    for ( auto vi : out.vertices () ) {
        auto pt = out.point (vi);
        coord_out[i++] = (double)pt.x ();
        coord_out[i++] = (double)pt.y ();
        coord_out[i++] = (double)pt.z ();
        c++;
    }
    n_coord_out = c;




    //Get face indices 
    faces_out = new int[out.number_of_faces () * 3];


    i = 0;
    c = 0;
    for ( auto face_index : out.faces () ) {
        std::vector<uint32_t> indices;
        CGAL::Vertex_around_face_circulator<CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>> vcirc (out.halfedge (face_index), out), done (vcirc);
        do indices.push_back (*vcirc++); while ( vcirc != done );
        //meshRebuilt.SetTriangle (face_index.idx (), indices[0], indices[1], indices[2]);

        faces_out[i++] = (int)indices[0];
        faces_out[i++] = (int)indices[1];
        faces_out[i++] = (int)indices[2];
        c++;
    }

    n_faces_out = c;

}









//To track colors
//https://github.com/CGAL/cgal/blob/master/Polygon_mesh_processing/examples/Polygon_mesh_processing/corefinement_mesh_union_with_attributes.cpp
PINVOKE void MeshBoolean_CreateArray (


    double* coord_mesh, int* n_coord_meshArray,//Flat array of coordinates / 0 256 512 flat array of vertices array / number of meshes
    int* faces_mesh, int* n_faces_meshArray,
    size_t n_mesh,

    size_t Difference_Union_Intersection,

    double*& coord_out, int& n_coord_out,
    int*& faces_out, int& n_faces_out,
    int& nValidMeshes

) {

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Create Mesh
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    std::vector< CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>> meshList;
    CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>  out;


    int numOfValidMeshes = 0;
    int numOfMeshes = 0;
    //std::ofstream myfile;

    //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\StepA.txt");
    for ( size_t i = 0; i < n_mesh; i++ ) {


        CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>  tempMesh;
        // tempMesh.reserve(n_coord_meshArray[i+1]-n_coord_meshArray[i], (n_coord_meshArray[i+1]-n_coord_meshArray[i]) * 3, n_faces_meshArray[i+1]-n_faces_meshArray[i]);


         //myfile << "\n n_coord_meshArray: ";
         //myfile << n_coord_meshArray[i];
         //myfile << "\n n_coord_meshArray:";
         //myfile << n_coord_meshArray[i + 1];

        for ( int j = n_coord_meshArray[i]; j < n_coord_meshArray[i + 1]; j++ ) {

            //myfile << "\n ";
            //myfile << coord_mesh[3 * j + 0];
            //myfile << "\n ";
            //myfile << coord_mesh[3 * j + 1];
            //myfile << "\n ";
            //myfile << coord_mesh[3 * j + 2];
            tempMesh.add_vertex (CGAL::Epick::Point_3 (coord_mesh[3 * j + 0], coord_mesh[3 * j + 1], coord_mesh[3 * j + 2]));

        }

        //myfile << "\n ";
        //myfile << "\n n_faces_meshArray: ";
        //myfile << n_faces_meshArray[i];
        //myfile << "\n n_faces_meshArray:";
        //myfile << n_faces_meshArray[i + 1];
        for ( int j = n_faces_meshArray[i]; j < n_faces_meshArray[i + 1]; j++ ) {




            tempMesh.add_face (CGAL::SM_Vertex_index (faces_mesh[3 * j + 0]), CGAL::SM_Vertex_index (faces_mesh[3 * j + 1]), CGAL::SM_Vertex_index (faces_mesh[3 * j + 2]));

        }



        if ( tempMesh.is_valid (false) ) {
            numOfValidMeshes++;
            meshList.push_back (tempMesh);
        }
        //myfile << "\n ";
        numOfMeshes++;
    }
    //  myfile.close ();

    nValidMeshes = numOfValidMeshes;
    // std::ofstream myfile;
     //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\Step1.txt");
     //myfile << "\n Number of valid meshes: ";
     //myfile << numOfValidMeshes;
     //myfile << "\n numOfMeshes:";
     //myfile << numOfMeshes;
     //myfile << "\n n_mesh:";
     //myfile << n_mesh;
     //myfile << "\n List of Meshes:";
     //myfile << meshList.size();




     //////////////////////////////////////////////////////////////////
     //// Perform CGAL Boolean
     //////////////////////////////////////////////////////////////////

     //CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>  mesh0 = meshList[0];
     //CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>  mesh1 = meshList[1];
     //bool valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_union (mesh0, mesh1, out);
     //out = meshList[0];


    CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>  lastMesh = meshList[0];
    for ( int i = 1; i < meshList.size (); i++ ) {//meshList.size ()

        bool valid_union = false;
        const bool throw_on_self_intersection = true;
        //const bool NamedParameters1(1) = ;
      // const throw_on_self_intersection (true);
        try {
            if ( Difference_Union_Intersection == 1 ) {
                valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_union (lastMesh, meshList[i], out, CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));//, throw_on_self_intersection
            } else if ( Difference_Union_Intersection == 2 ) {
                valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_intersection (lastMesh, meshList[i], out, CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));
            } else {
                valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_difference (lastMesh, meshList[i], out, CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));
            }
            if ( valid_union ) {
                //myfile << "\n Valid Boolean";
                lastMesh = out;
            } else {
                //myfile << "\n Not Valid Boolean";
                lastMesh = meshList[i];
            }
        } catch ( const std::exception& e ) {
            lastMesh = meshList[i];
        }
        out.clear ();
    }
    out = lastMesh;


    //myfile.close ();

    //if ( valid_union ) {
    //    //RhinoApp ().Print (L"Cockroach: Union was successfully computed\n");

    //    //std::ofstream myfile;
    //    //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\Step2.txt");
    //    //myfile << "Cockroach: Union was successfully computed\n";
    //    //myfile << out.number_of_vertices () << "\n";
    //    //myfile << out.number_of_faces () << "\n";;
    //    //myfile.close ();
    //} else {
    //    std::ofstream myfile;
    //    myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\Step2.txt");
    //    myfile << "Cockroach: Union could not be computed\n";
    //    myfile.close ();
    //    //RhinoApp ().Print (L"Cockroach: Union could not be computed\n");
    //    return;
    //}


    ////////////////////////////////////////////////////////////////
    // Return vertex and face array
    ////////////////////////////////////////////////////////////////


    //Get vertices coordinates
    const int vc = out.number_of_vertices () * 3;
    coord_out = new double[vc];


    int i = 0;
    int c = 0;
    for ( const auto& vi : out.vertices () ) {
        const auto& pt = out.point (vi);
        coord_out[i++] = (double)pt.x ();
        coord_out[i++] = (double)pt.y ();
        coord_out[i++] = (double)pt.z ();
        c++;
    }
    n_coord_out = c;




    //Get face indices 
    const int fc = out.number_of_faces () * 3;
    faces_out = new int[fc];


    i = 0;
    c = 0;
    for ( auto face_index : out.faces () ) {
        std::vector<uint32_t> indices;
        CGAL::Vertex_around_face_circulator<CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>> vcirc (out.halfedge (face_index), out), done (vcirc);
        do indices.push_back (*vcirc++); while ( vcirc != done );
        //meshRebuilt.SetTriangle (face_index.idx (), indices[0], indices[1], indices[2]);

        faces_out[i++] = (int)indices[0];
        faces_out[i++] = (int)indices[1];
        faces_out[i++] = (int)indices[2];
        c++;
    }

    n_faces_out = c;

}














// A modifier creating a triangle with the incremental builder.
template <class HDS>
class polyhedron_builder : public CGAL::Modifier_base<HDS> {
public:
    std::vector<double>& coords;
    std::vector<int>& tris;
    polyhedron_builder (std::vector<double>& _coords, std::vector<int>& _tris)
        : coords (_coords), tris (_tris) {}
    void operator()(HDS& hds) {
        typedef typename HDS::Vertex Vertex;
        typedef typename Vertex::Point Point;

        // create a cgal incremental builder
        CGAL::Polyhedron_incremental_builder_3<HDS> B (hds, true);
        B.begin_surface (coords.size () / 3, tris.size () / 3);

        // add the polyhedron vertices
        for ( int i = 0; i < (int)coords.size (); i += 3 ) {
            B.add_vertex (Point (coords[i + 0], coords[i + 1], coords[i + 2]));
        }

        // add the polyhedron triangles
        for ( int i = 0; i < (int)tris.size (); i += 3 ) {
            B.begin_facet ();
            B.add_vertex_to_facet (tris[i + 0]);
            B.add_vertex_to_facet (tris[i + 1]);
            B.add_vertex_to_facet (tris[i + 2]);
            B.end_facet ();
        }

        // finish up the surface
        B.end_surface ();
    }
};




struct Display_polylines {
    const Skeleton& skeleton;
    std::vector<vec3>& out;
    std::vector<int>& outID;
    int id = 0;
    //std::vector<vec3> pline;

    Display_polylines (const Skeleton& skeleton, std::vector<vec3>& out, std::vector<int>& outID)
        : skeleton (skeleton), out (out), outID (outID) {}

    void start_new_polyline () {
        // sstr.str("");
        // sstr.clear();
        id++;
        //pline = std::vector<vec3> ();
    }
    void add_node (Skeleton_vertex v) {
        out.push_back (vec3 (skeleton[v].point.x (), skeleton[v].point.y (), skeleton[v].point.z ()));
        outID.push_back (id);
        // sstr << " " << skeleton[v].point;
    }
    void end_polyline () {
        //out.push_back (pline);
        //  out << polyline_size << sstr.str() << "\n";
    }
};





PINVOKE void MeshSkeleton_Create (


    double* coord_mesh1, size_t n_coord_mesh1,
    int* faces_mesh1, size_t n_faces_mesh1,

    //List of Polylines
    double*& coord_out, int& n_coord_out,
    int*& coord_ID, int& n_coord_ID


) {








    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Create Mesh
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3> mesh1, mesh2, out;
    //mesh1.clear ();
    //mesh2.clear ();
    //mesh1.reserve (n_coord_mesh1, n_coord_mesh1 * 3, n_faces_mesh1);
    //mesh2.reserve (n_coord_mesh2, n_coord_mesh2 * 3, n_faces_mesh2);

    //for ( size_t i = 0; i < n_coord_mesh1; i++ ) {
    //    mesh1.add_vertex (CGAL::Epick::Point_3 (coord_mesh1[3 * i], coord_mesh1[3 * i + 1], coord_mesh1[3 * i + 2]));
    //}


    //for ( size_t i = 0; i < n_faces_mesh1; i++ ) {
    //    mesh1.add_face (CGAL::SM_Vertex_index (faces_mesh1[3 * i + 0]), CGAL::SM_Vertex_index (faces_mesh1[3 * i + 1]), CGAL::SM_Vertex_index (faces_mesh1[3 * i + 2]));
    //}



            ////////////////////////////////////////////////////////////////
            // Get coordinates and face vectors
            ////////////////////////////////////////////////////////////////
            // http://jamesgregson.blogspot.com/2012/05/example-code-for-building.html
    std::vector<double> coords;
    std::vector<int> tris;

    for ( size_t i = 0; i < n_coord_mesh1; i++ ) {
        coords.push_back (coord_mesh1[3 * i + 0]);
        coords.push_back (coord_mesh1[3 * i + 1]);
        coords.push_back (coord_mesh1[3 * i + 2]);
    }

    for ( size_t i = 0; i < n_faces_mesh1; i++ ) {
        tris.push_back (faces_mesh1[3 * i + 0]);
        tris.push_back (faces_mesh1[3 * i + 1]);
        tris.push_back (faces_mesh1[3 * i + 2]);
    }

    ////////////////////////////////////////////////////////////////
    // Build a polyhedron from the loaded arrays
    ////////////////////////////////////////////////////////////////
    CGAL::Polyhedron_3<Kernel> mesh1;
    polyhedron_builder<HalfedgeDS> builder (coords, tris);
    mesh1.delegate (builder);
    //delete meshRhino0;

    ////////////////////////////////////////////////////////////////
    // Perform skeletonization
    ////////////////////////////////////////////////////////////////
    Skeletonization::Skeleton skeleton;
    CGAL::extract_mean_curvature_flow_skeleton (mesh1, skeleton);
    double nv = boost::num_vertices (skeleton);
    double ne = boost::num_edges (skeleton);

    //std::ofstream myfile;
    //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_MeshSkeleton.txt");
    //myfile << "\n Number of vertices: ";
    //myfile << nv;
    //myfile << "\n Number of edges: ";
    //myfile << ne;
    //myfile.close();


   //RhinoApp ().Print (
   //    L"\n Cockroach: Number of vertices of the skeleton: %g \n",
   //    nv);
   //RhinoApp ().Print (
   //    L"\n Cockroach: Number of edges of the skeleton: %g \n",
   //    ne);

   ////////////////////////////////////////////////////////////////
   // Split skeleton graph into 2-valence components for polylines
   ////////////////////////////////////////////////////////////////
   //// Output all the edges of the skeleton.
   // std::ofstream output("skel-poly.cgal");
    std::vector<vec3> output;
    std::vector<int> outputID;
    Display_polylines display (skeleton, output, outputID);
    CGAL::split_graph_into_polylines (skeleton, display);

    //std::ofstream myfile1;
    //myfile1.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_MeshSkeletonSplit.txt");
    //myfile1 << "\n Number of vertices: ";
    //myfile1 << output.size();
    //myfile1 << "\n Number of edges: ";
    //myfile1 << outputID.size();
    //myfile1.close ();

    //for ( auto pline : output ) {
    //    context.m_doc.AddCurveObject (pline);
    //}




    ////////////////////////////////////////////////////////////////
  // Return vertex and id array
  ////////////////////////////////////////////////////////////////


  //Get vertices coordinates
    const int vc = output.size () * 3;
    coord_out = new double[vc];


    int i = 0;
    int c = 0;
    for ( const auto& pt : output ) {

        coord_out[i++] = pt.x;
        coord_out[i++] = pt.y;
        coord_out[i++] = pt.z;
        c++;
    }
    //double*& coord_out, int& coord_ID
    n_coord_out = c;

    /*   std::ofstream myfile2;
       myfile2.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_MeshSkeletonSplitToRhino.txt");
       myfile2 << "\n Number of vertices: ";
       myfile2 << c;*/





       //Get face indices 
    const int fc = outputID.size () * 1;
    coord_ID = new int[fc];


    i = 0;
    c = 0;
    for ( auto id : outputID ) {
        //std::vector<uint32_t> indices;
        //CGAL::Vertex_around_face_circulator<CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>> vcirc (out.halfedge (face_index), out), done (vcirc);
        //do indices.push_back (*vcirc++); while ( vcirc != done );
        ////meshRebuilt.SetTriangle (face_index.idx (), indices[0], indices[1], indices[2]);

        coord_ID[i++] = id;
        c++;
    }

    n_coord_ID = c;

    //myfile2 << "\n Number of vertices: ";
    //myfile2 << c;
    //myfile2.close ();




}













PINVOKE void ComputeNormals (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    
    double radius,
    int iterations,
    int neighbours,
    bool erase,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o) {


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to CGAL PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    std::list<PointVectorPair> points;
    //points.resize(p_c);

    for ( int i = 0; i < p_c; i++ ) {

        points.push_back(PointVectorPair(
            Kernel2::Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]),
            Kernel2::Vector_3 (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2])
        ));
    }






    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


  // Estimates normals direction.
    // Note: pca_estimate_normals() requiresa range of points
    // as well as property maps to access each point's position and normal.
    const int nb_neighbors = neighbours; // K-nearest neighbors = 3 rings
    if ( iterations != 3 ) // Use a fixed neighborhood radius
    {
        // First compute a spacing using the K parameter
        double spacing = CGAL::compute_average_spacing<Concurrency_tag>(points, nb_neighbors, CGAL::parameters::point_map (CGAL::First_of_pair_property_map<PointVectorPair> ()));
        // Then, estimate normals with a fixed radius
        CGAL::pca_estimate_normals<Concurrency_tag> (points,0, // when using a neighborhood radius, K=0 means no limit on the number of neighbors returns
                CGAL::parameters::point_map (CGAL::First_of_pair_property_map<PointVectorPair> ()).
                normal_map (CGAL::Second_of_pair_property_map<PointVectorPair> ()).
                neighbor_radius (2. * spacing)); // use 2*spacing as neighborhood radius
    } else // Use a fixed number of neighbors
    {
        CGAL::pca_estimate_normals<Concurrency_tag>
            (points, nb_neighbors,
                CGAL::parameters::point_map (CGAL::First_of_pair_property_map<PointVectorPair> ()).
                normal_map (CGAL::Second_of_pair_property_map<PointVectorPair> ()));
    }
    // Orients normals.
    // Note: mst_orient_normals() requires a range of points
    // as well as property maps to access each point's position and normal.
    std::list<PointVectorPair>::iterator unoriented_points_begin =
        CGAL::mst_orient_normals (points, nb_neighbors,
            CGAL::parameters::point_map (CGAL::First_of_pair_property_map<PointVectorPair> ()).
            normal_map (CGAL::Second_of_pair_property_map<PointVectorPair> ()));
    // Optional: delete points with an unoriented normal
    // if you plan to call a reconstruction algorithm that expects oriented normals.

    if(erase)
        points.erase (unoriented_points_begin, points.end ());


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    p_c_o = points.size () * 3;
    p_o = new double[p_c_o];

    n_c_o = points.size () * 3;
    n_o = new double[n_c_o];

    c_c_o = points.size () * 3;
    c_o = new double[c_c_o];

    int a = 0;
    int b = 0;
    int cc = 0;
    for(auto& e : points){

        p_o[a++] = e.first.x ();
        p_o[a++] = e.first.y ();
        p_o[a++] = e.first.z ();

        n_o[b++] = e.second.x ();
        n_o[b++] = e.second.y ();
        n_o[b++] = e.second.z ();

        c_o[cc++] = 0;
        c_o[cc++] = 0;
        c_o[cc++] = 0;
    }

}



























typedef CGAL::Exact_predicates_inexact_constructions_kernel  K;
typedef CGAL::Surface_mesh<K::Point_3>                       Mesh;

struct Visitor :
    public CGAL::Polygon_mesh_processing::Corefinement::Default_visitor<Mesh> {
    typedef Mesh::Face_index face_descriptor;

    boost::container::flat_map<const Mesh*, Mesh::Property_map<Mesh::Face_index, int> > properties;
    int face_id;

    Visitor () {
        properties.reserve (3);
        face_id = -1;
    }

    // visitor API overloaded
    void before_subface_creations (face_descriptor f_split, Mesh& tm) {
        face_id = properties[&tm][f_split];
    }

    void after_subface_created (face_descriptor f_new, Mesh& tm) {
        properties[&tm][f_new] = face_id;
    }

    void after_face_copy (face_descriptor f_src, Mesh& tm_src,
        face_descriptor f_tgt, Mesh& tm_tgt) {
        properties[&tm_tgt][f_tgt] = properties[&tm_src][f_src];
    }
};




//To track colors
//https://github.com/CGAL/cgal/blob/master/Polygon_mesh_processing/examples/Polygon_mesh_processing/corefinement_mesh_union_with_attributes.cpp
PINVOKE void MeshBoolean_CreateArrayTrackColors (


    double* coord_mesh, int* n_coord_meshArray,//Flat array of coordinates / 0 256 512 flat array of vertices array / number of meshes
    int* faces_mesh, int* n_faces_meshArray,
    size_t n_mesh,

    size_t Difference_Union_Intersection,

    double*& coord_out, int& n_coord_out,
    int*& faces_out, int& n_faces_out,
    int*& facesColors_out, int& n_facesColors_out,
    int& nValidMeshes

) {

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 //Create Mesh
 ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    std::vector< Mesh > meshList;
    std::vector<Mesh::Property_map<Mesh::Face_index, int>> meshList_id;
    







    int numOfValidMeshes = 0;
    //int numOfMeshes = 0;
    //std::ofstream myfile;
    //std::ofstream myfile00;
   // myfile00.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\x64\\Release\\TrackFaceIDStart00.txt");


    for ( size_t i = 0; i < n_mesh; i++ ) {


        CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>  tempMesh;
        // tempMesh.reserve(n_coord_meshArray[i+1]-n_coord_meshArray[i], (n_coord_meshArray[i+1]-n_coord_meshArray[i]) * 3, n_faces_meshArray[i+1]-n_faces_meshArray[i]);

        for ( int j = n_coord_meshArray[i]; j < n_coord_meshArray[i + 1]; j++ ) {
            tempMesh.add_vertex (CGAL::Epick::Point_3 (coord_mesh[3 * j + 0], coord_mesh[3 * j + 1], coord_mesh[3 * j + 2]));
        }

        for ( int j = n_faces_meshArray[i]; j < n_faces_meshArray[i + 1]; j++ ) {
            tempMesh.add_face (CGAL::SM_Vertex_index (faces_mesh[3 * j + 0]), CGAL::SM_Vertex_index (faces_mesh[3 * j + 1]), CGAL::SM_Vertex_index (faces_mesh[3 * j + 2]));
        }



        if ( tempMesh.is_valid (false) ) {

            /////////////////////////////////////////////////////////////////
            //Add Mesh
            /////////////////////////////////////////////////////////////////
             meshList.push_back (tempMesh);
            numOfValidMeshes++;
        }
        //myfile << "\n ";
        //numOfMeshes++;



    }




     //////////////////////////////////////////////////////////////////
     //// Perform CGAL Boolean
     //////////////////////////////////////////////////////////////////


    CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>& lastMesh = meshList[0];
    Mesh::Property_map<Mesh::Face_index, int>  lastMesh_id = lastMesh.add_property_map<Mesh::Face_index, int> ("f:id", -1).first;
    for ( Mesh::Face_index f : faces (lastMesh) )
        lastMesh_id[f] = 1;


    for ( int i = 1; i < meshList.size (); i++ ) {//meshList.size ()

        Visitor visitor;

        //Properties last mesh
        //Mesh::Property_map<Mesh::Face_index, int>  lastMesh_id = lastMesh.add_property_map<Mesh::Face_index, int> ("f:id", -1).first;
        //for ( Mesh::Face_index f : faces (lastMesh) )
        //    lastMesh_id[f] = 1;
        visitor.properties[&lastMesh] = lastMesh_id;//From previous iteration must or must not contain property map?

        ////Properties current mesh
        Mesh::Property_map<Mesh::Face_index, int>  mesh_id = meshList[i].add_property_map<Mesh::Face_index, int> ("f:id", -1).first;
        for ( Mesh::Face_index f : faces (meshList[i]) )
            mesh_id[f] = (i + 1);
        visitor.properties[&meshList[i]] = mesh_id;

        ////Properties Out
        Mesh   out;
        Mesh::Property_map<Mesh::Face_index, int>  out_id = out.add_property_map<Mesh::Face_index, int> ("f:id", -1).first;
        visitor.properties[&out] = out_id;

        bool valid_union = false;
        const bool throw_on_self_intersection = true;

        try {

            if ( Difference_Union_Intersection == 1 ) {
                valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_union (lastMesh, meshList[i], out, CGAL::Polygon_mesh_processing::parameters::visitor (visitor), CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));//, , CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true) , CGAL::Polygon_mesh_processing::parameters::visitor (visitor)
            } else if ( Difference_Union_Intersection == 2 ) {
                valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_intersection (lastMesh, meshList[i], out, CGAL::Polygon_mesh_processing::parameters::visitor (visitor), CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true)); //, CGAL::Polygon_mesh_processing::parameters::visitor (visitor)
            } else {
                valid_union = CGAL::Polygon_mesh_processing::corefine_and_compute_difference (lastMesh, meshList[i], out, CGAL::Polygon_mesh_processing::parameters::visitor (visitor), CGAL::Polygon_mesh_processing::parameters::throw_on_self_intersection (true));//, CGAL::Polygon_mesh_processing::parameters::visitor (visitor)
            }

            //std::ofstream myfile3;
            //myfile3.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\x64\\Release\\TrackFaceIDEnd.txt");
            //for ( Mesh::Face_index f : faces (out) )
            //    myfile3 << f << " ID" << out_id[f] << " \n";
            //myfile3.close ();

            lastMesh = valid_union ? out : meshList[i];
            
            //for ( Mesh::Face_index f : faces (out) )
            //    lastMesh_id[f] = out_id[f];

            lastMesh_id = lastMesh.add_property_map<Mesh::Face_index, int> ("f:id", -1).first;
            for ( Mesh::Face_index f : faces (out) ){
                auto faceID = out_id[f];
                lastMesh_id[f] = faceID;
            }
         


        } catch ( const std::exception& e ) {
            lastMesh = meshList[i];
        }

    }






    ////////////////////////////////////////////////////////////////
    // Return vertex and face array
    ////////////////////////////////////////////////////////////////


    //Get vertices coordinates
    const int vc = lastMesh.number_of_vertices () * 3;
    coord_out = new double[vc];


    int i = 0;
    int c = 0;
    for ( const auto& vi : lastMesh.vertices () ) {
        const auto& pt = lastMesh.point (vi);
        coord_out[i++] = (double)pt.x ();
        coord_out[i++] = (double)pt.y ();
        coord_out[i++] = (double)pt.z ();
        c++;
    }
    n_coord_out = c;




    //Get face indices 
    const int fc = lastMesh.number_of_faces () * 3;
    faces_out = new int[fc];
 


    i = 0;
    c = 0;
    for ( auto face_index : lastMesh.faces () ) {
        std::vector<uint32_t> indices;
        CGAL::Vertex_around_face_circulator<CGAL::Surface_mesh<CGAL::Exact_predicates_inexact_constructions_kernel::Point_3>> vcirc (lastMesh.halfedge (face_index), lastMesh), done (vcirc);
        do indices.push_back (*vcirc++); while ( vcirc != done );
        //meshRebuilt.SetTriangle (face_index.idx (), indices[0], indices[1], indices[2]);

        faces_out[i++] = (int)indices[0];
        faces_out[i++] = (int)indices[1];
        faces_out[i++] = (int)indices[2];
       // facesColors_out[c] = (int)lastMesh_id[face_index];
        c++;
    }

    n_faces_out = c;


    //Get face color indices 
    const int fcID = lastMesh.number_of_faces ();
    facesColors_out= new int[fcID];


    //std::ofstream myfile3;
    //myfile3.open ("C:\\libs\\PInvokeCPPCSHARP\\PInvoke\\x64\\Release\\TrackFaceIDEnd.txt");
   //myfile3 << fcID;

    int IDCounter = 0;
    for ( Mesh::Face_index f : faces (lastMesh) ) {
        int id = lastMesh_id[f];
        //myfile3 << "\n" << f << " ID" << lastMesh_id[f] << " " << IDCounter ;
        facesColors_out[IDCounter] = id;
        IDCounter++;
    }


  n_facesColors_out = IDCounter;
  //myfile3.close ();

}
