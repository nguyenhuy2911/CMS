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
using System.ComponentModel.DataAnnotations;
using Kooboo.Web.Mvc;
using Kooboo.CMS.Sites.Models;

using Kooboo.CMS.Sites.Services;
using Kooboo.CMS.Sites.Extension;
using System.Web.Mvc;
using System.Web.Routing;
using Kooboo.Extensions;
using Kooboo.ComponentModel;
using Kooboo.Web.Mvc.Grid2.Design;
using Kooboo.CMS.Web.Grid2;
namespace Kooboo.CMS.Web.Areas.Sites.Models
{
    [MetadataFor(typeof(AssemblyFile))]	
	[Grid(Checkable = true, IdProperty = "FileName")]
	public class AssemblyFile_Metadata
	{
		[GridColumn(Order = 1)]
		public string FileName { get; set; }
	}
	public class UploadAssemblyViewModel
	{
        [Required(ErrorMessage = "Required")]
		[UIHint("file")]
		[RegularExpression(".+\\.dll")]
		public string File { get; set; }
	}

}