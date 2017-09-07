﻿#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.Web.Mvc.Grid2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kooboo.CMS.Web.Areas.Sites.Models.Grid2
{
    public class ABPageTestHitsReport_HitTimes_GridItemColumn : GridItemColumn
    {
        public ABPageTestHitsReport_HitTimes_GridItemColumn(IGridColumn gridColumn, object dataItem, object propertyValue)
            : base(gridColumn, dataItem, propertyValue)
        {

        }
        public override System.Web.IHtmlString RenderItemColumn(System.Web.Mvc.ViewContext viewContext)
        {
            ABPageTestHitsReport report = (ABPageTestHitsReport)this.DataItem;

            return new HtmlString(string.Format("{0} ( {1}% )", report.HitTimes, (report.HitRate * 100).ToString(".00")));
        }
    }
}
