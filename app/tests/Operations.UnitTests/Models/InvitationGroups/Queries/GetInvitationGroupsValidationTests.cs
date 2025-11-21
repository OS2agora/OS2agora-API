using Agora.Models.Models;
using Agora.Operations.Models.InvitationGroups.Queries.GetInvitationGroups;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Queries
{
    public class GetInvitationGroupsValidationTests
    {
        [Test]
        public async Task GetInvitationGroups_NoValidation_ShouldNotThrowException()
        {
            var query = new GetInvitationGroupsQuery();
            await ValidationTestFramework
                .For<GetInvitationGroupsQuery, List<InvitationGroup>>()
                .WithoutValidators()
                .ShouldPassValidation(query);
        }
    }
}