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

namespace Kooboo.CMS.Content.Query.Expressions
{
    public class WhereNotInExpression : WhereFieldExpression
    {
        public WhereNotInExpression(string fieldName, object[] values)
            : this(null, fieldName, values)
        {

        }
        public WhereNotInExpression(IExpression expression, string fieldName, object[] values)
            : base(expression, fieldName)
        {
            this.Values = values;
        }
        public object[] Values { get; set; }
    }
}
