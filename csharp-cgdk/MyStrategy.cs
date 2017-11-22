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


        public static Player EnemyPlayer;
        public void Move (Player me, World world, Game game, Move move) {
            World = world;
            Game = game;
            EnemyPlayer = world.Players.First(p => !p.IsMe);
            InitializeTick(me, move);
            Commander.Step();
        }

        void InitializeTick (Player me,  Move move) {
            if (World.TickIndex == 0) {
                TerranTypes = new Dictionary<TerrainType, double>();
                TerranTypes.Add(TerrainType.Forest, Game.ForestTerrainVisionFactor);
                TerranTypes.Add(TerrainType.Plain, Game.PlainTerrainVisionFactor);
                TerranTypes.Add(TerrainType.Swamp, Game.SwampTerrainVisionFactor);

                foreach (Vehicle v in World.NewVehicles)
                    if (v.PlayerId == me.Id)
                        vehicles.Add(v.Id, new ActualUnit(v));
                    else
                        enemies.Add(v.Id, new ActualUnit(v));

                UpdateUnits(World);

                var friendlyClusters = Clusterer.Clusterize(vehicles);
                EnemyClusters = Clusterer.Clusterize(enemies);
                squads.Clear();
                Calc.ResetIds();
                friendlyClusters.ForEach(c => squads.Add(new Squad(c, friendlyClusters)));
                Commander.Reassign(squads);
            }
            else {
                UpdateUnits(World);
                EnemyClusters.ForEach(c => c.Update());
            }
            squads.RemoveAll(s => s.Cluster.Count == 0);
            squads.ForEach(s => s.Step());
            Commander.Update(World, move, me, squads);
            // Render.Update(enemies, vehicles, squads, EnemyClusters, Commander.Commands);

            if (World.TickIndex % 200 == 0 && World.TickIndex > 0)
                EnemyClusters = Clusterer.Clusterize(enemies);

            Nuclear.Step(move, World, me, Game, squads, EnemyClusters);
        }

        List<Squad> squads = new List<Squad>();

        public static List<Cluster> EnemyClusters { get; set; }
        public static TerrainType[][] Terrains { get; private set; }
        public static Dictionary<TerrainType, double> TerranTypes { get; private set; }
        public static World World { get; private set; }
        public static Game Game { get; private set; }

        private void UpdateUnits (World world) {
            Terrains = world.TerrainByCellXY;
            if (world.VehicleUpdates.Length > 0) {
                foreach (var update in world.VehicleUpdates) {

                    //if (update.Durability == 0) {
                    //    if (vehicles.ContainsKey(update.Id))
                    //        vehicles.Remove(update.Id);
                    //    else
                    //            if (enemies.ContainsKey(update.Id))
                    //        enemies.Remove(update.Id);
                    //    continue;
                    //}
                    if (vehicles.ContainsKey(update.Id))
                        vehicles[update.Id].Update(update);
                    else
                    if (enemies.ContainsKey(update.Id))
                        enemies[update.Id].Update(update);
                }
            }
        }
    }
}