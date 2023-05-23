using System.Reflection;
using AUserRegister.Features;
using AUserRegister.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// var config = new ConfigurationBuilder()
//     .AddJsonFile("appsettings.json", false, true)
//     .AddJsonFile("appsettings.local.json", false, true)
//     .AddEnvironmentVariables()
//     .Build();

// var connectionStrings = config.GetSection("ConnectionStrings").Get<ConnectionStrings>();

// builder.Services.AddSingleton(connectionStrings);


builder.Services.AddControllers();

// builder.Services.AddDbContextFactory<DataContext>(options => 
//     options.UseNpgsql(connectionStrings.DefaultConnection)
// );

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add necessary classes to container
//builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Register validators as an assembly
builder.Services.AddControllers().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();