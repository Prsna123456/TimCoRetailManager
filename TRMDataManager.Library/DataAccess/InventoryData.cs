﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
  public class InventoryData : IInventoryData
  {
    //private readonly IConfiguration _config;
    private readonly ISqlDataAccess _sqlDataAccess;

    public InventoryData(IConfiguration config, ISqlDataAccess sqlDataAccess)
    {
      //_config = config;
      _sqlDataAccess = sqlDataAccess;
    }
    public List<InventoryModel> GetInventory()
    {
      //SqlDataAccess sql = new SqlDataAccess(_config);

      var output = _sqlDataAccess.LoadData<InventoryModel, dynamic>
        ("spInventory_GetAll", new { }, "TRMData");

      return output;
    }

    public void SaveInventoryRecord(InventoryModel item)
    {
      //SqlDataAccess sql = new SqlDataAccess(_config);

      _sqlDataAccess.SaveData("spInventory_Insert", item, "TRMData");
    }
  }
}
