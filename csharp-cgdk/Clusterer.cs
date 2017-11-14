using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public class Cluster {
        private List<ActualUnit> units = new List<ActualUnit>();

        public double PositionX { get; private set; }
        public double PositionY { get; private set; }
        VehicleType ClusterType;
        public double Radius = 128;

        public Cluster (ActualUnit unit) {
            ClusterType = unit.UnitType;
            units.Add(unit);
            // Clip = new Clip(unit.Position.X, unit.Position.Y);
            Update();
        }

        public void Update () {

            //ln(x) * 2 + 2
            double x = 0, y = 0;

            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            units.ForEach(u => {
                x += u.Position.X;
                y += u.Position.Y;

                if (u.Position.X > maxX)
                    maxX = u.Position.X;
                if (u.Position.X < minX)
                    minX = u.Position.X;

                if (u.Position.Y > maxY)
                    maxY = u.Position.Y;
                if (u.Position.Y < minY)
                    minY = u.Position.Y;

            }
            );


            PositionX = minX + (maxX - minX) / 2;
            PositionY = minY + (maxY - minY) / 2;
        }

        internal bool IsNear (ActualUnit unit) {
            return unit.UnitType == ClusterType && unit.Position.DistanceTo(PositionX, PositionY) <= Radius;
        }

        internal void Add (ActualUnit unit) {
            units.Add(unit);
            Update();
        }

    }

    internal class Clusterer {

        internal static List<Cluster> Clusterize (Dictionary<long, ActualUnit> vehicles) {
            
            List<Cluster> clusters = new List<Cluster>();
            foreach (ActualUnit unit in vehicles.Values) {
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