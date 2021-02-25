using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace PInvokeCSharp
{
    public static class TestOpen3D
    {
        public static int GetSquare(int n) {
            return UnsafeOpen3D.Test_GetSquare(n);
        }


        public static PointCloud Downsample(PointCloud cloud, double voxelSize = 5000) {

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Conversion from Rhino PointCloud to Flat Array
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //var watch = System.Diagnostics.Stopwatch.StartNew();


            double[] p = new double[cloud.Count * 3];
            ulong p_c = (ulong)cloud.Count;

            double[] n =new double[cloud.Count * 3] ;
            ulong n_c = p_c;

            double[] c = new double[cloud.Count * 3];
            ulong c_c = p_c;

            bool hasColors = cloud.ContainsColors;

            System.Threading.Tasks.Parallel.For(0, cloud.Count, i =>
            {
            //for (int i = 0; i < cloud.Count; i++) {
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
           
            });
       

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


            UnsafeOpen3D.Open3DDownsample(
                p, p_c,
                n, n_c,
                c, c_c,
                voxelSize,
                ref p_pointer, ref p_pointer_c,
                ref n_pointer, ref n_pointer_c,
                ref c_pointer, ref c_pointer_c
                );

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("C++ Downsample "+ watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert Pointers to double arrays
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert faceIndicesPointer to C# int[]

            //watch = System.Diagnostics.Stopwatch.StartNew();
            double[] P = new double[p_pointer_c ];
            Marshal.Copy(p_pointer, P, 0, P.Length);

            double[] N = new double[n_pointer_c ];
            Marshal.Copy(n_pointer, N, 0, P.Length);

            double[] C = new double[c_pointer_c ];
            Marshal.Copy(c_pointer, C, 0, P.Length);
            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Convert Pointer to double Array "+watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UnsafeOpen3D.ReleaseDouble(p_pointer, true);
            UnsafeOpen3D.ReleaseDouble(n_pointer, true);
            UnsafeOpen3D.ReleaseDouble(c_pointer, true);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert C++ to C# Pointcloud
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();
            PointCloud newCloud = new PointCloud();


            //for (int i = 0; i < P.Length; i += 3) {
            //    newCloud.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

            //    if (C[i + 0] == 0 && C[i + 1] == 0 && C[i + 2] == 0) continue;
            //    newCloud[(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);

            //}


            //for (int i = 0; i < P.Length; i += 3) {
            //    if (N[i + 0] == 0 && N[i + 1] == 0 && N[i + 2] == 0) continue;
            //    newCloud[(int)Math.Round((i / 3.0))].Normal = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
            //}

            Point3d[] location = new Point3d[(int)(P.Length/3)];
            Vector3d[] normals = new Vector3d[(int)(P.Length / 3)];
            Color[] colors = new Color[(int)(P.Length / 3)];


            for (int i = 0; i < P.Length; i += 3) {

                int id = i / 3;
                location[id] = new Point3d(P[i + 0], P[i + 1], P[i + 2]);
                normals[id] = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
                colors[id] = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);

                //clouds[0].Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

                //if (C[i + 0] == 0 && C[i + 1] == 0 && C[i + 2] == 0) continue;
                //clouds[0][(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);

            }
            newCloud.AddRange(location, normals, colors);



            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return newCloud;

        }


   

        public static PointCloud Normals(PointCloud cloud, double radius=0.1, int iterations = 30, int neighbours = 100) {

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

            System.Threading.Tasks.Parallel.For(0, cloud.Count, i => {
                //for (int i = 0; i < cloud.Count; i++) {
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
                //}
                //if (Math.Abs(FastPlaneToPt(denom, eq[0], eq[1], eq[2], eq[3], points[i])) <= tol)
                //    sectionPoints.Add(points[i]);
            });



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


            UnsafeOpen3D.Open3DNormals(
                p, p_c,
                n, n_c,
                c, c_c,
                radius,
                iterations,
                neighbours,
                ref p_pointer, ref p_pointer_c,
                ref n_pointer, ref n_pointer_c,
                ref c_pointer, ref c_pointer_c
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
            Marshal.Copy(n_pointer, N, 0, P.Length);

            double[] C = new double[c_pointer_c];
            Marshal.Copy(c_pointer, C, 0, P.Length);
            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Convert Pointer to double Array "+watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UnsafeOpen3D.ReleaseDouble(p_pointer, true);
            UnsafeOpen3D.ReleaseDouble(n_pointer, true);
            UnsafeOpen3D.ReleaseDouble(c_pointer, true);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert C++ to C# Pointcloud
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();
            PointCloud newCloud = new PointCloud();

            //System.Threading.Tasks.Parallel.For(0, P.Length, i => {

            //    newCloud.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

            //    if (!(C[i + 0] == 0 && C[i + 1] == 0 && C[i + 2] == 0)) {
            //        newCloud[(int)(i / 3)].Color = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);
            //    }

            //});

            //System.Threading.Tasks.Parallel.For(0, P.Length, i => {

            //    if (!(N[i + 0] == 0 && N[i + 1] == 0 && N[i + 2] == 0)) {
            //        newCloud[(int)(i / 3)].Normal = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
            //    }

            //});







            for (int i = 0; i < P.Length; i += 3) {
                newCloud.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

                //if (c[i + 0] == 0 && c[i + 1] == 0 && c[i + 2] == 0) continue;
                newCloud[(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)c[i + 0], (int)c[i + 1], (int)c[i + 2]);

            }


            for (int i = 0; i < P.Length; i += 3) {
                if (N[i + 0] == 0 && N[i + 1] == 0 && N[i + 2] == 0) continue;
                newCloud[(int)Math.Round((i / 3.0))].Normal = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
            }




            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return newCloud;

        }




        public static Tuple<Mesh,double[]> Poisson(PointCloud cloud, ulong PoisonMaxDepth=8,  ulong PoisonMinDepth=0,    float PoisonScale=1.1f,    bool Linear=false) {

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
            IntPtr density_pointer = IntPtr.Zero;
            int density_pointer_c = 0;


            UnsafeOpen3D.Open3DPoisson(
                p, p_c,
                n, n_c,
                c, c_c,
                PoisonMaxDepth,
                PoisonMinDepth,
                PoisonScale,
                Linear,
                ref p_pointer, ref p_pointer_c,
                ref n_pointer, ref n_pointer_c,
                ref c_pointer, ref c_pointer_c,
                ref density_pointer, ref density_pointer_c
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

            double[] C = new double[c_pointer_c];
            Marshal.Copy(c_pointer, C, 0, C.Length);

            double[] D = new double[density_pointer_c];
            Marshal.Copy(density_pointer, D, 0, D.Length);
            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Convert Pointer to double Array "+watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UnsafeOpen3D.ReleaseDouble(p_pointer, true);
            UnsafeOpen3D.ReleaseDouble(n_pointer, true);
            UnsafeOpen3D.ReleaseDouble(c_pointer, true);
            UnsafeOpen3D.ReleaseDouble(density_pointer, true);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert C++ to C# Pointcloud
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();

            Mesh mesh = new Mesh();

            for (int i = 0; i < P.Length; i += 3) {
                mesh.Vertices.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

            }

            for (int i = 0; i < N.Length; i += 3) {
                mesh.Faces.AddFace((int)N[i + 0], (int)N[i + 1], (int)N[i + 2]);
            }

            for (int i = 0; i < C.Length; i += 3) {
                mesh.VertexColors.Add(System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]));

            }


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return Tuple.Create( mesh,D);

        }







        public static PointCloud DownsampleRhino(PointCloud cloud, int numberOfPoints = 5000) {

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Conversion from Rhino PointCloud to Flat Array
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Rhino.Runtime.Interop.NativeGeometryConstPointer;
            //Rhino.Runtime.Interop.NativeGeometryNonConstPointer;

            IntPtr c0 = Rhino.Runtime.Interop.NativeGeometryNonConstPointer(cloud);
            IntPtr c1 = IntPtr.Zero;
            UnsafeOpen3D.Open3DDownsampleRhinoSDK(c0, numberOfPoints, ref c1);

            if (IntPtr.Zero == c1)
                return null;
   
            GeometryBase o = Rhino.Runtime.Interop.CreateFromNativePointer(c1);
            PointCloud newCloud = (PointCloud)o;

            return newCloud;

        }







        public static PointCloud PopulateMesh(Mesh m1_,  int type = 0, int numberOfPoints = 100) {

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Clean Mesh
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            m1_.Vertices.UseDoublePrecisionVertices = true;
            Mesh m1 = m1_.DuplicateMesh();

            m1.Vertices.UseDoublePrecisionVertices = true;
            m1.Faces.ConvertQuadsToTriangles();

            m1.Vertices.CombineIdentical(true, true);
            m1.Vertices.CullUnused();
            m1.Weld(3.14159265358979);
           // m1.FillHoles();
            m1.RebuildNormals();


            if (!m1.IsValid )
                return null;


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Send Vertices and Faces to C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            double[] ptCoordArr1 = new double[m1.Vertices.Count * 3];
            for (int i = 0; i < m1.Vertices.Count; i++) {
                ptCoordArr1[i * 3 + 0] = m1.Vertices.Point3dAt(i).X;
                ptCoordArr1[i * 3 + 1] = m1.Vertices.Point3dAt(i).Y;
                ptCoordArr1[i * 3 + 2] = m1.Vertices.Point3dAt(i).Z;
            }
            var ptCount1 = (ulong)m1.Vertices.Count;


            int[] facesArr1 = m1.Faces.ToIntArray(true);
            var facesCount1 = (ulong)m1.Faces.Count;


            ////Rhino.RhinoApp.WriteLine(ptCoordArr1.Length.ToString() + " " + ptCount1.ToString());
            ////Rhino.RhinoApp.WriteLine(facesArr1.Length.ToString() + " " + facesCount1.ToString());
            ////Rhino.RhinoApp.WriteLine(ptCoordArr2.Length.ToString() + " " + ptCount2.ToString());
            ////Rhino.RhinoApp.WriteLine(facesArr2.Length.ToString() + " " + facesCount2.ToString());



            ////Inputs
            IntPtr p_pointer = IntPtr.Zero;
            int p_pointer_c = 0;
  

            //Call C++ method
            UnsafeOpen3D.Open3DMeshPopulate(ptCoordArr1, ptCount1, facesArr1, facesCount1, type, numberOfPoints,  ref p_pointer, ref p_pointer_c);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert Pointers to double arrays
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert faceIndicesPointer to C# int[]

            //watch = System.Diagnostics.Stopwatch.StartNew();
            double[] P = new double[p_pointer_c];
            Marshal.Copy(p_pointer, P, 0, P.Length);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UnsafeOpen3D.ReleaseDouble(p_pointer, true);



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert C++ to C# Pointcloud
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();
            PointCloud newCloud = new PointCloud();


            for (int i = 0; i < P.Length; i += 3) {
                newCloud.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));
            }



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return newCloud;



        }



        public static PointCloud[] RANSACPlane(PointCloud cloud, double distance = 0, double neighbours = 0, double iterations = 10, bool InliersOrOutliers = true) {

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

            // bool hasColors = cloud.ContainsColors;

            System.Threading.Tasks.Parallel.For(0, cloud.Count, i => {
                //for (int i = 0; i < cloud.Count; i++) {
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

            });


            //watch.Stop();
            // Rhino.RhinoApp.WriteLine("Convert PointCloud to double array "+watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Call C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            IntPtr p_pointer = IntPtr.Zero;
            int p_pointer_c = 0;
            IntPtr n_pointer = IntPtr.Zero;
            int n_pointer_c = 0;
            IntPtr c_pointer = IntPtr.Zero;
            int c_pointer_c = 0;

            UnsafeOpen3D.RANSACPlane(
                p, p_c,
                n,n_c,
                c,c_c,

                distance,
                (int)neighbours,
                (int)iterations,
                InliersOrOutliers,

                ref p_pointer, ref p_pointer_c,
                ref n_pointer, ref n_pointer_c,
                ref c_pointer, ref c_pointer_c
                );

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("C++ Downsample "+ watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert Pointers to double arrays
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert faceIndicesPointer to C# int[]

            //watch = System.Diagnostics.Stopwatch.StartNew();
            //int[] P = new int[p_pointer_c];
            double[] P = new double[p_pointer_c];
            Marshal.Copy(p_pointer, P, 0, P.Length);

            double[] N = new double[n_pointer_c];
            Marshal.Copy(n_pointer, N, 0, P.Length);

            double[] C = new double[c_pointer_c];
            Marshal.Copy(c_pointer, C, 0, P.Length);


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Convert Pointer to double Array "+watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            UnsafeOpen3D.ReleaseDouble(p_pointer, true);
            UnsafeOpen3D.ReleaseDouble(n_pointer, true);
            UnsafeOpen3D.ReleaseDouble(c_pointer, true);



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert C++ to C# Pointcloud
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();
            PointCloud[] clouds = new PointCloud[1];

            int count = P.Length / 3;
            Point3d[] location = new Point3d[count];
            Vector3d[] normals = new Vector3d[count];
            Color[] colors = new Color[count];

            for (int i = 0; i < clouds.Length; i++) {
                clouds[i] = new PointCloud();
            }

            for (int i = 0; i < P.Length; i += 3) {

                int id = i / 3;
                location[id] = new Point3d(P[i + 0], P[i + 1], P[i + 2]);
                normals[id] = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
                colors[id] = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);

                //clouds[0].Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

                //if (C[i + 0] == 0 && C[i + 1] == 0 && C[i + 2] == 0) continue;
                //clouds[0][(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);

            }
            clouds[0].AddRange(location,normals,colors);

            //for (int i = 0; i < P.Length; i += 3) {
            //    if (N[i + 0] == 0 && N[i + 1] == 0 && N[i + 2] == 0) continue;
            //    clouds[0][(int)Math.Round((i / 3.0))].Normal = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
            //}

            // Rhino.RhinoApp.WriteLine(hash.Count.ToString());


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return clouds;

        }







    }
}
