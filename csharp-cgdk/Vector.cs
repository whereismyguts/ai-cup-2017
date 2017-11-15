using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public struct Vector {
        public Vector (double x, double y) : this() {
            X = x;
            Y = y;
        }

        public static Vector operator / (Vector p1, float a) {
            return new Vector(p1.X / a, p1.Y / a);
        }

        public static Vector operator + (Vector p1, Vector p2) {
            return new Vector(p1.X + p2.X, p1.Y + p2.Y);
        }

        public IntVector IntVector { get { return new IntVector((int)X, (int)Y); } }
        public double X { get; set; }
        public double Y { get; set; }

        internal double SquareDistanceTo (double x, double y) {
            var dx = X - x;
            var dy = Y - y;
            return dx * dx + dy * dy;
        }

        internal double DistanceTo (double i, double j) {
            return Math.Sqrt(SquareDistanceTo(i, j));
        }

        internal double DistanceTo (Vector position) {
            return Math.Sqrt(SquareDistanceTo(position.X, position.Y));
        }

        public override bool Equals (object obj) {
            var v = (Vector)obj;
            return v.X == X && v.Y == Y;
        }

        internal static double MinAngle (IntVector v1, Vector v2) {
            //return AngleTo(new CoordPoint(0, -1));

            var x1 = v2.X;//0;
            var y1 = v2.Y;//-1;
            var x2 = v1.X;
            var y2 = v1.Y;
            var dot = x1 * x2 + y1 * y2;
            var det = x1 * y2 - y1 * x2;
            var angle = Math.Atan2(det, dot);

            //var angle = (Math.Atan2(0 - X, Y - (-1)));
            return Math.Abs( angle);
        }

        public override string ToString () {
            return "[" + X + "; " + Y + ";]";
        }
    }

    public struct IntVector {
        public IntVector (int x, int y) : this() {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Vector Vector { get { return new Vector(X, Y); } }

        public static IntVector operator + (IntVector p1, IntVector p2) {
            return new IntVector(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static IntVector operator / (IntVector p1, int a) {
            return new IntVector(p1.X / a, p1.Y / a);
        }

        internal int SquareDistanceTo (int x, int y) {
            var dx = X - x;
            var dy = Y - y;
            return dx * dx + dy * dy;
        }

        public override string ToString () {
            return "[" + X + "; " + Y + ";]";
        }
    }
}