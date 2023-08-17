using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/aspirantes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AspiranteController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<AspiranteController> Logger;
        private readonly IMapper Mapper;
        private readonly IQueueAspiranteService QueueAspiranteService; 
        private readonly IQueueAlumnoService QueueAlumnoService;
        public AspiranteController(KalumDBContext _dBContext, ILogger<AspiranteController> _logger, IMapper _mapper, IQueueAspiranteService _queueService, IQueueAlumnoService _queueAlumnoService)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;
            this.QueueAspiranteService = _queueService;
            this.QueueAlumnoService = _queueAlumnoService;
        }

        [HttpPost]
        [Route("alumno")]
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
                    Mensaje = $"El proceso de su solicitud fue enviado con exito, pronto recibira una respuesta al correo xxx@gmail.com"
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aspirante>>> Get()
        {
            Logger.LogDebug("Iniciando proceso de consulta");
            List<Aspirante> aspirantes = await this.dBContext.Aspirantes.ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta");
            if (aspirantes == null || aspirantes.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }

            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            //List<CarreraTecnicaListDTO> lista = this.Mapper.Map<List<CarreraTecnicaListDTO>>(carreras);
            return Ok(aspirantes);
        }

        [HttpGet]
        [Route("GetAspiranteAlumno")]
        public async Task<ActionResult<IEnumerable<AspiranteAlumnoDTO>>> GetAspiranteAlumno()
        {
            Logger.LogDebug("Iniciando proceso de consulta");
            // List<Aspirante> aspirantes = await this.dBContext.Aspirantes.ToListAsync();
            List<Aspirante> aspirantes = await this.dBContext.Aspirantes.Where(a => a.Estatus == "NO ASIGNADO").ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta");
            if (aspirantes == null || aspirantes.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }

            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            List<AspiranteAlumnoDTO> lista = this.Mapper.Map<List<AspiranteAlumnoDTO>>(aspirantes);
            return Ok(lista);
        }

        [HttpGet("{id}", Name = "GetAspirante")]
        public async Task<ActionResult<Aspirante>> GetAspirante(string id)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {id}");
            var aspirante = await this.dBContext.Aspirantes
                .Include(a => a.ExamenAdmision).Include(a => a.Jornada)
                .Include(a => a.CarrerasTecnica).FirstOrDefaultAsync(ct => ct.NoExpediente == id);
            if (aspirante == null)
            {
                Logger.LogWarning($"No existe registro con el id: {id}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {id}");
            return Ok(aspirante);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<Aspirante>>> GetAspirantePagination(int page)
        {
                var queryable = this.dBContext.Aspirantes.AsQueryable();
                int registros = await queryable.CountAsync();
                if(registros==0)
                {
                    return NoContent();
                }
                else
                {
                    //iQueryableExtensions.Pagination(queryable,page);
                    var aspirantes =  await queryable.OrderBy(aspirantes => aspirantes.Nombre).Pagination(page).ToListAsync();
                    PaginationResponse<Aspirante> response = new PaginationResponse<Aspirante>(aspirantes, page, registros);
                    return Ok(response);
                }
        }

        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<AspiranteDTO>> Post([FromBody] AspiranteCreateDTO aspiranteCreateDTO)
        {
            // // Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
            // Aspirante elemento = this.Mapper.Map<Aspirante>(value);            
            // //elemento.NoExpediente = Guid.NewGuid().ToString().ToUpper();
            // // Logger.LogDebug($"Generación de llave con el valor {elemento.CarreraId}");
            // await this.dBContext.Aspirantes.AddAsync(elemento);
            // await this.dBContext.SaveChangesAsync();
            // // Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
            // return new CreatedAtRouteResult("GetCarreraTecnica", new {id = elemento.CarreraId}, elemento);

            this.Logger.LogDebug("Iniciando el proceso para almacenar un registro de aspirante");
            CarreraTecnica carreraTecnica = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == aspiranteCreateDTO.CarreraId);
            if (carreraTecnica == null)
            {
                this.Logger.LogDebug($"No existe la carrera técnica con el id{aspiranteCreateDTO.CarreraId}");
                return BadRequest();
            }
            Jornada jornada = await this.dBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == aspiranteCreateDTO.JornadaId);
            if (jornada == null)
            {
                this.Logger.LogDebug($"No existe la carrera técnica con el id{aspiranteCreateDTO.JornadaId}");
                return BadRequest();
            }
            ExamenAdmision examenAdmision = await this.dBContext.ExamenesAdmision.FirstOrDefaultAsync(ex => ex.ExamenId == aspiranteCreateDTO.ExamenId);
            if (jornada == null)
            {
                this.Logger.LogDebug($"No existe la carrera técnica con el id{aspiranteCreateDTO.ExamenId}");
                return BadRequest();
            }


            bool resultado = await this.QueueAspiranteService.CrearSolicitudAspiranteAsync(aspiranteCreateDTO);
            this.Logger.LogInformation("Se finalizo el proceso de registro de un aspirante nuevo");
            
            AppLog appLog = new AppLog();
            appLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff"));

            AspiranteResponse aspiranteResponse = null;
            if (resultado)
            {
                aspiranteResponse = new AspiranteResponse()
                {
                    Estatus = "OK",
                    Mensaje = $"El proceso de su solicitud fue enviado con exito, pronto recibira una respuesta al correo {aspiranteCreateDTO.Email}"
                };

                Utilerias.ImprimirLog(appLog, 201, aspiranteResponse.Mensaje, "Information");
            }
            else
            {
                aspiranteResponse = new AspiranteResponse()
                {
                    Estatus = "Error",
                    Mensaje = "Hubo un error en su solicitud, favor de contactar al administrador"
                };

                Utilerias.ImprimirLog(appLog, 204, aspiranteResponse.Mensaje, "Error");
            }

            return Ok(aspiranteResponse);

            // Aspirante aspirante = this.Mapper.Map<Aspirante>(aspiranteCreateDTO);            
            // await this.dBContext.Aspirantes.AddAsync(aspirante);
            // await this.dBContext.SaveChangesAsync();
            // AspiranteDTO aspiranteDTO = Mapper.Map<AspiranteDTO>(aspirante);
            // return Ok(aspiranteDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Aspirante>> Delete(string id)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {id}");
            Aspirante aspirante = await this.dBContext.Aspirantes.FirstOrDefaultAsync(ct => ct.NoExpediente == id);
            if (aspirante == null)
            {
                Logger.LogInformation($"No existe información con el id: {id}");
                return NotFound();
            }
            else{
                this.dBContext.Aspirantes.Remove(aspirante);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {id}");
                return aspirante;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Aspirante>> Put(string id, [FromBody] Aspirante value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            Aspirante aspirante = await this.dBContext.Aspirantes.FirstOrDefaultAsync(ct => ct.NoExpediente == id);
            if (aspirante == null)
            {
                Logger.LogWarning($"No se encontro información para el id: {id}");
                return NotFound();
            }
            aspirante.Apellido = value.Apellido;
            aspirante.Nombre = value.Nombre;
            aspirante.Direccion = value.Direccion;
            aspirante.Telefono = value.Telefono;
            aspirante.Email = value.Email;
            aspirante.CarreraId = value.CarreraId;
            aspirante.JornadaId = value.JornadaId;
            aspirante.ExamenId = value.ExamenId;
            this.dBContext.Entry(aspirante).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}