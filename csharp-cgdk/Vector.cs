using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public struct Vector {
        public Vector(double x, double y) : this() {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        internal double SquareDistanceTo (double x, double y) {
            var dx = X - x;
            var dy =Y - y;
            return dx * dx + dy * dy;
        }
    }
}