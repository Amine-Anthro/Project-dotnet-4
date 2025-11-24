using Microsoft.AspNetCore.Authorization;
using Projet_Dotnet_4.Requirements;

namespace Projet_Dotnet_4.Handlers
{
    public class IsITMemberHandler : AuthorizationHandler<IsAllowedToManageMovieRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            IsAllowedToManageMovieRequirement requirement)
        {
            if (context.User.HasClaim(f => f.Type == "IT"))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
