using MVCTemplete.DAL.DTOs;
using MVCTemplete.Models;
using MVCTemplete.Models.DTOs;
using System.Data;
using System.Threading.Tasks;

public interface IAdminService
{
    ApiResponse<object> Login(LoginRequest request);
    ApiResponse<object> Refresh();
    ApiResponse<object> ValidateToken();
    Task<ApiResponse<object>> GetUsers(int pageNumber, int pageSize, string search);

    Task<ApiResponse<object>> SetContent(SaveContentModel model);

    Task<ApiResponse<object>> GetContent(int id);

    Task<ApiResponse<object>> GetAllContent();
    Task<ApiResponse<object>> ContentDelete(int id);
}