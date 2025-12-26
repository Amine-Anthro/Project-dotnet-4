using Microsoft.AspNetCore.Authorization;
using Project_Dotnet_4_V2.Requirements;

namespace Project_Dotnet_4_V2.Handlers
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
