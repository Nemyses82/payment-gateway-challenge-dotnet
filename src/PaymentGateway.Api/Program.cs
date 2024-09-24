using FluentValidation;
using FluentValidation.AspNetCore;

using PaymentGateway.Api.Bootstrap;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;
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

// Validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();

var app = builder.Build();

// Exception middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

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