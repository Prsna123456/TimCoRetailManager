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
  public class UserData : IUserData
  {
    //private readonly IConfiguration _config;
    private readonly ISqlDataAccess _sqlDataAccess;

    public UserData(IConfiguration config, ISqlDataAccess sqlDataAccess)
    {
      //_config = config;
      _sqlDataAccess = sqlDataAccess;
    }
    public List<UserModel> GetUserById(string Id)
    {
      //SqlDataAccess sql = new SqlDataAccess(_config);

      var p = new { Id = Id };

      var output = _sqlDataAccess.LoadData<UserModel, dynamic>("[dbo].[spUserLookup]", p, "TRMData");

      return output;
    }
  }
}
