using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GeoAttendance.Common.Models;
using GeoAttendance.Common.Enums;
using GeoAttendance.API.Data;

namespace GeoAttendance.API.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllEmployees()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Employee.ToString())
                .ToListAsync();
        }
    }
}