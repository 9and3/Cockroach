using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class CloudMerge : GH_Component {

        public CloudMerge()
  : base("CloudMerge", "CloudMerge",
      "CloudMerge",
      "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            var cgh = new List<GH_Cloud>();
            DA.GetDataList(0,  cgh);

            PointCloud cloudMerged = new PointCloud();
            foreach (var cloud in cgh) {
                cloudMerged.Merge(cloud.Value);
            }

            DA.SetData(0, new GH_Cloud(cloudMerged));


        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd6-edf34cd99ba1"); }
        }

        protected override Bitmap Icon => Properties.Resources.Merge;
    }
}