using System;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orchard.Data.Migration;
using Orchard.Environment.Shell;
using YesSql;
using YesSql.Indexes;
using YesSql.Provider.MySql;
using YesSql.Provider.PostgreSql;
using YesSql.Provider.Sqlite;
using YesSql.Provider.SqlServer;

namespace Orchard.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IDataMigrationManager, DataMigrationManager>();
            services.AddScoped<IModularTenantEvents, AutomaticDataMigrations>();

            // Adding supported databases
            services.TryAddDataProvider(name: "Sql Server", value: "SqlConnection", hasConnectionString: true);
            services.TryAddDataProvider(name: "Sqlite", value: "Sqlite", hasConnectionString: false);
			services.TryAddDataProvider(name: "MySql", value: "MySql", hasConnectionString: true);
			services.TryAddDataProvider(name: "Postgres", value: "Postgres", hasConnectionString: true);

			// Configuring data access

			services.AddSingleton<IStore>(sp =>
            {
                var shellSettings = sp.GetService<ShellSettings>();
                var hostingEnvironment = sp.GetService<IHostingEnvironment>();

                if (shellSettings.DatabaseProvider == null)
                {
                    return null;
                }
                
                var storeConfiguration = new Configuration();

                switch (shellSettings.DatabaseProvider)
                {
                    case "SqlConnection":
                        storeConfiguration.UseSqlServer(shellSettings.ConnectionString, IsolationLevel.ReadUncommitted);
                        break;
                    case "Sqlite":
                        var shellOptions = sp.GetService<IOptions<ShellOptions>>();
                        var option = shellOptions.Value;
                        var databaseFolder = Path.Combine(hostingEnvironment.ContentRootPath, option.ShellsRootContainerName, option.ShellsContainerName, shellSettings.Name);
                        var databaseFile = Path.Combine(databaseFolder, "yessql.db");
                        Directory.CreateDirectory(databaseFolder);
                        storeConfiguration.UseSqLite($"Data Source={databaseFile};Cache=Shared", IsolationLevel.ReadUncommitted);
                        break;
					case "MySql":
                        storeConfiguration.UseMySql(shellSettings.ConnectionString, IsolationLevel.ReadUncommitted);
						break;
					case "Postgres":
                        storeConfiguration.UsePostgreSql(shellSettings.ConnectionString, IsolationLevel.ReadUncommitted);
                        break;
					default:
                        throw new ArgumentException("Unknown database provider: " + shellSettings.DatabaseProvider);
                }

                if (!string.IsNullOrWhiteSpace(shellSettings.TablePrefix))
                {
                    storeConfiguration.TablePrefix = shellSettings.TablePrefix + "_";
                }

                var store = new Store(storeConfiguration);
                var indexes = sp.GetServices<IIndexProvider>();
                store.RegisterIndexes(indexes.ToArray());
                return store;
            });

            services.AddScoped(sp =>
            {
                var store = sp.GetService<IStore>();

                if (store == null)
                {
                    return null;
                }

                var session = store.CreateSession();

                return session;
            });

            return services;
        }
    }
}