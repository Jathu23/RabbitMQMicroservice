using EventContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using EventContracts;
using OrderService.Data;

namespace ProductService.Consumers
{
    public class OrderRequestConsumer : IConsumer<IOrderCreated>
    {
        private readonly ILogger<OrderRequestConsumer> _logger;
        private readonly ProductDbContext _dbContext;

        public OrderRequestConsumer(ILogger<OrderRequestConsumer> logger, ProductDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<IOrderCreated> context)
        {
            var order = context.Message;
            _logger.LogInformation($"📦 Checking Stock for ProductId={order.ProductId}");

            //Thread.Sleep(2000); 
             
            var product = await _dbContext.Products.FindAsync(order.ProductId);

            if (product!= null)
            {
                await context.RespondAsync<IOrderResponse>(new
                {

                    IsSucess = true,
                    Message = "Stock Available"
                });
            }
            else
            {
                _logger.LogInformation($"❌ Out of Stock for ProductId={order.ProductId}");
                await context.RespondAsync<IOrderResponse>(new
                {

                    IsSucess = false,
                    Message = "Product Out of Stock"
                });
            }
        }
    }
}
