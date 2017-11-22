using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    internal static class Calc {
        //static Dictionary<VectorSet, double> dic = new Dictionary<VectorSet, double>();

        internal static double Distance (Vector p1, Vector p2) {
                return p1.DistanceTo(p2);
        }

        internal static double Distance (Vector p1, int x2, int y2) {
            var p2 = new Vector(x2, y2);
            return Distance(p1, p2);
        }

        internal static double Distance (int x1, int y1, int x2, int y2) {
            var p2 = new Vector(x2, y2);
            var p1 = new Vector(x1, y1);
            return Distance(p1, p2);
        }
        internal static double Distance (double x1, double y1, double x2, double y2) {
            var p2 = new Vector(x2, y2);
            var p1 = new Vector(x1, y1);
            return Distance(p1, p2);
        }
        static HashSet<int> ids = new HashSet<int>();
        internal static int ClaimId () {
            int i = 1;
            while (ids.Contains(i)) {
                i++;
            }
            ids.Add(i);
            return i;
        }

        internal static void FreeId (int id) {
            ids.Remove(id);
        }

        internal static bool ShouldAvoid (Cluster subj, Cluster obj) {

            bool countLose = subj.Count < obj.Count;

            if (subj.ClusterType == VehicleType.Arrv)
                switch (obj.ClusterType) {
                    case VehicleType.Fighter: return false;
                    case VehicleType.Arrv: return false;
                    default:  return true;
                }


            if (subj.ClusterType == VehicleType.Fighter)
                switch (obj.ClusterType) {
                    case VehicleType.Arrv: return false;
                    case VehicleType.Helicopter: return false;
                    case VehicleType.Fighter: return countLose;
                    default: return true;
                }
            if (subj.ClusterType == VehicleType.Helicopter)
                switch (obj.ClusterType) {
                    case VehicleType.Tank: return countLose;
                    case VehicleType.Ifv: return true;
                    case VehicleType.Fighter: return true;
                    default: return false;
                }
            if (subj.ClusterType == VehicleType.Ifv)
                switch (obj.ClusterType) {
                    case VehicleType.Helicopter: return countLose;
                    case VehicleType.Fighter: return true;
                    case VehicleType.Ifv: return false;
                    default: return true;
                }
            if (subj.ClusterType == VehicleType.Tank) 
                switch (obj.ClusterType) {
                    case VehicleType.Tank: return countLose;
                    case VehicleType.Helicopter: return true;
                    default: return false;
                }
            

            return false;
        }

        internal static void ResetIds () {
            ids.Clear();
        }
    }


    //struct VectorSet {

    //    Vector p1;
    //    Vector p2;

    //    public VectorSet (Vector p1, Vector p2) {
    //        this.p1 = p1;
    //        this.p2 = p2;
    //    }

    //    public override int GetHashCode () {
    //      //  var res = new List<int>() { (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y }.OrderBy(p => p);
    //        return (int)(((long)p1.GetHashCode() + (long)p2.GetHashCode()) / 2);
    //    }
    //    public override bool Equals (object obj) {
    //        VectorSet s = (VectorSet)obj;
    //        return ((p1.Equals(s.p1) && p2.Equals(s.p2)) || (p1.Equals(s.p2) && p2.Equals(s.p1)));
    //    }
    //    public override string ToString () {
    //        var res = new List<string>() { p1.ToString(), p2.ToString() }.OrderBy(p => p);
    //        return string.Join("", res);
    //    }
    //}
}