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
using System.Web;
using Kooboo.CMS.Content.Models;

namespace Kooboo.CMS.Web.Areas.Contents.Models
{
    public class MediaContentGrid
    {
        public IEnumerable<Folder> ChildFolders { get; set; }
        public Kooboo.Web.Mvc.Paging.PagedList<Kooboo.CMS.Content.Models.MediaContent> Contents
        {
            get;
            set;
        }
    }
}