using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Rhino;
using Rhino.DocObjects;
using Grasshopper.Kernel.Data;
using GH_IO.Serialization;
using GH_IO;

namespace CockroachGH {
    public class PointCloudGH : GH_GeometricGoo<PointCloud>, IGH_PreviewData, IGH_BakeAwareObject {

        
        public PointCloudGH() {
            this.m_value = new PointCloud();
        }

        public PointCloudGH(PointCloud cloud) {
            this.ReferenceGuid = Guid.Empty;
            //this.ReferenceGuid = RefGuid;
            this.m_value = cloud;
        }

        public PointCloudGH(PointCloudGH other) {
            this.ReferenceGuid = Guid.Empty;
            m_value = (PointCloud)other.m_value;//.Duplicate();
        }



        public override BoundingBox Boundingbox {
            get {
                return this.m_value.GetBoundingBox(true);
            }
        }

        public override string TypeDescription {
            get {
                return "PointCloud For Grasshopper";
            }
        }

        public override string TypeName {
            get {
                return "PointCloud";
            }
        }

        public BoundingBox ClippingBox {
            get {
                return this.m_value.GetBoundingBox(true);
            }
        }

        public bool IsBakeCapable {
            get {
                return this.m_value.Count > 0;
            }
        }



        public override BoundingBox GetBoundingBox(Transform xform) {
            return this.m_value.GetBoundingBox(xform);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
            var dup = this.m_value.Duplicate();
            xmorph.Morph(dup);
            return new PointCloudGH((PointCloud)dup);
        }

        public override string ToString() {
            return String.Format("PointCloud {0}, HasNormals {1}, HasColors {2}", m_value.Count, m_value.ContainsNormals, m_value.ContainsColors);
        }

        public override IGH_GeometricGoo Transform(Transform xform) {
            var dup = this.m_value.Duplicate();
            dup.Transform(xform);
            return new PointCloudGH((PointCloud)dup);
        }

        public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
            BakeGeometry(doc, new ObjectAttributes(), obj_ids);
        }

        public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
            obj_ids.Add(doc.Objects.AddPointCloud(m_value, att));
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
            // No meshes to draw
        }

        public void DrawViewportWires(GH_PreviewWireArgs args) {
            args.Pipeline.DrawPointCloud(this.m_value, 1);
        }

        public override bool CastFrom(object source) {
            if (source.GetType() == typeof(PointCloud)) {
                this.m_value = (PointCloud)source;
                return true;
            }

            if (source.GetType() == typeof(PointCloudGH)) {
                this.m_value = ((PointCloudGH)source).Value;
                return true;
            }

            var asOtherPC = (GH_GeometricGoo<PointCloud>)source;
            if (asOtherPC != null) {
                this.m_value = asOtherPC.Value;
                return true;
            }

            if (source.GetType() == typeof(IEnumerable<Point3d>)) {
                this.m_value = new PointCloud((IEnumerable<Point3d>)source);
                return true;
            }

            return base.CastFrom(source);
        }

        //public override bool CastTo<T>(ref T target) {
        //    Type q = typeof(T);
        //    if (Value != null && Value.GetType().InheritsOrImplements(q)) {
        //        target = (T)(object)Value;
        //        return true;
        //    }
        //    return base.CastTo(ref target);
        //}
        public override bool CastTo<Q>(ref Q target) {
            Type q = typeof(Q);
            if (typeof(Q) == typeof(PointCloud)) {
                target = (Q)(object)this.m_value;
                return true;
            }

            if (typeof(Q) == typeof(PointCloudGH)) {
                target = (Q)(object)this;
                return true;
            }

            if (typeof(Q) == typeof(IEnumerable<Point3d>)) {
                target = (Q)(object)this.m_value.GetPoints();
                return true;
            }
            return base.CastTo(ref target);
            //return base.CastTo<Q>(out target);
        }

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, ref Guid obj_guid) {
            bool flag;
            if (this.IsValid) {
                obj_guid = doc.Objects.AddPointCloud(this.m_value, att);
                flag = true;
            } else {
                flag = false;
            }
            return flag;
        }

        private Guid ReferenceGuid;

        public override Guid ReferenceID {
            get {
                return this.ReferenceGuid;
            }
            set {
                this.ReferenceGuid = value;
            }
        }

        public override bool Read(GH_IReader reader) {
            this.ReferenceGuid = Guid.Empty;
            this.m_value = null;


            if (reader.ItemExists("ON_Data_Cloud")) {
            this.m_value = GH_Convert.ByteArrayToCommonObject<PointCloud>(reader.GetByteArray("ON_Data_Cloud"));
            }

            return true;
        }

        public override bool Write(GH_IWriter writer) {
            //writer.SetGuid("RefID", this.ReferenceGuid);

            if (this.m_value != null) {//this.ReferenceID == Guid.Empty && 
                byte[] byteArray = GH_Convert.CommonObjectToByteArray(this.m_value);
                if (byteArray != null) {
                    writer.SetByteArray("ON_Data_Cloud", byteArray);
                }
            }
            return true;
        }





        public override IGH_Goo Duplicate() {
            return this.DuplicateCloud();
        }

        public PointCloudGH DuplicateCloud() {
            return new PointCloudGH(this);
        }

        public override IGH_GeometricGoo DuplicateGeometry() {
            return this.DuplicateCloud();
        }









    }
}