﻿#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Services;
using Kooboo.Web.Mvc;
using Kooboo.Web.Mvc.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Kooboo.CMS.Web.Areas.Contents.Menu
{
    public abstract class FolderMenuItems<T> : IMenuItemContainer
        where T : Folder
    {
        protected abstract FolderManager<T> FolderManager { get; }
        protected virtual IEnumerable<MenuItem> GetContentFolderItems(Repository repository)
        {
            if (repository == null)
            {
                return new MenuItem[0];
            }
            var folders = FolderManager.All(repository, "").OrderBy(it => it.FriendlyText).ToArray();
            List<MenuItem> items = new List<MenuItem>();
            foreach (var folder in folders)
            {
                var menuItem = CreateFolderMenuItem(folder);
                if (menuItem != null)
                {
                    items.Add(menuItem);
                }
            }

            return items;
        }
        protected virtual MenuItem CreateFolderMenuItem(T folder)
        {
            folder = folder.AsActual();
            if (folder != null)
            {
                MenuItem menuItem = new FolderMenuItem(folder.AsActual());
                var childFolders = FolderManager.ChildFolders(folder).OrderBy(it => it.FriendlyText).ToArray();
                List<MenuItem> items = new List<MenuItem>();
                menuItem.Items = items;
                foreach (var child in childFolders)
                {
                    items.Add(CreateFolderMenuItem(child));
                }
                return menuItem;
            }
            return null;
        }


        #region IMenuItemContainer Members

        public virtual IEnumerable<MenuItem> GetItems(string areaName, ControllerContext controllerContext)
        {
            var repository = Repository.Current;

            return GetContentFolderItems(repository);
        }

        #endregion
    }

    public class ContentMenuItems : FolderMenuItems<TextFolder>
    {
        protected override FolderManager<TextFolder> FolderManager
        {
            get { return ServiceFactory.TextFolderManager; }
        }
    }
}