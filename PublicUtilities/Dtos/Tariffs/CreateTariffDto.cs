using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PublicUtilities.Dtos.Tariffs;

public class CreateTariffDto
{
    [SwaggerSchema("Услуга")]
    [Required(ErrorMessage = "Поле 'ServiceType' обязательно для заполнения")]
    [DefaultValue("ХВС")]
    public string ServiceType { get; set; }

    [SwaggerSchema("Часть")]
    public string? Component { get; set; }

    [SwaggerSchema("Тариф, руб/eд.изм")]
    [Required(ErrorMessage = "Поле 'Price' обязательно для заполнения")]
    [Range(1, double.MaxValue, ErrorMessage = "Цена тарифа должна быть больше 0")]
    [DefaultValue(10.0)]
    public double Price { get; set; }

    [SwaggerSchema("Норматив")]
    [Range(0, double.MaxValue, ErrorMessage = "Норматив должен быть больше 0")]
    [DefaultValue(4.0)]
    public double? Normative { get; set; }

    [SwaggerSchema("Единица измерения")]
    [Required(ErrorMessage = "Поле 'Unit' обязательно для заполнения")]
    [DefaultValue("метр кубический")]
    public string Unit { get; set; }
}
