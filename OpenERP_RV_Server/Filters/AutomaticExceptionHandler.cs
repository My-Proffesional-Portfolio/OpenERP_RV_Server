using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenERP_RV_Server.ExceptionTypes;
using OpenERP_RV_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Filters
{
    public class AutomaticExceptionHandler :ExceptionFilterAttribute 
    {

        public override void OnException(ExceptionContext context)
        {

            var statusCode = HttpStatusCode.InternalServerError;
            if (context.Exception is FriendlyNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }

            if (context.Exception is FriendlyTransactionException)
            {
                statusCode = HttpStatusCode.Conflict;
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = new JsonResult(new BaseResponse
            {
                ErrorMessages = new List<string> { context.Exception.Message },
                AdditionalInformation = context.Exception.InnerException != null ? context.Exception.InnerException.Message : "",
            });


        }
    }
}
