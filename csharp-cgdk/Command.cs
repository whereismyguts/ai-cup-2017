using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public class Commander {

        World world;
        Move move;

        int lastActionTick = -100;

        List<Command> commands = new List<Command>();

        internal void Select (VehicleType unitType) {
            commands.Add(new SelectCommand(unitType));
        }

        internal void Update (World world, Move move, Player me) {
            this.move = move;
            this.world = world;
            this.me = me;
        }

        public void Step () {
            if (me.RemainingActionCooldownTicks ==0) {
                
                ExecuteNextCommand();
                lastActionTick = world.TickIndex;
            }
        }

        private void ExecuteNextCommand () {
            if (commands.Count > 0) {
                commands[0].Execute(move);
                commands.RemoveAt(0);
            }
        }

        internal void Assign (int group) {
            commands.Add(new AssignCommand(group));
        }


        double lastMoveX = -10;
        double lastMoveY = -10;
        internal void Move (double x, double y) {

            commands.RemoveAll(c => c is MoveCommand);

            //if (lastMoveX == x && lastMoveY == y)
            //    return;
           // commands.Add(erase);
            commands.Add(new MoveCommand(x, y));
            lastMoveX = x;
            lastMoveY = y;
        }

        EraseCommand erase = new EraseCommand();
        private Player me;
    }



    abstract class Command {
        public abstract void Execute (Move move);
    }

    class EraseCommand: Command {
        public override void Execute (Move move) {
            move.Action = ActionType.None;
        }
    }

    class AssignCommand: Command {
        int group;
        public AssignCommand (int group) {
            this.group = group;
        }
        public override void Execute (Move move) {
            move.Action = ActionType.Assign;
            move.Group = group;
        }
    }

    class MoveCommand: Command {
        double x; double y;

        public MoveCommand (double x, double y) {
            this.x = x;
            this.y = y;
        }

        public override void Execute (Move move) {
            move.Action = ActionType.Move;
            move.X = x;
            move.Y = y;
        }
    }

    class SelectCommand: Command {
        VehicleType unitTypeToSelect;
        bool typeMatters = false;
        object rectangle;

        public SelectCommand (VehicleType unitType) {
            typeMatters = true;
            this.unitTypeToSelect = unitType;
        }

        public override void Execute (Move move) {
            move.Action = ActionType.ClearAndSelect;
            if (typeMatters)
                move.VehicleType = unitTypeToSelect;
            if (rectangle == null) {
                move.Top = 0;
                move.Bottom = 1024;
                move.Left = 0;
                move.Right = 1024;
            }
        }
    }
}
