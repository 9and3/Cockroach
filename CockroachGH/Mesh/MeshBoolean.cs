using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using PInvokeCSharp;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace CockroachGH {
    public class MeshBoolean : GH_Component {

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        Mesh m_ = new Mesh();
        List<Line> l_ = new List<Line>();
        BoundingBox bbox_ = BoundingBox.Unset;


        protected override void BeforeSolveInstance() {
            m_ = new Mesh();
            l_ = new List<Line>();
            bbox_ = BoundingBox.Unset;
        }

        public override BoundingBox ClippingBox => bbox_;

        public override void DrawViewportMeshes(IGH_PreviewArgs args) {

            if (this.Hidden || this.Locked || m_==null) return;
            args.Display.DrawMeshFalseColors(m_);//mat
            

        }

        public override void DrawViewportWires(IGH_PreviewArgs args) {

            bool edgesOn = Grasshopper.CentralSettings.PreviewMeshEdges;
           var col = Attributes.Selected ? args.WireColour_Selected : args.WireColour;

            if (this.Hidden || this.Locked || m_ == null) return;
            if (l_.Count == 0 && edgesOn) {

                args.Display.DrawMeshWires(m_, col);
            } else {
                args.Display.DrawLines(l_, System.Drawing.Color.Black, 2);
            }
        }

        public MeshBoolean()
          : base("MeshBoolean", "MeshBoolean",
              "MeshBoolean",
              "Cockroach", "Mesh") {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {

            pManager.AddMeshParameter("Mesh0", "M0", "One Mesh to subtract from", GH_ParamAccess.tree);
            pManager.AddMeshParameter("Mesh1", "M1", "Multiple Meshes as cutters", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Type", "T", "0 Difference, 1 Union, 2 Intersection", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Scale", "S", "Scale cutters", GH_ParamAccess.item, 1.000);
            pManager.AddColourParameter("Color", "C", "Color for cuts", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Edges", "E", "Show sharp mesh edge", GH_ParamAccess.item,false);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA) {

            Random random = new Random();
            //Input
            GH_Structure<GH_Mesh> B0 = new GH_Structure<GH_Mesh>();
            GH_Structure<GH_Mesh> B1 = new GH_Structure<GH_Mesh>();
            DA.GetDataTree(0, out B0);
            DA.GetDataTree(1, out B1);

            int t = 0;
            DA.GetData(2, ref t);



            double scale = 1.001;
            DA.GetData(3, ref scale);


            bool showEdges = false;
            DA.GetData(5, ref showEdges);


            var color = System.Drawing.Color.White;
            bool f = DA.GetData(4, ref color);
            if (color.R == 255 && color.G == 255 && color.B == 255) {
                f = false;
            }

            //Match trees

            var result = new DataTree<Mesh>();



            for (int i = 0; i < B0.PathCount; i++) {

                var path = B0.Paths[i];


                result.Add(B0.get_DataItem(path, 0).Value, path);
                if (!B1.PathExists(path)) continue;

                HashSet<Point3d> cylindersCenters = new HashSet<Point3d>();

  

                var nonNulls = new List<GH_Mesh>();
                foreach (var v in B1[path]) {
                    if (v != null) {
                        nonNulls.Add(v);
                    }
                }


                for (int j = 0; j < nonNulls.Count; j++) {

                    var brep = nonNulls[j].Value.DuplicateMesh();
                    brep.Transform(Rhino.Geometry.Transform.Scale(brep.GetBoundingBox(true).Center, scale));
                    Mesh mesh = brep;
                    result.Add(mesh, path);
                }

            }



            //Perform boolean
            var resultBoolean = new DataTree<Mesh>();

            for (int b = 0; b < result.BranchCount; b++) {
                GH_Path path = result.Paths[b];
                List<Mesh> m = result.Branch(path);

                //Rhino.RhinoApp.WriteLine(m.Count.ToString());
                if (m.Count == 1) {
                    Mesh mm = m[0].DuplicateMesh();
                    mm.Unweld(0, true);

                    //DA.SetDataList(0, new Mesh[] { mm });

                    //if (!f) 
                    mm.VertexColors.CreateMonotoneMesh(System.Drawing.Color.FromArgb(200, 200, 200));
                    m_.Append(mm);
                    if(showEdges)
                        l_.AddRange(MeshEdgesByAngle(mm, 0.37));
                    if (bbox_.IsValid) bbox_.Union(m_.GetBoundingBox(false)); else bbox_ = m_.GetBoundingBox(false);


                    resultBoolean.Add(mm, path);

                } else {
                    try {
                        //Mesh r = f ? PInvokeCSharp.TestLIBIGL.CreateLIBIGLMeshBoolean(m.ToArray(), t) : PInvokeCSharp.TestCGAL.CreateMeshBooleanArray(m.ToArray(), t);
                        Mesh r = f ? PInvokeCSharp.TestCGAL.CreateMeshBooleanArrayTrackColors(m.ToArray(), color,Math.Abs( t)) : PInvokeCSharp.TestCGAL.CreateMeshBooleanArray(m.ToArray(), Math.Abs(t));
                        // Mesh r = PInvokeCSharp.TestCGAL.CreateMeshBooleanArrayTrackColors(m.ToArray(), color, t) ;
                        //resultBoolean.Add(r, path);
                        if (r.IsValid) {

                            Mesh[] s = null;
                            if (t == 0 || t == 1) {
                              s = r.SplitDisjointPieces();

                                double[] a = new double[s.Length];
                                for (int i = 0; i < s.Length; i++)
                                    a[i] = s[i].GetBoundingBox(false).Diagonal.Length;

                                Array.Sort(a, s);
                            } else {
                                s = new Mesh[] { r };
                            }

                            if (!f) s[s.Length - 1].VertexColors.CreateMonotoneMesh(System.Drawing.Color.FromArgb(200, 200, 200));
                            m_.Append(s[s.Length - 1]);

                            if (showEdges)
                                l_.AddRange(MeshEdgesByAngle(s[s.Length - 1], 0.37));
                            if (bbox_.IsValid) bbox_.Union(m_.GetBoundingBox(false)); else bbox_ = m_.GetBoundingBox(false);
                            resultBoolean.Add(s[s.Length - 1], path);

                        }
                    } catch (Exception e) {
                        Rhino.RhinoApp.WriteLine(e.ToString());
                    }
                }
            }

            DA.SetDataTree(0, resultBoolean);

        }

        public List<Line> MeshEdgesByAngle( Mesh mesh, double d = 0.49) {


            //Display Naked edges
            List<Line> lines = new List<Line>();

            mesh.FaceNormals.ComputeFaceNormals();
            var te = mesh.TopologyEdges;

            for (int i = 0; i < mesh.TopologyEdges.Count; i++) {
                int[] f = mesh.TopologyEdges.GetConnectedFaces(i);
                if (f.Length == 2) {
                    double angle = Vector3d.VectorAngle(mesh.FaceNormals[f[0]], mesh.FaceNormals[f[1]]);

                    if (angle > (0.5 - d) * 3.14159265359 && angle < (0.5 + d) * 3.14159265359)
                        lines.Add(te.EdgeLine(i));
                }

            }


            return lines;
        }


        protected override System.Drawing.Bitmap Icon {
            get {
                return Properties.Resources.MeshBoolean;
            }
        }

        public override Guid ComponentGuid {
            get { return new Guid("2131b444-930f-406c-88b3-544093c64ba2"); }
        }
    }
}