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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FleetM360_PLL;
using FleetM360_Web.hub;
using System.Globalization;



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

// Add SignalR service

builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:44330")); // Change to your frontend URL
app.MapHub<TruckHub>("/truckHub");
app.MapHub<ChatHub>("/chatHub");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
