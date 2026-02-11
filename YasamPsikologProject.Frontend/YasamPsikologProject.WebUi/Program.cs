using Serilog;
using Microsoft.AspNetCore.HttpOverrides;
using YasamPsikologProject.WebUi.Services;
using YasamPsikologProject.WebUi.Filters;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/yasampsikolog-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("SessionTimeout"));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Filters
builder.Services.AddScoped<PsychologistAuthorizationFilter>();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

// HTTP Client Services - API Consume (WITHOUT POLLY - causes handler disposal issues)
builder.Services.AddHttpClient<IApiAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiPsychologistService, PsychologistService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiClientService, ClientService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiAppointmentService, AppointmentService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiWorkingHourService, WorkingHourService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiUserService, UserService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiSuperAdminService, SuperAdminService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiUnavailableTimeService, UnavailableTimeService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IApiSystemSettingService, SystemSettingService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

