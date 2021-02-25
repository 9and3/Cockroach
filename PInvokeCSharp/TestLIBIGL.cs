using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PInvokeCSharp {
    public static class TestLIBIGL {

        public static Mesh CreateMeshBoolean(Mesh m1_, List<Mesh> list_m2, int Difference_Union_Intersection = 0) {

            Mesh m1 = m1_;


            for (int i = 0; i < list_m2.Count; i++) {
                var result = CreateLIBIGLMeshBoolean(m1, list_m2[i], Difference_Union_Intersection);
                if (result != null) {
                    m1 = result;
                }
            }




            return m1;
        }
        public static Mesh CreateLIBIGLMeshBooleanNoColors(Mesh[] m_, int Difference_Union_Intersection = 0) {

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
            int[] vertexArrayCount = new int[meshes.Count + 1];
            vertexArrayCount[0] = 0;

            int[] faceID = new int[numberOfFaces * 3];
            int[] faceArrayCount = new int[meshes.Count + 1];
            faceArrayCount[0] = 0;

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


            //Call C++ method
            UnsafeLIBIGL.LIBIGL_MeshBoolean_CreateArrayNoColors(
                coord, vertexArrayCount,
                faceID, faceArrayCount,
                (ulong)nMesh,

                (ulong)Math.Max(0, Math.Min(Difference_Union_Intersection, 2)),

                ref vertexCoordPointer, ref nVertices,
                ref faceIndicesPointer, ref nFaces);//
                                                    //Rhino.RhinoApp.WriteLine(numberOfValidMeshes.ToString());
                                                    // return null;


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
            Mesh r_ = new Mesh();
            for (int i = 0; i < verticesCoordinates.Length; i += 3)
                r_.Vertices.Add(new Point3d(verticesCoordinates[i + 0], verticesCoordinates[i + 1], verticesCoordinates[i + 2]));

            for (int i = 0; i < faceIndices.Length; i += 3) {
                r_.Faces.AddFace(faceIndices[i + 0], faceIndices[i + 1], faceIndices[i + 2]);
            }

            //mesh.Vertices.Align(0.01);
            //mesh.Vertices.CombineIdentical(true, true);
            //mesh.Vertices.CullUnused();
            //mesh.Weld(3.14159265358979);
            //mesh.FillHoles();
            r_.RebuildNormals();


            //Release memory from output
            UnsafeLIBIGL.ReleaseDouble(vertexCoordPointer, true);
            UnsafeLIBIGL.ReleaseInt(faceIndicesPointer, true);



            return r_;
        }



        public static Mesh CreateLIBIGLMeshBoolean(Mesh[] m_, int Difference_Union_Intersection = 0) {

            //var watch = new System.Diagnostics.Stopwatch();
            //watch.Start();
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

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine($"Execution Time Clean Mesh: {watch.ElapsedMilliseconds} ms");
            //watch.Reset();
            //watch.Start();
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


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine($"Execution Copy coordinates: {watch.ElapsedMilliseconds} ms");
            //watch.Reset();
            //watch.Start();


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
            IntPtr facesColorsPointer = IntPtr.Zero;
            int nFaceColors = 0;
            int numberOfValidMeshes = -1;

            //Call C++ method
            UnsafeLIBIGL.LIBIGL_MeshBoolean_CreateArray(
                coord, vertexArrayCount,
                faceID, faceArrayCount,
                (ulong)nMesh,

                (ulong)Math.Max(0, Math.Min(Difference_Union_Intersection, 2)),

                ref vertexCoordPointer, ref nVertices,
                ref faceIndicesPointer, ref nFaces,
                ref facesColorsPointer, ref nFaceColors,
                ref numberOfValidMeshes);//

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine($"Execution LIBIGL_MeshBoolean_CreateArray: {watch.ElapsedMilliseconds} ms");
            //watch.Reset();
            //watch.Start();
            //Rhino.RhinoApp.WriteLine(numberOfValidMeshes.ToString());
            // return null;


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



            //Create mesh
            Mesh r_ = new Mesh();
            for (int i = 0; i < verticesCoordinates.Length; i += 3)
                r_.Vertices.Add(new Point3d(verticesCoordinates[i + 0], verticesCoordinates[i + 1], verticesCoordinates[i + 2]));

            for (int i = 0; i < faceIndices.Length; i += 3) {
                r_.Faces.AddFace(faceIndices[i + 0], faceIndices[i + 1], faceIndices[i + 2]);
            }


            //mesh.Vertices.Align(0.01);
            //mesh.Vertices.CombineIdentical(true, true);
            //mesh.Vertices.CullUnused();
            //mesh.Weld(3.14159265358979);
            //mesh.FillHoles();
            r_.RebuildNormals();


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine($"Execution Construct mesh: {watch.ElapsedMilliseconds} ms");
            //watch.Reset();
            //watch.Start();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Colorize Cuts
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //Mesh r_ = r.DuplicateMesh();
            r_.Unweld(0, true);
            r_.VertexColors.CreateMonotoneMesh(System.Drawing.Color.Black);


            Interval[] domains = new Interval[meshes.Count];

            int count = 0;
            for (int i = 0; i < meshes.Count; i++) {

                domains[i] = new Interval(count, count + meshes[i].Faces.Count);
                count += meshes[i].Faces.Count;
                //if (i == 0)
                //  count += meshes[1].Faces.Count;
                //groups[i] = new List<Mesh>();
            }



            var colorWhite = System.Drawing.Color.FromArgb(200, 200, 200);

            double colorScale = 255.0 / meshes.Count;

            int[] colors = new int[faceColorsIndices.Length];
            for (int i = 0; i < faceColorsIndices.Length; i++) {



                for (int j = 0; j < domains.Length; j++) {
                    if (domains[j].IncludesParameter(faceColorsIndices[i])) {

                        int c = (int)(colorScale * j);

                        if (c == 0) {

                            r_.VertexColors.SetColor(r_.Faces[i].A, colorWhite);
                            r_.VertexColors.SetColor(r_.Faces[i].B, colorWhite);
                            r_.VertexColors.SetColor(r_.Faces[i].C, colorWhite);
                        } else {
                            var colorRed = System.Drawing.Color.FromArgb(255, c, 0);
                            r_.VertexColors.SetColor(r_.Faces[i].A, colorRed);
                            r_.VertexColors.SetColor(r_.Faces[i].B, colorRed);
                            r_.VertexColors.SetColor(r_.Faces[i].C, colorRed);
                        }


                        colors[i] = j;
                    }
                }


            }


            //watch.Stop();
            //Rhino.RhinoApp.WriteLine($"Execution Create Colors: {watch.ElapsedMilliseconds} ms");
            //watch.Reset();
            //watch.Start();


            //Release memory from output
            UnsafeLIBIGL.ReleaseDouble(vertexCoordPointer, true);
            UnsafeLIBIGL.ReleaseInt(faceIndicesPointer, true);
            UnsafeLIBIGL.ReleaseInt(facesColorsPointer, true);

            //watch.Stop();
            //Rhino.RhinoApp.WriteLine($"Execution Release Memory: {watch.ElapsedMilliseconds} ms");
            //watch.Reset();


            return r_;
        }


        public static Mesh CreateLIBIGLMeshBoolean(Mesh m1_, Mesh m2_, int Difference_Union_Intersection = 0) {
            // return Unsafe.LIBIGL_MeshBoolean_Create(n);


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
            UnsafeLIBIGL.LIBIGL_MeshBoolean_Create(ptCoordArr1, ptCount1, facesArr1, facesCount1, ptCoordArr2, ptCount2, facesArr2, facesCount2, (ulong)Math.Max(0, Math.Min(Difference_Union_Intersection, 4)), ref vertexCoordPointer, ref nVertices, ref faceIndicesPointer, ref nFaces);




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
            UnsafeLIBIGL.ReleaseDouble(vertexCoordPointer, true);
            UnsafeLIBIGL.ReleaseInt(faceIndicesPointer, true);


            return mesh;
        }




    }
}
