using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using BudgetApi.Dtos.Auth;
using BudgetApi.Models;
using BudgetApi.Application.Interfaces;


namespace BudgetApi.Application.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IConfiguration _configuration;
  private readonly IMetricsService _metrics;

  public AuthService(UserManager<User> userManager, IConfiguration configuration, IMetricsService metrics)
  {
    _userManager = userManager;
    _configuration = configuration;
    _metrics = metrics;
  }

  public async Task<(bool Success, IEnumerable<object> Errors)> RegisterAsync(RegisterRequestDto request)
  {
    var user = new User { UserName = request.Email, Email = request.Email, Name = request.Name };
    var result = await _userManager.CreateAsync(user, request.Password);
    if (result.Succeeded) _metrics.RecordUserRegistered();
    return (result.Succeeded, result.Errors);
  }

  public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
    {
      _metrics.RecordLoginAttempt(false);
      return null;
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("displayName", user.Name)
        };

    var expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"]!));
    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: expires,
        signingCredentials: creds
    );

    _metrics.RecordLoginAttempt(true);

    return new AuthResponseDto(
      new JwtSecurityTokenHandler().WriteToken(token),
      expires
    );
  }

  public async Task<MeResponseDto?> GetCurrentUserAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
      return null;

    return new MeResponseDto(user.Id, user.Email ?? "No email provided.", user.Name);
  }
}