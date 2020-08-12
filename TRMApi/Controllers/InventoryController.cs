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
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class InventoryController : ControllerBase
  {
    // api/Inventory
    [Authorize(Roles = "Manager,Admin")]
    public List<InventoryModel> Get()
    {
      InventoryData data = new InventoryData();

      return data.GetInventory();
    }


    [Authorize(Roles = "Admin")]
    public void Post(InventoryModel item)
    {
      InventoryData data = new InventoryData();

      data.SaveInventoryRecord(item);
    }
  }
}
