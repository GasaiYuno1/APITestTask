using APITestTask.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Ad Platform API", 
        Version = "v1",
        Description = "API for managing ad platforms and searching by location"
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register our service
builder.Services.AddSingleton<IAdPlatformService, AdPlatformService>();

var app = builder.Build();

// Enable Swagger in all environments (not just Development)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ad Platform API v1");
    c.RoutePrefix = string.Empty; // Makes Swagger UI available at root URL
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
