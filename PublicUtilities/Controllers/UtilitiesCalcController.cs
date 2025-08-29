using Microsoft.AspNetCore.Mvc;
using PublicUtilities.Dtos.UtilitiesCalculation;
using PublicUtilities.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PublicUtilities.Controllers;

public class UtilitiesCalcController : ControllerBase
{
    private readonly IUtilitiesCalcService _utilitiesService;

    public UtilitiesCalcController(IUtilitiesCalcService utilitiesService)
    {
        _utilitiesService = utilitiesService;
    }

    [HttpPost("calculation")]
    [SwaggerOperation(Summary = "Расчет начислений")]
    [SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, расчет по показаниям", typeof(UtilitiesCalcResultDto))]
    public async Task<ActionResult<UtilitiesCalcResultDto>> UtilitiesCalculation(UtilitiesCalcDto utilities, [FromBody] List<ResidentPeriodDto> periods)
    {
        var result = await _utilitiesService.UtilitiesCalcProcess(utilities, periods);
        if (!result.IsCompleted) return BadRequest(result);

        return Ok(result);
    }
}
