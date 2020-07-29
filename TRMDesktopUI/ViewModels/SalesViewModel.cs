using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
  public class SalesViewModel : Screen
  {
    private BindingList<ProductModel> _products;
    private BindingList<ProductModel> _cart;
    private int _itemquantity;

    IProductEndpoint _productEndpoint;

    public SalesViewModel(IProductEndpoint productEndpoint)
    {
      _productEndpoint = productEndpoint;
    }

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      await LoadProducts();
    }

    private async Task LoadProducts()
    {
      var productList = await _productEndpoint.GetAll();
      Products = new BindingList<ProductModel>(productList);
    }

    public BindingList<ProductModel> Products
    {
      get { return _products; }
      set 
      {
        _products = value;
        NotifyOfPropertyChange(() => Products);
      }
    }


    public BindingList<ProductModel> Cart
    {
      get { return _cart; }
      set 
      {
        _cart = value;
        NotifyOfPropertyChange(() => Cart);
      }
    }
    

    public int ItemQuantity
    {
      get { return _itemquantity; }
      set 
      {
        _itemquantity = value;
        NotifyOfPropertyChange(() => ItemQuantity);
      }
    }

    public string SubTotal
    {
      get 
      { 
        //TODO - Replace with calculation
        return "$0.00";
      }
    }

    public string Total
    {
      get
      {
        //TODO - Replace with calculation
        return "$0.00";
      }
    }

    public string Tax
    {
      get
      {
        //TODO - Replace with calculation
        return "$0.00";
      }
    }


    public bool CanAddToCart
    {
      get
      {
        bool output = false;

        //Something is selected

        //if (UserName?.Length > 0 && Password?.Length > 0)
        //{
        //  output = true;
        //}
        return output;
      }
    }

    public void AddToCart()
    {

    }

    public bool CanRemoveFromCart
    {
      get
      {
        bool output = false;

        //Something is selected

        //if (UserName?.Length > 0 && Password?.Length > 0)
        //{
        //  output = true;
        //}
        return output;
      }
    }

    public void RemoveFromCart()
    {

    }

    public bool CanCheckOut
    {
      get
      {
        bool output = false;

        //Something is selected

        //if (UserName?.Length > 0 && Password?.Length > 0)
        //{
        //  output = true;
        //}
        return output;
      }
    }

    public void CheckOut()
    {

    }

  }
}
