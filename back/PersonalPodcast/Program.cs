using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var credentials = new Amazon.Runtime.BasicAWSCredentials("AKIAWCTVPQMQSJWH72N7", "scdRdSS/eEdef2RXJ4NLZMSTpa1FLp4WltQrqI0e");
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
