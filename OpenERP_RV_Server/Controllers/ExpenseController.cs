using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Filters;
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
    //[Authorize]
    [AutomaticExceptionHandler]
    public class ExpenseController : ControllerBase
    {

        [HttpPost]
        [Route("uploadCFDI")]
        //[SessionTokenManager]
        public IActionResult Post([FromForm] IFormFile xml)
        {
            for (int i = 0; i < 35000; i++)
            {
                new ExpenseService().AddExpenseFromCFDI(xml);
            }
            
            return Ok(null);
        }

        [HttpGet]
        [Route("uploadCFDI")]
        //[SessionTokenManager]
        public IActionResult Get()
        {
            new ExpenseService().GetAllExpenses();
            return Ok();
            
        }


    }
}
