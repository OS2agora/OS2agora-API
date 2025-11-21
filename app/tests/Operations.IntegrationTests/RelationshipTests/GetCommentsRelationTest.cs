using System.Collections.Generic;
using Agora.Operations.Common.Enums;
using Agora.Operations.Models.Comments.Queries.GetComments;
using MediatR;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using CommentType = Agora.Models.Enums.CommentType;

namespace Agora.Operations.IntegrationTests.RelationshipTests
{
    using static TestSetup;
    public class GetCommentsRelationTest : TestBase
    {
        protected ISender Mediator;

        [Test]
        public async Task GetCommentsWithNoRedaction()
        {
            using var scope = ScopeFactory.CreateScope();
            Mediator = scope.ServiceProvider.GetService(typeof(ISender)) as ISender;

            await RunAsUserWithAuthenticationMethod(AuthenticationMethod.AdfsEmployee);

            var comments = await Mediator.Send(new GetCommentsQuery
            {
                HearingIds = new List<int> {4}
            });

            Assert.IsTrue(comments.All(c =>
                c.CommentType != null &&
                !c.CommentType.Comments.Any() &&

                c.CommentStatus != null &&
                !c.CommentStatus.Comments.Any() &&

                c.Hearing != null &&
                !c.Hearing.Comments.Any() &&

                c.Contents != null &&
                c.Contents.All(co =>
                    co.ContentType != null &&
                    !co.ContentType.Contents.Any() &&
                    co.Hearing != null &&
                    !co.Hearing.Contents.Any() &&
                    co.Comment != null &&
                    co.Comment.Contents.Count == c.Contents.Count) &&

                c.User != null &&
                !c.User.Comments.Any() &&

                c.User.UserHearingRoles != null &&
                c.User.UserHearingRoles.All(uhr =>
                    uhr.User == null) &&
                c.CommentType.Type == CommentType.HEARING_RESPONSE &&
                c.Hearing.ShowComments));
        }
    }
}