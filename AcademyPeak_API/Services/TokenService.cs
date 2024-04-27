using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AcademyPeak_API.Services
{
    public class TokenService
    {
        byte[] anahtar = Encoding.UTF8.GetBytes("Core Anahtar for webAPI");

        public async Task<string> ApiLogin(string username, string password)
        {
            // normal sartlar altindda bunu kendi olusturdugumuz veritabani yapisi veya Identity uzerinden dogruladigimizi varsayalim
            bool checkname = username == "user" && password == "P@ssW0rd";

            if (checkname)
            {
                string tckn = "44";
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.PrimarySid, tckn) }),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(anahtar), SecurityAlgorithms.HmacSha256)
                };
                SecurityToken tokenObject = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(tokenObject);
            }


            bool checkname2 = username == "user2" && password == "P@ssW0rd";

            if (checkname2)
            {
                string tckn = "88";
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.PrimarySid, tckn) }),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(anahtar), SecurityAlgorithms.HmacSha256)
                };
                SecurityToken tokenObject = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(tokenObject);
            }

            return "FAILED";
        }
    }
}
