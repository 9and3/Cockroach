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
using System.Drawing;
using GH_IO.Types;
using Grasshopper;


namespace CockroachGH {
    public class GH_Cloud : GH_GeometricGoo<PointCloud>, IGH_PreviewData, IGH_BakeAwareData {
        private Guid ReferenceGuid;

        private PointCloud DisplayCloud;

        private Plane ScanPos;

        public override BoundingBox Boundingbox {
            get {
                BoundingBox boundingBox;
                boundingBox = (this.m_value != null ? this.m_value.GetBoundingBox(true) : BoundingBox.Empty);
                return boundingBox;
            }
        }

        public BoundingBox ClippingBox {
            get {
                return this.Boundingbox;
            }
        }

        public override bool IsGeometryLoaded {
            get { return base.m_value != null; }
        }

        public override bool IsValid {
            get {
                bool flag;
                flag = (this.m_value != null ? this.m_value.IsValid : false);
                return flag;
            }
        }

        public override string IsValidWhyNot {
            get {
                return "Fail";
            }
        }

        public override Guid ReferenceID {
            get {
                return this.ReferenceGuid;
            }
            set {
                this.ReferenceGuid = value;
            }
        }

        public Plane ScannerPosition {
            get {
                return this.ScanPos;
            }
            set {
                this.ScanPos = value;
            }
        }

        public override string TypeDescription {
            get {
                return "Point Cloud wrapper";
            }
        }

        public override string TypeName {
            get {
                return "Cloud";
            }
        }

        public GH_Cloud() {
            this.ScanPos = Plane.WorldXY;
            this.ReferenceGuid = Guid.Empty;
            this.ScanPos = Plane.WorldXY;
        }
        public GH_Cloud(GH_Cloud other) {
            this.ScanPos = Plane.WorldXY;
            this.ReferenceGuid = Guid.Empty;
            if (other == null) {
                throw new ArgumentException("other");
            }
            this.ReferenceGuid = other.ReferenceGuid;
            if (other.m_value != null) {
                this.m_value = (PointCloud)other.m_value.Duplicate();
            }
            this.ScanPos = other.ScanPos;
        }

        public GH_Cloud(PointCloud c) : base(c) {
            this.ScanPos = Plane.WorldXY;
            this.ReferenceGuid = Guid.Empty;
            this.ScanPos = Plane.WorldXY;
        }

        public GH_Cloud(Guid RefGuid) {
            this.ScanPos = Plane.WorldXY;
            this.ReferenceGuid = Guid.Empty;
            this.ReferenceGuid = RefGuid;
            this.ScanPos = Plane.WorldXY;
        }

        public override bool CastFrom(object source) {
            GH_Cloud gHCloud = this;
            return GH_CloudConvert.ToGHCloud( System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(System.Runtime.CompilerServices.RuntimeHelpers.GetObjectValue(source)),GH_Conversion.Both, ref gHCloud);
        }

        private T DirectCast<T>(object o) where T : class {
            T value = o as T;
            if (value == null && o != null) {
                throw new InvalidCastException();
            }
            return value;
        }

        public override bool CastTo<Q>(ref Q target) {
            bool flag;
            if (!typeof(Q).IsAssignableFrom(typeof(PointCloud))) {
                flag = false;
            } else if (this.m_value != null) {
                Rhino.RhinoApp.WriteLine("Hi");
                /////////////////? Dim obj2 As Object = Me.m_value.DuplicateShallow   target = DirectCast(obj2, Q)
                Object o = this.m_value;//.DuplicateShallow();
                target = (Q)o;
                flag = true;
            } else {
                flag = false;
            }
            return flag;
        }

        public override void ClearCaches() {
            if (this.IsReferencedGeometry) {
                this.m_value = null;
            }
        }

