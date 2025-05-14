using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.Services.Contracts.Auth;
using FleetM360_PLL.Services.Contracts.TermsConditions;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Implementation.Auth;
using FleetM360_PLL.Services.Implementation.TermsConditions;
using FleetM360_PLL.Services.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FleetM360_PLL;
using System.Globalization;
using FleetM360_PLL.Services;
using Microsoft.AspNetCore.Mvc;



var builder = WebApplication.CreateBuilder(args);
//var culture = new CultureInfo("en-US");
//CultureInfo.DefaultThreadCurrentCulture = culture;
//CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddDbContext<APPDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddDbContext<ApplicationDBContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register Identity services (for ASP.NET Identity)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<APPDBContext>()
    .AddDefaultTokenProviders();

// Register AutoMapper Manually
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
// Register IUserRepository and UserRepository
builder.Services.AddScoped<DbContext, ApplicationDBContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITripDriverService, TripDriverService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IPlannedTripLocationService, PlannedTripLocationService>();
builder.Services.AddScoped<ITruckService, TruckService>();
builder.Services.AddScoped<IJobSiteService, JobSiteService>();
builder.Services.AddScoped<ITruckSiloService, TruckSiloService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITermsConditionsService, TermsConditionsService>();
builder.Services.AddScoped<IPreCheckService, PreCheckService>();
builder.Services.AddScoped<ITripLogService, TripLogService>();
builder.Services.AddScoped<IRiskService, RiskService>();

// Dependency Injection
builder.Services.AddSingleton<InMemoryWebSocketStore>();
builder.Services.AddSingleton<WebSocketService>();
//builder.Services.ConfigAutoMapper();
// Add services to the container.
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Configuration.AddConfiguration(configuration);
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.Configure<MvcOptions>(options =>
{
    options.AllowEmptyInputInBodyModelBinding = true;
});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidateAudience = true,
//        ValidAudience = jwtSettings["Audience"],
//        ValidateLifetime = true,
//        ClockSkew = TimeSpan.Zero
//    };
//});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
builder.Services.AddAuthentication();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
        };

        // Add logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Auth failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated!");
                return Task.CompletedTask;
            }
        };
    });

//var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
//var jwtKey = builder.Configuration.GetSection("Jwt:Secret").Get<string>();
//builder.Services.AddAuthentication();
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(authenticationScheme: JwtBearerDefaults.AuthenticationScheme,
//   options =>
//   {
//       options.TokenValidationParameters = new TokenValidationParameters
//       {
//           ValidateIssuer = true,
//           ValidateAudience = true,
//           ValidateLifetime = true,
//           ValidateIssuerSigningKey = true,
//           ValidIssuer = jwtIssuer,
//           ValidAudience = jwtIssuer,
//           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//           ClockSkew = TimeSpan.Zero
//       };
//   });

builder.Services.AddAuthorization();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()   // Allows requests from any origin
              .AllowAnyMethod()   // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
              .AllowAnyHeader();  // Allows any headers
    });
});


builder.Services.AddHttpClient("AllowAnySSL", client =>
{
    client.BaseAddress = new Uri("https://example.com");
})
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        return handler;
    });

var app = builder.Build();

app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var userNumber = context.Request.Query["userNumber"];
            var truckId = context.Request.Query["truckId"];
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var service = context.RequestServices.GetRequiredService<WebSocketService>();
            await service.HandleConnectionAsync(Convert.ToInt32(userNumber!), truckId!, webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
    //Console.WriteLine($"Request path: {context.Request.Path}");
    //Console.WriteLine($"Is WebSocketRequest: {context.WebSockets.IsWebSocketRequest}");
    //if (context.Request.Path == "/FLMWebApi/ws")
    //{
    //    if (context.WebSockets.IsWebSocketRequest)
    //    {
    //        var userNumber = context.Request.Query["userNumber"];
    //        var truckId = context.Request.Query["truckId"];
    //        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    //        var service = context.RequestServices.GetRequiredService<WebSocketService>();
    //        await service.HandleConnectionAsync(Convert.ToInt32(userNumber!), truckId!, webSocket);
    //    }
    //    else
    //    {
    //        Console.WriteLine("Request was NOT a WebSocket request.");
    //        context.Response.StatusCode = 400;
    //        await context.Response.WriteAsync("Expected WebSocket request.");
    //        //context.Response.StatusCode = 400;
    //    }
    //}
    //else
    //{
    //    await next();
    //}
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
