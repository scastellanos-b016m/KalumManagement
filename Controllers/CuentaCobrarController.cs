using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KalumManagement.Dtos;
using KalumManagement.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KalumManagement.Controllers
{
    [ApiController]
    [Route("kalum-management/v1/cuentas-cobrar")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuentaCobrarController : ControllerBase
    {
        private readonly KalumDBContext dBContext;
        private readonly ILogger<CuentaCobrarController> Logger;
        private readonly IMapper Mapper;

        public CuentaCobrarController(KalumDBContext _dBContext, ILogger <CuentaCobrarController> _logger, IMapper _mapper)
        {
            this.dBContext = _dBContext;
            this.Logger = _logger;
            this.Mapper = _mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuentaCobrarListDTO>>> Get()
        {
            Logger.LogDebug("Iniciando proceso de consulta");
            List<CuentaCobrar> cuentasCobrar = await this.dBContext.CuentasCobrar.ToListAsync();
            Logger.LogDebug("Finalizando el proceso de consulta");
            if (cuentasCobrar == null || cuentasCobrar.Count == 0)
            {
                Logger.LogDebug("No existen registros actualmente en la base de datos");
                return new NoContentResult();
            }
            Logger.LogDebug("Se ejecuto de forma exitosa la consulta de la información");
            List<CuentaCobrarListDTO> lista = this.Mapper.Map<List<CuentaCobrarListDTO>>(cuentasCobrar);

            return Ok(lista);
        }

        [HttpGet("{Carne}", Name = "GetCuentasCobrar")]
        public async Task<ActionResult<CuentaCobrar>> GetCuentasCobrar(string Carne)
        {
            Logger.LogDebug($"Iniciando proceso de busqueda con id: {Carne}");
            // var resultadoCuentasCobrar = await this.dBContext.ResultadosExamenAdmision.FirstOrDefaultAsync(rea => rea.NoExpediente == NoExpediente && rea.Anio == Anio);
            // var resultadoCuentasCobrar = await Task.Run(() => dBContext.CuentasCobrar.SingleOrDefault(cc.Carne == Carne));
            var resultadoCuentasCobrar = await this.dBContext.CuentasCobrar.Where(cc => cc.Carne == Carne).ToListAsync();
            if (resultadoCuentasCobrar == null)
            {
                Logger.LogWarning($"No existe registro con el id: {Carne}");
                return new NoContentResult();
            }
            Logger.LogInformation($"Se ejecuto exitosamente la consulta con el id: {Carne}");
            List<CuentaCobrarListDTO> lista = this.Mapper.Map<List<CuentaCobrarListDTO>>(resultadoCuentasCobrar);
            return Ok(lista);
        }
    }
}