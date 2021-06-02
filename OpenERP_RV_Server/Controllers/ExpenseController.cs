using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.DataAccess;
using OpenERP_RV_Server.Filters;
using OpenERP_RV_Server.Models;
using OpenERP_RV_Server.Models.Expense;
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
        public IActionResult Post([FromForm] List<IFormFile> files)
        {

            var filesReq = Request.Form.Files;
            var response = new List<ExpenseModel>();

            foreach (var f in filesReq)
            {
                response.Add(new ExpenseService().AddExpenseFromCFDI(f, true));
            }
            return Ok(response);

            //for (int i = 0; i < 250; i++)
            //{
            //    new expenseservice().addexpensefromcfdi(file, true);
            //}

        }

        [HttpGet]
        [SessionTokenManager]
        public IActionResult Get(int currentPage = 0, int pageSize = 10)
        {

            return Ok(new ExpenseService().GetAllExpenses(currentPage, pageSize));

        }


    }
}
