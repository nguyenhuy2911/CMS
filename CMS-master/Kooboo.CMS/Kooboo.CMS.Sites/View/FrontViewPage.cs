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
using Kooboo.CMS.Sites.Web;

namespace Kooboo.CMS.Sites.View
{
    [Obsolete]
    public class FrontViewPage : System.Web.Mvc.ViewPage, IFrontPageView
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
        }

        #region IFrontPageView Members

        public Page_Context PageViewContext
        {
            get { return Page_Context.Current; }
        }

        #endregion
    }
}
