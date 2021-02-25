using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockroachGH.Cleaning {
    public class Instruction : Grasshopper.Kernel.Types.GH_Goo<Instruction> {
        public virtual Guid InstructionGUID {
            get {
                return Guid.Empty;
            }
        }

        public virtual string InstructionType {
            get {
                return "";
            }
        }

        public override bool IsValid {
            get {
                bool flag;
                flag = (Operators.CompareString(this.InstructionType, string.Empty, false) != 0 ? true : false);
                return flag;
            }
        }

        public int CompareString(string Left, string Right, bool TextCompare) {
            if (Left == Right)
                return 0;
            if (Left == null)
                return Right.Length == 0 ? 0 : -1;
            else if (Right == null) {
                return Left.Length == 0 ? 0 : 1;
            } else {
                int num = !TextCompare
                   ? string.CompareOrdinal(Left, Right)
                   : System.Globalization.CompareInfo.Compare(Left, Right, System.Globalization.CompareOptions.IgnoreCase | System.Globalization.CompareOptions.IgnoreKanaType | System.Globalization.CompareOptions.IgnoreWidth);
                if (num == 0)
                    return 0;
                return num > 0 ? 1 : -1;
            }
        }

        public override string TypeDescription {
            get {
                return this.InstructionType;
            }
        }

        public override string TypeName {
            get {
                return this.InstructionType;
            }
        }

        public Instruction() {
        }

        public override Grasshopper.Kernel.Types.IGH_Goo Duplicate() {
            return (Grasshopper.Kernel.Types.IGH_Goo)base.MemberwiseClone();
        }

        public virtual bool Execute(ref Rhino.Geometry.PointCloud PointCloud) {
            return true;
        }

        public override string ToString() {
            return this.InstructionType;
        }
    }
}
