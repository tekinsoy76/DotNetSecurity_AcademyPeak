using AcademyPeak_API;
using AcademyPeak_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AcademyPeak_API.Services.TokenService>();
byte[] anahtar = Encoding.UTF8.GetBytes("Core Anahtar for webAPI");
builder.Services.AddAuthentication(secenekler =>
{
    secenekler.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    secenekler.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(secenekler =>
    {
        secenekler.RequireHttpsMetadata = true;
        secenekler.SaveToken = true;
        secenekler.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(anahtar),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        secenekler.Events = new JwtBearerEvents()
        {
            OnAuthenticationFailed = c =>
            {
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(secenekler =>
{
    secenekler.AddPolicy("Yetkili", policy => policy.Requirements.Add(new WeatherRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, WeatherHandler>();

builder.Services.Configure<BlockedRequestsOptions>(builder.Configuration.GetSection("BlockedRequests"));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<FirewallMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandling>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

app.Run();
