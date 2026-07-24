using MVCTemplete.DAL.DTOs;
using MVCTemplete.Filters;
using MVCTemplete.Models;
using MVCTemplete.Models.DTOs;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[JWTAuthorize]
[RoutePrefix("api/admin")]
public class AdminController : BaseApiController
{

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public IHttpActionResult Login([FromBody] LoginRequest request)
    {
        var result = _adminService.Login(request);
        if (result != null){
            var otp = _authService.GenerateOTP();
        }
        return Content((HttpStatusCode)result.StatusCode, result);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("refresh")]
    public IHttpActionResult Refresh()
    {
        var result = _adminService.Refresh();
        return Content((HttpStatusCode)result.StatusCode, result);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("logout")]
    public IHttpActionResult Logout()
    {
        string refreshToken = _authService.GetRefreshToken();
        if (!string.IsNullOrEmpty(refreshToken))
        {
            _authService.RevokeRefreshToken(refreshToken);
        }
        _authService.ClearCookies();

        return Ok(ApiResponse<object>.SuccessResponse(null, "Logged out successfully."));
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("remember-me")]
    public IHttpActionResult RememberMe()
    {
        bool remember = _authService.IsRememberMe();

        return Ok(ApiResponse<bool>.SuccessResponse(remember));
    }
    [AllowAnonymous]
    [HttpPost]
    [Route("validate-token")]
    public IHttpActionResult ValidateToken()
    {
        var result = _adminService.ValidateToken();
        return Content((HttpStatusCode)result.StatusCode, result);
    }

    [HttpGet]
    [Route("get-users")]
    public async Task<IHttpActionResult> GetUsers(int pageNumber = 1, int pageSize = 10, string search = "")
    {
        int userid = _authService.GetCurrentUserId();
        var result = await _adminService.GetUsers(pageNumber, pageSize, search);
        return Content((HttpStatusCode)result.StatusCode, result);
    }

    [HttpGet]
    [Route("users")]
    public async Task<IHttpActionResult> GetUsersservices(int pageNumber = 1, int pageSize = 10, string search = "")
    {
        
        var users = await _userService.GetUsersAsync(pageNumber, pageSize, search);
        return Ok(ApiResponse<object>.SuccessResponse(users, "Users fetched successfully."));
    }

    [HttpPost]
    [Route("set-content")]
    public async Task<IHttpActionResult> SaveContent(SaveContentModel model)
    {
        
        var result = await _adminService.SetContent(model);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-content/{id}")]
    public async Task<IHttpActionResult> GetContent(int id)
    {
        var result = await _adminService.GetContent(id);
        return Ok(result);
    }
    [HttpDelete]
    [Route("delete-content/{id}")]
    public async Task<IHttpActionResult> DeleteContent(int  id)
    {
        var result = await _adminService.ContentDelete(id);
        return Ok(result);
    }

    [HttpGet]
    [Route("get-all-content")]
    public async Task<IHttpActionResult> GetAllContent()
    {
        var result = await _adminService.GetAllContent();
        return Ok(result);
    }
}
