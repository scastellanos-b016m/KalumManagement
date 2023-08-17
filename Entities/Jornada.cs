using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Entities
{
    public class Jornada
    {
        [Required(ErrorMessage = "El Campo {0} es requerido")]
        public string JornadaId { get; set; }
        [Required(ErrorMessage = "El Campo {0 }es requerido")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "La cantidad Minima de caracteres es {2} y el máximo es {1} para el campo {0}")]
        public string NombreCorto { get; set; }
        public string Descripcion { get; set; }
        public virtual List<Aspirante> Aspirantes { get; set; }
    }
}