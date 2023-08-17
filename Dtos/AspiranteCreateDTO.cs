using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KalumManagement.Helpers;

namespace KalumManagement.Dtos
{
    public class AspiranteCreateDTO
    {
        // [Required(ErrorMessage = "El campo {0} es requerido")]
        // [NoExpediente]
        // public string NoExpediente { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress]
        public string Email { get; set; }
        public string Estatus { get; set; } = "NO ASIGNADO";
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string CarreraId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string JornadaId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ExamenId { get; set; }
    }
}