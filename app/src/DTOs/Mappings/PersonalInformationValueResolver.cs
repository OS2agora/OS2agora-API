using Agora.DTOs.Models;
using Agora.Models.Models;
using AutoMapper;
using System.Collections.Generic;

namespace Agora.DTOs.Mappings
{
    /// <summary>
    /// Resolves the value of a specified <see langword="string"/> property from a <see cref="User"/> object to a <see cref="UserDto"/> object,
    /// based on the provided resolution context and options.
    /// </summary>
    /// <remarks>This resolver checks for the presence of the <see cref="_includePersonalInfo"/>
    /// option in the resolution context. If the option is set to <see langword="true"/>, the resolver retrieves the
    /// value of the specified property from the source object. Otherwise, it returns <see langword="null"/>.</remarks>
    public class PersonalInformationValueResolver : IValueResolver<User, UserDto, string>
    {
        private const string _includePersonalInfo = "IncludePersonalInfo";
        private readonly string _propertyName;

        /// <summary>
        /// Constructs <see cref="PersonalInformationValueResolver"/>.
        /// </summary>
        /// <param name="propertyName">The name of the source property to map from on the <see cref="User"/> object.</param>
        public PersonalInformationValueResolver(string propertyName)
        {
            _propertyName = propertyName;
        }

        // Parameterless constructor must be here for DI. The value resolver should be revoked via the constructor with the property name.
        public PersonalInformationValueResolver() {}

        public static KeyValuePair<string, object> GetIncludePersonalInfoOption(bool includePersonalInfo)
        {
            return new KeyValuePair<string, object>(_includePersonalInfo, includePersonalInfo);
        }

        public string Resolve(User source, UserDto destination, string destMember, ResolutionContext context)
        {
            if (context.Options.Items.TryGetValue(_includePersonalInfo, out var includePersonalInformationObj) &&
                includePersonalInformationObj is bool includePersonalInformation &&
                includePersonalInformation)
            {
                var sourceProp = typeof(User).GetProperty(_propertyName);
                var sourcePropValue = sourceProp?.GetValue(source) as string;
                return sourcePropValue;
            }
            else
            {
                return null;
            }
        }
    }
}