using Microsoft.EntityFrameworkCore;
using RaceMS_Repositories.NguyenDNT.DBContext;
using RaceMS_Services.NguyenDNT;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJockeyNguyenDntService, JockeyNguyenDntService>();
builder.Services.AddScoped<IRegistrationNguyenDntService, RegistrationNguyenDntService>();

builder.Services.AddDbContext<RaceManagementDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data")));


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
