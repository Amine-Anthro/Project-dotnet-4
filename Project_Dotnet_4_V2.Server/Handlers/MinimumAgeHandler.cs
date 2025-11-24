using Microsoft.AspNetCore.Authorization;
using Projet_Dotnet_4.Requirements;

namespace Projet_Dotnet_4.Handlers
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            MinimumAgeRequirement requirement)
        {
            if(!context.User.HasClaim(c => c.Type == "Age" ))
                return Task.CompletedTask;

            var age = int.Parse(
                context.User.Claims.First(c => c.Type == "Age").Value
                );

            if (age >= requirement.MinimumAge)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
