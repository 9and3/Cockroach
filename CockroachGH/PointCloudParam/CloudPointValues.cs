using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudPointValues : GH_Component {

        public CloudPointValues()
  : base("CloudPointValues", "PointValues",
         "Retrieve the extra PointValues attached to the points of a PointCloud.",
         "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud to retrieve the extra PointValues from.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddNumberParameter("Values", "V", "The extra PointValues attached to the points of the PointCloud (i.e. for intensity).", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);

                DA.SetDataList(0, cgh.Value.GetPointValues());
            }
            catch(Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("686a4bfe-5c53-401d-be82-ecc0dd99e54a"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Cloud;
    }
}