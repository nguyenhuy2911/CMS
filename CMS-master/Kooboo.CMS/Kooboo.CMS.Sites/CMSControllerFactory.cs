﻿#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
namespace Kooboo.CMS.Sites
{
    [Obsolete("To replace the controller, please register the controller dependency into IoC container via implementing IDependencyRegistrar.")]
    public sealed class ControllerTypeCache
    {
        private static Dictionary<string, Type> _cache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public static void RegisterController(string controllerTypeName, Type type)
        {
            _cache[controllerTypeName] = type;
        }

        public static Type GetControllerType(string controllerName, IEnumerable<string> namespaces)
        {
            foreach (var item in namespaces)
            {
                var controllerTypeName = item + "." + controllerName + "Controller";
                if (_cache.ContainsKey(controllerTypeName))
                {
                    return _cache[controllerTypeName];
                }
            }
            return null;
        }

    }
    [Obsolete("For compatible with older Kooboo CMS.")]
    public class CMSControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            Type controllerType = null;
            object obj2;
            if ((requestContext != null) && requestContext.RouteData.DataTokens.TryGetValue("Namespaces", out obj2))
            {
                IEnumerable<string> namespaces = obj2 as IEnumerable<string>;
                if ((namespaces != null) && namespaces.Any<string>())
                {
                    controllerType = ControllerTypeCache.GetControllerType(controllerName, namespaces);
                }
            }
            if (controllerType != null)
            {
                return controllerType;
            }

            return base.GetControllerType(requestContext, controllerName);
        }
    }
}
