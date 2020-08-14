using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
  public class SaleData : ISaleData
  {
    private readonly IConfiguration _config;
    private readonly IProductData _productData;
    private readonly ISqlDataAccess _sqlDataAccess;

    public SaleData(IConfiguration config, IProductData productData,
      ISqlDataAccess sqlDataAccess)
    {
      _config = config;
      _productData = productData;
      _sqlDataAccess = sqlDataAccess;
    }
    public void SaveSale(SaleModel saleInfo, string cashierId)
    {
      //TODO: Make this SOLID/DRY/

      List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
      //ProductData products = new ProductData(_config);
      var configHelper = new ConfigHelper(_config);

      var taxRate = configHelper.GetTaxRate() / 100;

      foreach (var item in saleInfo.SaleDetails)
      {
        var detail = new SaleDetailDBModel
        {
          ProductId = item.ProductId,
          Quantity = item.Quantity
        };

        var productInfo = _productData.GetProductById(detail.ProductId);

        if (productInfo == null)
        {
          throw new Exception($"The product Id of { detail.ProductId } could not be found in the database");
        }

        detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

        if (productInfo.IsTaxable)
        {
          detail.Tax = detail.PurchasePrice * taxRate;
        }

        details.Add(detail);
      }

      SaleDBModel sale = new SaleDBModel
      {
        SubTotal = details.Sum(x => x.PurchasePrice),
        Tax = details.Sum(x => x.Tax),
        CashierId = cashierId
      };

      sale.Total = sale.SubTotal + sale.Tax;

      //Save the sale model
      //using (SqlDataAccess sql = new SqlDataAccess(_config))
      //{
      try
      {
        _sqlDataAccess.StartTransaction("TRMData");

        //Save the sale model
        _sqlDataAccess.SaveDataInTransaction("dbo.spSale_Insert", sale);

        //Get the ID from the sale model
        sale.Id = _sqlDataAccess.LoadDataInTransaction<int, dynamic>("[spSale_Lookup]", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

        //Filling in the  sale detail models
        foreach (var item in details)
        {
          item.SaleId = sale.Id;

          _sqlDataAccess.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
        }

        _sqlDataAccess.Committransaction();
      }
      catch (Exception)
      {
        _sqlDataAccess.RollbackTransaction();
        throw;
      }
    }
    //}

    public List<SaleReportModel> GetSaleReport()
    {
      //SqlDataAccess sql = new SqlDataAccess(_config);

      var output = _sqlDataAccess.LoadData<SaleReportModel, dynamic>
        ("spSale_SaleReport", new { }, "TRMData");

      return output;
    }
  }
}
