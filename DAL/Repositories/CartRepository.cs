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
                .AsNoTracking()
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .AsNoTracking()
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
        }

        public async Task AddToCartAsync(int customerId, int productId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

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
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId);

            if (item != null && item.Product != null)
            {
                item.Quantity = quantity;
                item.SubTotal = quantity * (item.Product.Price ?? 0);

                await _context.SaveChangesAsync();

                var cart = await _context.Carts.FindAsync(item.CartId);
                if (cart != null)
                {
                    cart.TotalPrice = await _context.CartItems
                        .Where(ci => ci.CartId == item.CartId)
                        .SumAsync(ci => ci.SubTotal);

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                var cartId = item.CartId;

                _context.CartItems.Remove(item);

                await _context.SaveChangesAsync();

                var cart = await _context.Carts.FindAsync(cartId);
                if (cart != null)
                {
                    cart.TotalPrice = await _context.CartItems
                        .Where(ci => ci.CartId == cartId)
                        .SumAsync(ci => ci.SubTotal);

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}