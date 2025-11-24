using Microsoft.AspNetCore.Authorization;

namespace Projet_Dotnet_4.Requirements
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public MinimumAgeRequirement(int minimumAge) =>
            MinimumAge = minimumAge;

        public int MinimumAge { get;  }
    }
}
