using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCSharp {
    public static class TestCGAL {


        public static Mesh CreateMeshBoolean(Mesh m1_, List<Mesh> list_m2, int Difference_Union_Intersection = 0) {

            Mesh m1 = m1_;
            //if (LIBIGL_CGAL) {

            //    for (int i = 0; i < list_m2.Count; i++) {
            //        var result = CreateLIBIGLMeshBoolean(m1, list_m2[i], Difference_Union_Intersection);
            //        if (result != null) {
            //            m1 = result;
            //        }
            //    }
            //} else {
                for (int i = 0; i < list_m2.Count; i++) {
                    var result = CreateMeshBoolean(m1, list_m2[i], Difference_Union_Intersection);
                    if (result != null) {
                        m1 = result;
                    }
                }

            //}


            return m1;
        }

        public static Mesh CreateMeshBoolean(Mesh m1_, Mesh m2_, int Difference_Union_Intersection = 0) {

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Clean Mesh
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            m1_.Vertices.UseDoublePrecisionVertices = true;
            m2_.Vertices.UseDoublePrecisionVertices = true;
            Mesh m1 = m1_.DuplicateMesh();
            Mesh m2 = m2_.DuplicateMesh();
            m1.Vertices.UseDoublePrecisionVertices = true;
            m2.Vertices.UseDoublePrecisionVertices = true;
            m1.Faces.ConvertQuadsToTriangles();
            m2.Faces.ConvertQuadsToTriangles();

            m1.Vertices.CombineIdentical(true, true);
            m1.Vertices.CullUnused();
            m1.Weld(3.14159265358979);
            m1.FillHoles();
            m1.RebuildNormals();

            m2.Vertices.CombineIdentical(true, true);
            m2.Vertices.CullUnused();
            m2.Weld(3.14159265358979);
            m2.FillHoles();
            m2.RebuildNormals();


            if (!m1.IsValid || !m1.IsClosed || !m2.IsValid || !m2.IsClosed)
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


            double[] ptCoordArr2 = new double[m2.Vertices.Count * 3];
            for (int i = 0; i < m2.Vertices.Count; i++) {
                ptCoordArr2[i * 3 + 0] = m2.Vertices.Point3dAt(i).X;
                ptCoordArr2[i * 3 + 1] = m2.Vertices.Point3dAt(i).Y;
                ptCoordArr2[i * 3 + 2] = m2.Vertices.Point3dAt(i).Z;
            }
            var ptCount2 = (ulong)m2.Vertices.Count;

            int[] facesArr2 = m2.Faces.ToIntArray(true);
            var facesCount2 = (ulong)m2.Faces.Count;

            ////Rhino.RhinoApp.WriteLine(ptCoordArr1.Length.ToString() + " " + ptCount1.ToString());
            ////Rhino.RhinoApp.WriteLine(facesArr1.Length.ToString() + " " + facesCount1.ToString());
            ////Rhino.RhinoApp.WriteLine(ptCoordArr2.Length.ToString() + " " + ptCount2.ToString());
            ////Rhino.RhinoApp.WriteLine(facesArr2.Length.ToString() + " " + facesCount2.ToString());



            ////Inputs
            IntPtr vertexCoordPointer = IntPtr.Zero;
            int nVertices = 0;
            IntPtr faceIndicesPointer = IntPtr.Zero;
            int nFaces = 0;

            //Call C++ method
            UnsafeCGAL.MeshBoolean_Create(ptCoordArr1, ptCount1, facesArr1, facesCount1, ptCoordArr2, ptCount2, facesArr2, facesCount2, (ulong)Math.Max(0, Math.Min(Difference_Union_Intersection, 2)), ref vertexCoordPointer, ref nVertices, ref faceIndicesPointer, ref nFaces);




            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Get Vertices and Faces from C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Convert faceIndicesPointer to C# int[]
            double[] verticesCoordinates = new double[nVertices * 3];
            Marshal.Copy(vertexCoordPointer, verticesCoordinates, 0, verticesCoordinates.Length);

            int[] faceIndices = new int[nFaces * 3];
            Marshal.Copy(faceIndicesPointer, faceIndices, 0, faceIndices.Length);

            ////Rhino.RhinoApp.WriteLine(verticesCoordinates.Length.ToString()  + " " + nVertices.ToString());
            ////Rhino.RhinoApp.WriteLine(faceIndices.Length.ToString() + " " + nFaces.ToString());



            //Create mesh
            Mesh mesh = new Mesh();
            for (int i = 0; i < verticesCoordinates.Length; i += 3)
                mesh.Vertices.Add(new Point3d(verticesCoordinates[i + 0], verticesCoordinates[i + 1], verticesCoordinates[i + 2]));

            for (int i = 0; i < faceIndices.Length; i += 3) {
                mesh.Faces.AddFace(faceIndices[i + 0], faceIndices[i + 1], faceIndices[i + 2]);
            }

            //mesh.Vertices.Align(0.01);
            mesh.Vertices.CombineIdentical(true, true);
            mesh.Vertices.CullUnused();
            mesh.Weld(3.14159265358979);
            mesh.FillHoles();
            mesh.RebuildNormals();


            //Release memory from output
            UnsafeCGAL.ReleaseDouble(vertexCoordPointer, true);
            UnsafeCGAL.ReleaseInt(faceIndicesPointer, true);


            return mesh;


        }





        public static Mesh CreateMeshBooleanArray(Mesh[] m_, int Difference_Union_Intersection = 0) {

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Clean Mesh
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var meshes = new List<Mesh>(m_.Length);

            for (int i = 0; i < m_.Length; i++) {

                if (!m_[i].IsClosed)
                    continue;

                m_[i].Vertices.UseDoublePrecisionVertices = true;
                Mesh m = m_[i].DuplicateMesh();
                m.Vertices.UseDoublePrecisionVertices = true;
                m.Faces.ConvertQuadsToTriangles();
                m.Vertices.CombineIdentical(true, true);
                m.Vertices.CullUnused();
                m.Weld(3.14159265358979);
                m.FillHoles();
                m.RebuildNormals();
                m.Vertices.UseDoublePrecisionVertices = true;

                if (m.IsValid && m.IsClosed)
                    meshes.Add(m);
            }

            if (meshes.Count < 2)
                return null;




            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Send Vertices and Faces to C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////




            int numberOfVertices = 0;
            int numberOfFaces = 0;
            foreach (Mesh tempMesh in meshes) {
                numberOfVertices += tempMesh.Vertices.Count;
                numberOfFaces += tempMesh.Faces.Count;
            }

            double[] coord = new double[numberOfVertices * 3];
            int[] vertexArrayCount = new int[meshes.Count + 1]; vertexArrayCount[0] = 0;

            int[] faceID = new int[numberOfFaces * 3];
            int[] faceArrayCount = new int[meshes.Count + 1]; faceArrayCount[0] = 0;

            int nMesh = meshes.Count;



            for (int i = 0; i < meshes.Count; i++) {

                vertexArrayCount[i + 1] = vertexArrayCount[i] + meshes[i].Vertices.Count;
                faceArrayCount[i + 1] = faceArrayCount[i] + meshes[i].Faces.Count;

                for (int j = 0; j < meshes[i].Vertices.Count; j++) {
                    int n = vertexArrayCount[i] * 3;
                    coord[n + (j * 3 + 0)] = meshes[i].Vertices.Point3dAt(j).X;
                    coord[n + (j * 3 + 1)] = meshes[i].Vertices.Point3dAt(j).Y;
                    coord[n + (j * 3 + 2)] = meshes[i].Vertices.Point3dAt(j).Z;
                    ////Rhino.RhinoApp.WriteLine((n + (j * 3 + 0).ToString() + " " + meshes[i].Vertices[j].X.ToString()));
                    ////Rhino.RhinoApp.WriteLine((n + (j * 3 + 1).ToString() + " " + meshes[i].Vertices[j].Y.ToString()));
                    ////Rhino.RhinoApp.WriteLine((n + (j * 3 + 2).ToString() + " " + meshes[i].Vertices[j].Z.ToString()));
                }

                for (int j = 0; j < meshes[i].Faces.Count; j++) {
                    int n = faceArrayCount[i] * 3;
                    faceID[n + (j * 3 + 0)] = meshes[i].Faces[j].A;
                    faceID[n + (j * 3 + 1)] = meshes[i].Faces[j].B;
                    faceID[n + (j * 3 + 2)] = meshes[i].Faces[j].C;
                }

            }


            //Rhino.RhinoApp.WriteLine(coord.Length.ToString());
            //foreach(var c in vertexArrayCount)
            //Rhino.RhinoApp.Write(c.ToString()+" ");
            //Rhino.RhinoApp.WriteLine();
            //Rhino.RhinoApp.WriteLine(faceID.Length.ToString());
            // foreach (var c in faceArrayCount)
            //Rhino.RhinoApp.Write(c.ToString() + " ");
            //Rhino.RhinoApp.WriteLine();
            //Rhino.RhinoApp.WriteLine(nMesh.ToString());




            ////Inputs
            IntPtr vertexCoordPointer = IntPtr.Zero;
            int nVertices = 0;
            IntPtr faceIndicesPointer = IntPtr.Zero;
            int nFaces = 0;
            int numberOfValidMeshes = -1;

            //Call C++ method
            UnsafeCGAL.MeshBoolean_CreateArray(coord, vertexArrayCount, faceID, faceArrayCount, (ulong)nMesh, (ulong)Math.Max(0, Math.Min(Difference_Union_Intersection, 2)), ref vertexCoordPointer, ref nVertices, ref faceIndicesPointer, ref nFaces, ref numberOfValidMeshes);
            //Rhino.RhinoApp.WriteLine(numberOfValidMeshes.ToString());
            //return null;


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Get Vertices and Faces from C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Convert faceIndicesPointer to C# int[]
            double[] verticesCoordinates = new double[nVertices * 3];
            Marshal.Copy(vertexCoordPointer, verticesCoordinates, 0, verticesCoordinates.Length);

            int[] faceIndices = new int[nFaces * 3];
            Marshal.Copy(faceIndicesPointer, faceIndices, 0, faceIndices.Length);

            //Rhino.RhinoApp.WriteLine(verticesCoordinates.Length.ToString()  + " " + nVertices.ToString());
            //Rhino.RhinoApp.WriteLine(faceIndices.Length.ToString() + " " + nFaces.ToString());



            //Create mesh
            Mesh mesh = new Mesh();
            for (int i = 0; i < verticesCoordinates.Length; i += 3)
                mesh.Vertices.Add(new Point3d(verticesCoordinates[i + 0], verticesCoordinates[i + 1], verticesCoordinates[i + 2]));

            for (int i = 0; i < faceIndices.Length; i += 3) {
                mesh.Faces.AddFace(faceIndices[i + 0], faceIndices[i + 1], faceIndices[i + 2]);
            }

            //mesh.Vertices.Align(0.01);
            mesh.Vertices.CombineIdentical(true, true);
            mesh.Vertices.CullUnused();
            mesh.Weld(3.14159265358979);
            mesh.FillHoles();
            mesh.RebuildNormals();


            //Release memory from output
            UnsafeCGAL.ReleaseDouble(vertexCoordPointer, true);
            UnsafeCGAL.ReleaseInt(faceIndicesPointer, true);


            return mesh;


        }


        public static Mesh CreateMeshBooleanArrayTrackColors(Mesh[] m_, System.Drawing.Color color , int Difference_Union_Intersection = 0 ) {
            try {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Clean Mesh
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var meshes = new List<Mesh>(m_.Length);

                for (int i = 0; i < m_.Length; i++) {

                    if (!m_[i].IsClosed)
                        continue;

                    m_[i].Vertices.UseDoublePrecisionVertices = true;
                    Mesh m = m_[i].DuplicateMesh();
                    m.Vertices.UseDoublePrecisionVertices = true;
                    m.Faces.ConvertQuadsToTriangles();
                    m.Vertices.CombineIdentical(true, true);
                    m.Vertices.CullUnused();
                    m.Weld(3.14159265358979);
                    m.FillHoles();
                    m.RebuildNormals();
                    m.Vertices.UseDoublePrecisionVertices = true;

                    if (m.IsValid && m.IsClosed)
                        meshes.Add(m);
                }

                if (meshes.Count < 2)
                    return null;




                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Send Vertices and Faces to C++
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////




                int numberOfVertices = 0;
                int numberOfFaces = 0;
                foreach (Mesh tempMesh in meshes) {
                    numberOfVertices += tempMesh.Vertices.Count;
                    numberOfFaces += tempMesh.Faces.Count;
                }

                double[] coord = new double[numberOfVertices * 3];
                int[] vertexArrayCount = new int[meshes.Count + 1]; vertexArrayCount[0] = 0;

                int[] faceID = new int[numberOfFaces * 3];
                int[] faceArrayCount = new int[meshes.Count + 1]; faceArrayCount[0] = 0;

                int nMesh = meshes.Count;



                for (int i = 0; i < meshes.Count; i++) {

                    vertexArrayCount[i + 1] = vertexArrayCount[i] + meshes[i].Vertices.Count;
                    faceArrayCount[i + 1] = faceArrayCount[i] + meshes[i].Faces.Count;

                    for (int j = 0; j < meshes[i].Vertices.Count; j++) {
                        int n = vertexArrayCount[i] * 3;
                        coord[n + (j * 3 + 0)] = meshes[i].Vertices.Point3dAt(j).X;
                        coord[n + (j * 3 + 1)] = meshes[i].Vertices.Point3dAt(j).Y;
                        coord[n + (j * 3 + 2)] = meshes[i].Vertices.Point3dAt(j).Z;
                        ////Rhino.RhinoApp.WriteLine((n + (j * 3 + 0).ToString() + " " + meshes[i].Vertices[j].X.ToString()));
                        ////Rhino.RhinoApp.WriteLine((n + (j * 3 + 1).ToString() + " " + meshes[i].Vertices[j].Y.ToString()));
                        ////Rhino.RhinoApp.WriteLine((n + (j * 3 + 2).ToString() + " " + meshes[i].Vertices[j].Z.ToString()));
                    }

                    for (int j = 0; j < meshes[i].Faces.Count; j++) {
                        int n = faceArrayCount[i] * 3;
                        faceID[n + (j * 3 + 0)] = meshes[i].Faces[j].A;
                        faceID[n + (j * 3 + 1)] = meshes[i].Faces[j].B;
                        faceID[n + (j * 3 + 2)] = meshes[i].Faces[j].C;
                    }

                }


                //Rhino.RhinoApp.WriteLine(coord.Length.ToString());
                //foreach(var c in vertexArrayCount)
                //Rhino.RhinoApp.Write(c.ToString()+" ");
                //Rhino.RhinoApp.WriteLine();
                //Rhino.RhinoApp.WriteLine(faceID.Length.ToString());
                // foreach (var c in faceArrayCount)
                //Rhino.RhinoApp.Write(c.ToString() + " ");
                //Rhino.RhinoApp.WriteLine();
                //Rhino.RhinoApp.WriteLine(nMesh.ToString());




                ////Inputs

                ////Inputs
                IntPtr vertexCoordPointer = IntPtr.Zero;
                int nVertices = 0;
                IntPtr faceIndicesPointer = IntPtr.Zero;
                int nFaces = 0;
                IntPtr facesColorsPointer = IntPtr.Zero;
                int nFaceColors = 0;
                int numberOfValidMeshes = -1;

                //Call C++ method
                UnsafeCGAL.MeshBoolean_CreateArrayTrackColors(
                    coord, vertexArrayCount,
                    faceID, faceArrayCount,
                    (ulong)nMesh,

                    (ulong)Math.Max(0, Math.Min(Difference_Union_Intersection, 2)),

                    ref vertexCoordPointer, ref nVertices,
                    ref faceIndicesPointer, ref nFaces,
                    ref facesColorsPointer, ref nFaceColors,
                    ref numberOfValidMeshes);
                //Rhino.RhinoApp.WriteLine(numberOfValidMeshes.ToString());
                //return null;


                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Get Vertices and Faces from C++
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Convert faceIndicesPointer to C# int[]
                double[] verticesCoordinates = new double[nVertices * 3];
                Marshal.Copy(vertexCoordPointer, verticesCoordinates, 0, verticesCoordinates.Length);

                int[] faceIndices = new int[nFaces * 3];
                Marshal.Copy(faceIndicesPointer, faceIndices, 0, faceIndices.Length);

                int[] faceColorsIndices = new int[nFaceColors];
                Marshal.Copy(facesColorsPointer, faceColorsIndices, 0, faceColorsIndices.Length);

                //Rhino.RhinoApp.WriteLine(verticesCoordinates.Length.ToString()  + " " + nVertices.ToString());
                //Rhino.RhinoApp.WriteLine(faceIndices.Length.ToString() + " " + nFaces.ToString());
                //Rhino.RhinoApp.WriteLine(faceColorsIndices.Length.ToString() + " " + nFaceColors.ToString());



                //Create mesh
                Mesh mesh = new Mesh();
                for (int i = 0; i < verticesCoordinates.Length; i += 3)
                    mesh.Vertices.Add(new Point3d(verticesCoordinates[i + 0], verticesCoordinates[i + 1], verticesCoordinates[i + 2]));

                for (int i = 0; i < faceIndices.Length; i += 3) {
                    mesh.Faces.AddFace(faceIndices[i + 0], faceIndices[i + 1], faceIndices[i + 2]);
                }


                mesh.RebuildNormals();

                UnsafeCGAL.ReleaseDouble(vertexCoordPointer, true);
                UnsafeCGAL.ReleaseInt(faceIndicesPointer, true);
                UnsafeCGAL.ReleaseInt(facesColorsPointer, true);

                if (!mesh.IsValid) {

                    mesh.Vertices.Align(0.01);
                    mesh.Vertices.CombineIdentical(true, true);
                    mesh.Vertices.CullUnused();
                    mesh.Weld(3.14159265358979);
                    mesh.FillHoles();
                    mesh.Unweld(0, true);
                    mesh.VertexColors.CreateMonotoneMesh(System.Drawing.Color.FromArgb(200, 200, 200));
                    return mesh;
                    //Rhino.RhinoDoc.ActiveDoc.Objects.AddMesh(mesh.Vertices);
                }

                //Rhino.RhinoApp.WriteLine(mesh.IsValid.ToString() );


    
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////Colorize Cuts
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Mesh r_ = r.DuplicateMesh();
                mesh.Unweld(0, true);
                mesh.VertexColors.CreateMonotoneMesh(System.Drawing.Color.Black);

                bool flag = color.R == 0 && color.G == 0 && color.B == 0;

                var colorWhite = System.Drawing.Color.FromArgb(200, 200, 200);

                double colorScale = 255.0 / meshes.Count;

                //int[] colors = new int[faceColorsIndices.Length];
                for (int i = 0; i < faceColorsIndices.Length; i++) {

                    // Rhino.RhinoApp.WriteLine(faceColorsIndices[i].ToString());

                    int c = (int)(colorScale * (faceColorsIndices[i] - 1));

                    if (faceColorsIndices[i] == 1) {

                        mesh.VertexColors.SetColor(mesh.Faces[i].A, colorWhite);
                        mesh.VertexColors.SetColor(mesh.Faces[i].B, colorWhite);
                        mesh.VertexColors.SetColor(mesh.Faces[i].C, colorWhite);
                    } else if (faceColorsIndices[i] != 1 && flag) {

                        var colorRed = System.Drawing.Color.FromArgb(255, c, 0);
                        mesh.VertexColors.SetColor(mesh.Faces[i].A, colorRed);
                        mesh.VertexColors.SetColor(mesh.Faces[i].B, colorRed);
                        mesh.VertexColors.SetColor(mesh.Faces[i].C, colorRed);
                    } else if (faceColorsIndices[i] != 1) {
                        mesh.VertexColors.SetColor(mesh.Faces[i].A, color);
                        mesh.VertexColors.SetColor(mesh.Faces[i].B, color);
                        mesh.VertexColors.SetColor(mesh.Faces[i].C, color);
                    }


                }



                return mesh;
            }catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
                return new Mesh();
            }

        }

        public static Polyline[] CreateMeshSkeleton(Mesh m_) {


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Clean Mesh
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            m_.Vertices.UseDoublePrecisionVertices = true;
            Mesh m = m_.DuplicateMesh();
            m.Vertices.UseDoublePrecisionVertices = true;
            m.Faces.ConvertQuadsToTriangles();
            m.Vertices.CombineIdentical(true, true);
            m.Vertices.CullUnused();
            m.Weld(3.14159265358979);
            m.FillHoles();
            m.RebuildNormals();
            m.Vertices.UseDoublePrecisionVertices = true;

            if (!m.IsValid && !m.IsClosed)
                return null;


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Send Vertices and Faces to C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            double[] ptCoordArr1 = new double[m.Vertices.Count * 3];
            for (int i = 0; i < m.Vertices.Count; i++) {
                ptCoordArr1[i * 3 + 0] = m.Vertices[i].X;
                ptCoordArr1[i * 3 + 1] = m.Vertices[i].Y;
                ptCoordArr1[i * 3 + 2] = m.Vertices[i].Z;
            }
            var ptCount1 = (ulong)m.Vertices.Count;


            int[] facesArr1 = m.Faces.ToIntArray(true);
            var facesCount1 = (ulong)m.Faces.Count;

            //Rhino.RhinoApp..WriteLine("Number of Vertices " + ptCount1.ToString());
            //Rhino.RhinoApp..WriteLine("Number of Faces " + facesCount1.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Call unsafe method
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            IntPtr vertexCoordPointer = IntPtr.Zero;
            int nVertices = 0;
            IntPtr faceIndicesPointer = IntPtr.Zero;
            int nFaces = 0;

            UnsafeCGAL.MeshSkeleton_Create(ptCoordArr1, ptCount1, facesArr1, facesCount1, ref vertexCoordPointer, ref nVertices, ref faceIndicesPointer, ref nFaces);



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Get Vertices and Faces from C++
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Convert faceIndicesPointer to C# int[]
            double[] verticesCoordinates = new double[nVertices * 3];
            Marshal.Copy(vertexCoordPointer, verticesCoordinates, 0, verticesCoordinates.Length);

            int[] faceIndices = new int[nFaces];
            Marshal.Copy(faceIndicesPointer, faceIndices, 0, faceIndices.Length);

            //Rhino.RhinoApp..WriteLine(verticesCoordinates.Length.ToString()  + " " + nVertices.ToString());
            //Rhino.RhinoApp..WriteLine(faceIndices.Length.ToString() + " " + nFaces.ToString());



            //Create mesh

            List<Polyline> plines = new List<Polyline>(4);
            Polyline polyline = new Polyline();
            int lastID = 1;
            for (int i = 0; i < verticesCoordinates.Length; i += 3) {

                // 0 2 5

                int currID = i == 0 ? faceIndices[0] : faceIndices[(int)(i / 3)];
                //Rhino.RhinoApp..WriteLine("CurrID " + currID.ToString());
                if (lastID != currID) {
                    plines.Add(polyline);
                    polyline = new Polyline();
                }

                polyline.Add(new Point3d(verticesCoordinates[i + 0], verticesCoordinates[i + 1], verticesCoordinates[i + 2]));


                lastID = currID;

            }
            plines.Add(polyline);//last pline
                                 //Rhino.RhinoApp..WriteLine("Number of plines " + plines.Count.ToString());

            UnsafeCGAL.ReleaseDouble(vertexCoordPointer, true);
            UnsafeCGAL.ReleaseInt(faceIndicesPointer, true);

            return plines.ToArray();


        }




        public static PointCloud CreateNormals(PointCloud cloud, double radius = 0.1, int iterations = 30, int neighbours = 100,bool erase = true) {

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


            UnsafeCGAL.ComputeNormals(
                p, p_c,
                n, n_c,
                c, c_c,
                radius,
                iterations,
                neighbours,
                erase,
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

            int count = P.Length / 3;
            Point3d[] location = new Point3d[count];
            Vector3d[] normals = new Vector3d[count];
            Color[] colors = new Color[count];

 
            for (int i = 0; i < P.Length; i += 3) {

                int id = i / 3;
                location[id] = new Point3d(P[i + 0], P[i + 1], P[i + 2]);
                normals[id] = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
                colors[id] = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);
            }
           newCloud.AddRange(location, normals, colors);







            //for (int i = 0; i < P.Length; i += 3) {
            //    newCloud.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

            //    newCloud[(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)c[i + 0], (int)c[i + 1], (int)c[i + 2]);

            //}


            //for (int i = 0; i < P.Length; i += 3) {
            //    if (N[i + 0] == 0 && N[i + 1] == 0 && N[i + 2] == 0) continue;
            //    newCloud[(int)Math.Round((i / 3.0))].Normal = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
            //}




            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Rhino.RhinoApp.WriteLine(newCloud.Count.ToString());
            return newCloud;

        }




        public static Mesh CreatePoissonSurfaceReconstruction(PointCloud cloud, double radius = 0.1, int iterations = 30, int neighbours = 100) {

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


            UnsafeCGAL.ComputePoissonSurfaceReconstruction(
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
            Marshal.Copy(n_pointer, N, 0, N.Length);

            double[] C = new double[c_pointer_c];
            Marshal.Copy(c_pointer, C, 0, C.Length);
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


            Mesh mesh = new Mesh();

            for (int i = 0; i < P.Length; i += 3) {
                mesh.Vertices.Add(new Point3d(P[i + 0], P[i + 1], P[i + 2]));

            }

            for (int i = 0; i < N.Length; i += 3) {
                mesh.Faces.AddFace((int)N[i + 0], (int)N[i + 1], (int)N[i + 2]);

            }



            mesh.RebuildNormals();

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            return mesh;

        }









        public static PointCloud[] Clustering(PointCloud cloud, PointCloud cloudRef,  double radius =0,double neighbours = 0, bool split = false) {

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
  
            IntPtr n_pointer = IntPtr.Zero;
            int n_pointer_c = 0;
            IntPtr c_pointer = IntPtr.Zero;
            int c_pointer_c = 0;
            int numberOfClusters = 0;

            UnsafeCGAL.Cluster(
                p, p_c,
                n, n_c,
                c, c_c,
                radius,
                100,
                (int)neighbours,
                ref n_pointer, ref n_pointer_c,
                ref c_pointer, ref c_pointer_c,
                ref numberOfClusters
                );


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("C++ Downsample "+ watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert Pointers to double arrays
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert faceIndicesPointer to C# int[]

            //watch = System.Diagnostics.Stopwatch.StartNew();
            //double[] P = new double[p_pointer_c];
            //Marshal.Copy(p_pointer, P, 0, P.Length);

            int[] N = new int[n_pointer_c];
            Marshal.Copy(n_pointer, N, 0, N.Length);

            double[] C = new double[c_pointer_c];
            Marshal.Copy(c_pointer, C, 0, C.Length);
            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Convert Pointer to double Array "+watch.ElapsedMilliseconds.ToString());

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Release C++ memory
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
            UnsafeOpen3D.ReleaseInt(n_pointer, true);
            UnsafeOpen3D.ReleaseDouble(c_pointer, true);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Convert C++ to C# Pointcloud
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //watch = System.Diagnostics.Stopwatch.StartNew();
           // Rhino.RhinoApp.WriteLine("Input" + cloud.Count.ToString());
            if (!split) {
                PointCloud newCloud = new PointCloud(cloudRef);

                //int count = 0;
                for (int i = 0; i < C.Length; i += 3) {
                    newCloud[(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)C[i + 0], (int)C[i + 1], (int)C[i + 2]);                 
                }

                return new PointCloud[] { newCloud};
            } else {

                //Initialize number of clouds
                var clouds = new PointCloud[numberOfClusters];
                int[] counter = new int[clouds.Length];
                int[] counterIterative = new int[clouds.Length];


                Point3d[][] points = new Point3d[numberOfClusters][];
                Vector3d[][] normals = new Vector3d[numberOfClusters][];
                Color[][] colors = new Color[numberOfClusters][];

                for (int i = 0; i < numberOfClusters; i++) {
                    clouds[i] = new PointCloud();
                    counter[i] = 0;
                    counterIterative[i] = 0;
                }

                for (int i = 0; i < N.Length; i++) {
                    counter[N[i]]++;
                }

                for (int i = 0; i < numberOfClusters; i++) {
                    points[i] = new Point3d[counter[i]];
                    normals[i] = new Vector3d[counter[i]];
                    colors[i] = new System.Drawing.Color[counter[i]];
                }



                for (int i = 0; i < N.Length; i++) {

                    points[N[i]][counterIterative[N[i]]] = cloudRef[i].Location;
                    normals[N[i]][counterIterative[N[i]]] = cloudRef[i].Normal;
                    colors[N[i]][counterIterative[N[i]]] = cloudRef[i].Color;

                    counterIterative[N[i]]++;

                }


                for (int i = 0; i < numberOfClusters; i++) {
                    clouds[i].AddRange(points[i],normals[i],colors[i]);
                }

                    //Rhino.RhinoApp.WriteLine(clouds.Length.ToString());
                    return clouds;

            }

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      

        }










        public static PointCloud SOR(PointCloud cloud, double radius = 0, double percent = 0, int neighbours = 0, int type = 0) {

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


            UnsafeCGAL.SOR(
                p, p_c,
                n, n_c,
                c, c_c,
                radius,
                percent,
                neighbours,
                type,
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






            Point3d[] pts = new Point3d[(int)(P.Length/3)];
            for (int i = 0; i < P.Length; i += 3) {
                pts[(int)(i/3)] = new Point3d(P[i + 0], P[i + 1], P[i + 2]);
                //newCloud[(int)Math.Round((i / 3.0))].Color = System.Drawing.Color.FromArgb((int)c[i + 0], (int)c[i + 1], (int)c[i + 2]);

            }
            newCloud.AddRange(pts);

            //for (int i = 0; i < P.Length; i += 3) {
            //    if (N[i + 0] == 0 && N[i + 1] == 0 && N[i + 2] == 0) continue;
            //    newCloud[(int)Math.Round((i / 3.0))].Normal = new Vector3d(N[i + 0], N[i + 1], N[i + 2]);
            //}




            //watch.Stop();
            //Rhino.RhinoApp.WriteLine("Create PointCloud from double arrays "+ watch.ElapsedMilliseconds.ToString());


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Output
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Rhino.RhinoApp.WriteLine(newCloud.Count.ToString());
            return newCloud;

        }








    }
}
