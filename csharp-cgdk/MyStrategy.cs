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

        List<CombatGroup> groups = new List<CombatGroup>();
        Random rnd = new Random();

        float[,] helicsPotentials = new float[8, 8];

        public const int CellCount = 16;
        public static int CellWidth = 1024 / CellCount;

        public void Move (Player me, World world, Game game, Move move) {

            InitializeTick(me, world, move);


            if (world.TickIndex > 0 && world.TickIndex % 20 == 0)
                foreach (var g in groups) {
                    g.Update(vehicles.Values.ToList());
                    g.Step(enemies);
                }
            commander.Step();
               Render.Update(enemies, vehicles, groups);
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
                //TODO clustering 
                //  groups.Add(new HelicoptersGroup(commander, (int)VehicleType.Helicopter));
                groups.Add(new FightersGroup(commander, (int)VehicleType.Fighter, VehicleType.Fighter));

                new System.Threading.Tasks.Task(() => { Render.Run(); }).Start();
            }

            commander.Update(world, move, me);

            if (world.VehicleUpdates.Length > 0) {
                foreach (var update in world.VehicleUpdates) {

                    if (update.Durability == 0) {
                        if (vehicles.ContainsKey(update.Id))
                            vehicles.Remove(update.Id);
                        else
                                if (enemies.ContainsKey(update.Id))
                            enemies.Remove(update.Id);

                        continue;
                    }

                    if (vehicles.ContainsKey(update.Id))
                        vehicles[update.Id].Update(update);
                    else
                    if (enemies.ContainsKey(update.Id))
                        enemies[update.Id].Update(update);
                }
                //else
            }
        }
    }
}