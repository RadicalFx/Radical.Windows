using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiGenerator;
using Radical.Windows;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Radical.Tests.API
{
    [TestClass]
    public class APIApprovals
    {
        [TestMethod]
        [TestCategory("APIApprovals")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        [UseReporter(typeof(DiffReporter))]
        public void Approve_API()
        {
            ApiGeneratorOptions options = null;
            var type = Type.GetType("XamlGeneratedNamespace.GeneratedInternalTypeHelper");
            if (type != null) 
            {
                var typesToInclude = typeof(VisualTreeCrawler).Assembly.GetExportedTypes().Except(new Type[] { type }).ToArray();
                options = new ApiGeneratorOptions()
                {
                    IncludeTypes = typesToInclude
                };
            }

            var publicApi = ApiGenerator.GeneratePublicApi(typeof(VisualTreeCrawler).Assembly, options: options);

            Approvals.Verify(publicApi);
        }
    }
}