using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KalumManagement.Dtos;

namespace KalumManagement.Services
{
    public interface IQueueAlumnoService
    {
        public Task<bool> CrearSolicitudAlumnoAsync(AlumnoCreateDTO alumno);
    }
}