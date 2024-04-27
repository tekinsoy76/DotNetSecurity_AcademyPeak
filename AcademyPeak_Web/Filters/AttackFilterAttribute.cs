using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AcademyPeak_Web.Filters
{
    public class AttackFilterAttribute : Attribute, IActionFilter
    {
        private TimeSpan _timeSpan;
        private int maxReq;
        private int sec;
        public AttackFilterAttribute(int maxRequest, int seconds)
        {
            _timeSpan = TimeSpan.FromSeconds(seconds);
            maxReq = maxRequest;
            sec = seconds;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            string ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            string key = $"{ipAddress}-{context.ActionDescriptor.DisplayName}";

            int counter = context.HttpContext.Session.GetInt32("RequestCount") != null ? context.HttpContext.Session.GetInt32("RequestCount").Value : 0;
            string lastDate = string.IsNullOrEmpty(context.HttpContext.Session.GetString("FirstRequest")) ? DateTime.Now.ToLongTimeString() : context.HttpContext.Session.GetString("FirstRequest");

            if (counter == 0)
            {
                context.HttpContext.Session.SetString("FirstRequest", lastDate);
            }
            counter++;
            DateTime bugun = DateTime.Now;
            string[] zamanArray = lastDate.Split(':');
            DateTime ilkGiris = new DateTime(bugun.Year, bugun.Month, bugun.Day, Convert.ToInt32(zamanArray[0]), Convert.ToInt32(zamanArray[1]), Convert.ToInt32(zamanArray[2]));

            if (bugun.Subtract(ilkGiris).Seconds < sec && counter > maxReq)
            {
                context.Result = new ContentResult()
                {
                    Content = "Cok fazla request meydana geldi",
                    StatusCode = 429 //Http Status : too many requests
                };
                counter--;
                return;
            }
            else if(bugun.Subtract(ilkGiris).Seconds > sec)
            {
                counter = 0;
            }

            context.HttpContext.Session.SetInt32("RequestCount", counter);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
