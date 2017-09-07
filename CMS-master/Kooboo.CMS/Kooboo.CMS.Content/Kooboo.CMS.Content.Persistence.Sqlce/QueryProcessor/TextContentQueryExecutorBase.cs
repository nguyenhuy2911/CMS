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
using Kooboo.CMS.Content.Query;
using Kooboo.CMS.Content.Models;
using System.Data.SqlServerCe;

namespace Kooboo.CMS.Content.Persistence.Sqlce.QueryProcessor
{
    public abstract class TextContentQueryExecutorBase : ContentQueryExecutorBase<TextContent>
    {
        public TextContentQueryExecutorBase(ContentQuery<TextContent> contentQuery)
            : base(contentQuery)
        {           
        }
    }
}
