using Jwt.Common;
using Jwt.Contexts;
using Jwt.Dtos.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Usermanagement by JWT", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<JwtDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("JwtConStr")));


//fill configs from appsetting.json
//builder.Services.AddOptions();
builder.Services.AddOptions<SiteSetting>().Bind(builder.Configuration);
//builder.Services.Configure<SiteSetting>(options => builder.Configuration.Bind(options));

builder.Services.AddSingleton<IGenerateToken, GenerateToken>();

var sp = builder.Services.BuildServiceProvider();
var siteSetting = sp.GetService<IOptions<SiteSetting>>().Value;

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SaveToken = true;
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = siteSetting.JwtConfig.Issuer,//builder.Configuration["JwtConfigIssuer"],//"SampleJwtServer", 
        ValidAudience = siteSetting.JwtConfig.Audience,//builder.Configuration["JwtConfig:Audience"],//"SampleJwtClient", 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(siteSetting.JwtConfig.Key)),//(builder.Configuration["JwtConfig:Key"])), //("Top Secret Key 112358")), 
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
