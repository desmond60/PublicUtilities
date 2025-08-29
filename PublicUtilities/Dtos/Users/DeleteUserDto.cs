using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PublicUtilities.Dtos.Users;

public class DeleteUserDto
{
    [SwaggerSchema("Лицевой счёт пользователя")]
    [Required(ErrorMessage = "Поле 'PersonalAccount' обязательно для заполнения")]
    [StringLength(10, ErrorMessage = "Лицевой счёт не может быть длиннее 10 символов")]
    [DefaultValue("1234567890")]
    public string PersonalAccount { get; set; }
}
