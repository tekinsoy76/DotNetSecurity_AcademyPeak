namespace AcademyPeak_API
{
    public class ExceptionHandling
    {
        private readonly RequestDelegate request;

        public ExceptionHandling(RequestDelegate request)
        {
            this.request = request;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await request(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            // buraya gelen Exception ile ilgili mail atilabilir, loglama yapilabilir, response belli bir sayfaya yonlendirilebilir

            return httpContext.Response.WriteAsync("Hata oldu");
        }
    }
}
