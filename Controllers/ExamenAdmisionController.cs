using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KalumManagement.Entities;
using Microsoft.EntityFrameworkCore;
using KalumManagement.Dtos;
using AutoMapper;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/examenes-admision")]
    public class ExamenAdmisionController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<ExamenAdmisionController> Logger;
        private readonly IMapper Mapper;
        
        public ExamenAdmisionController(KalumDBContext _dBContext, ILogger<ExamenAdmisionController> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamenAdmision>>> Get()
        {
            Logger.LogDebug("Iniciando proceso de consulta");
            List<ExamenAdmision> examenesAdmision = await this.dBContext.ExamenesAdmision.ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta");
            if (examenesAdmision == null || examenesAdmision.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }
            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            return Ok(examenesAdmision);
        }

        [HttpGet("{id}", Name = "GetExamenAdmision")]
        public async Task<ActionResult<ExamenAdmision>> GetExamenAdmision(string id)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {id}");
            var examenAdmision = await this.dBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == id);
            if (examenAdmision == null)
            {
                Logger.LogWarning($"No existe registro con el id: {id}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {id}");
            return Ok(examenAdmision);
        }

        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<ExamenAdmisionCreateDTO>> Post([FromBody] ExamenAdmisionCreateDTO value)
        {
            Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
            ExamenAdmision elemento = this.Mapper.Map<ExamenAdmision>(value);
            // value.ExamenId = Guid.NewGuid().ToString().ToUpper();
            elemento.ExamenId = Guid.NewGuid().ToString().ToUpper();
            Logger.LogDebug($"Generación de llave con el valor {elemento.ExamenId}");
            await this.dBContext.ExamenesAdmision.AddAsync(elemento);
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
            return new CreatedAtRouteResult("GetExamenAdmision", new {id = elemento.ExamenId}, elemento);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ExamenAdmision>> Delete(string id)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {id}");
            ExamenAdmision examenAdmision = await this.dBContext.ExamenesAdmision.FirstOrDefaultAsync(ea => ea.ExamenId == id);
            if (examenAdmision == null)
            {
                Logger.LogInformation($"No existe información con el id: {id}");
                return NotFound();
            }
            else{
                this.dBContext.ExamenesAdmision.Remove(examenAdmision);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {id}");
                return examenAdmision;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ExamenAdmisionCreateDTO>> Put(string id, [FromBody] ExamenAdmisionCreateDTO value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            ExamenAdmision elemento = this.Mapper.Map<ExamenAdmision>(value);
            if (elemento == null)
            {
                Logger.LogWarning($"No se encontro información para el id: {id}");
                return NotFound();
            }
            elemento.ExamenId = id;
            this.dBContext.Entry(elemento).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}