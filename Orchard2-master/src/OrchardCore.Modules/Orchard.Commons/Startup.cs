using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Orchard.BackgroundTasks;
using Orchard.Data;
using Orchard.DeferredTasks;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Liquid;
using Orchard.DisplayManagement.TagHelpers;
using Orchard.Environment.Cache;
using Orchard.Environment.Extensions;
using Orchard.Environment.Shell.Data;
using Orchard.Mvc;
using Orchard.ResourceManagement;
using Orchard.ResourceManagement.TagHelpers;

namespace Orchard.Commons
{
    /// <summary>
    /// These services are registered on the tenant service collection
    /// </summary>
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDeferredTasks();
            services.AddDataAccess();
            services.AddBackgroundTasks();
            services.AddResourceManagement();
            services.AddGeneratorTagFilter();
            services.AddCaching();
            services.AddShellDescriptorStorage();
            services.AddExtensionManager();
            services.AddTheming();
            services.AddLiquidViews();
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            serviceProvider.AddTagHelpers(typeof(ResourcesTagHelper).GetTypeInfo().Assembly);
            serviceProvider.AddTagHelpers(typeof(ShapeTagHelper).GetTypeInfo().Assembly);
        }
    }

    /// <summary>
    /// Deferred tasks middleware is registered early as it has to run very late.
    /// </summary>
    public class DeferredTasksStartup : StartupBase
    {
        public override int Order => -50;

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            app.AddDeferredTasks();
        }
    }
}
