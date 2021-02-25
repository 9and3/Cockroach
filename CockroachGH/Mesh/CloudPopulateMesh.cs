using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class PopulateMesh : GH_Component {

        public PopulateMesh()
  : base("PopulateMesh", "PopulateMesh",
      "PopulateMesh",
      "Cockroach", "Mesh") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddMeshParameter( "Mesh", "M", "Mesh", GH_ParamAccess.item);
            pManager.AddIntegerParameter("N", "N", "Number of Points", GH_ParamAccess.item,100);
            pManager.AddIntegerParameter("Type", "T", "Type - 0 triangle normals false, 1 - triangle normals true", GH_ParamAccess.item,0);

            pManager[1].Optional = true;
            pManager[2].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);


        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                Mesh mesh = new Mesh();
                DA.GetData(0, ref mesh);

                int n = 100;  int type = 0;  
                DA.GetData(1, ref n);
                DA.GetData(2, ref type);


                DA.SetData(0, PInvokeCSharp.TestOpen3D.PopulateMesh(mesh, type,n));


                
            } catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd1-edf44cd15ba4"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.PopulateMesh;
    }
}