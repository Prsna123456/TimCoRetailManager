using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
  public class ProductData : IProductData
  {
    //private readonly IConfiguration _config;
    private readonly ISqlDataAccess _sqlDataAccess;

    public ProductData(IConfiguration config, ISqlDataAccess sqlDataAccess)
    {
      //_config = config;
      _sqlDataAccess = sqlDataAccess;
    }
    public List<ProductModel> GetProducts()
    {
      //SqlDataAccess sql = new SqlDataAccess(_config);

      var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("[dbo].[spProduct_GetAll]", new { }, "TRMData");

      return output;
    }

    public ProductModel GetProductById(int ProductId)
    {
      //SqlDataAccess sql = new SqlDataAccess(_config);

      var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("[dbo].[spProduct_GetById]", new { id = ProductId }, "TRMData").FirstOrDefault();

      return output;
    }
  }
}
