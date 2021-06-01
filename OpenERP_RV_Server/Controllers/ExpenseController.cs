using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Filters;
using OpenERP_RV_Server.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AutomaticExceptionHandler]
    public class ExpenseController : ControllerBase
    {

        public ExpenseController(IHttpContextAccessor accessor)
        {
            BaseService.HttpContext = accessor.HttpContext;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadCFDI")]
        [SessionTokenManager]
        //[Consumes("multipart/form-data")]
        public IActionResult Post([FromForm] IFormFile file)
        {
            //var files = Request.Form.Files;
            //for (int i = 0; i < 25000; i++)
            //{
           
            //}
            return Ok(new ExpenseService().AddExpenseFromCFDI(file, true));
        }

        //[HttpGet]
        //[Route("uploadCFDI")]
        //[SessionTokenManager]
        //public IActionResult Get()
        //{
        //    new ExpenseService().GetAllExpenses();
        //    return Ok();

        //}


    }
}
