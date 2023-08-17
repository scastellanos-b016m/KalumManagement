using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KalumManagement.Entities;

namespace KalumManagement.Dtos
{
    public class AspiranteDTO
    {
        public string NoExpediente { get; set; }
        public string NombreCompleto { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        public string Email { get; set; }
        public string Estatus { get; set; } = "NO ASIGNADO";
        // public string CarreraId { get; set; }
        // public string JornadaId { get; set; }
        // public string ExamenId { get; set; }
        public virtual ExamenAdmisionDTO ExamenAdmision { get; set; }
        public virtual JornadaDTO Jornada { get; set; }
        public virtual CarreraTecnicaDTO CarrerasTecnica { get; set; }
    }
}