﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
  public class SalesViewModel : Screen
  {
    private BindingList<ProductModel> _products;
    private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();
    private int _itemquantity = 1;

    IConfigHelper _configHelper;
    IProductEndpoint _productEndpoint;

    public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper)
    {
      _productEndpoint = productEndpoint;
      _configHelper = configHelper;
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

    private ProductModel _selectedProduct;
    public ProductModel SelectedProduct
    {
      get { return _selectedProduct; }
      set
      {
        _selectedProduct = value;
        NotifyOfPropertyChange(() => SelectedProduct);
        NotifyOfPropertyChange(() => CanAddToCart);
      }
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


    public BindingList<CartItemModel> Cart
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
        NotifyOfPropertyChange(() => CanAddToCart);
      }
    }

    public string SubTotal
    {
      get
      {
        return CalculateSubTotal().ToString("C");
      }
    }

    private decimal CalculateSubTotal()
    {
      decimal subTotal = 0;

      foreach (var item in Cart)
      {
        subTotal += item.Product.RetailPrice * item.QuantityInCart;
      }
      return subTotal;
    }

    public string Total
    {
      get
      {
        decimal total = CalculateSubTotal() + CalculateTax();

        return total.ToString("C");
      }
    }

    public string Tax
    {
      get
      {
        return CalculateTax().ToString("C");
      }
    }

    private decimal CalculateTax()
    {
      decimal taxAmount = 0;
      decimal taxRate = _configHelper.GetTaxRate() / 100;

      foreach (var item in Cart)
      {
        if (item.Product.IsTaxable)
        {
          taxAmount += item.Product.RetailPrice * item.QuantityInCart * taxRate;
        }
      }
      return taxAmount;
    }


    public bool CanAddToCart
    {
      get
      {
        bool output = false;

        //Something is selected
        if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
        {
          output = true;
        }
        return output;
      }
    }

    public void AddToCart()
    {
      CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

      if (existingItem != null)
      {
        existingItem.QuantityInCart += ItemQuantity;
        //NotifyOfPropertyChange(() => existingItem);
        //var i = existingItem.DisplayText;
        Cart.Remove(existingItem);
        Cart.Add(existingItem);
      }
      else
      {
        CartItemModel item = new CartItemModel()
        {
          Product = SelectedProduct,
          QuantityInCart = ItemQuantity
        };
        Cart.Add(item);
      }

      SelectedProduct.QuantityInStock -= ItemQuantity;
      ItemQuantity = 1;
      NotifyOfPropertyChange(() => SubTotal);
      NotifyOfPropertyChange(() => Tax);
      NotifyOfPropertyChange(() => Total);
      //NotifyOfPropertyChange(() => Cart);
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
      NotifyOfPropertyChange(() => SubTotal);
      NotifyOfPropertyChange(() => Tax);
      NotifyOfPropertyChange(() => Total);
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