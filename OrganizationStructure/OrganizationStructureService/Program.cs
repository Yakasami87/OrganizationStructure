global using OrganizationStructureService.Data.Models;
global using OrganizationStructureShared.Models;
global using OrganizationStructureShared.Models.DTOs;
global using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrganizationStructureService.Data.DataContexts;
using OrganizationStructureService.Services.PersonService;
using OrganizationStructureService.Services.RoleService;
using OrganizationStructureService.SignalIR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContext<OrgStrDataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("OrganizationStructureDBConnection")));

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IRoleService, RoleService>();

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
app.MapHub<MessageHub>("/messageHub");

app.Run();
