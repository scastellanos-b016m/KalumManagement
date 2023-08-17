using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Entities
{
    public class InscripcionPago
    {
        public string BoletaPago { get; set; }
        public string NoExpediente { get; set; }
        public string Anio { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
    }
}