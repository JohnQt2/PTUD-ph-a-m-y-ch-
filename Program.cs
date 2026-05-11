using MaterialApi.Services.Implementations;
using MaterialApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DI Configuration: Đổi thành QuerySyntaxMaterialService để test cú pháp Query
builder.Services.AddScoped<IMaterialService, QuerySyntaxMaterialService>();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global Exception Handling Middleware
app.UseMiddleware<MaterialApi.Middlewares.ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
