using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCTemplete.Models.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public  bool RememberMe { get; set; }
    }
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }
}