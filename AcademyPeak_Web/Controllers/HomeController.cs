using AcademyPeak_Web.Models;
using AcademyPeak_Web.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using AcademyPeak_Cryptography;

namespace AcademyPeak_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Settings settings;
        private readonly ApiProvider api;

        public HomeController(ILogger<HomeController> logger, Settings settings, ApiProvider api)
        {
            _logger = logger;
            this.settings = settings;
            this.api = api;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            bool sonuc = TestAuthenticator("user", "P@ssW0rd");
            if (sonuc)
            {
                return RedirectToAction(nameof(Index));
            }
            return Forbid();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Hash algoritmaya ornek
            
            AcademyPeak_Cryptography.Enigma enigma = new AcademyPeak_Cryptography.Enigma();
            string ilkKAydedilenSifre = enigma.HashEncryptor<SHA256CryptoServiceProvider>("n/EmKRJ/kMIe+GGIJoSPdrzARplp7IfmwG2o73p3pkk=");
            // veritabanina kaydettim

            string sonradanGirilenSifre = enigma.HashEncryptor<SHA256CryptoServiceProvider>("sifre1234");
            if (ilkKAydedilenSifre == sonradanGirilenSifre)
            {
                //dogru sifre
            }

            //string dahaKuvvetli = enigma.HashEncryptor("sifre1234", "emre");

            string encryptedVeri = enigma.SymmetricEncryptor<AesCryptoServiceProvider>("Academy Peak");
            string decryptedVeri = enigma.SymmetricDecryptor<AesCryptoServiceProvider>(encryptedVeri);


            // Normal sartlarda bu hareketi bir login ekrani yapmali:
            if(await api.LogInService("login", "user", "P@ssW0rd"))
            {
                //normal giris yapildiginda
                List<WeatherModel> model = await api.GetServiceAsync<WeatherModel>(settings.ProductAdres);
                return View(model);
            }

            ViewBag.Error = "Giris Yapilamadi";
            return View(new List<WeatherModel>());
        }

        [NonAction]
        private bool TestAuthenticator(string username, string password)
        {
            if (username == "user" && password == "P@ssW0rd")
            {

                Claim[] claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username)
                };

                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Core Anahtar for web"));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new JwtSecurityToken(
                issuer: "https://localhost:7188/",
                audience: "https://localhost:7188/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                Response.Cookies.Append("AuthToken", tokenString, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict });
                return true;
            }
            return false;
        }

        public IActionResult Crypto()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crypto(CryptoModel model)
        {
            Enigma enigma = new Enigma();
            ViewBag.Sonuc = enigma.SymmetricEncryptor<AesCryptoServiceProvider>(model.Word);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            var file = model.File;

            if (file == null || file.Length == 0)
            {
                return Content("Bos dosya");
            }

            string[] izinVerilenUzantilar = new string[] { ".jpg", ".png" };
            string[] izinVerilenTipler = new string[] { "image/jpg", "image/png" };

            string uzanti = Path.GetExtension(model.FileName).ToLowerInvariant();

            if (!izinVerilenUzantilar.Contains(uzanti))
            {
                return Content("Hatali dosya tipi");
            }

            return Ok();
        }
    
        
        public IActionResult UserProfilePage(int id)
        {
            // id ye gore kullanicinin bulunmasi ve ekrana bilgilerinin gonderilmesi
            // /Home/UserProfilePage/1 : burada 1 asil kullanicinin id'si iken;
            // /Home/UserProfilePAge/10 : yazar ise sistemin 10 numarali kullanici bilgilerini getirme tehlikesi
            return View();
        }

        // Bunun yerine
        public IActionResult UserProfile()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Burada id yi parametre olarak almaktansa; guvenli kaynaktan elde etmis olduk
            return View();
        }
    }

    public class FileUploadModel
    {
        public string File { get; set; }
        public string FileName { get; set; }
    }

}