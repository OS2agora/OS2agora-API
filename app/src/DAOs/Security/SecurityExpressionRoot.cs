using BallerupKommune.Operations.Common.Enums;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Resolvers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NovaSec.Compiler;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using SecurityConstants = BallerupKommune.Operations.Common.Constants.Security;

namespace BallerupKommune.DAOs.Security
{
    public class SecurityExpressionRoot : ISecurityExpressionRoot
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IUserHearingRoleResolver _userHearingRoleResolver;
        private readonly ICompanyHearingRoleResolver _companyHearingResolver;

        public SecurityExpressionRoot(ICurrentUserService currentUserService, IIdentityService identityService, IHttpContextAccessor httpContextAccessor, 
            IUserHearingRoleResolver userHearingRoleResolver, ICompanyHearingRoleResolver companyHearingResolver)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _userHearingRoleResolver = userHearingRoleResolver;
            _companyHearingResolver = companyHearingResolver;
        }

        public bool HasRole(string role)
        {
            switch (role)
            {
                // Idp roles
                case SecurityConstants.Roles.Administrator:
                case SecurityConstants.Roles.HearingCreator:
                    var userRoles = _httpContextAccessor.HttpContext.User.FindAll(ClaimTypes.Role);
                    return userRoles.Any(claim => claim.Value == role);
                // Hearing contextual roles
                case SecurityConstants.Roles.HearingOwner:
                    return _userHearingRoleResolver.IsHearingOwner().Result;
                case SecurityConstants.Roles.HearingResponder:
                    return _userHearingRoleResolver.IsHearingResponder().Result || _companyHearingResolver.IsHearingResponder().Result;
                case SecurityConstants.Roles.HearingInvitee:
                    return _userHearingRoleResolver.IsHearingInvitee().Result || _companyHearingResolver.IsHearingInvitee().Result;
                case SecurityConstants.Roles.HearingReviewer:
                    return _userHearingRoleResolver.IsHearingReviewer().Result;
                // Login contextual roles
                case SecurityConstants.Roles.Anonymous:
                    return !_currentUserService.AuthenticationMethod.HasValue;
                case SecurityConstants.Roles.Citizen:
                    var currentAuthenticationMethod = _currentUserService.AuthenticationMethod;
                    return currentAuthenticationMethod == AuthenticationMethod.MitIdCitizen ||
                           currentAuthenticationMethod == AuthenticationMethod.MitIdErhverv;
                case SecurityConstants.Roles.Employee:
                    return _currentUserService.AuthenticationMethod == AuthenticationMethod.AdfsEmployee;
                default:
                    throw new Exception("The requested security role does not exist");
            }
        }

        public bool HasAllRoles(List<string> roles)
        {
            return roles.All(HasRole);
        }

        public bool HasAnyRole(List<string> roles)
        {
            return roles.Any(HasRole);
        }

        private HearingRole MapSecurityRoleToHearingRole(string securityRole)
        {
            switch (securityRole)
            {
                case SecurityConstants.Roles.HearingOwner:
                    return HearingRole.HEARING_OWNER;
                case SecurityConstants.Roles.HearingResponder:
                    return HearingRole.HEARING_RESPONDER;
                case SecurityConstants.Roles.HearingInvitee:
                    return HearingRole.HEARING_INVITEE;
                case SecurityConstants.Roles.HearingReviewer:
                    return HearingRole.HEARING_REVIEWER;
                default:
                    return HearingRole.NONE;
            }
        }
    }
}