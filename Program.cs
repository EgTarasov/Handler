using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
        {
            Version = "v1",
            Title = "Miraculous app which can do nothing",
            Description = "Provides useless functions",
            Contact = new OpenApiContact
            {
                Name = "Creator",
                Url = new Uri("https://s0.rbk.ru/v6_top_pics/media/img/1/83/756079611261831.jpg")
            },
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley")
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();