using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Projet_Dotnet_4.Handlers;
using Projet_Dotnet_4.Requirements;
using Projet_Dotnet_4.Services;
using System.Security.Claims;
using System.Text;
using Projet_Dotnet_4.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IAuthorizationHandler, IsAccountNotDisabledHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsAdminHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsITMemberHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();


builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    ));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddCors(options => {
    options.AddPolicy("Angular", policy => {
        policy.WithOrigins("http://localhost:4200", "https://localhost:58965")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

});

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true, // identifier les parties
            ValidateAudience = true, // empecher les attaques par rejeu => un site ne peut pas rejouer un token sur un autre site
            ValidateLifetime = true, // limiter la durée de vie du token
            ValidateIssuerSigningKey = true, // demande l'utilisation de signature
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // emetteur du token
            ValidAudience = builder.Configuration["Jwt:Audience"], // receveur du token
            IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(
        builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.")
    )
), // récupérer la clé de chiffrement
            ClockSkew = TimeSpan.Zero // par défaut c'est 5 minutes. Il s'agit de l'écart de temps autorisé entre l'heure du client et celle du serveur

        };
    });



builder.Services.AddAuthorization(options =>
{
    // stratégies complexe avec handler et requirement
    options.AddPolicy("canManageMovie",
        policy =>
            policy.AddRequirements(
                new IsAccountEnabledRequirement(),
                new IsAllowedToManageMovieRequirement(),
                new MinimumAgeRequirement(21)
                ));



    // stratégies simples
    options.AddPolicy("BossOnly", policy => policy.RequireClaim("Boss"));
    options.AddPolicy("SuperIT",
        policy => policy.RequireClaim("Permission", "IT")
                        .RequireClaim("Boss")

        );
    options.AddPolicy("AutUsers", policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("BobOnly", policy => policy.RequireUserName("Bob"));
    options.AddPolicy("AdminRole", policy => policy.RequireRole("AdminRole"));

    // stratégies complèxes => lambda / handler-Requirements (class)
    options.AddPolicy("BossOrEmail", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value == "TheBoss"
         || context.User.HasClaim(claim => claim.Type == ClaimTypes.Email));
    }

    );
}

);

var app = builder.Build();
app.UseCors("Angular");
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ATTENTION L'ORDRE EST SUPER IMPORTANT
app.UseAuthentication(); // savoir à qui on a à faire
app.UseAuthorization(); // savoir ce qu'on peut faire 

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
