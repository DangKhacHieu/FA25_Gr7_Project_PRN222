using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly PhoneContext _context;
        public CartRepository(PhoneContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByCustomerAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task AddToCartAsync(int customerId, int productId, int quantity)
        {
            var cart = await GetCartByCustomerAsync(customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId, TotalPrice = 0, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity += quantity;
                item.SubTotal = item.Quantity * (product.Price ?? 0);
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    SubTotal = quantity * (product.Price ?? 0)
                });
            }

            cart.TotalPrice = cart.CartItems.Sum(i => i.SubTotal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var item = await _context.CartItems
                .Include(i => i.Product)
                .Include(i => i.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId);

            if (item != null && item.Product != null && item.Cart != null)
            {
                // Cập nhật lại số lượng và SubTotal của sản phẩm trong giỏ
                item.Quantity = quantity;
                item.SubTotal = quantity * (item.Product.Price ?? 0);

                // ✅ Cập nhật lại tổng tiền của giỏ hàng
                item.Cart.TotalPrice = item.Cart.CartItems.Sum(ci =>
                    (ci.CartItemId == item.CartItemId ? item.SubTotal : ci.SubTotal));

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
