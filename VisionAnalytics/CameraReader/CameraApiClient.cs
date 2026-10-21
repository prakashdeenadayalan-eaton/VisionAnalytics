using CameraReader.Contracts;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading;

namespace CameraReader
{
    public class CameraApiClient
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions s_jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CameraApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<CameraDto>> GetCamerasAsync(bool analyticsEnabled = true, CancellationToken cancellationToken = default)
        {
            var uri = $"api/Cameras/GetByAnalyticsStatus?analyticsEnabled={analyticsEnabled.ToString().ToLowerInvariant()}";

            var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var cameras = await response.Content.ReadFromJsonAsync<List<CameraDto>>(s_jsonOptions, cancellationToken).ConfigureAwait(false);
            return cameras ?? new List<CameraDto>();
        }
    }
}
