using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KalumManagement.Dtos;
using KalumManagement.Entities;
using KalumManagement.Models;
using KalumManagement.Services;
using KalumManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/alumnos")]
    public class AlumnoController : Controller
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<AlumnoController> Logger;
        private readonly IMapper Mapper;
        private readonly IQueueAlumnoService QueueAlumnoService;

        public AlumnoController(KalumDBContext _dBContext, ILogger<AlumnoController> _logger, IMapper _mapper, IQueueAlumnoService _queueAlumnoService)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;
            this.QueueAlumnoService = _queueAlumnoService;
        }

        [HttpPost]
        [Route("inscripcion")]
        public async Task<ActionResult<AlumnoDTO>> Post([FromBody] AlumnoCreateDTO alumnoCreateDTO)
        {
            this.Logger.LogDebug("Iniciando el proceso para almacenar un registro de alumno");
            CarreraTecnica carreraTecnica = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == alumnoCreateDTO.CarreraId);
            if (carreraTecnica == null)
            {
                this.Logger.LogDebug($"No existe la carrera técnica con el id{alumnoCreateDTO.CarreraId}");
                return BadRequest();
            }
            
            Cargo cargoInscripcion = await this.dBContext.Cargos.FirstOrDefaultAsync(c => c.CargoId == alumnoCreateDTO.InscripcionCargoId);
            if (cargoInscripcion == null)
            {
                this.Logger.LogDebug($"No existe el cargo con el id{alumnoCreateDTO.InscripcionCargoId}");
                return BadRequest();
            }

            Cargo cargoCarne = await this.dBContext.Cargos.FirstOrDefaultAsync(c => c.CargoId == alumnoCreateDTO.CarneCargoId);
            if (cargoCarne == null)
            {
                this.Logger.LogDebug($"No existe el cargo con el id{alumnoCreateDTO.CarneCargoId}");
                return BadRequest();
            }

            Cargo cargoMensual = await this.dBContext.Cargos.FirstOrDefaultAsync(c => c.CargoId == alumnoCreateDTO.CargoMensualId);
            if (cargoMensual == null)
            {
                this.Logger.LogDebug($"No existe el cargo con el id{alumnoCreateDTO.CargoMensualId}");
                return BadRequest();
            }

            bool resultado = await this.QueueAlumnoService.CrearSolicitudAlumnoAsync(alumnoCreateDTO);
            this.Logger.LogInformation("Se finalizo el proceso de registro de un alumno nuevo");
            
            AppLog appLog = new AppLog();
            appLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff"));

            AlumnoResponse alumnoResponse = null;
            if (resultado)
            {
                alumnoResponse = new AlumnoResponse()
                {
                    Estatus = "OK",
                    Mensaje = $"El proceso de su solicitud fue enviado con exito, pronto recibira una respuesta al correo {alumnoCreateDTO.Email}"
                    // Mensaje = $"El proceso de su solicitud fue enviado con exito, pronto recibira una respuesta al correo {aspiranteCreateDTO.Email}"
                };

                Utilerias.ImprimirLog(appLog, 201, alumnoResponse.Mensaje, "Information");
            }
            else
            {
                alumnoResponse = new AlumnoResponse()
                {
                    Estatus = "Error",
                    Mensaje = "Hubo un error en su solicitud, favor de contactar al administrador"
                };

                Utilerias.ImprimirLog(appLog, 204, alumnoResponse.Mensaje, "Error");
            }

            return Ok(alumnoResponse);
        }
    }
}