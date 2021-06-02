using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenERP_RV_Server.Models.PagedModels
{
    public class PagedListModel<T> : BaseResponse
    {
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
        public decimal? Total { get; set; }
        public decimal? Subtotal { get; set; }
    }
}
