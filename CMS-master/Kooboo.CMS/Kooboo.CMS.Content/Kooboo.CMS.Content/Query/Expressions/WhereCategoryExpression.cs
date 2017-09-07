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
using Kooboo.CMS.Content.Models;

namespace Kooboo.CMS.Content.Query.Expressions
{
    public class WhereCategoryExpression : Expression, IWhereExpression
    {
        public WhereCategoryExpression(IContentQuery<TextContent> categoryQuery)
            : this(null, categoryQuery)
        {

        }
        public WhereCategoryExpression(IExpression expression, IContentQuery<TextContent> categoryQuery)
            : base(expression)
        {
            this.CategoryQuery = categoryQuery;
        }
        public IContentQuery<TextContent> CategoryQuery { get; private set; }
    }
}
