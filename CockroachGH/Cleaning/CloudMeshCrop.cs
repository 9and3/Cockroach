using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class CloudMeshCrop : GH_Component {

        public CloudMeshCrop()
  : base("CloudMeshCrop", "CloudMeshCrop",
         "Crop a PointCloud with one or multiple Meshes, returning either the inside or the outside of the Mesh(es) as a new PointCloud.",
         "Cockroach", "Crop") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud to crop.", GH_ParamAccess.item);
            pManager.AddMeshParameter("Meshes", "M", "The Mesh(es) to crop the PointCloud with.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Inverse","I", "If set to True, will get the outside part of the Mesh.", GH_ParamAccess.item, false);
            pManager[2].Optional = true;
          }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The cropped PointCloud.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            var cgh = new GH_Cloud();
            DA.GetData(0, ref cgh);

            var  meshes = new List<Mesh>(); 
            DA.GetDataList(1, meshes);

           bool inverse = false;
            DA.GetData(2, ref inverse);

            PointCloud c = CropCloud(cgh.Value, meshes,inverse);

            DA.SetData(0, new GH_Cloud(c));
        }


        public PointCloud CropCloud(PointCloud cloud, List<Mesh> M, bool inverse = false) {

            RTree rTree = Rhino.Geometry.RTree.CreatePointCloudTree(cloud);
            
            bool[] flags = new bool[cloud.Count];

            for (int i = 0; i < M.Count; i++) {
                Mesh m = M[i];

                BoundingBox bBox = m.GetBoundingBox(false);
                var boxSearchData = new BoxSearchData();
                rTree.Search(bBox, BoundingBoxCallback, boxSearchData);

                foreach (int id in boxSearchData.Ids)
                {
                    if (m.IsPointInside(cloud[id].Location, 0, true))
                    {
                        flags[id] = true;
                    }
                }
            }


            int count = 0;
            List<int> idList = new List<int>();
            for (int i = 0; i < cloud.Count; i++)
            {
                bool f = inverse ? !flags[i] : flags[i];
                if (f)
                {
                    idList.Add(i);
                    count++;
                }
            }

            int[] idArray = idList.ToArray();
            Point3d[] points = new Point3d[count];
            Vector3d[] normals = new Vector3d[count];
            Color[] colors = new Color[count];
            double[] pvalues = new double[count];

            System.Threading.Tasks.Parallel.For(0, idArray.Length, i =>
            {
                int id = idArray[i];
                var p = cloud[(int)id];
                points[i] = p.Location;
                normals[i] = p.Normal;
                colors[i] = p.Color;
                pvalues[i] = p.PointValue;
            });

            PointCloud croppedCloud = new PointCloud();
            if (cloud.ContainsPointValues)
            {
                croppedCloud.AddRange(points, normals, colors, pvalues);
            }
            else
            {
                croppedCloud.AddRange(points, normals, colors);
            }

            //for (int i = 0; i < f.Length; i++) {
            //    bool flag = inverse ? !f[i] : f[i];
            //    if (flag)
            //        if (cloud.ContainsNormals) {
            //            c.Add(cloud[i].Location, cloud[i].Normal, cloud[i].Color);
            //        } else {
            //            c.Add(cloud[i].Location, cloud[i].Color);
            //        }
            //}

            return croppedCloud;

        }


        public void BoundingBoxCallback(object sender, RTreeEventArgs e) {
            var boxSearchData = e.Tag as BoxSearchData;
            boxSearchData.Ids.Add(e.Id);
        }


        public class BoxSearchData {
            public BoxSearchData() {
                Ids = new List<int>();
            }

            public List<int> Ids { get; set; }
        }

        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-41c8-8fd6-edf88cd44ba1"); }
        }

        protected override Bitmap Icon => Properties.Resources.MeshCrop;
    }
}