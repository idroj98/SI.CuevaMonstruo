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

        public static Celda[,] Mapa;
        private int _dimesionMapa;
        private ObjetoSeleccionado _objetoSeleccionado;
        private List<Agente> _agentes;
        private bool _isOn = false;

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            _dispatcherTimer.Tick += new EventHandler(Update);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);

            _agentes = new List<Agente>();

            InitializeComponent();

            this.Seleccion.ItemsSource = Enum.GetValues(typeof(ObjetoSeleccionado)).Cast<ObjetoSeleccionado>();
        }

        private void Update(object sender, EventArgs e)
        {
            foreach (var agente in _agentes)
            {
                agente.Update();
                Grid.SetRow(agente.Elemento, agente.Posicion.X);
                Grid.SetColumn(agente.Elemento, agente.Posicion.Y);
            }
        }

        private void CrearMapa_Click(object sender, RoutedEventArgs e)
        {
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
            switch (_objetoSeleccionado)
            {
                case ObjetoSeleccionado.Monstruo:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Hedor = add;
                        ActualizarCasilla(fila + 1, columna);
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Hedor = add;
                        ActualizarCasilla(fila - 1, columna);
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Hedor = add;
                        ActualizarCasilla(fila, columna + 1);
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Hedor = add;
                        ActualizarCasilla(fila, columna - 1);
                    }
                    break;
                case ObjetoSeleccionado.Precipicio:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Brisa = add;
                        ActualizarCasilla(fila + 1, columna);
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Brisa = add;
                        ActualizarCasilla(fila - 1, columna);
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Brisa = add;
                        ActualizarCasilla(fila, columna + 1);
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Brisa = add;
                        ActualizarCasilla(fila, columna - 1);
                    }
                    break;
            }
        }

        private void ActualizarCasilla(int fila, int columna)
        {
            if (Mapa[fila, columna].Monstruo)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Green;
            }
            else if (Mapa[fila, columna].Precipicio)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Black;
            }
            else if (Mapa[fila, columna].Hedor && Mapa[fila, columna].Brisa && Mapa[fila, columna].Resplandor)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Red;
            }
            else if (Mapa[fila, columna].Hedor && Mapa[fila, columna].Brisa)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Gray;
            }
            else if (Mapa[fila, columna].Resplandor)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.Gold;
            }
            else if (Mapa[fila, columna].Hedor)
            {
                GetBorderByIndex(fila, columna).Background = Brushes.LightGreen;
            }
            else if (Mapa[fila, columna].Brisa)
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
                    Source = new BitmapImage(new Uri("https://res.cloudinary.com/pixel-art/image/upload/v1554320836/robot/1466134-robot-pixel-art.png")),
                    Margin = new Thickness(0.5)
                };
                var agente = new Agente(_dimesionMapa, new Posicion() { X = fila, Y = columna }, elemento);
                Mapa[fila, columna].Agente = agente;
                _agentes.Add(agente);

                Grid.SetRow(agente.Elemento, agente.Posicion.X);
                Grid.SetColumn(agente.Elemento, agente.Posicion.Y);

                this.Cueva.Children.Add(agente.Elemento);
            }
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
    }
}
