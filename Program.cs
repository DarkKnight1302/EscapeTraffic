using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Radzen;
using TrafficEscape.Areas.Identity;
using TrafficEscape.Data;
using TrafficEscape.Services;
using TrafficEscape.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DefaultConnection");
var password = builder.Configuration.GetValue<string>("COLLEAGUE_CASTLE_EMAIL_PASSWORD") ?? Environment.GetEnvironmentVariable("COLLEAGUE_CASTLE_EMAIL_PASSWORD");
builder.Services.AddFluentEmail("admin@colleaguecastle.in")
                .AddMailKitSender(new SmtpClientOptions
                {
                    Server = "smtp.gmail.com",
                    Port = 587,
                    Password = password,
                    UseSsl = false,
                    User = "Test"
                });
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = builder.Configuration.GetValue<string>("Authentication_Microsoft_ClientId") ?? Environment.GetEnvironmentVariable("Authentication_Microsoft_ClientId");
    microsoftOptions.ClientSecret = builder.Configuration.GetValue<string>("Authentication_Microsoft_ClientSecret") ?? Environment.GetEnvironmentVariable("Authentication_Microsoft_ClientSecret");
});
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<ISecretService, SecretService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IGooglePlaceService, GooglePlaceService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRadzenComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
