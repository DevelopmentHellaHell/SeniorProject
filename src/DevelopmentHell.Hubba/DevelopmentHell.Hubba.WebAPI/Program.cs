using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Analytics.Service.Implementation;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementation;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Implementation;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using HubbaAuth = DevelopmentHell.Hubba.Authentication.Service.Implementation;
using DevelopmentHell.Hubba.SqlDataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using Microsoft.Net.Http.Headers;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementation;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Transient new instance for every controller and service
// Scoped is same object from same request but different for other requests??
// Singleton is one instance across all requests
builder.Services.AddSingleton<ILoggerService, LoggerService>(s =>
{
	return new LoggerService(
		new LoggerDataAccess(
			System.Configuration.ConfigurationManager.AppSettings["LogsConnectionString"]!,
			System.Configuration.ConfigurationManager.AppSettings["LogsTable"]!
		)
	);
});
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>(s =>
{
	return new AnalyticsService(
		new AnalyticsDataAccess(
			System.Configuration.ConfigurationManager.AppSettings["LogsConnectionString"]!,
			System.Configuration.ConfigurationManager.AppSettings["LogsTable"]!
		),
		s.GetService<ILoggerService>()!
	);
});
builder.Services.AddTransient<IRegistrationManager, RegistrationManager>(s => 
	new RegistrationManager(
		new RegistrationService(
			new UserAccountDataAccess(
				System.Configuration.ConfigurationManager.AppSettings["UsersConnectionString"]!,
				System.Configuration.ConfigurationManager.AppSettings["UserAccountsTable"]!
			),
			s.GetService<ILoggerService>()!
		),
		s.GetService<ILoggerService>()!
	)
);
builder.Services.AddTransient<IAuthorizationService, AuthorizationService>(s =>
	new AuthorizationService(
		System.Configuration.ConfigurationManager.AppSettings,
        new UserAccountDataAccess(
                System.Configuration.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                System.Configuration.ConfigurationManager.AppSettings["UserAccountsTable"]!
        )

    )
);
builder.Services.AddTransient<IAuthenticationManager, AuthenticationManager>(s =>
    new AuthenticationManager(
        new HubbaAuth.AuthenticationService(
            new UserAccountDataAccess(
                System.Configuration.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                System.Configuration.ConfigurationManager.AppSettings["UserAccountsTable"]!
            ),
            s.GetService<ILoggerService>()!
        ),
		new OTPService(
			new OTPDataAccess(
                System.Configuration.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                System.Configuration.ConfigurationManager.AppSettings["UserOTPsTable"]!
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(System.Configuration.ConfigurationManager.AppSettings["JwtKey"]!)),
        ValidateLifetime = true
    };
});
builder.Services.AddCors();

var app = builder.Build();

app.Use(async (httpContext, next) =>
{
	// inbound code
    var output = await httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

    if (output.Succeeded)
    {
		Thread.CurrentPrincipal = output.Principal;
	}
	else
    {
		Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "default") }));
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