using GeoAttendance.Web.Models;

namespace GeoAttendance.Web.Services
{
    public interface IGeofenceService
    {
        Task<IEnumerable<GeofenceViewModel>> GetAllGeofencesAsync();
        Task<GeofenceViewModel> GetGeofenceAsync(int id);
        Task<GeofenceViewModel> CreateGeofenceAsync(GeofenceViewModel geofence);
        Task<bool> UpdateGeofenceAsync(GeofenceViewModel geofence);
        Task<bool> DeleteGeofenceAsync(int id);
        Task<bool> CheckLocationAsync(float latitude, float longitude);
    }
}