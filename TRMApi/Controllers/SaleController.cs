using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class SaleController : ControllerBase
  {
    // POST api/sale
    [Authorize(Roles = "Cashier")]
    public void Post(SaleModel sale)
    {
      SaleData data = new SaleData();

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
      SaleData data = new SaleData();

      return data.GetSaleReport();
    }
  }
}
