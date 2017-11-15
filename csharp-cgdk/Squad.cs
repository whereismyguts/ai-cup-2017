using System;
using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public class Squad {

        VehicleType TargetRules (VehicleType me) {
            switch (me) {
                case VehicleType.Arrv: return VehicleType.Arrv;
                case VehicleType.Fighter: return VehicleType.Helicopter;
                case VehicleType.Helicopter: return VehicleType.Tank;
                case VehicleType.Ifv: return VehicleType.Fighter;
                case VehicleType.Tank: return VehicleType.Tank;
                default: return VehicleType.Arrv;
            }
        }

        // -- constants --
        float maxEnemyDistance = 350;
        int potentialCellCount = 8;
        float potentialRadius = 100;
        //---

        private Cluster cluster;
        Vector Position { get { return cluster.Position; } }

        public int Id { get; private set; }
        public bool IsSelected { get { return cluster.Units.FirstOrDefault(u => u.IsSelected) != null; } }

        public Cluster Cluster { get { return cluster; } }

        public Vector Goal { get; private set; }
        List<Cluster> Enemies { get { return MyStrategy.EnemyClusters; } }

        public Dictionary<IntVector, double> Potentials { get; private set; }

        public Squad (Cluster c) {
            cluster = c;
            Id = Calc.ClaimId();
        }

        internal void Step () {
            cluster.Update();
            Cluster target = FindTarget();
            bool enemyNear = false;

            Dictionary<IntVector, double> potentials = new Dictionary<IntVector, double>();

            double theta = -Math.PI;
            double step = Math.PI * 2 / potentialCellCount;


            while (theta < Math.PI) {
                int x = (int)(Position.X + potentialRadius * Math.Cos(theta));
                int y = (int)(Position.Y + potentialRadius * Math.Sin(theta));

                theta += step;

                if (x < 0 || y < 0 || x >= 1024 || y >= 1024)
                    continue;

                var cell = new IntVector(x, y);


                double cellDistToEnemyValue = 0;

                foreach (var e in Enemies)
                    if (e != target) {
                        double distanceToEnemy = Calc.Distance(e.Position, Position);
                        if (distanceToEnemy > maxEnemyDistance)
                            continue;

                        bool avoid = Calc.ShouldAvoid(cluster, e);

                        cellDistToEnemyValue += avoid ? Calc.Distance(e.Position, x, y) : 0;

                        enemyNear = true;
                    }
                if (cellDistToEnemyValue == 0)
                    continue;

                //      cellDistToEnemyValue /= Enemies.Count - 1;

                var cellTargetValue = 1024 - Calc.Distance(target.Position, x, y);

                if (potentials.ContainsKey(cell))
                    potentials[cell] += (cellDistToEnemyValue + cellTargetValue) / 2;
                else potentials[cell] = (cellDistToEnemyValue + cellTargetValue) / 2;
            }

            //enemyNear = true;
            Goal = potentials.Count > 0 ? potentials.OrderBy(p => p.Value).Last().Key.Vector : target.Position;
            Potentials = potentials;



            if (sinceLastShrink > 100 && Math.Max(cluster.MaxX - cluster.MinX, cluster.MaxY - cluster.MinY) > 150) {
                Commander.CommandShrink(this);
                sinceLastShrink = 0;
            }
            sinceLastShrink++;
            Commander.CommandMove(this); // TODO send vector

        }
        int sinceLastShrink = 0;

        private Cluster FindTarget () {
            return Enemies.FirstOrDefault(e => e.ClusterType == TargetRules(cluster.ClusterType)) ?? Enemies[0];
        }
    }


}