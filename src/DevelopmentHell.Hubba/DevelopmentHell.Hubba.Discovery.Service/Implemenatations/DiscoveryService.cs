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
        private readonly IProjectShowcasesDataAccess _projectShowcaseDataAccess;
        private readonly ILoggerService _loggerService;

        public DiscoveryService(IListingsDataAccess listingsDataAccess, ICollaboratorsDataAccess collaboratorsDataAccess, IProjectShowcasesDataAccess projectShowcasesDataAccess, ILoggerService loggerService)
        {
            _listingsDataAccess = listingsDataAccess;
            _collaboratorsDataAccess = collaboratorsDataAccess;
            _projectShowcaseDataAccess = projectShowcasesDataAccess;
            _loggerService = loggerService;
        }

        public async Task<Result<Dictionary<string, List<Dictionary<string, object>>?>>> GetCurated(int offset)
        {
            var listingsResult = await _listingsDataAccess.Curate().ConfigureAwait(false);
            if (!listingsResult.IsSuccessful)
            {
                return new (Result.Failure(listingsResult.ErrorMessage!, listingsResult.StatusCode));
            }

			var collaboratorsResult = await _collaboratorsDataAccess.Curate().ConfigureAwait(false);
			if (!collaboratorsResult.IsSuccessful)
			{
				return new(Result.Failure(collaboratorsResult.ErrorMessage!, collaboratorsResult.StatusCode));
			}

            var showcasesResult = await _projectShowcaseDataAccess.Curate().ConfigureAwait(false);
			if (!showcasesResult.IsSuccessful)
			{
				return new(Result.Failure(showcasesResult.ErrorMessage!, showcasesResult.StatusCode));
			}

			var payload = new Dictionary<string, List<Dictionary<string, object>>?>();
			payload.Add("listings", listingsResult.Payload);
			payload.Add("collaborators", collaboratorsResult.Payload);
            payload.Add("showcases", showcasesResult.Payload);

			return Result<Dictionary<string, List<Dictionary<string, object>>?>>.Success(payload);
		}
    }
}