using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeoAttendance.Common.Models;
using GeoAttendance.API.Services;

namespace GeoAttendance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin")] // Only admins can view the list of employees
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<User>>> GetEmployees()
        {
            var employees = await _userService.GetAllEmployees();
            return Ok(employees);
        }
    }
}