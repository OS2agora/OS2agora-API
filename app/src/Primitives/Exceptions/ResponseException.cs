namespace Agora.Primitives.Exceptions
{
    public class ResponseException : BaseException
    {
        public ResponseException(string message, int errorCode)
            : base(message, errorCode)
        {
        }
    }
}