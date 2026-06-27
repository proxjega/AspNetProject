using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetProject.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetProject.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace AspNetProject.Controllers;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AuthController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == registerDTO.Email);

        if (exists) return BadRequest("User with this email already exists");

        var user = new User
        {
            Email = registerDTO.Email,
            Name = registerDTO.Name,
            Surname = registerDTO.Surname,
            PasswordHash = ""
        };

        var hasher = new PasswordHasher<User>();
        var hashedPassword = hasher.HashPassword(user, registerDTO.password);
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Register),
            new { id = user.Id },
            UserController.UserToDTO(user));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == loginDTO.Email);
        if (!exists) return BadRequest("User with this email does not exist");

        var userFromDB = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDTO.Email);
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(
            userFromDB!,
            userFromDB!.PasswordHash,
            loginDTO.Password);
        if (result == PasswordVerificationResult.Failed) return BadRequest("Password is wrong");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userFromDB.Id.ToString()),
            new(ClaimTypes.Email, userFromDB.Email)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        // ISSUE COOKIE
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return Ok();
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (HttpContext.User == null) return BadRequest();

        await HttpContext.SignOutAsync();
        return Ok();
    }
}