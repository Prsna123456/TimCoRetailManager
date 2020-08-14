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
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class InventoryController : ControllerBase
  {
    //private readonly IConfiguration _config;
    private readonly IInventoryData _inventoryData;

    public InventoryController(IConfiguration config, IInventoryData inventoryData)
    {
      //_config = config;
      _inventoryData = inventoryData;
    }

    // api/Inventory
    [Authorize(Roles = "Manager,Admin")]
    [HttpGet]
    public List<InventoryModel> Get()
    {
      //InventoryData data = new InventoryData(_config);
      //return data.GetInventory();
      return _inventoryData.GetInventory();
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public void Post(InventoryModel item)
    {
      //InventoryData data = new InventoryData(_config);
      //data.SaveInventoryRecord(item);
      _inventoryData.SaveInventoryRecord(item);
    }
  }
}
