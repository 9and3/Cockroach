using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCSharp {
    internal static class UnsafeCilantro {

        private const string dllNameCilantro = "PInvokeCilantro.dll";


        [DllImport(dllNameCilantro, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseDouble(IntPtr arr, bool isArray); // release input coordinates


        [DllImport(dllNameCilantro, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ReleaseInt(IntPtr arr, bool isArray); //release output indices array




        [DllImport(dllNameCilantro, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ClusterConnectedComponentRadius(

          [MarshalAs(UnmanagedType.LPArray)] double[] p, ulong p_c,
          [MarshalAs(UnmanagedType.LPArray)] double[] n, ulong n_n,
          [MarshalAs(UnmanagedType.LPArray)] double[] c, ulong c_c,


            double voxelSizeSearch_,//0.1
            double normalThresholdDegree_,//0.2
            int minClusterSize_,//100
            bool colorPointCloud_,//true

          ref IntPtr p_o, ref int p_c_o,
          ref IntPtr n_o, ref int n_n_o,
          ref IntPtr c_o, ref int c_c_o,
          ref IntPtr clusterID_o, ref int clusterID_c_o
        );



    }

}
