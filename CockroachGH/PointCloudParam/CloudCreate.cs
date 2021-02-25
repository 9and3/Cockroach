using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudCreate : GH_Component {

        public CloudCreate()
  : base("CloudCreate", "Create",
      "CloudCreate",
      "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
       
            pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Normals", "N", "Normals", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "C", "Colors", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {

                var p = new List<Point3d>();
                var n = new List<Vector3d>();
                var c = new List<System.Drawing.Color>();

                DA.GetDataList(0, p);
                DA.GetDataList(1, n);
                DA.GetDataList(2, c);

                PointCloud cloud = new PointCloud();



                if (p.Count == n.Count && p.Count == c.Count)
                    cloud.AddRange(p, n, c);
                else if (p.Count == n.Count)
                    cloud.AddRange(p, n);
                else if (p.Count == c.Count)
                    cloud.AddRange(p, c);
                else if (p.Count > 0)
                    cloud.AddRange(p);
                else
                    return;

                var cgh = new GH_Cloud(cloud);
                DA.SetData(0, cgh);
            } catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-44c6-8fd1-edf34cd00ba4"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateCloud;
    }
}