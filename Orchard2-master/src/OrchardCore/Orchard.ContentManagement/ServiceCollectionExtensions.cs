using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orchard.ContentManagement.Cache;
using Orchard.ContentManagement.Drivers.Coordinators;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Data.Migration;
using Orchard.Environment.Cache;
using YesSql.Indexes;

namespace Orchard.ContentManagement
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContentManagement(this IServiceCollection services)
        {
            services.AddScoped<ICacheContextProvider, ContentDefinitionCacheContextProvider>();
            services.TryAddScoped<IContentDefinitionManager, ContentDefinitionManager>();
            services.TryAddScoped<IContentManager, DefaultContentManager>();
            services.TryAddScoped<IContentManagerSession, DefaultContentManagerSession>();
            services.AddSingleton<IIndexProvider, ContentItemIndexProvider>();
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<IContentHandler, UpdateContentsHandler>();
            services.AddScoped<IContentHandler, ContentPartHandlerCoordinator>();
            services.AddSingleton<ITypeActivatorFactory<ContentPart>, ContentPartFactory>();
            services.AddSingleton<ITypeActivatorFactory<ContentField>, ContentFieldFactory>();

            services.AddSingleton<IIdGenerator, IdGenerator>();
            services.AddSingleton<IContentItemIdGenerator, DefaultContentItemIdGenerator>();
            services.AddScoped<IContentAliasManager, ContentAliasManager>();

            return services;
        }
    }
}
