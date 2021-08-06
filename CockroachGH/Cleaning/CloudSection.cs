using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CockroachGH {
    public class CloudSection : GH_Component {

        public CloudSection()
  : base("CloudSection", "CloudSection",
         "Intersect a PointCloud with one or multiple Planes using a defined Tolerance, returning the section(s) as a new PointCloud.",
         "Cockroach", "Crop") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud to the section(s) for.", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Planes", "P", "The section Planes", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Searches closest points to planes, if value is negative points are projected.", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The section(s) of the PointCloud", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            // Input
            var cgh = new GH_Cloud();
            DA.GetData(0, ref cgh);

            List<Plane> planes = new List<Plane>(); ;
            DA.GetDataList(1, planes);

            double distance = 1;
            DA.GetData(2, ref distance);


            DA.SetData(0, new GH_Cloud(SectionCloud(cgh.Value,planes,Math.Abs(distance), distance < 0)));
        }

        public PointCloud SectionCloud(PointCloud cloud, List<Plane> planes,  double tol = 0.001, bool project = false) {

            bool[] flags = new bool[cloud.Count];
            int[] cID = new int[cloud.Count];

            int count = 1;
            foreach (Plane plane in planes) {

                   double[] eq = plane.GetPlaneEquation();
                   double denom = 1 / Math.Sqrt(eq[0] * eq[0] + eq[1] * eq[1] + eq[2] * eq[2]);


                System.Threading.Tasks.Parallel.For(0, cloud.Count, i =>
                {
                    if (flags[i]) return;

                    if (Math.Abs(FastPlaneToPt(denom, eq[0], eq[1], eq[2], eq[3], cloud[i].Location)) <= tol)
                    {
                        flags[i] = true;
                        cID[i] = count;
                    }
                });

                count++;
            }

            int cc = 0;
            List<int> idList = new List<int>();
            for (int i = 0; i < cloud.Count; i++)
            {
                //bool f = inverse ? !flags[i] : flags[i];
                if (flags[i])
                {
                    idList.Add(i);
                    cc++;
                }
            }

            int[] idArray = idList.ToArray();
            Point3d[] points = new Point3d[cc];
            Vector3d[] normals = new Vector3d[cc];
            Color[] colors = new Color[cc];
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
            //    //bool f = inverse ? !flags[i] : flags[i];
            //    bool f =  flags[i];
            //    if (f) {
            //        if (cloud.ContainsNormals) {
            //            croppedCloud.Add(cloud[i].Location, cloud[i].Normal, cloud[i].Color);
            //        } else {
            //            croppedCloud.Add(cloud[i].Location, cloud[i].Color);
            //        }
            //    }
            //}


            if (project) {
               // if (planes.Count == 1) {
                    croppedCloud.Transform(Rhino.Geometry.Transform.PlanarProjection(planes[0]));
                //} else {

                //    for (int i = 0; i < cloud.Count; i++) {

                //    }
                }
                return croppedCloud;

            }



        //public PointCloud Section(PointCloud cloud, Plane y, bool project = true, double tol = 0.5) {
        //    double[] eq = y.GetPlaneEquation();
        //    double denom = 1 / Math.Sqrt(eq[0] * eq[0] + eq[1] * eq[1] + eq[2] * eq[2]);
        //    PointCloud cloudSection = new PointCloud();
        //    foreach (PointCloudItem it in cloud) {
        //        if (Math.Abs(FastPlaneToPt(denom, eq[0], eq[1], eq[2], eq[3], it.Location)) <= tol) {
        //            if (cloud.ContainsNormals) {
        //                cloudSection.Add(it.Location, it.Normal, it.Color);
        //            } else {
        //                cloudSection.Add(it.Location, it.Color);
        //            }

        //        }
        //    }
        //    if (project)
        //        cloudSection.Transform(Rhino.Geometry.Transform.PlanarProjection(y));
        //    return cloudSection;
        //}

        //public PointCloud SectionParallel(PointCloud cloud, Plane y, bool project = true, double tol = 0.5) {
        //    double[] eq = y.GetPlaneEquation();
        //    double denom = 1 / Math.Sqrt(eq[0] * eq[0] + eq[1] * eq[1] + eq[2] * eq[2]);
        //    PointCloud cloudSection = new PointCloud();


        //    bool[] f = new bool[cloud.Count];

        //    System.Threading.Tasks.Parallel.For(0, cloud.Count, i => {
        //        if (Math.Abs(FastPlaneToPt(denom, eq[0], eq[1], eq[2], eq[3], cloud[i].Location)) <= tol) {
        //            f[i] = true;
        //        }

        //    });


        //    for (int i = 0; i < f.Length; i++) {
        //        if (f[i])
        //            if (cloud.ContainsNormals) {
        //                cloudSection.Add(cloud[i].Location, cloud[i].Normal, cloud[i].Color);
        //            } else {
        //                cloudSection.Add(cloud[i].Location, cloud[i].Color);
        //            }
        //    }


        //    if (project)
        //        cloudSection.Transform(Rhino.Geometry.Transform.PlanarProjection(y));
        //    return cloudSection;
        //}
        public double FastPlaneToPt(double Denom, double a, double b, double c, double d, Point3d Pt) {
            return ((a * Pt.X + b * Pt.Y + c * Pt.Z + d) * Denom);
        }


        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd6-edf44cd51ba1"); }
        }

        protected override Bitmap Icon => Properties.Resources.PlaneClip;
    }
}