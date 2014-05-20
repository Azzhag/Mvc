// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using BasicWebSite;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class HomeScenarioTests
    {
        private readonly IServiceProvider _provider;
        private readonly Action<IBuilder> _app = new Startup().Configure;

        // Some tests require comparing the actual response body against an expected response baseline
        // so they require a reference to the assembly on which the resources are located, in order to
        // make the tests less verbose, we get a reference to the assembly with the resources and we
        // use it on all the resto f the tests.
        private readonly Assembly _resourcesAssembly = typeof(HomeScenarioTests).GetTypeInfo().Assembly;

        public HomeScenarioTests()
        {
            var originalProvider = CallContextServiceLocator.Locator.ServiceProvider;
            IApplicationEnvironment appEnvironment = originalProvider.GetService<IApplicationEnvironment>();
            // When an application executes in a regular context, the application base path points to the root
            // directory where the application is located, for example MvcSample.Web. However, when executing
            // an aplication as part of a test, the ApplicationBasePath of the IApplicationEnvironment points
            // to the root folder of the test project.
            // To compensate for this, we need to calculate the original path and override the application
            // environment value so that components like the view engine work properly in the context of the
            // test.
            string appBasePath = CalculateApplicationBasePath(appEnvironment);
            _provider = new ServiceCollection()
                .AddInstance(typeof(IApplicationEnvironment), new MockApplicationEnvironment(appEnvironment, appBasePath))
                .BuildServiceProvider(originalProvider);
        }

        [Fact]
        public async Task CanRenderMainView()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.Handler;

            // Act
            var result = await client.GetAsync("http://localhost:12345/");

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(result.ContentType, "text/html; charset=utf-8");
            var responseContent = await result.ReadBodyAsStringAsync();
            var expectedContent = await _resourcesAssembly.ReadResourceAsStringAsync("BasicWebSite.Home.Index.html");
            Assert.Equal(expectedContent, responseContent);
        }

        // Represents an application environment that overrides the base path of the original
        // application environment in order to make it point to the folder of the original web
        // aplication so that components like ViewEngines can find views as if they were executing
        // in a regular context.
        private class MockApplicationEnvironment : IApplicationEnvironment
        {
            private readonly IApplicationEnvironment _originalAppEnvironment;
            private readonly string _applicationBasePath;

            public MockApplicationEnvironment(IApplicationEnvironment originalAppEnvironment, string appBasePath)
            {
                _originalAppEnvironment = originalAppEnvironment;
                _applicationBasePath = appBasePath;
            }

            public string ApplicationName
            {
                get { return _originalAppEnvironment.ApplicationName; }
            }

            public string Version
            {
                get { return _originalAppEnvironment.Version; }
            }

            public string ApplicationBasePath
            {
                get { return _applicationBasePath; }
            }

            public FrameworkName TargetFramework
            {
                get { return _originalAppEnvironment.TargetFramework; }
            }
        }

        // Calculate the path relative to the current application base path. 
        private static string CalculateApplicationBasePath(IApplicationEnvironment appEnvironment)
        {
            // Mvc/test/MvcSample.Test
            var appBase = appEnvironment.ApplicationBasePath;

            // Mvc/test
            var test = Path.GetDirectoryName(appBase);

            // Mvc/Samples/MvcSample.Web
            return Path.GetFullPath(Path.Combine(appBase, "..", "Websites", "BasicWebSite"));
        }
    }
}