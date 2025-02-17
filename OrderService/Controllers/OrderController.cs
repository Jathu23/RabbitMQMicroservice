using EventContracts;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<IOrderCreated> _requestClient;

        public OrderController(IPublishEndpoint publishEndpoint, IRequestClient<IOrderCreated> requestClient)
        {
            _publishEndpoint = publishEndpoint;
            _requestClient = requestClient;
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

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity)
        {
            var orderId = new Random().Next(1000, 9999); // Random Order ID

            // Send Order Request & Wait for Response
            var response = await _requestClient.GetResponse<IOrderResponse>(new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            });

            if (response.Message.IsAvailable)
            {
                return Ok($"✅ Order {orderId} Placed Successfully!");
            }
            else
            {
                return BadRequest($"❌ Order Failed: {response.Message.Message}");
            }
        }
    }
}
