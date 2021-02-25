using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudCluster : GH_Component {

        public CloudCluster()
  : base("CloudCluster", "CloudCluster",
      "CloudCluster",
      "Cockroach", "Cluster") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "RefPointCloud", "R", "PointCloud as a reference", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius, 0 default ", GH_ParamAccess.item,0);
            //pManager.AddIntegerParameter("Iter", "I", "Iterations", GH_ParamAccess.item,30);
            pManager.AddIntegerParameter("Neigbhours", "N", "Neighbours for reorientation, 0 default", GH_ParamAccess.item,0);
            pManager.AddBooleanParameter("Split", "S", "Split", GH_ParamAccess.item, false);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.list);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);
                var cghRef = new GH_Cloud();
               if(! DA.GetData(1, ref cghRef)) {
                    cghRef = cgh;
                }

                double r = 0.1;
               // int it = 30;
                int n = 10;
                bool split = false;
                DA.GetData(2, ref r);
                //DA.GetData(2, ref it);
                DA.GetData(3, ref n);
                DA.GetData(4, ref split);


                var c =  PInvokeCSharp.TestCGAL.Clustering(cgh.Value, cghRef.Value, r,n, split);
                //this.Message = c.Count.ToString();

                GH_Cloud[] clouds = new GH_Cloud[c.Length];
                for (int i = 0; i < c.Length; i++) {
                    clouds[i] = new GH_Cloud(c[i]);
                }

                DA.SetDataList(0, clouds);
            }catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf5ed-7834-48c6-8fd1-edf34cd55ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Cluster2;
    }
}