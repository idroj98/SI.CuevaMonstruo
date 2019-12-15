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
        private int _dimesionMapa;
        private Stack<Posicion> _caminoRecorrido;
        private bool _tesorosCogidos;
        private int _nTesorosCogidos;
        private bool _atascado;
        private Celda[,] _mapa;
        private MovimientosCasilla[,] _movimientosMapa;

        public Posicion Posicion { get; }
        public UIElement Elemento { get; }
        public static MainWindow MainWindow { get; set; }

        

        public Agente(int dimension, Posicion posicion, UIElement elemento, MainWindow mainWindow)
        {
            _mapa = new Celda[dimension, dimension];
            _movimientosMapa = new MovimientosCasilla[dimension, dimension];

            _caminoRecorrido = new Stack<Posicion>();
            _caminoRecorrido.Push(posicion);
            _dimesionMapa = dimension;
            _tesorosCogidos = false;
            _nTesorosCogidos = 0;
            _atascado = false;


            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    _movimientosMapa[i, j] = new MovimientosCasilla();
                }
            }

            Posicion = posicion;
            Elemento = elemento;
            MainWindow = mainWindow;
        }

        public void Update()
        {
            if (!_tesorosCogidos)
            {
                _mapa[Posicion.X, Posicion.Y] = MainWindow.Mapa[Posicion.X, Posicion.Y];
                var currentCelda = _mapa[Posicion.X, Posicion.Y];
                var movimientoCurrentCelda = _movimientosMapa[Posicion.X, Posicion.Y];

                if (currentCelda.Resplandor) {
                    _nTesorosCogidos++;
                    MainWindow.CogerTesoro(Posicion);
                    if (MainWindow.TotalTesoroRestantes == 0)
                    {
                        _tesorosCogidos = true;
                    }
                } else if (currentCelda.HasHedor) {
                    var posDisparo = posicionDisparo();

                    if (posDisparo != null)
                    {
                        bool disparoCertero = dispararYEscuchar();
                        if (disparoCertero)
                        {
                            // quitarMonstruo de posicionDisparo
                        } else
                        {
                            // marcar como libre posición disparo --> este es un caso especial que todavía no hemos tenido en cuenta
                        }
                    }
                    else
                    {
                        movimientoCurrentCelda.Visitada = true;

                        var posicionAnterior = _caminoRecorrido.Pop();
                        Posicion.X = posicionAnterior.X;
                        Posicion.Y = posicionAnterior.Y;

                    }

                } else if (currentCelda.HasBrisa)
                {
                    movimientoCurrentCelda.Visitada = true;

                    var posicionAnterior = _caminoRecorrido.Pop();
                    Posicion.X = posicionAnterior.X;
                    Posicion.Y = posicionAnterior.Y;
                } else
                {
                    // Evaluar si podemos causar hedor

                    // El orden es:
                    // Comprobar si se ha intentado el movimiento
                    // Si queda alguno sin intentar hacer ese. 
                    // Si se han hecho todos los movimiento, comprobar si alguna casilla es hedor
                    //      Si alguna es hedor, ir hacia esa (hacia la primera que comprobemos)

                    /* OJO! Si hay más de un hedor, puede que convenga guardarlo por si fallamos 
                     * el disparo con el otro y luego estamos atascados */
                    // De todas formas esto es un caso tan rebuscado que no lo haría


                    if (!_movimientosMapa[Posicion.X, Posicion.Y].DerechaDone)
                    {

                    }
                    else if (!_movimientosMapa[Posicion.X, Posicion.Y].ArribaDone)
                    {

                    }
                    else if (!_movimientosMapa[Posicion.X, Posicion.Y].AbajoDone)
                    {

                    }
                    else if (!_movimientosMapa[Posicion.X, Posicion.Y].IzquierdaDone)
                    {

                    }
                    else
                    {
                        // Comprobar si hay alguna posición que sea hedor (aunque ya esté visitada)
                        // Si no hay ningún hedor al que ir para probar un disparo, nos quedamos atascados y lo marcamos
                    }

                }

            } else
            {
                // Aquí estamos haciendo la vuelta atrás por el camino que hemos venido
                if(_caminoRecorrido.Count == 0)
                {
                    // Hemos acabado
                } else
                {
                    var posicionAnterior = _caminoRecorrido.Pop();
                    Posicion.X = posicionAnterior.X;
                    Posicion.Y = posicionAnterior.Y;
                }
            }
        }

         

    }
}
