using MVCTemplete.Models;
using MVCTemplete.Service.Implementation;
using MVCTemplete.Service.Interface;
using System.Web.Http;

/// <summary>
/// Base for any Web API controller that needs authenticated-session context.
/// Every shared dependency is wired up exactly once, here. New controllers just
/// inherit from this and call AuthService / AdminService / UserService directly —
/// no constructor wiring, no DbContext handling, no Dispose override needed.
/// </summary>
public abstract class BaseApiController : ApiController
{
    protected readonly JwtAuthAppDbEntities _dbContext;
    protected readonly IRefreshTokenRepository RefreshTokenRepository;
    protected readonly IAuthService _authService;
    protected readonly IAdminService _adminService;
    protected readonly IUserServices _userService;

    protected BaseApiController()
    {
        _dbContext = new JwtAuthAppDbEntities();
        RefreshTokenRepository = new RefreshTokenRepository(_dbContext);
        _authService = new AuthService(RefreshTokenRepository);
        _adminService = new AdminService(_authService, RefreshTokenRepository);
        _userService = new UserServices();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dbContext?.Dispose();
        }

        base.Dispose(disposing);
    }
}