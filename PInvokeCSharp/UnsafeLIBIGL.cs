using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCSharp {
    internal static class UnsafeLIBIGL {

        private const string dllNameLIBIGL = "PInvokeLIBIGL.dll";


        [DllImport(dllNameLIBIGL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDouble(IntPtr arr, bool isArray); // release input coordinates


        [DllImport(dllNameLIBIGL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseInt(IntPtr arr, bool isArray); //release output indices array




        [DllImport(dllNameLIBIGL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int LIBIGL_MeshBoolean_Create(

           [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh1, ulong n_coord_mesh1,
           [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh1, ulong n_faces_mesh1,

           [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh2, ulong n_coord_mesh2,
           [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh2, ulong n_faces_mesh2,

           ulong Difference_Union_Intersection,

           ref IntPtr coord_out, ref int n_coord_out,
           ref IntPtr faces_out, ref int n_faces_out

           );


        [DllImport(dllNameLIBIGL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int LIBIGL_MeshBoolean_CreateArrayNoColors(

            [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_coordMeshArray,
            [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_faces_meshArray,
            ulong n_mesh,

            ulong Difference_Union_Intersection,

            ref IntPtr coord_out, ref int n_coord_out,
            ref IntPtr faces_out, ref int n_faces_out

        );



        [DllImport(dllNameLIBIGL, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int LIBIGL_MeshBoolean_CreateArray(
        [MarshalAs(UnmanagedType.LPArray)] double[] coord_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_coordMeshArray,
        [MarshalAs(UnmanagedType.LPArray)] int[] faces_mesh, [MarshalAs(UnmanagedType.LPArray)] int[] n_faces_meshArray,
        ulong n_mesh,

        ulong Difference_Union_Intersection,

        ref IntPtr coord_out, ref int n_coord_out,
        ref IntPtr faces_out, ref int n_faces_out,
        ref IntPtr facesColors_out, ref int n_facesColors_out,
        ref int nValidMeshes
        );




    }
}
