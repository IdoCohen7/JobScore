using FluentValidation;
using FluentValidation.AspNetCore;
using JobScoreServer.Data;
using JobScoreServer.Hubs;
using JobScoreServer.Services;
using JobScoreServer.Services.Interfaces;
using JobScoreServer.Services.Rules;
using JobScoreServer.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero 
    };
});

builder.Services.AddAuthorization(options =>
{
    // admin policy
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
});

builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateJobDescriptionValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EvaluateJobDescriptionValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token in the format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IJobDescriptionService, JobDescriptionService>();
builder.Services.AddScoped<IBuzzwordService, BuzzwordService>();
builder.Services.AddScoped<IJobEvaluatorService, JobEvaluatorService>();
builder.Services.AddScoped<IMetricService, MetricService>();

// singletons for stateless rules
builder.Services.AddSingleton<IJobEvaluationRule, ReadingTimeRule>();
builder.Services.AddSingleton<IJobEvaluationRule, ContactEmailRule>();
builder.Services.AddSingleton<IJobEvaluationRule, StandardizationRule>();
builder.Services.AddSingleton<IJobEvaluationRule, ToneOfVoiceRule>();
builder.Services.AddSingleton<IJobEvaluationRule, TransparencyRule>();
builder.Services.AddSingleton<IJobEvaluationRule, AllCapsRule>();
builder.Services.AddSingleton<IJobEvaluationRule, ReadabilityRule>();
builder.Services.AddSingleton<IJobEvaluationRule, EngagementRule>();
builder.Services.AddSingleton<IJobEvaluationRule, InclusivityRule>();
builder.Services.AddScoped<IJobEvaluationRule, ProfessionalismRule>(); // Scoped due to IBuzzwordService dependency

builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSignalR();


var app = builder.Build();

app.MapHub<ChatHub>("/chathub");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ClientPermission");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

