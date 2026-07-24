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
        // NOTE: this attribute instance is created ONCE by Web API and cached/reused
        // for every request that hits the decorated action (and shared across
        // concurrent requests). It must never carry per-request mutable state on
        // instance fields — the old _message/_statusCode/_errors fields did exactly
        // that, so failures from one request leaked into the next (that's why the
        // Errors array kept accumulating duplicate entries) and it wasn't safe under
        // concurrent load either. Per-request state below is stashed on the
        // request's own Properties bag instead, which is unique per HttpRequestMessage.
        private const string FailureKey = "JWTAuthorize.Failure";

        private class AuthFailure
        {
            public string Message = "Unauthorized";
            public HttpStatusCode StatusCode = HttpStatusCode.Unauthorized;
            public List<string> Errors = new List<string>();
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var failure = new AuthFailure();
            actionContext.Request.Properties[FailureKey] = failure;

            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["AccessToken"];

                if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
                {
                    failure.Message = "Access Token Missing";
                    failure.Errors.Add("Access token cookie was not found.");
                    return false;
                }

                string token = cookie.Value;
                var claims = JWTHelper.GetClaims(token);

                if (claims == null)
                {
                    failure.Message = "Invalid Token";
                    failure.Errors.Add("Access token is invalid.");
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
                        failure.Message = "Forbidden";
                        failure.StatusCode = HttpStatusCode.Forbidden;
                        failure.Errors.Add("You do not have permission to access this resource.");
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

                failure.Message = "Authentication Failed";
                failure.Errors.Add("Unable to authenticate the request.");
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            object failureObj;
            var failure = actionContext.Request.Properties.TryGetValue(FailureKey, out failureObj)
                ? (AuthFailure)failureObj
                : new AuthFailure();

            var response = ApiResponse<object>.FailureResponse(
                message: failure.Message,
                errors: failure.Errors,
                statusCode: (int)failure.StatusCode);

            actionContext.Response = actionContext.Request.CreateResponse(
                failure.StatusCode);

            actionContext.Response.Content = new ObjectContent(
                typeof(ApiResponse<object>),
                response,
                new JsonMediaTypeFormatter());
        }
    }
}
