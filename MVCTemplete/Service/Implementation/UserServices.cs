using MVCTemplete.DTO;
using MVCTemplete.Helpers;
using MVCTemplete.Models;
using MVCTemplete.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MVCTemplete.Service.Implementation
{
    public class UserServices:IUserServices
    {
        private readonly DBHelper _dbHelper;
        private readonly JwtAuthAppDbEntities _dbContext;
        public UserServices()
        {
            _dbHelper = new DBHelper();
            _dbContext = new JwtAuthAppDbEntities();
        }

        public async Task<PagedResult<UserResponseDto>> GetUsersAsync(int pageNumber, int pageSize, string search)
        {
            DataTable dt = await _dbHelper.GetDataTableAsync(
                "USP_GetUsers",
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Search",
                    string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search)
            );

            var users = dt.AsEnumerable().Select(row => new UserResponseDto
            {
                UserID = row.Field<int>("Id"),
                Name = row.Field<string>("Name"),
                UserName = row.Field<string>("UserName"),
                Email = row.Field<string>("Email"),
                MobileNo = row.Field<string>("MobileNo"),
                Role = row.Field<string>("RoleName"),
                IsActive = row.Field<bool>("IsActive"),
                CreatedDate = Configuration.ToDateTime(row.Field<DateTime>("CreatedAt"))
            }).ToList();

            int totalRecords = dt.Rows.Count == 0
                ? 0
                : Convert.ToInt32(dt.Rows[0]["TotalRecords"]);

            return new PagedResult<UserResponseDto>
            {
                Items = users,
                TotalRecords = totalRecords
            };
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            DataTable dt = await _dbHelper.GetDataTableAsync(
                "USP_GetUserById",
                new SqlParameter("@UserID", userId));

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new UserDto
            {
                UserID = Convert.ToInt32(row["UserID"]),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["Email"].ToString()
            };
        }

        public async Task<int> CreateUserAsync(UserDto model)
        {
            return await _dbHelper.ExecuteAsync(
                "USP_CreateUser",
                new SqlParameter("@FirstName", model.FirstName),
                new SqlParameter("@LastName", model.LastName),
                new SqlParameter("@Email", model.Email));
        }

        public async Task<int> UpdateUserAsync(UserDto model)
        {
            return await _dbHelper.ExecuteAsync(
                "USP_UpdateUser",
                new SqlParameter("@UserID", model.UserID),
                new SqlParameter("@FirstName", model.FirstName),
                new SqlParameter("@LastName", model.LastName),
                new SqlParameter("@Email", model.Email));
        }

        public async Task<int> DeleteUserAsync(int userId)
        {
            return await _dbHelper.ExecuteAsync(
                "USP_DeleteUser",
                new SqlParameter("@UserID", userId));
        }
    }
}