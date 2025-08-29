using PublicUtilities.Services;

namespace PublicUtilities.Extention;

public static class AddAppService
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUtilitiesCalcService, UtilitiesCalcService>();
        services.AddScoped<ITariffService, TariffService>();

        return services;
    }
}
