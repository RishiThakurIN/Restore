using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    public class BasketController : BaseAPIController
    {
        private readonly StoreContext _Context;
        public BasketController(StoreContext context)
        {
            _Context = context;
        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetriveBasket();
            if (basket == null) return NotFound();
            return MapBasketToDto(basket);
        }



        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            var basket = await RetriveBasket();
            if (basket == null) basket = CreateBasket();

            var product = await _Context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            basket.AddItem(product, quantity);

            var result = await _Context.SaveChangesAsync() > 0;
            if (result) return MapBasketToDto(basket);
            return BadRequest(new ProblemDetails { Title = "Problem saving item into basket" });
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteBasket(int productid, int quantity)
        {
            var basket = await RetriveBasket();
            if (basket == null) return NotFound();
            basket.RemoveItem(productid, quantity);
            var result = await _Context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest(new ProblemDetails { Title = "Problem removing basket item" });

        }



        #region Private methods
        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOption = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Append("buyerId", buyerId, cookieOption);
            var basket = new Basket { BuyerId = buyerId };
            _Context.Baskets.Add(basket);
            return basket;
        }

        private async Task<Basket> RetriveBasket()
        {
            return await _Context.Baskets
            .Include(i => i.Item)
            .ThenInclude(j => j.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private static ActionResult<BasketDto> MapBasketToDto(Basket basket)
        {
            return new BasketDto
            {
                BasketId = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Item.Select(item => new BasketItemDto
                {
                    Brand = item.Product.Brand,
                    Name = item.Product.Name,
                    PictureUrl = item.Product.PictureUrl,
                    Price = item.Product.Price,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Type = item.Product.Type
                }).ToList()
            };
        }
        #endregion
    }
}