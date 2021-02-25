using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudExplode : GH_Component {

        public CloudExplode()
  : base("CloudExplode", "Explode",
      "CloudExplode",
      "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Points", GH_ParamAccess.list);
            pManager.AddVectorParameter("Normals", "N", "Normals", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "C", "Colors", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);

                DA.SetDataList(0, cgh.Value.GetPoints());
                DA.SetDataList(1, cgh.Value.GetNormals());
                DA.SetDataList(2, cgh.Value.GetColors());
            } catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd4-edf34cd00ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.ExplodeCloud;
    }
}