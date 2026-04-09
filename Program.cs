using Microsoft.EntityFrameworkCore;
using BudgetTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register AppDbContext with PostgreSQL.
// GetConnectionString looks for "DefaultConnection" inside "ConnectionStrings" in appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Fix for DateTime timezone error with PostgreSQL.
// By default, Npgsql requires DateTime values to have Kind=Utc when writing to
// 'timestamp with time zone' columns. This switch enables legacy behaviour so
// that DateTime values with Kind=Unspecified (the default in C#) are accepted.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
