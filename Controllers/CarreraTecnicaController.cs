using Microsoft.AspNetCore.Mvc;
using KalumManagement.Entities;
using Microsoft.EntityFrameworkCore;
using KalumManagement.Dtos;
using KalumManagement.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/carreras-tecnicas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarreraTecnicaController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<CarreraTecnicaController> Logger;
        private readonly IMapper Mapper;
        
        public CarreraTecnicaController(KalumDBContext _dBContext, ILogger<CarreraTecnicaController> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;

        }

        // [HttpGet]
        // public List<CarreraTecnica> Get()
        // {
        //     return dBContext.CarrerasTecnicas.ToList();
        // }
        /*
        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<CarreraTecnicaListDTO>>> GetCarreraTecnicaPagination(int page)
        {
            var queryable = this.dBContext.CarrerasTecnicas.AsQueryable();
            int registros = await queryable.CountAsync();
            if (registros == 0)
            {
                return NoContent();
            }
            else
            {
                // var carrerasTecnicas = queryable.OrderBy(carrerasTecnicas => carrerasTecnicas.Nombre);
                // await IQueryableExtension.Pagination(carrerasTecnicas, 1).ToListAsync();
                var carrerasTecnicas = await queryable.OrderBy(carrerasTecnicas => carrerasTecnicas.Nombre).Pagination(page).ToListAsync();
            }
        }
        */

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<CarreraTecnica>>> GetCarreraTecnicaPagination(int page)
        {
                var queryable = this.dBContext.CarrerasTecnicas.AsQueryable();
                //var queryable = this.dBContext.CarrerasTecnicas.Include(ct => ct.Aspirantes).AsQueryable();
                int registros = await queryable.CountAsync();
                if(registros==0)
                {
                    return NoContent();
                }
                else
                {
                    //iQueryableExtensions.Pagination(queryable,page);
                    var carrerasTecnicas =  await queryable.OrderBy(carrerasTecnicas => carrerasTecnicas.Nombre).Pagination(page).ToListAsync();
                    PaginationResponse<CarreraTecnica> response = new PaginationResponse<CarreraTecnica>(carrerasTecnicas, page, registros);
                    return Ok(response);
                }
        }

        [HttpGet]
        // [ResponseCache(Duration = 15)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CarreraTecnicaListDTO>>> Get()
        {
            Logger.LogDebug("Iniciando proceso de consulta");
            List<CarreraTecnica> carreras = await this.dBContext.CarrerasTecnicas.Include(ct => ct.Aspirantes).ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta");
            if (carreras == null || carreras.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }

            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            List<CarreraTecnicaListDTO> lista = this.Mapper.Map<List<CarreraTecnicaListDTO>>(carreras);
            // Guid uuid = Guid.NewGuid();
            // return Ok(uuid.ToString());
            return Ok(lista);
        }

        [HttpGet("{id}", Name = "GetCarreraTecnica")]
        public async Task<ActionResult<CarreraTecnica>> GetCarreraTecnica(string id)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {id}");
            var carrera = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == id);
            if (carrera == null)
            {
                Logger.LogWarning($"No existe registro con el id: {id}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {id}");
            return Ok(carrera);
        }

        //comentado 19/01/2023 para poder utilizar los dto
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<CarreraTecnica>>> Get()
        // {
        //     Logger.LogDebug("Iniciando proceso de consulta");
        //     // List<CarreraTecnica> carreras = await this.dBContext.CarrerasTecnicas.ToListAsync();
        //     List<CarreraTecnica> carreras = await this.dBContext.CarrerasTecnicas.Include(ct => ct.Aspirante).ToListAsync();
        //     Logger.LogDebug("Finalizando el proceso de consulta");
        //     if (carreras == null || carreras.Count == 0)
        //     {
        //         Logger.LogDebug("No existen registros actualmente en la base de datos");
        //         return new NoContentResult();
        //     }
        //     Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
        //     return Ok(carreras);
        // }

        // [HttpGet("{id}", Name = "GetCarreraTecnica")]
        // public async Task<ActionResult<CarreraTecnica>> GetCarreraTecnica(string id)
        // {
        //     Logger.LogDebug($"Iniciando proceso de busqueda con id: {id}");
        //     var carrera = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == id);
        //     if (carrera == null)
        //     {
        //         Logger.LogWarning($"No existe registro con el id: {id}");
        //         return new NoContentResult();
        //     }
        //     Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {id}");
        //     return Ok(carrera);
        // }

        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<CarreraTecnicaCreateDTO>> Post([FromBody] CarreraTecnicaCreateDTO value)
        {
            Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
            CarreraTecnica elemento = this.Mapper.Map<CarreraTecnica>(value);            
            elemento.CarreraId = Guid.NewGuid().ToString().ToUpper();
            Logger.LogDebug($"Generación de llave con el valor {elemento.CarreraId}");
            await this.dBContext.CarrerasTecnicas.AddAsync(elemento);
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
            return new CreatedAtRouteResult("GetCarreraTecnica", new {id = elemento.CarreraId}, elemento);
        }
        //comentado para usar dto
        // [HttpPost]
        // [Route("post")]
        // public async Task<ActionResult<CarreraTecnica>> Post([FromBody] CarreraTecnica value)
        // {
        //     Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
        //     value.CarreraId = Guid.NewGuid().ToString().ToUpper();
        //     Logger.LogDebug($"Generación de llave con el valor {value.CarreraId}");
        //     await this.dBContext.CarrerasTecnicas.AddAsync(value);
        //     await this.dBContext.SaveChangesAsync();
        //     Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
        //     return new CreatedAtRouteResult("GetCarreraTecnica", new {id = value.CarreraId}, value);
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CarreraTecnica>> Delete(string id)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {id}");
            CarreraTecnica carreraTecnica = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == id);
            if (carreraTecnica == null)
            {
                Logger.LogInformation($"No existe información con el id: {id}");
                return NotFound();
            }
            else{
                this.dBContext.CarrerasTecnicas.Remove(carreraTecnica);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {id}");
                return carreraTecnica;
            }
        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult<CarreraTecnica>> Put(string id, [FromBody] CarreraTecnica value)
        // {
        //     Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
        //     CarreraTecnica carreraTecnica = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == id);
        //     if (carreraTecnica == null)
        //     {
        //         Logger.LogWarning($"No se encontro información para el id: {id}");
        //         return NotFound();
        //     }
        //     carreraTecnica.Nombre = value.Nombre;
        //     this.dBContext.Entry(carreraTecnica).State = EntityState.Modified;
        //     await this.dBContext.SaveChangesAsync();
        //     Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
        //     return NoContent();
        // }

        [HttpPut("{id}")]
        public async Task<ActionResult<CarreraTecnicaCreateDTO>> Put(string id, [FromBody] CarreraTecnicaCreateDTO value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            // CarreraTecnica carreraTecnica = await this.dBContext.CarrerasTecnicas.FirstOrDefaultAsync(ct => ct.CarreraId == id);
            CarreraTecnica elemento = this.Mapper.Map<CarreraTecnica>(value);
            if (elemento == null)
            {
                Logger.LogWarning($"No se encontro información para el id: {id}");
                return NotFound();
            }
            elemento.CarreraId = id;
            // elemento.Nombre = value.Nombre;
            this.dBContext.Entry(elemento).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}