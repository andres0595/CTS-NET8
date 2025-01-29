using CTS_NET8.Connection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CTS_NET8.Configurations
{
    public class TokenGenerator
    {
        public string GenerateTokenJwt(string username)
        {
            var interfaceConfig = new InterfaceConfig();
            interfaceConfig.InitializeConfig();
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(interfaceConfig.secretKeyJWT));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            //ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) });
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim("user", username) });

            // create token to the user
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: interfaceConfig.audienceJWT,
                issuer: interfaceConfig.issuerJWT,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(interfaceConfig.expirationJWT)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }


        public string GenerateRefreshTokenJwt()
        {
            var byteArray = new byte[64];
            var refresToken = string.Empty;
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
                refresToken = Convert.ToBase64String(byteArray);
            }
            return refresToken;
        }
    }
}
