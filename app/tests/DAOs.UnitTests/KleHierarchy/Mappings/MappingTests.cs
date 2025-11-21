using Agora.DAOs.KleHierarchy.DTOs;
using Agora.DAOs.KleHierarchy.Mappings;
using AutoMapper;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace Agora.DAOs.UnitTests.KleHierarchy.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void AssertIsConfigurationValid()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(typeof(KleMainGroupDto), typeof(Agora.Models.Models.KleHierarchy))]
        [TestCase(typeof(KleGroupDto), typeof(Agora.Models.Models.KleHierarchy))]
        [TestCase(typeof(KleTopicDto), typeof(Agora.Models.Models.KleHierarchy))]
        public void TestMappingFromSourceToDestination(Type source, Type destination)
        {
            var sourceInstance = GetInstanceOf(source);
            var destinationInstance = GetInstanceOf(destination);

            // Map from Jsonfile to Model
            _mapper.Map(sourceInstance, destinationInstance);
        }

        private object GetInstanceOf(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null
                ? Activator.CreateInstance(type)
                : FormatterServices.GetUninitializedObject(type);
        }
    }
}