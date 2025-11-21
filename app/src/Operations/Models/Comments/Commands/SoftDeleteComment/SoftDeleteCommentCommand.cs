using System.Collections.Generic;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.Exceptions;
using NovaSec.Attributes;

namespace Agora.Operations.Models.Comments.Commands.SoftDeleteComment
{
    [PreAuthorize("@Security.IsCommentOwnerByCommentId(#request.Id)")]
    public class SoftDeleteCommentCommand : IRequest<Comment>
    {
        public int Id { get; set; }
        public int HearingId { get; set; }

        public class SoftDeleteCommentCommandHandler : IRequestHandler<SoftDeleteCommentCommand, Comment>
        {
            private readonly ICommentDao _commentDao;

            public SoftDeleteCommentCommandHandler(ICommentDao commentDao)
            {
                _commentDao = commentDao;
            }

            public async Task<Comment> Handle(SoftDeleteCommentCommand request, CancellationToken cancellationToken)
            {
                var currentComment = await _commentDao.GetAsync(request.Id);

                if (currentComment == null)
                {
                    throw new NotFoundException(nameof(Comment), request.Id);
                }

                currentComment.IsDeleted = true;
                currentComment.PropertiesUpdated = new List<string> {nameof(Comment.IsDeleted)};

                var defaultIncludes = IncludeProperties.Create<Comment>();
                var updatedComment = await _commentDao.UpdateAsync(currentComment, defaultIncludes);

                return updatedComment;
            }
        }
    }
}