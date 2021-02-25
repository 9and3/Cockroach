using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class GuidToCloud : GH_Component {

        public GuidToCloud()
  : base("DownsampleOpen3D", "DownsampleOpen3D",
      "DownsampleOpen3D",
      "Cockroach", "Sample") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddIntegerParameter("N", "N", "Number of Points",GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            GH_Cloud cgh = new GH_Cloud();
            DA.GetData(0, ref cgh);

            int n = 50;
            DA.GetData(1, ref n);
            n = Math.Min(cgh.Value.Count, Math.Max(0,n));

            var c = PInvokeCSharp.TestOpen3D.Downsample(cgh.Value, n);
            //var c = PInvokeCSharp.TestOpen3D.DownsampleRhino(cgh.Value, n);
            //this.Message = c.Count.ToString();

            DA.SetData(0, new GH_Cloud(c));


        }

        public PointCloud DownsampleUniform(PointCloud cloud, int numberOfPoints) {

            int nOfPoints = numberOfPoints;
            int b = (int)(cloud.Count * (1.0 / numberOfPoints * 1.0));
            int nth = Math.Max(1, b);

            PointCloud cloudNew = new PointCloud();

            int count = 0;
            for(int i = 0; i<cloud.Count; i += nth) {
                cloudNew.Add(cloud[i].Location);
                //cloudNew.AppendNew();
                //cloudNew[count].Location = cloud[i].Location;
                //cloudNew[count].Normal = cloud[i].Normal;
                //cloudNew[count].Color = cloud[i].Color;
                count++;
            }


            return cloudNew;
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd6-edf34cd50ba1"); }
        }
       
        protected override Bitmap Icon => Properties.Resources.Downsample;
    }
}