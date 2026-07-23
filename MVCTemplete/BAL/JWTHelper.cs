using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MVCTemplete.Helpers
{
    public static class JWTHelper
    {
        // Minimum 32 characters
        private static readonly string SecretKey =
            ConfigurationManager.AppSettings["JWTSecretKey"];

        private static readonly string Issuer =
            ConfigurationManager.AppSettings["JWTIssuer"];

        private static readonly string Audience =
            ConfigurationManager.AppSettings["JWTAudience"];

        private static readonly int ExpiryMinutes =
            Convert.ToInt32(ConfigurationManager.AppSettings["JWTExpiryMinutes"]);

        /// <summary>
        /// Generate JWT Token
        /// </summary>
        public static string GenerateToken(long userId,string userName,string email,string role)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SecretKey));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validate JWT Token
        /// </summary>
        public static ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Issuer,
                    ValidAudience = Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(SecretKey)),

                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;

                var principal = tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get Claims From Token
        /// </summary>
        public static Dictionary<string, string> GetClaims(string token)
        {
            var principal = ValidateToken(token);

            if (principal == null)
                return null;

            return principal.Claims.ToDictionary(
                x => x.Type,
                x => x.Value);
        }

        /// <summary>
        /// Get User Id
        /// </summary>
        public static long GetUserId(string token)
        {
            var principal = ValidateToken(token);

            if (principal == null)
                return 0;

            return Convert.ToInt64(
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Hash a refresh token for storage/lookup. The raw token lives only in the
        /// user's HttpOnly cookie; the DB only ever sees this one-way hash, so a DB
        /// dump alone can't be used to impersonate a session.
        /// </summary>
        public static string HashRefreshToken(string rawToken)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Check Expiry
        /// </summary>
        public static bool IsExpired(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            return jwt.ValidTo < DateTime.UtcNow;
        }

        /// <summary>
        /// Read JWT Token
        /// </summary>
        public static JwtSecurityToken ReadToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
    }
}
