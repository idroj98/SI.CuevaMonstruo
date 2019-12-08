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

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            _dispatcherTimer.Tick += new EventHandler(Update);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);

            _agentes = new List<Agente>();

            InitializeComponent();
        }

        private void Update(object sender, EventArgs e)
        {
            foreach(var agente in _agentes)
            {
                var posicion = agente.Update();
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
            var pan = sender as Border;

            var columna = Grid.GetColumn(pan);
            var fila = Grid.GetRow(pan);

            if (pan.Background == Brushes.White)
            {
                switch (_objetoSeleccionado)
                {
                    case ObjetoSeleccionado.Monstruo:
                        pan.Background = Brushes.Green;
                        Mapa[fila, columna].Monstruo = true;
                        break;
                    case ObjetoSeleccionado.Precipicio:
                        pan.Background = Brushes.Black;
                        Mapa[fila, columna].Precipicio = true;
                        break;
                    case ObjetoSeleccionado.Agente:
                        var elemento = new Image()
                        {
                            Name = "Robot",
                            Source = new BitmapImage(new Uri("https://res.cloudinary.com/pixel-art/image/upload/v1554320836/robot/1466134-robot-pixel-art.png")),
                            Margin = new Thickness(0.5)
                        };
                        var agente = new Agente(_dimesionMapa, new Posicion() { X = fila, Y = columna }, elemento);
                        _agentes.Add(agente);
                        SetPosicionAgenteMapa(agente);
                        break;
                    case ObjetoSeleccionado.Tesoro:
                        pan.Background = Brushes.Gold;
                        Mapa[fila, columna].Resplandor = true;
                        break;
                }

                AddAlrededores(fila, columna, _objetoSeleccionado);
            }
            else
            {
                switch (_objetoSeleccionado)
                {
                    case ObjetoSeleccionado.Monstruo:
                        Mapa[fila, columna].Monstruo = false;
                        break;
                    case ObjetoSeleccionado.Precipicio:
                        Mapa[fila, columna].Precipicio = false;
                        break;
                    case ObjetoSeleccionado.Agente:
                        break;
                    case ObjetoSeleccionado.Tesoro:
                        Mapa[fila, columna].Monstruo = false;
                        break;
                }

                pan.Background = Brushes.White;

                DeleteAlrededores(fila, columna, _objetoSeleccionado);
            }
        }

        private void AddAlrededores(int fila, int columna, ObjetoSeleccionado objetoSeleccionado)
        {
            switch (objetoSeleccionado)
            {
                case ObjetoSeleccionado.Monstruo:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Hedor = true;
                        GetBorderByIndex(fila + 1, columna).Background = Brushes.DarkGreen;
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Hedor = true;
                        GetBorderByIndex(fila - 1, columna).Background = Brushes.DarkGreen;
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Hedor = true;
                        GetBorderByIndex(fila, columna + 1).Background = Brushes.DarkGreen;
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Hedor = true;
                        GetBorderByIndex(fila, columna - 1).Background = Brushes.DarkGreen;
                    }
                    break;
                case ObjetoSeleccionado.Precipicio:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Brisa = true;
                        GetBorderByIndex(fila + 1, columna).Background = Brushes.LightBlue;
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Brisa = true;
                        GetBorderByIndex(fila - 1, columna).Background = Brushes.LightBlue;
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Brisa = true;
                        GetBorderByIndex(fila, columna + 1).Background = Brushes.LightBlue;
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Brisa = true;
                        GetBorderByIndex(fila, columna - 1).Background = Brushes.LightBlue;
                    }
                    break;
            }
        }

        private void DeleteAlrededores(int fila, int columna, ObjetoSeleccionado objetoSeleccionado)
        {
            switch (objetoSeleccionado)
            {
                case ObjetoSeleccionado.Monstruo:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Hedor = true;
                        GetBorderByIndex(fila + 1, columna).Background = Brushes.White;
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Hedor = true;
                        GetBorderByIndex(fila - 1, columna).Background = Brushes.White;
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Hedor = true;
                        GetBorderByIndex(fila, columna + 1).Background = Brushes.White;
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Hedor = true;
                        GetBorderByIndex(fila, columna - 1).Background = Brushes.White;
                    }
                    break;
                case ObjetoSeleccionado.Precipicio:
                    if (fila + 1 < _dimesionMapa)
                    {
                        Mapa[fila + 1, columna].Brisa = true;
                        GetBorderByIndex(fila + 1, columna).Background = Brushes.White;
                    }
                    if (fila - 1 >= 0)
                    {
                        Mapa[fila - 1, columna].Brisa = true;
                        GetBorderByIndex(fila - 1, columna).Background = Brushes.White;
                    }
                    if (columna + 1 < _dimesionMapa)
                    {
                        Mapa[fila, columna + 1].Brisa = true;
                        GetBorderByIndex(fila, columna + 1).Background = Brushes.White;
                    }
                    if (columna - 1 >= 0)
                    {
                        Mapa[fila, columna - 1].Brisa = true;
                        GetBorderByIndex(fila, columna - 1).Background = Brushes.White;
                    }
                    break;
            }
        }

        private void SetPosicionAgenteMapa(Agente agente)
        {
            Grid.SetRow(agente.Elemento, agente.Posicion.X);
            Grid.SetColumn(agente.Elemento, agente.Posicion.Y);
            
            this.Cueva.Children.Add(agente.Elemento);
        }

        private Border GetBorderByIndex(int fila, int columna)
        {
            var aux = Cueva.Children
                          .Cast<UIElement>()
                          .First(s => Grid.GetRow(s) == fila && Grid.GetColumn(s) == columna);

            return (Border)aux;
        }

        private void SeleccionMonstruo_Click(object sender, RoutedEventArgs e)
        {
            _objetoSeleccionado = ObjetoSeleccionado.Monstruo;
        }

        private void SeleccionPrecipicio_Click(object sender, RoutedEventArgs e)
        {
            _objetoSeleccionado = ObjetoSeleccionado.Precipicio;
        }

        private void SeleccionAgente_Click(object sender, RoutedEventArgs e)
        {
            _objetoSeleccionado = ObjetoSeleccionado.Agente;
        }

        private void SeleccionTesoro_Click(object sender, RoutedEventArgs e)
        {
            _objetoSeleccionado = ObjetoSeleccionado.Tesoro;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
        }
    }
}
