using DevelopmentHell.Hubba.Discovery.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.SqlDataAccess.Abstractions;

namespace DevelopmentHell.Hubba.Discovery.Service.Implemenatations
{
    public class DiscoveryService : IDiscoveryService
    {
        private readonly IListingsDataAccess _listingsDataAccess;
        private readonly ICollaboratorsDataAccess _collaboratorsDataAccess;
        private readonly ILoggerService _loggerService;

        public DiscoveryService(IListingsDataAccess listingsDataAccess, ICollaboratorsDataAccess collaboratorsDataAccess, ILoggerService loggerService)
        {
            _listingsDataAccess = listingsDataAccess;
            _collaboratorsDataAccess = collaboratorsDataAccess;
            _loggerService = loggerService;
        }

        public async Task<Result<Dictionary<string, List<Dictionary<string, object>>?>>> GetCurated(int offset)
        {
            var result = new Result<Dictionary<string, List<Dictionary<string, object>>?>>();
            result.Payload = new ();

            var listingsResult = await _listingsDataAccess.Curate().ConfigureAwait(false);
            if (!listingsResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = listingsResult.ErrorMessage;
                return result;
            }

			var collaboratorsResult = await _collaboratorsDataAccess.Curate().ConfigureAwait(false);
			if (!collaboratorsResult.IsSuccessful)
			{
				result.IsSuccessful = false;
				result.ErrorMessage = collaboratorsResult.ErrorMessage;
				return result;
			}

            result.Payload.Add("listings", listingsResult.Payload);
			result.Payload.Add("collaborators", collaboratorsResult.Payload);

			return result;
		}
    }
}