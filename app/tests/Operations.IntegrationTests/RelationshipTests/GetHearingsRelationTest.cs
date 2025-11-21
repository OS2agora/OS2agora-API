using System.Linq;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Enums;
using Agora.Operations.Models.Hearings.Queries.GetHearings;
using Agora.TestUtilities.Extensions;
using MediatR;
using NUnit.Framework;

namespace Agora.Operations.IntegrationTests.RelationshipTests
{
    using static TestSetup;
    public class GetHearingsRelationTest : TestBase
    {
        // Testing that the result of the GetHearingsQuery are pruned correctly according to the allowed includes to ensure that no data is leaking.
        protected ISender Mediator;

        [Test]
        public async Task GetHearingWithNoRedaction()
        {
            using var scope = ScopeFactory.CreateScope();
            Mediator = scope.ServiceProvider.GetService(typeof(ISender)) as ISender;

            // Using administrator role to have no redactions
            await RunAsUserWithRole("Administrator");
            var hearings = await Mediator.Send(new GetHearingsQuery());

            // Checking UserHearingRoles Relations
            Assert.IsTrue(hearings.All(h =>
                                            h.UserHearingRoles != null &&
                                            h.UserHearingRoles.All(uhr =>
                                                  uhr.User != null &&
                                                  uhr.Hearing != null &&
                                                  uhr.HearingRole != null &&
                                                  !uhr.HearingRole.UserHearingRoles.Any() &&
                                                  !uhr.User.UserHearingRoles.Any() &&
                                                  uhr.Hearing.UserHearingRoles.Count() == h.UserHearingRoles.Count()
                                                  )));

            // Checking SubjectArea Relations
            Assert.IsTrue(hearings.All(h =>
                                           h.SubjectArea != null &&
                                           !h.SubjectArea.Hearings.Any()));

            // Checking HearingStatus Relations
            Assert.IsTrue(hearings.All(h =>
                                            h.HearingStatus != null &&
                                            !h.HearingStatus.Hearings.Any()));

            // Checking HearingType Relations
            Assert.IsTrue(hearings.All(h =>
                                            h.HearingType != null &&
                                            !h.HearingType.Hearings.Any() &&

                                            h.HearingType.HearingTemplate != null &&
                                            !h.HearingType.HearingTemplate.HearingTypes.Any() &&
                                            h.HearingType.HearingTemplate.Fields != null &&

                                            h.HearingType.HearingTemplate.Fields.All(f =>
                                                f.FieldTemplates != null &&

                                                f.FieldTemplates.All(ft =>
                                                    ft.Field != null &&
                                                    ft.Field.FieldTemplates.Count() == f.FieldTemplates.Count()) &&

                                                f.FieldType != null &&
                                                !f.FieldType.Fields.Any() &&
                                                f.FieldType.FieldTypeSpecifications != null &&

                                                f.FieldType.FieldTypeSpecifications.All(fts =>
                                                    fts.FieldType != null) &&
                                                !f.Contents.Any() &&
                                                f.HearingTemplate != null
                                                )));

            // Checking Content Relations
            Assert.IsTrue(hearings.All(h => h.Contents != null &&
                                            h.Contents.All(c =>
                                                c.Hearing != null &&
                                                c.Hearing.Contents.Count() == h.Contents.Count() &&
                                                c.Field != null &&
                                                !c.Field.Contents.Any() &&
                                                c.ContentType != null &&
                                                !c.ContentType.Contents.Any())));
        }

