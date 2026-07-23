using MVCTemplete.DAL.DTOs;
using MVCTemplete.Helpers;
using MVCTemplete.Models;
using MVCTemplete.Models.DTOs;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public class AdminService : IAdminService
{
    private readonly DBHelper _dbHelper;
    private readonly IAuthService _authService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AdminService(IAuthService authService, IRefreshTokenRepository refreshTokenRepository)
    {
        _dbHelper = new DBHelper();
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    private static int RefreshTokenExpiryDays =>
        Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenExpiryDays"]);

    public ApiResponse<object> Login(LoginRequest request)
    {
        if (request.Username != "admin" || request.Password != "password")
        {
            return ApiResponse<object>.FailureResponse(
                "Unauthorized",
                new System.Collections.Generic.List<string> { "Invalid username or password." },
                401);
        }

        int userId = 1;

        string accessToken = JWTHelper.GenerateToken(userId, "admin", "admin@example.com", "Admin");
        string refreshToken = JWTHelper.GenerateRefreshToken();

        // Retire any tokens from previous sessions before issuing a fresh one.
        _refreshTokenRepository.RevokeAllForUser(userId);
        _refreshTokenRepository.Add(userId, refreshToken, DateTime.UtcNow.AddDays(RefreshTokenExpiryDays));
        _refreshTokenRepository.SaveChanges();

        _authService.SetAccessTokenCookie(accessToken);
        _authService.SetRefreshTokenCookie(refreshToken);

        _authService.SetRememberMeCookie(request.RememberMe);

        return ApiResponse<object>.SuccessResponse(null, "Login Successful");
    }

    public ApiResponse<object> Refresh()
    {
        string refreshToken = _authService.GetRefreshToken();

        if (string.IsNullOrEmpty(refreshToken))
        {
            return ApiResponse<object>.FailureResponse(
                "Unauthorized",
                new System.Collections.Generic.List<string> { "Refresh token cookie not found." },
                401);
        }

        var activeToken = _refreshTokenRepository.GetActive(refreshToken);

        if (activeToken == null)
        {
            // Not active — either genuinely unknown/expired, or (more interesting) a
            // token that WAS valid but has already been rotated/used once before.
            // The latter means this exact refresh token got used twice, which only
            // happens if it leaked and both the real user and an attacker are now
            // racing to use it. Treat that as a compromise signal and kill every
            // active session for that user, not just this one request.
            var anyMatch = _refreshTokenRepository.FindAny(refreshToken);

            if (anyMatch != null && anyMatch.IsRevoked)
            {
                _refreshTokenRepository.RevokeAllForUser(anyMatch.UserId);
                _refreshTokenRepository.SaveChanges();

                return ApiResponse<object>.FailureResponse(
                    "Unauthorized",
                    new System.Collections.Generic.List<string> { "Refresh token reuse detected. Please log in again." },
                    401);
            }

            return ApiResponse<object>.FailureResponse(
                "Unauthorized",
                new System.Collections.Generic.List<string> { "Refresh token is invalid or expired." },
                401);
        }

        int userId = activeToken.UserId;

        string accessToken = JWTHelper.GenerateToken(userId, "admin", "admin@example.com", "Admin");
        string newRefreshToken = JWTHelper.GenerateRefreshToken();

        // Rotate: retire the used token, issue a new one.
        _refreshTokenRepository.Revoke(activeToken);
        _refreshTokenRepository.Add(userId, newRefreshToken, DateTime.UtcNow.AddDays(RefreshTokenExpiryDays));
        _refreshTokenRepository.SaveChanges();

        _authService.SetAccessTokenCookie(accessToken);
        _authService.SetRefreshTokenCookie(newRefreshToken);

        return ApiResponse<object>.SuccessResponse(
            new { UserID = userId },
            "Token refreshed successfully.");
    }

    public ApiResponse<object> ValidateToken()
    {
        string token = _authService.GetAccessToken();

        if (string.IsNullOrWhiteSpace(token))
        {
            return ApiResponse<object>.FailureResponse(
                "Unauthorized",
                new System.Collections.Generic.List<string> { "Access token cookie not found." },
                401);
        }
        if (JWTHelper.IsExpired(token))
        {
            return ApiResponse<object>.FailureResponse(
                "Unauthorized",
                new System.Collections.Generic.List<string> { "Access token cookie not found." },
                401);
        }
        var claims = _authService.ValidateAccessToken();

        if (claims == null)
        {
            return ApiResponse<object>.FailureResponse(
                "Invalid Token",
                new System.Collections.Generic.List<string> { "Token is invalid or expired." },
                401);
        }

        return ApiResponse<object>.SuccessResponse(null, "Token is valid");
    }

    public async Task<ApiResponse<object>> GetUsers(int pageNumber, int pageSize, string search)
    {
        try
        {
            // Guard against absurd/abusive paging requests reaching the DB unclamped.
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200;

            DataTable dt = await _dbHelper.GetDataTableAsync(
                "USP_GetUsers",
                new System.Data.SqlClient.SqlParameter("@PageNumber", pageNumber),
                new System.Data.SqlClient.SqlParameter("@PageSize", pageSize),
                new System.Data.SqlClient.SqlParameter("@Search",
                    string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search)
            );

            return ApiResponse<object>.SuccessResponse(dt, "Users fetched successfully.");
        }
        catch (Exception ex)
        {
            // Log full detail server-side only — never echo exception internals
            // (SQL/connection/stack details) back to the client.
            Trace.TraceError("AdminService.GetUsers failed: " + ex);

            return ApiResponse<object>.FailureResponse(
                "Error",
                new System.Collections.Generic.List<string> { "An error occurred while fetching users. Please try again later." },
                500);
        }

    }
    public async Task<ApiResponse<object>> SetContent(SaveContentModel model)
    {
        try
        {
            if (model == null)
            {
                return ApiResponse<object>.FailureResponse("Invalid request.");
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return ApiResponse<object>.FailureResponse("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(model.HtmlContent))
            {
                return ApiResponse<object>.FailureResponse("HTML content is required.");
            }

            // Maximum HTML size = 1 MB
            var size = Encoding.UTF8.GetByteCount(model.HtmlContent);

            if (size > 1024 * 1024)
            {
                return ApiResponse<object>.FailureResponse("HTML content cannot exceed 1 MB.");
            }

            DataTable dt = await _dbHelper.GetDataTableAsync(
                "USP_SaveHtmlContent",
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Title", model.Title),
                new SqlParameter("@HtmlContent", model.HtmlContent)
            );

            return ApiResponse<object>.SuccessResponse(dt, "Content saved successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.FailureResponse(ex.Message);
        }
    }
    public async Task<ApiResponse<object>> GetContent(int id)
    {
        try
        {
            DataTable dt = await _dbHelper.GetDataTableAsync(
                "USP_GetHtmlContent",
                new SqlParameter("@Id", id)
            );

            return ApiResponse<object>.SuccessResponse(dt, "Content fetched successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.FailureResponse(ex.Message);
        }
    }
    public async Task<ApiResponse<object>> GetAllContent()
    {
        try
        {
            DataTable dt = await _dbHelper.GetDataTableAsync(
                "USP_GetAllHtmlContent"
            );

            return ApiResponse<object>.SuccessResponse(dt, "Contents fetched successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.FailureResponse(ex.Message);
        }
    }

}
