using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PInvokeCSharp {
    internal static class UnsafeOpen3D {

        private const string dllNameOpen3D = "PInvokeOpen3D.dll";
        //private const string dllNameCilantro = "PInvokeCilantro.dll";



        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Test_GetSquare(int n);



        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Open3DDownsample(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

          double numberOfPoints,

          ref IntPtr p_o, ref int p_c_o,
          ref IntPtr n_o, ref int n_n_o,
          ref IntPtr c_o, ref int c_c_o
      );


        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Open3DNormals(

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




        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Open3DPoisson(
          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

            ulong PoisonMaxDepth,
            ulong PoisonMinDepth,
            float PoisonScale,
            bool Linear,

            ref IntPtr p_o, ref int p_c_o,
            ref IntPtr n_o, ref int n_c_o,
            ref IntPtr c_o, ref int c_c_o,
            ref IntPtr density, ref int density_c
           );



        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Open3DMeshPopulate(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] int[] f, ulong f_n,

            int type,// = 30,
            int numberOfPoints,// = 100, //reorientation

          ref IntPtr p_o, ref int p_c_o

        );


        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RANSACPlane(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,

           double distanceThreshold_,
           int neighbours_,
           int iterations_,
           bool InliersOrOutliers,
            ref IntPtr p_o, ref int p_c_o,
            ref IntPtr n_o, ref int n_c_o,
            ref IntPtr c_o, ref int c_c_o

        );




            [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Open3DDownsampleRhinoSDK(

          IntPtr c0,
          int numberOfPoints,
          ref IntPtr c1
        );





        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDouble(IntPtr arr, bool isArray); // release input coordinates


        [DllImport(dllNameOpen3D, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseInt(IntPtr arr, bool isArray); //release output indices array


    }
}
