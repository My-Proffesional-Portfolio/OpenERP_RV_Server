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



        [HttpGet]
        [SessionTokenManager]
        public IActionResult Get(int currentPage = 0, int pageSize = 10,
            string searchTerm = "", DateTime? emissionStartDate = null, DateTime? emissionEndDate = null,
             DateTime? creationStartDate = null, DateTime? creationEndDate = null)
        {

            return Ok(new ExpenseService().GetAllExpenses(currentPage, pageSize, searchTerm));

        }

        [HttpGet]
        [SessionTokenManager]
        [Route("detail")]
        public IActionResult Get(Guid id)
        {
            return Ok(new ExpenseService().GetExpenseById(id));
        }



        [HttpDelete]
        [SessionTokenManager]
        public IActionResult Delete(Guid id)
        {
            return Ok(new ExpenseService().DeleteExpenseByID(id));
        }



    }
}
