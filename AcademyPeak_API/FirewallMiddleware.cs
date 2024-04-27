using Microsoft.Extensions.Options;
using System.Net;

namespace AcademyPeak_API
{
    public class FirewallMiddleware
    {
        private readonly RequestDelegate request;
        private readonly BlockedRequestsOptions options;

        public FirewallMiddleware(RequestDelegate request, IOptions<BlockedRequestsOptions> options)
        {
            this.request = request;
            this.options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value;
            string ip = context.Connection.RemoteIpAddress.ToString();

            if (options.Paths.Contains(path) || options.IPs.Contains(ip))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Izinsiz giris");
                return;
            }

            await request(context);
        }
    }
}
