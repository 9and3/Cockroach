using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class CloudDownsampleVoxel : GH_Component {

        public CloudDownsampleVoxel()
  : base("DownsampleVoxel", "DownsampleVoxel",
      "DownsampleVoxel",
      "Cockroach", "Sample") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddNumberParameter("VoxeSize", "S", "VoxelSize",GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            GH_Cloud cgh = new GH_Cloud();
            DA.GetData(0, ref cgh);

             double voxelsize = 1;
            DA.GetData(1, ref voxelsize);
           // n = Math.Min(cgh.Value.Count, Math.Max(0,n));


            DA.SetData(0, new GH_Cloud(PInvokeCSharp.TestOpen3D.Downsample(cgh.Value,voxelsize)));


        }

        public PointCloud DownsampleUniform(PointCloud cloud, int numberOfPoints) {

            int nOfPoints = numberOfPoints;
            int b = (int)(cloud.Count * (1.0 / numberOfPoints * 1.0));
            int nth = Math.Max(1, b);

            PointCloud cloudNew = new PointCloud();



            int count = 0;
            for (int i = 0; i < cloud.Count; i += nth) {
                count++;
            }

            Point3d[] points = new Point3d[count];
            Vector3d[] normals = new Vector3d[count];
            Color[] colors = new Color[count];


            System.Threading.Tasks.Parallel.For(0, cloud.Count /  nth, k => {

                int ii = nth * k;
                var p = cloud[(int)ii];
                points[k] = p.Location;
                normals[k] = p.Normal;
                colors[k] = p.Color;

            });

            cloudNew.AddRange(points, normals, colors);


            return cloudNew;
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-44c6-8fd6-edf44cd51ba4"); }
        }
       
        protected override Bitmap Icon => Properties.Resources.DownsampleVoxel;
    }
}