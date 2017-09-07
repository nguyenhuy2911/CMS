﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.DisplayManagement.Theming;
using Orchard.Environment.Extensions;
using Orchard.Tests.Stubs;
using Xunit;

namespace Orchard.Tests.DisplayManagement
{
    public class ShapeHelperTests
    {
        IServiceProvider _serviceProvider;

        public ShapeHelperTests()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddScoped<IHttpContextAccessor, StubHttpContextAccessor>();
            serviceCollection.AddScoped<IHtmlDisplay, DefaultHtmlDisplay>();
            serviceCollection.AddScoped<IExtensionManager, StubExtensionManager>();
            serviceCollection.AddScoped<IThemeManager, ThemeManager>();
            serviceCollection.AddScoped<IShapeFactory, DefaultShapeFactory>();
            serviceCollection.AddScoped<IShapeTableManager, TestShapeTableManager>();


            var defaultShapeTable = new TestShapeTable
            {
                Descriptors = new Dictionary<string, ShapeDescriptor>(StringComparer.OrdinalIgnoreCase),
                Bindings = new Dictionary<string, ShapeBinding>(StringComparer.OrdinalIgnoreCase)
            };
            serviceCollection.AddSingleton(defaultShapeTable);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public void CreatingNewShapeTypeByName()
        {
            dynamic shape = _serviceProvider.GetService<IShapeFactory>();

            var alpha = shape.Alpha();

            Assert.Equal("Alpha", alpha.Metadata.Type);
        }

        [Fact]
        public void CreatingShapeWithAdditionalNamedParameters()
        {
            dynamic shape = _serviceProvider.GetService<IShapeFactory>();

            var alpha = shape.Alpha(one: 1, two: "dos");

            Assert.Equal("Alpha", alpha.Metadata.Type);
            Assert.Equal(1, alpha.one);
            Assert.Equal("dos", alpha.two);
        }

        [Fact]
        public void WithPropertyBearingObjectInsteadOfNamedParameters()
        {
            dynamic shape = _serviceProvider.GetService<IShapeFactory>();

            var alpha = shape.Alpha(new { one = 1, two = "dos" });

            Assert.Equal("Alpha", alpha.Metadata.Type);
            Assert.Equal(1, alpha.one);
            Assert.Equal("dos", alpha.two);
        }
    }
}