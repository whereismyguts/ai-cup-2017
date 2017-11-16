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
        float maxEnemyDistance = 200;
        int potentialCellCount = 16;
        float potentialRadius = 200;
        float maxFriendDistance = 100;
        //---

        private Cluster cluster;
        Vector Position { get { return cluster.Position; } }

        public int Id { get; private set; }
        public bool IsSelected { get { return cluster.Units.FirstOrDefault(u => u.IsSelected) != null; } }

        public Cluster Cluster { get { return cluster; } }

        public Vector Goal { get; private set; }
        List<Cluster> Enemies { get { return MyStrategy.EnemyClusters; } }
        List<Cluster> friends;

        public Dictionary<IntVector, double> Potentials { get; private set; }

        public Squad (Cluster c, List<Cluster> friends) {
            this.friends = friends;
            cluster = c;
            Id = Calc.ClaimId();
        }

        internal void Step () {

            cluster.Update();
            Cluster target = FindTarget();
            //bool enemyNear = false;
            if (cluster.ClusterType != VehicleType.Arrv) {
                Dictionary<IntVector, double> potentials = new Dictionary<IntVector, double>();



                int count = potentialCellCount;

                for (double r = potentialRadius; r > 0; r -= potentialRadius / 2) {
                    double theta = -Math.PI;
                    double step = Math.PI * 2 / count;

                    while (theta < Math.PI) {
                        int x = (int)(Position.X + r * Math.Cos(theta));
                        int y = (int)(Position.Y + r * Math.Sin(theta));

                        theta += step;

                        if (x < 0 || y < 0 || x >= 1024 || y >= 1024)
                            continue;

                        var cell = new IntVector(x, y);

                        bool enemyNear = false;
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

                        if (!enemyNear)
                            foreach (var f in friends)
                                if (f != cluster) {
                                    double distanceToFriend = Calc.Distance(f.Position, Position);
                                    if (distanceToFriend > maxFriendDistance)
                                        continue;

                                    cellDistToEnemyValue += Calc.Distance(f.Position, x, y);
                                }

                        for (int i = 0; i <= 1024; i += 256)
                            for (int j = 0; j <= 1024; j += 256)
                                if (i == 0 || j == 0 || i == 1024 || j == 1024) {
                                    var d = Calc.Distance(x, y, i, j);
                                    if (d < 300)
                                        cellDistToEnemyValue += Calc.Distance(x, y, i, j) / 10;

                                }


                        //if (cellDistToEnemyValue == 0)
                        //    continue;

                        //      cellDistToEnemyValue /= Enemies.Count - 1;

                        var cellTargetValue = target == null ? 0 : (1500 - Calc.Distance(target.Position, x, y)) * 1.5;

                        //   if (potentials.ContainsKey(cell))
                        //      potentials[cell] += (cellDistToEnemyValue + cellTargetValue) ;
                        //                else 
                        potentials[cell] = cellDistToEnemyValue + cellTargetValue;
                    }
                    count /= 2;
                }

                //enemyNear = true;
                Goal = potentials.Count > 0 ? potentials.OrderBy(p => p.Value).Last().Key.Vector : target!=null? target.Position : FollowFriend();
                Potentials = potentials;
            }
            else {

                Goal = FollowFriend();

            }

            if (sinceLastShrink > 100 && Math.Max(cluster.MaxX - cluster.MinX, cluster.MaxY - cluster.MinY) > 100) {
                Commander.CommandShrink(this);
                sinceLastShrink = 0;
            }
            sinceLastShrink++;
            Commander.CommandMove(this); // TODO send vector

        }

        private Vector FollowFriend () {
            Vector goal = new Vector(512,512);
            double minDist = double.MaxValue;
            foreach (var f in friends) {
                if (f != cluster && Calc.Distance(f.Position, Position) < minDist) {
                    minDist = Calc.Distance(f.Position, Position);
                    goal = f.Position;
                }
            }
            return goal;
        }

        int sinceLastShrink = 0;

        private Cluster FindTarget () {

            if (cluster.ClusterType == VehicleType.Arrv)
                return null;

            double maxValue = 0;
            int index = -1;
            for (int i = 0; i < Enemies.Count; i++)
                if (Enemies[i].Count > 0) {
                    var attackValue = AttackValue(Enemies[i]);
                    if (attackValue == 0)
                        continue;
                    var value = (1 / Calc.Distance(Enemies[i].Position, Position) + (Enemies[i].Count <= cluster.Count ? 1 : 0)) + 10 * attackValue;
                    if (value > maxValue) {
                        index = i;
                        maxValue = value;
                    }
                }
            return index > -1 ? Enemies[index] : null;
        }

        List<VehicleType> fTarget = new List<VehicleType>() { VehicleType.Helicopter, VehicleType.Fighter };
        List<VehicleType> hTarget = new List<VehicleType>() { VehicleType.Tank, VehicleType.Arrv };
        List<VehicleType> iTarget = new List<VehicleType>() { VehicleType.Fighter, VehicleType.Helicopter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank };
        List<VehicleType> tTarget = new List<VehicleType>() { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv, VehicleType.Helicopter, VehicleType.Fighter };

        private int AttackValue (Cluster e) {
            List<VehicleType> targetRule =
            cluster.ClusterType == VehicleType.Fighter ? fTarget :
            cluster.ClusterType == VehicleType.Helicopter ? hTarget :
            cluster.ClusterType == VehicleType.Ifv ? iTarget :
            cluster.ClusterType == VehicleType.Tank ? tTarget : new List<VehicleType>();

            if (targetRule.Contains(e.ClusterType))
                return 5 - targetRule.IndexOf(e.ClusterType);
            return 0;
        }
    }
}