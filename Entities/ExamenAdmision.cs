using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Entities
{
    public class ExamenAdmision
    {
        [Required(ErrorMessage = "el campo {0} es requerido")]
        public string ExamenId { get; set; }
        [Required(ErrorMessage = "el campo {0} es requerido")]
        public DateTime FechaExamen { get; set; }
        public virtual List<Aspirante> Aspirante { get; set; }
    }
}