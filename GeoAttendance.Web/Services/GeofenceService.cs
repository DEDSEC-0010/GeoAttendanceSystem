using System.Net.Http.Json;
using GeoAttendance.Web.Models;

namespace GeoAttendance.Web.Services
{
    public class GeofenceService : IGeofenceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeofenceService> _logger;
        private readonly string _baseUrl;

        public GeofenceService(HttpClient httpClient, IConfiguration configuration, ILogger<GeofenceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] + "/api/geofence";
        }

        public async Task<IEnumerable<GeofenceViewModel>> GetAllGeofencesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<GeofenceViewModel>>()
                    ?? Enumerable.Empty<GeofenceViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all geofences");
                throw;
            }
        }

        public async Task<GeofenceViewModel> GetGeofenceAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GeofenceViewModel>()
                    ?? throw new Exception("Failed to deserialize geofence data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting geofence with ID: {Id}", id);
                throw;
            }
        }

        public async Task<GeofenceViewModel> CreateGeofenceAsync(GeofenceViewModel geofence)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, geofence);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<GeofenceViewModel>()
                    ?? throw new Exception("Failed to deserialize created geofence data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating geofence: {Name}", geofence.Name);
                throw;
            }
        }

        public async Task<bool> UpdateGeofenceAsync(GeofenceViewModel geofence)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{geofence.Id}", geofence);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating geofence with ID: {Id}", geofence.Id);
                throw;
            }
        }

        public async Task<bool> DeleteGeofenceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting geofence with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> CheckLocationAsync(float latitude, float longitude)
        {
            try
            {
                var checkRequest = new { latitude, longitude };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/check-location", checkRequest);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking location: Lat={Latitude}, Long={Longitude}",
                    latitude, longitude);
                throw;
            }
        }
    }
}