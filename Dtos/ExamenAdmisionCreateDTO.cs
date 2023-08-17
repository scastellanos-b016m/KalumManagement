using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos
{
    public class ExamenAdmisionCreateDTO
    {
        [Required(ErrorMessage = "el campo {0} es requerido")]
        public DateTime FechaExamen { get; set; }
    }
}