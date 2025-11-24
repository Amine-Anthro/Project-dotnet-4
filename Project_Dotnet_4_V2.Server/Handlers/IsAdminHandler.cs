using Microsoft.AspNetCore.Authorization;
using Projet_Dotnet_4.Requirements;

namespace Projet_Dotnet_4.Handlers
{
    public class IsAdminHandler : AuthorizationHandler<IsAllowedToManageMovieRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            IsAllowedToManageMovieRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
