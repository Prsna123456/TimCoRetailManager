using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class SaleController : ControllerBase
  {
    private readonly IConfiguration _config;

    public SaleController(IConfiguration config)
    {
      _config = config;
    }

    // POST api/sale
    [Authorize(Roles = "Cashier")]
    public void Post(SaleModel sale)
    {
      SaleData data = new SaleData(_config);

      string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //RequestContext.Principal.Identity.GetUserId();

      data.SaveSale(sale, userId);
    }


    //GET api/sale/GetSalesReport
    [Authorize(Roles = "Admin,Manager")]
    [Route("GetSalesReport")]
    public List<SaleReportModel> GetSalesReport()
    {
      //if (RequestContext.Principal.IsInRole("Admin"))
      //{
      //  //Do admin stuff
      //}
      SaleData data = new SaleData(_config);

      return data.GetSaleReport();
    }
  }
}
