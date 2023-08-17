using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Dtos
{
    public class PaginationResponse<T> : PaginacionDTO<T>
    {
        public PaginationResponse(List<T> source, int number, int registros)
        {
            this.Number = number;
            int cantidadRegistrosPorPagina = 5;
            int totalRegistros = registros;
            this.TotalPages = (int)Math.Ceiling((Double)totalRegistros / cantidadRegistrosPorPagina);
            this.Content = source;
            if (number == 0)
            {
                this.First = true;
            }
            else if ((this.Number + 1) == this.TotalPages)
            {
                this.Last = true;
            }
        }
    }
}