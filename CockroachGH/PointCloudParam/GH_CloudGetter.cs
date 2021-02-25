using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace CockroachGH {
    public sealed class GH_CloudGetter {
        private static bool m_reference;

        static GH_CloudGetter() {
            GH_CloudGetter.m_reference = true;
        }

        public GH_CloudGetter() {
            GH_CloudGetter.m_reference = true;
        }

        public static GH_Cloud GetCloud() {
            GH_Cloud gHCloud;
            GetResult getResult;
            GetObject getObject = null;
            while (true) {
                getObject = new GetObject();
                if (GH_CloudGetter.m_reference) {
                    getObject.SetCommandPrompt("Cloud to reference");
                    getObject.AddOption("Mode", "Reference");
                } else {
                    getObject.SetCommandPrompt("Cloud to copy");
                    getObject.AddOption("Mode", "Copy");
                }
                getObject.GeometryFilter=(ObjectType.PointSet);
                getResult = getObject.Get();
                if ((ulong)getResult != (long)3) {
                    break;
                }
                GH_CloudGetter.m_reference = !GH_CloudGetter.m_reference;
            }
            if ((ulong)getResult == (long)12) {
                gHCloud = (!GH_CloudGetter.m_reference ? new GH_Cloud(getObject.Object(0).PointCloud()) : new GH_Cloud(getObject.Object(0).ObjectId));
            } else {
                gHCloud = null;
            }
            return gHCloud;
        }

        public static List<GH_Cloud> GetClouds() {
            List<GH_Cloud> gHClouds;
            GetResult multiple;
            GetObject getObject = null;
            while (true) {
                getObject = new GetObject();
                if (GH_CloudGetter.m_reference) {
                    getObject.SetCommandPrompt("Clouds to reference");
                    getObject.AddOption("Mode", "Reference");
                } else {
                    getObject.SetCommandPrompt("Clouds to copy");
                    getObject.AddOption("Mode", "Copy");
                }
                getObject.GeometryFilter=(ObjectType.PointSet);
                multiple = getObject.GetMultiple(1, 0);
                if ((ulong)multiple != (long)3) {
                    break;
                }
                GH_CloudGetter.m_reference = !GH_CloudGetter.m_reference;
            }
            if ((ulong)multiple == (long)12) {
                List<GH_Cloud> gHClouds1 = new List<GH_Cloud>();
                int objectCount = checked(getObject.ObjectCount - 1);
                int num = 0;
                do {
                    if (GH_CloudGetter.m_reference) {
                        gHClouds1.Add(new GH_Cloud(getObject.Object(num).ObjectId));
                    } else {
                        gHClouds1.Add(new GH_Cloud(getObject.Object(num).PointCloud()));
                    }
                    num = checked(num + 1);
                }
                while (num <= objectCount);
                gHClouds = gHClouds1;
            } else {
                gHClouds = null;
            }
            return gHClouds;
        }
    }
}
