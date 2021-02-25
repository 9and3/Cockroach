using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class MeshRepair : GH_Component {

        public MeshRepair()
  : base("MeshRepair", "MeshRepair",
      "MeshRepair",
      "Cockroach", "Mesh") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {

            pManager.AddMeshParameter( "Mesh", "M", "Mesh", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);


        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            Mesh mesh3D = new Mesh();
            DA.GetData(0, ref mesh3D);

            mesh3D.Vertices.CombineIdentical(true, true);
            mesh3D.Vertices.CullUnused();
            mesh3D.Weld(3.14159265358979);
            //mesh3D.FaceNormals.ComputeFaceNormals();
            //mesh3D.Normals.ComputeNormals();


            Mesh mesh = new Mesh();
            mesh.Vertices.AddVertices(mesh3D.Vertices);
            if (mesh3D.Vertices.Count == mesh3D.VertexColors.Count)
                foreach (var color in mesh3D.VertexColors)
                    mesh.VertexColors.Add(color);



            int num = checked(mesh3D.Vertices.Count - 1);


            foreach (var p in mesh3D.Faces)
                if (p.IsValid())
                    if (!(p.A > num || p.B > num || p.C > num || p.D > num))
                        mesh.Faces.AddFace(p);


            mesh.RebuildNormals();
            DA.SetData(0, mesh);







        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd6-edf31cd61ba1"); }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.MeshRepair;
    }
}