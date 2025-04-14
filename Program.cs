var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapRazorPages();  // Add this line
app.MapControllers(); // For your API controllers
app.Run();