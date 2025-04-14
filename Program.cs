using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using FantasySports.Data;
using System;
using FantasySports.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development // Force development environment
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// Development configuration
if (builder.Environment.IsDevelopment())
{
    // Enable runtime compilation for Hot Reload
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}

// Add TeamService registration
builder.Services.AddScoped<TeamService>();

// Use in-memory stores
builder.Services.AddTransient<IUserStore<IdentityUser>, InMemoryUserStore>();
builder.Services.AddSingleton<InMemoryUserContext>();

// Add in-memory Identity
builder.Services.AddIdentityCore<IdentityUser>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
})
.AddSignInManager<SignInManager<IdentityUser>>()
.AddDefaultTokenProviders();

// Add auth services
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
}).AddIdentityCookies();

// Add authorization
builder.Services.AddAuthorization();

// Add MVC services
builder.Services.AddRazorPages();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // app.UseBrowserLink(); // Add this line
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();  // Add this line
app.MapControllers(); // For your API controllers

app.Run();