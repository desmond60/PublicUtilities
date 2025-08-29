using Microsoft.AspNetCore.Mvc;
using PublicUtilities.Dtos.Users;
using PublicUtilities.Models;
using PublicUtilities.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PublicUtilities.Controllers;

public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("create")]
    [SwaggerOperation(Summary = "Создание лицевого счета пользователя")]
    [SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, созданный лицевой счет", typeof(CreateUserResultDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Не получилось создать лицевой счет")]
    public async Task<ActionResult<CreateUserResultDto>> CreateUser(CreateUserDto user)
    {
        var result = await _userService.CreateUser(user);
        if (!result.IsAdd) return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("delete")]
    [SwaggerOperation(Summary = "Удаление лицевого счета пользователя")]
    [SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, удаленный лицевой счет", typeof(DeleteUserResultDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Не получилось удалить лицевой счет")]
    public async Task<ActionResult<DeleteUserResultDto>> DeleteUser(DeleteUserDto user)
    {
        var result = await _userService.DeleteUser(user);
        if (!result.IsDeleted) return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("allUsers")]
    [SwaggerOperation(Summary = "Получение всех лицевых счетов пользователей")]
    [SwaggerResponse(StatusCodes.Status200OK, "Успешный ответ, список пользователей", typeof(List<User>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Пользователи отсутствуют")]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var result = await _userService.GetAllUsers();
        if (!result.Any()) return NotFound(new { Message = "Пользователи отстутствуют" });

        return Ok(result);
    }
}
