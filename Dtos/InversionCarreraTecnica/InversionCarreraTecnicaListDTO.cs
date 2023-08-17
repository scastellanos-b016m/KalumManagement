using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos.InversionCarreraTecnica
{
    public class InversionCarreraTecnicaListDTO
    {
        public string InversionId { get; set; }
        public string CarreraId { get; set; }
        public decimal MontoInscripcion { get; set; }
        public int NumeroPagos { get; set; }
        public decimal MontoPagos { get; set; }
        public List<CarreraTecnicaDTO> CarrerasTecnicas { get; set; }
    }
}