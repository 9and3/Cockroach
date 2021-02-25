using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudToGuid : GH_Component {

        public CloudToGuid()
  : base("GuidCloud", "GuidCloud",
      "GuidCloud",
      "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            IGH_Param geometryAsGuid = new Grasshopper.Kernel.Parameters.Param_Guid();
            pManager.AddParameter(geometryAsGuid, "Guid", "G", "Guid as PointCloud from Rhino", GH_ParamAccess.item);
            //pManager.AddMeshParameter("M", "M", "M",GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new PointCloudGHParam(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
  

            GH_Guid g = null;
            DA.GetData(0, ref g);
            GeometryBase rhino_obj = Rhino.RhinoDoc.ActiveDoc.Objects.Find(g.Value).Geometry;
            PointCloud c = rhino_obj as PointCloud;

            DA.SetData(0, new PointCloudGH(c) );

            base.Message = c.Count.ToString();




        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd6-edf38cd62ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Guid;
    }
}