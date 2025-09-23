using Moq;
using NovaSec;
using NovaSec.Attributes;
using NovaSec.Exceptions;
using NovaSec.Compiler;
using NovaSec.Compiler.Resolvers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NovaSecTest
{
    public class SecurityContextTest
    {
        private Mock<ISecurityExpressionRoot> _securityExpressionRoot;
        private Mock<IInjectResolver> _injectResolverMock;
        private SecurityContext _systemUnderTest;
        private RedactionTestObject _redactionObject;
        private List<RedactionTestObject> _resultCollection;

        [SetUp]
        public void Setup()
        {
            _securityExpressionRoot = new Mock<ISecurityExpressionRoot>();
            _injectResolverMock = new Mock<IInjectResolver>();
            _systemUnderTest = new SecurityContext(_securityExpressionRoot.Object, _injectResolverMock.Object);
            _redactionObject = new RedactionTestObject
            {
                NormalProperty = "some property value",
                NormalField = "some field value",
                Nested = new RedactionTestObject
                {
                    NormalProperty = "some other property value",
                    NormalField = "some other field value",
                    ListProperty = new List<RedactionTestObject>
                    {
                        new RedactionTestObject
                        {
                            NormalProperty = "NormalProperty value",
                            NormalField = "NormalField value"
                        }
                    }
                },
                ListProperty = new List<RedactionTestObject>
                {
                    new RedactionTestObject
                    {
                        NormalProperty = "NormalProperty value",
                        NormalField = "NormalField value"
                    }
                }
            };
            _resultCollection = new List<RedactionTestObject>
            {
                new RedactionTestObject
                {
                    Id = 1,
                    NormalProperty = "NormalProperty value 1",
                    Nested = new RedactionTestObject
                    {
                        Id = 11,
                        NormalProperty = "NormalProperty value 11",
                    }
                },
                new RedactionTestObject
                {
                    Id = 2,
                    NormalProperty = "NormalProperty value 1",
                    Nested = new RedactionTestObject
                    {
                        Id = 21,
                        NormalProperty = "NormalProperty value 21",
                    }
                },
                new RedactionTestObject
                {
                    Id = 3,
                    NormalProperty = "NormalProperty value 1",
                    Nested = new RedactionTestObject
                    {
                        Id = 31,
                        NormalProperty = "NormalProperty value 31",
                    }
                }
            };
        }

        [Test]
        public void RedactionReductionTest()
        {
            var redaction = new List<List<string>>()
            {
                new List<string>
                {
                    "a.b.c",
                    "a.c"
                },
                new List<string>
                {
                    "a.b.d",
                    "a.c.e"
                }
            };

            var result = _systemUnderTest.FindCommonRedactionPaths(redaction);
            Assert.IsNotNull(result.FirstOrDefault(s => s == "a.c"));
            Assert.IsNotNull(result.FirstOrDefault(s => s == "a.b.c"));
            Assert.IsNotNull(result.FirstOrDefault(s => s == "a.b.d"));
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void RedactListPropertyTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string>{"ListProperty"});
            Assert.IsNull(_redactionObject.ListProperty);
            Assert.IsNotNull(_redactionObject.Nested.NormalProperty);
        }

        [Test]
        public void RedactListPropertyNestedPropertyTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> { "ListProperty.NormalProperty" });
            Assert.IsTrue(_redactionObject.ListProperty.All(x => x.NormalProperty == null));
            Assert.IsTrue(_redactionObject.ListProperty.All(x => x.NormalField != null));
            Assert.IsNotNull(_redactionObject.ListProperty);
        }

        [Test]
        public void RedactListPropertyNestedFieldTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> { "ListProperty.NormalField" });
            Assert.IsTrue(_redactionObject.ListProperty.All(x => x.NormalField == null));
            Assert.IsTrue(_redactionObject.ListProperty.All(x => x.NormalProperty != null));
            Assert.IsNotNull(_redactionObject.ListProperty);
        }

        [Test]
        public void RedactNestedListPropertyNestedFieldTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> { "Nested.ListProperty.NormalField" });
            Assert.IsTrue(_redactionObject.Nested.ListProperty.All(x => x.NormalField == null));
            Assert.IsTrue(_redactionObject.Nested.ListProperty.All(x => x.NormalProperty != null));
            Assert.IsNotNull(_redactionObject.Nested.ListProperty);
        }

        [Test]
        public void RedactNestedListPropertyNestedPropertyTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> { "Nested.ListProperty.NormalProperty" });
            Assert.IsTrue(_redactionObject.Nested.ListProperty.All(x => x.NormalProperty == null));
            Assert.IsTrue(_redactionObject.Nested.ListProperty.All(x => x.NormalField != null));
            Assert.IsNotNull(_redactionObject.Nested.ListProperty);
        }

        [Test]
        public void RedactNestedListProperty()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> { "Nested.ListProperty" });
            Assert.IsNull(_redactionObject.Nested.ListProperty);
            Assert.IsNotNull(_redactionObject.Nested);
        }

        [Test]
        public void RedactImmediatePropertyTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> {"NormalProperty"});
            Assert.IsNull(_redactionObject.NormalProperty);
            Assert.IsNotNull(_redactionObject.Nested.NormalProperty);
        }

        [Test]
        public void RedactNestedPropertyTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> {"Nested.NormalProperty"});
            Assert.IsNotNull(_redactionObject.NormalProperty);
            Assert.IsNull(_redactionObject.Nested.NormalProperty);
        }

        [Test]
        public void RedactInaccessibleFieldTest()
        {
            Assert.Throws<SecurityExpressionRedactionException>(() =>
                _systemUnderTest.RedactObject(_redactionObject, new List<string> {"PrivateField"}));
        }

        [Test]
        public void RedactInaccessiblePropertyTest()
        {
            Assert.Throws<SecurityExpressionRedactionException>(() =>
                _systemUnderTest.RedactObject(_redactionObject, new List<string> {"PrivateProperty"}));
        }

        [Test]
        public void RedactPropertyWithoutGetterTest()
        {
            _systemUnderTest.RedactObject(_redactionObject, new List<string> {"PropertyWithNoGetter"});
            Assert.Pass();
        }

        [Test]
        public void RedactPropertyWithoutSetterTest()
        {
            Assert.Throws<SecurityExpressionRedactionException>(() =>
                _systemUnderTest.RedactObject(_redactionObject, new List<string> {"PropertyWithNoSetter"}));
        }

        [Test]
        public void PreAuthorizeAcceptTest()
        {
            var attributes = new List<PreAuthorizeAttribute>
                {
                    new PreAuthorizeAttribute("#alwaysTrue"),
                    new PreAuthorizeAttribute("#alwaysFalse")
                };
            var methodInfo = typeof(AccessTestClass).GetMethod("AllwaysTrueAllwaysFalse");

            var result = _systemUnderTest.PreAuthorize(methodInfo, new object[] { true, false }, attributes);
            Assert.IsTrue(result);
        }

        [Test]
        public void PreAuthorizeRejectTest()
        {
            var attributes = new List<PreAuthorizeAttribute>
                {
                    new PreAuthorizeAttribute("#alwaysFalse")
                };
            var methodInfo = typeof(AccessTestClass).GetMethod("AllwaysFalse");
            var result = _systemUnderTest.PreAuthorize(methodInfo, new object[] {false}, attributes);
            Assert.IsFalse(result);
        }

        [Test]
        public void PostFilterPassiveAcceptTest()
        {
            var result = _systemUnderTest.PostFilter<RedactionTestObject>(_resultCollection, new List<PostFilterAttribute>());
            Assert.AreEqual(_resultCollection, result);
        }

        [Test]
        public void PostFilterAcceptWithoutRedactionTest()
        {
            var attributes = new List<PostFilterAttribute>{ new PostFilterAttribute("true") };
            var result = _systemUnderTest.PostFilter(_resultCollection, attributes);
            Assert.IsTrue(_resultCollection != result);
            Assert.AreEqual(_resultCollection.Count, result.Count());
            var resultAsList = result.ToList();
            for (var i = 0; i < result.Count(); i++)
            {
                Assert.AreEqual(_resultCollection[i], resultAsList[i]);
            }
        }

        [Test]
        public void PostFilterRejectAllTest()
        {
            var attributes = new List<PostFilterAttribute> { new PostFilterAttribute("false") };
            var result = _systemUnderTest.PostFilter(_resultCollection, attributes);
            Assert.IsTrue(_resultCollection != result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void PostFilterAcceptId2Test()
        {
            var attributes = new List<PostFilterAttribute> { new PostFilterAttribute("resultObject.Id == #id2") };
            var methodInfo = typeof(RedactionTestObject).GetMethod("Ids");
            var result = _systemUnderTest.PostFilter(_resultCollection, attributes, methodInfo, new object[] { 1, 2 });

            Assert.AreEqual(1, result.Count());
            var item1 = result.ToList()[0];
            Assert.AreEqual(2, item1.Id);
            Assert.IsNotNull(item1.NormalProperty);
        }

        [Test]
        public void PostFilterAcceptId2WithRedactionTest()
        {
            var attributes = new List<PostFilterAttribute> { new PostFilterAttribute("resultObject.Id == #id2", "NormalProperty") };
            var methodInfo = typeof(RedactionTestObject).GetMethod("Ids");
            var result = _systemUnderTest.PostFilter(_resultCollection, attributes, methodInfo, new object[] { 1, 2 });

            Assert.AreEqual(1, result.Count());
            var item1 = result.ToList()[0];
            Assert.AreEqual(2, item1.Id);
            Assert.IsNull(item1.NormalProperty);
        }

        [Test]
        public void PostFilterInterleavedRedactionTest()
        {
            var attributes = new List<PostFilterAttribute>
            {
                new PostFilterAttribute("resultObject.Id == #id1"),
                new PostFilterAttribute("resultObject.Id == #id2", "NormalProperty"),
                new PostFilterAttribute("resultObject.Id >= #id2", "Nested"),
            };
            var methodInfo = typeof(RedactionTestObject).GetMethod("Ids");
            var result = _systemUnderTest.PostFilter(_resultCollection, attributes, methodInfo, new object[] { 1, 2 });

            Assert.AreEqual(3, result.Count());
            var resultAsList = result.ToList();
            var itemWithId1 = resultAsList.First(i => i.Id == 1);
            var itemWithId2 = resultAsList.First(i => i.Id == 2);
            var itemWithId3 = resultAsList.First(i => i.Id == 3);

            Assert.IsNotNull(itemWithId1.NormalProperty);
            Assert.IsNotNull(itemWithId1.Nested);
            Assert.IsNull(itemWithId2.NormalProperty);
            Assert.IsNull(itemWithId2.Nested);
            Assert.IsNotNull(itemWithId3.NormalProperty);
            Assert.IsNull(itemWithId3.Nested);
        }
    }
}