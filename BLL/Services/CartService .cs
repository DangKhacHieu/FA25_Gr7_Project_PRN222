using BLL.Common;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Cart?> GetCartAsync(int customerId)
            => await _cartRepository.GetCartByCustomerAsync(customerId);

        public async Task<OperationResult> AddToCartWithCheckAsync(int customerId, int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.IsDelete == 1)
                return OperationResult.Submit("❌ Sản phẩm không tồn tại hoặc đã bị xóa.", false);

            if (product.Quantity_Product <= 0)
                return OperationResult.Submit($"❌ Sản phẩm {product.ProductName} đã hết hàng.", false);

            var cart = await _cartRepository.GetCartByCustomerAsync(customerId);
            var existingItem = cart?.CartItems.FirstOrDefault(i => i.ProductId == productId);

            int totalQuantityAfterAdd = (existingItem?.Quantity ?? 0) + quantity;
            if (totalQuantityAfterAdd > product.Quantity_Product)
                return OperationResult.Submit($"⚠️ Sản phẩm {product.ProductName} chỉ còn {product.Quantity_Product} cái trong kho.", false);
            

            await _cartRepository.AddToCartAsync(customerId, productId, quantity);
            var checkProduct = _cartRepository.GetCartItemByIdAsync(productId);
            if (checkProduct != null)
                return OperationResult.Submit("✅ Đã cập nhật giỏ hàng thành công!", true);

            return OperationResult.Submit("✅ Đã thêm sản phẩm thành công!", true);
        }

        public async Task<OperationResult> UpdateCartItemWithCheckAsync(int cartItemId, int quantity)
        {
            if (quantity < 1)
                return OperationResult.Submit($"⚠️ Số lượng không hợp lệ.", false);

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null) return OperationResult.Submit("❌ Không tìm thấy sản phẩm trong giỏ hàng.", false);

            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null || product.IsDelete == 1)
                return OperationResult.Submit("❌ Sản phẩm này hiện không còn tồn tại.", false);

            if (quantity > product.Quantity_Product)
            return OperationResult.Submit($"⚠️ Số lượng tối đa có thể mua là {product.Quantity_Product}.", false);

            await _cartRepository.UpdateCartItemAsync(cartItemId, quantity);
            return OperationResult.Submit("✅ Cập nhật giỏ hàng thành công!", true);
        }

        public async Task RemoveCartItemAsync(int cartItemId)
            => await _cartRepository.RemoveCartItemAsync(cartItemId);
    }
}
