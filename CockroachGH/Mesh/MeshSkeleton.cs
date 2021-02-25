using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace CockroachGH {
    public class MeshSkeleton : GH_Component{
        /// <summary>
        /// Initializes a new instance of the MeshEdgeAnalysis class.
        /// </summary>
        public MeshSkeleton()
          : base("MeshSkeleton", "MeshSkeleton",
              "MeshSkeleton",
               "Cockroach", "Mesh") {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddMeshParameter("Mesh","M","Mesh",GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            pManager.AddCurveParameter("Polylines", "P","Polylines",GH_ParamAccess.list);
           
        }


        protected override void SolveInstance(IGH_DataAccess DA) {
            Mesh mesh = new Mesh();
            DA.GetData(0, ref mesh);
            var plines = PInvokeCSharp.TestCGAL.CreateMeshSkeleton(mesh);
            DA.SetDataList(0, plines);

            var pts = new List<Point3d>(8);
            foreach (var item in plines) {
                pts.Add(item[0]);
                pts.Add(item[item.Count-1]);
            }
           // this.PreparePreview(mesh, DA.Iteration,System.Linq.Enumerable.ToList( plines),false,pts);
        }


        protected override System.Drawing.Bitmap Icon {
            get {

                return Properties.Resources.MeshSkeleton;
            }
        }

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        public override Guid ComponentGuid {
            get { return new Guid("644d1829-f91b-4ca7-afdf-7131751e94a6"); }
        }
    }
}