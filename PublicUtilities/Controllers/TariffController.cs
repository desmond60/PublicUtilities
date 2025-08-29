using Microsoft.AspNetCore.Mvc;
using PublicUtilities.Dtos.Tariffs;
using PublicUtilities.Models;
using PublicUtilities.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PublicUtilities.Controllers;

public class TariffController : ControllerBase
{
    private readonly ITariffService _tariffService;

    public TariffController(ITariffService tariffService)
    {
        _tariffService = tariffService;
    }

    //[HttpPost("createTariff")]
    //[SwaggerOperation(Summary = "Создание тарифа")]
    //[SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, созданный тариф", typeof(CreateTariffResultDto))]
    //[SwaggerResponse(StatusCodes.Status400BadRequest, "Не получилось создать тариф")]
    //public async Task<ActionResult<CreateTariffResultDto>> CreateTariff(CreateTariffDto tariff)
    //{
    //    var result = await _tariffService.CreateTariff(tariff);
    //    if (!result.IsAdd) return BadRequest(result);

    //    return Ok(result);
    //}

    //[HttpPost("deleteTariff")]
    //[SwaggerOperation(Summary = "Удаление тарифа")]
    //[SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, удаленный тариф", typeof(DeleteTariffResultDto))]
    //[SwaggerResponse(StatusCodes.Status400BadRequest, "Не получилось удалить тариф")]
    //public async Task<ActionResult<DeleteTariffResultDto>> DeleteTariff(DeleteTariffDto tariff)
    //{
    //    var result = await _tariffService.DeleteTariff(tariff);
    //    if (!result.IsDeleted) return BadRequest(result);

    //    return Ok(result);
    //}

    [HttpGet("allTariff")]
    [SwaggerOperation(Summary = "Получение всех тарифов")]
    [SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, список тарифов", typeof(List<Tariff>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "тарифы отсутствуют")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var result = await _tariffService.GetAllTariffs();
        if (!result.Any()) return NotFound(new { Message = "Тарифы отстутствуют" });

        return Ok(result);
    }
}
