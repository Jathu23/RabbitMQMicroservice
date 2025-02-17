using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventContracts
{
    public interface IGetOrdersRequest
    {
        int ProductId { get; }
    }

    public interface IGetOrdersResponse
    {
        int ProductId { get; }
        List<OrderDto> Orders { get; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        
    }
}
