using core_backend.Infrastructures.Service;
using core_backend.Data;
using Microsoft.EntityFrameworkCore;
using CommonCore.Utils.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureControllerWithNewtonsoftJson();
builder.Services.ConfigureInvalidModelState();
builder.Services.ConfigureCorsDomain(builder.Configuration, builder.Environment);
builder.Services.ConfigureAuthService(builder.Configuration);
builder.Services.ConfigurePostgresqlDatabase(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureSwaggerService();
builder.Services.ConfigureSignalR();
builder.Services.ConfigureAutoMapper();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        AppLogger.Instance.Info("Applying database migrations...");
        // Apply pending migrations automatically
        await dbContext.Database.MigrateAsync();
        AppLogger.Instance.Debug("✓ Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        AppLogger.Instance.Info($"✗ Error applying migrations: {ex.Message}");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