        [Test]
        [TestCase(AuthenticationMethod.AdfsEmployee)]
        public async Task GetHearingWithEmployeeRedactions(AuthenticationMethod authenticationMethod)
        {
            using var scope = ScopeFactory.CreateScope();
            Mediator = scope.ServiceProvider.GetService(typeof(ISender)) as ISender;

            // Using employee role to have employee redactions
            await RunAsUserWithAuthenticationMethod(authenticationMethod);
            var hearings = await Mediator.Send(new GetHearingsQuery());

            // Checks that all received hearings abide the postfilters
            Assert.IsTrue(hearings.All(CanEmployeeSeeHearing));

            // Checking that UserHearingRoles are now redacted
            Assert.IsTrue(hearings.All(h =>
                                            h.UserHearingRoles == null));

            // Checking SubjectArea relations
            Assert.IsTrue(hearings.All(h =>
                                           h.SubjectArea != null &&
                                           !h.SubjectArea.Hearings.Any()));

            // Checking HearingStatus relations
            Assert.IsTrue(hearings.All(h =>
                                            h.HearingStatus != null &&
                                            !h.HearingStatus.Hearings.Any()));

            // checking HearingType relations
            Assert.IsTrue(hearings.All(h =>
                                            h.HearingType != null &&
                                             !h.HearingType.Hearings.Any() &&
                                            h.HearingType.HearingTemplate != null &&
                                            !h.HearingType.HearingTemplate.HearingTypes.Any() &&
                                            h.HearingType.HearingTemplate.Fields != null &&

                                            h.HearingType.HearingTemplate.Fields.All(f =>
                                                f.FieldTemplates == null &&
                                                f.FieldType != null &&
                                                !f.FieldType.Fields.Any() &&
                                                f.FieldType.FieldTypeSpecifications != null &&

                                                f.FieldType.FieldTypeSpecifications.All(fts =>
                                                    fts.FieldType != null) &&
                                                !f.Contents.Any() &&
                                                f.HearingTemplate != null
                                                  )));

            // Checking Content relations
            Assert.IsTrue(hearings.All(h => h.Contents != null &&
                                            h.Contents.All(c =>
                                                c.Hearing != null &&
                                                CanEmployeeSeeHearing(c.Hearing) &&
                                                c.Hearing.Contents.Count() == h.Contents.Count() &&
                                                c.Field != null &&
                                                !c.Field.Contents.Any() &&
                                                c.ContentType != null &&
                                                !c.ContentType.Contents.Any())));
        }

        private static bool CanEmployeeSeeHearing(Hearing hearing)
        {
            return (!hearing.ClosedHearing && hearing.IsInternalHearing() && hearing.IsHearingPublished());
        }

        [Test]
        [TestCase(AuthenticationMethod.MitIdCitizen)]
        [TestCase(AuthenticationMethod.MitIdErhverv)]
        [TestCase(null)]
        public async Task GetHearingWithCitizenAndAnonymousRedactions(AuthenticationMethod authenticationMethod)
        {
            using var scope = ScopeFactory.CreateScope();
            Mediator = scope.ServiceProvider.GetService(typeof(ISender)) as ISender;

            // Using Citizen and Anonymous roles to test redactions
            await RunAsUserWithAuthenticationMethod(authenticationMethod);
            var hearings = await Mediator.Send(new GetHearingsQuery());

            // Checks that all received hearings abide the postfilters
            Assert.IsTrue(hearings.All(CanCitizenAndAnonymousSeeHearing));

            // Checking that UserHearingRoles are now redacted
            Assert.IsTrue(hearings.All(h =>
                                            h.UserHearingRoles == null));


            // Checking HearingStatus relations
            Assert.IsTrue(hearings.All(h =>
                                            h.HearingStatus != null &&
                                            !h.HearingStatus.Hearings.Any()));

            // Checking HearingType relations
            Assert.IsTrue(hearings.All(h =>
                                            h.HearingType != null));

            // Checking Content relations
            Assert.IsTrue(hearings.All(h => h.Contents != null &&
                                          h.Contents.All(c =>
                                              c.Hearing != null &&
                                              CanCitizenAndAnonymousSeeHearing(c.Hearing) &&
                                              c.Hearing.Contents.Count() == h.Contents.Count() &&
                                              c.ContentType != null &&
                                              !c.ContentType.Contents.Any())));
        }

        private static bool CanCitizenAndAnonymousSeeHearing(Hearing hearing)
        {
            return (!hearing.ClosedHearing && !hearing.IsInternalHearing() && hearing.IsHearingPublished());
        }
    }
}