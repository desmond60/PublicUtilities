using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PublicUtilities.Dtos.UtilitiesCalculation;

public class ResidentPeriodDto
{
    [SwaggerSchema("Дата начала периода")]
    [DefaultValue("2025-01-01")]
    public DateTime StartDate { get; set; }

    [SwaggerSchema("Дата начала периода")]
    [DefaultValue("2025-01-31")]
    public DateTime EndDate { get; set; }

    [SwaggerSchema("Дата начала периода")]
    [DefaultValue("1")]
    public int ResidentCount { get; set; }
}

public class UtilitiesCalcDto
{
    [SwaggerSchema("Лицевой счёт пользователя")]
    [Required(ErrorMessage = "Поле 'PersonalAccount' обязательно для заполнения")]
    [StringLength(10, ErrorMessage = "Лицевой счёт не может быть длиннее 10 символов")]
    [DefaultValue("1234567890")]
    public string PersonalAccount { get; set; }

    [SwaggerSchema("Дата показания (Год-месяц)")]
    [Required(ErrorMessage = "Поле 'Date' обязательно для заполнения")]
    [DefaultValue("2025-01")]
    public DateTime Date { get; set; }

    [SwaggerSchema("Показания по ХВС", Description = "0 значит нет прибора")]
    [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть > 0 или = 0 если прибора нет")]
    [DefaultValue(0)]
    public double ColdWater { get; set; }

    [SwaggerSchema("Показания по ГВС", Description = "0 значит нет прибора")]
    [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть > 0 или = 0 если прибора нет")]
    [DefaultValue(0)]
    public double HotWater { get; set; }

    [SwaggerSchema("Показания по ЭЭ (дневная)", Description = "0 значит нет прибора")]
    [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть > 0 или = 0 если прибора нет")]
    [DefaultValue(0)]
    public double ElectricityDay { get; set; }

    [SwaggerSchema("Показания по ЭЭ (ночная)", Description = "0 значит нет прибора")]
    [Range(0, int.MaxValue, ErrorMessage = "Значение должно быть > 0 или = 0 если прибора нет")]
    [DefaultValue(0)]
    public double ElectricityNight { get; set; }

    [SwaggerSchema("Признак сохранения в БД")]
    [DefaultValue(false)]
    public bool SaveToDb { get; set; }
}
