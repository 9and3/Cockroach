using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudRANSAC : GH_Component {

        public CloudRANSAC()
  : base("CloudRANSAC", "CloudRANSAC",
      "CloudRANSAC",
      "Cockroach", "Cluster") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

            pManager.AddNumberParameter("Radius", "R", "Radius, 0 default ", GH_ParamAccess.item,0.1);
            pManager.AddNumberParameter("Neigbhours", "N", "Neighbours for reorientation, 0 default", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("Iterations", "I", "Iterations", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("InliersOrOutliers", "I/O", "InliersOrOutliers", GH_ParamAccess.item, true);
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
   

                double distance = 0.1;
                double neighbours = 10;
                double iterations = 10;
    bool InliersOrOutliers = true; 
                DA.GetData(1, ref distance);
                DA.GetData(2, ref neighbours);
                DA.GetData(3, ref iterations);
                DA.GetData(4, ref InliersOrOutliers);


                var c =  PInvokeCSharp.TestOpen3D.RANSACPlane(cgh.Value, distance, neighbours, iterations, InliersOrOutliers);
  

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
            get { return new Guid("98aaf5ed-7831-48c6-8fd1-edf31cd55ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.RANSAC;
    }
}