using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;

using System.IO;

namespace SICuevaMonstruo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum ObjetoSeleccionado
        {
            Monstruo, Precipicio, Tesoro, Agente
        }

        public Celda[,] Mapa;
        public int TotalTesoroRestantes;
        private int _dimesionMapa;
        private ObjetoSeleccionado _objetoSeleccionado;
        private List<Agente> _agentes;
        private bool _isOn = false;
        private int _numMonstruos = 0;
        private bool _hayGanador = false;
        private Brush[] _colorAgentes = { Brushes.Lavender, Brushes.Khaki, Brushes.Moccasin, Brushes.MistyRose };

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            _dispatcherTimer.Tick += new EventHandler(Update);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);

            TotalTesoroRestantes = 0;

            InitializeComponent();

            this.Seleccion.ItemsSource = Enum.GetValues(typeof(ObjetoSeleccionado)).Cast<ObjetoSeleccionado>();
        }

        private void Update(object sender, EventArgs e)
        {
            string info_agentes = "";
            foreach (var agente in _agentes)
            {
                agente.Update();
                Grid.SetRow(agente.Elemento, agente.Posicion.X);
                Grid.SetColumn(agente.Elemento, agente.Posicion.Y);
                info_agentes += agente + "\n";
            }
            this.InfoAgentes.Content = info_agentes;
        }

        private void CrearMapa_Click(object sender, RoutedEventArgs e)
        {
            _agentes = new List<Agente>();
            this._hayGanador = false;
            this._numMonstruos = 0;

            var cueva = this.Cueva;
            try
            {
                _dimesionMapa = Int32.Parse(this.Dimesion.Text); ;

                Mapa = new Celda[_dimesionMapa, _dimesionMapa];

                for (int i = 0; i < _dimesionMapa; i++)
                {
                    for (int j = 0; j < _dimesionMapa; j++)
                    {
                        Mapa[i, j] = new Celda();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Número de dimensiones incorrecto!");
                return;
            }

            cueva.Children.Clear();
            cueva.ColumnDefinitions.Clear();
            cueva.RowDefinitions.Clear();

            for (var i = 0; i < _dimesionMapa; i++)
            {
                var col = new ColumnDefinition();
                cueva.ColumnDefinitions.Add(col);
            }

            for (var i = 0; i < _dimesionMapa; i++)
            {
                var row = new RowDefinition();
                cueva.RowDefinitions.Add(row);
            }

            for (var i = 0; i < _dimesionMapa; i++)
            {
                for (var j = 0; j < _dimesionMapa; j++)
                {
                    var panel = CrearCelda(Brushes.White);

                    Grid.SetColumn(panel, j);
                    Grid.SetRow(panel, i);
                    cueva.Children.Add(panel);
                }
            }
        }

        private Border CrearCelda(SolidColorBrush color)
        {
            var panel = new Border
            {
                Background = color,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5)
            };
            panel.MouseLeftButtonUp += ClickCeldaLeft;

            return panel;
        }

        private void ClickCeldaLeft(object sender, MouseEventArgs e)
        {
            if (!_isOn)
            {
                var pan = sender as Border;

                var columna = Grid.GetColumn(pan);
                var fila = Grid.GetRow(pan);

                switch (_objetoSeleccionado)
                {
                    case ObjetoSeleccionado.Monstruo:
                        Mapa[fila, columna].Monstruo = !Mapa[fila, columna].Monstruo;
                        UpdateAlrededores(fila, columna, Mapa[fila, columna].Monstruo);
                        if (Mapa[fila, columna].Monstruo)
                            _numMonstruos++;
                        else
                            _numMonstruos--;
                        break;
                    case ObjetoSeleccionado.Precipicio:
                        Mapa[fila, columna].Precipicio = !Mapa[fila, columna].Precipicio;
                        UpdateAlrededores(fila, columna, Mapa[fila, columna].Precipicio);
                        break;
                    case ObjetoSeleccionado.Agente:
                        SetPosicionAgenteMapa(fila, columna);
                        break;
                    case ObjetoSeleccionado.Tesoro:
                        Mapa[fila, columna].Resplandor = !Mapa[fila, columna].Resplandor;
                        UpdateAlrededores(fila, columna, Mapa[fila, columna].Resplandor);
                        break;
                }

                ActualizarCasilla(fila, columna);
            }
        }

        private void UpdateAlrededores(int fila, int columna, bool add)
        {
            var valueAdded = 1;
            if (!add)
            {
                valueAdded = -1;
            }

            switch (_objetoSeleccionado)
            {
                case ObjetoSeleccionado.Monstruo:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Hedor += valueAdded;
                        ActualizarCasilla(fila + 1, columna);
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Hedor += valueAdded;
                        ActualizarCasilla(fila - 1, columna);
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Hedor += valueAdded;
                        ActualizarCasilla(fila, columna + 1);
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Hedor += valueAdded;
                        ActualizarCasilla(fila, columna - 1);
                    }
                    break;
                case ObjetoSeleccionado.Precipicio:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Brisa += valueAdded;
                        ActualizarCasilla(fila + 1, columna);
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Brisa += valueAdded;
                        ActualizarCasilla(fila - 1, columna);
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Brisa += valueAdded;
                        ActualizarCasilla(fila, columna + 1);
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Brisa += valueAdded;
                        ActualizarCasilla(fila, columna - 1);
                    }
                    break;
                case ObjetoSeleccionado.Tesoro:
                    TotalTesoroRestantes += valueAdded;
                    break;
            }
        }

        public void ActualizarCasilla(int fila, int columna)
        {
            if (Mapa[fila, columna].Monstruo)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Green;
            }
            else if (Mapa[fila, columna].Precipicio)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Black;
            }
            else if (Mapa[fila, columna].HasHedor && Mapa[fila, columna].HasBrisa && Mapa[fila, columna].Resplandor)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Red;
            }
            else if (Mapa[fila, columna].HasHedor && Mapa[fila, columna].HasBrisa)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Gray;
            }
            else if (Mapa[fila, columna].Resplandor)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Gold;
            }
            else if (Mapa[fila, columna].HasHedor)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.LightGreen;
            }
            else if (Mapa[fila, columna].HasBrisa)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.LightBlue;
            }
            else
            {
                GetBorderByIndex(fila, columna).Background = Brushes.White;
            }
        }

        private void SetPosicionAgenteMapa(int fila, int columna)
        {
            if (Mapa[fila,columna].Agente != null)
            {
                this._agentes.Remove(Mapa[fila, columna].Agente);
                this.Cueva.Children.Remove(Mapa[fila, columna].Agente.Elemento);
                Mapa[fila, columna].Agente = null;
            }
            else
            {
                var elemento = new Image()
                {
                    Name = "Robot",
                    Source = new BitmapImage(new Uri(@"https://res.cloudinary.com/pixel-art/image/upload/v1554320836/robot/1466134-robot-pixel-art.png")),
                    Margin = new Thickness(0.5),

                };
                elemento.MouseLeftButtonUp += DeleteAgente;
                var agente = new Agente(_dimesionMapa, new Posicion(fila, columna), elemento, this);
                Mapa[fila, columna].Agente = agente;
                _agentes.Add(agente);

                Grid.SetRow(agente.Elemento, agente.Posicion.X);
                Grid.SetColumn(agente.Elemento, agente.Posicion.Y);

                this.Cueva.Children.Add(agente.Elemento);
            }
        }

        private void DeleteAgente(object sender, MouseEventArgs e)
        {
            var img = sender as Image;

            var columna = Grid.GetColumn(img);
            var fila = Grid.GetRow(img);

            this._agentes.Remove(Mapa[fila, columna].Agente);
            this.Cueva.Children.Remove(Mapa[fila, columna].Agente.Elemento);
            Mapa[fila, columna].Agente = null;
        }

        private Border GetBorderByIndex(int fila, int columna)
        {
            var aux = Cueva.Children
                          .Cast<UIElement>()
                          .First(s => Grid.GetRow(s) == fila && Grid.GetColumn(s) == columna);

            return (Border)aux;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _isOn = true;
            var i = 0;
            foreach (Agente agente in _agentes)
            {
                if (i == _colorAgentes.Length)
                    i = 0;

                agente.ColorCasilla = _colorAgentes[i];
                agente.NumFlechas = _numMonstruos;
                i++;
            }
            _dispatcherTimer.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _isOn = false;
            _dispatcherTimer.Stop();
        }

        private void Seleccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._objetoSeleccionado = (ObjetoSeleccionado)this.Seleccion.SelectedItem;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / ((int)this.Velocidad.Value + 1));
        }

        private void Reiniciar_Click(object sender, RoutedEventArgs e)
        {
            this._isOn = false;
            this._hayGanador = false;
            this._numMonstruos = 0; 
            _dispatcherTimer.Stop();
            CrearMapa_Click(sender, e);
        }

        public void CogerTesoro(Posicion pos)
        {
            TotalTesoroRestantes--;
            Mapa[pos.X, pos.Y].Resplandor = false;
            ActualizarCasilla(pos.X, pos.Y);
        }

        public bool Disparar(Posicion pos)
        {
            if (Mapa[pos.X, pos.Y].Monstruo)
            {
                Mapa[pos.X, pos.Y].Monstruo = false;
                ActualizarCasilla(pos.X, pos.Y);

                _objetoSeleccionado = ObjetoSeleccionado.Monstruo;

                UpdateAlrededores(pos.X, pos.Y, false);

                return true;
            }

            return false;
        }

        public void TirarsePedo(Posicion pos)
        {
            Mapa[pos.X, pos.Y].Hedor++;
            GetBorderByIndex(pos.X, pos.Y).Background = Brushes.Purple;
        }

        public void AgenteFinaliza(Posicion pos)
        {
            if (!_hayGanador)
            {
                GetBorderByIndex(pos.X, pos.Y).Background = Brushes.Tomato;
                _hayGanador = true;
                //Stop_Click(null, null);
                //MessageBox.Show("WE HAVE A WINNER!");
                //Reiniciar_Click(null, null);
            }
        }

        public void SetColorCasilla(Brush color, Posicion pos)
        {
            if (!Mapa[pos.X, pos.Y].HasHedor && !Mapa[pos.X, pos.Y].HasBrisa)
            {
                GetBorderByIndex(pos.X, pos.Y).Background = color;
            }
        }

    }
}
