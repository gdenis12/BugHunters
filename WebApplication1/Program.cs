using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using WebApplication1;
using WebApplication1.endpoint;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolAppCon")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapParentEndpoints();
app.MapStudentEndpoints();
app.MapTeacherEndpoints();
app.MapGroupEndpoints();
app.MapEventTypeEndpoints();
app.MapEventEndpoints();

app.Run();