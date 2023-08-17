using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos
{
    public class AlumnoCreateDTO
    {
        public string NoExpediente { get; set; }
        public string Ciclo { get; set; }
        public int MesInicioPago { get; set; }
        public string CarreraId { get; set; }
        public string InscripcionCargoId { get; set; }
        public string CarneCargoId { get; set; }
        public string CargoMensualId { get; set; }
        public string DiaPago { get; set; }
        public string Email { get; set; }
    }
}