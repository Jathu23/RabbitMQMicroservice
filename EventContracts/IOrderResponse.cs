using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventContracts
{
    public interface IOrderResponse
    {
        int OrderId { get; }
        bool IsSucess { get; }
        string Message { get; }
    }
}
