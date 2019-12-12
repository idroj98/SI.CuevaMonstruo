namespace SICuevaMonstruo
{
    public class Celda
    {
        public bool Resplandor { get; set; }
        public int Brisa { get; set; }
        public int Hedor { get; set; }
        public bool Monstruo { get; set; }
        public bool Precipicio { get; set; }
        public bool HasHedor => Hedor > 0;
        public bool HasBrisa => Brisa > 0;
        public Agente Agente { get; set; }
    }
}
