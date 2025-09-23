using System.Threading.Tasks;
using NUnit.Framework;

namespace BallerupKommune.Operations.IntegrationTests
{
    using static TestSetup;

    public class TestBase
    {
        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();
        }
    }
}