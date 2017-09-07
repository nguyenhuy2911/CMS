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


namespace Kooboo.CMS.Form.Html.Controls
{
    public class Display : ControlBase
    {
        public override string Name
        {
            get { return "Display"; }
        }

        protected override string RenderInput(IColumn column)
        {
            return string.Format(@"<input  type=""hidden"" name=""{0}"" value=""@(Model.{0} ?? """")""/>
                                <label>@(Model.{0} ?? """")</label>", column.Name);
        }

    }
}
