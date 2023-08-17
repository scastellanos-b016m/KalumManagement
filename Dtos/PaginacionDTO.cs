using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos
{
    public class PaginacionDTO<T> //Tipo de dato generico
    {
        public int Number { get; set; }
        public int TotalPages { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }   
        public List<T> Content { get; set; }  //Tipo de dato generico
    }
}