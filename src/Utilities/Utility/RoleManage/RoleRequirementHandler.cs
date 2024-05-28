using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Utility.RoleManage
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            IEnumerable<IAuthorizationRequirement> requirements = context.Requirements;

            // Role Check
            if (!context.User.HasClaim(x => x.Type == ClaimTypes.Role))
            {
                context.Fail(new AuthorizationFailureReason(this, "User token has no role"));
                return Task.CompletedTask;
            }

            var role = context.User.FindFirst(x => x.Type == ClaimTypes.Role)!.Value;

            string[] roles = role.Split(',');
            string expectedRole = requirement.Role;

            //if (roles != null && (roles.Contains("SuperAdmin")))
            //{
            //    context.Succeed(requirement);
            //    return Task.CompletedTask;
            //}

            string[] requireRoles = requirements.Where(y => y.GetType() == typeof(RoleRequirement)).Select(x => ((RoleRequirement)x).Role).ToArray();

            var isMatch = requireRoles.Any(x => roles.Any(y => x == y));

            if (!isMatch)
            {
                context.Fail(new AuthorizationFailureReason(this, "User token doesn't has the required role"));
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
