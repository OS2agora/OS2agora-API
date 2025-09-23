using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Models.Comments.Queries.GetComments;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.Comments.Queries
{
    public class GetCommentQueryTests : ModelsTestBase<GetCommentsQuery, List<Comment>>
    {
        public GetCommentQueryTests()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns((List<string> param) => param.Any(x => SecurityExpressionRoot.Object.HasRole(x)));
        }

        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResult()));

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingReviewer(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsCommentApproved(It.IsAny<Comment>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsCommentOwner(It.IsAny<Comment>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsCommentOwnerByCommentId(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.CanSeeHearing( It.IsAny<int>()))
                .Returns(true);

            SecurityExpressionsMock.Setup(x => x.CanHearingShowComments(It.IsAny<int>()))
                .Returns(true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetComment_CanSeeHearing(bool canSeeHearing)
        {
            var comments = GetHandlerResult();

            SecurityExpressionsMock.Setup(x => x.CanSeeHearing( It.IsAny<int>()))
                .Returns(canSeeHearing);

            // Returns true so that at least one PostFilter attribute evaluates to true
            SecurityExpressionsMock.Setup(x => x.IsCommentOwner(It.IsAny<Comment>())).Returns(true);

            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(comments));

            var request = new GetCommentsQuery();
            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (!canSeeHearing)
            {
                Assert.IsEmpty(result);
            }
            else
            {
                Assert.That(result.Count, Is.EqualTo(3));
                Assert.IsTrue(result.All(x => x.Hearing != null && !x.ContainsSensitiveInformation
                                                                && x.Contents.Any() && x.CommentType != null));
            }
        }

        [Test]
        [TestCase(false, false, false, true)]
        [TestCase(false, false, true, true)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(true, false, false, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, true)]
        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, false)]
        public async Task GetComment_Citizen_Employee_Anonymous(bool isCommentApproved, bool isACommentDeleted, bool hearingAllowsCommentsToBeSeen, bool isCommentResponseReply)
        {
            var comments = GetHandlerResult(deletedComment: isACommentDeleted);

            SecurityExpressionsMock.Setup(x => x.IsCommentApproved(It.IsAny<Comment>()))
                .Returns(isCommentApproved);

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string role) => role == Security.Roles.Anonymous || role == Security.Roles.Citizen || role == Security.Roles.Employee);

            SecurityExpressionsMock.Setup(x => x.CanHearingShowComments( It.IsAny<int>()))
                .Returns(hearingAllowsCommentsToBeSeen);

            SecurityExpressionsMock.Setup(x => x.IsCommentResponseReply((It.IsAny<Comment>())))
                .Returns(isCommentResponseReply);

            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(comments));

            var request = new GetCommentsQuery();
            var result =
                    await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (!isCommentApproved || !hearingAllowsCommentsToBeSeen || isCommentResponseReply)
            {
                Assert.IsEmpty(result);
            }
            else
            {
                if (isACommentDeleted)
                {
                    Assert.That(result.Count, Is.EqualTo(2));
                    Assert.IsTrue(result.All(x => x.Hearing != null && !x.ContainsSensitiveInformation
                                                                    && x.Contents.Any() && x.CommentType != null));
                }
                else
                {
                    Assert.That(result.Count, Is.EqualTo(3));
                    Assert.IsTrue(result.All(x => x.Hearing != null && !x.ContainsSensitiveInformation
                                                                    && x.Contents.Any() && x.CommentType != null));
                }
            }
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(false, false, false)]
        public async Task GetComment_HearingOwner_OwnsComment(bool isHearingOwner, bool ownsComment, bool isACommentDeleted)
        {
            var comments = GetHandlerResult(deletedComment: isACommentDeleted);

            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                .Returns(isHearingOwner);
            SecurityExpressionsMock.Setup(x => x.IsCommentOwnerByCommentId(It.IsAny<int>()))
                .Returns(ownsComment);
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(comments));

            var request = new GetCommentsQuery();
            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);


            if (!isHearingOwner)
            {
                Assert.IsEmpty(result);
            }
            else
            {
                if (isACommentDeleted)
                {
                    Assert.That(result.Count, Is.EqualTo(2));
                    Assert.IsTrue(result.All(x => !x.ContainsSensitiveInformation && x.CommentType != null));
                }
                else
                {
                    Assert.That(result.Count, Is.EqualTo(3));
                    Assert.IsTrue(result.All(x => !x.ContainsSensitiveInformation && x.CommentType != null));
                }
            }
        }

        [Test]
        [TestCase(true, true, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, false, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(false, true, false, true)]
        [TestCase(false, false, true, true)]
        [TestCase(false, false, false, true)]
        [TestCase(true, true, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, false, false, false)]
        public async Task GetComment_Reviewer(bool isCommentApproved, bool isReviewer, bool isACommentDeleted, bool isCommentResponseReply)
        {
            var comments = GetHandlerResult(deletedComment: isACommentDeleted);

            SecurityExpressionsMock.Setup(x => x.IsHearingReviewer( It.IsAny<int>()))
                .Returns(isReviewer);

            SecurityExpressionsMock.Setup(x => x.IsCommentApproved(It.IsAny<Comment>()))
                .Returns(isCommentApproved);

            SecurityExpressionsMock.Setup(x => x.IsCommentResponseReply((It.IsAny<Comment>())))
                .Returns(isCommentResponseReply);

            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(comments));

            var request = new GetCommentsQuery();
            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (isCommentApproved && isReviewer && !isCommentResponseReply)
            {
                if (isACommentDeleted)
                {
                    Assert.That(result.Count, Is.EqualTo(2));
                    Assert.IsTrue(result.All(x => x.Hearing != null && !x.ContainsSensitiveInformation
                                                                     && x.CommentType != null));
                }
                else
                {
                    Assert.That(result.Count, Is.EqualTo(3));
                    Assert.IsTrue(result.All(x => x.Hearing != null && !x.ContainsSensitiveInformation
                                                                     && x.CommentType != null));
                }
            }
            else
            {
                Assert.IsEmpty(result);
            }
        }



        private List<Comment> GetHandlerResult(bool approved = true, bool deletedComment = false)
        {
            return new List<Comment>
            {
                new Comment()
                {
                    Id = 1,
                    Hearing = new Hearing
                    {
                        Id = 11
                    },
                    CommentStatus = new CommentStatus
                    {
                        Status = approved
                            ? BallerupKommune.Models.Enums.CommentStatus.APPROVED
                            : BallerupKommune.Models.Enums.CommentStatus.AWAITING_APPROVAL
                    },
                    ContainsSensitiveInformation = false,
                    IsDeleted = deletedComment,
                    Contents = new List<Content>
                    {
                        new Content()
                    },
                    CommentType = new CommentType(),
                },
                new Comment()
                {
                    Id = 2,
                    Hearing = new Hearing
                    {
                        Id = 22
                    },
                    CommentStatus = new CommentStatus
                    {
                        Status = approved
                            ? BallerupKommune.Models.Enums.CommentStatus.APPROVED
                            : BallerupKommune.Models.Enums.CommentStatus.AWAITING_APPROVAL
                    },
                    ContainsSensitiveInformation = false,
                    IsDeleted = false,
                    Contents = new List<Content>
                    {
                        new Content()
                    },
                    CommentType = new CommentType(),
                },
                new Comment()
                {
                    Id = 3,
                    Hearing = new Hearing
                    {
                        Id = 33
                    },
                    CommentStatus = new CommentStatus
                    {
                        Status = approved
                            ? BallerupKommune.Models.Enums.CommentStatus.APPROVED
                            : BallerupKommune.Models.Enums.CommentStatus.AWAITING_APPROVAL
                    },
                    ContainsSensitiveInformation = false,
                    IsDeleted = false,
                    Contents = new List<Content>
                    {
                        new Content()
                    },
                    CommentType = new CommentType(),
                }
            };
        }
    }
}