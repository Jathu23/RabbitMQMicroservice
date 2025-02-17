using EventContracts;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<IOrderCreated> _requestClient;
        private readonly OrderDbContext _context;

        public OrderController(IPublishEndpoint publishEndpoint, IRequestClient<IOrderCreated> requestClient, OrderDbContext orderDbContext)
        {
            _publishEndpoint = publishEndpoint;
            _requestClient = requestClient;
            _context = orderDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder1(int productId, int quantity)
        {
            var orderId = new Random().Next(1000, 9999); // Random Order ID

            // Send Order Created Event to RabbitMQ
            await _publishEndpoint.Publish<IOrderCreated>(new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            });

            return Ok($"Order {orderId} placed successfully!");
        }


        [HttpPost("placeOrder")]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity)
        {
            var order = new Order
            {
                ProductId = productId,
                Quantity = quantity,
                IsProcessed = false

            };

            // Send Order Request & Wait for Response
            var response = await _requestClient.GetResponse<IOrderResponse>(new
            {
                ProductId = productId,
                Quantity = quantity
            });

            if (response.Message.IsSucess)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok($"✅ Order {order.Id} placed successfully!");
            }


            return BadRequest($"❌ Order Failed: {response.Message.Message}");


        }
    }
}
