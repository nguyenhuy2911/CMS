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
using System.IO;
[assembly: System.Web.PreApplicationStartMethod(typeof(Kooboo.CMS.Search.AssemblyInitializer), "Initialize")]
namespace Kooboo.CMS.Search
{
    public static class AssemblyInitializer
    {
        public static void Initialize()
        {
            Kooboo.CMS.Content.EventBus.EventBus.Subscribers.Add(new ContentEventsSubscriber());
        }
    }
}
