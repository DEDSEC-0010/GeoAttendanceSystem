using GeoAttendance.API.Data;
using GeoAttendance.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User> Authenticate(string username, string password)
    {
        // Explicit query construction
        var query = _context.Users
       .Where(u => u.Username == username)
       .AsQueryable();  // Explicitly mark as queryable

        var user = await query.FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}