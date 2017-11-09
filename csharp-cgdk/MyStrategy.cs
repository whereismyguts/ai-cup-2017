using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public sealed class MyStrategy: IStrategy {

        public enum CombatGroupId {
            RepareUnits = 1,
            Fighters = 2,
            Helicopters = 3,
            Missiles = 4,
            Tanks = 5
        }

        Dictionary<long, ActualUnit> vehicles = new Dictionary<long, ActualUnit>();
        Dictionary<long, ActualUnit> enemies = new Dictionary<long, ActualUnit>();

        Commander commander = new Commander();

        CombatGroup helics;
        Random rnd = new Random();

        float[,] helicsPotentials = new float[8, 8];

        public  const int CellCount = 16;
        public static int CellWidth = 1024 / CellCount;

        public void Move (Player me, World world, Game game, Move move) {
             
            InitializeTick(me, world, move);

            helics.Update(vehicles.Values.ToList());
            helics.Step(enemies);
            /*
            helicsPotentials = new float[CellCount, CellCount];



            


            for (int i = 0; i < CellCount; i++)
                for (int j = 0; j < CellCount; j++) {


                    var x = Math.Abs( (i- CellCount/2f) / (CellCount/2f));
                    var y = Math.Abs((j - CellCount/2f) / (CellCount/2f));
                    //double dDistance = helics.Position.SquareDistanceTo(i * 32, j * 32);
                    var dValue = 1-(float)Math.Max(x, y)/10;
                    //float dValue = (float)(dDistance/40000000);
                    float eValue = 0;

                    foreach (ActualUnit unit in enemies.Values.Where(u=>u.Durability>0)) {
                        double eDistance = unit.Position.SquareDistanceTo(i * CellWidth, j * CellWidth);
                        if (eDistance < 20000 && eDistance>1)
                            eValue += (float)(GetUnitTypeValue(unit) * 1500f / eDistance);
                    }

                    foreach (ActualUnit unit in vehicles.Values.Where(u => u.Durability > 0)) {
                        double eDistance = unit.Position.SquareDistanceTo(i * CellWidth, j * CellWidth);
                        if (eDistance < 20000 && eDistance > 1)
                            eValue += (float)(GetUnitTypeValue(unit) * 1500f / eDistance);
                    }

                    helicsPotentials[i, j] = Math.Min( Math.Max(dValue+  eValue,-150), 150);
                }



            //  if (me.RemainingActionCooldownTicks > 0)
            //          return;


            //if(world.TickIndex == 0) {
            //    move.Action = ActionType.ClearAndSelect;
            //    move.Right = world.Width;
            //    move.Bottom = world.Height;
            //    return;
            //}

            //if(world.TickIndex == 1) {
            //    move.Action = ActionType.Move;
            //    move.X = 0;// world.Width / 2.0D;
            //    move.Y = 0;// world.Height / 2.0D;
            //}

            // helics.Step(world.TickIndex);

            if (!helics.IsAlive)
                return;
            int hX = (int)Math.Floor(helics.Position.X / CellWidth);
            int hY = (int)Math.Floor(helics.Position.Y / CellWidth);




            int bestX = 0;
            int bestY = 0;
            float bestValue = float.MinValue;



            for (int i = hX - 1; i <= hX + 1; i++)
                for (int j = hY - 1; j <= hY + 1; j++)


                    if (i >= 0 && j >= 0 && i < CellCount && j < CellCount && bestValue < helicsPotentials[i, j]) {
                        bestValue = helicsPotentials[i, j];
                        bestX = i * CellWidth;
                        bestY = j * CellWidth;

                    }

            helics.Goal = new Vector(bestX, bestY);
            

            commander.Move(bestX - helics.Position.X, bestY - helics.Position.Y);
            */
            commander.Step();

            

            Render.Update(enemies, vehicles, helics);
        }

        float GetUnitTypeValue (ActualUnit unit) {
            if (unit.Durability == 0)
                return 0;
            if (unit.IsMy) {
                if (unit.UnitType == VehicleType.Fighter) 
                   return -0.3f;
                return 0;
            }

            switch (unit.UnitType) {
                case VehicleType.Arrv: return 0.001f;
                case VehicleType.Fighter: return -0.1f;
                case VehicleType.Helicopter: return -0.1f;
                case VehicleType.Ifv: return -0.1f;
                case VehicleType.Tank: return 0.02f;
                default: return 0;
            }
        }

        private void InitializeTick (Player me, World world, Move move) {
            if (world.TickIndex == 0) {
                foreach (Vehicle v in world.NewVehicles)
                    if (v.PlayerId == 1)
                        vehicles.Add(v.Id, new ActualUnit(v));
                    else
                        enemies.Add(v.Id, new ActualUnit(v));
                helics = new Helicopters(commander);

                commander.Select(VehicleType.Helicopter);
                commander.Assign(3);

                new System.Threading.Tasks.Task(() => { Render.Run(); }).Start();


            }

            commander.Update(world, move, me);

            if (world.VehicleUpdates.Length > 0) {
                foreach (var update in world.VehicleUpdates)
                    if (vehicles.ContainsKey(update.Id))
                        vehicles[update.Id].Update(update);
                    else
                        if (enemies.ContainsKey(update.Id))
                        enemies[update.Id].Update(update);
                //else
            }
        }
    }
}