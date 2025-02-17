using MassTransit;
using EventContracts;
namespace ProductService.Consumers
{
    public class OrderCreatedConsumer : IConsumer<IOrderCreated>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderCreated> context)
        {
            var order = context.Message;
           

            await Task.CompletedTask;
        }
    }
}
