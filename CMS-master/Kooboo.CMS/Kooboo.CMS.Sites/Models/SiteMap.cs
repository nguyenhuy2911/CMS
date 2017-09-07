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

namespace Kooboo.CMS.Sites.Models
{
    public class SiteMapNode
    {
        public Page Page { get; set; }
        public IEnumerable<SiteMapNode> Children { get; set; }
    }
    public class SiteMap
    {
        public SiteMapNode Root { get; set; }
    }
}
