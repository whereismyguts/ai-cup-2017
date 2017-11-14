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

        public virtual void Step (Dictionary<long, ActualUnit> enemies, Dictionary<long, ActualUnit> vehicles) {
            Potentials = new Dictionary<IntVector, double>();
            var dValues = new Dictionary<IntVector, double>();

          //  bool includeDistanceValue = false;

            for (int i = (int)Position.X - CellWidth*3; i <= (int)Position.X + CellWidth*3; i += CellWidth)
                for (int j = (int)Position.Y - CellWidth*3; j <= (int)Position.Y + CellWidth*3; j += CellWidth) {

                    if (i < 1 || j < 1 || i > 1022 || j > 1022 || Position.DistanceTo(i,j) > CellWidth*3.5)
                        continue;

                    double eValue = 0;

                    var cell = new IntVector(i, j);

                    dValues[cell]  = (Math.Min(Math.Abs(i - Position.X) / 1024f, Math.Abs(j - Position.Y) / 1024f) + 1 - Math.Min(Math.Abs(i - 512) / 512f, Math.Abs(j - 512) / 512f)) / 5f;

                    foreach (var e in enemies) {
                        var distanceToEnemy = e.Value.Position.DistanceTo(i, j);
                
                            var newValue =( 100/ distanceToEnemy) * GetEnemyValue(e.Value);
                            if (Math.Abs(newValue) > 0 && Math.Abs(newValue) > Math.Abs(eValue)) 
                                eValue = newValue;
                    }
                    foreach (var v in vehicles) {
                        if (v.Value.UnitType == GroupType)
                            continue;
                        var distance = v.Value.Position.DistanceTo(i, j);
                        if (distance <= v.Value.WorkDistance  && distance > 0) {
                            var newValue = v.Value.WorkDistance/distance  * -GetFriendUnitDanger(v.Value) ;
                              if (Math.Abs(newValue) > 0 &&  Math.Abs(newValue) > Math.Abs(eValue)) 
                            eValue = newValue;
                        }
                    }

                    Potentials[cell] = eValue  ;
                }

    //        if (includeDistanceValue)
    //            foreach (var dValue in dValues)
      //              Potentials[dValue.Key] += dValue.Value;

            var best = Potentials.OrderBy(p => 
                p.Value + (1- Vector.MinAngle(p.Key, direction)/Math.PI) 
              ).Last().Key ;

            Goal = new Vector(best.X, best.Y);
            direction = new Vector(best.X - Position.X, best.Y - Position.Y);
            Commander.Move(direction.X, direction.Y, Id); // TODO send vector
        }
        Vector direction;
        protected virtual double GetFriendUnitDanger (ActualUnit unit) {
            return 0;
        }

        protected virtual double GetEnemyValue (ActualUnit unit) {
            return 0;
        }

        internal void Update (Dictionary<long, ActualUnit> unitList) {
            double x = 0;
            double y = 0;


            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            Count = 0;

            foreach (var u in unitList.Values)
                if (u.UnitType == GroupType && u.Durability > 0) {
                    if (u.Position.X > maxX)
                        maxX = u.Position.X;
                     if (u.Position.X < minX)
                        minX = u.Position.X;

                    if (u.Position.Y > maxY)
                        maxY = u.Position.Y;
                     if (u.Position.Y < minY)
                        minY = u.Position.Y;

                    x += u.Position.X;
                    y += u.Position.Y;
                    Count++;
                }

            Position = Count > 0 ? new Vector(x / Count, y / Count) : new Vector();

            if (TimeSinceLastScaling > 30 && (maxX - minX < 150 || maxY - minY < 150)){
                Commander.Shrink(Id, Position, 0.5);
                TimeSinceLastScaling = 0;
            }

            TimeSinceLastScaling++;
        }

        int TimeSinceLastScaling = 0;
    }


    public class FightersGroup: CombatGroup {
        public FightersGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }
        protected override double GetFriendUnitDanger (ActualUnit unit) {
            switch (unit.UnitType) {
                case VehicleType.Fighter: return 0;
                case VehicleType.Helicopter: return 0.5;
                case VehicleType.Arrv: return -0.3;
                default: return 0;
            }
        }
        protected override double GetEnemyValue (ActualUnit unit) {
            switch (unit.UnitType) {
                case VehicleType.Fighter: return 0.5;
                case VehicleType.Helicopter: return 1;
                case VehicleType.Ifv: return -1;
                case VehicleType.Tank: return 0;
                default: return 0;
            }
        }
    }

    public class HelicoptersGroup: CombatGroup {
        public HelicoptersGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }
        protected override double GetFriendUnitDanger (ActualUnit unit) {
            switch (unit.UnitType) {
                case VehicleType.Fighter: return 0.5;
                case VehicleType.Helicopter: return 0;
                default: return -0.5;
            }
        }
        protected override double GetEnemyValue (ActualUnit unit) {
            switch (unit.UnitType) {
                case VehicleType.Fighter: return 1;
                case VehicleType.Helicopter: return 1;
                case VehicleType.Ifv: return 1;
                case VehicleType.Tank: return -1;
                default: return 0;
            }
        }
    }

    public class IfvGroup: CombatGroup {
        public IfvGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }

        protected override double GetEnemyValue (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return 1;
                case VehicleType.Helicopter: return 1;
                default: return -1;
            }
        }
        protected override double GetFriendUnitDanger (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return 0;
                case VehicleType.Helicopter: return 0;
                default: return 1;
            }
        }
    }

    public class ArrvGroup: CombatGroup {
        public ArrvGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }

        protected override double GetEnemyValue (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return 0;
                default: return -0.1;
            }
        }
        protected override double GetFriendUnitDanger (ActualUnit value) {
                 return -1;
        }
    }

    public class TankGroup: CombatGroup {
        public TankGroup (Commander commander, int id, VehicleType type) : base(commander, id, type) {
        }

        protected override double GetEnemyValue (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return 0;
                case VehicleType.Helicopter: return -1;
                default: return 1;
            }
        }
        protected override double GetFriendUnitDanger (ActualUnit value) {
            switch (value.UnitType) {
                case VehicleType.Fighter: return 0;
                case VehicleType.Helicopter: return 0;
                default: return 1;
            }
        }
    }
}