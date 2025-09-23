using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Resolvers;
using ContentType = BallerupKommune.Models.Enums.ContentType;
using FieldType = BallerupKommune.Models.Enums.FieldType;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;

namespace BallerupKommune.Operations.Common.Extensions
{
    public static class HearingExtensions
    {
        /// <summary>
        /// Gets the hearing owner of the hearing.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the hearing owner of. The includes <c>"UserHearingRoles"</c> and
        /// <c>"UserHearingRoles.User"</c> are required on the hearing.
        /// </param>
        /// <param name="hearingRoleResolver">Resolver for getting hearing roles.</param>
        /// <returns>The hearing owner user.</returns>
        public static async Task<User> GetHearingOwner(this Hearing hearing, IHearingRoleResolver hearingRoleResolver)
        {
            return (await hearing.GetUsersWithRole(hearingRoleResolver, HearingRole.HEARING_OWNER)).Single();
        }

        /// <summary>
        /// Gets all users with specified hearing role on the hearing.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the hearing owner of. The includes <c>"UserHearingRoles"</c> and
        /// <c>"UserHearingRoles.User"</c> are required on the hearing.
        /// </param>
        /// <param name="hearingRoleResolver">Resolver for getting hearing roles.</param>
        /// <param name="role">The specified role.</param>
        /// <returns>List of users with the specified role on the hearing.</returns>
        public static async Task<List<User>> GetUsersWithRole(this Hearing hearing,
            IHearingRoleResolver hearingRoleResolver, HearingRole role)
        {
            return await hearing.GetUsersWithAnyOfTheRoles(hearingRoleResolver, role);
        }

        /// <summary>
        /// Gets all users with at least one of the specified hearing roles on the hearing.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the hearing owner of. The includes <c>"UserHearingRoles"</c> and
        /// <c>"UserHearingRoles.User"</c> are required on the hearing.
        /// </param>
        /// <param name="hearingRoleResolver">Resolver for getting hearing roles.</param>
        /// <param name="roles">The specified roles.</param>
        /// <returns>List of users with the specified roles on the hearing.</returns>
        public static async Task<List<User>> GetUsersWithAnyOfTheRoles(this Hearing hearing,
            IHearingRoleResolver hearingRoleResolver, params HearingRole[] roles)
        {
            int[] hearingRoleIds = 
                (await hearingRoleResolver.GetHearingRoles(roles.ToArray())).Select(role => role.Id).ToArray();
            
            return hearing.UserHearingRoles
                .Where(userHearingRole => hearingRoleIds.Contains(userHearingRole.HearingRoleId))
                .Select(userHearingRole => userHearingRole.User)
                .ToList();
        }

        /// <summary>
        /// Gets all companies with at least one of the specified hearing roles on the hearing.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the hearing owner of. The includes <c>"CompanyHearingRoles"</c> and
        /// <c>"CompanyHearingRoles.Company"</c> are required on the hearing.
        /// </param>
        /// <param name="hearingRoleResolver">Resolver for getting hearing roles.</param>
        /// <param name="roles">The specified roles.</param>
        /// <returns>List of companies with the specified roles on the hearing.</returns>
        public static async Task<List<Company>> GetCompaniesWithAnyOfTheRoles(this Hearing hearing,
            IHearingRoleResolver hearingRoleResolver, params HearingRole[] roles)
        {
            int[] hearingRoleIds =
                (await hearingRoleResolver.GetHearingRoles(roles.ToArray())).Select(role => role.Id).ToArray();

            return hearing.CompanyHearingRoles
                .Where(companyHearingRole => hearingRoleIds.Contains(companyHearingRole.HearingRoleId))
                .Select(companyHearingRole => companyHearingRole.Company)
                .ToList();
        }
        
        /// <summary>
        /// Gets the text content of specified field type.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the text content from. The includes <c>"Contents"</c> and
        /// <c>"Contents.ContentType"</c> are required on the hearing.
        /// </param>
        /// <param name="fieldSystemResolver">Resolver for getting fields.</param>
        /// <param name="fieldType">The specified field type.</param>
        /// <exception cref="InvalidOperationException">
        /// If there are more than one text content on the hearing of the specified field type.
        /// </exception>
        /// <returns>The content. Will be <c>null</c> if not found.</returns>
        public static async Task<Content> GetTextContentOfFieldType(this Hearing hearing,
            IFieldSystemResolver fieldSystemResolver, FieldType fieldType)
        {
            return (await hearing.GetContentsOfFieldTypeAndContentType(fieldSystemResolver, fieldType, ContentType.TEXT))
                .SingleOrDefault();
        }

        /// <summary>
        /// Gets the file contents of the specified field type.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the file contents from. The includes <c>"Contents"</c> and
        /// <c>"Contents.ContentType"</c> are required on the hearing.
        /// </param>
        /// <param name="fieldSystemResolver">Resolver for getting fields.</param>
        /// <param name="fieldType">The specified field type.</param>
        /// <returns>List of file contents.</returns>
        public static async Task<List<Content>> GetFileContentsOfFieldType(this Hearing hearing,
            IFieldSystemResolver fieldSystemResolver, FieldType fieldType)
        {
            return await hearing.GetContentsOfFieldTypeAndContentType(fieldSystemResolver, fieldType, ContentType.FILE);
        }
        
        /// <summary>
        /// Gets the hearing contents of the specified type.
        /// </summary>
        /// <param name="hearing">
        /// The hearing to get the file contents from. The includes <c>"Contents"</c> and
        /// <c>"Contents.ContentType"</c> are required on the hearing.
        /// </param>
        /// <param name="fieldSystemResolver">Resolver for getting fields.</param>
        /// <param name="fieldType">The specified field type.</param>
        /// <returns>List of file contents.</returns>
        public static async Task<List<Content>> GetContentsOfFieldType(this Hearing hearing,
            IFieldSystemResolver fieldSystemResolver, FieldType fieldType)
        {
            List<int> fieldIds = await fieldSystemResolver.GetFieldsIds(fieldType);
            return hearing.Contents
                .Where(content => content.FieldId.HasValue && fieldIds.Contains(content.FieldId.Value))
                .ToList();
        }

        private static async Task<List<Content>> GetContentsOfFieldTypeAndContentType(this Hearing hearing,
            IFieldSystemResolver fieldSystemResolver, FieldType fieldType, ContentType contentType)
        {
            return (await hearing.GetContentsOfFieldType(fieldSystemResolver, fieldType))
                .Where(content => content.ContentType.Type == contentType).ToList();
        }
    }
}