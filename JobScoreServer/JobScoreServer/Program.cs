using FluentValidation;
using JobScoreServer.Data;
using JobScoreServer.Services;
using JobScoreServer.Services.Interfaces;
using JobScoreServer.Services.Rules;
using JobScoreServer.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // name of cookie in login
            context.Token = context.Request.Cookies["jwt_token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    // admin policy
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IJobDescriptionService, JobDescriptionService>();
builder.Services.AddScoped<IBuzzwordService, BuzzwordService>();
builder.Services.AddScoped<IJobEvaluatorService, JobEvaluatorService>();

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
