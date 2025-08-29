using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PublicUtilities.Dtos.Tariffs;

public class DeleteTariffDto
{
    [SwaggerSchema("Услуга")]
    [Required(ErrorMessage = "Поле 'ServiceType' обязательно для заполнения")]
    [DefaultValue("ХВС")]
    public string ServiceType { get; set; }

    [SwaggerSchema("Компонент если присутствует")]
    public string Component { get; set; }
}
