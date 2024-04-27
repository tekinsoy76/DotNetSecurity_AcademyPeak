using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AcademyPeak_Web.Providers
{
    public class ApiProvider
    {
        private readonly Settings settings;
        private readonly IHttpContextAccessor contextAccessor;

        public ApiProvider(Settings settings, IHttpContextAccessor contextAccessor)
        {
            this.settings = settings;
            this.contextAccessor = contextAccessor;
        }

        private HttpClient getClient()
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(settings.ApiBaseAdres)
            };
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //Authorization
            string token = contextAccessor.HttpContext.Session.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            }

            return client;
        }

        public async Task<List<T>> GetServiceAsync<T>(string adres)
        {
            HttpClient httpClient = getClient();
            HttpResponseMessage response = await httpClient.GetAsync(adres);
            List<T> responseList = new List<T>();
            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                responseList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(jsonData);
            }
            return responseList;
        }


        public async Task<bool> LogInService(string adres, string username, string password)
        {
            HttpClient client = getClient();
            HttpResponseMessage response = await client.GetAsync($"{adres}/{username}/{password}");
            if (response.IsSuccessStatusCode)
            {
                string jsonToken = await response.Content.ReadAsStringAsync();
                string token = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(jsonToken);
                if (token != "FAILED")
                {
                    contextAccessor.HttpContext.Session.SetString("ApiToken", token);
                    return true;
                }
            }
            return false;
        }
    }
}
