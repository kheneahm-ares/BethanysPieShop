using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethanysPieShop.Views.Shared.Components
{
    public class ShoppingCartSummary : ViewComponent
    {
        private ShoppingCart _shoppingCart;

        public ShoppingCartSummary(ShoppingCart cart)
        {
            _shoppingCart = cart;
        }

        //will be invoked automatically bc it is a view component
        public IViewComponentResult Invoke()
        {


            //creating mock up data to test view component for shopping cart summary
            //var items = new List<ShoppingCartItem>()
            //{
            //    new ShoppingCartItem(),
            //    new ShoppingCartItem()
            //};

            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;


            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(shoppingCartViewModel);
        }
    }
}
