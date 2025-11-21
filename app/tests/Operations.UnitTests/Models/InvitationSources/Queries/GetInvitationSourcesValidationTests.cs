using Agora.Models.Models;
using Agora.Operations.Models.InvitationSources.Queries.GetInvitationSources;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationSources.Queries
{
    public class GetInvitationSourcesValidationTests
    {
        [Test]
        public async Task GetInvitationSources_NoValidation_ShouldNotThrowException()
        {
            var query = new GetInvitationSourcesQuery();
            await ValidationTestFramework
                .For<GetInvitationSourcesQuery, List<InvitationSource>>()
                .WithoutValidators()
                .ShouldPassValidation(query);
        }
    }
}