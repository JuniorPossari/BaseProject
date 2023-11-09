using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JWT.Serializers;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using BaseProject.Util;
using BaseProject.DAO.Models;
using BaseProject.DAO.IService;

namespace BaseProject.DAO.Service
{

    public class JwtToken
    {
        public long exp { get; set; }
    }

    public class ServiceToken : IServiceToken
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IJsonSerializer _serializer = new JsonNetSerializer();
        private IDateTimeProvider _provider = new UtcDateTimeProvider();
        private IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        private IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();

        public ServiceToken(
            IConfiguration config,
            IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        public DateTime GetExpiryTimestamp(string accessToken)
        {
            try
            {
                IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);
                var token = decoder.DecodeToObject<JwtToken>(accessToken);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(token.exp);
                return dateTimeOffset.LocalDateTime;
            }
            catch (TokenExpiredException)
            {
                return DateTime.MinValue;
            }
            catch (SignatureVerificationException)
            {
                return DateTime.MinValue;
            }
            catch (Exception e)
            {
                return DateTime.MinValue;
            }
        }

        public string GenerateToken(AspNetUser user, List<string> currentRoles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("Secret"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim("IdAspNetUser", user.Id.ToString()),
                    new Claim("IdUsuario", user.Usuario.Id.ToString()),
                    new Claim("IdEmpresa", user.Usuario.IdEmpresa.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            foreach (string role in currentRoles)
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public CookieOptions GenerateCookies()
        {
            CookieOptions cookie = new CookieOptions();

            cookie.HttpOnly = true;

            if (_webHostEnvironment.IsProduction())
            {
                cookie.Secure = true;
                cookie.SameSite = SameSiteMode.None;
            }

            return cookie;
        }

        public CookieOptions GenerateCookies(string userAgent)
        {
            CookieOptions cookie = new CookieOptions();

            cookie.HttpOnly = true;

            if (_webHostEnvironment.IsProduction())
            {
                if (DependencyInjenctionExtensions.DisallowsSameSiteNone(userAgent))
                    cookie.SameSite = SameSiteMode.Unspecified;
                else
                    cookie.SameSite = SameSiteMode.None;
                cookie.Secure = true;
            }

            return cookie;
        }
    }

}