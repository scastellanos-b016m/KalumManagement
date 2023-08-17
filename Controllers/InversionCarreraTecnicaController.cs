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
    [Route("kalum-management/v1/inversion-carrera-tecnica")]
    public class InversionCarreraTecnicaController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<InversionCarreraTecnica> Logger;
        private readonly IMapper Mapper;
        
        public InversionCarreraTecnicaController(KalumDBContext _dBContext, ILogger<InversionCarreraTecnica> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;

        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<InversionCarreraTecnica>>> GetInversionCarreraTecnicaPagination(int page)
        {
                //var queryable = this.dBContext.InversionCarreraTecnicas.Include(ict => ict.CarrerasTecnicas).AsQueryable();
                var queryable = this.dBContext.InversionCarreraTecnicas.AsQueryable();
                int registros = await queryable.CountAsync();
                if(registros==0)
                {
                    return NoContent();
                }
                else
                {
                    //iQueryableExtensions.Pagination(queryable,page);
                    var inversionCarreraTecnica =  await queryable.OrderBy(inversionCarreraTecnica => inversionCarreraTecnica.InversionId).Pagination(page).ToListAsync();
                    PaginationResponse<InversionCarreraTecnica> response = new PaginationResponse<InversionCarreraTecnica>(inversionCarreraTecnica, page, registros);
                    return Ok(response);
                }
        }
        
        [HttpGet("{InversionId}", Name = "GetInversionCarreraTecnica")]
        public async Task<ActionResult<InversionCarreraTecnica>> GetInversionCarreraTecnica(string InversionId)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {InversionId}");
            // var resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            var inversionCarreraTecnica = await Task.Run(() => dBContext.InversionCarreraTecnicas.SingleOrDefault(ict => ict.InversionId == InversionId));
            if (inversionCarreraTecnica == null)
            {
                Logger.LogWarning($"No existe registro con el id: {InversionId}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {InversionId}");
            return Ok(inversionCarreraTecnica);
        }

        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<InversionCarreraTecnica>> Post([FromBody] InversionCarreraTecnica value)
        {
            Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
            value.InversionId = Guid.NewGuid().ToString().ToUpper();
            await this.dBContext.InversionCarreraTecnicas.AddAsync(value);
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
            return new CreatedAtRouteResult("GetInversionCarreraTecnica", new {InversionId = value.InversionId}, value);
        }

        [HttpDelete("{InversionId}")]
        public async Task<ActionResult<InversionCarreraTecnica>> Delete(string InversionId)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {InversionId}");
            InversionCarreraTecnica inversionCarreraTecnica = await this.dBContext.InversionCarreraTecnicas.FirstOrDefaultAsync(ict => ict.InversionId == InversionId);
            if (inversionCarreraTecnica == null)
            {
                Logger.LogInformation($"No existe información con el id: {InversionId}");
                return NotFound();
            }
            else{
                this.dBContext.InversionCarreraTecnicas.Remove(inversionCarreraTecnica);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {InversionId}");
                return inversionCarreraTecnica;
            }
        }

        [HttpPut("{InversionId}")]
        public async Task<ActionResult<InversionCarreraTecnica>> Put(string InversionId, [FromBody] InversionCarreraTecnica value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            InversionCarreraTecnica inversionCarreraTecnica = await this.dBContext.InversionCarreraTecnicas.FirstOrDefaultAsync(ict => ict.InversionId == InversionId);
            if (inversionCarreraTecnica == null)
            {
                Logger.LogDebug($"No se encontro información para el id: {InversionId}");
                return NotFound();
            }
            inversionCarreraTecnica.CarreraId = value.CarreraId;
            inversionCarreraTecnica.MontoInscripcion = value.MontoInscripcion;
            inversionCarreraTecnica.NumeroPagos = value.NumeroPagos;
            inversionCarreraTecnica.MontoPagos = value.MontoPagos;
            this.dBContext.Entry(inversionCarreraTecnica).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}