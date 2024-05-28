using Microsoft.AspNetCore.Authorization;

namespace Utility.RoleManage
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(string role) => Role = role;

        public string Role { get; }
    }
}
