using MVCTemplete.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVCTemplete.Service.Interface
{
    public interface IUserServices
    {
        Task<DataTable> GetUsersAsync(int pageNumber, int pageSize, string search);

        Task<int> CreateUserAsync(UserDto model);

        Task<int> UpdateUserAsync(UserDto model);

        Task<int> DeleteUserAsync(int userId);

        Task<UserDto> GetUserByIdAsync(int userId);

    }
}