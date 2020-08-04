using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
  public class SalesViewModel : Screen
  {
    private BindingList<ProductDisplayModel> _products;
    private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
    private int _itemquantity = 1;

    IConfigHelper _configHelper;
    IProductEndpoint _productEndpoint;
    ISaleEndpoint _saleEndpoint;
    IMapper _mapper;

    public SalesViewModel(IProductEndpoint productEndpoint, 
      IConfigHelper configHelper, ISaleEndpoint saleEndpoint, IMapper mapper)
    {
      _productEndpoint = productEndpoint;
      _configHelper = configHelper;
      _saleEndpoint = saleEndpoint;
      _mapper = mapper;
    }

    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      await LoadProducts();
    }

    private async Task ResetSalesViewModel()
    {
      Cart = new BindingList<CartItemDisplayModel>();
      await LoadProducts();

      NotifyOfPropertyChange(() => SubTotal);
      NotifyOfPropertyChange(() => Tax);
      NotifyOfPropertyChange(() => Total);
      NotifyOfPropertyChange(() => CanCheckOut);
    }

    private async Task LoadProducts()
    {
      var productList = await _productEndpoint.GetAll();
      var products = _mapper.Map<List<ProductDisplayModel>>(productList);
      Products = new BindingList<ProductDisplayModel>(products);
      //Products = new BindingList<ProductDisplayModel>(productList);
    }

    private ProductDisplayModel _selectedProduct;
    public ProductDisplayModel SelectedProduct
    {
      get { return _selectedProduct; }
      set
      {
        _selectedProduct = value;
        NotifyOfPropertyChange(() => SelectedProduct);
        NotifyOfPropertyChange(() => CanAddToCart);
      }
    }

    //SelectedCartItem
    private CartItemDisplayModel _selectedCartItem;
    public CartItemDisplayModel SelectedCartItem
    {
      get { return _selectedCartItem; }
      set
      {
        _selectedCartItem = value;
        NotifyOfPropertyChange(() => SelectedCartItem);
        NotifyOfPropertyChange(() => CanRemoveFromCart);
      }
    }

    public BindingList<ProductDisplayModel> Products
    {
      get { return _products; }
      set
      {
        _products = value;
        NotifyOfPropertyChange(() => Products);
      }
    }


    public BindingList<CartItemDisplayModel> Cart
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

      taxAmount = Cart
        .Where(x => x.Product.IsTaxable)
        .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

      //foreach (var item in Cart)
      //{
      //  if (item.Product.IsTaxable)
      //  {
      //    taxAmount += item.Product.RetailPrice * item.QuantityInCart * taxRate;
      //  }
      //}
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
      CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

      if (existingItem != null)
      {
        existingItem.QuantityInCart += ItemQuantity;
        //NotifyOfPropertyChange(() => existingItem);
        //var i = existingItem.DisplayText;
        //Cart.Remove(existingItem);
        //Cart.Add(existingItem);
      }
      else
      {
        CartItemDisplayModel item = new CartItemDisplayModel()
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
      NotifyOfPropertyChange(() => CanCheckOut);
    }

    public bool CanRemoveFromCart
    {
      get
      {
        bool output = false;

        if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
        {
          output = true;
        }

        return output;
      }
    }

    public void RemoveFromCart()
    {

      SelectedCartItem.Product.QuantityInStock += 1;

      if (SelectedCartItem.QuantityInCart > 1)
      {
        SelectedCartItem.QuantityInCart -= 1;
      }
      else
      {
        Cart.Remove(SelectedCartItem);
      }

      NotifyOfPropertyChange(() => SubTotal);
      NotifyOfPropertyChange(() => Tax);
      NotifyOfPropertyChange(() => Total);
      NotifyOfPropertyChange(() => CanCheckOut);
      NotifyOfPropertyChange(() => CanAddToCart);
    }

    public bool CanCheckOut
    {
      get
      {
        bool output = false;

        //Something is selected
        if (Cart.Count > 0)
        {
          output = true;
        }

        return output;
      }
    }

    public async Task CheckOutAsync()
    {
      //Create a SaleModel and post to api
      SaleModel sale = new SaleModel();

      foreach (var item in Cart)
      {
        sale.SaleDetails.Add(new SaleDetailModel
        {
          ProductId = item.Product.Id,
          Quantity = item.QuantityInCart
        });
      }

      await _saleEndpoint.PostSale(sale);

      await ResetSalesViewModel();
    }
  }
}
