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
                AllowNamespacePrefixes = ["Microsoft", "System"],
                ExcludeAttributes = ["System.Runtime.Versioning.TargetFrameworkAttribute", "System.Reflection.AssemblyMetadataAttribute"]
            };

            var type = Type.GetType("XamlGeneratedNamespace.GeneratedInternalTypeHelper, Radical.Windows");
            if (type != null)
            {
                var typesToInclude = typeof(VisualTreeCrawler).Assembly
                    .GetExportedTypes()
                    .Except([type])
                    .ToArray();

                options.IncludeTypes = typesToInclude;
            }

            var publicApi = typeof(VisualTreeCrawler).Assembly.GeneratePublicApi(options: options);

            Approvals.Verify(publicApi);
        }
    }
}