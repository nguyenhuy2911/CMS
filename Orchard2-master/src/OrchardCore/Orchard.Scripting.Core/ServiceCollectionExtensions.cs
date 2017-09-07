using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Scripting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScripting(this IServiceCollection services)
        {
            services.AddSingleton<IScriptingManager, DefaultScriptingManager>();
            services.AddSingleton<IGlobalMethodProvider, CommonGeneratorMethods>();
            return services;
        }
    }
}