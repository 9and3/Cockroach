using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudSwap : GH_Component {

        public CloudSwap()
  : base("CloudSwap", "CloudSwap",
      "CloudSwap",
      "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {

            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddTextParameter("Mask", "M", "Mask, write 3 letters P - points, N - normals, C- colors, i.e. PCN, CPN",GH_ParamAccess.item, "CPN");
            pManager[1].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            GH_Cloud cloud = new GH_Cloud();
            DA.GetData(0, ref cloud);

            string txt = "CPN";
            DA.GetData(1, ref txt);

            char[] characters = txt.ToCharArray();

            if (characters == null) return;
            if (characters.Length != 3) return;

            Point3d[] points = new Point3d[cloud.Value.Count];
            Vector3d[] normals = new Vector3d[cloud.Value.Count];
            System.Drawing.Color[] colors = new System.Drawing.Color[cloud.Value.Count];

            //PointCloud swapCloud = new PointCloud();

            System.Threading.Tasks.Parallel.For(0, cloud.Value.Count, i => {
                //for (int i = 0; i < cloud.Value.Count; i++) {

                //swapCloud.AppendNew();

                //Points

                switch (characters[0]) {

                    case ('p'):
                    case ('P'):
                        points[i] = cloud.Value[i].Location;

                        break;

                    case ('c'):
                    case ('C'):
                        var color = cloud.Value[i].Color;
                        points[i] = new Point3d(color.R, color.G, color.B);
                        break;

                    case ('n'):
                    case ('N'):
                        points[i] = new Point3d(cloud.Value[i].Normal);
                        break;

                }

                switch (characters[1]) {

                    case ('p'):
                    case ('P'):
                        normals[i] = new Vector3d(cloud.Value[i].Location);

                        break;

                    case ('c'):
                    case ('C'):
                        var color = cloud.Value[i].Color;
                        normals[i] = new Vector3d(color.R, color.G, color.B);
                        break;

                    case ('n'):
                    case ('N'):
                        normals[i] = new Vector3d(cloud.Value[i].Normal);
                        break;

                }

                switch (characters[2]) {

                    case ('p'):
                    case ('P'):
                        colors[i] = cloud.Value[i].Color;

                        break;

                    case ('c'):
                    case ('C'):

                        colors[i] = cloud.Value[i].Color;
                        break;

                    case ('n'):
                    case ('N'):
                        colors[i] = cloud.Value[i].Color;
                        break;

                }

           // }
            });

            PointCloud c = new PointCloud();
            c.AddRange(points,normals,colors);

            DA.SetData(0,new GH_Cloud(c));
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-44c6-8fd1-edf34cd00ba1"); }
        }

        protected override System.Drawing.Bitmap Icon =>Properties.Resources.Swap;
    }
}