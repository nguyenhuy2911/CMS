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
using System.Web;
using System.Web.Mvc;

namespace Kooboo.CMS.Web.Controllers
{
    public class NotFoundController : Controller
    {
        //
        // GET: /NotFound/

        public virtual ActionResult Index()
        {
            return View();
        }
    }
}
