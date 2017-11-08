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


        public void Move (Player me, World world, Game game, Move move) {
            InitializeTick(me, world, move);

            helics.Update(vehicles.Values.ToList());
            helicsPotentials = new float[32, 32];

          

            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++) {

                    double dDistance = helics.Position.SquareDistanceTo(i * 32, j * 32);
                    float dValue =  (float)(  dDistance/ 3000000);
                    float eValue = 0;

                    foreach (ActualUnit unit in enemies.Values) {
                        double eDistance = unit.Position.SquareDistanceTo(i*32,j*32);
                        if(eDistance<100000)
                        eValue += (float)(GetUnitTypeValue(unit.UnitType) * 3000f /  eDistance);
                    }

                    helicsPotentials[i, j] = dValue + eValue  ;
                }



            if (me.RemainingActionCooldownTicks > 0)
                return;


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


            int hX = (int)Math.Floor(helics.Position.X / 32);
            int hY = (int)Math.Floor(helics.Position.Y / 32);




            int bestX = 0;
            int bestY = 0;
            float bestValue = float.MinValue;



            for (int i = hX-5; i <= hX+5; i++)
                for (int j = hY-5; j <= hY+5; j++) 
                    
                    
                    if (i>=0 && j>=0 && bestValue < helicsPotentials[i, j]) {
                        bestValue = helicsPotentials[i, j];
                        bestX = i * 32;
                        bestY = j * 32;
                    
                }
            
            commander.Move(bestX-helics.Position.X,bestY - helics.Position.Y);

            commander.Step();

            Render.Update(vehicles.Values.ToList());
        }

        float GetUnitTypeValue (VehicleType type) {

            switch (type) {
                case VehicleType.Arrv: return 0.1f;
                case VehicleType.Fighter: return -0.3f;
                case VehicleType.Helicopter: return -0.2f;
                case VehicleType.Ifv: return -0.3f;
                case VehicleType.Tank: return 0.4f;
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

            commander.Update(world, move);

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