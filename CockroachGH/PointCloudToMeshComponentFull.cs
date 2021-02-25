using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace CockroachGH {
    public class PointCloudToMeshComponentFull : GH_Component {

        public PointCloudToMeshComponentFull()
          : base("PointCloudToMeshFull", "CloudToMeshFull",
              "PointCloudToMesh",
 "Cockroach", "Mesh") {
        }

   
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new PointCloudGHParam(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Downsample", "D", "Downsample", GH_ParamAccess.item, 5000);
            pManager.AddIntegerParameter("NormalsNeighbours", "N", "NormalsNeighbours", GH_ParamAccess.item, 100);
            pManager.AddBooleanParameter("Debug", "D", "No Debug Info", GH_ParamAccess.item, false);

            pManager.AddIntegerParameter("PoissonMaxDepth", "PD", "MaximumDeptOfReconstructionSurfaceTree", GH_ParamAccess.item, 8);
            pManager.AddIntegerParameter("FinestLevel", "PF", "TargetWidthOfTheFinestLevelOctree", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("CubeRatio", "PC", "RatioBetweenReconCubeAndBBCubeStd", GH_ParamAccess.item, 1.1);
            pManager.AddBooleanParameter("LinearInterpolation", "PI", "ReconstructorUsingLinearInterpolation", GH_ParamAccess.item, false);



            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;


            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Iso", "I", "Iso", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA) {

            int Downsample = 5000;
            int NormalsNeighbours = 100;
            bool debug = false;

            int maximumDeptOfReconstructionSurfaceTree = 8;
            int targetWidthOfTheFinestLevelOctree = 0;
            double ratioBetweenReconCubeAndBBCubeStd = 1.1;
            bool ReconstructorUsingLinearInterpolation = false;

            DA.GetData(1, ref Downsample);
            DA.GetData(2, ref NormalsNeighbours);
            DA.GetData(3, ref debug);
            DA.GetData(4, ref maximumDeptOfReconstructionSurfaceTree);
            DA.GetData(5, ref targetWidthOfTheFinestLevelOctree);
            DA.GetData(6, ref ratioBetweenReconCubeAndBBCubeStd);
            DA.GetData(7, ref ReconstructorUsingLinearInterpolation);

            //Guid to PointCloud
            //PointCloud c = new PointCloud();
            PointCloudGH c = new PointCloudGH();

            string debugInfo = debug ? "1" : "0";
            

            if (DA.GetData(0, ref c)) {
                if (!c.IsValid) return;
                if (c.Value.Count==0) return;
                Downsample = Math.Min(Downsample, c.Value.Count);

               // var watch = System.Diagnostics.Stopwatch.StartNew();
                // the code that you want to measure comes here

                /////////////////////////////////////////////////////////////////
                //Get Directory
                /////////////////////////////////////////////////////////////////
                string assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string assemblyPath = System.IO.Path.GetDirectoryName(assemblyLocation);


                /////////////////////////////////////////////////////////////////
                //write PointCloud to PLY
                /////////////////////////////////////////////////////////////////
                PlyReaderWriter.PlyWriter.SavePLY(c.Value, assemblyPath + @"\in.ply");
                //Rhino.RhinoApp.WriteLine("PointCloudToMesh. Saved Input: " + assemblyPath + @"\in.ply");
                //watch.Stop();
                //Rhino.RhinoApp.WriteLine((watch.ElapsedMilliseconds / 1000.0).ToString());

                /////////////////////////////////////////////////////////////////
                //Ply to Mesh to Obj
                /////////////////////////////////////////////////////////////////
                //watch = System.Diagnostics.Stopwatch.StartNew();
                //tring argument = assemblyPath + "TestVisualizer.exe " + "-1 " + "100";//--asci 
                string argument = " "+Downsample.ToString()+ " " + NormalsNeighbours.ToString() + " " + debugInfo + " " + maximumDeptOfReconstructionSurfaceTree.ToString() + " " + targetWidthOfTheFinestLevelOctree.ToString() + " " + ratioBetweenReconCubeAndBBCubeStd.ToString() + " " + Convert.ToInt32(ReconstructorUsingLinearInterpolation).ToString();
                //--asci 
                                                                                                                  // Rhino.RhinoApp.WriteLine("PointCloudToMesh. Arguments: " + argument );



                // Rhino.RhinoApp.WriteLine("PointCloudToMesh. Directory: " + assemblyPath + @"\TestVisualizer.exe");

                if (debug) {
                    var proc = new System.Diagnostics.Process {
                        StartInfo = new System.Diagnostics.ProcessStartInfo {
                            FileName = assemblyPath + @"\TestVisualizer.exe",//filePath+"PoissonRecon.exe",
                            Arguments = argument,
                            //UseShellExecute = false,
                            //RedirectStandardOutput = true,
                            CreateNoWindow = false,
                            // WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            WorkingDirectory = assemblyPath + @"\TestVisualizer.exe"
                        }
                    };

                    proc.Start();
                 
                    proc.WaitForExit();
                } else {
                    var proc = new System.Diagnostics.Process {
                        StartInfo = new System.Diagnostics.ProcessStartInfo {
                            FileName = assemblyPath + @"\TestVisualizer.exe",//filePath+"PoissonRecon.exe",
                            Arguments = argument,
                            //UseShellExecute = false,
                            //RedirectStandardOutput = true,
                            CreateNoWindow = true,
                             WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            WorkingDirectory = assemblyPath + @"\TestVisualizer.exe"
                        }
                    };
                    proc.Start();
                    proc.WaitForExit();
                }

               // watch.Stop();
                //Rhino.RhinoApp.WriteLine((watch.ElapsedMilliseconds / 1000.0).ToString());

                /////////////////////////////////////////////////////////////////
                //Read Obj
                /////////////////////////////////////////////////////////////////
                ///

                //Outputs
               // watch = System.Diagnostics.Stopwatch.StartNew();

                // Initialize
                var obj = new ObjParser.Obj();

                // Read Wavefront OBJ file
                //obj.LoadObj(@"C:\libs\windows\out.obj");
            

                //PlyReaderWriter.PlyLoader plyLoader = new PlyReaderWriter.PlyLoader();
                //Mesh mesh3D = plyLoader.load(assemblyPath + @"\out.ply")[0];


                //Rhino.RhinoApp.WriteLine(assemblyPath + @"\windows\out.obj");
                obj.LoadObj(assemblyPath + @"\out.obj");

                Mesh mesh3D = new Mesh();
                foreach (ObjParser.Types.Vertex v in obj.VertexList) {
                    mesh3D.Vertices.Add(new Point3d(v.X, v.Y, v.Z));
                    mesh3D.VertexColors.Add(System.Drawing.Color.FromArgb((int)(v.r * 255), (int)(v.g * 255), (int)(v.b * 255)  ));
                }
 
                int num = checked(mesh3D.Vertices.Count - 1);

                foreach (ObjParser.Types.Face f in obj.FaceList) {

                    string[] lineData = f.ToString().Split(' ');
                    string[] v0 = lineData[1].Split('/');
                    string[] v1 = lineData[2].Split('/');
                    string[] v2 = lineData[3].Split('/');

                    MeshFace mf3D = new MeshFace(Convert.ToInt32(v0[0]) - 1, Convert.ToInt32(v1[0]) - 1, Convert.ToInt32(v2[0]) - 1);
                    if (mf3D.IsValid())
                        if (!(mf3D.A > num || mf3D.B > num || mf3D.C > num || mf3D.D > num))
                            mesh3D.Faces.AddFace(mf3D);


                }








                DA.SetData(0, mesh3D);

                /////////////////////////////////////////////////////////////////
                //Output Iso Values
                /////////////////////////////////////////////////////////////////
                string[] lines = System.IO.File.ReadAllLines(assemblyPath + @"\out.txt");
                double[] iso = new double[lines.Length];
                for (int i = 0; i < lines.Length; i++) { 
                    iso[i] = Convert.ToDouble(lines[i]);
                }
                //watch.Stop();
                //Rhino.RhinoApp.WriteLine((watch.ElapsedMilliseconds/1000.0).ToString());

                DA.SetDataList(1, iso);

            }


        }


        protected override System.Drawing.Bitmap Icon {
            get {
                return null;
            }
        }


        public override Guid ComponentGuid {
            get { return new Guid("43e64ee0-a0c8-45a7-abcc-cbb911d4a950"); }
        }
    }
}
