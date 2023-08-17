using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KalumManagement.Dtos;

namespace KalumManagement.Services
{
    public interface IQueueAspiranteService
    {
        public Task<bool> CrearSolicitudAspiranteAsync(AspiranteCreateDTO aspirante);

    }
}