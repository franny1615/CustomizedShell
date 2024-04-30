using Inventory.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory.API.Utilities;

public static class Auth
{
    public static string MinJWTForUser(User user)
    {
        var keyBytes = Encoding.ASCII.GetBytes(Env.JWTPrivateKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", $"{user.Id}"),
                new Claim("CompanyID", $"{user.CompanyID}"),
                new Claim("IsCompanyOwner", $"{user.IsCompanyOwner}"),
                new Claim("UserName", user.UserName),
                new Claim("Email", user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = Env.JWTIssuer,
            Audience = Env.JWTAudience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
}
