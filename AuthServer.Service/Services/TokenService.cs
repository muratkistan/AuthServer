using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> tokenOption)
        {
            _userManager = userManager;
            _tokenOption = tokenOption.Value;
        }

        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(numberByte);
                return Convert.ToBase64String(numberByte);
            }
        }

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences)
        {
            //var userRoles = await _userManager.GetRolesAsync(userApp);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email, userApp.Email),
                new Claim(ClaimTypes.Name, userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim("city",userApp.City),
                //new Claim("birth-date", userApp.BirthDate.ToShortDateString())
            };

            userClaims.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            //userList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));

            return userClaims;
        }

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var clientClaims = new List<Claim>();
            clientClaims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            clientClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            clientClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString()));

            return clientClaims;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                    issuer: _tokenOption.Issuer,
                    expires: accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: GetClaims(userApp, _tokenOption.Audience),
                    signingCredentials: signingCredentials
                    );
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                    issuer: _tokenOption.Issuer,
                    expires: accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: GetClaimsByClient(client),
                    signingCredentials: signingCredentials
                    );
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);

            var clientTokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };

            return clientTokenDto;
        }
    }
}