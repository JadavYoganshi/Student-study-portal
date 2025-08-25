using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using SSP.Data;
using SSP.Models.Domain;
using SSP.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);

// ✅ Add services to the container
builder.Services.AddControllersWithViews();

// ✅ Register DbContext
builder.Services.AddDbContext<StudyPortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudyPortalDbConnectionString")));

// ✅ Register password hasher services
builder.Services.AddSingleton<IPasswordHasher<Admin>, PasswordHasher<Admin>>();
builder.Services.AddSingleton<IPasswordHasher<Student>, PasswordHasher<Student>>();

// ✅ Enable distributed memory cache (required for session)
builder.Services.AddDistributedMemoryCache();

// ✅ Enable session with timeout
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Required for session and user access in views/layout
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// ✅ Configure authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// ✅ Optional: Add policy (not strictly required now)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentPolicy", policy => policy.RequireClaim(System.Security.Claims.ClaimTypes.Email));
});

// ✅ Add the custom session management and YouTube video preference tracking
builder.Services.AddScoped<IYouTubeService, YouTubeService>();
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
var app = builder.Build();

// ✅ Auto-create or update Admin if not exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StudyPortalDbContext>();
    var adminPasswordHasher = services.GetRequiredService<IPasswordHasher<Admin>>();

    try
    {
        // Apply pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        // Find or create Admin
        var admin = context.Admins.FirstOrDefault(a => a.A_Email == "admin@example.com");

        if (admin == null)
        {
            // Create a new admin
            admin = new Admin
            {
                A_Id = Guid.NewGuid(),
                A_Name = "Admin",
                A_Email = "admin@example.com",
                A_Password = adminPasswordHasher.HashPassword(null, "admin123")
            };

            context.Admins.Add(admin);
        }
        else
        {
            // Update admin password if it's not hashed correctly
            if (admin.A_Password.Length < 50)
            {
                admin.A_Password = adminPasswordHasher.HashPassword(null, "admin123");
                context.Admins.Update(admin);
            }
        }

        context.SaveChanges();
        Console.WriteLine("✅ Default Admin Created or Updated Successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during database initialization: {ex.Message}");
    }
}

// ✅ Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Auth before session
app.UseAuthentication();
app.UseAuthorization();

// ✅ Enable session
app.UseSession();

// ✅ Default route — system opens at Home/Index
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
