using DevelopmentHell.Hubba.Discovery.Manager.Abstractions;
using DevelopmentHell.Hubba.Discovery.Service.Abstractions;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

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
			if (offset < 0)
			{
				return new(Result.Failure("Invalid offset.", StatusCodes.Status400BadRequest));
			}

			var result = await _discoveryService.GetCurated(offset).ConfigureAwait(false);
            if (!result.IsSuccessful)
            {
                return new (Result.Failure(result.ErrorMessage!, result.StatusCode));
            }

            var payload = result.Payload!;

            return Result<Dictionary<string, List<Dictionary<string, object>>?>>.Success(payload);
		}

        public async Task<Result<List<Dictionary<string, object>>>> GetSearch(string query, string category, string filter, int offset)
        {
            if (offset < 0)
            {
                return new(Result.Failure("Invalid offset.", StatusCodes.Status400BadRequest));
            }

            if (query.Length > 200)
            {
                return new(Result.Failure("Query is longer than 200 characters.", StatusCodes.Status414RequestUriTooLong));
            }

            if (query.Length == 0)
            {
                return new(Result.Failure("Query is empty.", StatusCodes.Status400BadRequest));
            }

			var result = await _discoveryService.GetSearch(query, category, filter, offset).ConfigureAwait(false);
			if (!result.IsSuccessful)
			{
				return new(Result.Failure(result.ErrorMessage!, result.StatusCode));
			}

			var payload = result.Payload!;

			return Result<List<Dictionary<string, object>>>.Success(payload);
		}
	}
}