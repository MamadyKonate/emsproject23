using emsproject23.Controllers;
using emsproject23.Data;
using emsproject23.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(); //for hot reloading

var connectionString = builder.Configuration.GetConnectionString("EMSDbContext");
builder.Services.AddDbContext<EMSDbContext>();

//Configuring Session 
//Then add app.UseSession() - see below
//Then leverage Session in controller - using dependancy injection
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    //options.Cookie.HttpOnly = true;
    //options.Cookie.IsEssential = true;
});

//for making the currently logged in user available to all controllers and views
builder.Services.AddSingleton<CurrentUser2>();


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
