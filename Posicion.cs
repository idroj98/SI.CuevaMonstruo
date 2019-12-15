using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SICuevaMonstruo
{
    public class Posicion
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Posicion(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Posicion()
        {

        }

        public override string ToString()
        {
            return "X: " + this.X + ", Y: " + this.Y;
        }

        public Posicion posDerecha()
        {
            return new Posicion(X + 1, Y);
        }

        public Posicion posIzquierda()
        {
            return new Posicion(X - 1, Y);
        }

        public Posicion posArriba()
        {
            return new Posicion(X, Y + 1);
        }

        public Posicion posAbajo()
        {
            return new Posicion(X, Y - 1);
        }

        public bool EqualTo(Posicion pos)
        {
            return this.X == pos.X && this.Y == pos.Y;
        }
    }
}
