using Development.Hubba.JWTHandler.Service.Abstractions;
using Development.Hubba.JWTHandler.Service.Implementations;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Abstraction;
using DevelopmentHell.Hubba.AccountDeletion.Manager.Implementations;
using DevelopmentHell.Hubba.AccountDeletion.Service.Abstractions;
using DevelopmentHell.Hubba.AccountDeletion.Service.Implementations;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Abstractions;
using DevelopmentHell.Hubba.AccountRecovery.Manager.Implementations;
using DevelopmentHell.Hubba.AccountRecovery.Service.Implementations;
using DevelopmentHell.Hubba.AccountSystem.Implementations;
using DevelopmentHell.Hubba.AccountSystem.Manager.Abstractions;
using DevelopmentHell.Hubba.AccountSystem.Manager.Implementations;
using DevelopmentHell.Hubba.Analytics.Service.Abstractions;
using DevelopmentHell.Hubba.Analytics.Service.Implementations;
using DevelopmentHell.Hubba.Authentication.Manager.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Abstractions;
using DevelopmentHell.Hubba.Authentication.Service.Implementations;
using DevelopmentHell.Hubba.Authorization.Service.Abstractions;
using DevelopmentHell.Hubba.Authorization.Service.Implementations;
using DevelopmentHell.Hubba.CellPhoneProvider.Service.Implementations;
using DevelopmentHell.Hubba.Collaborator.Manager.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Manager.Implementations;
using DevelopmentHell.Hubba.Collaborator.Service.Abstractions;
using DevelopmentHell.Hubba.Collaborator.Service.Implementations;
using DevelopmentHell.Hubba.Cryptography.Service.Abstractions;
using DevelopmentHell.Hubba.Cryptography.Service.Implementations;
using DevelopmentHell.Hubba.Discovery.Manager.Abstractions;
using DevelopmentHell.Hubba.Discovery.Manager.Implementations;
using DevelopmentHell.Hubba.Discovery.Service.Implemenatations;
using DevelopmentHell.Hubba.Email.Service.Abstractions;
using DevelopmentHell.Hubba.Email.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Implementations;
using DevelopmentHell.Hubba.ListingProfile.Manager.Implementations;
using DevelopmentHell.Hubba.ListingProfile.Service.Implementations;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.Notification.Manager.Abstractions;
using DevelopmentHell.Hubba.Notification.Manager.Implementations;
using DevelopmentHell.Hubba.Notification.Service.Abstractions;
using DevelopmentHell.Hubba.Notification.Service.Implementations;
using DevelopmentHell.Hubba.OneTimePassword.Service.Abstractions;
using DevelopmentHell.Hubba.OneTimePassword.Service.Implementations;
using DevelopmentHell.Hubba.Registration.Manager.Abstractions;
using DevelopmentHell.Hubba.Registration.Manager.Implementations;
using DevelopmentHell.Hubba.Registration.Service.Abstractions;
using DevelopmentHell.Hubba.Registration.Service.Implementations;
using DevelopmentHell.Hubba.Scheduling.Manager;
using DevelopmentHell.Hubba.Scheduling.Manager.Abstraction;
using DevelopmentHell.Hubba.Scheduling.Service.Abstractions;
using DevelopmentHell.Hubba.Scheduling.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;
using DevelopmentHell.Hubba.SqlDataAccess.Implementation;
using DevelopmentHell.Hubba.Testing.Service.Abstractions;
using DevelopmentHell.Hubba.Testing.Service.Implementations;
using DevelopmentHell.Hubba.UserManagement.Manager.Abstractions;
using DevelopmentHell.Hubba.UserManagement.Manager.Implementations;
using DevelopmentHell.Hubba.UserManagement.Service.Abstractions;
using DevelopmentHell.Hubba.UserManagement.Service.Implementations;
using DevelopmentHell.Hubba.Validation.Service.Abstractions;
using DevelopmentHell.Hubba.Validation.Service.Implementations;
using DevelopmentHell.ListingProfile.Manager.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Implementations;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using HubbaAuthenticationManager = DevelopmentHell.Hubba.Authentication.Manager.Implementations;
using HubbaConfig = System.Configuration;
using DevelopmentHell.Hubba.ProjectShowcase.Manager.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Abstractions;
using DevelopmentHell.Hubba.ProjectShowcase.Service.Implementations;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Files.Service.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<ITestingService, TestingService>(s =>
{
    return new TestingService(
        HubbaConfig.ConfigurationManager.AppSettings["JwtKey"]!,
        new TestsDataAccess()
    );
});

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
        s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!
    );
});
builder.Services.AddScoped<IValidationService, ValidationService>(s =>
{
    return new ValidationService();
});
builder.Services.AddTransient<ICryptographyService, CryptographyService>(s =>
{
    return new CryptographyService(HubbaConfig.ConfigurationManager.AppSettings["CryptographyKey"]!);
});
builder.Services.AddTransient<IJWTHandlerService, JWTHandlerService>(s =>
{
    return new JWTHandlerService(HubbaConfig.ConfigurationManager.AppSettings["JwtKey"]!);
});
builder.Services.AddTransient<IEmailService, EmailService>(s =>
{
    return new EmailService(
        HubbaConfig.ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!,
        HubbaConfig.ConfigurationManager.AppSettings["SENDGRID_API_KEY"]!,
        HubbaConfig.ConfigurationManager.AppSettings["COMPANY_EMAIL"]!
    );
});
builder.Services.AddTransient<INotificationService, NotificationService>(s =>
    new NotificationService(
        new NotificationDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserNotificationsTable"]!
        ),
        new NotificationSettingsDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["NotificationsConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["NotificationSettingsTable"]!
        ),
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IAccountSystemManager, AccountSystemManager>(s =>
    new AccountSystemManager(
        new AccountSystemService(
            s.GetService<IUserAccountDataAccess>()!,
            s.GetService<ILoggerService>()!
        ),
        s.GetService<IOTPService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ICryptographyService>()!,
        s.GetService<INotificationManager>()!,
        s.GetService<IValidationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<INotificationManager, NotificationManager>(s =>
    new NotificationManager(
        s.GetService<INotificationService>()!,
        new CellPhoneProviderService(),
        s.GetService<IEmailService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<IValidationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IListingsDataAccess, ListingsDataAccess>(s =>
    new ListingsDataAccess(
        HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
        HubbaConfig.ConfigurationManager.AppSettings["ListingsTable"]!
    )
);

builder.Services.AddTransient<ICollaboratorsDataAccess, CollaboratorsDataAccess>(s =>
	new CollaboratorsDataAccess(
		HubbaConfig.ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
        HubbaConfig.ConfigurationManager.AppSettings["CollaboratorsTable"]!
    )
);
builder.Services.AddTransient<IDiscoveryManager, DiscoveryManager>(s =>
    new DiscoveryManager(
        new DiscoveryService(
            s.GetService<IListingsDataAccess>()!,
            s.GetService<ICollaboratorsDataAccess>()!,
			s.GetService<IProjectShowcaseDataAccess>()!,
			s.GetService<ILoggerService>()!
        ),
		s.GetService<ILoggerService>()!
	)
);
builder.Services.AddTransient<IRegistrationService, RegistrationService>(s =>
    new RegistrationService(
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        s.GetService<ICryptographyService>()!,
        s.GetService<IValidationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IRegistrationManager, RegistrationManager>(s =>
    new RegistrationManager(
        s.GetService<IRegistrationService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ICryptographyService>()!,
        s.GetService<INotificationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IOTPService, OTPService>(s =>
{
    return new OTPService(
        new OTPDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserOTPsTable"]!
        ),
        s.GetService<IEmailService>()!,
        s.GetService<ICryptographyService>()!
    );
});

builder.Services.AddTransient<IFileService, FTPFileService>(s =>
    new FTPFileService
            (
                HubbaConfig.ConfigurationManager.AppSettings["FTPServer"]!,
                HubbaConfig.ConfigurationManager.AppSettings["FTPUsername"]!,
                HubbaConfig.ConfigurationManager.AppSettings["FTPPassword"]!,
                s.GetService<ILoggerService>()!
            )
);
builder.Services.AddTransient<IAuthorizationService, AuthorizationService>(s =>
    new AuthorizationService(
        new UserAccountDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        s.GetService<IJWTHandlerService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>(s =>
    new AuthenticationService(
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        new UserLoginDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserLoginsTable"]!
        ),
        s.GetService<ICryptographyService>()!,
        s.GetService<IJWTHandlerService>()!,
        s.GetService<IValidationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IAuthenticationManager, HubbaAuthenticationManager.AuthenticationManager>(s =>
    new HubbaAuthenticationManager.AuthenticationManager(
        s.GetService<IAuthenticationService>()!,
        s.GetService<IOTPService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ICryptographyService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IAccountRecoveryManager, AccountRecoveryManager>(s =>
    new AccountRecoveryManager(
        new AccountRecoveryService(
            new UserAccountDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
            ),
            new UserLoginDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserLoginsTable"]!
            ),
            new RecoveryRequestDataAccess(
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["RecoveryRequestsTable"]!
            ),
            s.GetService<IValidationService>()!,
            s.GetService<ILoggerService>()!
        ),
        s.GetService<IOTPService>()!,
        s.GetService<IAuthenticationService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IAccountDeletionService, AccountDeletionService>(s =>
    new AccountDeletionService(
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        s.GetService<INotificationManager>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IAccountDeletionManager, AccountDeletionManager>(s =>
    new AccountDeletionManager(
        s.GetService<IAccountDeletionService>()!,
        s.GetService<IAuthenticationService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IUserManagementService, UserManagementService>(s =>
    new UserManagementService(
        s.GetService<ILoggerService>()!,
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        new UserNamesDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserNamesTable"]!
        )
    )
);
builder.Services.AddTransient<IUserManagementManager, UserManagementManager>(s =>
    new UserManagementManager(
        s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!,
        s.GetService<IRegistrationService>()!,
        s.GetService<IUserManagementService>()!,
        s.GetService<IValidationService>()!,
        s.GetService<IAccountDeletionService>()!,
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        )
    )
);
builder.Services.AddTransient<ICollaboratorService, CollaboratorService>(s =>
    new CollaboratorService(
        new CollaboratorsDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorsTable"]!),
        new CollaboratorFileDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorFilesTable"]!),
        new CollaboratorFileJunctionDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorFileJunctionTable"]!),
        new CollaboratorUserVoteDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorProfilesConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["CollaboratorUserVotesTable"]!),
        s.GetService<IFileService>()!,
        s.GetService<ILoggerService>()!,
        s.GetService<IValidationService>()!
    )
);
builder.Services.AddTransient<ICollaboratorManager, CollaboratorManager>(s =>
    new CollaboratorManager(
        s.GetService<ICollaboratorService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!,
        s.GetService<IValidationService>()!
    )
);
builder.Services.AddTransient<IListingProfileManager, ListingProfileManager>(s =>
    new ListingProfileManager
    (
        new ListingProfileService
        (
            new ListingsDataAccess
            (
                HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["ListingsTable"]!
            ),
            new ListingAvailabilitiesDataAccess
            (
                HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!
            ),
            new ListingHistoryDataAccess
            (
                HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["ListingHistoryTable"]!
            ),
            new RatingDataAccess
            (
                HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["ListingRatingsTable"]!
            ),
            new UserAccountDataAccess
            (
                HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
                HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
            ),
            s.GetService<ILoggerService>()!
        ),
        s.GetService<IFileService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<ILoggerService>()!,
        s.GetService<IValidationService>()!,
        s.GetService<ICryptographyService>()!
    )
);
builder.Services.AddTransient<IProjectShowcaseDataAccess, ProjectShowcaseDataAccess>(s =>
	new ProjectShowcaseDataAccess(
		HubbaConfig.ConfigurationManager.AppSettings["ProjectShowcasesConnectionString"]!,
		HubbaConfig.ConfigurationManager.AppSettings["ShowcasesTable"]!,
		HubbaConfig.ConfigurationManager.AppSettings["ShowcaseCommentsTable"]!,
		HubbaConfig.ConfigurationManager.AppSettings["ShowcaseVotesTable"]!,
		HubbaConfig.ConfigurationManager.AppSettings["ShowcaseCommentVotesTable"]!,
		HubbaConfig.ConfigurationManager.AppSettings["ShowcaseReportsTable"]!,
		HubbaConfig.ConfigurationManager.AppSettings["ShowcaseCommentReportsTable"]!
	)
);
builder.Services.AddTransient<IProjectShowcaseService, ProjectShowcaseService>(s =>
    new ProjectShowcaseService(
        s.GetService<IProjectShowcaseDataAccess>()!,
        new UserAccountDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["UsersConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["UserAccountsTable"]!
        ),
        s.GetService<IValidationService>()!,
        s.GetService<ILoggerService>()!
    )
);
builder.Services.AddTransient<IProjectShowcaseManager, ProjectShowcaseManager>(s =>
    new ProjectShowcaseManager(
        s.GetService<IProjectShowcaseService>()!,
        s.GetService<IFileService>()!,
        s.GetService<ILoggerService>()!,
        s.GetService<IAuthorizationService>()!
    )
);

builder.Services.AddTransient<IAvailabilityService, AvailabilityService>(s =>
    new AvailabilityService(
        new ListingsDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["ListingsTable"]!
        ),
        new ListingAvailabilitiesDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["ListingProfilesConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["ListingAvailabilitiesTable"]!
        ),
        new BookedTimeFramesDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["SchedulingsConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["BookedTimeFramesTable"]!
        )
    )
);
builder.Services.AddTransient<IBookingService, BookingService>(s =>
    new BookingService(
        new BookingsDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["SchedulingsConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["BookingsTable"]!
        ),
        new BookedTimeFramesDataAccess(
            HubbaConfig.ConfigurationManager.AppSettings["SchedulingsConnectionString"]!,
            HubbaConfig.ConfigurationManager.AppSettings["BookedTimeFramesTable"]!
        )
    )
);
builder.Services.AddTransient<ISchedulingManager, SchedulingManager>(s =>
    new SchedulingManager(
        s.GetService<IBookingService>()!,
        s.GetService<IAvailabilityService>()!,
        s.GetService<IAuthorizationService>()!,
        s.GetService<INotificationService>()!,
        s.GetService<ILoggerService>()!
    )
);

builder.Services.AddCors();

var app = builder.Build();

app.Use(async (httpContext, next) =>
{
    // inbound request
    var accessToken = httpContext.Request.Cookies["access_token"];
    var idToken = httpContext.Request.Cookies["id_token"];
    var key = HubbaConfig.ConfigurationManager.AppSettings["JwtKey"]!;
    var jwtHandlerService = new JWTHandlerService(key);
    if (accessToken is not null)
    {
        if (jwtHandlerService.ValidateJwt(accessToken))
        {
            var principal = jwtHandlerService.GetPrincipal(accessToken);
            Thread.CurrentPrincipal = principal;
        }
        else
        {
            var token = jwtHandlerService.GenerateInvalidToken();
            httpContext.Response.Cookies.Append("access_token", token, new CookieOptions { SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None, Secure = true });
            httpContext.Response.Cookies.Append("id_token", token, new CookieOptions { SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None, Secure = true });
            Thread.CurrentPrincipal = null;
        }
    }

    if (idToken is not null && accessToken is not null && Thread.CurrentPrincipal is not null)
    {
        if (!jwtHandlerService.ValidateJwt(idToken))
        {
            var token = jwtHandlerService.GenerateInvalidToken();
            httpContext.Response.Cookies.Append("access_token", token, new CookieOptions { SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None, Secure = true });
            httpContext.Response.Cookies.Append("id_token", token, new CookieOptions { SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None, Secure = true });
            Thread.CurrentPrincipal = null;
        }
    }

    if (Thread.CurrentPrincipal is null)
    {
        Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("role", "DefaultUser") }));
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