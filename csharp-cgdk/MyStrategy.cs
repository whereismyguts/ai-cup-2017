using System.Collections.Generic;
using System.Linq;
using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

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

        //Commander commander = new Commander();

        //List<CombatGroup> groups = new List<CombatGroup>();
        Random rnd = new Random();

        float[,] helicsPotentials = new float[8, 8];

        public const int CellCount = 16;
        public static int CellWidth = 1024 / CellCount;

        public void Move (Player me, World world, Game game, Move move) {
            InitializeTick(me, world, move);

            //   if ( world.TickIndex % 2 == 0) {
            Commander.Step();
            //    }
        }

        void InitializeTick (Player me, World world, Move move) {
            if (world.TickIndex == 0) {
                foreach (Vehicle v in world.NewVehicles)
                    if (v.PlayerId == 1)
                        vehicles.Add(v.Id, new ActualUnit(v));
                    else
                        enemies.Add(v.Id, new ActualUnit(v));

                UpdateUnits(world);

                var friendlyClusters = Clusterer.Clusterize(vehicles);
                EnemyClusters = Clusterer.Clusterize(enemies);
                squads.Clear();
                Calc.ResetIds();
                friendlyClusters.ForEach(c => squads.Add(new Squad(c)));
                Commander.Reassign(squads);
                new System.Threading.Tasks.Task(() => { Render.Run(); }).Start();
            }
            //   var clusters = Clusterer.Clusterize(vehicles);
            else {
                UpdateUnits(world);
                EnemyClusters.ForEach(c => c.Update());
            }
            squads[0].Step();
            Commander.Update(world, move, me, squads);
            Render.Update(enemies, vehicles, squads, EnemyClusters, Commander.Commands);
        }

        List<Squad> squads = new List<Squad>();

        public static List<Cluster> EnemyClusters { get; set; }
        //public static List<Cluster> FriendlyClusters { get; set; }

        private void UpdateUnits (World world) {
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