using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApiGenerator;
using Radical.Windows;
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
            var publicApi = ApiGenerator.GeneratePublicApi(typeof(VisualTreeCrawler).Assembly, options: null);

            Approvals.Verify(publicApi);
        }
    }
}