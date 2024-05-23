using PathFinder_Plus.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GreedyService>();
builder.Services.AddScoped<BacktrackingService>();

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
