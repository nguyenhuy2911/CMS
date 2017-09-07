﻿using Microsoft.Extensions.Localization;
using Orchard.Environment.Navigation;
using System;

namespace Orchard.Deployment.Remote
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Content"], content => content
                    .Add(T["Import/Export"], import => import
                        .Add(T["Remote Instances"], "5.1", remote => remote
                            .Action("Index", "RemoteInstance", new { area = "Orchard.Deployment.Remote" })
                            .Permission(Permissions.ManageRemoteInstances)
                            .LocalNav()
                        )
                        .Add(T["Remote Clients"], remote => remote
                            .Action("Index", "RemoteClient", new { area = "Orchard.Deployment.Remote" })
                            .Permission(Permissions.ManageRemoteClients)
                            .LocalNav()
                        )
                    )
                );
        }
    }
}
