using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KalumManagement.Dtos;
using KalumManagement.Entities;
using KalumManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/resultado-examen-admision")]
    public class ResultadoExamenAdmisionController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<ResultadoExamenAdmision> Logger;
        private readonly IMapper Mapper;
        
        public ResultadoExamenAdmisionController(KalumDBContext _dBContext, ILogger<ResultadoExamenAdmision> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;

        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<ResultadoExamenAdmision>>> GetResultadoExamenAdmisionPagination(int page)
        {
                var queryable = this.dBContext.ResultadosExamenAdmision.AsQueryable();
                //var queryable = this.dBContext.CarrerasTecnicas.Include(ct => ct.Aspirantes).AsQueryable();
                int registros = await queryable.CountAsync();
                if(registros==0)
                {
                    return NoContent();
                }
                else
                {
                    //iQueryableExtensions.Pagination(queryable,page);
                    var resultadoExamenAdmision =  await queryable.OrderBy(resultadoExamenAdmision => resultadoExamenAdmision.NoExpediente).Pagination(page).ToListAsync();
                    PaginationResponse<ResultadoExamenAdmision> response = new PaginationResponse<ResultadoExamenAdmision>(resultadoExamenAdmision, page, registros);
                    return Ok(response);
                }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultadoExamenAdmision>>> Get()
        {
            Logger.LogDebug("Iniciando proceso de consulta");
            List<ResultadoExamenAdmision> resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta");
            if (resultadoExamenAdmision == null || resultadoExamenAdmision.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }
            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            return Ok(resultadoExamenAdmision);
        }
        
        [HttpGet("{NoExpediente}/{Anio}", Name = "GetResultadoExamenAdmision")]
        public async Task<ActionResult<ResultadoExamenAdmision>> GetResultadoExamenAdmision(string NoExpediente, string Anio)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {NoExpediente + Anio}");
            // var resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            var resultadoExamenAdmision = await Task.Run(() => dBContext.ResultadosExamenAdmision.SingleOrDefault(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio));
            if (resultadoExamenAdmision == null)
            {
                Logger.LogWarning($"No existe registro con el id: {NoExpediente + Anio}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {NoExpediente + Anio}");
            return Ok(resultadoExamenAdmision);
        }

        [HttpGet("{NoExpediente}", Name = "GetResultadoExamenAdmisionPorExpediente")]
        public async Task<ActionResult<ResultadoExamenAdmision>> GetResultadoExamenAdmisionPorExpediente(string NoExpediente)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {NoExpediente}");
            // var resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            var resultadoExamenAdmision = await Task.Run(() => dBContext.ResultadosExamenAdmision.Where(rea => rea.NoExpediente == NoExpediente).ToListAsync());
            if (resultadoExamenAdmision == null)
            {
                Logger.LogWarning($"No existe registro con el id: {NoExpediente}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {NoExpediente}");
            return Ok(resultadoExamenAdmision);
        }

        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<ResultadoExamenAdmision>> Post([FromBody] ResultadoExamenAdmision value)
        {
            Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
            await this.dBContext.ResultadosExamenAdmision.AddAsync(value);
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
            return new CreatedAtRouteResult("GetResultadoExamenAdmision", new {NoExpediente = value.NoExpediente, Anio = value.Anio}, value);
        }

        [HttpDelete("{NoExpediente}/{Anio}")]
        public async Task<ActionResult<ResultadoExamenAdmision>> Delete(string NoExpediente, string Anio)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {NoExpediente}");
            ResultadoExamenAdmision resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            if (resultadoExamenAdmision == null)
            {
                Logger.LogInformation($"No existe información con el id: {NoExpediente}");
                return NotFound();
            }
            else{
                this.dBContext.ResultadosExamenAdmision.Remove(resultadoExamenAdmision);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {NoExpediente}");
                return resultadoExamenAdmision;
            }
        }

        [HttpPut("{NoExpediente}/{Anio}")]
        public async Task<ActionResult<ResultadoExamenAdmision>> Put(string NoExpediente, string Anio, [FromBody] ResultadoExamenAdmision value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            ResultadoExamenAdmision resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            if (resultadoExamenAdmision == null)
            {
                Logger.LogDebug($"No se encontro información para el id: {NoExpediente}");
                return NotFound();
            }
            resultadoExamenAdmision.Descripcion = value.Descripcion;
            resultadoExamenAdmision.Nota = value.Nota;
            this.dBContext.Entry(resultadoExamenAdmision).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}