using Clipper642;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class CloudPolygonCrop : GH_Component {

        public CloudPolygonCrop()
  : base("CloudPolygonCrop", "CloudPolygonCrop",
         "Crop a PointCloud with one or multiple closed Polylines, returning either the inside or the outside of the Polyline(s) as a new PointCloud.",
         "Cockroach", "Crop") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud to crop.", GH_ParamAccess.item);
            pManager.AddCurveParameter("Polylines", "P", "Closed Polylines to crop the PointCloud with.",GH_ParamAccess.list);
            pManager.AddBooleanParameter("Inverse","I", "If set to True, will get the outside part of the Polygon(s).", GH_ParamAccess.item, false);
            pManager[2].Optional = true;
          }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The cropped PointCloud.", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            var cgh = new GH_Cloud();
            DA.GetData(0, ref cgh);

            List<Curve> curves = new List<Curve>(); 
            DA.GetDataList(1,  curves);

           bool inverse = false;
            DA.GetData(2, ref inverse);


   

            DA.SetData(0, new GH_Cloud(CropCloud(cgh.Value, curves, inverse)));


        }
        public PointCloud CropCloud(PointCloud cloud, List<Curve> curves, bool inverse = false, double scale = 1e10, double tol = 0.001) {

            bool[] flags = new bool[cloud.Count];

       

            foreach (Curve crv in curves) {

                bool f = crv.TryGetPolyline(out Polyline polyline_); if (!f) continue;


                PointCloud cloud_ = new PointCloud(cloud);


                Polyline polyline = new Polyline(polyline_);
                Plane plane;
                Plane.FitPlaneToPoints(polyline, out plane);
                Transform xform = Transform.PlaneToPlane(plane, Plane.WorldXY);

                polyline.Transform(xform);
                cloud_.Transform(xform);
                var input = Geometry.RhinClip.PolylineToIntPoint(polyline, scale);

                System.Threading.Tasks.Parallel.For(0, cloud.Count, i =>
                {
                    if (flags[i]) return;

                    Point3d p = new Point3d(cloud_[i].Location);
                    var inputP = new IntPoint(p.X * scale, p.Y * scale);

                    bool flag = Clipper642.Clipper.PointInPolygon(inputP, input) != 0;//;: Clipper642.Clipper.PointInPolygon(inputP, input) == 0;
                    if (flag) flags[i] = true;
                });



            }

            int count = 0;
            List<int> idList = new List<int>();
            //System.Threading.Tasks.Parallel.For(0, cloud.Count, i => {
            for (int i = 0; i < cloud.Count; i++)
            {
                bool f = inverse ? !flags[i] : flags[i];
                if (f)
                {
                    idList.Add(i);
                    count++;
                }
            }
            //});

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

            //PointCloud croppedCloud = new PointCloud();
            //for (int i = 0; i < cloud.Count; i++) {
            //    bool f = inverse ? !flags[i] : flags[i];
            //    if (f) {
            //        if (cloud.ContainsNormals) {
            //            croppedCloud.Add(cloud[i].Location, cloud[i].Normal, cloud[i].Color);
            //        } else {
            //            croppedCloud.Add(cloud[i].Location, cloud[i].Color);
            //        }
            //    }
            //}

            return croppedCloud;
        }

 
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7534-48c8-8fd6-edf88cd41ba1"); }
        }

        protected override Bitmap Icon => Properties.Resources.PolygonCrop;//Properties.Resources.BoundingBox
    }
}