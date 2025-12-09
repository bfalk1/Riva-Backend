using RivaAssessment.Middleware;
using RivaAssessment.Repositories;
using RivaAssessment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the legacy billing repository
builder.Services.AddSingleton<ILegacyBillingRepository, LegacyBillingRepository>();

// Register the credit service (you'll implement this)
// builder.Services.AddSingleton<ICreditService, CreditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Riva Assessment API v1");
        c.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
    });
}

// HTTPS redirection - disabled for development to allow Swagger on HTTP
// Uncomment the line below if you set up HTTPS certificate: dotnet dev-certs https --trust
// app.UseHttpsRedirection();

// Register the Credit Enforcement Middleware
// TODO: Uncomment and configure once you implement the middleware
// app.UseMiddleware<CreditEnforcementMiddleware>();

// Authorization - commented out since no authentication is configured
// Uncomment if you add authentication later
// app.UseAuthorization();

app.MapControllers();

app.Run();

