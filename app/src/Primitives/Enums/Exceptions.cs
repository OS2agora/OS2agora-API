namespace BallerupKommune.Primitives.Enums
{
    public enum Exceptions
    {
        BuiltInException = 0,
        EntityNotFound = 10,
        InvalidId = 20,
        ValidationFailed = 30,
        MapExpressionToModelField = 31,
        Transaction = 40,
        ParseError = 50,
        Unauthorized = 100,
        MethodNotAllowed = 110,
        DeleteBreaksForeignKeys = 200,
        BadRequest = 400,
        Forbidden = 403,
        NetworkAuthenticationRequired = 511,
        FileTooLarge = 601,
        FileIsEmpty = 611
    }
}