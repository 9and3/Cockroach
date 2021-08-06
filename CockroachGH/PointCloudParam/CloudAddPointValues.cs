using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudAddPointValues : GH_Component {

        public CloudAddPointValues()
  : base("CloudAddPointValues", "AddPointValues",
         "Add extra values (i.e. for intensity) to the points of an existing PointCloud.",
         "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud to add extra values to.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Values", "V", "The extra values to add to the points of the PointCloud (i.e. intensity). If the length of supplied values is less than the number of points in the cloud, it will be filled with zeros. If it exceeds the number of points, it will be truncated.", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "The PointCloud with extra values added to its points.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            try
            {
                // retrieve PointCloud from input data
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);

                var v = new List<double>();
                DA.GetDataList(1, v);

                // create new PointCloud
                PointCloud cloud = new PointCloud();

                // get points, initialize numbers and colors
                var p = cgh.Value.GetPoints();
                Vector3d[] n;
                System.Drawing.Color[] c;

                // check and sanitize normals if necessary
                n = cgh.Value.GetNormals();
                if (n.Length == 0)
                {
                    n = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat(Vector3d.Unset, p.Length));
                }

                // check and sanitize colors if necessary
                c = cgh.Value.GetColors();
                if (c.Length == 0)
                {
                    c = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat(System.Drawing.Color.Black, p.Length));
                }

                // check length of list of input values
                if (p.Length == v.Count)
                {
                    // add everying to new cloud
                    cloud.AddRange(p, n, c, v);
                }
                else
                {
                    // truncate / fill values list
                    double[] arrv = new double[p.Length];
                    System.Threading.Tasks.Parallel.For(0, arrv.Length, i =>
                    {
                        if (i < v.Count) arrv[i] = v[i];
                    });
                    // add everying to new cloud
                    cloud.AddRange(p, n, c, arrv);
                }

                // clear normals if input cloud did not have them 
                if (!cgh.Value.ContainsNormals)
                {
                    cloud.ClearNormals();
                }
                // clear colors if input cloud did not have them
                if (!cgh.Value.ContainsColors)
                {
                    cloud.ClearNormals();
                }

                // prepare data for output
                DA.SetData(0, new GH_Cloud(cloud));
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("88037ed0-5dda-483b-be15-e05ef45dcaf0"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Cloud;
    }
}