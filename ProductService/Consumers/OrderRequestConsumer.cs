using EventContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using EventContracts;

namespace ProductService.Consumers
{
    public class OrderRequestConsumer : IConsumer<IOrderCreated>
    {
        private readonly ILogger<OrderRequestConsumer> _logger;

        public OrderRequestConsumer(ILogger<OrderRequestConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderCreated> context)
        {
            var order = context.Message;
            _logger.LogInformation($"📦 Checking Stock for ProductId={order.ProductId}");

            // Simulating Product Availability Check
            bool isAvailable = order.ProductId % 2 == 0; // Even IDs available, Odd IDs out of stock

            if (isAvailable)
            {
                _logger.LogInformation($"✅ Stock Available for ProductId={order.ProductId}");
                await context.RespondAsync<IOrderResponse>(new
                {
                    OrderId = order.OrderId,
                    IsAvailable = true,
                    Message = "Stock Available"
                });
            }
            else
            {
                _logger.LogInformation($"❌ Out of Stock for ProductId={order.ProductId}");
                await context.RespondAsync<IOrderResponse>(new
                {
                    OrderId = order.OrderId,
                    IsAvailable = false,
                    Message = "Product Out of Stock"
                });
            }
        }
    }
}
