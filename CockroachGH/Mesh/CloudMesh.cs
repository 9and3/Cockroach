using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudMesh : GH_Component {

        public CloudMesh()
  : base("CloudMesh", "CloudMesh",
      "CloudMesh",
      "Cockroach", "Mesh") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "Cloud", "PointCloud", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Max", "Max", "Max", GH_ParamAccess.item,8);
            pManager.AddIntegerParameter("Min", "Min", "Min", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Scale", "Scale", "Scale", GH_ParamAccess.item,1.1);
            pManager.AddBooleanParameter("Linear", "Linear", "Linear", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Color", "Color", "Color transfer from pointcloud to mesh", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Open3d/CGAL", "O/C", "Run Open3D - true, CGAL - false", GH_ParamAccess.item, true);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddNumberParameter("Density", "D", "Density", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);

                int PoisonMaxDepth = 8;  int PoisonMinDepth = 0;    double PoisonScale = 1.1f; bool Linear = false; bool color = false; bool Open3d_CGAL = true;
                DA.GetData(1, ref PoisonMaxDepth);
                DA.GetData(2, ref PoisonMinDepth);
                DA.GetData(3, ref PoisonScale);
                DA.GetData(4, ref Linear);
                DA.GetData(5, ref color);
                DA.GetData(6, ref Open3d_CGAL);


      

                if (Open3d_CGAL) {
                    var m = PInvokeCSharp.TestOpen3D.Poisson(cgh.Value, (ulong)PoisonMaxDepth, (ulong)PoisonMinDepth, (float)PoisonScale, Linear);
                    Mesh mm = m.Item1;
                    if (!color)
                        mm.VertexColors.Clear();
                    //this.Message = c.Count.ToString();

                    DA.SetData(0, mm);
                    DA.SetDataList(1, m.Item2);
                } else {
                    DA.SetData(0, PInvokeCSharp.TestCGAL.CreatePoissonSurfaceReconstruction(cgh.Value));

                }

                
            } catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd1-edf34cd15ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Poisson2;
    }
}