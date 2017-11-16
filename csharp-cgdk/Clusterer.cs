using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public class Cluster {
        private List<ActualUnit> units = new List<ActualUnit>();

        public double X { get { return Position.X; } }
        public double Y { get { return Position.Y; } }
        public Vector Position { get; internal set; }
        public int Count { get { return count; } }

        public List<ActualUnit> Units { get { return units; } }

        public VehicleType Border { get; internal set; }
        public double MaxY { get; private set; }
        public double MinY { get; private set; }
        public double MaxX { get; private set; }
        public double MinX { get; private set; }

        public VehicleType ClusterType;
        public double Radius = 128;

        public Cluster (ActualUnit unit) {
            ClusterType = unit.UnitType;
            units.Add(unit);
            // Clip = new Clip(unit.Position.X, unit.Position.Y);
            Update();
        }



        int count = 0;
        public void Update () {

            //ln(x) * 2 + 2
            double x = 0, y = 0;

            MinX = double.MaxValue;
            MaxX = double.MinValue;
            MinY = double.MaxValue;
            MaxY = double.MinValue;

            count = 0;

            units.ForEach(u => {
                if (u.Durability != 0) {
                    x += u.Position.X;
                    y += u.Position.Y;

                    if (u.Position.X > MaxX)
                        MaxX = u.Position.X;
                    if (u.Position.X < MinX)
                        MinX = u.Position.X;

                    if (u.Position.Y > MaxY)
                        MaxY = u.Position.Y;
                    if (u.Position.Y < MinY)
                        MinY = u.Position.Y;
                    count++;
                }
            }
            );


            Position = new Vector(MinX + (MaxX - MinX) / 2, MinY + (MaxY - MinY) / 2);
        }

        internal bool IsNear (ActualUnit unit) {
            return unit.UnitType == ClusterType && unit.Position.DistanceTo(X, Y) <= Radius;
        }

        internal void Add (ActualUnit unit) {
            units.Add(unit);
            Update();
        }

    }

    internal class Clusterer {
        internal static List<Cluster> Clusterize (Dictionary<long, ActualUnit> vehicles) {
            List<Cluster> clusters = new List<Cluster>();
            foreach (ActualUnit unit in vehicles.Values) 
                if(unit.Durability>0)
                {
                bool newCluster = true;
                foreach (var c in clusters)
                    if (c.IsNear(unit)) {
                        c.Add(unit);
                        newCluster = false;
                        break;
                    }

                if (newCluster)
                    clusters.Add(new Cluster(unit));
            }
            return clusters;
        }
    }
}