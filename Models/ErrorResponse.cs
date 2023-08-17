using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Models
{
    public class ErrorResponse
    {
        public string TipoError { get; set; }
        public string HttpStatusCode { get; set; }
        public string Mensaje { get; set; }
    }
}