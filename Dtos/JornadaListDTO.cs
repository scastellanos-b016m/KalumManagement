using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos
{
    public class JornadaListDTO
    {
        public string JornadaId { get; set; }
        public string NombreCorto { get; set; }
        public string Descripcion { get; set; }
        public virtual List<JornadaAspiranteListDTO> Aspirantes { get; set; }
    }
}