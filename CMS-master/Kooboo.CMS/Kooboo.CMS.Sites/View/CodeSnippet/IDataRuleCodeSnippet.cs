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
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Sites.DataRule;

namespace Kooboo.CMS.Sites.View.CodeSnippet
{
    public interface IDataRuleCodeSnippet
    {
        string Generate(Repository repository, DataRuleSetting dataRule, bool inlineEdit);
    }
}
