using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.Client.Response
{
    public class ConfirmationResponseModel
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public Guid CorporateOfficeID { get; set; }
        public long Number { get;  set; }
    }
}
