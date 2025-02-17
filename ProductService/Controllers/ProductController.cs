using EventContracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;

using ProductService.Models;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;
        private readonly IRequestClient<IGetOrdersRequest> _requestClient;

        public ProductController(ProductDbContext context, IRequestClient<IGetOrdersRequest> requestClient)
        {
            _context = context;
            _requestClient = requestClient;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (string.IsNullOrEmpty(product.Name) || product.Stock < 0 )
            {
                return BadRequest("Invalid product details.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok($"✅ Product '{product.Name}' added successfully!");
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductWithOrders(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound($"❌ Product with ID {productId} not found.");
            }

            // Request orders from OrderService via RabbitMQ
            var response = await _requestClient.GetResponse<IGetOrdersResponse>(new { ProductId = productId });

            return Ok(new
            {
                Product = product,
                Orders = response.Message.Orders
            });
        }
        }
}
