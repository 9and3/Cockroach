using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class CloudBoxCrop : GH_Component {

        public CloudBoxCrop()
  : base("CloudBoxCrop", "CloudBoxCrop",
         "Crop a PointCloud with one or multiple Boxes, returning either the inside or the outside of the Box(es) as a new PointCloud.",
         "Cockroach", "Crop") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud to crop.", GH_ParamAccess.item);
            pManager.AddBrepParameter("Boxes", "B", "The Box(es) to crop PointCloud with.",GH_ParamAccess.list);
            pManager.AddBooleanParameter("Inverse","I","If set to True, will get the outside part of the Box.", GH_ParamAccess.item, false);
            pManager[2].Optional = true;
          }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The cropped PointCloud.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            GH_Cloud cgh = new GH_Cloud();
            DA.GetData(0, ref cgh);

            List<Brep> boxes = new List<Brep>();
            DA.GetDataList(1, boxes);

            bool inverse = false;
            DA.GetData(2, ref inverse);

            PointCloud c = CropBox(cgh.Value, boxes, inverse, true);

            DA.SetData(0, new GH_Cloud(c));
        }

        public PointCloud CropBox(PointCloud cloud, List<Brep> boxes, bool inverse = false, bool parallel = false) {


            bool[] flags = new bool[cloud.Count];

            foreach (var brep in boxes)
            {
                brep.Surfaces[0].TryGetPlane(out Plane plane);
                Box box = new Box(plane, brep);

                Transform xform = Transform.PlaneToPlane(box.Plane, Plane.WorldXY);
                Transform xformI = Transform.PlaneToPlane(Plane.WorldXY, box.Plane);

                PointCloud cloudCopy = new PointCloud(cloud);

                Box b = new Box(box.Plane, new Point3d[] { box.PointAt(0, 0, 0), box.PointAt(1, 1, 1) });
             
                cloudCopy.Transform(xform);
                b.Transform(xform);
                Point3d Min = b.PointAt(0, 0, 0);
                Point3d Max = b.PointAt(1, 1, 1);
                //Rhino.RhinoDoc.ActiveDoc.Objects.AddBox(b);

                System.Threading.Tasks.Parallel.For(0, cloudCopy.Count, i =>
                {
                    if (flags[i]) return;
                    var p = cloudCopy[i].Location;
                    bool flag = (Min.X < p.X) && (Max.X > p.X) && (Min.Y < p.Y) && (Max.Y > p.Y) &&  (Min.Z < p.Z) && (Max.Z > p.Z);
                    if (flag) flags[i] = true;
                });
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

        //public void CropCloud(PointCloud cloud, ref bool[] flags, Box box, bool inverse = false, bool parallel = false) {

        //    Transform xform = Transform.PlaneToPlane(box.Plane, Plane.WorldXY);
        //    Transform xformI = Transform.PlaneToPlane(Plane.WorldXY, box.Plane);

        //    PointCloud cloudCopy = new PointCloud(cloud);

        //    Box b = new Box(box.Plane, new Point3d[] { box.PointAt(0, 0, 0), box.PointAt(1, 1, 1) });
        //    cloudCopy.Transform(xform);
        //    b.Transform(xform);

        //    if (!parallel)
        //        CropCloud(cloudCopy, ref flags, b.PointAt(0, 0, 0), b.PointAt(1, 1, 1), inverse);
        //    else
        //        CropCloudParallel(cloudCopy, ref  flags, b.PointAt(0, 0, 0), b.PointAt(1, 1, 1), inverse);
        //    //cloudCopy.Transform(xformI);
        //   // return cloudCopy;
        //}

        //public bool[] CropCloudParallel(PointCloud cloud, ref bool[] flags, Point3d Min, Point3d Max, bool inverse = false) {



        //    PointCloud croppedCloud = new PointCloud();
        //    bool[] f = new bool[cloud.Count];

        //    System.Threading.Tasks.Parallel.For(0, cloud.Count, i => {

        //        if (flags[i]) return;
                
        //        var p = cloud[i].Location;



        //        bool flag = (Min.X < p.X) && (Max.X > p.X) &&
        //            (Min.Y < p.Y) && (Max.Y > p.Y) &&
        //            (Min.Z < p.Z) && (Max.Z > p.Z);

        //        if (inverse)
        //            flag = !flag;

        //        if (flag) {
        //            f[i] = true;
        //        }

        //    });

        //    //for(int i = 0; i< f.Length; i++) {
        //    //    if(f[i])
        //    //        if (cloud.ContainsNormals) {
        //    //            croppedCloud.Add(cloud[i].Location, cloud[i].Normal, cloud[i].Color);
        //    //        } else {
        //    //            croppedCloud.Add(cloud[i].Location, cloud[i].Color);
        //    //        }
        //    //}

        //    return f;

        //}

        //public bool[] CropCloud(PointCloud cloud,  ref bool[] flags, Point3d Min, Point3d Max,bool inverse = false) {


          
        //    //PointCloud croppedCloud = new PointCloud();
        //    bool[] flags = new bool[cloud.Count];
        //    int i = 0;
        //    foreach (PointCloudItem it in cloud) {
        //        Point3d p = it.Location;

        //        bool flag = (Min.X < p.X) && (Max.X > p.X) &&
        //            (Min.Y < p.Y) && (Max.Y > p.Y) &&
        //            (Min.Z < p.Z) && (Max.Z > p.Z);

        //        flags[i] = flag;

        //        //if (inverse)
        //        //    flag = !flag;

        //        //if (flag) {
        //        //    if (cloud.ContainsNormals) {
        //        //        croppedCloud.Add(it.Location, it.Normal, it.Color);
        //        //    } else {
        //        //        croppedCloud.Add(it.Location, it.Color);
        //        //    }

        //        //}
        //        i++;
        //    }
        //    return flags;
        //    //return croppedCloud;
        //}

        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c8-8fd6-edf88cd51ba1"); }
        }

        protected override Bitmap Icon => Properties.Resources.BoundingBox;
    }
}