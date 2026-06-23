using BibleStudy.Application.Interfaces;
using BibleStudy.Application.Services;
using BibleStudy.Infrastructure.Data;
using BibleStudy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BibleStudy.Infrastructure.Bible;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Services
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IReadingRepository, ReadingRepository>();
builder.Services.AddScoped<IReadingService, ReadingService>();


// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHttpClient<IBibleTextProvider, BibleApiTextProvider>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();