using PaymentGateway.Api.Bootstrap;
using PaymentGateway.Processor.Configuration;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Service").Get<ServiceConfig>();
builder.Services.AddSingleton(settings);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// bootstrap
builder.Services.AddPaymentServices();

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

public partial class Program
{
}