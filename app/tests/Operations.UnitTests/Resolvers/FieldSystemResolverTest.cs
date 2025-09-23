using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Resolvers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumFieldType = BallerupKommune.Models.Enums.FieldType;

namespace BallerupKommune.Operations.UnitTests.Resolvers
{
    public class FieldSystemResolverTest
    {

        private FieldSystemResolver _resolver;
        private Mock<IHearingTemplateDao> _hearingTemplateDaoMock;

        [SetUp]
        public void SetUp()
        {
            _hearingTemplateDaoMock = new Mock<IHearingTemplateDao>();
            _resolver = new FieldSystemResolver(_hearingTemplateDaoMock.Object);
        }

        [Test]
        public async Task GetFieldSystem_all()
        {
            var fieldSystem = new List<HearingTemplate>
            {
                new HearingTemplate {
                    Id = 1,
                    Name = "Default Template",
                    Fields = new List<Field>
                    {
                        new Field {
                            Id = 1,
                            Name = "ESDH Titel",
                            FieldType = new FieldType
                            {
                                Id = 5,
                                Type = BallerupKommune.Models.Enums.FieldType.ESDH_TITLE,
                            }
                        },
                        new Field {
                            Id = 2,
                            Name = "Titel",
                            FieldType = new FieldType
                            {
                                Id = 6,
                                Type = BallerupKommune.Models.Enums.FieldType.TITLE,
                            }
                        },
                        new Field {
                            Id = 3,
                            Name = "Summary",
                            FieldType = new FieldType
                            {
                                Id = 3,
                                Type = BallerupKommune.Models.Enums.FieldType.SUMMARY,
                            }
                        },
                        new Field {
                            Id = 4,
                            Name = "BodyInformation",
                            FieldType = new FieldType
                            {
                                Id = 2,
                                Type = BallerupKommune.Models.Enums.FieldType.BODYINFORMATION,
                            }
                        },
                        new Field {
                            Id = 5,
                            Name = "Conclusion",
                            FieldType = new FieldType
                            {
                                Id = 1,
                                Type = BallerupKommune.Models.Enums.FieldType.CONCLUSION,
                            }
                        },
                        new Field {
                            Id = 6,
                            Name = "Image",
                            FieldType = new FieldType
                            {
                                Id = 4,
                                Type = BallerupKommune.Models.Enums.FieldType.IMAGE,
                            }
                        }
                    }
                }
            };
            _hearingTemplateDaoMock
                .Setup(dao => dao.GetAllAsync(It.IsAny<IncludeProperties>())).ReturnsAsync(fieldSystem);

            List<HearingTemplate> hearingTemplatesResult = await _resolver.GetFieldSystem();

            Assert.That(hearingTemplatesResult,Is.EqualTo(fieldSystem));
        }

        [Test]
        public async Task GetFieldsIds_FromTitel()
        {
            var fieldSystem = new List<HearingTemplate>
            {
                new HearingTemplate {
                    Id = 1,
                    Name = "Default Template",
                    Fields = new List<Field>
                    {
                        new Field {
                            Id = 1,
                            Name = "ESDH Titel",
                            FieldType = new FieldType
                            {
                                Id = 5,
                                Type = EnumFieldType.ESDH_TITLE,
                            }
                        },
                        new Field {
                            Id = 2,
                            Name = "Titel",
                            FieldType = new FieldType
                            {
                                Id = 6,
                                Type = EnumFieldType.TITLE,
                            }
                        },
                        new Field {
                            Id = 3,
                            Name = "Summary",
                            FieldType = new FieldType
                            {
                                Id = 3,
                                Type = EnumFieldType.SUMMARY,
                            }
                        },
                        new Field {
                            Id = 4,
                            Name = "BodyInformation",
                            FieldType = new FieldType
                            {
                                Id = 2,
                                Type = EnumFieldType.BODYINFORMATION,
                            }
                        },
                        new Field {
                            Id = 5,
                            Name = "Conclusion",
                            FieldType = new FieldType
                            {
                                Id = 1,
                                Type = EnumFieldType.CONCLUSION,
                            }
                        },
                        new Field {
                            Id = 6,
                            Name = "Image",
                            FieldType = new FieldType
                            {
                                Id = 4,
                                Type = EnumFieldType.IMAGE,
                            }
                        }
                    }
                }
            };
            _hearingTemplateDaoMock
                .Setup(dao => dao.GetAllAsync(It.IsAny<IncludeProperties>())).ReturnsAsync(fieldSystem);

            List<int> hearingTemplatesResult = await _resolver.GetFieldsIds(EnumFieldType.TITLE);
            List<int> correctResult = new List<int>
            {
                2,
            };

            Assert.That(hearingTemplatesResult, Is.EqualTo(correctResult));

        }

    }
}
