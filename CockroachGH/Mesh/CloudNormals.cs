using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public class CloudNormals : GH_Component {

        Line[] lines = new Line[0];
        BoundingBox bbox = new BoundingBox();

        public override BoundingBox ClippingBox => bbox;
        public override void DrawViewportWires(IGH_PreviewArgs args) {
            if (Attributes.Selected == true)
                args.Display.DrawLines(lines, args.WireColour_Selected);
            else
                args.Display.DrawLines(lines, args.WireColour);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Normals;
        public CloudNormals()
  : base("CloudNormals", "Normals",
      "CloudNormals",
      "Cockroach", "Mesh") {
        }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Iter", "I", "Iterations", GH_ParamAccess.item,30);
            pManager.AddIntegerParameter("Neigbhours", "N", "Neighbours for reorientation", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("D", "D", "Display Normals as Lines", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Erase", "E", "Erase non oriented normals", GH_ParamAccess.item, true);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {

            //pManager.AddGenericParameter("PointCloud", "C", "PointCloud", GH_ParamAccess.item);
            pManager.AddParameter(new Param_Cloud(), "PointCloud", "C", "PointCloud", GH_ParamAccess.item);

        }

        protected override void SolveInstance(IGH_DataAccess DA) {

            // Input
            try {
                var cgh = new GH_Cloud();
                DA.GetData(0, ref cgh);

                double r = 0.1;
                int it = 30;
                int n = 10;
                double length = 1;
                bool erase = true;
                DA.GetData(1, ref r);
                DA.GetData(2, ref it);
                DA.GetData(3, ref n);
                DA.GetData(4, ref length);
                DA.GetData(5, ref erase);

                var c =  PInvokeCSharp.TestCGAL.CreateNormals(cgh.Value, r, it, n,erase);//f ? PInvokeCSharp.TestOpen3D.Normals(cgh.Value, r, it, n) :
                bbox = c.GetBoundingBox(false);


                if (length > 0.0001) {
                    lines = new Line[c.Count];
                    for (int i = 0; i < c.Count; i++) {
                        lines[i] = new Line(c[i].Location,c[i].Location+c[i].Normal*length);
                    }
                }


                //this.Message = c.Count.ToString();

                DA.SetData(0, new GH_Cloud(c));
            }catch(Exception e) {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("98aaf1ed-7834-48c6-8fd1-edf34cd11ba1"); }
        }
    }
}