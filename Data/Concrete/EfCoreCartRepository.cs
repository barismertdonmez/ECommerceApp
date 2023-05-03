using Data.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Concrete
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart, ShopContext>, ICartRepository
    {
        public void DeleteFromCart(int cartId, int productId)
        {
            using (var contex = new ShopContext())
            {
                var cmd = @"delete from CartItems where CartId=@p0 and ProductId=@p1";
                contex.Database.ExecuteSqlRaw(cmd, cartId, productId);
            }
        }

        public Cart GetByUserId(string userId)
        {
            using (var context = new ShopContext())
            {
                return context.Carts
                              .Include(p => p.CartItems)
                              .ThenInclude(p => p.Product)
                              .FirstOrDefault(p => p.UserId == userId);
            }
        }

        public override void Update(Cart entity)
        {
            using (var context = new ShopContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
        }
    }
}
