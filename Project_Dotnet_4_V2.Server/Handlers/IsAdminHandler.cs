using Microsoft.AspNetCore.Authorization;
using Project_Dotnet_4_V2.Requirements;

namespace Project_Dotnet_4_V2.Handlers
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
