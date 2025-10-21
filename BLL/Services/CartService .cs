using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart?> GetCartAsync(int customerId)
            => await _cartRepository.GetCartByCustomerAsync(customerId);

        public async Task AddToCartAsync(int customerId, int productId, int quantity)
            => await _cartRepository.AddToCartAsync(customerId, productId, quantity);

        public async Task UpdateCartItemAsync(int cartItemId, int quantity)
        {
            await _cartRepository.UpdateCartItemAsync(cartItemId, quantity);
        }

        public async Task RemoveCartItemAsync(int cartItemId)
            => await _cartRepository.RemoveCartItemAsync(cartItemId);
    }
}
