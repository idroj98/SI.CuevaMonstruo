using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SICuevaMonstruo
{
    public class MovimientosCasilla
    {
        public bool Visitada { get; set; }
        public bool Descubierta { get; set; }
        public bool DerechaDone { get; set; }
        public bool AbajoDone { get; set; }
        public bool IzquierdaDone { get; set; }
        public bool ArribaDone { get; set; }
        public bool movimientosHechos { get; set; }
        public bool Disparada { get; set; }

    }
}
