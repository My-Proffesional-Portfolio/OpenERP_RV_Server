using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.CompanyOrganization
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            ErrorMessages = new List<string>();
        }
        public List<string> ErrorMessages { get; set; }
        public string AdditionalInformation { get; set; }
    }
}
