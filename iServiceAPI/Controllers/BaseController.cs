using Microsoft.AspNetCore.Mvc;

namespace iServiceAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected string GetJwtToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
    }
}
