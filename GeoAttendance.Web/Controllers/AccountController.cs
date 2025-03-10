using GeoAttendance.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public AccountController(
        IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.PostAsJsonAsync("api/auth/login", new
            {
                Username = model.Username,
                Password = model.Password
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            var responseContent = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (string.IsNullOrEmpty(responseContent?.Token))
            {
                throw new InvalidOperationException("Token not received from API");
            }

            await CreateAuthCookie(responseContent.Token);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            
            ModelState.AddModelError("", "Login service unavailable");
            return View(model);
        }
    }

    // Add this class
    public class LoginResponse
    {
        public string Token { get; set; }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var client = _clientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("api/auth/register", new
        {
            Username = model.Username,
            Password = model.Password,
            Role = model.Role
        });

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Registration failed");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task CreateAuthCookie(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new SecurityTokenException("Empty token received");
            }

            var handler = new JwtSecurityTokenHandler();

            // Validate token structure first
            if (!handler.CanReadToken(token))
            {
                throw new SecurityTokenMalformedException("Invalid token format");
            }

            var jwtToken = handler.ReadJwtToken(token);

            // Ensure required claims exist
            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value
                          ?? jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                          ?? jwtToken.Subject;

            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                          ?? jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                          ?? "Employee";

            if (string.IsNullOrEmpty(nameClaim))
            {
                throw new SecurityTokenException("Username claim not found in token");
            }

            // Build claims identity
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nameClaim),
            new Claim(ClaimTypes.Role, roleClaim)
        };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwtToken.ValidTo
                });
        }
        catch (Exception ex)
        {
            // Log the error here
            
            throw new SecurityTokenException("Invalid authentication token", ex);
        }
    }
}