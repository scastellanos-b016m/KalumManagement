using System.ComponentModel.DataAnnotations;
using KalumManagement.Helpers;

namespace KalumManagement.Entities
{
    public class Aspirante
    {
        public string NoExpediente { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Estatus { get; set; } = "NO ASIGNADO";
        public string CarreraId { get; set; }
        public string JornadaId { get; set; }
        public string ExamenId { get; set; }
        // [NotMapped] para que no lo mapee a la base de datos
        // public string Edad { get; set; }
        public virtual ExamenAdmision ExamenAdmision { get; set; }
        public virtual Jornada Jornada { get; set; }
        public virtual CarreraTecnica CarrerasTecnica { get; set; }
    }
}