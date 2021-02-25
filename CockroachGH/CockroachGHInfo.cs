using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace CockroachGH {
    public class CockroachGHInfo : GH_AssemblyInfo {
        public override string Name {
            get {
                return "CockroachGH";
            }
        }
        public override Bitmap Icon {
            get {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description {
            get {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id {
            get {
                return new Guid("b17c1cbd-f954-4501-9586-77a8b3a7c692");
            }
        }

        public override string AuthorName {
            get {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact {
            get {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
