using Microsoft.AspNetCore.Authorization;

namespace Asp.netCore_MVC.Security
{
    public class SuperAdminHandler: AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        /// <summary>
        /// Custom Authorization Policy For Super Admin To Edit Role
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            if(context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
