using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TibberRobotService.Data;
using TibberRobotService.Services;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRobotService, RobotService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler(a => a.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionFeature.Error;

        // Log the exception...

        var result = JsonConvert.SerializeObject(new { error = "An error occurred while processing your request." });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }));

app.Run();
