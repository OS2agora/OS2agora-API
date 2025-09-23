using System;
using System.Collections.Generic;
using System.Reflection;
using BallerupKommune.DAOs.Utility;
using BallerupKommune.Entities.Attributes;
using BallerupKommune.Entities.Common;
using BallerupKommune.Models.Common;
using NUnit.Framework;
using InvalidOperationException = BallerupKommune.Operations.Common.Exceptions.InvalidOperationException;

namespace BallerupKommune.DAOs.UnitTests.Utility
{
    public class IncludePropertiesExtensionsTests
    {
        [TestCaseSource(nameof(ValidTestCases))]
        public void ValidateRequestIncludes_ValidCases_DoesNotThrow(Type entityType, List<string> requestIncludes)
        {
            var includes = new IncludeProperties(requestIncludes);
            MethodInfo validateMethod = GetValidateMethod(entityType);
            Assert.DoesNotThrow(() => validateMethod.Invoke(null, new object[] {includes}));
        }

        [TestCaseSource(nameof(InvalidTestCases))]
        public void ValidateRequestIncludes_InvalidCases_Throws(Type entityType, List<string> requestIncludes)
        {
            var includes = new IncludeProperties(requestIncludes);
            MethodInfo validateMethod = GetValidateMethod(entityType);
            var exception =
                Assert.Throws<TargetInvocationException>(() => validateMethod.Invoke(null, new object[] {includes}));
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        private static IEnumerable<object[]> ValidTestCases()
        {
            yield return new object[] {typeof(TestEntityA), new List<string>()};
            yield return new object[] {typeof(TestEntityA), new List<string> {"B"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"B.As"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.A"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.Bs"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.Bs.As"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"B", "Cs"}};

            yield return new object[] {typeof(TestEntityB), new List<string>()};
            yield return new object[] {typeof(TestEntityB), new List<string> {"As"}};

            yield return new object[] {typeof(TestEntityC), new List<string>()};
            yield return new object[] {typeof(TestEntityC), new List<string> {"A"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"Bs"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"Bs.As"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"A", "Bs"}};
        }

        private static IEnumerable<object[]> InvalidTestCases()
        {
            yield return new object[] {typeof(TestEntityA), new List<string> {"B.C"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"B.As.B"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"B.As.Cs"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.A.B"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.A.Cs"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.Bs.C"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.Bs.As.B"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"Cs.Bs.As.Cs"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"A", "B.C"}};
            yield return new object[] {typeof(TestEntityA), new List<string> {"B.C", "A"}};

            yield return new object[] {typeof(TestEntityB), new List<string> {"C"}};
            yield return new object[] {typeof(TestEntityB), new List<string> {"As.B"}};
            yield return new object[] {typeof(TestEntityB), new List<string> {"As.Cs"}};

            yield return new object[] {typeof(TestEntityC), new List<string> {"A.B"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"A.Cs"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"Bs.C"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"Bs.As.B"}};
            yield return new object[] {typeof(TestEntityC), new List<string> {"Bs.As.Cs"}};
        }

        private class TestEntityA : BaseEntity
        {
            [AllowRequestInclude(2)] public TestEntityB B { get; set; }

            [AllowRequestInclude(3)] public ICollection<TestEntityC> Cs { get; set; } = new List<TestEntityC>();
        }

        private class TestEntityB : BaseEntity
        {
            public TestEntityC C { get; set; }

            [AllowRequestInclude] public ICollection<TestEntityA> As { get; set; } = new List<TestEntityA>();
        }

        private class TestEntityC : BaseEntity
        {
            [AllowRequestInclude] public TestEntityA A { get; set; }

            [AllowRequestInclude(2)] public ICollection<TestEntityB> Bs { get; set; } = new List<TestEntityB>();
        }

        private static MethodInfo GetValidateMethod(Type entityType)
        {
            return typeof(IncludePropertiesExtensions)
                .GetMethod(nameof(IncludePropertiesExtensions.ValidateRequestIncludes))!
                .MakeGenericMethod(entityType);
        }
    }
}