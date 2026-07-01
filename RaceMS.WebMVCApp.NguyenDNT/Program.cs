using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RaceMS.WebMVCApp.NguyenDNT.Hubs;
using RaceMS_Repositories.NguyenDNT;
using RaceMS_Repositories.NguyenDNT.DBContext;
using RaceMS_Services.NguyenDNT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<RaceManagementDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data")));

builder.Services.AddScoped<JockeyNguyenDntRepository>();
builder.Services.AddScoped<IJockeyNguyenDntService, JockeyNguyenDntService>();

builder.Services.AddScoped<IRegistrationNguyenDntService, RegistrationNguyenDntService>();

builder.Services.AddScoped<SystemUserAccountRepository>();
builder.Services.AddScoped<ISystemUserAccountService, SystemUserAccountService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = new PathString("/Account/Login");
        options.AccessDeniedPath = new PathString("/Account/Forbidden");
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapHub<ChatHub>("/chatHub");
app.MapHub<MSRaceHub>("/msRaceHub");

app.Run();
