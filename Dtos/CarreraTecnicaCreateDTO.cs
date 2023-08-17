using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KalumManagement.Helpers;

namespace KalumManagement.Dtos
{
    public class CarreraTecnicaCreateDTO //: IValidatableObject
    {
        [Required(ErrorMessage = "el campo {0} es requerido")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "La cantidad minima de caracteres es {2} el maximo es {1} para el campo {0}")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var primeraLetra = Nombre[0].ToString();
            if (Nombre[0].ToString() != primeraLetra.ToUpper())
            {
                yield return new ValidationResult("La primera letra de la carrera técnica debe ser mayuscula", new string[] {nameof(Nombre)});
            }
        }
        */
    }
}