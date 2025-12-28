using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Handlers;
using Project_Dotnet_4_V2.Repositories;
using Project_Dotnet_4_V2.Requirements;
using Project_Dotnet_4_V2.Server.Hubs;
using Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.Server.Repositories.Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.Server.Services;
using Project_Dotnet_4_V2.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IEventNotificationService, EventNotificationService>();

builder.Services.AddSingleton<IAuthorizationHandler, IsAccountNotDisabledHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsAdminHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsITMemberHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();



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
            ClockSkew = TimeSpan.Zero, // par défaut c'est 5 minutes. Il s'agit de l'écart de temps autorisé entre l'heure du client et celle du serveur
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
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
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));


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

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:58965",  
                "https://localhost:5173",   
                "http://localhost:4200"     
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

var app = builder.Build();
app.UseWebSockets();
app.UseCors();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    Console.WriteLine($"=== Request to: {context.Request.Path}");
    Console.WriteLine($"=== Has Authorization header: {context.Request.Headers.ContainsKey("Authorization")}");
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        Console.WriteLine($"=== Authorization value: {context.Request.Headers["Authorization"]}");
    }
    await next();
    Console.WriteLine($"=== Response status: {context.Response.StatusCode}");
    if (context.User.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine($"=== User is authenticated: {context.User.Identity.Name}");
        Console.WriteLine($"=== User claims:");
        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($"    {claim.Type}: {claim.Value}");
        }
    }
    else
    {
        Console.WriteLine($"=== User is NOT authenticated");
    }
});
// ATTENTION L'ORDRE EST SUPER IMPORTANT
app.UseAuthentication(); // savoir à qui on a à faire
app.UseAuthorization(); // savoir ce qu'on peut faire 

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<EventHub>("/hubs/event");

app.Run();
