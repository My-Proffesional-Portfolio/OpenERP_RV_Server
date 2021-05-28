using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Backend
{
    public class ExpenseService : BaseService
    {

        public ExpenseModel AddExpenseFromCFDI(IFormFile xml)
        {
            var xmlString = UtilService.ReadFormFileAsync(xml);
            var cfdi = UtilService.Deserialize<Comprobante>(xmlString);



            var tfd = cfdi.Complemento.SelectMany(sm => sm.Any).Where(w => w.Name.Contains("tfd:TimbreFiscalDigital")).Select(s => s.Attributes);
            var uuid = tfd.ToList().FirstOrDefault().GetNamedItem("UUID").Value;
        }
    }
}
