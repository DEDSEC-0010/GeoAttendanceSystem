using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using GeoAttendance.Common.Models;
using GeoAttendance.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GeoAttendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        => await _context.Employees.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEmployees), new { id = employee.Id }, employee);
    }
}