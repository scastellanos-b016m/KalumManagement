using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos
{
    public class CuentaCobrarListDTO
    {
        public string Correlativo { get; set; }
        public string Anio { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAplica { get; set; }
        public decimal Monto { get; set; }
        public decimal Mora { get; set; }
        public decimal Descuento { get; set; }
    }
}