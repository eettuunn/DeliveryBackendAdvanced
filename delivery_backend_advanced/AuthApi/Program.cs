using AuthApi.DAL;
using Common.Configurations;
using delivery_backend_advanced.Configurations;
using delivery_backend_advanced.Services.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthApiServices();

builder.ConfigureSwagger();

builder.ConfigureJwt();

builder.ConfigureIdentity();

builder.ConfigureAuthDAL();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureAuthDAL();

app.UseHttpsRedirection();

app.UseLoggerMiddleware();

app.UseExceptionMiddleware();

app.UseAuthentication();

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();