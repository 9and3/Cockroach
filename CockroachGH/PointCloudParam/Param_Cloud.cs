using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;


namespace CockroachGH {
    public class Param_Cloud : GH_PersistentGeometryParam<GH_Cloud>, IGH_BakeAwareObject, IGH_PreviewObject {
        private bool m_hidden;

        public BoundingBox ClippingBox {
            get {
                return base.Preview_ComputeClippingBox();
            }
        }

        public override Guid ComponentGuid {
            get {
                return new Guid("{E285577D-197D-42AB-9FA8-E634FB1DDDDD}");
            }
        }

        //private virtual Control_Display DispContr {
        //    get {
        //        return this._DispContr;
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    set {
        //        Control_Display.DynamicClickedEventHandler dynamicClickedEventHandler = new Control_Display.DynamicClickedEventHandler(this.DynamicSwitch);
        //        Control_Display.PlusClickedEventHandler plusClickedEventHandler = new Control_Display.PlusClickedEventHandler(this.IncreaseRadius);
        //        Control_Display.MinusClickedEventHandler minusClickedEventHandler = new Control_Display.MinusClickedEventHandler(this.DecreaseRadius);
        //        Control_Display controlDisplay = this._DispContr;
        //        if (controlDisplay != null) {
        //            controlDisplay.DynamicClicked -= dynamicClickedEventHandler;
        //            controlDisplay.PlusClicked -= plusClickedEventHandler;
        //            controlDisplay.MinusClicked -= minusClickedEventHandler;
        //        }
        //        this._DispContr = value;
        //        controlDisplay = this._DispContr;
        //        if (controlDisplay != null) {
        //            controlDisplay.DynamicClicked += dynamicClickedEventHandler;
        //            controlDisplay.PlusClicked += plusClickedEventHandler;
        //            controlDisplay.MinusClicked += minusClickedEventHandler;
        //        }
        //    }
        //}

        public override GH_Exposure Exposure {
            get {
                return GH_Exposure.primary;
            }
        }

        public bool Hidden {
            get;
            set;
        }

        protected override Bitmap Icon {
            get {
                return Properties.Resources.Cloud;// Resources.Icon_CloudParam;
            }
        }

        public bool IsBakeCapable {
            get {
                return !this.m_data.IsEmpty;
            }
        }

        public bool IsPreviewCapable {
            get {
                return true;
            }
        }

        public override string TypeName {
            get {
                return "Cloud";
            }
        }

        public Param_Cloud() : base(new GH_InstanceDescription("PointCloud", "Cloud", "PointCloud", "Cockroach", "Cloud")) {
           // this.DispContr = new Control_Display();
            this.m_hidden = false;
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
            base.AppendAdditionalMenuItems(menu);
            GH_DocumentObject.Menu_AppendSeparator(menu);
            //if (!StaticParameters.DisplayDynamic) {
            //    this.DispContr.DispButBack = Color.White;
            //    this.DispContr.DispButFrame = Color.FromArgb(255, 189, 189, 189);
            //} else {
            //    this.DispContr.DispButBack = Color.FromArgb(255, 196, 225, 255);
            //    this.DispContr.DispButFrame = Color.FromArgb(255, 51, 153, 255);
            //}
            //GH_DocumentObject.Menu_AppendCustomItem(menu, this.DispContr);

            //GH_DocumentObject.Menu_AppendItem(menu, "Show scanner position", (object a0, EventArgs a1) => this.PositionSwitch(), true, StaticParameters.DisplayPositions);
        }

        public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
            this.BakeGeometry(doc, null, obj_ids);
        }

        public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
            Guid guid = new Guid();
            IEnumerator enumerator = null;
            if (att == null) {
                att = doc.CreateDefaultAttributes();
            }
            try {
                enumerator = this.m_data.GetEnumerator();
                while (enumerator.MoveNext()) {
                    IGH_BakeAwareData current = (IGH_BakeAwareData)enumerator.Current;
                    if (current == null || !current.BakeGeometry(doc, att, out guid)) {
                        continue;
                    }
                    obj_ids.Add(guid);
                }
            } finally {
                if (enumerator is IDisposable) {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        private void DecreaseRadius() {
            if (StaticParameters.DisplayRadius > 1) {
                StaticParameters.DisplayRadius = checked(StaticParameters.DisplayRadius - 1);
            }
            this.ExpirePreview(true);
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args) {
        }

        public void DrawViewportWires(IGH_PreviewArgs args) {
            base.Preview_DrawWires(args);
        }

        private void DynamicSwitch() {
            StaticParameters.DisplayDynamic = !StaticParameters.DisplayDynamic;
            if (!StaticParameters.DisplayDynamic) {
                //this.DispContr.DispButBack = Color.White;
                //this.DispContr.DispButFrame = Color.FromArgb(255, 189, 189, 189);
                return;
            }
            //this.DispContr.DispButBack = Color.FromArgb(255, 196, 225, 255);
            //this.DispContr.DispButFrame = Color.FromArgb(255, 51, 153, 255);
        }

        private void IncreaseRadius() {
            StaticParameters.DisplayRadius = checked(StaticParameters.DisplayRadius + 1);
            this.ExpirePreview(true);
        }

        protected override GH_Cloud InstantiateT() {
            return new GH_Cloud();
        }

        private void PositionSwitch() {
            StaticParameters.DisplayPositions = !StaticParameters.DisplayPositions;
            this.ExpirePreview(true);
        }

        protected override GH_Cloud PreferredCast(object data) {
            GH_Cloud gHCloud;
            if (data is PointCloud) {
                gHCloud = new GH_Cloud((PointCloud)data);
            } else {
                gHCloud = null;
            }
            return gHCloud;
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Cloud> values) {
            GH_GetterResult gHGetterResult;
            values = GH_CloudGetter.GetClouds();
            gHGetterResult = (values == null || values.Count == 0 ? GH_GetterResult.cancel : GH_GetterResult.success);
            return gHGetterResult;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Cloud value) {
            value = GH_CloudGetter.GetCloud();
            return (value != null ? GH_GetterResult.success : GH_GetterResult.cancel);
        }

        public override string ToString() {
            return "Cloud";
        }
    }
}
