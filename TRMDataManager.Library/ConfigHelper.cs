using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library
{
  public class ConfigHelper
  {
    private readonly IConfiguration _config;

    //TODO: Move this from config to API
    public ConfigHelper(IConfiguration config)
    {
      _config = config;
    }

    public string GetConnectionString(string name)
    {
      return _config.GetConnectionString(name);
      //return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
    public static decimal GetTaxRate()
    {
      string rateText = ConfigurationManager.AppSettings["taxRate"];

      bool IsValidRate = Decimal.TryParse(rateText, out decimal output);

      if (IsValidRate == false)
      {
        throw new ConfigurationErrorsException("The tax rate is not set up properly");
      }

      return output;
    }
  }
}
