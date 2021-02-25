using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudSOR : GH_Component {

        public CloudSOR()
  : base("CloudSOR", "CloudSOR",
      "CloudSOR",
      "Cockroach", "Cluster") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

            pManager.AddNumberParameter("Radius", "R", "Radius, 0 default ", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Percent", "P", "Percent ", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Neighbours", "N", "Neighbours ", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Type", "T", "Type ", GH_ParamAccess.item, 0);


            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

      

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);
      

                double radius = 0;
                double percent = 0;
                double neighbours = 0;
                double type = 0;

                DA.GetData(1, ref radius);
                DA.GetData(2, ref percent);
                DA.GetData(3, ref neighbours);
                DA.GetData(4, ref type);


                var c =  PInvokeCSharp.TestCGAL.SOR(cgh.Value, radius,percent,(int)neighbours, (int)type);
                DA.SetData(0, new GH_Cloud(c));

            }catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("44aaf5ed-7834-48c6-4fd1-edf44cd55ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.SOR;
    }
}