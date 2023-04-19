using DevelopmentHell.Hubba.Discovery.Service.Abstractions;
using DevelopmentHell.Hubba.Discovery.Service.Implemenatations;
using DevelopmentHell.Hubba.Logging.Service.Implementations;
using DevelopmentHell.Hubba.SqlDataAccess;
using System.Configuration;

var listingsDao = new ListingsDataAccess("Server=.;Database=DevelopmentHell.Hubba.ListingProfiles;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.ListingProfile;Password=password");
var collaboratorsDao = new CollaboratorsDataAccess("Server=.;Database=DevelopmentHell.Hubba.CollaboratorProfiles;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.CollaboratorProfile;Password=password");

IDiscoveryService discoveryService = new DiscoveryService(listingsDao, collaboratorsDao, new LoggerService(
	new LoggerDataAccess(
		ConfigurationManager.AppSettings["LogsConnectionString"]!, ConfigurationManager.AppSettings["LogsTable"]!
	)
));

var result = await discoveryService.GetCurated(0).ConfigureAwait(false);
if (!result.IsSuccessful)
{
	Console.WriteLine(result.ErrorMessage);
	return;
}