using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services; // Include this to access IAzureBlobStorageService

var builder = WebApplication.CreateBuilder(args);

// Hardcoded connection string 
var connectionString = "Server=tcp:eventeasebooking.database.windows.net,1433;Initial Catalog=EventEaseBookingDB;Persist Security Info=False;User ID=LesegoMarota;Password=BengandRama*1738;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Azure Blob Storage service
builder.Services.AddScoped<IAzureBlobStorage, AzureBlobStorage>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
