using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using static Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.MyStrategy;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public abstract class CombatGroup {
        protected Commander Commander { get; }
        public VehicleType GroupType { get; protected set; }
        public Vector Position { get; internal set; }
        public int Id { get; }
        public Vector Goal { get; internal set; }
        public Dictionary<IntVector, double> Potentials { get; private set; } = new Dictionary<IntVector, double>();
        public int Count { get; internal set; }
        public CombatGroup (Commander commander, int id, VehicleType type) {
            this.Commander = commander;
            Id = id;
            GroupType = type;
            commander.Select(type);
            commander.Assign(id);
        }

        public virtual void Step (Dictionary<long, ActualUnit> enemies) {
            Potentials = new Dictionary<IntVector, double>();

            for (int i = (int)Position.X - CellWidth * 2; i <= (int)Position.X + CellWidth * 2; i += CellWidth)
                for (int j = (int)Position.Y - CellWidth * 2; j <= (int)Position.Y + CellWidth * 2; j += CellWidth) {

                    if (i < 1 || j < 1 || i > 1022 || j > 1022)
                        continue;

                    double eValue = 1;

                    double dValue = (Math.Min(Math.Abs(i - Position.X) / 1024f, Math.Abs(j - Position.Y) / 1024f) + 1-Math.Min(Math.Abs(i - 512) / 512f, Math.Abs(j - 512) / 512f))/2f;

                    foreach (var e in enemies) {
                        var distanceToEnemy = e.Value.Position.DistanceTo(i, j);
                        if (distanceToEnemy <= e.Value.WorkDistance && distanceToEnemy > 1) {
                            var newValue = distanceToEnemy / e.Value.WorkDistance * GetUnitDanger(e.Value);
                            if (newValue > 0 && newValue < eValue)
                                eValue = newValue;
                        }
                    }
                    Potentials[new IntVector(i, j)] = eValue + dValue ;
                }

            var best = Potentials.OrderBy(p => p.Value).Last().Key;
            Goal = new Vector(best.X, best.Y);
            Commander.Move(best.X - Position.X, best.Y - Position.Y, Id);
        }

        protected virtual double GetUnitDanger (ActualUnit value) {
            return 0;
        }

        internal void Update (List<ActualUnit> unitList) {
            double x = 0;
            double y = 0;

            Count = 0;

            unitList.ForEach(u => {
                if (u.UnitType == GroupType && u.Durability > 0) {
                    x += u.Position.X;
                    y += u.Position.Y;
                    Count++;
                }
            });
            Position = Count > 0? new Vector(x / Count, y / Count) : new Vector();
        }
    }


    public class FightersGroup: CombatGroup {
        public FightersGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }


        protected override double GetUnitDanger (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return -1;
                case VehicleType.Helicopter: return -1;
                case VehicleType.Ifv: return 1;
                case VehicleType.Tank: return 0;
                default: return 0;
            }
        }
    }

    public class HelicoptersGroup: CombatGroup {
        public HelicoptersGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }

        protected override double GetUnitDanger (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return 1;
                case VehicleType.Helicopter: return 1;
                case VehicleType.Ifv: return 1;
                case VehicleType.Tank: return -1;
                default: return 0;
            }
        }
    }

}