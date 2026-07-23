using System.Collections.Generic;
using System.Security.Claims;

public interface IAuthService
{
    string GetAccessToken();
    string GetRefreshToken();
    string GenerateOTP();
    bool IsRememberMe();
    ClaimsIdentity GetCurrentIdentity();
    int GetCurrentUserId();
    string GetCurrentUserName();
    string GetCurrentUserEmail();
    Dictionary<string, string> ValidateAccessToken();
    bool RevokeRefreshToken(string refreshToken);
    void ClearCookies();
    void SetAccessTokenCookie(string accessToken);
    void SetRefreshTokenCookie(string refreshToken);
    void SetRememberMeCookie(bool rememberMe);
}