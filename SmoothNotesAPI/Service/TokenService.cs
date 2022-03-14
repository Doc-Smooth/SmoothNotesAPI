using Microsoft.IdentityModel.Tokens;
using SmoothNotesAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmoothNotesAPI.Service;

public class TokenService
{
    public IConfiguration _configuration { get; }
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public string CreateToken(Profile profile)
    //{
    //    List<Claim> claims = new List<Claim>
    //    {
    //        new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString().ToUpper()),
    //        new Claim(ClaimTypes.Name, profile.Name)
    //    };

    //    //Creating a symmetric key to be use for the creation of the token.
    //    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    //    var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);

    //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

    //    return jwt;
    //}
}
