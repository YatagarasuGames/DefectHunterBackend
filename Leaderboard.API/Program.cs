using Leaderboard.API.Abstractions;
using Leaderboard.API.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Database.Abstractions;
using Shared.Database.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IPlayerScoreRepository, PlayerScoreRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)));
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity", policy =>
    {
        policy.WithOrigins(
                "http://localhost",
                "http://127.0.0.1",
                "http://192.168.56.1", // ваш IP
                "http://192.168.0.107" // ваш IP
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowUnity");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
