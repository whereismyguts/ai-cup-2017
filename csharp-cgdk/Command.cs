using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public static class Commander {

        static World World;
        static Move Move;
        static Player Me;

        static List<Command> commands = new List<Command>();

        internal static void Update (World world, Move move, Player me, List<Squad> squads) {
            Move = move;
            World = world;
            Me = me;

            foreach (Squad s in squads)
                if (s.IsSelected) {
                    selectedSquad = s.Id;
                    break;
                }
        }

        static public void Step () {
            if (Me.RemainingActionCooldownTicks == 0) {
                ExecuteNextCommand();

            }
        }

        static void ExecuteNextCommand () {
            if (commands.Count > 0) {
                commands[0].Execute(Move);
                if (commands[0].CanRemove)
                    commands.RemoveAt(0);
            }
        }

        internal static void CommandAssign (Squad squad) {
            commands.Add(new AssignCommand(squad));
        }

        static Dictionary<int, Vector> lastMoveCommand = new Dictionary<int, Vector>();

        internal static void CommandMove (Squad squad) {

            //if (commands.FirstOrDefault(c => c.Squad.Id == squad.Id && c is ShrinkCommand) != null)
            //    return;

            SelectMoveCommand selMoveCommand = (SelectMoveCommand)commands.FirstOrDefault(
                c => c.Squad != null && c.Squad.Id == squad.Id && c is SelectMoveCommand);

            if (selMoveCommand == null)
                commands.Add(new SelectMoveCommand(squad));
        }


        static internal void CommandShrink (Squad squad) {
            commands.Add(new SelectCommand(squad));
            commands.Add(new ShrinkCommand(squad));
        }

        static int selectedSquad = 0;



        static bool first = true;

        public static int SelectedSquad { get { return selectedSquad; } }

        public static List<Command> Commands { get { return commands; } }

        internal static void Reassign (List<Squad> squads) {

            if (!first) {
                commands.Clear();
                CommandResetGrouping();

            }
            foreach (Squad s in squads) {
                CommandSelect(s.Cluster.MinX - 1, s.Cluster.MinY - 1, s.Cluster.MaxX + 1, s.Cluster.MaxY + 1);
                CommandAssign(s);
            }

            first = false;
        }

        private static void CommandResetGrouping () {
            commands.Add(new SelectAllCommand());
            commands.Add(new DismissCommand());
        }

        private static void CommandSelect (double left, double top, double right, double bottom) {
            commands.Add(new SelectCommand(left, top, right, bottom));
        }


    }

    internal class DismissCommand: Command {
        public DismissCommand () : base(null) {
        }

        public override void Execute (Move move) {
            move.Action = ActionType.Dismiss;
        }
    }

    internal class SelectAllCommand: Command {
        ActionType act = ActionType.ClearAndSelect;
        public SelectAllCommand (bool select = true) : base(null) { // or deselect
            if (!select)
                act = ActionType.Deselect;
        }
        public override void Execute (Move move) {
            move.Action = act;
            move.Left = 0;
            move.Right = 1024;
            move.Top = 0;
            move.Bottom = 1024;
        }
    }

    public abstract class Command {
        public bool CanRemove { get; protected set; } = true;
        public Squad Squad { get; internal set; }

        public Command (Squad squad) {
            Squad = squad;
        }
        public abstract void Execute (Move move);
    }

    class ShrinkCommand: Command {
        public ShrinkCommand (Squad squad) : base(squad) {
        }

        public override void Execute (Move move) {
            move.Factor = 0.1;
            move.X = Squad.Cluster.Position.X;
            move.Y = Squad.Cluster.Position.Y;
            move.Action = ActionType.Scale;
        }
    }
    class AssignCommand: Command {
        public AssignCommand (Squad squad) : base(squad) {
        }

        public override void Execute (Move move) {
            move.Action = ActionType.Assign;
            move.Group = Squad.Id;
        }
    }
    class SelectMoveCommand: Command {
        public SelectMoveCommand (Squad squad) : base(squad) {
            CanRemove = false;
        }

        public override void Execute (Move move) {
            if (Squad.Cluster.Count == 0) {
                CanRemove = true;
                return;
            }

            if (Squad.IsSelected) {
                move.Action = ActionType.Move;
                move.X = Squad.Goal.X - Squad.Cluster.Position.X;
                move.Y = Squad.Goal.Y - Squad.Cluster.Position.Y;
                CanRemove = true;
            }
            else {
                move.Action = ActionType.ClearAndSelect;
                move.Group = Squad.Id;
            }
        }
    }

    class SelectCommand: Command {
        private double left;
        private double top;
        private double right;
        private double bottom;

        public SelectCommand (Squad squad) : base(squad) {

        }

        public SelectCommand (double left, double top, double right, double bottom) : base(null) {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public override void Execute (Move move) {
            move.Action = ActionType.ClearAndSelect;
            if (Squad == null) {
                move.Top = Math.Max(0, top);
                move.Bottom = Math.Min(1024, bottom);
                move.Left = Math.Max(0, left);
                move.Right = Math.Min(1024, right);
            }
            else {
                move.Group = Squad.Id;
            }
        }
    }
}
