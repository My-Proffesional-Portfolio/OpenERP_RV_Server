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
        //https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AspNetCoreGuidance.md#do-not-store-ihttpcontextaccessorhttpcontext-in-a-field
        //This example stores the IHttpContextAccesor itself in a field and uses the HttpContext field at the correct time(checking for null).
        public ExpenseController(IHttpContextAccessor accessor)
        {
            BaseService.accessor = accessor;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadCFDI")]
        [SessionTokenManager]
        //[Consumes("multipart/form-data")]
        public IActionResult Post([FromForm] List<IFormFile> files)
        {
            var filesReq = Request.Form.Files.ToList();
            return Ok(new ExpenseService().ProcessFiles(filesReq));
        }


        [HttpPost]
        [SessionTokenManager]
        [Route("addExpense")]
        //[Consumes("multipart/form-data")]
        public IActionResult Post([FromBody] NewExpenseModel newExpense)
        {
            return Ok(new ExpenseService().AddExpense(newExpense));
        }



        [HttpGet]
        [SessionTokenManager]
        public IActionResult Get(int currentPage = 0, int pageSize = 10,
            string searchTerm = "", DateTime? emissionStartDate = null, DateTime? emissionEndDate = null,
             DateTime? creationStartDate = null, DateTime? creationEndDate = null)
        {

            return Ok(new ExpenseService().GetAllExpenses(currentPage, pageSize, searchTerm, emissionStartDate, emissionEndDate));

        }


        [HttpGet]
        [SessionTokenManager]
        [Route("GetAllExpenseItems")]
        public IActionResult GetAllExpenseItems()
        {

            return Ok(new ExpenseService().GetAllExpenseItems());

        }
        

        [HttpGet]
        [SessionTokenManager]
        [Route("detail")]
        public IActionResult Get(Guid id)
        {
            return Ok(new ExpenseService().GetExpenseById(id));
        }


        [HttpGet]
        [SessionTokenManager]
        [Route("report")]
        public IActionResult report(string searchTerm = "", DateTime? emissionStartDate = null, DateTime? emissionEndDate = null)
        {

            return Ok(new ReportService().GetExpenseBySuppliersAmount(searchTerm, emissionStartDate, emissionEndDate));

        }



        [HttpDelete]
        [SessionTokenManager]
        public IActionResult Delete(Guid id)
        {
            return Ok(new ExpenseService().DeleteExpenseByID(id));
        }

        [HttpDelete]
        [SessionTokenManager]
        [Route("deleteAll")]
        public IActionResult Delete()
        {
            return Ok(new ExpenseService().DeleteAllExpenses());
        }


        [HttpPut]
        [SessionTokenManager]
        [Route("UpdateFullFilledItem")]
        //[Consumes("multipart/form-data")]
        public IActionResult Post(bool statusUpdate, Guid Uuid)
        {
            return Ok(new ExpenseService().UpdateExpenseItemStatus(statusUpdate, Uuid));
        }

    }
}
