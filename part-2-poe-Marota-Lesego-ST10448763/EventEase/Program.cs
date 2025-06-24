using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Services;

var builder = WebApplication.CreateBuilder(args);

// Hardcoded connection string (Make sure this is safe in your environment)
var connectionString = "Server=tcp:eventeasebookingsv.database.windows.net,1433;Initial Catalog=EventEaseSystemDB;Persist Security Info=False;User ID=LesegoRMarota;Password=BengandRama*1738;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with EnableRetryOnFailure for transient fault handling
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Number of retries before failing
            maxRetryDelay: TimeSpan.FromSeconds(30), // Max delay between retries
            errorNumbersToAdd: null // Use default transient errors
        )
    )
);

// Register Azure Blob Storage service
builder.Services.AddScoped<IAzureBlobStorage, AzureBlobStorage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
