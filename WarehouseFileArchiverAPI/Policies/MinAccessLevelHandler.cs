using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WarehouseFileArchiverAPI.Policies
{
    public class MinAccessLevelHandler : AuthorizationHandler<MinimumAccessLevel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAccessLevel requirement)
        {
            var accessLevel = context.User.FindFirst("AccessLevel")?.Value;
            var role = context.User.FindFirst("role")?.Value;

            
            if (role != null && role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

        
            var levels = new List<string> { "Read-only", "Write", "Admin" }; 

            int userLevel = levels.IndexOf(accessLevel);
            int requiredLevel = levels.IndexOf(requirement.MinAccessLevel);

            if (userLevel >= requiredLevel && userLevel != -1)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}