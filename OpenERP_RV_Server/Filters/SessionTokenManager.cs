using Microsoft.AspNetCore.Mvc.Filters;
using OpenERP_RV_Server.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Filters
{
    public class SessionTokenManager : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            new UserService().MockUserSessionByToken(context);
        }
    }
}
