using System.Collections.Concurrent;

namespace AcademyPeak_API
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate request;
        private static ConcurrentDictionary<string, (DateTime, int)> requestDictionary = new ConcurrentDictionary<string, (DateTime, int)>();

        public RateLimitMiddleware(RequestDelegate request)
        {
            this.request = request;
        }

        public async Task Invoke(HttpContext context)
        {
            string ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            DateTime simdi = DateTime.Now;

            requestDictionary.AddOrUpdate(ip, (simdi, 1), (k, v) =>
            {
                if (simdi.Subtract(v.Item1).TotalSeconds > 60)
                {
                    return (simdi, 1);
                }
                else
                {
                    return (v.Item1, v.Item2 + 1);
                }
            });

            if (requestDictionary[ip].Item2 > 1000)// burada limitimizi 1000 request/60sn. olarak belirledik
            {
                context.Response.StatusCode = 429; //too many request
                await context.Response.WriteAsync("Request limiti asildi. Daha sonra tekrar deneyiniz.");
                return;
            }

            await request(context);
        }
    }
}
