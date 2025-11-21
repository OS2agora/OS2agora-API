using System.Collections;
using Agora.DAOs.Utility;
using Agora.Models.Common;
using Agora.Models.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Agora.DAOs.UnitTests.Utility
{
    public class ModelPruneUtilityTests
    {
        private bool InheritsFromBaseModel(PropertyInfo propInfo) =>
            propInfo.PropertyType.IsSubclassOf(typeof(BaseModel));

        private bool IsCollectionProperty(PropertyInfo propInfo) =>
            propInfo.PropertyType.IsConstructedGenericType &&
            propInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>);


        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void PruneIncludes_IncludesIsEmptyOrNull(bool shouldBeNull)
        {
            IncludeProperties includes = shouldBeNull ? null : new IncludeProperties();

            var model = new Hearing
            {
                Comments = new List<Comment>
                {
                    new Comment()
                },
                Contents = new List<Content>
                {
                    new Content()
                },
                UserHearingRoles = new List<UserHearingRole>
                {
                    new UserHearingRole()
                },
                HearingStatus = new HearingStatus(),
                HearingType = new HearingType(),
                KleHierarchy = new Agora.Models.Models.KleHierarchy()
            };

            var prunedModel = ModelPruneUtility.PruneIncludes(model, includes);

            Assert.IsTrue(prunedModel.Comments.Count == 0 && 
                          prunedModel.Contents.Count == 0 && 
                          prunedModel.UserHearingRoles.Count == 0 && 
                          prunedModel.HearingStatus == null &&
                          prunedModel.HearingType == null &&
                          prunedModel.KleHierarchy == null);
        }

        [Test]
        [TestCase("OneToOneChild")]
        [TestCase("ManyToOneChild")]
        [TestCase("OneToManyChild")]
        [TestCase("OneToOneChild", "ManyToOneChild")]
        [TestCase("OneToOneChild", "OneToManyChild")]
        [TestCase("ManyToOneChild", "OneToManyChild")]
        [TestCase("OneToOneChild", "ManyToOneChild", "OneToManyChild")]
        public void PruneIncludes_NonNestedIncludes(params string[] includeStrings)
        {
            var model = GenerateModelWithChildren();

            var includeProperties = new IncludeProperties(null, null, includeStrings.ToList());

            var prunedModel = ModelPruneUtility.PruneIncludes(model, includeProperties);

            var modelType = prunedModel.GetType();

            // Check that included properties have not been removed
            foreach (var include in includeStrings)
            {
                var property = modelType.GetProperty(include);

                if (IsCollectionProperty(property))
                {
                    var value = property.GetValue(prunedModel) as ICollection;
                    Assert.IsNotNull(value);
                    Assert.IsNotEmpty(value);

                    // Check that for all props, the reverse reference to MainModel is null
                    foreach (var subValue in value)
                    {
                        var parentProp = subValue.GetType().GetProperty(nameof(MainModel));
                        var parentValue = parentProp.GetValue(subValue);
                        Assert.IsNull(parentValue);
                    }
                }
                else
                {
                    var value = property.GetValue(prunedModel);
                    Assert.IsNotNull(value);

                    var parentProp = value.GetType().GetProperty(nameof(MainModel));

                    if (IsCollectionProperty(parentProp))
                    {
                        var parentValue = parentProp.GetValue(value) as ICollection;
                        Assert.IsNotNull(parentValue);
                        Assert.IsEmpty(parentValue);
                    }
                    else
                    {
                        var parentValue = parentProp.GetValue(value);
                        Assert.IsNull(parentValue);
                    }
                }
            }

            // Check that non-included properties has been removed
            var allProperties = modelType.GetProperties().Where(prop =>IsCollectionProperty(prop) || InheritsFromBaseModel(prop));

            foreach (var prop in allProperties)
            {
                if (includeStrings.All(include => include != prop.Name))
                {
                    if (IsCollectionProperty(prop))
                    {
                        var value = prop.GetValue(prunedModel) as ICollection;
                        Assert.IsNotNull(value);
                        Assert.IsEmpty(value);
                    }
                    else
                    {
                        var value = prop.GetValue(prunedModel);
                        Assert.IsNull(value);
                    }
                }
            }
        }

        [Test]
        public void PruneModel_ReverseIncluded()
        {
            var include = new List<string> {"OneToOneChild.MainModel", "ManyToOneChild", "OneToManyChild"};
            var includeProperties = new IncludeProperties(null, null, include.ToList());

            var model = GenerateModelWithChildren();
            var prunedModel = ModelPruneUtility.PruneIncludes(model, includeProperties);

            Assert.IsNotNull(prunedModel.ManyToOneChild);
            Assert.IsNotEmpty(prunedModel.OneToManyChild);
            Assert.IsNotNull(prunedModel.OneToOneChild);
        }

        public MainModel GenerateModelWithChildren()
        {
            var result = new MainModel
            {
                OneToOneChild = new OneToOneChild(),
                ManyToOneChild = new ManyToOneChild(),
                OneToManyChild = new List<OneToManyChild>{ new OneToManyChild(), new OneToManyChild()}
            };

            result.OneToOneChild.MainModel = result;
            result.ManyToOneChild.MainModel = new List<MainModel> {result, new MainModel()};
            result.OneToManyChild.ToList().ForEach(child => child.MainModel = result);

            return result;
        }

        public class MainModel : BaseModel
        {
            public OneToOneChild OneToOneChild { get; set; }
            public ManyToOneChild ManyToOneChild { get; set; }
            public ICollection<OneToManyChild> OneToManyChild { get; set; }
        }

        public class OneToOneChild : BaseModel
        {
            public MainModel MainModel { get; set; }
        }

        public class ManyToOneChild : BaseModel
        {
            public ICollection<MainModel> MainModel { get; set; }
        }

        public class OneToManyChild : BaseModel
        {
            public MainModel MainModel { get; set; }
        }
    }
}