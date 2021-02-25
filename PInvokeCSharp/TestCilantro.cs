using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCSharp {
    public static class TestCilantro {


        public static PointCloud[] GetClusterConnectedComponentRadius(PointCloud cloud, double voxelSizeSearch = 0.1, double normalThresholdDegree = 2.0, int minClusterSize = 100, bool colorPointCloud = true, bool split = false) {

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Conversion from Rhino PointCloud to Flat Array
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //var watch = System.Diagnostics.Stopwatch.StartNew();


            double[] p = new double[cloud.Count * 3];
            ulong p_c = (ulong)cloud.Count;

            double[] n = new double[cloud.Count * 3];
            ulong n_c = p_c;

            double[] c = new double[cloud.Count * 3];
            ulong c_c = p_c;

            bool hasColors = cloud.ContainsColors;

            //System.Threading.Tasks.Parallel.For(0, cloud.Count, i => {
            for (int i = 0; i < cloud.Count; i++) {
                p[i * 3 + 0] = cloud[i].Location.X;
                p[i * 3 + 1] = cloud[i].Location.Y;
                p[i * 3 + 2] = cloud[i].Location.Z;
                if (cloud.ContainsNormals) {
                    n[i * 3 + 0] = cloud[i].Normal.X;
                    n[i * 3 + 1] = cloud[i].Normal.Y;
                    n[i * 3 + 2] = cloud[i].Normal.Z;
                }
                if (cloud.ContainsColors) {
                    c[i * 3 + 0] = cloud[i].Color.R;
                    c[i * 3 + 1] = cloud[i].Color.G;
                    c[i * 3 + 2] = cloud[i].Color.B;
                }
            }

            //Rhino.RhinoApp.WriteLine(p.Length.ToString());

            //if (Math.Abs(FastPlaneToPt(denom, eq[0], eq[1], eq[2], eq[3], points[i])) <= tol)
            //    sectionPoints.Add(points[i]);
            //});



            //watch.Stop();
            // Rhino.RhinoApp.WriteLine("Convert PointCloud to double array "+watch.ElapsedMilliseconds.ToString());

     
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Call C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();
            IntPtr p_pointer = IntPtr.Zero;
            int p_pointer_c = 0;
            IntPtr n_pointer = IntPtr.Zero;
            int n_pointer_c = 0;
            IntPtr c_pointer = IntPtr.Zero;
            int c_pointer_c = 0;
            IntPtr clusterID_pointer = IntPtr.Zero;
            int clusterID_pointer_c = 0;


            UnsafeCilantro.ClusterConnectedComponentRadius(
                p, p_c,
                n, n_c,
                c, c_c,
                voxelSizeSearch,
                normalThresholdDegree,
                minClusterSize,
                colorPointCloud,
                ref p_pointer, ref p_pointer_c,
                ref n_pointer, ref n_pointer_c,
                ref c_pointer, ref c_pointer_c,
                ref clusterID_pointer, ref clusterID_pointer_c
                );

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("C++ Downsample "+ watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert Pointers to double arrays
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         

            //Convert faceIndicesPointer to C# int[]

            //watch = System.Diagnostics.Stopwatch.StartNew();
            double[] P = new double[p_pointer_c];
            Marshal.Copy(p_pointer, P, 0, P.Length);

            double[] N = new double[n_pointer_c];
            Marshal.Copy(n_pointer, N, 0, N.Length);
         
            int[] C = new int[c_pointer_c];
            Marshal.Copy(c_pointer, C, 0, C.Length);

            int[] ClusterID = new int[clusterID_pointer_c];
            Marshal.Copy(clusterID_pointer, ClusterID, 0, ClusterID.Length);


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Convert Pointer to double Array "+watch.ElapsedMilliseconds.ToString());
 
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UnsafeOpen3D.ReleaseDouble(p_pointer, true);
            UnsafeOpen3D.ReleaseDouble(n_pointer, true);
            UnsafeOpen3D.ReleaseInt(c_pointer, true);
            UnsafeOpen3D.ReleaseInt(clusterID_pointer, true);


            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // //Convert C++ to C# Pointcloud
            // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            PointCloud[] clouds = new PointCloud[0];

            if(!split){


                int count = P.Length / 3;
                Point3d[] location = new Point3d[count];
                Vector3d[] normals = new Vector3d[count];
                Color[] colors = new Color[count];


                for (int i = 0; i < P.Length; i += 3) {
                    //Rhino.RhinoApp.WriteLine((C[i + 0]).ToString() + " " + (C[i + 1]).ToString() + " " + (C[i + 2]).ToString());
                    int id = i / 3;
                    location[id] = new Point3d(P[i + 0], P[i + 1], P[i + 2]);
                    normals[id] = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
                    colors[id] = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);
                }

                PointCloud newCloud = new PointCloud();
                newCloud.AddRange(location, normals, colors);
                clouds = new PointCloud[] { newCloud };
            } else {




                Point3d[][] location = new Point3d[ClusterID.Length][];
                Vector3d[][] normals = new Vector3d[ClusterID.Length][];
                Color[][] colors = new Color[ClusterID.Length][];

                int[] flatClusterID = new int[(int)(P.Length / 3)];
                int[] flatGroupClusterID = new int[(int)(P.Length / 3)];
                int flatCounter = 0;

                for (int i = 0; i < ClusterID.Length; i++) {
                    location[i] = new Point3d[ClusterID[i]];
                    normals[i] = new Vector3d[ClusterID[i]];
                    colors[i] = new System.Drawing.Color[ClusterID[i]];

                    for (int j = 0; j < ClusterID[i]; j++) {
                        flatClusterID[flatCounter] = i;
                        flatGroupClusterID[flatCounter] = j;
                        flatCounter++;
                    }
                }




                for (int i = 0; i < P.Length; i += 3) {
                    //Rhino.RhinoApp.WriteLine((C[i + 0]).ToString() + " " + (C[i + 1]).ToString() + " " + (C[i + 2]).ToString());
                    int id = i / 3;
                    location[flatClusterID[id]][flatGroupClusterID[id]] = new Point3d(P[i + 0], P[i + 1], P[i + 2]);
                    normals[flatClusterID[id]][flatGroupClusterID[id]] = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
                    colors[flatClusterID[id]][flatGroupClusterID[id]] = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);

                }

                clouds = new PointCloud[ClusterID.Length];
                for (int i = 0; i < ClusterID.Length; i++) {
                    clouds[i] = new PointCloud();
                    clouds[i].AddRange(location[i], normals[i], colors[i]);
                }


            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return clouds;

        }










    }
}
