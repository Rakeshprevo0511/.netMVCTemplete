using MVCTemplete.Helpers;
using MVCTemplete.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MVCTemplete.Filters
{
    public class JWTAuthorizeAttribute : AuthorizeAttribute
    {
        private string _message = "Unauthorized";
        private HttpStatusCode _statusCode = HttpStatusCode.Unauthorized;
        private List<string> _errors = new List<string>();

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["AccessToken"];

                if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
                {
                    _message = "Access Token Missing";
                    _errors.Add("Access token cookie was not found.");
                    return false;
                }

                string token = cookie.Value;
                var claims = JWTHelper.GetClaims(token);

                if (claims == null)
                {
                    _message = "Invalid Token";
                    _errors.Add("Access token is invalid.");
                    return false;
                }

                var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)), "JWT");
                var principal = new ClaimsPrincipal(identity);

                // Base AuthorizeAttribute exposes Roles/Users for exactly this purpose,
                // but since IsAuthorized is fully overridden here it was never actually
                // being enforced — e.g. [JWTAuthorize(Roles = "Admin")] would silently
                // let in anyone with a valid token, regardless of role.
                if (!string.IsNullOrWhiteSpace(Roles))
                {
                    var allowedRoles = Roles.Split(',')
                        .Select(r => r.Trim())
                        .Where(r => r.Length > 0);

                    if (!allowedRoles.Any(role => principal.IsInRole(role)))
                    {
                        _message = "Forbidden";
                        _statusCode = HttpStatusCode.Forbidden;
                        _errors.Add("You do not have permission to access this resource.");
                        return false;
                    }
                }

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log full detail server-side only. Returning ex.Message to the client
                // can leak internals (types, stack/config info) to anyone probing the
                // endpoint with a malformed token.
                Trace.TraceError("JWTAuthorizeAttribute failed: " + ex);

                _message = "Authentication Failed";
                _errors.Add("Unable to authenticate the request.");
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var response = ApiResponse<object>.FailureResponse(
                message: _message,
                errors: _errors,
                statusCode: (int)_statusCode);

            actionContext.Response = actionContext.Request.CreateResponse(
                _statusCode);

            actionContext.Response.Content = new ObjectContent(
                typeof(ApiResponse<object>),
                response,
                new JsonMediaTypeFormatter());
        }
    }
}
