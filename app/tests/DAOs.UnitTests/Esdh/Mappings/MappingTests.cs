using Agora.DAOs.Esdh.Mappings;
using Agora.DAOs.Esdh.Sbsip.DTOs;
using Agora.Models.Models.Esdh;
using AutoMapper;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;

namespace Agora.DAOs.UnitTests.Esdh.Mappings
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
        [TestCase(typeof(SagDtoV10), typeof(Case))]
        [TestCase(typeof(SagDto), typeof(Case))]
        public void TestMappingFromSourceToDestination(Type source, Type destination)
        {
            var sourceInstance = GetInstanceOf(source);
            var destinationSource = GetInstanceOf(destination);

            // Map from Esdh to Model
            _mapper.Map(sourceInstance, destinationSource);

            // Map from Model To Esdh
            _mapper.Map(destinationSource, sourceInstance);
        }

        private object GetInstanceOf(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null
                ? Activator.CreateInstance(type)
                : FormatterServices.GetUninitializedObject(type);
        }
    }
}