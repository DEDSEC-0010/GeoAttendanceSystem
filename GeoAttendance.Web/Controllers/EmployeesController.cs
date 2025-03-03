using Microsoft.AspNetCore.Mvc;

using GeoAttendance.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using GeoAttendance.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace GeoAttendance.Web.Controllers;
[Authorize(Policy = "AdminOnly")]
public class EmployeesController : Controller
{
    private readonly HttpClient _httpClient;

    public EmployeesController(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _httpClient.GetFromJsonAsync<List<Employee>>("api/employees");
        return View(employees);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        var response = await _httpClient.PostAsJsonAsync("api/employees", employee);
        return response.IsSuccessStatusCode
            ? RedirectToAction(nameof(Index))
            : View(employee);
    }
}
