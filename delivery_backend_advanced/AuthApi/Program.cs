using AuthApi.DAL;
using Common.Configurations;
using delivery_backend_advanced.Configurations;
using delivery_backend_advanced.Services.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthApiServices();

builder.ConfigureJwt();

builder.ConfigureAuthApiIdentity();

builder.ConfigureDeliveryApiDAL();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureDeliveryApiDAL();

app.UseHttpsRedirection();

app.UseExceptionMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();