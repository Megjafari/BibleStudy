using BibleStudy.Application.Interfaces;
using BibleStudy.Application.Services;
using BibleStudy.Infrastructure.Data;
using BibleStudy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BibleStudy.Infrastructure.Bible;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IReadingRepository, ReadingRepository>();

// Services
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IReadingService, ReadingService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<IBibleTextProvider, BibleApiTextProvider>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS: origins come from configuration (env vars in production)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply pending migrations on startup (Coolify runs no manual migration step).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();