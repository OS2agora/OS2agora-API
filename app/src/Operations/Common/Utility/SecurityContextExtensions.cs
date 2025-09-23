using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BallerupKommune.Models.Common;
using NovaSec;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Common.Utility
{
    public static class  SecurityContextExtensions
    {
        public static bool DoesAnyPostFilterOnModelEvaluateToTrue(this SecurityContext securityContext, BaseModel model)
        {
            // Get PostFilterAttributes and remove redactions
            var postFilterAttributes = model.GetType().GetCustomAttributes<PostFilterAttribute>()
                .Select(attribute => new PostFilterAttribute(attribute.SecurityExpression)).ToList();

            var resultCollection = new List<BaseModel> { model };
            return securityContext.PostFilter(resultCollection, postFilterAttributes, null) != null;
        }
    }
}
