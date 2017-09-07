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
using System.Web;

namespace Kooboo.CMS.Form.Html.Controls
{
    public class Password : Input
    {
        public override string Type
        {
            get { return "Password"; }
        }

        public override string Name
        {
            get { return "Password"; }
        }
    }
}
