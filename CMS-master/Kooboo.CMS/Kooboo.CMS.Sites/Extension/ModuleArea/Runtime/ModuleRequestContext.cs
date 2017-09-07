﻿#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.CMS.Sites.Extension.ModuleArea.Runtime
{
    public class ModuleRequestContext : RequestContext
    {
        public ModuleRequestContext(HttpContextBase httpContext, RouteData routeData, ModuleContext moduleContext)
            : base(httpContext, routeData)
        {
            this.ModuleContext = moduleContext;

        }
        public ModuleContext ModuleContext { get; private set; }
        public ControllerContext PageControllerContext { get; set; }
    }
}
