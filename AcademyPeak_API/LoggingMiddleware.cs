namespace AcademyPeak_API
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger<LoggingMiddleware> logger;

        public LoggingMiddleware(RequestDelegate requestDelegate, ILogger<LoggingMiddleware> logger)
        {
            this.requestDelegate = requestDelegate;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // belirli durumlarda mail atilmasi
            // veritabanin yazilmasi
            await requestDelegate(context);
        }
    }
}
