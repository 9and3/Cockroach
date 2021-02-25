using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudPreview : GH_Component {

        public CloudPreview()
  : base("CloudPreview", "CloudPreview",
      "CloudPreview",
      "Cockroach", "Cloud") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {

            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddColourParameter("Colors", "C", "Colors", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Thickness", "T", "Thickness of PointDisplay", GH_ParamAccess.item,2);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager.HideParameter(0);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {


            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {


                GH_Cloud cloud = new GH_Cloud();
                DA.GetData(0, ref cloud);

                var color = System.Drawing.Color.Red;
                DA.GetData(1, ref color);

                int t = 2;
                DA.GetData(2, ref t);
                StaticParameters.DisplayRadius = t;

                var points  = cloud.Value.GetPoints();
                //var normals = cloud.Value.GetNormals();
                var colors = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat(color,cloud.Value.Count));

                //System.Threading.Tasks.Parallel.For(0, cloud.Value.Count, i => {
                //    colors[i] = color;

                //});
     
                PointCloud c = new PointCloud();
                c.AddRange(points, colors);
     
        
                DA.SetData(0, new GH_Cloud(c));







            } catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-44c6-1fd1-edf34cd00ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources. Color;
    }
}