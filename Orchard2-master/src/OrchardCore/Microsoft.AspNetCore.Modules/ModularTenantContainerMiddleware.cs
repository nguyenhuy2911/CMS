using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Orchard.Environment.Shell;

namespace Microsoft.AspNetCore.Modules
{
    /// <summary>
    /// This middleware replaces the default service provider by the one for the current tenant
    /// </summary>
    public class ModularTenantContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IShellHost _orchardHost;
        private readonly IRunningShellTable _runningShellTable;

        public ModularTenantContainerMiddleware(
            RequestDelegate next,
            IShellHost orchardHost,
            IRunningShellTable runningShellTable)
        {
            _next = next;
            _orchardHost = orchardHost;
            _runningShellTable = runningShellTable;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // Ensure all ShellContext are loaded and available.
            _orchardHost.Initialize();

            var shellSetting = _runningShellTable.Match(httpContext);

            // Register the shell settings as a custom feature.
            httpContext.Features.Set(shellSetting);

            // We only serve the next request if the tenant has been resolved.
            if (shellSetting != null)
            {
                var shellContext = _orchardHost.GetOrCreateShellContext(shellSetting);

                var existingRequestServices = httpContext.RequestServices;
                try
                {
                    using (var scope = shellContext.EnterServiceScope())
                    {
                        if (!shellContext.IsActivated)
                        {
                            lock (shellContext)
                            {
                                // The tenant gets activated here
                                if (!shellContext.IsActivated)
                                {
                                    var tenantEvents = scope.ServiceProvider.GetServices<IModularTenantEvents>();

                                    foreach (var tenantEvent in tenantEvents)
                                    {
                                        tenantEvent.ActivatingAsync().Wait();
                                    }

                                    httpContext.Items["BuildPipeline"] = true;
                                    shellContext.IsActivated = true;

                                    foreach (var tenantEvent in tenantEvents)
                                    {
                                        tenantEvent.ActivatedAsync().Wait();
                                    }
                                }
                            }
                        }

                        shellContext.RequestStarted();
                        await _next.Invoke(httpContext);
                    }
                }
                finally
                {
                    shellContext.RequestEnded();
                }
            }
        }
    }
}