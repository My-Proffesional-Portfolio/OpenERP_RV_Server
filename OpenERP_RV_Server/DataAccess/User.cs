using System;
using System.Collections.Generic;

#nullable disable

namespace OpenERP_RV_Server.DataAccess
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Salt { get; set; }
        public string HashedPassword { get; set; }
        public bool Status { get; set; }
        public Guid CompanyId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
