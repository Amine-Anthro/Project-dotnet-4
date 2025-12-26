using Microsoft.AspNetCore.Authorization;
using Project_Dotnet_4_V2.Requirements;

namespace Project_Dotnet_4_V2.Handlers
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
