using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Entities
{
    public class CarreraTecnica
    {
        [Required(ErrorMessage = "el campo {0} es requerido")]
        public string CarreraId { get; set; }
        [Required(ErrorMessage = "el campo {0} es requerido")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "La cantidad minima de caracteres es {2} el maximo es {1} para el campo {0}")]
        public string Nombre { get; set; }
        public virtual List<Aspirante> Aspirantes { get; set; }
        public virtual List<InversionCarreraTecnica> InversionCarreraTecnicas { get; set; }
    }
}