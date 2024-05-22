using PathFinder_Plus.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<Greedy>();
builder.Services.AddScoped<Backtracking>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddCors(options => options.AddPolicy(name: "DevOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:3000");
        builder.WithHeaders("Content-Type");
    })
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
