using Microsoft.AspNetCore.Authorization;
using Projet_Dotnet_4.Requirements;

namespace Projet_Dotnet_4.Handlers
{
    public class IsAccountNotDisabledHandler : AuthorizationHandler<IsAccountEnabledRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            IsAccountEnabledRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "Disabled"))
                context.Fail();
            else
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
