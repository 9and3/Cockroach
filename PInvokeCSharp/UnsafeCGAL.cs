using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCSharp {
    internal static class UnsafeCGAL {

        private const string dllNameCGAL = "PInvokeCGAL.dll";


        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDouble(IntPtr arr, bool isArray); // release input coordinates


        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseInt(IntPtr arr, bool isArray); //release output indices array




        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MeshBoolean_Create(
            [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh1, ulong n_coord_mesh1,
            [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh1, ulong n_faces_mesh1,

            [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh2, ulong n_coord_mesh2,
            [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh2, ulong n_faces_mesh2,

            ulong Difference_Union_Intersection,

            ref IntPtr coord_out, ref int n_coord_out,
            ref IntPtr faces_out, ref int n_faces_out
        );


        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MeshBoolean_CreateArray(
        [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_coordMeshArray,
        [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_faces_meshArray,
        ulong n_mesh,

        ulong Difference_Union_Intersection,

        ref IntPtr coord_out, ref int n_coord_out,
        ref IntPtr faces_out, ref int n_faces_out,
        ref int nValidMeshes
        );


        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MeshBoolean_CreateArrayTrackColors(
        [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_coordMeshArray,
        [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_faces_meshArray,
        ulong n_mesh,

        ulong Difference_Union_Intersection,

        ref IntPtr coord_out, ref int n_coord_out,
        ref IntPtr faces_out, ref int n_faces_out,
        ref IntPtr facesColors_out, ref int n_facesColors_out,
        ref int nValidMeshes
        );




        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MeshSkeleton_Create(
        [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh1, ulong n_coord_mesh1,
        [MarshalAs(UnmanagedType.LPArray)] int[] size_t, ulong n_faces_mesh1,
        ref IntPtr coord_out, ref int n_coord_out,
        ref IntPtr coord_ID, ref int n_coord_ID

        );


        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ComputeNormals(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

            double radius,// = 0.1,
            int iterations,// = 30,
            int neighbours,// = 100, //reorientation
            bool erase,

          ref IntPtr p_o, ref int p_c_o,
          ref IntPtr n_o, ref int n_n_o,
          ref IntPtr c_o, ref int c_c_o
        );


        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ComputePoissonSurfaceReconstruction(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

            double radius,// = 0.1,
            int iterations,// = 30,
            int neighbours,// = 100, //reorientation

          ref IntPtr p_o, ref int p_c_o,
          ref IntPtr n_o, ref int n_n_o,
          ref IntPtr c_o, ref int c_c_o
        );



        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Cluster(
             [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

            double radius,// = 0.1,
            int iterations,// = 30,
            int neighbours,// = 100, //reorientation

          //ref IntPtr p_o, ref int p_c_o,
          ref IntPtr n_o, ref int n_n_o,
          ref IntPtr c_o, ref int c_c_o,
          ref int numberOfClusters
        );






        [DllImport(dllNameCGAL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SOR(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

            double radius,// = 0.1,
            double percent,// = 30,
            int neighbours,// = 100, //reorientation
               int type,

          ref IntPtr p_o, ref int p_c_o,
          ref IntPtr n_o, ref int n_n_o,
          ref IntPtr c_o, ref int c_c_o

            );




    }
}
