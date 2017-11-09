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
        //public Vector Position { get; set; }
        public bool IsSelected { get; set; } = false;
        //public double Y { get { return Position.X; } }
        //public double X { get; private set; }
        public bool IsMy { get; }
        public double VisionRange { get; }
        public double AerialAtackRange { get; }
        public double GroundAttackRange { get; }
        public Vector Position { get; internal set; }

        //public double VisionRange { get; }

        public ActualUnit (Vehicle newVehicle) {
            UnitType = newVehicle.Type;
            Durability = newVehicle.Durability;
            Id = newVehicle.Id;
            Position = new Vector(newVehicle.X, newVehicle.Y);
            VisionRange = newVehicle.VisionRange;
            AerialAtackRange = newVehicle.AerialAttackRange;
            GroundAttackRange = newVehicle.GroundAttackRange;
            IsMy = newVehicle.PlayerId == 1;
        }

        internal void Update (VehicleUpdate update) {
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
