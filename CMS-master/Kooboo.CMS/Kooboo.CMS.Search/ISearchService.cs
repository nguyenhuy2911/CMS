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
using Kooboo.Web.Mvc.Paging;

namespace Kooboo.CMS.Search
{
    public interface ISearchService
    {
        void Add<T>(T o);
        void Update<T>(T o);
        void Delete<T>(T o);
        void BatchAdd<T>(IEnumerable<T> list);
        void BatchUpdate<T>(IEnumerable<T> list);
        void BatchDelete<T>(IEnumerable<T> list);
        void BatchDelete(string folderName);

        PagedList<Models.ResultObject> Search(string key, int pageIndex, int pageSize, params string[] folders);
    }
}
