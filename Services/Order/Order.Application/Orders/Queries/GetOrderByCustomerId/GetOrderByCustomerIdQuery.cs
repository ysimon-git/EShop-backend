using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Application.Orders.Queries.GetOrderByCustomerId
{
    public sealed record GetOrderByCustomerIdQuery(Guid customerId);
    
    
}
