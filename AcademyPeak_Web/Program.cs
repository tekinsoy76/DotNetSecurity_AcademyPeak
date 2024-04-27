using AcademyPeak_Web.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

AcademyPeak_Web.Settings settings = new AcademyPeak_Web.Settings();
builder.Configuration.Bind("PortalSettings", settings);
builder.Services.AddSingleton(settings);

// Add services to the container.
AcademyPeak_Cryptography.Enigma enigma = new AcademyPeak_Cryptography.Enigma();
var connectionString =  enigma.SymmetricDecryptor<AesCryptoServiceProvider>(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<AcademyPeak_Web.Providers.ApiProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(secenekler =>
    {
        secenekler.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            ValidIssuer = "https://localhost:7188/",
            ValidAudience = "https://localhost:7188/",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Core Anahtar for web"))
        };
    });

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

//builder.Services.Configure<CookiePolicyOptions>(secenekler =>
//{
//    secenekler.CheckConsentNeeded = context => true; // cookie onayi gerektirir
//    secenekler.MinimumSameSitePolicy = SameSiteMode.None; // Cross-site requestler icin
//    secenekler.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always; // Cookie lerin HttpOnly olarak olusturulmasi
//    secenekler.Secure = CookieSecurePolicy.Always; // Yalnizca HTTPS
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
// Asagidaki kod oregi css klasorunu de yasakladigindan wwwroot klasorunun tamamina erisim icin degistirildi:
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(app.Environment.WebRootPath),//Path.Combine(app.Environment.WebRootPath, "Uploads")), 
    RequestPath = "",// "/Uploads",
    ServeUnknownFileTypes = false, // bilinmeyen dosya turleri acilmasin
    OnPrepareResponse = ctx =>
    {
        if (!ctx.Context.User.Identity.IsAuthenticated || !ctx.Context.User.IsInRole("Admin"))
        {
            //ctx.Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}) ;

app.UseRouting();

//app.UseCookiePolicy();

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["AuthToken"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", "Bearer " + token);
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
