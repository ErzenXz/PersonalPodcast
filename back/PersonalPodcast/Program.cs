using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

string accessToken = configuration["AWS:AccessKey"];
string secret = configuration["AWS:SecretKey"];


var credentials = new Amazon.Runtime.BasicAWSCredentials(accessToken, secret);
var config = new AmazonS3Config
{
    RegionEndpoint = Amazon.RegionEndpoint.EUWest3
};

builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, config));

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
