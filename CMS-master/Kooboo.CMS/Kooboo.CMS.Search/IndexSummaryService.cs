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
using Kooboo.CMS.Content.Models;
using System.Threading;
using Kooboo.CMS.Content.Query;
using Kooboo.CMS.Search.Models;
using Kooboo.CMS.Search.Persistence;

namespace Kooboo.CMS.Search
{
    public class IndexSummaryService
    {
        ISearchSettingProvider SearchSettingProvider { get; set; }
        ILastActionProvider LastActionProvider { get; set; }
        public IndexSummaryService(ISearchSettingProvider searchSettingProvider, ILastActionProvider lastActionProvider)
        {
            SearchSettingProvider = searchSettingProvider;
            LastActionProvider = lastActionProvider;
        }
        public static IDictionary<string, Thread> indexThreads = new Dictionary<string, Thread>(StringComparer.OrdinalIgnoreCase);

        public virtual IEnumerable<FolderIndexInfo> GetFolderIndexInfoes(Repository repository)
        {
            List<FolderIndexInfo> list = new List<FolderIndexInfo>();
            var searchSettings = SearchSettingProvider.All(repository).ToArray();
            foreach (var folder in Kooboo.CMS.Content.Services.ServiceFactory.TextFolderManager.All(repository, ""))
            {
                GetFolderIndexInfoes(folder, ref list, searchSettings);
            }
            return list;
        }
        private void GetFolderIndexInfoes(TextFolder textFolder, ref List<FolderIndexInfo> list, SearchSetting[] searchSettings)
        {
            if (searchSettings.Where(it => it.FolderName.EqualsOrNullEmpty(textFolder.FullName, StringComparison.OrdinalIgnoreCase)).Count() > 0)
            {
                FolderIndexInfo folderIndexInfo = new FolderIndexInfo();

                folderIndexInfo.FolderName = textFolder.FullName;
                folderIndexInfo.IndexedContents = SearchHelper.Search(textFolder.Repository, "", 1, 1, textFolder.FullName).TotalItemCount;
                folderIndexInfo.Rebuilding = IsRebuilding(textFolder);
                list.Add(folderIndexInfo);
            }

            foreach (var folder in Kooboo.CMS.Content.Services.ServiceFactory.TextFolderManager.ChildFolders(textFolder))
            {
                GetFolderIndexInfoes(folder, ref list, searchSettings);
            }
        }

        public virtual IEnumerable<LastAction> GetLastActions(Repository repository)
        {
            return LastActionProvider.All(repository);
        }

        public virtual void Rebuild(TextFolder textFolder)
        {
            var isBuilding = IsRebuilding(textFolder);
            if (!isBuilding)
            {
                var key = GetIndexThreadKey(textFolder);
                Thread thread = new Thread(delegate()
                {
                    var searchService = SearchHelper.OpenService(textFolder.Repository);
                    searchService.BatchDelete(textFolder.FullName);
                    var folderQuery = textFolder.CreateQuery().WhereEquals("Published", true);
                    searchService.BatchAdd(folderQuery);
                    indexThreads.Remove(key);
                });
                indexThreads.Add(key, thread);
                thread.Start();
            }
        }
        private string GetIndexThreadKey(TextFolder textFolder)
        {
            return string.Format("Repository:{0};FullName:{1}", textFolder.Repository.Name, textFolder.FullName);
        }
        protected virtual bool IsRebuilding(TextFolder textFolder)
        {
            var key = GetIndexThreadKey(textFolder);
            return indexThreads.ContainsKey(key);
        }
    }
}
