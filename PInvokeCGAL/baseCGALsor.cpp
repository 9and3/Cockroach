#include "baseCGALsor.h"


//neighbours
//radius
//percent
PINVOKE void SOR (
    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double radius,
    double percent,
    int neighbours,
    int type,

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,//user normals to track id
    double*& c_o, int& c_c_o

) {

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to CGAL PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // std::list<PointVectorPair> points;
   
    //Point_set points;
    //points.reserve(p_c);
    //points.resize(p_c);

    std::vector< Kernel::Point_3> points;
    for ( int i = 0; i < p_c; i++ ) {

        points.push_back(Kernel::Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]));

        //points.insert (
        //    Kernel::Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]),
        //    Kernel::Vector_3 (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2])
        //);
    }



    //const char* fname = (argc > 1) ? argv[1] : "data/oni.xyz";
    //// Reads a .xyz point set file in points[].
    //// The Identity_property_map property map can be omitted here as it is the default value.
    //std::vector<Point> points;
    //std::ifstream stream (fname);
    //if ( !stream ||
    //    !CGAL::read_xyz_points (stream, std::back_inserter (points),
    //        CGAL::parameters::point_map (CGAL::Identity_property_map<Point> ())) ) {
    //    std::cerr << "Error: cannot read file " << fname << std::endl;
    //    return EXIT_FAILURE;
    //}


    // Removes outliers using erase-remove idiom.
    // The Identity_property_map property map can be omitted here as it is the default value.
    const int nb_neighbors = neighbours < 3  ? 24 : neighbours; // considers 24 nearest neighbor points

    // Estimate scale of the point set with average spacing
    const double average_spacing = radius == 0 ?  CGAL::compute_average_spacing<CGAL::Sequential_tag>     (points, nb_neighbors) : radius;

    if(type!=0){

      

    // FIRST OPTION //
    // I don't know the ratio of outliers present in the point set
    std::vector<Kernel::Point_3>::iterator first_to_remove  = CGAL::remove_outliers<CGAL::Parallel_if_available_tag> (points, nb_neighbors,
        CGAL::parameters::threshold_percent (100.). // No limit on the number of outliers to remove
            threshold_distance (2. * average_spacing)); // Point with distance above 2*average_spacing are considered outliers
    //std::cerr << (100. * std::distance (first_to_remove, points.end ()) / (double)(points.size ()))
    //    << "% of the points are considered outliers when using a distance threshold of "
    //    << 2. * average_spacing << std::endl;
    points.erase(first_to_remove);
    }else{

    // SECOND OPTION //
    // I know the ratio of outliers present in the point set

    const double removed_percentage = percent == 0 ? 5.0 : percent; // percentage of points to remove
    points.erase (CGAL::remove_outliers<CGAL::Parallel_if_available_tag> (points,  nb_neighbors,
            CGAL::parameters::threshold_percent (removed_percentage). // Minimum percentage to remove
            threshold_distance (0.)), // No distance threshold (can be omitted)
        points.end ());

    }

    // Optional: after erase(), use Scott Meyer's "swap trick" to trim excess capacity
    std::vector<Kernel::Point_3> (points).swap (points);







    //Ouput
    p_c_o = points.size () * 3;
    p_o = new double[p_c_o];

    n_c_o = points.size () * 3;
    n_o = new double[n_c_o];

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
    for ( auto& e : points ) {//.points ()

        p_o[a++] = e.x ();
        p_o[a++] = e.y ();
        p_o[a++] = e.z ();

        //c_o[cc++] = red[col];
        //c_o[cc++] = green[col];
        //c_o[cc++] = blue[col];
        //col++;
    }

    //for ( Point_set::Index idx : points ) {

    //    CGAL::Random rand (cluster_map[idx]);



    //    n_o[b++] = cluster_map[idx];//e.second.x ();
    //    n_o[b++] = cluster_map[idx];//e.second.y ();
    //    n_o[b++] = cluster_map[idx];//e.second.z ();


    //    //c_o[cc++] = cluster_map[idx];
    //    //c_o[cc++] = cluster_map[idx];
    //    //c_o[cc++] = cluster_map[idx];

    //    c_o[cc++] = rand.get_int (64, 192);
    //    c_o[cc++] = rand.get_int (64, 192);
    //    c_o[cc++] = rand.get_int (64, 192);
    //    //col++;
    //}


}