#include "baseCGALPoisson.h"



PINVOKE void ComputePoissonSurfaceReconstruction (

    double* p, size_t p_c,
    double* n, size_t n_c,
    double* c, size_t c_c,

    double radius,// = 0.1,
    int iterations,// = 30,
    int neighbours,// = 100, //reorientation

    double*& p_o, int& p_c_o,
    double*& n_o, int& n_c_o,
    double*& c_o, int& c_c_o
) {


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Convert Input to CGAL PointCloud
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   // std::list<PointVectorPair> points;
    CGAL::Point_set_3<Kernel2::Point_3, Kernel2::Vector_3> points;
    points.resize(p_c);
    //points.resize(p_c);
   // std::ofstream myfile3;
   // myfile3.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\PoissonStep0.txt");

    if ( p_c < 9 )return;

    for ( int i = 0; i < p_c; i++ ) {
        //myfile3 << p[3 * i + 0] << "\n" << "\n";
        //myfile3 << p[3 * i + 1] << "\n";
        //myfile3 << p[3 * i + 2] << "\n";
        //myfile3 << n[3 * i + 0] << "\n" << "\n";
        //myfile3 << n[3 * i +1] << "\n";
        //myfile3 << n[3 * i + 2] << "\n";

        points.insert(
            Kernel2::Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]),
            Kernel2::Vector_3 (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2])
            );
      
        //points.push_back (PointVectorPair (
        //    Kernel2::Point_3 (p[3 * i + 0], p[3 * i + 1], p[3 * i + 2]),
        //    Kernel2::Vector_3 (n[3 * i + 0], n[3 * i + 1], n[3 * i + 2])

        // 

        //));

    }
    //myfile3.close();





    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Run Open3D Method
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////





    try {

        CGAL::Polyhedron_3<Kernel2, CGAL::Polyhedron_items_with_id_3> output_mesh;
        
        double average_spacing = CGAL::compute_average_spacing<CGAL::Sequential_tag> (points, 6);
        if (CGAL::poisson_surface_reconstruction_delaunay (points.begin (), points.end (), points.point_map (), points.normal_map (), output_mesh, average_spacing) )


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Run Open3D Method
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            p_c_o = output_mesh.size_of_vertices () * 3;
            p_o = new double[p_c_o];

            n_c_o = output_mesh.size_of_facets () * 3;
            n_o = new double[n_c_o];

            c_c_o = output_mesh.size_of_vertices () * 3;
            c_o = new double[c_c_o];



            //std::ofstream myfile;
           //myfile.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\PoissonStep1.txt");

            std::size_t counter = 0;
            for ( auto it = output_mesh.vertices_begin (); it != output_mesh.vertices_end (); ++it ) {
                it->id () = counter++;
            }

            std::size_t counter2 = 0;
            for ( auto it = output_mesh.facets_begin (); it != output_mesh.facets_end (); ++it ) {
                it->id () = counter2++;
            }


            int i = 0;
            int a = 0;
            int c = 0;
            for ( auto it = output_mesh.vertices_begin (); it != output_mesh.vertices_end (); ++it ) {

                // it->id () = i++;
                p_o[a++] = it->point ().x ();
                p_o[a++] = it->point ().y ();
                p_o[a++] = it->point ().z ();
                c_o[c++] = 0;
                c_o[c++] = 0;
                c_o[c++] = 0;
                //myfile << it->point ().x () << "\n";
                //myfile << it->point ().y () << "\n";
                //myfile << it->point ().z () << "\n";

                //myfile << "\n ID" << it->id () << "\n";

            }
            // myfile.close ();


            // std::ofstream myfile2;


             //myfile2.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\PoissonStep2.txt");

             //for ( Polyhedron3::Facet_handle fh : output_mesh.face () ) {
             //    Polyhedron3::Halfedge_handle start = fh->halfedge (), h = start;
             //    do {
             //        std::cout << h->vertex ()->id () << "\n";
             //        h = h->next ();
             //    } while ( h != start );
             //}
            int b = 0;
            for ( auto it = output_mesh.facets_begin (); it != output_mesh.facets_end (); ++it ) {


                auto circ = it->facet_begin ();

                //myfile2 << circ->id () << "\n" << "\n";

                do {
                    n_o[b++] = circ->vertex ()->id ();
                    // myfile2 << circ->vertex ()->id () << "\n" << "\n";
                } while ( ++circ != it->facet_begin () );

            }





            //myfile2.close ();



       // }



    } catch ( const std::exception& e ) {

        std::ofstream myfile2;
        myfile2.open ("C:\\libs\\Cockroach\\CockroachPInvoke\\Cockroach_CSHARP_DLL\\bin\\x64\\PoissonDebug.txt");
        myfile2 << e.what ();
        myfile2.close ();
    }



}







