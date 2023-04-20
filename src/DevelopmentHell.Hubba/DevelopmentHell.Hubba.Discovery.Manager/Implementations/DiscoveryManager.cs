using DevelopmentHell.Hubba.Discovery.Manager.Abstractions;
using DevelopmentHell.Hubba.Discovery.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Discovery.Manager.Implementations
{
    public class DiscoveryManager : IDiscoveryManager
    {
        private readonly IDiscoveryService _discoveryService;
        private readonly ILoggerService _loggerService;

        public DiscoveryManager(IDiscoveryService discoveryService, ILoggerService loggerService) {
            _discoveryService = discoveryService;
            _loggerService = loggerService;
        }

        public async Task<Result<Dictionary<string, List<Dictionary<string, object>>?>>> GetCurated(int offset)
        {
			var result = await _discoveryService.GetCurated(offset).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new (Result.Failure(result.ErrorMessage!, result.StatusCode));
            }

            var payload = result.Payload!;

            return Result<Dictionary<string, List<Dictionary<string, object>>?>>.Success(payload);
		}
	}
}