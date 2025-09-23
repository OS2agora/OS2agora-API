using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Api.Models.JsonApi.Interfaces;

namespace BallerupKommune.Api.Models.Common
{
    public abstract class BaseDto<T> : BaseModifyControlDto<T>, IJsonApiResource, IValidatableObject
        where T : BaseAttributeDto, IJsonApiAttributes
    {
        public T Attributes { get; set; }

        public string Id { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Type != JsonApiType)
            {
                results.Add(new ValidationResult(
                    $"The provided resource type ({Type}) did not correspond to the expected type ({JsonApiType})",
                    new[] {"Type"}));
            }

            var allowedRelationshipNames = AllowedRelationships.Select(r => r.RelationName).ToList();

            if (Relationships != null)
            {
                foreach (var (relationshipName, relationship) in Relationships)
                {
                    var allowedRelationship = AllowedRelationships.FirstOrDefault(allowedRelation =>
                        string.Equals(allowedRelation.RelationName, relationshipName,
                            StringComparison.CurrentCultureIgnoreCase));

                    if (allowedRelationship != null)
                    {
                        if (relationship.Data is RelationshipResourceIdentifierDto
                                allowedRelationshipAsRelationshipResourceIdentifierDto &&
                            allowedRelationshipAsRelationshipResourceIdentifierDto.Type !=
                            allowedRelationship.Type)
                        {
                            results.Add(new ValidationResult(
                                $"Type in relationship {relationshipName} did not match expectation. Allowed is {allowedRelationship.Type}",
                                new[] {"relationships." + relationshipName}));
                        }
                    }
                    else
                    {
                        results.Add(new ValidationResult(
                            "Unexpected relationship " + relationshipName +
                            $". Supported are {string.Join(", ", allowedRelationshipNames)}",
                            new[] {"relationships." + relationshipName}));
                    }
                }
            }

            return results;
        }
    }
}