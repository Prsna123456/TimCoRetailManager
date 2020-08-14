using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
  [Authorize(Roles = "Cashier")]
  [Route("api/[controller]")]
  [ApiController]
  public class ProductController : ControllerBase
  {
    //private readonly IConfiguration _config;
    private readonly IProductData _productData;

    public ProductController(IConfiguration config, IProductData productData)
    {
      //_config = config;
      _productData = productData;
    }

    // GET api/product
    [HttpGet]
    public List<ProductModel> Get()
    {
      //ProductData data = new ProductData(_config);

      return _productData.GetProducts();
    }
  }
}
