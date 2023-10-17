using Microsoft.EntityFrameworkCore;
using PrsCSharpServer.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connStrKey = "ProdDb";
#if DOCKER
    connStrKey = "DockerDb";
#elif DEBUG
    connStrKey = "DevDb";
#endif
builder.Services.AddDbContext<PrsDbContext>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString(connStrKey))
);

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
