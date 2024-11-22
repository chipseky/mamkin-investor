using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web10.Models;

// после регистрации в паке DataBaseFiles создатся база данных.

[ApiController]
[Route("api/[controller]")]

public class AuthorizeController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JWTSettings _options;


    public AuthorizeController(UserManager<IdentityUser> user, SignInManager<IdentityUser> signIn, IOptions<JWTSettings> optAccess)
    {
        _userManager = user;
        _signInManager = signIn;
        _options = optAccess.Value;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(OtherParamUser paramUser)
    {
        var user = new IdentityUser { UserName = paramUser.UserName, Email = paramUser.Email };
       
        var result = await _userManager.CreateAsync(user, paramUser.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Profession", paramUser.Profession.ToString()));
            claims.Add(new Claim("SeniorManager", paramUser.SeniorManager.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, paramUser.Email));

            await _userManager.AddClaimsAsync(user, claims);
        }
        else
        {
            return BadRequest();
        }
        return Ok();
    }

    private string GetToken(IdentityUser user, IEnumerable<Claim> prinicpal)
    {
        var claims = prinicpal.ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.UserName));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var iwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

        return new JwtSecurityTokenHandler().WriteToken(iwt);
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn(ParamUser paramUser)
    {
        var user = await _userManager.FindByEmailAsync(paramUser.Email);
        var result = await _signInManager.PasswordSignInAsync(user, paramUser.Password, false, false);

        
        if (result.Succeeded)
        {
            IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user);
            var token = GetToken(user, claims);

            return Ok(token);
        }

        return BadRequest();
    }

}