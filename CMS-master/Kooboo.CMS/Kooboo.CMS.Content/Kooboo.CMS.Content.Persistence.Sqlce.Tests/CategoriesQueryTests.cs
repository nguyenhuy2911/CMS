﻿#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kooboo.CMS.Content.Models;
using Kooboo.CMS.Content.Query;
using Kooboo.Data;

namespace Kooboo.CMS.Content.Persistence.Sqlce.Tests
{
    /// <summary>
    /// Summary description for SimpleQueryTests
    /// </summary>
    [TestClass]
    public class CategoriesQueryTests
    {
        static Repository repository = null;
        static Schema newsSchema;
        static TextFolder newsFolder;
        static Schema categorySchema;
        static TextFolder categoryFolder;
        static TextContentProvider textContentProvider = new TextContentProvider();
        static CategoriesQueryTests()
        {
            repository = new Repository("CategoriesQueryTests");

            categorySchema = new Schema(repository, "category");
            categorySchema.AddColumn(new Column() { Name = "title", DataType = DataType.String, Length = 256 });

            categoryFolder = new TextFolder(repository, "category") { SchemaName = categorySchema.Name };

            newsSchema = new Schema(repository, "news");
            newsSchema.AddColumn(new Column()
            {
                Name = "Title",
                DataType = DataType.String,
                Length = 256
            });
            newsSchema.AddColumn(new Column()
            {
                Name = "Comments",
                DataType = DataType.Int
            });

            newsFolder = new TextFolder(repository, "news") { SchemaName = newsSchema.Name, CategoryFolders = new[] { categoryFolder.Name } };

            ProviderFactory providerFactory = new ProviderFactory();
            Providers.DefaultProviderFactory = providerFactory;
            providerFactory.GetProvider<IRepositoryProvider>().Add(repository);
            providerFactory.GetProvider<ISchemaProvider>().Add(categorySchema);
            providerFactory.GetProvider<ISchemaProvider>().Add(newsSchema);
            providerFactory.GetProvider<ITextFolderProvider>().Add(newsFolder);
            providerFactory.GetProvider<ITextFolderProvider>().Add(categoryFolder);
            InitializeData();
        }
        [TestMethod]
        public void TestQuery()
        {
            var query = newsFolder.CreateQuery().WhereEquals("title", "news1").Categories(categoryFolder);

            Assert.AreEqual(2, query.Count());

            var first = query.OrderBy("id").First();
            Assert.AreEqual("category1", first["title"]);

            var last = query.OrderBy("id").Last();
            Assert.AreEqual("category2", last["title"]);
        }


        private static void InitializeData()
        {
            dynamic category1 = new TextContent(repository.Name, categorySchema.Name, categoryFolder.Name)
          {
              UserKey = "category1"
          };
            category1.Title = "category1";

            textContentProvider.Add(category1);

            dynamic category2 = new TextContent(repository.Name, categorySchema.Name, categoryFolder.Name)
            {
                UserKey = "category2"
            };
            category2.Title = "category2";

            textContentProvider.Add(category2);

            dynamic news1 = new TextContent(repository.Name, newsSchema.Name, newsFolder.Name)
            {
                UserKey = "news1"
            };
            news1.title = "news1";
            textContentProvider.Add(news1);

            textContentProvider.AddCategories(news1, new Category() { ContentUUID = news1.uuid, CategoryFolder = categoryFolder.FullName, CategoryUUID = (string)(category1.UUID) },
                 new Category() { ContentUUID = news1.uuid, CategoryFolder = categoryFolder.FullName, CategoryUUID = (string)(category2.UUID) });
        }
    }
}
