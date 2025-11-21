namespace Agora.Operations.Common.Constants
{
    public static class FilterOperations
    {
        public static FilterOperation Equal => new FilterOperation
        {
            Name = "eq",
            MethodName = "GetEqualOperationExpression"
        };

        public static FilterOperation Contains => new FilterOperation
        {
            Name = "contains", 
            MethodName = "GetContainsOperationExpression"
        };
    }

    public class FilterOperation
    {
        public string Name { get; set; }
        public string MethodName { get; set; }
    }
}
