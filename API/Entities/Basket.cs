using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }

        public List<BasketItem> Item { get; set; } = new();

        public void AddItem(Product product, int Quantity)
        {
            if (Item.All(item => item.ProductId != product.Id))
            {
                Item.Add(new BasketItem { Product = product, Quantity = Quantity });
                return;
            }

            var existingItem = Item.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null) existingItem.Quantity += Quantity;
        }

        public void RemoveItem(int productId, int Quanity)
        {
            var baseketItem = Item.FirstOrDefault(x => x.ProductId == productId);
            if (baseketItem == null) return;

            baseketItem.Quantity -= Quanity;
            if (baseketItem.Quantity == 0) Item.Remove(baseketItem);


        }
    }
}