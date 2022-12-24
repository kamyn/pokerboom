using Microsoft.AspNetCore.ResponseCompression;
using PokerBoom.Server.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using PokerBoom.Server.Data;
using Microsoft.EntityFrameworkCore;
using PokerBoom.Server.Entities;
using Microsoft.AspNetCore.Identity;
using PokerBoom.Server;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using PokerBoom.Server.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DBConnection"),
        new MySqlServerVersion(new Version(8, 0, 29))
)).
    AddIdentity<ApplicationUser, IdentityRole>(config =>    // change config
    {
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireUppercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequiredLength = 3; 
    }).
    AddRoles<IdentityRole>().
    AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.SECURITY_KEY)),
        ValidateIssuerSigningKey = true,
    };
}).
    AddOAuth("VK", "vkontakte", config =>
    {
        config.ClientId = AuthOptions.VK_CLIENT_ID;
        config.ClientSecret = AuthOptions.VK_CLIENT_SECRET;
        config.ClaimsIssuer = "vk";
        config.CallbackPath = new PathString(AuthOptions.VK_CALLBACK_PATH);
        config.AuthorizationEndpoint = AuthOptions.VK_AUTH_END_POINT;
        config.TokenEndpoint = AuthOptions.VK_TOKEN_END_POINT;
        config.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "VkId"); 
    });         

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, "Administrator");
    });
    options.AddPolicy("User", builder =>
    {
        builder.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "User") || x.User.HasClaim(ClaimTypes.Role, "Administrator"));
    });
});

builder.Services.AddScoped<ITableRepository, TableRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    DbInit.Init(scope.ServiceProvider);
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<GameHub>("/gamehub");
app.MapFallbackToFile("index.html");


app.Run();
