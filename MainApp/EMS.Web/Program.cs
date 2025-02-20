using EMS.Domain.Identity;
using EMS.Domain.Models;
using EMS.Repository;
using EMS.Repository.Implementation;
using EMS.Repository.Interface;
using EMS.Service;
using EMS.Service.Implementation;
using EMS.Service.Interface;
using EMS.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Database and Identity setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<IdentityDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<EMSApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityDataContext>().AddDefaultTokenProviders(); ;

builder.Services.AddDefaultIdentity<EMSApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddRazorPages();

// Stripe settings
StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];

// Dependency Injection for Services and Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<ITicketService, TicketService>();
builder.Services.AddTransient<IEventService, EMS.Service.Implementation.EventService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<ITicketInEventService, TicketInEventService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<ITicketInOrderService, TicketInOrderService>();
builder.Services.AddTransient<ICalendarService, CalendarService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();


// Configure Stripe Settings from appsettings.json
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false; // Disable email confirmation
});



var app = builder.Build();

// Middleware and routing setup
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
