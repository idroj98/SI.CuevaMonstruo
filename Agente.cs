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
        //private int _nTesorosCogidos;
        private Celda[,] _mapa;
        private MovimientosCasilla[,] _movimientosMapa;
        private Posicion HedorPropio =  new Posicion(-1, -1);

        public int NumFlechas { get; set; }
        public Posicion Posicion { get; }
        public UIElement Elemento { get; }
        public static MainWindow MainWindow { get; set; }

        private bool retornar = false;



        public Agente(int dimension, Posicion posicion, UIElement elemento, MainWindow mainWindow)
        {
            _mapa = new Celda[dimension, dimension];
            _movimientosMapa = new MovimientosCasilla[dimension, dimension];

            _caminoRecorrido = new Stack<Posicion>();
            _caminoRecorrido.Push(new Posicion(posicion.X, posicion.Y));
            _dimesionMapa = dimension;
            _tesorosCogidos = false;
            //_nTesorosCogidos = 0;


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

        public override string ToString()
        {
            return "Posición: " + Posicion + ", Flechas restantes: " + NumFlechas + ", Retornar: " + retornar;
        }

        public void Update()
        {
            var posDerecha = Posicion.posDerecha();
            var posIzquierda = Posicion.posIzquierda();
            var posArriba = Posicion.posArriba();
            var posAbajo = Posicion.posAbajo();

            if (!_tesorosCogidos)
            {
                if(retornar)
                {
                    bool posAnterior = false;
                    if(!_movimientosMapa[Posicion.X, Posicion.Y].IzquierdaDone)
                    {
                        if (esPosibleIrA(posIzquierda.X, posIzquierda.Y))
                        {
                            retornar = false;
                            Posicion.X = posIzquierda.X;
                            Posicion.Y = posIzquierda.Y;
                            _movimientosMapa[Posicion.X, Posicion.Y].IzquierdaDone = true;
                            _caminoRecorrido.Push(new Posicion(posIzquierda.X, posIzquierda.Y));
                        }
                        else
                        {
                            posAnterior = true;
                        }
                    }
                    else if (!_movimientosMapa[Posicion.X, Posicion.Y].ArribaDone)
                    {
                        if (esPosibleIrA(posArriba.X, posArriba.Y))
                        {
                            retornar = false;
                            Posicion.X = posArriba.X;
                            Posicion.Y = posArriba.Y;
                            _movimientosMapa[Posicion.X, Posicion.Y].ArribaDone = true;
                            _caminoRecorrido.Push(new Posicion(posArriba.X, posArriba.Y));
                        }
                        else
                        {
                            posAnterior = true;
                        }
                    }
                    else if (!_movimientosMapa[Posicion.X, Posicion.Y].AbajoDone)
                    {
                        if (esPosibleIrA(posAbajo.X, posAbajo.Y))
                        {
                            retornar = false;
                            Posicion.X = posAbajo.X;
                            Posicion.Y = posAbajo.Y;
                            _movimientosMapa[Posicion.X, Posicion.Y].AbajoDone = true;
                            _caminoRecorrido.Push(new Posicion(posAbajo.X, posAbajo.Y));
                        }
                        else
                        {
                            posAnterior = true;
                        }
                    }
                    else if (!_movimientosMapa[Posicion.X, Posicion.Y].DerechaDone)
                    {
                        if (esPosibleIrA(posDerecha.X, posDerecha.Y))
                        {
                            retornar = false;
                            Posicion.X = posDerecha.X;
                            Posicion.Y = posDerecha.Y;
                            _movimientosMapa[Posicion.X, Posicion.Y].DerechaDone = true;
                            _caminoRecorrido.Push(new Posicion(posDerecha.X, posDerecha.Y));
                        }
                        else
                        {
                            posAnterior = true;
                            
                        }
                    } else
                    {
                        posAnterior = true;
                    }

                    if (posAnterior)
                    {
                        
                        if(_caminoRecorrido.Count == 0)
                        {
                            retornar = false;
                            for (int i = 0; i < _dimesionMapa; i++)
                            {
                                for (int j = 0; j < _dimesionMapa; j++)
                                {
                                    _mapa[i, j] = null;
                                    _movimientosMapa[i, j] = new MovimientosCasilla();
                                }
                            }
                            _caminoRecorrido.Push(new Posicion(Posicion.X, Posicion.Y));
                        }
                        else
                        {
                            var posicionAnterior = _caminoRecorrido.Pop();
                            Posicion.X = posicionAnterior.X;
                            Posicion.Y = posicionAnterior.Y;
                        }
                    }
                }
                else
                {
                    _mapa[Posicion.X, Posicion.Y] = MainWindow.Mapa[Posicion.X, Posicion.Y];
                    var currentCelda = _mapa[Posicion.X, Posicion.Y];
                    _movimientosMapa[Posicion.X, Posicion.Y].Descubierta = true;
                    var movimientoCurrentCelda = _movimientosMapa[Posicion.X, Posicion.Y];

                    if (currentCelda.Resplandor)
                    {
                        MainWindow.CogerTesoro(Posicion);
                        _tesorosCogidos = true;

                    }
                    else if (currentCelda.HasHedor && !Posicion.EqualTo(HedorPropio))
                    {
                        var pos = PosicionDisparo();
                        if (NumFlechas > 0 && pos != null)
                        {
                            this.NumFlechas--;
                            var grito = MainWindow.Disparar(pos);
                            if (grito)
                            {
                                if (HedorPropio.EqualTo(new Posicion(-1, -1)))
                                {
                                    HedorPropio = new Posicion(Posicion.X, Posicion.Y);
                                    MainWindow.TirarsePedo(HedorPropio);
                                }
                            }
                        }
                        else
                        {
                            _caminoRecorrido.Pop();
                            var posIr = _caminoRecorrido.Peek();
                            Posicion.X = posIr.X;
                            Posicion.Y = posIr.Y;
                        }

                    }
                    else if (currentCelda.HasBrisa)
                    {
                        movimientoCurrentCelda.Visitada = true;

                        _caminoRecorrido.Pop();
                        var posIr = _caminoRecorrido.Peek();
                        Posicion.X = posIr.X;
                        Posicion.Y = posIr.Y;
                    }
                    else
                    {
                        Posicion posicionDestino = null;

                        if (!PosicionOutOfBounds(posDerecha) && !_movimientosMapa[posDerecha.X, posDerecha.Y].Descubierta)
                        {
                            _movimientosMapa[posDerecha.X, posDerecha.Y].Descubierta = true;
                            _movimientosMapa[Posicion.X, Posicion.Y].DerechaDone = true;
                            posicionDestino = posDerecha;

                        }
                        else if (!PosicionOutOfBounds(posArriba) && !_movimientosMapa[posArriba.X, posArriba.Y].Descubierta)
                        {
                            _movimientosMapa[posArriba.X, posArriba.Y].Descubierta = true;
                            _movimientosMapa[Posicion.X, Posicion.Y].ArribaDone = true;
                            posicionDestino = posArriba;

                        }
                        else if (!PosicionOutOfBounds(posIzquierda) && !_movimientosMapa[posIzquierda.X, posIzquierda.Y].Descubierta)
                        {
                            _movimientosMapa[posIzquierda.X, posIzquierda.Y].Descubierta = true;
                            _movimientosMapa[Posicion.X, Posicion.Y].IzquierdaDone = true;
                            posicionDestino = posIzquierda;

                        }
                        else if (!PosicionOutOfBounds(posAbajo) && !_movimientosMapa[posAbajo.X, posAbajo.Y].Descubierta)
                        {
                            _movimientosMapa[posAbajo.X, posAbajo.Y].Descubierta = true;
                            _movimientosMapa[Posicion.X, Posicion.Y].AbajoDone = true;
                            posicionDestino = posAbajo;

                        }
                        else
                        {
                            /*****************************************************/
                            // Todas estan descubiertas
                            // Hay que hacer algún movimiento que no hayamos hecho antes
                            if (!_movimientosMapa[Posicion.X, Posicion.Y].DerechaDone)
                            {
                                _movimientosMapa[Posicion.X, Posicion.Y].DerechaDone = true;
                                if (esPosibleIrA(posDerecha.X, posDerecha.Y))
                                {
                                    posicionDestino = posDerecha;
                                }

                            }
                            else if (!_movimientosMapa[Posicion.X, Posicion.Y].AbajoDone)
                            {
                                _movimientosMapa[Posicion.X, Posicion.Y].AbajoDone = true;
                                if (esPosibleIrA(posAbajo.X, posAbajo.Y))
                                {
                                    posicionDestino = posAbajo;
                                }

                            }
                            else if (!_movimientosMapa[Posicion.X, Posicion.Y].ArribaDone)
                            {
                                _movimientosMapa[Posicion.X, Posicion.Y].ArribaDone = true;
                                if (esPosibleIrA(posArriba.X, posArriba.Y))
                                {
                                    posicionDestino = posArriba;
                                }
                            }
                            else if (!_movimientosMapa[Posicion.X, Posicion.Y].IzquierdaDone)
                            {
                                _movimientosMapa[Posicion.X, Posicion.Y].IzquierdaDone = true;
                                if (esPosibleIrA(posIzquierda.X, posIzquierda.Y))
                                {
                                    posicionDestino = posIzquierda;
                                }
                            }
                            //else
                            //{
                            //    _movimientosMapa[Posicion.X, Posicion.Y].movimientosHechos = true;
                                
                            //    if (!movimientoHechosEn(posDerecha))
                            //    {
                            //        if (esPosibleIrA(posDerecha.X, posDerecha.Y))
                            //            posicionDestino = posDerecha;
                            //    }
                            //    else if (!movimientoHechosEn(posAbajo))
                            //    {
                            //        if (esPosibleIrA(posAbajo.X, posAbajo.Y))
                            //            posicionDestino = posAbajo;
                            //    }
                            //    else if (!movimientoHechosEn(posArriba))
                            //    {
                            //        if (esPosibleIrA(posArriba.X, posArriba.Y))
                            //            posicionDestino = posArriba;
                            //    }
                            //    else if (!movimientoHechosEn(posIzquierda))
                            //    {
                            //        if (esPosibleIrA(posIzquierda.X, posIzquierda.Y))
                            //            posicionDestino = posIzquierda;
                            //    }
                                else
                                {
                                    _movimientosMapa[Posicion.X, Posicion.Y].Visitada = true;
                                    //_movimientosMapa[Posicion.X, Posicion.Y].Visitada = true;
                                    //if (!PosicionOutOfBounds(posDerecha) && !_movimientosMapa[posDerecha.X, posDerecha.Y].Visitada)
                                    //{
                                    //    posicionDestino = posDerecha;
                                    //}
                                    //else if (!PosicionOutOfBounds(posAbajo) && !_movimientosMapa[posAbajo.X, posAbajo.Y].Visitada)
                                    //{
                                    //    posicionDestino = posAbajo;
                                    //}
                                    //else if (!PosicionOutOfBounds(posIzquierda) && !_movimientosMapa[posIzquierda.X, posIzquierda.Y].Visitada)
                                    //{
                                    //    posicionDestino = posIzquierda;
                                    //}
                                    //else if (!PosicionOutOfBounds(posArriba) && !_movimientosMapa[posArriba.X, posArriba.Y].Visitada)
                                    //{
                                    //    posicionDestino = posArriba;
                                    //}
                                //}
                            }
                        }

                        var nVisitados = 0;
                        if (PosicionOutOfBounds(posDerecha) || _movimientosMapa[posDerecha.X, posDerecha.Y].Visitada)
                        {
                            nVisitados++;
                        }

                        if (PosicionOutOfBounds(posIzquierda) || _movimientosMapa[posIzquierda.X, posIzquierda.Y].Visitada)
                        {
                            nVisitados++;
                        }

                        if (PosicionOutOfBounds(posAbajo) || _movimientosMapa[posAbajo.X, posAbajo.Y].Visitada)
                        {
                            nVisitados++;
                        }

                        if (PosicionOutOfBounds(posArriba) || _movimientosMapa[posArriba.X, posArriba.Y].Visitada)
                        {
                            nVisitados++;
                        }

                        if (posicionDestino != null)
                        {
                            if (nVisitados == 3)
                            {
                                _movimientosMapa[Posicion.X, Posicion.Y].Visitada = true;
                            }

                            //_caminoRecorrido.Push(new Posicion(Posicion.X, Posicion.Y));
                            _caminoRecorrido.Push(new Posicion(posicionDestino.X, posicionDestino.Y));
                            Posicion.X = posicionDestino.X;
                            Posicion.Y = posicionDestino.Y;
                        }
                        else
                        {
                            retornar = true;
                            var posicionVolver = _caminoRecorrido.Pop();
                            Posicion.X = posicionVolver.X;
                            Posicion.Y = posicionVolver.Y;
                        }

                    }
                }
            }
            else
            {
                // Aquí estamos haciendo la vuelta atrás por el camino que hemos venido
                if (_caminoRecorrido.Count == 0)
                {
                    MainWindow.AgenteFinaliza(Posicion);
                }
                else
                {
                    var posicionAnterior = _caminoRecorrido.Pop();
                    Posicion.X = posicionAnterior.X;
                    Posicion.Y = posicionAnterior.Y;
                }
            }
        }

        private bool esPosibleIrA(int xDestino, int yDestino)
        {
            bool esPosible = true;
            // Puede ser que en este método haya que mirar también si ya ha sido visitado o no
            var posDestino = new Posicion(xDestino, yDestino);

            esPosible = !PosicionOutOfBounds(posDestino);
            if (esPosible)
            {
                if (_movimientosMapa[xDestino, yDestino].Visitada)
                {
                    esPosible = false;
                }
            }

            return esPosible;
        }

        private bool PosicionOutOfBounds(Posicion pos)
        {
            return (pos.X < 0) || (pos.Y < 0) || (pos.X >= this._dimesionMapa) || (pos.Y >= this._dimesionMapa);
        }

        private bool PosicionOutOfBounds(int posX, int posY)
        {
            return (posX < 0) || (posY < 0) || (posX >= this._dimesionMapa) || (posY >= this._dimesionMapa);
        }

        private bool movimientoHechosEn(Posicion pos)
        {
            // Si es outOfBounds es como si los movimientos estuvieran hechos
            bool movimientosHechos = PosicionOutOfBounds(pos);
            if (!movimientosHechos)
            {
                movimientosHechos = _movimientosMapa[pos.X, pos.Y].movimientosHechos;
            }
            return movimientosHechos;
        }

        private Posicion PosicionDisparo()
        {
            var alrededor = PosicionesHedorAlrededor();
            var movAlrededor = MoimientoAgenteAlrededor();
            var listPosHedor = PosicionesHedor(alrededor);

            switch (listPosHedor.Count())
            {
                case 2:
                    if (listPosHedor[0] == 0 && listPosHedor[1] == 2)
                    {
                        if (alrededor[1] == null && !movAlrededor[1].Disparada)
                        {
                            movAlrededor[1].Disparada = true;
                            return IndexToPos(1);
                        }
                    }
                    else if (listPosHedor[0] == 2 && listPosHedor[1] == 7)
                    {
                        if (alrededor[4] == null && !movAlrededor[4].Disparada)
                        {
                            movAlrededor[4].Disparada = true;
                            return IndexToPos(4);
                        }
                    }
                    else if (listPosHedor[0] == 0 && listPosHedor[1] == 5)
                    {
                        if (alrededor[3] == null && !movAlrededor[3].Disparada)
                        {
                            movAlrededor[3].Disparada = true;
                            return IndexToPos(3);
                        }
                    }
                    else if (listPosHedor[0] == 5 && listPosHedor[1] == 7)
                    {
                        if (alrededor[6] == null && !movAlrededor[6].Disparada)
                        {
                            movAlrededor[6].Disparada = true;
                            return IndexToPos(6);
                        }
                    }
                    break;
                case 1:
                    if (listPosHedor[0] == 0)
                    {
                        if (alrededor[1] == null && !movAlrededor[1].Disparada)
                        {
                            movAlrededor[1].Disparada = true;
                            return IndexToPos(1);
                        }
                        else if(alrededor[3] == null && !movAlrededor[3].Disparada)
                        {
                            movAlrededor[3].Disparada = true;
                            return IndexToPos(3);
                        }
                    }
                    else if (listPosHedor[0] == 2)
                    {
                        if (alrededor[4] == null && !movAlrededor[1].Disparada)
                        {
                            movAlrededor[4].Disparada = true;
                            return IndexToPos(4);
                        }
                        else if (alrededor[1] == null && !movAlrededor[1].Disparada)
                        {
                            movAlrededor[1].Disparada = true;
                            return IndexToPos(1);
                        }
                    }
                    else if (listPosHedor[0] == 5)
                    {
                        if (alrededor[3] == null && !movAlrededor[3].Disparada)
                        {
                            movAlrededor[3].Disparada = true;
                            return IndexToPos(3);
                        }
                        else if (alrededor[6] == null && !movAlrededor[6].Disparada)
                        {
                            movAlrededor[6].Disparada = true;
                            return IndexToPos(6);
                        }
                    }
                    else if (listPosHedor[0] == 7)
                    {
                        if (alrededor[6] == null && !movAlrededor[6].Disparada)
                        {
                            movAlrededor[6].Disparada = true;
                            return IndexToPos(6);
                        }
                        else if (alrededor[4] == null && !movAlrededor[4].Disparada)
                        {
                            movAlrededor[4].Disparada = true;
                            return IndexToPos(4);
                        }
                    }
                    break;
                case 0:
                    if (movAlrededor[1].Visitada && movAlrededor[3].Visitada && movAlrededor[4].Visitada)
                    {
                        return IndexToPos(6);
                    }
                    else if (movAlrededor[1].Visitada && movAlrededor[4].Visitada && movAlrededor[6].Visitada)
                    {
                        return IndexToPos(3);
                    }
                    else if (movAlrededor[1].Visitada && movAlrededor[3].Visitada && movAlrededor[6].Visitada)
                    {
                        return IndexToPos(4);
                    }
                    else if (movAlrededor[3].Visitada && movAlrededor[4].Visitada && movAlrededor[6].Visitada)
                    {
                        return IndexToPos(1);
                    }
                    break;
                default:
                    return null;

            }

            return null;
        }

        private Celda[] PosicionesHedorAlrededor()
        {
            var result = new Celda[8];

            var pos = new Posicion() { X = Posicion.X - 1, Y = Posicion.Y - 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[0] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X - 1, Y = Posicion.Y };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[1] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X - 1, Y = Posicion.Y + 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[2] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X, Y = Posicion.Y - 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[3] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X, Y = Posicion.Y + 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[4] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X + 1, Y = Posicion.Y - 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[5] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X + 1, Y = Posicion.Y };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[6] = this._mapa[pos.X, pos.Y];
            }

            pos = new Posicion() { X = Posicion.X + 1, Y = Posicion.Y + 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._mapa[pos.X, pos.Y] != null)
                    result[7] = this._mapa[pos.X, pos.Y];
            }

            return result;
        }

        private MovimientosCasilla[] MoimientoAgenteAlrededor()
        {
            var result = new MovimientosCasilla[8];

            var pos = new Posicion() { X = Posicion.X - 1, Y = Posicion.Y - 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[0] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[0] = new MovimientosCasilla();
            }
            else
            {
                result[0] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X - 1, Y = Posicion.Y };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[1] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[1] = new MovimientosCasilla();
            }
            else
            {
                result[1] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X - 1, Y = Posicion.Y + 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[2] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[2] = new MovimientosCasilla();
            }
            else
            {
                result[2] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X, Y = Posicion.Y - 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[3] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[3] = new MovimientosCasilla();
            }
            else
            {
                result[3] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X, Y = Posicion.Y + 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[4] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[4] = new MovimientosCasilla();
            }
            else
            {
                result[4] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X + 1, Y = Posicion.Y - 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[5] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[5] = new MovimientosCasilla();
            }
            else
            {
                result[5] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X + 1, Y = Posicion.Y };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[6] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[6] = new MovimientosCasilla();
            }
            else
            {
                result[6] = new MovimientosCasilla();
            }

            pos = new Posicion() { X = Posicion.X + 1, Y = Posicion.Y + 1 };
            if (!PosicionOutOfBounds(pos))
            {
                if (this._movimientosMapa[pos.X, pos.Y] != null)
                    result[7] = this._movimientosMapa[pos.X, pos.Y];
                else
                    result[7] = new MovimientosCasilla();
            }
            else
            {
                result[7] = new MovimientosCasilla();
            }

            return result;
        }

        private List<int> PosicionesHedor(Celda[] alrededor)
        {
            var result = new List<int>();

            for (int i = 0; i < alrededor.Length; i++)
            {
                if (alrededor[i] != null && alrededor[i].HasHedor)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        private Posicion IndexToPos(int i)
        {
            switch (i)
            {
                case 0:
                    return new Posicion() { X = Posicion.X - 1, Y = Posicion.Y - 1 };
                case 1:
                    return new Posicion() { X = Posicion.X - 1, Y = Posicion.Y }; ;
                case 2:
                    return new Posicion() { X = Posicion.X - 1, Y = Posicion.Y + 1 }; ;
                case 3:
                    return new Posicion() { X = Posicion.X, Y = Posicion.Y - 1 }; ;
                case 4:
                    return new Posicion() { X = Posicion.X, Y = Posicion.Y + 1 }; ;
                case 5:
                    return new Posicion() { X = Posicion.X + 1, Y = Posicion.Y - 1 }; ;
                case 6:
                    return new Posicion() { X = Posicion.X + 1, Y = Posicion.Y }; ;
                case 7:
                    return new Posicion() { X = Posicion.X + 1, Y = Posicion.Y + 1 }; ;
                default:
                    return null;
            }
        }

    }
}
