using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudClusterCilantro : GH_Component {

        public CloudClusterCilantro()
  : base("CloudClusterCilantro", "CloudClusterCilantro",
      "CloudClusterCilantro",
      "Cockroach", "Cluster") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddNumberParameter("VoxelSizeSearch", "V", "VoxelSizeSearch ", GH_ParamAccess.item, 0.1);
            pManager.AddNumberParameter("NormalThresholdDegree", "N", "NormalThresholdDegree ", GH_ParamAccess.item, 2.0);
            pManager.AddIntegerParameter("MinClusterSize", "S", "MinClusterSize", GH_ParamAccess.item,100);
            pManager.AddBooleanParameter("ColorPointCloud", "C", "ColorPointCloud", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Split", "S", "Split", GH_ParamAccess.item, false);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;


            //double voxelSizeSearch = 0.1, double normalThresholdDegree = 2.0, int minClusterSize = 100, bool colorPointCloud = true, bool split = false
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

                double voxelSizeSearch = 0.1;
                double normalThresholdDegree = 2.0;
                int minClusterSize = 100;
                bool colorPointCloud = true;
                bool split = false;

                DA.GetData(1, ref voxelSizeSearch);
                DA.GetData(2, ref normalThresholdDegree);
                DA.GetData(3, ref minClusterSize);
                DA.GetData(4, ref colorPointCloud);
                DA.GetData(5, ref split);


                var c =  PInvokeCSharp.TestCilantro.GetClusterConnectedComponentRadius(cgh.Value, voxelSizeSearch, normalThresholdDegree, minClusterSize, colorPointCloud, split);
                //this.Message = c.Count.ToString();

                //GH_Cloud[] clouds = new GH_Cloud[1] { new GH_Cloud(c) };
                GH_Cloud[] clouds = new GH_Cloud[c.Length];
                for (int i = 0; i < c.Length; i++) {
                    clouds[i] = new GH_Cloud(c[i]);
                }

                DA.SetDataList(0, clouds);
            } catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf5ed-7834-48c6-8fd1-edf44cd88ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.ClusterNormals;
    }
}