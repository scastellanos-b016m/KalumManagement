using Microsoft.AspNetCore.Mvc;
using KalumManagement.Entities;
using Microsoft.EntityFrameworkCore;
using KalumManagement.Dtos;
using AutoMapper;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/jornadas")]
    public class JornadaController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<JornadaController> Logger;
        private readonly IMapper Mapper;
        
        public JornadaController(KalumDBContext _dBContext, ILogger<JornadaController> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;

        }

         [HttpGet]
        public async Task<ActionResult<IEnumerable<JornadaListDTO>>> Get()
        {
            Logger.LogDebug("Iniciando proceso de consulta jornada");
            List<Jornada> jornadas = await dBContext.Jornadas.Include(j => j.Aspirantes).ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta jornada");
            if (jornadas == null || jornadas.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }
            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            
            List<JornadaListDTO> lista = this.Mapper.Map<List<JornadaListDTO>>(jornadas);

            return Ok(lista);
        }

        //Comentado para usar DTO
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Jornada>>> Get()
        // {
        //     Logger.LogDebug("Iniciando proceso de consulta jornada");
        //     List<Jornada> jornadas = await dBContext.Jornadas.ToListAsync();
        //     Logger.LogDebug("Finalizando el proceso de consulta jornada");
        //     if (jornadas == null || jornadas.Count == 0)
        //     {
        //         Logger.LogDebug("No existen registros actualmente en la base de datos");
        //         return new NoContentResult();
        //     }
        //     Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
        //     return Ok(jornadas);
        // }

        [HttpGet("{id}", Name = "GetJornada")]
        public async Task<ActionResult<Jornada>> GetJornada(string id)
        {
            var jornada = await dBContext.Jornadas.FirstOrDefaultAsync(j => j.JornadaId == id);
            if (jornada == null)
            {
                return new NoContentResult();
            }
            return Ok(jornada);
        }

        // [HttpPost]
        // [Route("post")]
        // public async Task<ActionResult<Jornada>> Post([FromBody] Jornada value)
        // {
        //     value.JornadaId = Guid.NewGuid().ToString().ToUpper();
        //     await dBContext.Jornadas.AddAsync(value);
        //     await dBContext.SaveChangesAsync();
        //     return new CreatedAtRouteResult("GetJornada", new {id = value.JornadaId}, value);
        // }
        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<JornadaCreateDTO>> Post([FromBody] JornadaCreateDTO value)
        {
            Jornada elemento = this.Mapper.Map<Jornada>(value);
            elemento.JornadaId = Guid.NewGuid().ToString().ToUpper();
            await dBContext.Jornadas.AddAsync(elemento);
            await dBContext.SaveChangesAsync();
            return new CreatedAtRouteResult("GetJornada", new {id = elemento.JornadaId}, elemento);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Jornada>> Delete(string id)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {id}");
            Jornada jornada = await this.dBContext.Jornadas.FirstOrDefaultAsync(ct => ct.JornadaId == id);
            if (jornada == null)
            {
                Logger.LogInformation($"No existe información con el id: {id}");
                return NotFound();
            }
            else{
                this.dBContext.Jornadas.Remove(jornada);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {id}");
                return jornada;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<JornadaCreateDTO>> Put(string id, [FromBody] JornadaCreateDTO value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            Jornada elemento = this.Mapper.Map<Jornada>(value);
            if (elemento == null)
            {
                Logger.LogWarning($"No se encontro información para el id: {id}");
                return NotFound();
            }
            elemento.JornadaId = id;
            // elemento.NombreCorto = value.NombreCorto;
            // elemento.Descripcion = value.Descripcion;
            this.dBContext.Entry(elemento).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}