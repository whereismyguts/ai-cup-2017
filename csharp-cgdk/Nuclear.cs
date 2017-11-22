using System;
using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {

    class NuclearTargetInfo {
        public double X;
        public double Y;

        public NuclearTargetInfo (double x, double y, int count) {
            this.X = x;
            this.Y = y;
            Count = count;
        }

        public NuclearTargetInfo () {
            Count = 0;
        }

        public int Count { get; set; }
        public bool IsIdle { get { return idleTick > 20; } }

        int idleTick = 0;

        internal void Update (double x, double y) {
            if (Math.Abs(X - x) < 0.03 && Math.Abs(Y - y) < 0.03)
                idleTick++;
            else
                idleTick = 0;
            X = x; Y = y;
        }
    }

    public class Nuclear {
        static Dictionary<int, NuclearTargetInfo> info = new Dictionary<int, NuclearTargetInfo>();
        internal static void Step (Move move, World world, Player me, Game game, List<Squad> squads, List<Cluster> enemyClusters) {
            if (info == null)
                info = new Dictionary<int, NuclearTargetInfo>();
            foreach (Cluster e in enemyClusters) {
                if (info.ContainsKey(e.Id))
                    info[e.Id].Update(e.X, e.Y);
                else
                    info.Add(e.Id, new NuclearTargetInfo(e.X, e.Y, e.Count));

                if (me.RemainingNuclearStrikeCooldownTicks > 0)
                    return;

                var eInfo = info[e.Id];

                if (!eInfo.IsIdle)
                    continue;
                double minDistanceFromEnemyCluster = MinDistance(e.X, e.Y, squads);
                if (minDistanceFromEnemyCluster < game.TacticalNuclearStrikeRadius)
                    continue;

                foreach (Squad squad in squads)
                    foreach (ActualUnit unit in squad.Cluster.Units) {
                        var distancefromUnitToCluster = e.Position.DistanceTo(unit.Position);
                        if ( distancefromUnitToCluster < unit.VisionRange) {
                            Commander.CommandStrike(e, unit);
                            return;
                        }
                    }
            }
        }
        static double MinDistance (double x, double y, List<Squad> squads) {
            double result = double.MaxValue;
            foreach (var s in squads) {
                var d = s.Cluster.Position.DistanceTo(x, y);
                if (d < result)
                    result = d;
            }
            return result;
        }

        internal static void ClearInfo () {
            info.Clear();
        }
    }
}