using Microsoft.AspNetCore.Mvc;
using OrdenesInversion.Models;
using OrdenesInversion.Services;

namespace OrdenesInversion.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdenesController : ControllerBase
    {
        private readonly ILogger<OrdenesController> _logger;
        private readonly OrdenesService _ordenesService;

        public OrdenesController(ILogger<OrdenesController> logger, OrdenesService ordenesService)
        {
            _logger = logger;
            _ordenesService = ordenesService;
        }

        [HttpPost("[action]")]
        public IActionResult Create([FromBody] OrdenInversionRequest orden)
        {
            try
            {
                Dictionary<string, string> errors = _ordenesService.validateParamsOrden(orden);
                if (errors.Count > 0)
                {
                    return StatusCode(422, errors);
                }

                if (!_ordenesService.addOrden(orden))
                    return BadRequest("Se produjo un error al intentar almacenar la orden en la base de datos.");

                return Ok("Orden creada con éxito.");
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        public IActionResult Read([FromQuery] int idOrden)
        {
            try
            {
                OrdenInversion orden = _ordenesService.GetOrden(idOrden);
                return Ok(orden == null ? "Orden no encontrada en la base de datos." : orden);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("[action]")]
        public IActionResult Update([FromQuery] int idOrden, int estado)
        {
            try
            {
                if (!Enum.IsDefined(typeof(OrdenEstados), estado))
                    return StatusCode(422, "El estado indicado no es válido.");

                return Ok(_ordenesService.UpdateOrden(idOrden, estado) ? "Orden actualizada con éxito." : "Orden no encontrada en base de datos.");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("[action]")]
        public IActionResult Delete([FromQuery] int idOrden)
        {
            try
            {
                return Ok(_ordenesService.DeleteOrden(idOrden) ? "Orden eliminada con éxito." : "Orden no encontrada en base de datos.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
