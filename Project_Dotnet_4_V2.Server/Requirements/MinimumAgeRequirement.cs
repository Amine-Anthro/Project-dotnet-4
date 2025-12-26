using Microsoft.AspNetCore.Authorization;

namespace Project_Dotnet_4_V2.Requirements
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public MinimumAgeRequirement(int minimumAge) =>
            MinimumAge = minimumAge;

        public int MinimumAge { get;  }
    }
}
