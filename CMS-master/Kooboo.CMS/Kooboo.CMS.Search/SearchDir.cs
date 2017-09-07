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
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Models.Paths;

namespace Kooboo.CMS.Search
{
    public static class SearchDir
    {
        public static string GetBasePhysicalPath(Repository repository)
        {
            var repositoryPath = new RepositoryPath(repository);
            return Path.Combine(repositoryPath.PhysicalPath, SearchDir.DirName);
        }
        public static string DirName = "Search";
    }
}
