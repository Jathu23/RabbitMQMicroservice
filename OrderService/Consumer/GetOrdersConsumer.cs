using EventContracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;

namespace OrderService.Consumer
{
    public class GetOrdersConsumer : IConsumer<IGetOrdersRequest>
    {
        private readonly OrderDbContext _context;

        public GetOrdersConsumer(OrderDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<IGetOrdersRequest> context)
        {
            var orders = await _context.Orders
                .Where(o => o.ProductId == context.Message.ProductId)
                .Select(o => new OrderDto
                {
                    OrderId = o.Id,
                    Quantity = o.Quantity,
                   
                }).ToListAsync();


            await context.RespondAsync<IGetOrdersResponse>(new
            {
                ProductId = context.Message.ProductId,
                Orders = orders
            });
        }
    }
}
