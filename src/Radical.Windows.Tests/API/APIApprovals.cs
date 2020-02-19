using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiGenerator;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Radical.Windows.Tests.API
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
            ApiGeneratorOptions options = new ApiGeneratorOptions()
            {
                WhitelistedNamespacePrefixes = new[] { "Microsoft", "System" }
            };
            var type = Type.GetType("XamlGeneratedNamespace.GeneratedInternalTypeHelper, Radical.Windows");
            if (type != null)
            {
                var typesToInclude = typeof(VisualTreeCrawler).Assembly
                    .GetExportedTypes()
                    .Except(new Type[] { type })
                    .ToArray();

                options.IncludeTypes = typesToInclude;
            }

            var publicApi = ApiGenerator.GeneratePublicApi(typeof(VisualTreeCrawler).Assembly, options: options);

            Approvals.Verify(publicApi);
        }
    }
}