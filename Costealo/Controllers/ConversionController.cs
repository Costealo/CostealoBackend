using System.Threading.Tasks;
using CostealoBackend.Dto;
using CostealoBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CostealoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]  // Actívalo si querés proteger este endpoint con JWT
    public class ConversionController : ControllerBase
    {
        private readonly IUnitConversionService _svc;

        public ConversionController(IUnitConversionService svc)
        {
            _svc = svc;
        }

        [HttpPost("convert")]
        public async Task<ActionResult<UnitConversionResponseDto>> Convert(UnitConversionRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Type) ||
                string.IsNullOrWhiteSpace(dto.FromUnit) ||
                string.IsNullOrWhiteSpace(dto.ToUnit))
                return BadRequest("Faltan parámetros obligatorios: type, fromUnit, toUnit, fromValue.");

            var result = await _svc.ConvertAsync(dto);
            return Ok(result);
        }
    }
}