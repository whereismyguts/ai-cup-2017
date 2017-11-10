using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using static Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.MyStrategy;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public abstract class CombatGroup {
        protected Commander Commander { get; }
        public abstract VehicleType GroupType { get; }
        public Vector Position { get; internal set; }
        public bool IsAlive { get; private set; }
        public Vector Goal { get; internal set; }
        public Dictionary<IntVector, double> Potentials { get; private set; } = new Dictionary<IntVector, double>();
        public CombatGroup (Commander commander) {
            this.Commander = commander;
        }

        public virtual void Step (Dictionary<long, ActualUnit> enemies) {
            Potentials = new Dictionary<IntVector, double>();

            for (int i = (int)Position.X - 100; i <= (int)Position.X + 100; i += 50)
                for (int j = (int)Position.Y - 100; j <= (int)Position.Y + 100; j += 50) {

                    if (i < 1 || j < 1 || i > 1022 || j > 1022)
                        continue;

                    double eValue = 0;

                    foreach (var u in enemies.Values.Where(u => u.Durability > 0)) {
                        eValue += GetUnitValue(u, i,j);
                    }

                    Potentials[new IntVector(i,j)] = eValue;
                }
            var best = Potentials.OrderBy(p => p.Value).Last().Key;
            Goal = new Vector(best.X, best.Y);
            Commander.Move(best.X- Position.X , best.Y- Position. Y);
        }

        public abstract double GetUnitValue (ActualUnit u, int i, int j);

        internal void Update (List<ActualUnit> unitList) {
            double x = 0;
            double y = 0;

            int count = 0;

            unitList.ForEach(u => {
                if (u.UnitType == GroupType && u.Durability > 0) {
                    x += u.Position.X;
                    y += u.Position.Y;
                    count++;
                }
            });

            IsAlive = count > 0;
            Position = new Vector(x / count, y / count);
        }
    }

    public class Helicopters: CombatGroup {
        public Helicopters (Commander commander) : base(commander) {
        }

        Random rnd = new Random();

        public override VehicleType GroupType {
            get {
                return VehicleType.Helicopter;
            }
        }



        public override double GetUnitValue(ActualUnit unit,int i,int j ) {


            if (unit.Durability == 0)
                return 0;

            //if (unit.IsMy) {
            //    if (unit.UnitType == VehicleType.Fighter)
            //        return -0.3f;
            //    return 0;
            //}

            var dist = unit.Position.SquareDistanceTo(i, j);

            var value =  GetValueByType(unit);


            return dist*value /100000f;
        }

        private static double GetValueByType (ActualUnit unit) {
            switch (unit.UnitType) {
                case VehicleType.Arrv: return 0.001f;
                case VehicleType.Fighter: return -0.1f;
                case VehicleType.Helicopter: return -0.1f;
                case VehicleType.Ifv: return -0.1f;
                case VehicleType.Tank: return 0.02f;
                default: return 0;
            }
        }
    }

}