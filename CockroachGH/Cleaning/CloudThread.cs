using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
//using Microsoft.VisualBasic.CompilerServices;
using Rhino;
using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
//using Volvox.My.Resources;
//using Volvox_Cloud;
//using Volvox_Instr;

namespace CockroachGH {
    public class Cloud_Enginex : GH_Component, IGH_VariableParameterComponent {
        internal bool ShowInfo;

        private DataTree<PointCloud> InputClouds;

        private bool finished;

        private bool failed;

        private bool running;

        private List<Instr_Base> inlist;

        private Thread tmaster;

        internal int MessagePercent;

        internal string MessageTitle;

        internal string MessageCustom;

        private Action DrawMessage;

        private Stopwatch st;

        public override Guid ComponentGuid {
            get {
                return GuidsRelease2.Comp_EngineX;
            }
        }

        public override GH_Exposure Exposure {
            get {
                return 2;
            }
        }

        protected override Bitmap Icon {
            get {
                return null;
            }
        }

        public Cloud_Enginex() : base("Cloud EngineX", "CloudX", "Point Cloud manipulation engine.", "Cockroach", "Crop") {
            this.ShowInfo = true;
            this.InputClouds = new DataTree<PointCloud>();
            this.finished = false;
            this.failed = false;
            this.running = false;
            this.inlist = new List<Instr_Base>();
            this.tmaster = null;
            this.MessagePercent = -1;
            this.MessageTitle = "...";
            this.MessageCustom = string.Empty;
            this.DrawMessage = new Action(this.ExpireDisplay);
            this.st = new Stopwatch();
            this.get_Params().add_ParameterSourcesChanged(new GH_ComponentParamServer.ParameterSourcesChangedEventHandler(this, Cloud_Enginex.InputChanged));
        }

