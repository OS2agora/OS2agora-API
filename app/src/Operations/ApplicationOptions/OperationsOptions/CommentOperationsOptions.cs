namespace Agora.Operations.ApplicationOptions.OperationsOptions
{
    public class CommentOperationsOptions
    {
        public const string Comments = "Comments";

        public CreateCommentOptions CreateComment { get; set; }


        public class CreateCommentOptions
        {
            public int HearingOwnerResponseLimit { get; set; } = -1;
            public int ResponseLimit { get; set; } = -1;
        }
    }
}