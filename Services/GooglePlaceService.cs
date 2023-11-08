using GoogleApi;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Details.Request;
using GoogleApi.Entities.Places.Details.Response;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using TrafficEscape.Helpers;
using TrafficEscape.Models;
using TrafficEscape.Services.Interfaces;

namespace TrafficEscape.Services
{
    public class GooglePlaceService : IGooglePlaceService
    {
        private const string baseUrl = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
        private readonly IMemoryCache memoryCache;
        private RequestThresholdPerDay requestThresholdPerDay;
        private readonly ISecretService secretService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient = new HttpClient();

        public GooglePlaceService(
            ISecretService secretService,
            IMemoryCache memoryCache,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.memoryCache = memoryCache;
            this.secretService = secretService;
            this.requestThresholdPerDay = new RequestThresholdPerDay(500);
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<PlacePrediction>> GetSuggestionsAsync(string input)
        {
            string sessionToken = "sample";
            IdentityUser user = await this.userManager.GetUserAsync(this.httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                sessionToken = user.Id;
            }
            if (!this.requestThresholdPerDay.AllowRequest())
            {
                return Enumerable.Empty<PlacePrediction>();
            }
            HttpResponseMessage httpResponse = await this.httpClient.GetAsync($"{baseUrl}?input={input}&key={this.secretService.GetSecretValue("GOOGLE_PLACE_API_KEY")}&components=country:in&sessiontoken={sessionToken}").ConfigureAwait(false);
            if (httpResponse.IsSuccessStatusCode && httpResponse.Content != null)
            {
                AutoCompletePlaceApiResponse response = await httpResponse.Content.ReadFromJsonAsync<AutoCompletePlaceApiResponse>().ConfigureAwait(false);
                return response.predictions.Where(x => x.PlaceId != null);
            }
            return Enumerable.Empty<PlacePrediction>();
        }

        public async Task<PlaceResult> GetPlaceDetailsAsync(string placeId)
        {
            if (this.memoryCache.TryGetValue(placeId, out PlaceResult result))
            {
                return result;
            }
            if (!this.requestThresholdPerDay.AllowRequest())
            {
                return null;
            }

            var request = new PlacesDetailsRequest { PlaceId = placeId, Key = this.secretService.GetSecretValue("GOOGLE_PLACE_API_KEY") };
            PlacesDetailsResponse response = await GooglePlaces.Details.QueryAsync(request, null).ConfigureAwait(false);
            if (response?.Result != null && response.Status == GoogleApi.Entities.Common.Enums.Status.Ok)
            {
                var memoryCacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) };
                this.memoryCache.Set<PlaceResult>(placeId, response.Result, memoryCacheEntryOptions);
                return response.Result;
            }
            return null;
        }
    }
}