        private List<Line> CreateAxes(double S) {
            List<Line> lines = new List<Line>()
            {
                new Line(new Point3d(0, 0, 0), new Point3d(S * 1.5, 0, 0)),
                new Line(new Point3d(0, 0, 0), new Point3d(0, S * 1.5, 0)),
                new Line(new Point3d(0, 0, 0), new Point3d(0, 0, S * 1.5))
            };
            Transform plane = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, this.ScanPos);
            int count = checked(lines.Count - 1);
            for (int i = 0; i <= count; i = checked(i + 1)) {
                Line item = lines[i];
                item.Transform(plane);
                lines[i] = item;
            }
            return lines;
        }

        private List<Line> CreatePosLines(double S) {
            List<Line> lines = new List<Line>();
            double s = S;
            double num = 2 * S;
            bool flag = num >= 0;
            double s1 = -S;
            while (true) {
                if ((flag ? s1 > s : s1 < s)) {
                    break;
                }
                Line line = new Line(new Point3d(-S, s1, 0), new Point3d(S, s1, 0));
                Line line1 = new Line(new Point3d(s1, -S, 0), new Point3d(s1, S, 0));
                Transform plane = Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, this.ScanPos);
                line.Transform(plane);
                line1.Transform(plane);
                lines.Add(line);
                lines.Add(line1);
                s1 += num;
            }
            return lines;
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
        }

        public void DrawViewportWires(GH_PreviewWireArgs args) {
            if (this.m_value != null) {
                if (StaticParameters.DisplayPositions) {
                    args.Pipeline.DrawLines(this.CreatePosLines(CentralSettings.PreviewPlaneRadius), args.Color, checked(args.Thickness * 2));
                    List<Line> lines = this.CreateAxes(CentralSettings.PreviewPlaneRadius);
                    Color previewColourSelected = Grasshopper.Instances.ActiveCanvas.Document.PreviewColourSelected;
                    previewColourSelected = Color.FromArgb(255, (int)previewColourSelected.R, (int)previewColourSelected.G, (int)previewColourSelected.B);
                    if (previewColourSelected != args.Color) {
                        args.Pipeline.DrawLine(lines[0], Rhino.ApplicationSettings.AppearanceSettings.GridXAxisLineColor, checked(args.Thickness * 2));
                        args.Pipeline.DrawLine(lines[1], Rhino.ApplicationSettings.AppearanceSettings.GridYAxisLineColor, checked(args.Thickness * 2));
                        args.Pipeline.DrawLine(lines[2], Rhino.ApplicationSettings.AppearanceSettings.GridZAxisLineColor, checked(args.Thickness * 2));
                    } else {
                        args.Pipeline.DrawLine(lines[0], args.Color, checked(args.Thickness * 2));
                        args.Pipeline.DrawLine(lines[1], args.Color, checked(args.Thickness * 2));
                        args.Pipeline.DrawLine(lines[2], args.Color, checked(args.Thickness * 2));
                    }
                }
                if (StaticParameters.DisplayDynamic) {
                    if (!args.Pipeline.IsDynamicDisplay) {
                        args.Pipeline.DrawPointCloud(this.m_value, StaticParameters.DisplayRadius, args.Color);
                        return;
                    }
                    this.ResolveDisplay();
                    args.Pipeline.DrawPointCloud(this.DisplayCloud, checked(StaticParameters.DisplayRadius + 1), args.Color);
                    return;
                }
                args.Pipeline.DrawPointCloud(this.m_value, StaticParameters.DisplayRadius, args.Color);
            }
        }

        public override IGH_Goo Duplicate() {
            return this.DuplicateCloud();
        }

        public GH_Cloud DuplicateCloud() {
            return new GH_Cloud(this);
        }

        public override IGH_GeometricGoo DuplicateGeometry() {
            return this.DuplicateCloud();
        }

        public override IGH_GooProxy EmitProxy() {
            return new GH_Cloud.GH_CloudProxy(this);
        }

        public override BoundingBox GetBoundingBox(Transform xform) {
            BoundingBox boundingBox;
            boundingBox = (this.m_value != null ? this.m_value.GetBoundingBox(xform) : BoundingBox.Empty);
            return boundingBox;
        }
     
        public override bool LoadGeometry(RhinoDoc doc) {
            bool flag;
            RhinoObject rhinoObject = doc.Objects.Find(this.ReferenceID);
            if (rhinoObject == null || rhinoObject.Geometry.ObjectType != ObjectType.PointSet || !(rhinoObject.Geometry is PointCloud)) {
                flag = false;
            } else {
                this.m_value = (PointCloud)((PointCloud)rhinoObject.Geometry).DuplicateShallow();
                this.ScanPos = Plane.WorldXY;
                flag = true;
            }
            return flag;
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
            IGH_GeometricGoo gHGeometricGoo;
            if (this.IsValid) {
                double modelAbsoluteTolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance * 1000;
                Plane WorldXY = Plane.WorldXY;
                Plane plane = new Plane(new Point3d(0, 0, 0), new Vector3d(0, -1, 0), new Vector3d(1, 0, 0));
                Point3d origin = this.ScannerPosition.Origin;
                WorldXY.Translate(new Vector3d(-modelAbsoluteTolerance, 0, 0));
                plane.Translate(new Vector3d(0, modelAbsoluteTolerance, 0));
                Circle circle = new Circle(WorldXY, modelAbsoluteTolerance);
                Circle circle1 = new Circle(plane, modelAbsoluteTolerance);
                circle.Transform(Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, this.ScannerPosition));
                circle1.Transform(Rhino.Geometry.Transform.PlaneToPlane(Plane.WorldXY, this.ScannerPosition));
                Curve nurbsCurve = circle.ToNurbsCurve();
                Curve curve = circle1.ToNurbsCurve();
                xmorph.Morph(nurbsCurve);
                xmorph.Morph(curve);
                origin = xmorph.MorphPoint(origin);
                this.ScannerPosition = new Plane(origin, curve.TangentAt(0), nurbsCurve.TangentAt(0));
                xmorph.Morph(this.m_value);
                this.ReferenceID = Guid.Empty;
                gHGeometricGoo = this;
            } else {
                gHGeometricGoo = null;
            }
            return gHGeometricGoo;
        }

        public override bool Read(GH_IReader reader) {
            this.ReferenceGuid = Guid.Empty;
            this.m_value = null;
            this.ReferenceGuid = reader.GetGuid("RefID");
            //Grasshopper.Kernel.Types.GH_Plane plane = reader.GetPlane("ScannerPosition");
            //GH_Point3D origin = plane.Origin;
            //GH_Point3D xAxis = plane.XAxis;
            //GH_Point3D yAxis = plane.YAxis;
            //this.ScanPos = new Plane(new Point3d(origin.x, origin.y, origin.z), new Point3d(xAxis.x, xAxis.y, xAxis.z), new Point3d(yAxis.x, yAxis.y, yAxis.z));
            if (reader.ItemExists("ON_Data")) {
                this.m_value = GH_Convert.ByteArrayToCommonObject<PointCloud>(reader.GetByteArray("ON_Data"));
            }
            return true;
        }

        private void ResolveDisplay() {
            Random random = new Random();
            this.DisplayCloud = null;
            this.DisplayCloud = new PointCloud();
            if (this.m_value.Count < 1000) {
                this.DisplayCloud.AddRange(this.m_value.GetPoints());
                return;
            }
            int count = checked(this.m_value.Count - 1);
            for (int i = 0; i <= count; i = checked(i + 1000)) {
                long num = (long)random.Next(0, checked(this.m_value.Count() - 1));
                this.DisplayCloud.Add(this.m_value[checked((int)num)].Location);
                this.DisplayCloud[checked(this.DisplayCloud.Count() - 1)].Color = (this.m_value[checked((int)num)].Color);
            }
        }

        public override string ToString() {
            return string.Format("PointCloud {0} HasNormals {1} HasColors {2} HasPointValues {3}", this.m_value.Count, this.m_value.ContainsNormals, this.m_value.ContainsColors, this.m_value.ContainsPointValues).ToString();
        }

        public override IGH_GeometricGoo Transform(Transform xform) {
            IGH_GeometricGoo gHGeometricGoo;
            IEnumerator<PointCloudItem> enumerator = null;
            if (this.IsValid) {
                this.m_value.Transform(xform);
                if (this.m_value.ContainsNormals) {
                    try {
                        enumerator = this.m_value.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            PointCloudItem current = enumerator.Current;
                            Vector3d normal = current.Normal;
                            normal.Transform(xform);
                            current.Normal=(normal);
                        }
                    } finally {
                        if (enumerator != null) {
                            enumerator.Dispose();
                        }
                    }
                }
                this.ReferenceID = Guid.Empty;
                this.ScanPos.Transform(xform);
                gHGeometricGoo = this;
            } else {
                gHGeometricGoo = null;
            }
            return gHGeometricGoo;
        }

        public override bool Write(GH_IWriter writer) {
            writer.SetGuid("RefID", this.ReferenceGuid);
            Point3d point3d = new Point3d(this.ScanPos.Origin + this.ScanPos.XAxis);
            Point3d point3d1 = new Point3d(this.ScanPos.Origin + this.ScanPos.YAxis);
            GH_Point3D gHPoint3D = new GH_Point3D(point3d.X, point3d.Y, point3d.Z);
            GH_Point3D gHPoint3D1 = new GH_Point3D(point3d1.X, point3d1.Y, point3d1.Z);
            Point3d origin = this.ScanPos.Origin;
            double x = origin.X;
            origin = this.ScanPos.Origin;
            double y = origin.Y;
            origin = this.ScanPos.Origin;
            GH_Point3D gHPoint3D2 = new GH_Point3D(x, y, origin.Z);
            //writer.SetPlane("ScannerPosition", new Grasshopper.Kernel.Types.GH_Plane(gHPoint3D2, gHPoint3D, gHPoint3D1));
            if (this.ReferenceID == Guid.Empty && this.m_value != null) {
                byte[] byteArray = GH_Convert.CommonObjectToByteArray(this.m_value);
                if (byteArray != null) {
                    writer.SetByteArray("ON_Data", byteArray);
                }
            }
            return true;
        }
        
        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid) {
            bool flag=false;
            obj_guid = new Guid();
            if (this.IsValid) {
                obj_guid = doc.Objects.AddPointCloud(this.m_value, att);
                flag = true;
            } else {
                flag = false;
            }
            return flag;
        }



        public class GH_CloudProxy : GH_GooProxy<GH_Cloud> {
            public string ObjectID {
                get {
                    string str;
                    str = (!base.Owner.IsReferencedGeometry ? "none" : string.Format("{0}", base.Owner.ReferenceID));
                    return str;
                }
                set {
                    if (base.Owner.IsReferencedGeometry) {
                        try {
                            Guid guid = new Guid(value);
                            base.Owner.ReferenceID = guid;
                            base.Owner.ClearCaches();
                            base.Owner.LoadGeometry();
                            base.Owner.ScanPos = Plane.WorldXY;
                        } catch (Exception exception) {
                            Rhino.RhinoApp.WriteLine(exception.ToString());
                            //ProjectData.SetProjectError(exception);
                            //ProjectData.ClearProjectError();
                        }
                    }
                }
            }

            public string Type {
                get {
                    string str;
                    if (base.Owner.Value != null) {
                        str = (!(base.Owner.Value is PointCloud) ? "Other" : "Point Cloud");
                    } else {
                        str = "No cloud";
                    }
                    return str;
                }
            }

            public GH_CloudProxy(GH_Cloud owner) : base(owner) {
            }

            public override void Construct() {
                try {
                    Instances.DocumentEditorFadeOut();
                    GH_Cloud gHCloud = null;
                    if (gHCloud != null) {
                        base.Owner.m_value = gHCloud.m_value;
                        base.Owner.ReferenceGuid = gHCloud.ReferenceGuid;
                        base.Owner.LoadGeometry();
                        base.Owner.ScanPos = Plane.WorldXY;
                    }
                } finally {
                    Instances.DocumentEditorFadeIn();
                }
            }

            public override bool FromString(string @in) {
                return false;
            }
        }




    }
}
