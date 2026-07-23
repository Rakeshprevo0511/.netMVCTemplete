using MVCTemplete.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Web;

public class AuthService : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    private static int AccessTokenExpiryMinutes =>
        Convert.ToInt32(ConfigurationManager.AppSettings["JWTExpiryMinutes"]);

    private static int RefreshTokenExpiryDays =>
        Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenExpiryDays"]);

    public string GetAccessToken()
    {
        return HttpContext.Current.Request.Cookies["AccessToken"]?.Value;
    }

    public string GetRefreshToken()
    {
        return HttpContext.Current.Request.Cookies["RefreshToken"]?.Value;
    }

    public ClaimsIdentity GetCurrentIdentity()
    {
        return HttpContext.Current.User.Identity as ClaimsIdentity;
    }

    public int GetCurrentUserId()
    {
        var identity = GetCurrentIdentity();
        if (identity == null)
            return 0;

        var claim = identity.FindFirst(ClaimTypes.NameIdentifier);
        return claim == null ? 0 : Convert.ToInt32(claim.Value);
    }

    public Dictionary<string, string> ValidateAccessToken()
    {
        string token = GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        if (JWTHelper.IsExpired(token))
            return null;

        return JWTHelper.GetClaims(token);
    }

    public string GetCurrentUserName()
    {
        return GetCurrentIdentity()?.FindFirst(ClaimTypes.Name)?.Value;
    }

    public string GetCurrentUserEmail()
    {
        return GetCurrentIdentity()?.FindFirst(ClaimTypes.Email)?.Value;
    }

    // Revokes by the raw token value alone. Presenting a still-active refresh token
    // cookie is itself sufficient proof of the session — this no longer requires an
    // independently valid (non-expired) access token, so logout works even right
    // after the access token has expired and hasn't been refreshed yet.
    public bool RevokeRefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        return _refreshTokenRepository.RevokeByToken(refreshToken);
    }

    public void ClearCookies()
    {
        HttpContext.Current.Response.Cookies.Add(new HttpCookie("AccessToken")
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            Path = "/"
        });

        HttpContext.Current.Response.Cookies.Add(new HttpCookie("RefreshToken")
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            Path = "/"
        });
    }

    public void SetAccessTokenCookie(string accessToken)
    {
        var cookie = new HttpCookie("AccessToken", accessToken)
        {
            HttpOnly = true,
            Secure = HttpContext.Current.Request.Url.Scheme == Uri.UriSchemeHttps,
            SameSite = SameSiteMode.Strict,
            // Keep the cookie's browser-side TTL in sync with the JWT's own "exp" claim
            // (previously hardcoded to 1 hour while the token itself expired in 1 minute).
            Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
            Path = "/"
        };

        HttpContext.Current.Response.Cookies.Add(cookie);
    }

    public void SetRefreshTokenCookie(string refreshToken)
    {
        var cookie = new HttpCookie("RefreshToken", refreshToken)
        {
            HttpOnly = true,
            Secure = HttpContext.Current.Request.Url.Scheme == Uri.UriSchemeHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
            Path = "/"
        };

        HttpContext.Current.Response.Cookies.Add(cookie);
    }
}
