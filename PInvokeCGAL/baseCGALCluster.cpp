#include "baseCGALCluster.h"


//Number of clusters - number of pointcloud

PINVOKE void Cluster (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double radius,
    int iterations,
    int neighbours,

    //double*& p_o, int& p_c_o,
    int*& n_o, int& n_c_o,// track id
    double*& c_o, int& c_c_o,
    int& numberOfClusters
    ) {


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to CGAL PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   // std::list<PointVectorPair> points;
    Point_set points;
    //points.reserve(p_c);
    //points.resize(p_c);


    for ( int i = 0; i < p_c; i++ ) {

        points.insert (
            Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2])
            //Kernel2::Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]),
            //Kernel2::Vector_3 (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2])
        );
    }






    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Read input file



    // Add a cluster map
    Point_set::Property_map<int> cluster_map = points.add_property_map<int> ("cluster", -1).first;

    // Compute average spacing
    int neighbours_ = neighbours == 0 ? 12 : neighbours;
    double spacing = CGAL::compute_average_spacing<CGAL::Parallel_if_available_tag> (points, neighbours_);
    if(radius>0)
        spacing = radius;


//    std::ofstream myfile1;
//myfile1.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_Cluster0.txt");
//myfile1 << "Spacing = " << spacing;
//myfile1.close ();
    //std::cerr << "Spacing = " << spacing << std::endl;

    // Adjacencies stored in vector
    std::vector<std::pair<std::size_t, std::size_t> > adjacencies;

    // Compute clusters
    CGAL::Real_timer t;
    t.start ();
    std::size_t nb_clusters
        = CGAL::cluster_point_set (points, cluster_map,
            points.parameters ().neighbor_radius (spacing).
            adjacencies (std::back_inserter (adjacencies)));
    t.stop ();

    numberOfClusters = nb_clusters;
    

    //std::cerr << "Found " << nb_clusters << " clusters with " << adjacencies.size ()
       // << " adjacencies in " << t.time () << " seconds" << std::endl;
    //std::ofstream myfile2;
    //myfile2.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_Cluster2.txt");
    //myfile2 << "Found " << nb_clusters << " clusters with " << adjacencies.size () << " adjacencies in " << t.time () << " seconds" ;
    //myfile2.close ();

    // Output a colored PLY file
   // std::ofstream myfile3;
    //myfile3.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\CGAL_Cluster3.txt");
    Point_set::Property_map<unsigned char> red = points.add_property_map<unsigned char> ("red", 0).first;
    Point_set::Property_map<unsigned char> green = points.add_property_map<unsigned char> ("green", 0).first;
    Point_set::Property_map<unsigned char> blue = points.add_property_map<unsigned char> ("blue", 0).first;
    //for ( Point_set::Index idx : points ) {
        // One color per cluster
       // CGAL::Random rand (cluster_map[idx]);
       // red[idx] = rand.get_int (64, 192);
        //green[idx] = rand.get_int (64, 192);
       // blue[idx] = rand.get_int (64, 192);
       // myfile3 << cluster_map[idx] << "\n";
        //myfile3 << green[idx];
        //myfile3 << blue[idx];
    //}
    //myfile3.close ();

    //std::ofstream ofile ("out.ply", std::ios_base::binary);
    //CGAL::set_binary_mode (ofile);
    //ofile << points;



    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //p_c_o = points.size () * 3;
    //p_o = new double[p_c_o];

    n_c_o = points.size () ;
    n_o = new int[n_c_o];

    c_c_o = points.size () * 3;
    c_o = new double[c_c_o];

    int a = 0;
    int b = 0;
    int cc = 0;

    //    Point_set::iterator it = points.points.begin();
    //while (it != data.point_set.end())
    //  {
    //    cout << (int)color[*it][0] <<  " " << (int)color[*it][1] << " " << (int)color[*it][2] << endl;
    //    it++;
    //  }

    //int col = 0;
    //for ( auto& e : points.points () ) {

    //    p_o[a++] = e.x ();
    //    p_o[a++] = e.y ();
    //    p_o[a++] = e.z ();

    //    //c_o[cc++] = red[col];
    //    //c_o[cc++] = green[col];
    //    //c_o[cc++] = blue[col];
    //    //col++;
    //}

    for ( Point_set::Index idx : points ) {

        CGAL::Random rand (cluster_map[idx]);



        n_o[b++] = cluster_map[idx];//e.second.x ();
        //n_o[b++] = cluster_map[idx];//e.second.y ();
        //n_o[b++] = cluster_map[idx];//e.second.z ();



        c_o[cc++] = rand.get_int (64, 192);
        c_o[cc++] = rand.get_int (64, 192);
        c_o[cc++] = rand.get_int (64, 192);
        //col++;
    }

}






