using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Analytics.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using HubbaAuthenticationManager = DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using HubbaAuthenticationService = DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using HubbaConfig = System.Configuration;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Transient new instance for every controller and service
// Scoped is same object from same request but different for other requests??
// Singleton is one instance across all requests
builder.Services.AddSingleton<ILoggerService, LoggerService>(s =>
{
	return new LoggerService(
		new LoggerDataAccess(
			HubbaConfig.ConfigurationManager.AppSettings["LogsConnectionString"]!,
			HubbaConfig.ConfigurationManager.AppSettings["LogsTable"]!
		)
	);
});
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>(s =>
{
	return new AnalyticsService(
		new AnalyticsDataAccess(
			HubbaConfig.ConfigurationManager.AppSettings["LogsConnectionString"]!,
			HubbaConfig.ConfigurationManager.AppSettings["LogsTable"]!
		),
		new AuthorizationService(
			HubbaConfig.ConfigurationManager.AppSettings,
			new UserAccountDataAccess(
					HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
					HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
			)
		),
		s.GetService<ILoggerService>()!
	);
});
builder.Services.AddTransient<IRegistrationManager, RegistrationManager>(s => 
	new RegistrationManager(
		new RegistrationService(
			new UserAccountDataAccess(
				HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
				HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
			),
			s.GetService<ILoggerService>()!
		),
		s.GetService<ILoggerService>()!
	)
);
builder.Services.AddTransient<IAuthorizationService, AuthorizationService>(s =>
	new AuthorizationService(
		HubbaConfig.ConfigurationManager.AppSettings,
        new UserAccountDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        )
    )
);
builder.Services.AddTransient<IAuthenticationManager, HubbaAuthenticationManager.AuthenticationManager>(s =>
    new HubbaAuthenticationManager.AuthenticationManager(
        new HubbaAuthenticationService.AuthenticationService(
            new UserAccountDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
            ),
            s.GetService<ILoggerService>()!
        ),
		new OTPService(
			new OTPDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserOTPsTable"]!
			)
		),
		s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!
    )
);

//Found on google, source lost
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(HubbaConfig.ConfigurationManager.AppSettings["JwtKey"]!)),
        ValidateLifetime = true
    };
});
builder.Services.AddCors();

var app = builder.Build();

app.Use(async (httpContext, next) =>
{
	// inbound code
	var jwtToken = httpContext.Request.Cookies["access_token"];

	if (jwtToken is not null)
    {
		// Parse the JWT token and extract the principal
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(HubbaConfig.ConfigurationManager.AppSettings["JwtKey"]!);
		var validationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			IssuerSigningKey = new SymmetricSecurityKey(key)
		};

		try
		{
			SecurityToken validatedToken;
			var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out validatedToken);

			Thread.CurrentPrincipal = principal;
		}
		catch (Exception)
		{
			// Handle token validation errors
			Thread.CurrentPrincipal = null;
		}
	}

	if (Thread.CurrentPrincipal is null)
	{
		Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "DefaultUser") }));
	}

    // Go to next middleware
    await next(httpContext);

	// Explicitly only wanting code to execite on the way out of pipeline (Response/outbound direction)
	if (httpContext.Response.Headers.ContainsKey(HeaderNames.XPoweredBy))
	{
		httpContext.Response.Headers.Remove(HeaderNames.XPoweredBy);
	}

    

    //httpContext.Response.Headers.Server = "";
});


// Defining a custom middleware AND adding it to Kestral's request pipeline
app.Use((httpContext, next) =>
{
	// Example of explicitly targeting preflight requests
	// NOT production ready implementation as X-Requested-With can 
	if (httpContext.Request.Method.ToUpper() == nameof(HttpMethod.Options).ToUpper() &&
		httpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
	{
		var allowedMethods = new List<string>()
		{
			HttpMethods.Get,
			HttpMethods.Post,
			HttpMethods.Options,
			HttpMethods.Head
		};

		httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, "*");
		httpContext.Response.Headers.AccessControlAllowMethods.Append(string.Join(",", allowedMethods)); // "GET, POST, OPTIONS, HEAD"
		httpContext.Response.Headers.AccessControlAllowHeaders.Append("*");
		httpContext.Response.Headers.AccessControlMaxAge.Append(TimeSpan.FromHours(2).Seconds.ToString());
	}

	// If you need code to execute both downstream and upstream the middleware pipeline
	// next.Invoke(httpContext);

	return next(httpContext);
});

app.UseHttpsRedirection();

app.UseCors(x => x
		.AllowAnyMethod()
		.AllowAnyHeader()
		.SetIsOriginAllowed(origin => true) // allow any origin 
		.AllowCredentials());

app.UseRouting();

app.MapControllers();
app.Run();