        public void AbortAllThreads() {
            int count = checked(this.inlist.Count - 1);
            for (int i = 0; i <= count; i = checked(i + 1)) {
                Instr_BaseReporting item = this.inlist[i] as Instr_BaseReporting;
                if (item != null) {
                    item.Abort();
                }
            }
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
            base.AppendAdditionalMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, "Show Progress", (object a0, EventArgs a1) => this.ChangeShowInfo(), true, this.ShowInfo);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index) {
            bool flag;
            flag = (side != null || !(index != 0 & index != 1 & index != 2) ? false : true);
            return flag;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index) {
            bool flag;
            if (side == null) {
                if (this.Params.Input.Count != 4) {
                    if (!(index != 0 & index != 1) | index == 2) {
                        flag = false;
                        return flag;
                    }
                    flag = true;
                    return flag;
                } else {
                    if (!(index != 0 & index != 1 & index != 2) | index == 3) {
                        flag = false;
                        return flag;
                    }
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        public void ChangeShowInfo() {
            this.ShowInfo = !this.ShowInfo;
            this.OnDisplayExpired(false);
        }

        public override void CreateAttributes() {
            this.m_attributes = new Cloud_EngineAttx(this);
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index) {
            Param_Instr paramInstr = new Param_Instr();
            ((GH_Param<Instr_Base>)paramInstr).set_Access(2);
            ((GH_Param<Instr_Base>)paramInstr).set_Optional(true);
            this.VariableParameterMaintenance();
            return paramInstr;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index) {
            this.VariableParameterMaintenance();
            return true;
        }

        private void ExpireComponent() {
            this.ExpireSolution(true);
        }

        private void ExpireDisplay() {
            this.OnDisplayExpired(false);
        }

        protected override string HtmlHelp_Source() {
            return base.HtmlHelp_Source();
        }

        public void InputChanged(object sender, GH_ParamServerEventArgs e) {
            if (e.get_ParameterIndex() == checked(this.get_Params().get_Input().Count - 1)) {
                Param_Instr paramInstr = new Param_Instr();
                paramInstr.set_Access(2);
                paramInstr.set_Optional(true);
                this.get_Params().RegisterInputParam(paramInstr);
                this.VariableParameterMaintenance();
                this.get_Params().OnParametersChanged();
            }
        }

        private void InvokeCustom(object sender, Instr_BaseReporting.ReportingCustomArgs e) {
            this.MessageCustom = e.Custom;
            Action action = new Action(this.ExpireDisplay);
            if (RhinoApp.get_MainApplicationWindow().get_InvokeRequired()) {
                RhinoApp.get_MainApplicationWindow().Invoke(action);
            }
        }

        private void InvokePercent(object sender, Instr_BaseReporting.ReportingPercentArgs e) {
            this.MessagePercent = e.Percent;
            Action action = new Action(this.ExpireDisplay);
            if (RhinoApp.get_MainApplicationWindow().get_InvokeRequired()) {
                RhinoApp.get_MainApplicationWindow().Invoke(action);
            }
        }

        public override bool Read(GH_IReader reader) {
            this.ShowInfo = reader.GetBoolean("showinfo");
            return base.Read(reader);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "Cloud", "C", "Cloud to process", 2);
            pManager.AddBooleanParameter("Run", "R", "Run engine", 0, false);
            pManager.AddBooleanParameter("Abort", "A", "Abort execution", 0, false);
            ((Param_Cloud)pManager.get_Param(0)).Hidden = true;
            pManager.AddParameter(new Param_Instr(), "Instruction0", "I0", "Instruction to execute, data tree is flattened internally.", 2);
            pManager.get_Param(3).set_Optional(true);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddParameter(new Param_Cloud(), "Cloud", "C", "Processed cloud", 0);
        }

        public override void RemovedFromDocument(GH_Document document) {
            this.running = false;
            if (this.tmaster != null) {
                this.tmaster.Abort();
            }
            this.tmaster = null;
            this.AbortAllThreads();
            this.finished = false;
            this.failed = false;
            this.inlist.Clear();
            this.InputClouds.Clear();
            GC.Collect();
            base.RemovedFromDocument(document);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            IEnumerator<IGH_Goo> enumerator = null;
            bool flag = false;
            if (DA.GetData<bool>(1, ref flag)) {
                bool flag1 = false;
                if (DA.GetData<bool>(2, ref flag1)) {
                    if (!flag & !flag1 & this.tmaster != null) {
                        RhinoApp.get_MainApplicationWindow().Invoke(this.DrawMessage);
                    }
                    if (flag & this.tmaster == null) {
                        this.st.Start();
                        this.running = true;
                        this.MessagePercent = -1;
                        this.MessageTitle = "...";
                        this.MessageCustom = string.Empty;
                        this.finished = false;
                        this.failed = false;
                        this.InputClouds.Clear();
                        GH_Structure<GH_Cloud> gHStructure = new GH_Structure<GH_Cloud>();
                        if (!DA.GetDataTree<GH_Cloud>(0, ref gHStructure)) {
                            return;
                        }
                        int count = checked(gHStructure.get_Branches().Count - 1);
                        for (int i = 0; i <= count; i = checked(i + 1)) {
                            int num = checked(gHStructure.get_Branch(i).Count - 1);
                            for (int j = 0; j <= num; j = checked(j + 1)) {
                                GH_Cloud item = (GH_Cloud)gHStructure.get_Branch(i)[j];
                                if (item == null) {
                                    this.InputClouds.Add(null, gHStructure.get_Paths()[i]);
                                } else {
                                    this.InputClouds.Add((PointCloud)item.get_Value().Duplicate(), gHStructure.get_Paths()[i]);
                                }
                            }
                        }
                        int count1 = checked(this.get_Params().get_Input().Count - 1);
                        for (int k = 3; k <= count1; k = checked(k + 1)) {
                            GH_Structure<Instr_Base> gHStructure1 = new GH_Structure<Instr_Base>();
                            if (DA.GetDataTree<Instr_Base>(k, ref gHStructure1)) {
                                try {
                                    enumerator = gHStructure1.AllData(true).GetEnumerator();
                                    while (enumerator.MoveNext()) {
                                        Instr_Base current = (Instr_Base)enumerator.Current;
                                        this.inlist.Add((Instr_Base)current.Duplicate());
                                    }
                                } finally {
                                    if (enumerator != null) {
                                        enumerator.Dispose();
                                    }
                                }
                            }
                        }
                        this.tmaster = new Thread(new ThreadStart(this.ThreadEngine)) {
                            Name = "CloudEngineX_MasterThread",
                            IsBackground = true,
                            Priority = ThreadPriority.Highest
                        };
                        this.tmaster.Start();
                    }
                    if (this.failed) {
                        this.running = false;
                        if (this.tmaster != null) {
                            this.tmaster.Abort();
                        }
                        this.tmaster = null;
                        this.AbortAllThreads();
                        this.finished = false;
                        this.failed = false;
                        this.inlist.Clear();
                        this.InputClouds.Clear();
                        this.st.Reset();
                        GC.Collect();
                        this.MessageCustom = string.Empty;
                        this.AddRuntimeMessage(20, "Instruction failed to extecute.");
                    }
                    if (this.finished) {
                        this.running = false;
                        this.MessagePercent = -1;
                        this.MessageTitle = string.Empty;
                        this.tmaster = null;
                        this.finished = false;
                        this.inlist.Clear();
                        this.MessageCustom = string.Concat(Conversions.ToString(this.st.ElapsedMilliseconds), " ms");
                        this.st.Reset();
                        GC.Collect();
                    }
                    if (flag1) {
                        this.running = false;
                        if (this.tmaster != null) {
                            this.tmaster.Abort();
                        }
                        this.tmaster = null;
                        this.AbortAllThreads();
                        this.MessagePercent = -1;
                        this.MessageTitle = "Aborted";
                        this.MessageCustom = string.Empty;
                        this.finished = false;
                        this.failed = false;
                        this.inlist.Clear();
                        this.InputClouds.Clear();
                        this.st.Reset();
                        GC.Collect();
                    }
                    if (!this.failed & !flag1 & !this.running) {
                        DA.SetDataTree(0, this.InputClouds);
                    }
                }
            }
        }

        public void ThreadEngine() {
            List<Instr_Base>.Enumerator enumerator = new List<Instr_Base>.Enumerator();
            this.failed = false;
            int dataCount = this.InputClouds.get_DataCount();
            int num = 1;
            lock (this.inlist) {
                lock (this.InputClouds) {
                    int count = checked(this.InputClouds.get_Paths().Count - 1);
                    for (int i = 0; i <= count; i = checked(i + 1)) {
                        int count1 = checked(this.InputClouds.Branch(i).Count - 1);
                        for (int j = 0; j <= count1; j = checked(j + 1)) {
                            PointCloud item = this.InputClouds.Branch(i)[j];
                            if (item != null) {
                                if (item.get_UserDictionary().ContainsKey("VolvoxFileMask")) {
                                    string str = item.get_UserDictionary().GetString("VolvoxFilePath");
                                    string str1 = item.get_UserDictionary().GetString("VolvoxFileMask");
                                    int integer = item.get_UserDictionary().GetInteger("VolvoxFileSeed");
                                    double num1 = item.get_UserDictionary().GetDouble("VolvoxFilePercent");
                                    Instr_LoadMulti instrLoadMulti = new Instr_LoadMulti(str, str1, num1, integer);
                                    this.inlist.Insert(0, instrLoadMulti);
                                } else if (item.get_UserDictionary().ContainsKey("VolvoxFilePath")) {
                                    string str2 = item.get_UserDictionary().GetString("VolvoxFilePath");
                                    double num2 = item.get_UserDictionary().GetDouble("VolvoxFilePercent");
                                    bool flag = item.get_UserDictionary().GetBool("VolvoxFileCheck");
                                    Instr_LoadE57 instrLoadE57 = new Instr_LoadE57(str2, num2, flag);
                                    this.inlist.Insert(0, instrLoadE57);
                                }
                                PointCloud pointCloud = item;
                                try {
                                    enumerator = this.inlist.GetEnumerator();
                                    while (enumerator.MoveNext()) {
                                        Instr_Base current = enumerator.Current;
                                        Instr_BaseReporting instrBaseReporting = current as Instr_BaseReporting;
                                        if (instrBaseReporting != null) {
                                            this.MessageTitle = string.Concat(new string[] { "Cloud ", Conversions.ToString(num), "/", Conversions.ToString(dataCount), "\r\n", instrBaseReporting.InstructionType });
                                            instrBaseReporting.ReportingPercent += new Instr_BaseReporting.ReportingPercentEventHandler(this.InvokePercent);
                                            instrBaseReporting.ReportingCustom += new Instr_BaseReporting.ReportingCustomEventHandler(this.InvokeCustom);
                                            if (instrBaseReporting.Execute(ref pointCloud)) {
                                                instrBaseReporting.ReportingPercent -= new Instr_BaseReporting.ReportingPercentEventHandler(this.InvokePercent);
                                                instrBaseReporting.ReportingCustom -= new Instr_BaseReporting.ReportingCustomEventHandler(this.InvokeCustom);
                                            } else {
                                                this.MessagePercent = -1;
                                                this.MessageTitle = "Instruction failed";
                                                this.MessageCustom = string.Empty;
                                                this.failed = true;
                                                goto Label0;
                                            }
                                        } else {
                                            this.MessagePercent = -1;
                                            this.MessageCustom = string.Empty;
                                            this.MessageTitle = string.Concat(new string[] { "Cloud ", Conversions.ToString(num), "/", Conversions.ToString(dataCount), "\r\n", current.InstructionType });
                                            RhinoApp.MainApplicationWindow.Invoke(this.DrawMessage);
                                            if (!current.Execute(ref pointCloud)) {
                                                this.MessagePercent = -1;
                                                this.MessageTitle = "Instruction failed";
                                                this.failed = true;
                                                goto Label0;
                                            }
                                        }
                                        this.MessageCustom = string.Empty;
                                    }
                                } finally {
                                    ((IDisposable)enumerator).Dispose();
                                }
                            Label0:
                                if (pointCloud.UserDictionary.ContainsKey("VolvoxFilePath")) {
                                    this.inlist.RemoveAt(0);
                                    pointCloud.UserDictionary.Remove("VolvoxFilePath");
                                    pointCloud.UserDictionary.Remove("VolvoxFileMask");
                                    pointCloud.UserDictionary.Remove("VolvoxFilePercent");
                                    pointCloud.UserDictionary.Remove("VolvoxFileSeed");
                                }
                                this.InputClouds.Branch(i)[j] = pointCloud;
                                num = checked(num + 1);
                            }
                        }
                    }
                }
            }
            this.inlist.Clear();
            this.finished = true;
            Action action = new Action(this.ExpireComponent);
            RhinoApp.MainApplicationWindow.Invoke(action);
        }

        public void VariableParameterMaintenance() {
            int count = checked(this.Params.Input.Count - 1);
            for (int i = 3; i <= count; i = checked(i + 1)) {
                IGH_Param item = this.Params.Input[i];
                item.Name=(string.Concat("Instruction", (checked(i - 3)).ToString() ));
                item.NickName=(string.Concat("I", (checked(i - 3)).ToString() ));
                item.Optional=(true);
                item.Description=("Instruction to execute, data tree is flattened internally.");
            }
        }

        public override bool Write(GH_IWriter writer) {
            writer.SetBoolean("showinfo", this.ShowInfo);
            return base.Write(writer);
        }
    }
}
