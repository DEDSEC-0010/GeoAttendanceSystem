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

        public async Task<GeofenceViewModel> CreateGeofenceAsync(GeofenceViewModel model)
        {
            try
            {
                // Create a DTO that matches the API's expected format
                var createDto = new
                {
                    Name = model.Name,
                    CenterLatitude = model.CenterLatitude,
                    CenterLongitude = model.CenterLongitude,
                    Radius = model.Radius,
                    Description = model.Description
                };

                _logger.LogInformation("Attempting to create geofence: {@GeofenceData}", createDto);

                var response = await _httpClient.PostAsJsonAsync(_baseUrl, createDto);

                // Log the response for debugging
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response: Status={StatusCode}, Content={Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API returned error: {StatusCode}, {Content}",
                        response.StatusCode, responseContent);
                    throw new HttpRequestException($"API returned {response.StatusCode}: {responseContent}");
                }

                var createdGeofence = await response.Content.ReadFromJsonAsync<GeofenceViewModel>();
                if (createdGeofence == null)
                {
                    throw new Exception("Failed to deserialize created geofence data");
                }

                return createdGeofence;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating geofence: {Name}", model.Name);
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