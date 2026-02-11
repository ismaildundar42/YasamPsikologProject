using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using YasamPsikologProject.BussinessLayer.Abstract;
using YasamPsikologProject.BussinessLayer.Concrete;
using YasamPsikologProject.DataAccessLayer.Abstract;
using YasamPsikologProject.DataAccessLayer.Concerete;
using YasamPsikologProject.DataAccessLayer.EntityFramework;
using YasamPsikologProject.DataAccessLayer.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPsychologistRepository, PsychologistRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IWorkingHourRepository, WorkingHourRepository>();
builder.Services.AddScoped<IUnavailableTimeRepository, UnavailableTimeRepository>();
builder.Services.AddScoped<IAppointmentNotificationRepository, AppointmentNotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
builder.Services.AddScoped<IPsychologistArchiveRepository, PsychologistArchiveRepository>();

// Business Services
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IPsychologistService, PsychologistManager>();
builder.Services.AddScoped<IClientService, ClientManager>();
builder.Services.AddScoped<IAppointmentService, AppointmentManager>();
builder.Services.AddScoped<IWorkingHourService, WorkingHourManager>();
builder.Services.AddScoped<INotificationService, NotificationManager>();
builder.Services.AddScoped<IPsychologistArchiveService, PsychologistArchiveManager>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// CORS Policy - Frontend erişimi için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7223", "http://localhost:5110")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders();
app.UsePathBase("/portal");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
