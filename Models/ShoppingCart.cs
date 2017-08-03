using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BethanysPieShop.Models
{
    public class ShoppingCart
    {
        private readonly AppDbContext _appDbContext;
        private ShoppingCart(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public string ShoppingCartId { get; set; }

        //grabs all shopping cart items from current user based on ^ shopping cart id
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }



        public static ShoppingCart GetCart(IServiceProvider services)
        {
            //grabs current session 
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext.Session;

            //get access to our appdb context
            var context = services.GetService<AppDbContext>();

            //checks if there is already a car id session if not, create a new GUID
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            //create a cartid for the session based on the guid we just created
            session.SetString("CartId", cartId);

            //return a new shopping cart with the appdbcontext along with a new shopping cart id
            //using object initialization syntax
            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Pie pie, int amount)
        {
            var shoppingCartItem =
                    _appDbContext.ShoppingCartItems.SingleOrDefault(
                        s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

            //if the current shopping cart item does not exist
            //in our cart, create one with the specified amount
            //and then add it to our cart
            if (shoppingCartItem == null)
            {
               
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Pie = pie,
                    Amount = 1
                };

                _appDbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            //if it does exist, then increase the amount 
            else 
            {
                shoppingCartItem.Amount++;
            }
            _appDbContext.SaveChanges();
        }

        //remove from cart
        //pretty simple,
        //we grab the shopping cart item based on its properties
        
        public int RemoveFromCart(Pie pie)
        {
            var shoppingCartItem =
                    _appDbContext.ShoppingCartItems.SingleOrDefault(
                        s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;


            //only remove if the shopping cart item exists
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }

            _appDbContext.SaveChanges();

            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??
                   (ShoppingCartItems =
                       _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                           .Include(s => s.Pie)
                           .ToList());
        }

        public void ClearCart()
        {
            var cartItems = _appDbContext
                .ShoppingCartItems
                .Where(cart => cart.ShoppingCartId == ShoppingCartId);

            _appDbContext.ShoppingCartItems.RemoveRange(cartItems);

            _appDbContext.SaveChanges();
        }



        public decimal GetShoppingCartTotal()
        {
            var total = _appDbContext.ShoppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Pie.Price * c.Amount).Sum();
            return total;
        }
    }
}

