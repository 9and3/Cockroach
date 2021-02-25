using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CockroachGH {
    internal static class GH_CloudConvert {
        public static bool ToCloud(object data, ref PointCloud rpc) {
            bool flag;
            Guid guid = new Guid("3977cd01-cd46-3cff-82e5-83559c2e75c4");
            if (data != null) {
                Guid gUID = data.GetType().GUID;
                if (gUID == guid) {
                    rpc = (PointCloud)data;
                    flag = true;
                    return flag;
                } else if (gUID != GH_TypeLib.id_gh_curve) {
                    if (!(data is Curve)) {
                        flag = false;
                        return flag;
                    }
                    rpc = (PointCloud)data;
                    flag = true;
                    return flag;
                } else {
                    rpc = ((GH_Cloud)data).Value;
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        public static bool ToGHCloud(object data, GH_Conversion conversion_level, ref GH_Cloud target) {
            bool gHCloudPrimary;
            switch (conversion_level) {
                case GH_Conversion.Primary: 
                        gHCloudPrimary = GH_CloudConvert.ToGHCloud_Primary(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref target);
                        break;
                    
                case GH_Conversion.Secondary: 
                        gHCloudPrimary = GH_CloudConvert.ToGHCloud_Secondary(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref target);
                        break;
                    
                case GH_Conversion.Both: 
                        if (GH_CloudConvert.ToGHCloud_Primary(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref target)) {
                            gHCloudPrimary = true;
                            break;
                        } else {
                            gHCloudPrimary = GH_CloudConvert.ToGHCloud_Secondary(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref target);
                            break;
                        }

                    
                default: 
                        gHCloudPrimary = false;
                        break;
                    
            }
            return gHCloudPrimary;
        }

        public static bool ToGHCloud_Primary(object data, ref GH_Cloud target) {
            bool flag;
            Guid guid = new Guid("3977cd01-cd46-3cff-82e5-83559c2e75c4");

            if (data != null) {
                Guid gUID = data.GetType().GUID;
                if (gUID == guid) {
                    if (target != null) {
                        target.ReferenceID = Guid.Empty;
                        target.Value=((PointCloud)data);
                    } else {
                        target = new GH_Cloud((PointCloud)data);
                    }
                    flag = true;
                } else if (gUID != guid) {
                    flag = false;
                } else if (target != null) {
                    target.Value=(((GH_Cloud)data).Value);
                    target.ReferenceID = ((GH_Cloud)data).ReferenceID;
                    flag = true;
                } else {
                    target = (GH_Cloud)data;
                    flag = true;
                }
            } else {
                flag = false;
            }
            return flag;
        }

        public static bool ToGHCloud_Secondary(object data, ref GH_Cloud target) {
            bool isValid;
            Guid guid = new Guid();
            if (data == null) {
                isValid = false;
            } else if (!GH_Convert.ToGUID_Primary(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref guid)) {
                string str = null;
                if (GH_Convert.ToString_Primary(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref str)) {
                    RhinoObject rhinoObject = GH_Convert.FindRhinoObjectByNameAndType(str, ObjectType.PointSet);
                    if (rhinoObject == null) {
                        goto Label1;
                    }
                    if (target == null) {
                        target = new GH_Cloud();
                    }
                    target.ReferenceID = rhinoObject.Id;
                    target.ClearCaches();
                    target.LoadGeometry();
                    isValid = target.IsValid;
                    return isValid;
                }
            Label1:
                PointCloud pointCloud = null;
                if (GH_CloudConvert.ToCloud(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(data)), ref pointCloud)) {
                    if (target != null) {
                        target.Value=(pointCloud);
                        target.ReferenceID = Guid.Empty;
                    } else {
                        target = new GH_Cloud(pointCloud);
                    }
                    isValid = true;
                } else {
                    isValid = false;
                }
            } else {
                if (target != null) {
                    target.ReferenceID = guid;
                } else {
                    target = new GH_Cloud(guid);
                }
                target.ClearCaches();
                target.LoadGeometry();
                isValid = target.IsValid;
            }
            return isValid;
        }
    }
}
