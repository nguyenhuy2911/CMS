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
using System.Web.Mvc;

namespace Kooboo.CMS.Sites.View
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class FrontViewControl : ViewUserControl, IFrontPageView
    {
        #region IFrontPageView Members

        public Page_Context PageViewContext
        {
            get { return Page_Context.Current; }
        }

        #endregion
    }
}
