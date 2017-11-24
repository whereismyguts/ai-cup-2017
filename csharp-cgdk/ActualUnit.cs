using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public class ActualUnit {
        public VehicleType UnitType { get; }
        public int Durability { get; set; }
        public int[] Groups { get; set; }

        public long Id { get; }
        public bool IsSelected { get; set; } = false;

        public double VisionRange { get; private set; }
        double visionRange;
        double visionFactor = 1;

        public double AerialAtackRange { get; }
        public double GroundAttackRange { get; }
        public Vector Position { get; internal set; }
        public double WorkDistance {
            get {
                switch (UnitType) {
                    case VehicleType.Fighter: return 100;
                    case VehicleType.Helicopter: return 50;
                    default: return 60;
                }
            }
        }

        public long PlayerId { get; private set; }
        public double MainAttackRange {
            get {
                switch (UnitType) {
                    case VehicleType.Fighter: return AerialAtackRange;
                    case VehicleType.Helicopter: return GroundAttackRange;
                    case VehicleType.Ifv: return AerialAtackRange;
                    case VehicleType.Tank: return GroundAttackRange;
                    default: return 0;

                }
            }
        }

        public ActualUnit (Vehicle newVehicle) {
            UnitType = newVehicle.Type;
            Durability = newVehicle.Durability;
            Id = newVehicle.Id;
            Position = new Vector(newVehicle.X, newVehicle.Y);
            visionRange = newVehicle.VisionRange;
            AerialAtackRange = newVehicle.AerialAttackRange;
            GroundAttackRange = newVehicle.GroundAttackRange;
            PlayerId = newVehicle.PlayerId;
            Groups = new int[] { };
        }

        internal void Update (VehicleUpdate update) {
            if (Position.X != update.X && Position.Y != update.Y) {
                visionFactor = MyStrategy.TerranTypes[MyStrategy.Terrains[(int)update.X / 32][(int)update.Y / 32]];
                VisionRange = visionRange * visionFactor;
            }
            Position = new Vector(update.X, update.Y);
            Durability = update.Durability;
            Groups = update.Groups;
            IsSelected = update.IsSelected;
        }

        public override string ToString () {
            return string.Format("{0} {1}:{2}", UnitType.ToString(), Position.X.ToString("f1"), Position.Y.ToString("f1"));
        }
    }
}
