using BLL.Common;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(int customerId);
        Task<OperationResult> AddToCartWithCheckAsync(int customerId, int productId, int quantity);
        Task<OperationResult> UpdateCartItemWithCheckAsync(int cartItemId, int quantity);
        Task RemoveCartItemAsync(int cartItemId);
    }
}
