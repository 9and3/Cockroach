using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockroachGH {
    public static class StaticParameters {
        public static bool DisplayDynamic {
            get;
            set;
        }

        public static bool DisplayPositions {
            get;
            set;
        }

        public static int DisplayRadius {
            get;
            set;
        }

        static StaticParameters() {
            StaticParameters.DisplayDynamic = false;
            StaticParameters.DisplayRadius = 2;
            StaticParameters.DisplayPositions = false;
        }
    }
}
