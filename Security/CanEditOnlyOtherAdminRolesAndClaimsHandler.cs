using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Asp.netCore_MVC.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler:
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        /// <summary>
        /// Policy For Not To Edit By Myself Login user
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            /** Get Login Admin Id Using Handler Context */
            string loggedInAdminId =
                context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Console.WriteLine(loggedInAdminId);

            /** Get Request User Id */
            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            /** Check User Has Permission or not */
            if(context.User.IsInRole("Admin") && 
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                adminIdBeingEdited.ToLower() == loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
