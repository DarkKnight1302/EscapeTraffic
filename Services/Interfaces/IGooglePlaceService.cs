using TrafficEscape.Models;
using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Places.Common;

namespace TrafficEscape.Services.Interfaces
{
    public interface IGooglePlaceService
    {
        /// <summary>
        /// Get suggestions for location based on user input.
        /// </summary>
        /// <param name="input">input string.</param>
        /// <returns>Predictions.</returns>
        public Task<IEnumerable<PlacePrediction>> GetSuggestionsAsync(string input);

        /// <summary>
        /// Get place location.
        /// </summary>
        /// <param name="placeId">PlaceId.</param>
        /// <returns>Result.</returns>
        public Task<PlaceResult> GetPlaceDetailsAsync(string placeId);
    }
}
