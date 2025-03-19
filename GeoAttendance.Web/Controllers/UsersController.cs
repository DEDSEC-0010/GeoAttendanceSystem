using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using GeoAttendance.Common.DTOs;

namespace GeoAttendance.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public UsersController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("api/users/employees");

            if (response.IsSuccessStatusCode)
            {
                var employees = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                return View(employees);
            }

            TempData["Error"] = "Unable to fetch employees list";
            return View(new List<UserDto>());
        }
    }
}