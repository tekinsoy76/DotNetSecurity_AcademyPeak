using Microsoft.AspNetCore.Authorization;

namespace AcademyPeak_API
{
    public class WeatherHandler : AuthorizationHandler<WeatherRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WeatherRequirement requirement)
        {
            string tckn = context.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid")?.Value;

            if (!string.IsNullOrWhiteSpace(tckn) && tckn == "44")
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
