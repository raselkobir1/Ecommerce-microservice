using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Utility.RoleManage
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                context.Fail(new AuthorizationFailureReason(this, "User token has no DateOfBirth"));
                return Task.CompletedTask;
            }

            var dateOfBirth = Convert.ToDateTime(context?.User?.FindFirst(c => c.Type == ClaimTypes.DateOfBirth)?.Value);
            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
                context?.Succeed(requirement);
            else
                context?.Fail(new AuthorizationFailureReason(this, "Must be grater then  user DateOfBirth 18 or equal"));

            return Task.CompletedTask;
        }
    }
}
