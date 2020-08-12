using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
  [Authorize(Roles = "Cashier")]
  [Route("api/[controller]")]
  [ApiController]
  public class ProductController : ControllerBase
  {
    // GET api/product
    public List<ProductModel> Get()
    {
      ProductData data = new ProductData();

      return data.GetProducts();
    }
  }
}
