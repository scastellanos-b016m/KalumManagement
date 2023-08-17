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
    [Route("kalum-management/v1/inscripcion-pago")]
    public class InscripcionPagoController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<InscripcionPago> Logger;
        private readonly IMapper Mapper;
        
        public InscripcionPagoController(KalumDBContext _dBContext, ILogger<InscripcionPago> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;

        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<IEnumerable<InscripcionPago>>> GetInscripcionPagoPagination(int page)
        {
                var queryable = this.dBContext.InscripcionPagos.AsQueryable();
                //var queryable = this.dBContext.CarrerasTecnicas.Include(ct => ct.Aspirantes).AsQueryable();
                int registros = await queryable.CountAsync();
                if(registros==0)
                {
                    return NoContent();
                }
                else
                {
                    //iQueryableExtensions.Pagination(queryable,page);
                    var inscripcionPagos =  await queryable.OrderBy(inscripcionPagos => inscripcionPagos.NoExpediente).Pagination(page).ToListAsync();
                    PaginationResponse<InscripcionPago> response = new PaginationResponse<InscripcionPago>(inscripcionPagos, page, registros);
                    return Ok(response);
                }
        }
        
        [HttpGet("{boletapago}/{NoExpediente}/{Anio}", Name = "GetInscripcionPago")]
        public async Task<ActionResult<InscripcionPago>> GetInscripcionPago(string BoletaPago, string NoExpediente, string Anio)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {BoletaPago + NoExpediente + Anio}");
            // var resultadoExamenAdmision = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            var inscripcionPago = await Task.Run(() => dBContext.InscripcionPagos.SingleOrDefault(ip => ip.BoletaPago == BoletaPago && ip.NoExpediente == NoExpediente && ip.Anio == Anio));
            if (inscripcionPago == null)
            {
                Logger.LogWarning($"No existe registro con el id: {BoletaPago + NoExpediente + Anio}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {BoletaPago + NoExpediente + Anio}");
            return Ok(inscripcionPago);
        }

        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<InscripcionPago>> Post([FromBody] InscripcionPago value)
        {
            Logger.LogDebug($"Iniciando el proceso de registro con la siguiente información: {value}");
            await this.dBContext.InscripcionPagos.AddAsync(value);
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se ejecuto exitosamente el proceso de almacenamiento");
            return new CreatedAtRouteResult("GetInscripcionPago", new {BoletaPago = value.BoletaPago, NoExpediente = value.NoExpediente, Anio = value.Anio}, value);
        }

        [HttpDelete("{boletapago}/{NoExpediente}/{Anio}")]
        public async Task<ActionResult<InscripcionPago>> Delete(string BoletaPago, string NoExpediente, string Anio)
        {
            Logger.LogDebug($"Iniciando el proceso de eliminación con el id: {NoExpediente}");
            InscripcionPago inscripcionPago = await this.dBContext.InscripcionPagos.FirstOrDefaultAsync(ip => ip.BoletaPago == BoletaPago && ip.NoExpediente == NoExpediente && ip.Anio == Anio);
            if (inscripcionPago == null)
            {
                Logger.LogInformation($"No existe información con el id: {BoletaPago + NoExpediente + Anio}");
                return NotFound();
            }
            else{
                this.dBContext.InscripcionPagos.Remove(inscripcionPago);
                await this.dBContext.SaveChangesAsync();
                Logger.LogDebug($"Se ejecuto exitosamente el proceso de eliminación con el id: {BoletaPago + NoExpediente + Anio}");
                return inscripcionPago;
            }
        }

        [HttpPut("{boletapago}/{NoExpediente}/{Anio}")]
        public async Task<ActionResult<InscripcionPago>> Put(string BoletaPago, string NoExpediente, string Anio, [FromBody] InscripcionPago value)
        {
            Logger.LogDebug($"Iniciando proceso de actualización con la información: {value}");
            InscripcionPago inscripcionPago = await this.dBContext.InscripcionPagos.FirstOrDefaultAsync(ip => ip.BoletaPago == BoletaPago && ip.NoExpediente == NoExpediente && ip.Anio == Anio);
            if (inscripcionPago == null)
            {
                Logger.LogDebug($"No se encontro información para el id: {BoletaPago + NoExpediente + Anio}");
                return NotFound();
            }
            inscripcionPago.FechaPago = value.FechaPago;
            inscripcionPago.Monto = value.Monto;
            this.dBContext.Entry(inscripcionPago).State = EntityState.Modified;
            await this.dBContext.SaveChangesAsync();
            Logger.LogInformation($"Se proceso exitosamente la actualización de datos");
            return NoContent();
        }
    }
}