using Microsoft.AspNetCore.Mvc;
using OpenERP_RV_Server.Backend;
using OpenERP_RV_Server.Filters;
using OpenERP_RV_Server.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OpenERP_RV_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        [SessionTokenManager]
        public IActionResult Get(int currentPage = 0, int pageSize = 10)
        {
            return Ok(new ProductService().GetAllProducts(currentPage, pageSize));
        }

        // POST api/<ProductController>
        [HttpPost]
        [SessionTokenManager]
        public IActionResult Post([FromBody] ProductModel productModel)
        {
            return Ok(new ProductService().AddNewProduct(productModel));
        }

    }
}
