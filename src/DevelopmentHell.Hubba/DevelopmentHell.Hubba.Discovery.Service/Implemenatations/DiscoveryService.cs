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
        private readonly IProjectShowcaseDataAccess _projectShowcaseDataAccess;
        private readonly ILoggerService _loggerService;

        public DiscoveryService(IListingsDataAccess listingsDataAccess, ICollaboratorsDataAccess collaboratorsDataAccess, IProjectShowcaseDataAccess projectShowcasesDataAccess, ILoggerService loggerService)
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
                return new(Result.Failure(listingsResult.ErrorMessage!, listingsResult.StatusCode));
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

        public async Task<Result<List<Dictionary<string, object>>>> GetSearch(string query, string category, string filter, int offset)
        {
            double FTTWeight = 0.5;
            double otherWeights = 0.5;
            switch (filter)
            {
                case "popular":
                    FTTWeight = 0;
                    otherWeights = 1;
                    break;
                default: // none
                    FTTWeight = 0.5;
                    otherWeights = 0.5;
                    break;
            }

            var payload = new List<Dictionary<string, object>>();
            switch (category)
            {
                case "collaborators":
                    var collaboratorsResult = await _collaboratorsDataAccess.Search(query, offset, FTTWeight, otherWeights).ConfigureAwait(false);
                    if (!collaboratorsResult.IsSuccessful)
                    {
                        return new(Result.Failure(collaboratorsResult.ErrorMessage!, collaboratorsResult.StatusCode));
                    }

                    payload = collaboratorsResult.Payload;
                    break;
                case "showcases":
                    var showcasesResult = await _projectShowcaseDataAccess.Search(query, offset, FTTWeight, otherWeights).ConfigureAwait(false);
                    if (!showcasesResult.IsSuccessful)
                    {
                        return new(Result.Failure(showcasesResult.ErrorMessage!, showcasesResult.StatusCode));
                    }

                    payload = showcasesResult.Payload;
                    break;
                default: // listings
                    var listingsResult = await _listingsDataAccess.Search(query, offset, FTTWeight, otherWeights / 2, otherWeights / 2).ConfigureAwait(false);
                    if (!listingsResult.IsSuccessful)
                    {
                        return new(Result.Failure(listingsResult.ErrorMessage!, listingsResult.StatusCode));
                    }

                    payload = listingsResult.Payload;
                    break;
            }

            return Result<List<Dictionary<string, object>>>.Success(payload!);
        }
    }
}