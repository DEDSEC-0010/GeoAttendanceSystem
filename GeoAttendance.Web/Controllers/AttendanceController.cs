using Microsoft.AspNetCore.Mvc;
using GeoAttendance.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using GeoAttendance.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace GeoAttendance.Web.Controllers;
[Authorize(Policy = "EmployeeOnly")]
public class AttendanceController : Controller
{
    private readonly HttpClient _httpClient;

    public AttendanceController(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public IActionResult MarkAttendance() => View();

    [HttpPost]
    public async Task<IActionResult> MarkAttendance(AttendanceViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/attendance", new
        {
            model.EmployeeId,
            model.Latitude,
            model.Longitude
        });

        if (response.IsSuccessStatusCode)
        {
            var record = await response.Content.ReadFromJsonAsync<AttendanceRecord>();
            return View("AttendanceResult", record);
        }
        return View("Error");
    }
}