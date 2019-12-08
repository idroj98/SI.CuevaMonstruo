using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SICuevaMonstruo
{
    public class Agente
    {
        private Celda[,] _mapa;
        public Posicion Posicion { get; }
        public UIElement Elemento { get; }

        public Agente(int dimension, Posicion posicion, UIElement elemento)
        {
            _mapa = new Celda[dimension, dimension];

            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    _mapa[i, j] = new Celda();
                }
            }

            Posicion = posicion;
            Elemento = elemento;
        }

        public Posicion Update()
        {

            return null;
        }
    }
}
