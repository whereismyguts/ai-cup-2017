using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System;
using static Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.MyStrategy;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public abstract class CombatGroup {
        protected Commander Commander { get; }
        public abstract VehicleType GroupType { get;}
        public Vector Position { get; internal set; }

        public CombatGroup (Commander commander) {
            this.Commander = commander;
        }

        public virtual void Step (int tick) {

        }


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

        public override void Step (int tick) {
            if (tick == 1) {
                Commander.Select(VehicleType.Helicopter);
                Commander.Assign((int)CombatGroupId.Helicopters);
            }

            if (tick > 5 && tick % 50 == 0) {
                var alpha = rnd.NextDouble()* Math.PI * 2;

                var x = 500 * Math.Cos(alpha);
                var y = 500 * Math.Sin(alpha);
                //              x = r × cos(θ)
                //y = r × sin(θ)
                Commander.Move(x,y);
            }
            

            base.Step(tick);
        }
    }

}